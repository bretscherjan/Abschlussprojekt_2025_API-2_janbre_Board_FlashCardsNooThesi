<?php
/**
 * auth.php
 *
 * Handles authentication and session management.
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */
require_once 'config.php';

header('Content-Type: application/json');
$baseCode = '4gdrsh92z7';

/**
 * Initializes session based on provided sessionID
 * @param string|null $sessionID
 */
function initializeSession($sessionID) {
    if ($sessionID) {
        session_id($sessionID);
    }
    session_start();
}

/**
 * Generates a new session token
 * @return array Token and session ID
 */
function getToken() {
    global $fh;
    session_regenerate_id(true);
    $_SESSION['token'] = generateRandomString(10);
    $response = [
        'token' => $_SESSION['token'],
        'sessionID' => session_id()
    ];
    session_write_close();
    return $response;
}

/**
 * Validates authentication
 * @param string $clientToken Client-provided token
 * @param string $user Username
 * @param string $password User password
 * @return bool True if valid, false otherwise
 */
function validateAuth($clientToken, $user, $password) {
    global $fh, $baseCode;
    if (!isset($_SESSION['token']) || !isset($_SESSION['user']) || $_SESSION['user'] !== $user) {
        fwrite($fh, date(DATE_RFC2822) . " : Ungültige Session oder Benutzer : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
        return false;
    }
    $fullToken = $_SESSION['token'] . $baseCode . $password;
    $serverHash = hash('sha256', $fullToken);
    if ($clientToken !== $serverHash) {
        fwrite($fh, date(DATE_RFC2822) . " : Authentifizierung fehlgeschlagen : " . $_SERVER['HTTP_CLIENT_IP'] . "\n");
        return false;
    }
    return true;
}
?>