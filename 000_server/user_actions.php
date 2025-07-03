<?php
/**
 * user_actions.php
 *
 * Handles user-related operations (create, update, delete, credentials).
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */
require_once 'config.php';

/**
 * Creates a new user
 * @param string $user Username
 * @param string $email Email
 * @param string $password Password
 * @return array Result of operation
 */
function createUser($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $email, $password, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);
    if (is_array($dbh)) {
        return $dbh;
    }

    $checkSql = "SELECT id FROM users WHERE username = ? OR email = ?";
    $checkStmt = mysqli_prepare($dbh, $checkSql);
    if (!$checkStmt) {
        mysqli_close($dbh);
        fwrite($fh, date(DATE_RFC2822) . " : SQL Prepare (Check) fehlgeschlagen\n");
        return ['error' => 'SQL Prepare fehlgeschlagen'];
    }

    mysqli_stmt_bind_param($checkStmt, "ss", $user, $email);
    mysqli_stmt_execute($checkStmt);
    $checkResult = mysqli_stmt_get_result($checkStmt);
    if (mysqli_num_rows($checkResult) > 0) {
        mysqli_stmt_close($checkStmt);
        mysqli_close($dbh);
        return ['error' => 'Benutzername oder E-Mail bereits vergeben'];
    }
    mysqli_stmt_close($checkStmt);

    $sql = "INSERT INTO users (username, email, password) VALUES (?, ?, ?)";
    $stmt = mysqli_prepare($dbh, $sql);
    if (!$stmt) {
        mysqli_close($dbh);
        fwrite($fh, date(DATE_RFC2822) . " : SQL Prepare fehlgeschlagen\n");
        return ['error' => 'SQL Prepare fehlgeschlagen'];
    }

    mysqli_stmt_bind_param($stmt, "sss", $user, $email, $password);
    if (!mysqli_stmt_execute($stmt)) {
        mysqli_stmt_close($stmt);
        mysqli_close($dbh);
        fwrite($fh, date(DATE_RFC2822) . " : Fehler beim Erstellen des Benutzers: " . mysqli_stmt_error($stmt) . "\n");
        return ['error' => 'Fehler beim Erstellen des Benutzers'];
    }

    $userId = mysqli_insert_id($dbh);
    mysqli_stmt_close($stmt);
    mysqli_close($dbh);
    return [
        'success' => true,
        'userId' => $userId,
        'message' => 'Benutzer erfolgreich erstellt'
    ];
}

/**
 * Updates an existing user
 * @param string $user New username
 * @param string $email New email
 * @param string $password New password
 * @param string $userOld Old username
 * @return array Result of operation
 */
function updateUser($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $email, $password, $userOld, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);
    if (is_array($dbh)) {
        return $dbh;
    }

    $sql = "UPDATE users SET username = ?, email = ?, password = ? WHERE username = ?";
    $stmt = mysqli_prepare($dbh, $sql);
    if (!$stmt) {
        mysqli_close($dbh);
        fwrite($fh, date(DATE_RFC2822) . " : SQL Prepare fehlgeschlagen\n");
        return ['error' => 'SQL Prepare fehlgeschlagen'];
    }

    mysqli_stmt_bind_param($stmt, "ssss", $user, $email, $password, $userOld);
    if (!mysqli_stmt_execute($stmt)) {
        mysqli_stmt_close($stmt);
        mysqli_close($dbh);
        fwrite($fh, date(DATE_RFC2822) . " : Fehler beim Update des Benutzers: " . mysqli_stmt_error($stmt) . "\n");
        return ['error' => 'Fehler beim Update des Benutzers'];
    }

    $userId = mysqli_insert_id($dbh);
    mysqli_stmt_close($stmt);
    mysqli_close($dbh);
    return [
        'success' => true,
        'userId' => $userId,
        'message' => 'Benutzer erfolgreich aktualisiert'
    ];
}

/**
 * Deletes a user
 * @param string $user Username
 * @return array Result of operation
 */
function deleteUser($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);
    if (is_array($dbh)) {
        return $dbh;
    }

    mysqli_begin_transaction($dbh);
    try {
        $sql = "DELETE FROM users WHERE username = ?";
        $stmt = mysqli_prepare($dbh, $sql);
        if (!$stmt) {
            throw new Exception("Prepare failed: " . mysqli_error($dbh));
        }

        mysqli_stmt_bind_param($stmt, "s", $user);
        if (!mysqli_stmt_execute($stmt)) {
            throw new Exception("Execute failed: " . mysqli_stmt_error($stmt));
        }

        $affectedRows = mysqli_stmt_affected_rows($stmt);
        mysqli_stmt_close($stmt);
        mysqli_commit($dbh);
        mysqli_close($dbh);
        return [
            'success' => true,
            'affected_rows' => $affectedRows
        ];
    } catch (Exception $e) {
        mysqli_rollback($dbh);
        mysqli_close($dbh);
        return ['error' => $e->getMessage()];
    }
}

/**
 * Retrieves user credentials
 * @param string $user Username
 * @return array User credentials or error
 */
function getUserCredentials($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);
    if (is_array($dbh)) {
        return $dbh;
    }

    $sql = "SELECT username, email, password FROM users WHERE username = ?";
    $stmt = mysqli_prepare($dbh, $sql);
    if (!$stmt) {
        mysqli_close($dbh);
        return ['error' => 'SQL Prepare fehlgeschlagen'];
    }

    mysqli_stmt_bind_param($stmt, "s", $user);
    mysqli_stmt_execute($stmt);
    $result = mysqli_stmt_get_result($stmt);
    if (!$result) {
        mysqli_stmt_close($stmt);
        mysqli_close($dbh);
        return ['error' => 'Fehler bei der Abfrage'];
    }

    $userData = [];
    while ($row = mysqli_fetch_assoc($result)) {
        $userData[] = $row;
    }

    mysqli_stmt_close($stmt);
    mysqli_close($dbh);
    return $userData;
}

/**
 * Retrieves user password
 * @param string $user Username
 * @return string|array Password or error
 */
function getUserPassword($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);
    if (is_array($dbh)) {
        return $dbh;
    }

    $sql = "SELECT password FROM users WHERE username = ?";
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
?>