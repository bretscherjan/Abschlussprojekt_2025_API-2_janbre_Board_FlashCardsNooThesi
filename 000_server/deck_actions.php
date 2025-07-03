<?php
/**
 * deck_actions.php
 *
 * Handles deck-related operations (get, add, delete).
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */
require_once 'config.php';

/**
 * Retrieves decks for a user
 * @param string $user Username
 * @return array Decks or error
 */
function getDecks($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);
    if (is_array($dbh)) {
        return $dbh;
    }

    $sql = "SELECT d.id, d.title, d.alt, d.is_private, d.created_at, dc.start_color, dc.end_color 
            FROM decks d
            LEFT JOIN deck_colors dc ON d.id = dc.deck_id
            WHERE d.creator_id = (SELECT id FROM users WHERE username = ?) 
               OR d.id IN (SELECT deck_id FROM collaborators WHERE user_id = (SELECT id FROM users WHERE username = ?))";
    $stmt = mysqli_prepare($dbh, $sql);
    if (!$stmt) {
        mysqli_close($dbh);
        return ['error' => 'SQL Prepare fehlgeschlagen'];
    }

    mysqli_stmt_bind_param($stmt, "ss", $user, $user);
    mysqli_stmt_execute($stmt);
    $result = mysqli_stmt_get_result($stmt);
    if (!$result) {
        mysqli_stmt_close($stmt);
        mysqli_close($dbh);
        return ['error' => 'Fehler bei der Abfrage'];
    }

    $decks = [];
    while ($row = mysqli_fetch_assoc($result)) {
        $decks[] = $row;
    }

    mysqli_stmt_close($stmt);
    mysqli_close($dbh);
    return $decks;
}

/**
 * Adds a new deck
 * @param string $user Username
 * @param string $startColor Start color
 * @param string $endColor End color
 * @param string $title Deck title
 * @param string $alt Alternate title
 * @param string|null $collaborator Collaborator username
 * @return array Result of operation
 */
function addDeck($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $startColor, $endColor, $title, $alt, $collaborator, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);
    if (is_array($dbh)) {
        return $dbh;
    }

    $error = null;
    $deckId = null;

    $sql1 = "INSERT INTO decks (title, alt, is_private, creator_id) VALUES (?, ?, FALSE, (SELECT id FROM users WHERE username = ?))";
    $stmt1 = mysqli_prepare($dbh, $sql1);
    if ($stmt1 && mysqli_stmt_bind_param($stmt1, "sss", $title, $alt, $user) && mysqli_stmt_execute($stmt1)) {
        $deckId = mysqli_insert_id($dbh);
        mysqli_stmt_close($stmt1);

        $sql2 = "INSERT INTO deck_colors (deck_id, start_color, end_color) VALUES (?, ?, ?)";
        $stmt2 = mysqli_prepare($dbh, $sql2);
        if ($stmt2 && mysqli_stmt_bind_param($stmt2, "iss", $deckId, $startColor, $endColor) && mysqli_stmt_execute($stmt2)) {
            mysqli_stmt_close($stmt2);

            if ($collaborator) {
                $sql3 = "INSERT INTO collaborators (deck_id, user_id) VALUES (?, (SELECT id FROM users WHERE username = ?))";
                $stmt3 = mysqli_prepare($dbh, $sql3);
                if ($stmt3 && mysqli_stmt_bind_param($stmt3, "is", $deckId, $collaborator) && mysqli_stmt_execute($stmt3)) {
                    mysqli_stmt_close($stmt3);
                } else {
                    $error = 'Fehler beim Hinzufügen von Mitarbeitern: ' . ($stmt3 ? mysqli_stmt_error($stmt3) : mysqli_error($dbh));
                }
            }
        } else {
            $error = 'Fehler beim Hinzufügen der Deck-Farben: ' . ($stmt2 ? mysqli_stmt_error($stmt2) : mysqli_error($dbh));
        }
    } else {
        $error = 'Fehler beim Erstellen des Decks: ' . ($stmt1 ? mysqli_stmt_error($stmt1) : mysqli_error($dbh));
    }

    mysqli_close($dbh);
    return $error ? ['error' => $error] : ['success' => true, 'deckId' => $deckId];
}

/**
 * Deletes a deck
 * @param string $user Username
 * @param int $deckId Deck ID
 * @return array Result of operation
 */
function deleteDeck($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $deckId, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);
    if (is_array($dbh)) {
        return $dbh;
    }

    mysqli_begin_transaction($dbh);
    try {
        $queries = [
            "DELETE FROM collaborators WHERE deck_id = ?",
            "DELETE FROM deck_colors WHERE deck_id = ?",
            "DELETE FROM cards WHERE deck_id = ?",
            "DELETE FROM quiz WHERE deck_id = ?",
            "DELETE FROM quiz_options WHERE quiz_id IN (SELECT id FROM quiz WHERE deck_id = ?)",
            "DELETE FROM decks WHERE id = ?"
        ];

        foreach ($queries as $sql) {
            $stmt = mysqli_prepare($dbh, $sql);
            if (!$stmt) {
                throw new Exception('SQL Prepare fehlgeschlagen: ' . mysqli_error($dbh));
            }
            mysqli_stmt_bind_param($stmt, "i", $deckId);
            if (!mysqli_stmt_execute($stmt)) {
                throw new Exception('SQL Ausführung fehlgeschlagen: ' . mysqli_stmt_error($stmt));
            }
            mysqli_stmt_close($stmt);
        }

        mysqli_commit($dbh);
        mysqli_close($dbh);
        return ['success' => true];
    } catch (Exception $e) {
        mysqli_rollback($dbh);
        mysqli_close($dbh);
        return ['error' => $e->getMessage()];
    }
}
?>