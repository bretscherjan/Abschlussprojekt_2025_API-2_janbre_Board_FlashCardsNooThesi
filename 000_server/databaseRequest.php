<?php
// Logfile-Path:/var/www/owncloud/data/jan/files/jan-bretscher/01_zli/FlashCards/log.txt

$mysql_host = 'herkules.net.letsbuild.ch:3306';
$mysql_user = 'jan';
$mysql_password = 'VEezZ85d';
$mysql_database = 'FlashCards';

// Globalen Filehandler erstellen
$fh = fopen('./log.txt', 'a');

$action = $_GET['action'] ?? '';
$requestMessage = $_GET['request'] ?? null;
$user = $_GET['user'] ?? null;
$clientToken = $_GET['token'] ?? null;
$sessionID = $_GET['sessionID'] ?? null;
$deckId = $_GET['deckId'] ?? null;

$startColor = $_GET['startColor'] ?? '#ffffff';
$endColor = $_GET['endColor'] ?? '#000000';
$title = $_GET['title'] ?? null;
$alt = $_GET['alt'] ?? $title;

$baseCode = '4gdrsh92z7';
header('Content-Type: application/json');

// Session nur starten wenn SessionID übergeben wurde
if ($sessionID) {
    session_id($sessionID);
}
session_start();

/////////////////////////////////////////////////////////////////
/// action === getToken
/////////////////////////////////////////////////////////////////
if ($action === 'getToken') {
    
    // Neue Session starten
    session_regenerate_id(true);
    
    $_SESSION['token'] = generateRandomString(10);
    
    echo json_encode([
        'token' => $_SESSION['token'],
        'sessionID' => session_id()
    ]);
    exit;
}

/////////////////////////////////////////////////////////////////
/// action === getData
/////////////////////////////////////////////////////////////////
if ($action === 'getData') {
    $_SESSION['user'] = $user;

    // Prüfen ob alle erforderlichen Parameter vorhanden sind
    if (empty($user) || empty($clientToken) || empty($sessionID)) {
        fwrite($fh, date(DATE_RFC2822) . " : Fehlende Parameter : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
        echo json_encode(['error' => 'Fehlende Parameter']);
        exit;
    }
    
    // Prüfen ob Session existiert und gültig ist
    if (!isset($_SESSION['token']) || !isset($_SESSION['user'])) {
        fwrite($fh, date(DATE_RFC2822) . " : Ungültige Session : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
        echo json_encode(['error' => 'Ungültige Session']);
        exit;
    }

    // Prüfen ob der Benutzer übereinstimmt
    if ($_SESSION['user'] !== $user) {
        fwrite($fh, date(DATE_RFC2822) . " : Benutzer stimmt nicht überein : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
        echo json_encode(['error' => 'Benutzer stimmt nicht überein']);
        exit;
    }

    // Passwort aus Datenbank holen
    $password = getUserPassword($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $fh);
    
    if (is_array($password) && isset($password['error'])) {
        fwrite($fh, date(DATE_RFC2822) . " : " . $password['error'] . " : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
        echo json_encode($password);
        exit;
    }

    // Server-seitigen Hash generieren
    $fullToken = $_SESSION['token'] . $baseCode . $password;
    $serverHash = hash('sha256', $fullToken);
    
    // Client-Hash mit Server-Hash vergleichen
    if ($clientToken !== $serverHash) {
        session_destroy();
        fwrite($fh, date(DATE_RFC2822) . " : Authentifizierung fehlgeschlagen : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
        echo json_encode(['error' => 'Authentifizierung fehlgeschlagen']);
        exit;
    }

    switch ($requestMessage) {
        case 'getDecks':
            $result = getDecks($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $fh);
            break;
            
        case 'getCards':
            $result = getCards($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $deckId, $fh);
            break;

        case 'deleteDeck':
            $result = deleteDeck($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $deckId, $fh);
            break;

        case 'addDeck':
            $startColor = '#' . $startColor;
            $endColor = '#' . $endColor;
            $result = addDeck($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $startColor, $endColor, $title, $alt, $fh);
            break;
        
        case 'addCards':
            $result = addCards($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $deckId, $requestMessage, $fh);
            break;

        case 'importCards':
            $result = addCards($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $deckId, $requestMessage, $fh);
            break;

        default:
            fwrite($fh, date(DATE_RFC2822) . " : Unbekannte Anfrage : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
            echo json_encode(['error' => 'Unbekannte Anfrage']);
            exit;
    }

    // debugging informationen momentan nicht aktiv
    $data = array(
        "result" => $result ?? [],
        "ServerHash" => $serverHash,
        "clientToken" => $clientToken,
        "fullToken" => $fullToken,
        "status" => "success",
        "user" => $_SESSION['user'],
        "request" => $requestMessage,
        "message" => "Authentifizierung erfolgreich"
        // Hier würdest du die eigentlichen Ergebnisse der Datenbankabfrage hinzufügen
    );
    
    echo json_encode($result);
    
    // Session nach erfolgreicher Anfrage zerstören
    session_destroy();
    exit;
}

// Falls keine passende Action gefunden wurde
fwrite($fh, date(DATE_RFC2822) . " : Ungültige Aktion : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
echo json_encode(['error' => 'Ungültige Aktion']);
fclose($fh);
exit;

///////////////////////////////////////
// Function to generate a token
///////////////////////////////////////
function generateRandomString($length) {
    $token = '';
    $characters = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
    for ($i = 0; $i < $length; $i++) {
        $randomIndex = random_int(0, strlen($characters) - 1);
        $token .= $characters[$randomIndex];
    }
    return $token;
}

///////////////////////////////////////
// Function create Database Connection
///////////////////////////////////////
function createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh) {
    $dbh = mysqli_connect($mysql_host, $mysql_user, $mysql_password);
    if (!$dbh) {
        fwrite($fh, date(DATE_RFC2822) . " : Keine Verbindung zu mysql\n");
        return ['error' => 'Keine Verbindung zu mysql'];
    }

    if (!mysqli_select_db($dbh, $mysql_database)) {
        mysqli_close($dbh);
        fwrite($fh, date(DATE_RFC2822) . " : Konnte die Datenbank nicht auswählen\n");
        return ['error' => 'Konnte die Datenbank nicht auswählen'];
    }

    return $dbh;
}

//////////////////////////////////////////////////////////////////////
// Function get User Password
//////////////////////////////////////////////////////////////////////

function getUserPassword($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);
    

    $sql = "SELECT password FROM users WHERE userName = ?";
    $stmt = mysqli_prepare($dbh, $sql);
    
    if (!$stmt) {
        mysqli_close($dbh);
        fwrite($fh, date(DATE_RFC2822) . " : SQL Prepare fehlgeschlagen\n");
        return ['error' => 'SQL Prepare fehlgeschlagen'];
    }
    
    mysqli_stmt_bind_param($stmt, "s", $user);
    mysqli_stmt_execute($stmt);
    $result = mysqli_stmt_get_result($stmt);
    
    if (!$result || mysqli_num_rows($result) == 0) {
        mysqli_stmt_close($stmt);
        mysqli_close($dbh);
        fwrite($fh, date(DATE_RFC2822) . " : Benutzer nicht gefunden\n");
        return ['error' => 'Benutzer nicht gefunden'];
    }
    
    $row = mysqli_fetch_assoc($result);
    $password = $row['password'];
    
    mysqli_stmt_close($stmt);
    mysqli_close($dbh);

    return $password;
}

//////////////////////////////////////////////////////////////////////
// function getDecks
//////////////////////////////////////////////////////////////////////

function getDecks($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);

    $sql = "SELECT d.id, d.title, d.alt, d.is_private, d.created_at, dc.start_color, dc.end_color 
            FROM decks d
            LEFT JOIN deck_colors dc ON d.id = dc.deck_id
            WHERE d.creator_id = (
                SELECT u.id FROM users u WHERE u.username = ?) OR d.id IN (
                    SELECT deck_id FROM collaborators WHERE user_id = (
                        SELECT u.id FROM users u WHERE u.username = ?));
            ";
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

//////////////////////////////////////////////////////////////////////
// function getCards
//////////////////////////////////////////////////////////////////////

function getCards($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $deckId, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);
    

    $sql = "SELECT 
                'card' AS type,
                c.id,
                c.question,
                c.answer,
                NULL AS correct_answer,
                NULL AS first_option,
                NULL AS second_option,
                NULL AS third_option,
                NULL AS fourth_option,
                c.is_fav,
                c.status,
                c.created_at,
                d.title
            FROM cards c
            JOIN decks d ON c.deck_id = d.id
            WHERE c.deck_id = ?

            UNION ALL

            SELECT 
                'quiz' AS type,
                q.id,
                q.question,
                NULL AS answer,
                q.correct_answer,
                qo.first_option,
                qo.second_option,
                qo.third_option,
                qo.fourth_option,
                q.is_fav,
                q.status,
                q.created_at,
                d.title
            FROM quiz q
            JOIN quiz_options qo ON q.id = qo.quiz_id
            JOIN decks d ON q.deck_id = d.id
            WHERE q.deck_id = ?;";

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

//////////////////////////////////////////////////////////////////////
// function deleteDeck
//////////////////////////////////////////////////////////////////////

function deleteDeck($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $deckId, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);

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

//////////////////////////////////////////////////////////////////////
// function addDeck
//////////////////////////////////////////////////////////////////////

function addDeck($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $startColor, $endColor, $title, $alt, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);
    $error = null;
    $deckId = null;

    // Erste Transaktion: Deck erstellen
    $sql1 = "INSERT INTO decks (title, alt, is_private, creator_id) VALUES (?, ?, FALSE, (SELECT id FROM users WHERE username = ?))";
    $stmt1 = mysqli_prepare($dbh, $sql1);
    
    if ($stmt1 && mysqli_stmt_bind_param($stmt1, "sss", $title, $alt, $user) && mysqli_stmt_execute($stmt1)) {
        $deckId = mysqli_insert_id($dbh);
        mysqli_stmt_close($stmt1);
        
        // Zweite Transaktion: Farben hinzufügen
        $sql2 = "INSERT INTO deck_colors (deck_id, start_color, end_color) VALUES (?, ?, ?)";
        $stmt2 = mysqli_prepare($dbh, $sql2);
        
        if ($stmt2 && mysqli_stmt_bind_param($stmt2, "iss", $deckId, $startColor, $endColor) && mysqli_stmt_execute($stmt2)) {
            mysqli_stmt_close($stmt2);
        } else {
            $error = 'Fehler beim Hinzufügen der Deck-Farben: ' . ($stmt2 ? mysqli_stmt_error($stmt2) : mysqli_error($dbh));
        }
    } else {
        $error = 'Fehler beim Erstellen des Decks: ' . ($stmt1 ? mysqli_stmt_error($stmt1) : mysqli_error($dbh));
    }
    
    mysqli_close($dbh);
    
    return $error 
        ? ['error' => $error] 
        : ['success' => true, 'deckId' => $deckId];
}

//////////////////////////////////////////////////////////////////////
// function addCards
//////////////////////////////////////////////////////////////////////

function addCards($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $deckId, $requestMessage, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);
    $error = null;

    // Decode the JSON input
    $input = json_decode(file_get_contents('php://input'), true);
    if (!$input) {
        return ['error' => 'Invalid JSON input'];
    }

    // Validate deckId
    if (!is_numeric($deckId) || $deckId <= 0) {
        return ['error' => 'Ungültige deckId'];
    }

    mysqli_begin_transaction($dbh);

    try {

        if ( $requestMessage == 'addCards'){

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

        // Process normal cards
        if (isset($input['normalCards'])) {
            foreach ($input['normalCards'] as $card) {
                $question = $card['question'] ?? '';
                $answer = $card['answer'] ?? '';
                $isFav = $card['is_fav'] ? 1 : 0; // Convert boolean to integer
                $status = $card['status'] ?? 'needs_practice';

                // Validate inputs
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
                $isFav = $quizCard['is_fav'] ? 1 : 0; // Convert boolean to integer
                $status = $quizCard['status'] ?? 'needs_practice';
                $correctIndex = $quizCard['correctIndex'] ?? 1;

                // Validate inputs
                if (empty($question)) {
                    throw new Exception("Frage darf nicht leer sein");
                }

                // Insert into quiz table
                $sql = "INSERT INTO quiz (deck_id, question, is_fav, correct_answer, status) VALUES (?, ?, ?, ?, ?)";
                $stmt = mysqli_prepare($dbh, $sql);
                mysqli_stmt_bind_param($stmt, "issis", $deckId, $question, $isFav, $correctIndex, $status);
                mysqli_stmt_execute($stmt);
                $quizId = mysqli_insert_id($dbh);
                mysqli_stmt_close($stmt);

                // Insert options into quiz_options table
                $firstOption = $quizCard['option1'] ?? '';
                $secondOption = $quizCard['option2'] ?? '';
                $thirdOption = $quizCard['option3'] ?? '';
                $fourthOption = $quizCard['option4'] ?? '';

                // Validate options
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