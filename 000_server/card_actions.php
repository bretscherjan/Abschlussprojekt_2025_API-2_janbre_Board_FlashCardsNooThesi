<?php
/**
 * card_actions.php
 *
 * Handles card-related operations (get, add, update favorite).
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */
require_once 'config.php';

/**
 * Retrieves cards for a deck
 * @param string $user Username
 * @param int $deckId Deck ID
 * @return array Cards or error
 */
function getCards($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $deckId, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);
    if (is_array($dbh)) {
        return $dbh;
    }

    $sql = "SELECT 
                'card' AS type, c.id, c.question, c.answer, NULL AS correct_answer, NULL AS first_option, 
                NULL AS second_option, NULL AS third_option, NULL AS fourth_option, c.is_fav, c.status, c.created_at, d.title
            FROM cards c
            JOIN decks d ON c.deck_id = d.id
            WHERE c.deck_id = ?
            UNION ALL
            SELECT 
                'quiz' AS type, q.id, q.question, NULL AS answer, q.correct_answer, qo.first_option, 
                qo.second_option, qo.third_option, qo.fourth_option, q.is_fav, q.status, q.created_at, d.title
            FROM quiz q
            JOIN quiz_options qo ON q.id = qo.quiz_id
            JOIN decks d ON q.deck_id = d.id
            WHERE q.deck_id = ?";

    $stmt = mysqli_prepare($dbh, $sql);
    if (!$stmt) {
        mysqli_close($dbh);
        return ['error' => 'SQL Prepare fehlgeschlagen'];
    }

    mysqli_stmt_bind_param($stmt, "ii", $deckId, $deckId);
    mysqli_stmt_execute($stmt);
    $result = mysqli_stmt_get_result($stmt);
    if (!$result) {
        mysqli_stmt_close($stmt);
        mysqli_close($dbh);
        return ['error' => 'Fehler bei der Abfrage'];
    }

    $cards = [];
    while ($row = mysqli_fetch_assoc($result)) {
        $cards[] = $row;
    }

    mysqli_stmt_close($stmt);
    mysqli_close($dbh);
    return $cards;
}

/**
 * Adds or imports cards to a deck
 * @param string $user Username
 * @param int $deckId Deck ID
 * @param string $requestMessage Type of request (addCards, importCards)
 * @return array Result of operation
 */
function addCards($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $deckId, $requestMessage, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);
    if (is_array($dbh)) {
        return $dbh;
    }

    $input = json_decode(file_get_contents('php://input'), true);
    if (!$input) {
        return ['error' => 'Invalid JSON input'];
    }

    if (!is_numeric($deckId) || $deckId <= 0) {
        return ['error' => 'Ungültige deckId'];
    }

    mysqli_begin_transaction($dbh);
    try {
        if ($requestMessage === 'addCards') {
            $sql = "DELETE FROM cards WHERE deck_id = ?";
            $stmt = mysqli_prepare($dbh, $sql);
            mysqli_stmt_bind_param($stmt, "i", $deckId);
            mysqli_stmt_execute($stmt);
            mysqli_stmt_close($stmt);
            $sql = "DELETE FROM quiz WHERE deck_id = ?";
            $stmt = mysqli_prepare($dbh, $sql);
            mysqli_stmt_bind_param($stmt, "i", $deckId);
            mysqli_stmt_execute($stmt);
            mysqli_stmt_close($stmt);
        }

        if (isset($input['normalCards'])) {
            foreach ($input['normalCards'] as $card) {
                $question = $card['question'] ?? '';
                $answer = $card['answer'] ?? '';
                $isFav = $card['is_fav'] ? 1 : 0;
                $status = $card['status'] ?? 'needs_practice';

                if (empty($question) || empty($answer)) {
                    throw new Exception("Frage oder Antwort darf nicht leer sein");
                }

                $sql = "INSERT INTO cards (deck_id, question, answer, is_fav, status) VALUES (?, ?, ?, ?, ?)";
                $stmt = mysqli_prepare($dbh, $sql);
                mysqli_stmt_bind_param($stmt, "isssi", $deckId, $question, $answer, $isFav, $status);
                mysqli_stmt_execute($stmt);
                mysqli_stmt_close($stmt);
            }
        }

        if (isset($input['quizCards'])) {
            foreach ($input['quizCards'] as $quizCard) {
                $question = $quizCard['question'] ?? '';
                $isFav = $quizCard['is_fav'] ? 1 : 0;
                $status = $quizCard['status'] ?? 'needs_practice';
                $correctIndex = $quizCard['correctIndex'] ?? 1;

                if (empty($question)) {
                    throw new Exception("Frage darf nicht leer sein");
                }

                $sql = "INSERT INTO quiz (deck_id, question, is_fav, correct_answer, status) VALUES (?, ?, ?, ?, ?)";
                $stmt = mysqli_prepare($dbh, $sql);
                mysqli_stmt_bind_param($stmt, "issis", $deckId, $question, $isFav, $correctIndex, $status);
                mysqli_stmt_execute($stmt);
                $quizId = mysqli_insert_id($dbh);
                mysqli_stmt_close($stmt);

                $firstOption = $quizCard['option1'] ?? '';
                $secondOption = $quizCard['option2'] ?? '';
                $thirdOption = $quizCard['option3'] ?? '';
                $fourthOption = $quizCard['option4'] ?? '';

                if (empty($firstOption) || empty($secondOption) || empty($thirdOption) || empty($fourthOption)) {
                    throw new Exception("Alle Optionen müssen ausgefüllt sein");
                }
                if ($correctIndex < 1 || $correctIndex > 4) {
                    throw new Exception("correctIndex muss zwischen 1 und 4 liegen");
                }

                $sql = "INSERT INTO quiz_options (quiz_id, first_option, second_option, third_option, fourth_option) VALUES (?, ?, ?, ?, ?)";
                $stmt = mysqli_prepare($dbh, $sql);
                mysqli_stmt_bind_param($stmt, "issss", $quizId, $firstOption, $secondOption, $thirdOption, $fourthOption);
                mysqli_stmt_execute($stmt);
                mysqli_stmt_close($stmt);
            }
        }

        mysqli_commit($dbh);
        return ['success' => true];
    } catch (Exception $e) {
        mysqli_rollback($dbh);
        return ['error' => 'Fehler beim Hinzufügen der Karten: ' . $e->getMessage()];
    } finally {
        mysqli_close($dbh);
    }
}

/**
 * Updates favorite status of a card or quiz
 * @param string $user Username
 * @param int $deckId Deck ID
 * @param int $cardId Card/Quiz ID
 * @param bool $isFav Favorite status
 * @param string $type Card or quiz
 * @return array Result of operation
 */
function updateCardFavorite($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $deckId, $cardId, $isFav, $type, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);
    if (is_array($dbh)) {
        return $dbh;
    }

    $sql = ($type === 'card') ? "UPDATE cards SET is_fav = ? WHERE id = ?" : "UPDATE quiz SET is_fav = ? WHERE id = ?";
    $stmt = mysqli_prepare($dbh, $sql);
    if (!$stmt) {
        mysqli_close($dbh);
        fwrite($fh, date(DATE_RFC2822) . " : SQL Prepare fehlgeschlagen\n");
        return ['error' => 'SQL Prepare fehlgeschlagen'];
    }

    mysqli_stmt_bind_param($stmt, "ii", $isFav, $cardId);
    if (!mysqli_stmt_execute($stmt)) {
        mysqli_stmt_close($stmt);
        mysqli_close($dbh);
        fwrite($fh, date(DATE_RFC2822) . " : Fehler beim Aktualisieren des Favoritenstatus: " . mysqli_stmt_error($stmt) . "\n");
        return ['error' => 'Fehler beim Aktualisieren des Favoritenstatus'];
    }

    $affectedRows = mysqli_stmt_affected_rows($stmt);
    mysqli_stmt_close($stmt);
    mysqli_close($dbh);

    if ($affectedRows > 0) {
        fwrite($fh, date(DATE_RFC2822) . " : Successfully updated favorite status for $type ID $cardId to $isFav\n");
        return [
            'success' => true,
            'cardId' => $cardId,
            'isFav' => $isFav,
            'message' => 'Favorite status updated successfully'
        ];
    }
    return ['error' => 'No rows affected - item may not exist'];
}
?>