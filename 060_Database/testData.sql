INSERT INTO users (username, email, password) VALUES ('user1', 'user1@example.com', 'password1');
INSERT INTO users (username, email, password) VALUES ('user2', 'user2@example.com', 'password2');
INSERT INTO users (username, email, password) VALUES ('user3', 'user3@example.com', 'password3');
INSERT INTO users (username, email, password) VALUES ('user4', 'user4@example.com', 'password4');
INSERT INTO users (username, email, password) VALUES ('user5', 'user5@example.com', 'password5');
INSERT INTO users (username, email, password) VALUES ('user6', 'user6@example.com', 'password6');
INSERT INTO users (username, email, password) VALUES ('user7', 'user7@example.com', 'password7');

INSERT INTO follows (follower_id, followed_id) VALUES (1, 2);
INSERT INTO follows (follower_id, followed_id) VALUES (1, 3);
INSERT INTO follows (follower_id, followed_id) VALUES (2, 3);
INSERT INTO follows (follower_id, followed_id) VALUES (2, 4);
INSERT INTO follows (follower_id, followed_id) VALUES (3, 4);
INSERT INTO follows (follower_id, followed_id) VALUES (3, 5);
INSERT INTO follows (follower_id, followed_id) VALUES (4, 5);

INSERT INTO decks (title, alt, is_private, creator_id) VALUES ('Deck 1', 'Alt 1', TRUE, 1);
INSERT INTO decks (title, alt, is_private, creator_id) VALUES ('Deck 2', 'Alt 2', FALSE, 2);
INSERT INTO decks (title, alt, is_private, creator_id) VALUES ('Deck 3', 'Alt 3', TRUE, 3);
INSERT INTO decks (title, alt, is_private, creator_id) VALUES ('Deck 4', 'Alt 4', FALSE, 4);
INSERT INTO decks (title, alt, is_private, creator_id) VALUES ('Deck 5', 'Alt 5', TRUE, 5);
INSERT INTO decks (title, alt, is_private, creator_id) VALUES ('Deck 6', 'Alt 6', FALSE, 6);
INSERT INTO decks (title, alt, is_private, creator_id) VALUES ('Deck 7', 'Alt 7', TRUE, 7);

INSERT INTO collaborators (deck_id, user_id, can_edit) VALUES (1, 2, FALSE);
INSERT INTO collaborators (deck_id, user_id, can_edit) VALUES (1, 3, TRUE);
INSERT INTO collaborators (deck_id, user_id, can_edit) VALUES (2, 3, FALSE);
INSERT INTO collaborators (deck_id, user_id, can_edit) VALUES (2, 4, TRUE);
INSERT INTO collaborators (deck_id, user_id, can_edit) VALUES (3, 4, FALSE);
INSERT INTO collaborators (deck_id, user_id, can_edit) VALUES (3, 5, TRUE);
INSERT INTO collaborators (deck_id, user_id, can_edit) VALUES (4, 5, FALSE);

INSERT INTO deck_colors (deck_id, start_color, end_color) VALUES (1, '#FF5733', '#C70039');
INSERT INTO deck_colors (deck_id, start_color, end_color) VALUES (2, '#900C3F', '#581845');
INSERT INTO deck_colors (deck_id, start_color, end_color) VALUES (3, '#FFC300', '#FF5733');
INSERT INTO deck_colors (deck_id, start_color, end_color) VALUES (4, '#DAF7A6', '#FFC300');
INSERT INTO deck_colors (deck_id, start_color, end_color) VALUES (5, '#C70039', '#900C3F');
INSERT INTO deck_colors (deck_id, start_color, end_color) VALUES (6, '#581845', '#DAF7A6');
INSERT INTO deck_colors (deck_id, start_color, end_color) VALUES (7, '#FF5733', '#C70039');

INSERT INTO cards (deck_id, question, answer, is_fav, status) VALUES (1, 'Question 1', 'Answer 1', FALSE, 'needs_practice');
INSERT INTO cards (deck_id, question, answer, is_fav, status) VALUES (1, 'Question 2', 'Answer 2', TRUE, 'mastered');
INSERT INTO cards (deck_id, question, answer, is_fav, status) VALUES (2, 'Question 3', 'Answer 3', FALSE, 'needs_practice');
INSERT INTO cards (deck_id, question, answer, is_fav, status) VALUES (2, 'Question 4', 'Answer 4', TRUE, 'mastered');
INSERT INTO cards (deck_id, question, answer, is_fav, status) VALUES (3, 'Question 5', 'Answer 5', FALSE, 'needs_practice');
INSERT INTO cards (deck_id, question, answer, is_fav, status) VALUES (3, 'Question 6', 'Answer 6', TRUE, 'mastered');
INSERT INTO cards (deck_id, question, answer, is_fav, status) VALUES (4, 'Question 7', 'Answer 7', FALSE, 'needs_practice');

INSERT INTO quiz (deck_id, question, is_fav, correct_answer, status) VALUES (1, 'Quiz Question 1', FALSE, 1, 'needs_practice');
INSERT INTO quiz (deck_id, question, is_fav, correct_answer, status) VALUES (1, 'Quiz Question 2', TRUE, 2, 'mastered');
INSERT INTO quiz (deck_id, question, is_fav, correct_answer, status) VALUES (2, 'Quiz Question 3', FALSE, 3, 'needs_practice');
INSERT INTO quiz (deck_id, question, is_fav, correct_answer, status) VALUES (2, 'Quiz Question 4', TRUE, 4, 'mastered');
INSERT INTO quiz (deck_id, question, is_fav, correct_answer, status) VALUES (3, 'Quiz Question 5', FALSE, 1, 'needs_practice');
INSERT INTO quiz (deck_id, question, is_fav, correct_answer, status) VALUES (3, 'Quiz Question 6', TRUE, 2, 'mastered');
INSERT INTO quiz (deck_id, question, is_fav, correct_answer, status) VALUES (4, 'Quiz Question 7', FALSE, 3, 'needs_practice');

INSERT INTO quiz_options (quiz_id, first_option, second_option, third_option, fourth_option) VALUES (1, 'Option 1', 'Option 2', 'Option 3', 'Option 4');
INSERT INTO quiz_options (quiz_id, first_option, second_option, third_option, fourth_option) VALUES (2, 'Option 1', 'Option 2', 'Option 3', 'Option 4');
INSERT INTO quiz_options (quiz_id, first_option, second_option, third_option, fourth_option) VALUES (3, 'Option 1', 'Option 2', 'Option 3', 'Option 4');
INSERT INTO quiz_options (quiz_id, first_option, second_option, third_option, fourth_option) VALUES (4, 'Option 1', 'Option 2', 'Option 3', 'Option 4');
INSERT INTO quiz_options (quiz_id, first_option, second_option, third_option, fourth_option) VALUES (5, 'Option 1', 'Option 2', 'Option 3', 'Option 4');
INSERT INTO quiz_options (quiz_id, first_option, second_option, third_option, fourth_option) VALUES (6, 'Option 1', 'Option 2', 'Option 3', 'Option 4');
INSERT INTO quiz_options (quiz_id, first_option, second_option, third_option, fourth_option) VALUES (7, 'Option 1', 'Option 2', 'Option 3', 'Option 4');
