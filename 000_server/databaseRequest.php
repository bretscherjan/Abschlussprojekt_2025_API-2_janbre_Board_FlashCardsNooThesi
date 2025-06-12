<?php

$mysql_host = 'herkules.net.letsbuild.ch:3306';
$mysql_user = 'jan';
$mysql_password = 'VEezZ85d';
$mysql_database = 'FlashCards';

$fh = fopen('./log.txt', 'a');

$action = $_GET['action'] ?? '';
$requestMessage = $_GET['request'] ?? null;
$user = $_GET['user'] ?? null;
$clientToken = $_GET['token'] ?? null;
$sessionID = $_GET['sessionID'] ?? null;
$deckId = $_GET['deckId'] ?? null;

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
        $erfolg = fwrite($fh, date(DATE_RFC2822) . " : Fehlende Parameter : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
        echo json_encode(['error' => 'Fehlende Parameter']);
        exit;
    }
    
    // Prüfen ob Session existiert und gültig ist
    if (!isset($_SESSION['token']) || !isset($_SESSION['user'])) {
        $erfolg = fwrite($fh, date(DATE_RFC2822) . " : Ungültige Session : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
        echo json_encode(['error' => 'Ungültige Session']);
        exit;
    }

    // Prüfen ob der Benutzer übereinstimmt
    if ($_SESSION['user'] !== $user) {
        $erfolg = fwrite($fh, date(DATE_RFC2822) . " : Benutzer stimmt nicht überein : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
        echo json_encode(['error' => 'Benutzer stimmt nicht überein']);
        exit;
    }



    // Passwort aus Datenbank holen
    $password = getUserPassword($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user);
    
    if (is_array($password) && isset($password['error'])) {
        $erfolg = fwrite($fh, date(DATE_RFC2822) . " : " . $password['error'] . " : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
        echo json_encode($password);
        exit;
    }

    // Server-seitigen Hash generieren
    $fullToken = $_SESSION['token'] . $baseCode . $password;
    $serverHash = hash('sha256', $fullToken);
    
    // Client-Hash mit Server-Hash vergleichen
    if ($clientToken !== $serverHash) {
        session_destroy();
        $erfolg = fwrite($fh, date(DATE_RFC2822) . " : Authentifizierung fehlgeschlagen : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
        echo json_encode(['error' => 'Authentifizierung fehlgeschlagen']);
        
        exit;
    }


    switch ($requestMessage) {
        case 'getDecks':
            $result = getDecks($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user);
            break;
            
        case 'getCards':
            $result = getCards($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $deckId);
            break;

        case 'deleteDeck':
            $result = deleteDeck($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $deckId);
            break;
        
        default:
            $erfolg = fwrite($fh, date(DATE_RFC2822) . " : Unbekannte Anfrage : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
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
$erfolg = fwrite($fh, date(DATE_RFC2822) . " : Ungültige Aktion : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
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
        $erfolg = fwrite($fh, date(DATE_RFC2822) . " : Keine Verbindung zu mysql\n");
        return ['error' => 'Keine Verbindung zu mysql'];
    }

    if (!mysqli_select_db($dbh, $mysql_database)) {
        mysqli_close($dbh);
        $erfolg = fwrite($fh, date(DATE_RFC2822) . " : Konnte die Datenbank nicht auswählen\n");
        return ['error' => 'Konnte die Datenbank nicht auswählen'];
    }

    return $dbh;
}




//////////////////////////////////////////////////////////////////////
/// Function get User Password
//////////////////////////////////////////////////////////////////////

function getUserPassword($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, fopen('./log.txt', 'a'));
    

    $sql = "SELECT password FROM users WHERE userName = ?";
    $stmt = mysqli_prepare($dbh, $sql);
    
    if (!$stmt) {
        mysqli_close($dbh);
        $erfolg = fwrite($fh, date(DATE_RFC2822) . " : SQL Prepare fehlgeschlagen\n");
        return ['error' => 'SQL Prepare fehlgeschlagen'];
    }
    
    mysqli_stmt_bind_param($stmt, "s", $user);
    mysqli_stmt_execute($stmt);
    $result = mysqli_stmt_get_result($stmt);
    
    if (!$result || mysqli_num_rows($result) == 0) {
        mysqli_stmt_close($stmt);
        mysqli_close($dbh);
        $erfolg = fwrite($fh, date(DATE_RFC2822) . " : Benutzer nicht gefunden\n");
        return ['error' => 'Benutzer nicht gefunden'];
    }
    
    $row = mysqli_fetch_assoc($result);
    $password = $row['password'];
    
    mysqli_stmt_close($stmt);
    mysqli_close($dbh);

    return $password;
}




//////////////////////////////////////////////////////////////////////
/// function getDecks
//////////////////////////////////////////////////////////////////////

function getDecks($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, fopen('./log.txt', 'a'));

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
/// function getCards
//////////////////////////////////////////////////////////////////////

function getCards($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $deckId) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, fopen('./log.txt', 'a'));
    


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
/// function deleteDeck
//////////////////////////////////////////////////////////////////////

function deleteDeck($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $deckId) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, fopen('./log.txt', 'a'));



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