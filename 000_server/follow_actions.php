<?php
/**
 * follow_actions.php
 *
 * Handles follow and unfollow operations.
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */
require_once 'config.php';

/**
 * Adds a follow relationship
 * @param string $user Username
 * @param string $follow User to follow
 * @return array Result of operation
 */
function addFollow($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $follow, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);
    if (is_array($dbh)) {
        return $dbh;
    }

    if (empty($user) || empty($follow)) {
        mysqli_close($dbh);
        return ['error' => 'Benutzername oder Follower-ID fehlt'];
    }

    $sql = "INSERT INTO follows (follower_id, followed_id) VALUES ((SELECT id FROM users WHERE username = ?), (SELECT id FROM users WHERE username = ?))";
    $stmt = mysqli_prepare($dbh, $sql);
    if (!$stmt) {
        $error = 'Fehler beim Vorbereiten der Abfrage: ' . mysqli_error($dbh);
        mysqli_close($dbh);
        return ['error' => $error];
    }

    if (!mysqli_stmt_bind_param($stmt, "ss", $user, $follow) || !mysqli_stmt_execute($stmt)) {
        $error = 'Fehler beim Ausführen der Abfrage: ' . mysqli_stmt_error($stmt);
        mysqli_stmt_close($stmt);
        mysqli_close($dbh);
        return ['error' => $error];
    }

    $followId = mysqli_insert_id($dbh);
    mysqli_stmt_close($stmt);
    mysqli_close($dbh);
    return ['success' => true, 'followId' => $followId];
}

/**
 * Removes a follow relationship
 * @param string $user Username
 * @param string $follow User to unfollow
 * @return array Result of operation
 */
function unfollow($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $follow, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);
    if (is_array($dbh)) {
        return $dbh;
    }

    if (empty($user) || empty($follow)) {
        mysqli_close($dbh);
        return ['error' => 'Benutzername oder Follower-ID fehlt'];
    }

    $sql = "DELETE FROM follows WHERE follower_id = (SELECT id FROM users WHERE username = ?) AND followed_id = (SELECT id FROM users WHERE username = ?)";
    $stmt = mysqli_prepare($dbh, $sql);
    if (!$stmt) {
        $error = 'Fehler beim Vorbereiten der Abfrage: ' . mysqli_error($dbh);
        mysqli_close($dbh);
        return ['error' => $error];
    }

    if (!mysqli_stmt_bind_param($stmt, "ss", $user, $follow) || !mysqli_stmt_execute($stmt)) {
        $error = 'Fehler beim Ausführen der Abfrage: ' . mysqli_stmt_error($stmt);
        mysqli_stmt_close($stmt);
        mysqli_close($dbh);
        return ['error' => $error];
    }

    $affectedRows = mysqli_stmt_affected_rows($stmt);
    mysqli_stmt_close($stmt);
    mysqli_close($dbh);
    return ['success' => true, 'affected_rows' => $affectedRows];
}

/**
 * Retrieves users based on follow type
 * @param string $user Username
 * @param string $type Type of users to retrieve (followers, following, notFollowing)
 * @return array Users or error
 */
function getUsers($mysql_host, $mysql_user, $mysql_password, $mysql_database, $user, $type, $fh) {
    $dbh = createDatabaseConnection($mysql_host, $mysql_user, $mysql_password, $mysql_database, $fh);
    if (is_array($dbh)) {
        return $dbh;
    }

    if ($type === 'followers') {
        $sql = "SELECT u.username 
                FROM users u
                JOIN follows f ON u.id = f.follower_id
                WHERE f.followed_id = (SELECT id FROM users WHERE username = ?) AND username != ?";
    } elseif ($type === 'notFollowing') {
        $sql = "SELECT u.username 
                FROM users u
                LEFT JOIN follows f ON u.id = f.followed_id AND f.follower_id = (SELECT id FROM users WHERE username = ?)
                WHERE f.followed_id IS NULL 
                AND u.username != ?";
    } elseif ($type === 'following') {
        $sql = "SELECT u.username 
                FROM users u
                JOIN follows f ON u.id = f.followed_id
                WHERE f.follower_id = (SELECT id FROM users WHERE username = ?) AND username != ?";
    } else {
        mysqli_close($dbh);
        return ['error' => 'Ungültiger Typ'];
    }

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

    $users = [];
    while ($row = mysqli_fetch_assoc($result)) {
        $users[] = $row;
    }

    mysqli_stmt_close($stmt);
    mysqli_close($dbh);
    return $users;
}
?>