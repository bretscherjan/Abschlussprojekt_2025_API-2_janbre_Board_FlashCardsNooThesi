<?php
/**
 * config.php
 *
 * Stores database credentials and common utility functions.
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */

$mysql_host = 'herkules.net.letsbuild.ch:3306';
$mysql_user = 'jan';
$mysql_password = 'VEezZ85d';
$mysql_database = 'FlashCards';

// Global file handler for logging
$fh = fopen('./log.txt', 'a');

/**
 * Creates a database connection
 * @return mysqli|array Returns database handle or error array
 */
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

/**
 * Generates a random string for token
 * @param int $length Length of the string
 * @return string Random string
 */
function generateRandomString($length) {
    $token = '';
    $characters = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
    for ($i = 0; $i < $length; $i++) {
        $randomIndex = random_int(0, strlen($characters) - 1);
        $token .= $characters[$randomIndex];
    }
    return $token;
}
?>