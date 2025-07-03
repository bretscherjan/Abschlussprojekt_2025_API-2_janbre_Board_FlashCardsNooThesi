<?php
/**
 * api.php
 *
 * Main entry point for API requests, routes to appropriate handlers.
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */
require_once 'auth.php';
require_once 'user_actions.php';
require_once 'deck_actions.php';
require_once 'card_actions.php';
require_once 'follow_actions.php';
require_once 'config.php';

header('Content-Type: application/json');

// Read input data
$input = json_decode(file_get_contents('php://input'), true) ?? [];
$action = $input['action'] ?? $_GET['action'] ?? '';
$sessionID = $input['sessionID'] ?? $_GET['sessionID'] ?? null;
$clientToken = $input['token'] ?? $_GET['token'] ?? null;
$user = $input['user'] ?? $_GET['user'] ?? null;

initializeSession($sessionID);

if ($action === 'getToken') {
    echo json_encode(getToken());
    exit;
}

if (!$user || !$clientToken || !$sessionID) {
    fwrite($fh, date(DATE_RFC2822) . " : Fehlende Parameter : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
    echo json_encode(['error' => 'Fehlende Parameter']);
    session_destroy();
    fclose($fh);
    exit;
}

$_SESSION['user'] = $user;
$password = getUserPassword($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $fh);
if (is_array($password) && isset($password['error'])) {
    fwrite($fh, date(DATE_RFC2822) . " : " . $password['error'] . " : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
    echo json_encode($password);
    session_destroy();
    fclose($fh);
    exit;
}

if (!validateAuth($clientToken, $user, $password)) {
    echo json_encode(['error' => 'Authentifizierung fehlgeschlagen']);
    session_destroy();
    fclose($fh);
    exit;
}

switch ($action) {
    case 'createUser':
        $result = createUser($mysql_host, $mysql_user, $mysql_password, $mysql_database, 
            $input['username'], $input['email'], $input['password'], $fh);
        break;

    case 'updateUser':
        $result = updateUser($mysql_host, $mysql_user, $mysql_password, $mysql_database, 
            $input['username'], $input['email'], $input['password'], $input['OldUser'], $fh);
        break;

    case 'deleteUser':
        $result = deleteUser($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $fh);
        break;

    case 'getUserCredentials':
        $result = getUserCredentials($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $fh);
        break;

    case 'getDecks':
        $result = getDecks($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $fh);
        break;

    case 'addDeck':
        $startColor = $input['startColor'] ?? '#ffffff';
        $endColor = $input['endColor'] ?? '#000000';
        $title = $input['title'] ?? '';
        $alt = $input['alt'] ?? $title;
        $collaborator = $input['collaborator'] ?? null;
        $result = addDeck($mysql_host, $mysql_user, $mysql_password, $mysql_database, 
            $user, $startColor, $endColor, $title, $alt, $collaborator, $fh);
        break;

    case 'deleteDeck':
        $deckId = $input['deckId'] ?? null;
        $result = deleteDeck($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $deckId, $fh);
        break;

    case 'getCards':
        $deckId = $input['deckId'] ?? null;
        $result = getCards($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $deckId, $fh);
        break;

    case 'addCards':
    case 'importCards':
        $deckId = $input['deckId'] ?? null;
        $result = addCards($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $deckId, $action, $fh);
        break;

    case 'updateCardFavorite':
        $deckId = $input['deckId'] ?? null;
        $cardId = $input['cardId'] ?? null;
        $isFav = $input['isFav'] ?? null;
        $type = $input['type'] ?? null;
        $result = updateCardFavorite($mysql_host, $mysql_user, $mysql_password, $mysql_database, 
            $user, $deckId, $cardId, $isFav, $type, $fh);
        break;

    case 'getUsersFollowers':
        $result = getUsers($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, 'followers', $fh);
        break;

    case 'getUsersFollowing':
        $result = getUsers($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, 'following', $fh);
        break;

    case 'getUsersNotFollowing':
        $result = getUsers($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, 'notFollowing', $fh);
        break;

    case 'addFollow':
        $follow = $input['follow'] ?? null;
        $result = addFollow($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $follow, $fh);
        break;

    case 'unfollow':
        $follow = $input['follow'] ?? null;
        $result = unfollow($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $follow, $fh);
        break;

    case 'verifyAccount':
        $result = true;
        break;

    default:
        fwrite($fh, date(DATE_RFC2822) . " : Unbekannte Anfrage : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
        $result = ['error' => 'Unbekannte Anfrage'];
}

echo json_encode($result);
session_destroy();
fclose($fh);
exit;
?>