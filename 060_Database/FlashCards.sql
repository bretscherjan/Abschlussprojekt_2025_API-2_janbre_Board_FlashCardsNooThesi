-- --------------------------------------------------------
-- Host:                         herkules.net.letsbuild.ch
-- Server version:               8.0.42-0ubuntu0.22.04.1 - (Ubuntu)
-- Server OS:                    Linux
-- HeidiSQL Version:             12.8.0.6908
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumping database structure for FlashCards
CREATE DATABASE IF NOT EXISTS `FlashCards` /*!40100 DEFAULT CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `FlashCards`;

-- Dumping structure for table FlashCards.cards
CREATE TABLE IF NOT EXISTS `cards` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `deck_id` bigint unsigned NOT NULL,
  `question` text COLLATE utf8mb3_unicode_ci NOT NULL,
  `answer` text COLLATE utf8mb3_unicode_ci NOT NULL,
  `is_fav` tinyint(1) DEFAULT '0',
  `status` varchar(20) COLLATE utf8mb3_unicode_ci DEFAULT 'needs_practice',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_cards_deck_id` (`deck_id`),
  CONSTRAINT `fk_cards_deck_id` FOREIGN KEY (`deck_id`) REFERENCES `decks` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;

-- Dumping data for table FlashCards.cards: ~9 rows (approximately)
INSERT IGNORE INTO `cards` (`id`, `deck_id`, `question`, `answer`, `is_fav`, `status`, `created_at`) VALUES
	(1, 1, 'What is 2 + 2?', '4', 1, 'known', '2025-06-01 08:08:00'),
	(2, 1, 'What is 5 * 3?', '15', 0, 'needs_practice', '2025-06-01 08:09:00'),
	(3, 3, 'What is H2O?', 'Water', 0, 'known', '2025-06-01 08:10:00'),
	(4, 4, 'What is photosynthesis?', 'Process by which plants convert light into energy', 0, 'needs_practice', '2025-06-02 08:13:00'),
	(5, 4, 'What is DNA?', 'Deoxyribonucleic acid, genetic material', 1, 'known', '2025-06-02 08:14:00'),
	(6, 6, 'Who wrote "Pride and Prejudice"?', 'Jane Austen', 1, 'known', '2025-06-02 08:15:00'),
	(7, 6, 'What is the setting of "1984"?', 'Dystopian future', 0, 'needs_practice', '2025-06-02 08:16:00'),
	(8, 7, 'What is Newton’s First Law?', 'An object at rest stays at rest unless acted upon', 0, 'known', '2025-06-02 08:17:00'),
	(9, 8, 'What is the derivative of x^2?', '2x', 1, 'known', '2025-06-02 08:18:00');

-- Dumping structure for table FlashCards.collaborators
CREATE TABLE IF NOT EXISTS `collaborators` (
  `deck_id` bigint unsigned NOT NULL,
  `user_id` bigint unsigned NOT NULL,
  `can_edit` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`deck_id`,`user_id`),
  KEY `fk_user_id` (`user_id`),
  CONSTRAINT `fk_collaborators_deck_id` FOREIGN KEY (`deck_id`) REFERENCES `decks` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;

-- Dumping data for table FlashCards.collaborators: ~0 rows (approximately)

-- Dumping structure for table FlashCards.decks
CREATE TABLE IF NOT EXISTS `decks` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `title` varchar(255) COLLATE utf8mb3_unicode_ci NOT NULL,
  `alt` text COLLATE utf8mb3_unicode_ci,
  `card_type` tinyint NOT NULL,
  `is_private` tinyint(1) NOT NULL DEFAULT '1',
  `creator_id` bigint unsigned NOT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_creator_id` (`creator_id`),
  CONSTRAINT `fk_creator_id` FOREIGN KEY (`creator_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;

-- Dumping data for table FlashCards.decks: ~8 rows (approximately)
INSERT IGNORE INTO `decks` (`id`, `title`, `alt`, `card_type`, `is_private`, `creator_id`, `created_at`) VALUES
	(1, 'Math Basics', 'Basic math flashcards', 1, 1, 1, '2025-06-01 08:05:00'),
	(2, 'History Quiz', 'World history quiz', 2, 1, 2, '2025-06-01 08:06:00'),
	(3, 'Shared Science', 'Collaborative science deck', 1, 1, 3, '2025-06-01 08:07:00'),
	(4, 'Biology Flashcards', 'Basic biology concepts', 1, 1, 4, '2025-06-02 08:08:00'),
	(5, 'Geography Quiz', 'World geography questions', 2, 1, 5, '2025-06-02 08:09:00'),
	(6, 'Shared Literature', 'Collaborative literature study deck', 1, 1, 6, '2025-06-02 08:10:00'),
	(7, 'Physics Basics', 'Introduction to physics', 1, 1, 1, '2025-06-02 08:11:00'),
	(8, 'Math Advanced', 'Advanced math problems', 1, 1, 2, '2025-06-02 08:12:00');

-- Dumping structure for table FlashCards.deck_colors
CREATE TABLE IF NOT EXISTS `deck_colors` (
  `deck_id` bigint unsigned NOT NULL,
  `start_color` varchar(7) COLLATE utf8mb3_unicode_ci NOT NULL,
  `end_color` varchar(7) COLLATE utf8mb3_unicode_ci NOT NULL,
  PRIMARY KEY (`deck_id`),
  CONSTRAINT `fk_deck_colors_deck_id` FOREIGN KEY (`deck_id`) REFERENCES `decks` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;

-- Dumping data for table FlashCards.deck_colors: ~8 rows (approximately)
INSERT IGNORE INTO `deck_colors` (`deck_id`, `start_color`, `end_color`) VALUES
	(1, '#FF0000', '#FFFF00'),
	(2, '#00FF00', '#0000FF'),
	(3, '#FFFFFF', '#000000'),
	(4, '#00FFFF', '#FF00FF'),
	(5, '#FFA500', '#800080'),
	(6, '#A52A2A', '#FFD700'),
	(7, '#008000', '#FFC0CB'),
	(8, '#000080', '#00CED1');

-- Dumping structure for table FlashCards.follows
CREATE TABLE IF NOT EXISTS `follows` (
  `follower_id` bigint unsigned NOT NULL,
  `followed_id` bigint unsigned NOT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`follower_id`,`followed_id`),
  KEY `fk_followed_id` (`followed_id`),
  CONSTRAINT `fk_followed_id` FOREIGN KEY (`followed_id`) REFERENCES `users` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_follower_id` FOREIGN KEY (`follower_id`) REFERENCES `users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;

-- Dumping data for table FlashCards.follows: ~7 rows (approximately)
INSERT IGNORE INTO `follows` (`follower_id`, `followed_id`, `created_at`) VALUES
	(1, 2, '2025-06-01 08:03:00'),
	(1, 4, '2025-06-02 08:06:00'),
	(2, 3, '2025-06-01 08:04:00'),
	(2, 5, '2025-06-02 08:07:00'),
	(4, 1, '2025-06-02 08:03:00'),
	(5, 2, '2025-06-02 08:04:00'),
	(6, 3, '2025-06-02 08:05:00');

-- Dumping structure for table FlashCards.quiz
CREATE TABLE IF NOT EXISTS `quiz` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `deck_id` bigint unsigned NOT NULL,
  `question` text COLLATE utf8mb3_unicode_ci NOT NULL,
  `is_fav` tinyint(1) DEFAULT '0',
  `correct_answer` tinyint NOT NULL,
  `status` varchar(20) COLLATE utf8mb3_unicode_ci DEFAULT 'needs_practice',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  KEY `fk_quiz_deck_id` (`deck_id`),
  CONSTRAINT `fk_quiz_deck_id` FOREIGN KEY (`deck_id`) REFERENCES `decks` (`id`) ON DELETE CASCADE,
  CONSTRAINT `quiz_chk_1` CHECK ((`correct_answer` between 1 and 4))
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;

-- Dumping data for table FlashCards.quiz: ~4 rows (approximately)
INSERT IGNORE INTO `quiz` (`id`, `deck_id`, `question`, `is_fav`, `correct_answer`, `status`, `created_at`) VALUES
	(1, 2, 'Who was the first US president?', 0, 1, 'needs_practice', '2025-06-01 08:11:00'),
	(2, 5, 'What is the capital of Brazil?', 0, 2, 'needs_practice', '2025-06-02 08:19:00'),
	(3, 5, 'Which river is the longest?', 1, 1, 'known', '2025-06-02 08:20:00'),
	(4, 2, 'When did WWII end?', 0, 3, 'needs_practice', '2025-06-02 08:21:00');

-- Dumping structure for table FlashCards.quiz_options
CREATE TABLE IF NOT EXISTS `quiz_options` (
  `quiz_id` bigint unsigned NOT NULL,
  `first_option` text COLLATE utf8mb3_unicode_ci NOT NULL,
  `second_option` text COLLATE utf8mb3_unicode_ci NOT NULL,
  `third_option` text COLLATE utf8mb3_unicode_ci NOT NULL,
  `fourth_option` text COLLATE utf8mb3_unicode_ci NOT NULL,
  PRIMARY KEY (`quiz_id`),
  CONSTRAINT `fk_quiz_id` FOREIGN KEY (`quiz_id`) REFERENCES `quiz` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;

-- Dumping data for table FlashCards.quiz_options: ~4 rows (approximately)
INSERT IGNORE INTO `quiz_options` (`quiz_id`, `first_option`, `second_option`, `third_option`, `fourth_option`) VALUES
	(1, 'George Washington', 'Abraham Lincoln', 'Thomas Jefferson', 'John Adams'),
	(2, 'São Paulo', 'Brasília', 'Rio de Janeiro', 'Salvador'),
	(3, 'Nile', 'Amazon', 'Yangtze', 'Mississippi'),
	(4, '1943', '1944', '1945', '1946');

-- Dumping structure for table FlashCards.users
CREATE TABLE IF NOT EXISTS `users` (
  `id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `username` varchar(255) COLLATE utf8mb3_unicode_ci NOT NULL,
  `email` varchar(255) COLLATE utf8mb3_unicode_ci NOT NULL,
  `password` varchar(255) COLLATE utf8mb3_unicode_ci NOT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `username` (`username`),
  UNIQUE KEY `email` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;

-- Dumping data for table FlashCards.users: ~6 rows (approximately)
INSERT IGNORE INTO `users` (`id`, `username`, `email`, `password`, `created_at`) VALUES
	(1, 'alice', 'alice@example.com', 'hashed_password1', '2025-06-01 08:00:00'),
	(2, 'bob', 'bob@example.com', 'hashed_password2', '2025-06-01 08:01:00'),
	(3, 'charlie', 'charlie@example.com', 'hashed_password3', '2025-06-01 08:02:00'),
	(4, 'diana', 'diana@example.com', 'hashed_password4', '2025-06-02 08:00:00'),
	(5, 'emma', 'emma@example.com', 'hashed_password5', '2025-06-02 08:01:00'),
	(6, 'frank', 'frank@example.com', 'hashed_password6', '2025-06-02 08:02:00');

-- Dumping structure for view FlashCards.user_accessible_decks
-- Creating temporary table to overcome VIEW dependency errors
CREATE TABLE `user_accessible_decks` (
	`id` BIGINT UNSIGNED NOT NULL,
	`title` VARCHAR(1) NOT NULL COLLATE 'utf8mb3_unicode_ci',
	`alt` TEXT NULL COLLATE 'utf8mb3_unicode_ci',
	`card_type` TINYINT NOT NULL,
	`is_private` TINYINT(1) NOT NULL,
	`creator_id` BIGINT UNSIGNED NOT NULL,
	`created_at` TIMESTAMP NULL
) ENGINE=MyISAM;

-- Dumping structure for trigger FlashCards.trigger_update_deck_privacy
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `trigger_update_deck_privacy` AFTER INSERT ON `collaborators` FOR EACH ROW BEGIN
    UPDATE decks
    SET is_private = FALSE
    WHERE id = NEW.deck_id;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Removing temporary table and create final VIEW structure
DROP TABLE IF EXISTS `user_accessible_decks`;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `user_accessible_decks` AS select `d`.`id` AS `id`,`d`.`title` AS `title`,`d`.`alt` AS `alt`,`d`.`card_type` AS `card_type`,`d`.`is_private` AS `is_private`,`d`.`creator_id` AS `creator_id`,`d`.`created_at` AS `created_at` from `decks` `d` where ((`d`.`creator_id` = (select `users`.`id` from `users` where (`users`.`username` = 'alice'))) or `d`.`id` in (select `c`.`deck_id` from `collaborators` `c` where (`c`.`user_id` = (select `users`.`id` from `users` where (`users`.`username` = 'alice')))));

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
