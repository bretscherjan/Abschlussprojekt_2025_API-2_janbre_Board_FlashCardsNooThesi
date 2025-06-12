-- Insert test users
INSERT INTO users (username, email, password) VALUES
('john_doe', 'john@example.com', 'password123'),
('jane_smith', 'jane@example.com', 'securepass456'),
('alice_wonder', 'alice@example.com', 'alice789'),
('bob_builder', 'bob@example.com', 'build2023'),
('emma_learn', 'emma@example.com', 'learn101');

-- Insert follows relationships
INSERT INTO follows (follower_id, followed_id) VALUES
(1, 2), -- John follows Jane
(1, 3), -- John follows Alice
(2, 1), -- Jane follows John
(3, 4), -- Alice follows Bob
(4, 1), -- Bob follows John
(5, 3); -- Emma follows Alice

-- Insert decks
INSERT INTO decks (title, alt, is_private, creator_id) VALUES
('Math Basics', 'Basic math concepts', TRUE, 1),
('History Trivia', 'Fun history facts', FALSE, 2),
('Science Quiz', 'General science questions', TRUE, 3),
('Literature', 'Classic books', FALSE, 4),
('Programming', 'Coding concepts', TRUE, 1),
('Geography', 'World geography', FALSE, 5);

-- Insert collaborators
INSERT INTO collaborators (deck_id, user_id, can_edit) VALUES
(1, 2, TRUE), -- Jane can edit John's Math Basics
(2, 3, FALSE), -- Alice can view Jane's History Trivia
(3, 1, TRUE), -- John can edit Alice's Science Quiz
(4, 5, FALSE), -- Emma can view Bob's Literature
(5, 3, TRUE); -- Alice can edit John's Programming

-- Insert deck colors
INSERT INTO deck_colors (deck_id, start_color, end_color) VALUES
(1, '#FF0000', '#FF9999'),
(2, '#00FF00', '#99FF99'),
(3, '#0000FF', '#9999FF'),
(4, '#FFFF00', '#FFFF99'),
(5, '#FF00FF', '#FF99FF'),
(6, '#00FFFF', '#99FFFF');

-- Insert cards
INSERT INTO cards (deck_id, question, answer, is_fav, status) VALUES
(1, 'What is 2 + 2?', '4', TRUE, 'mastered'),
(1, 'What is 5 * 3?', '15', FALSE, 'needs_practice'),
(2, 'Who was Cleopatra?', 'Queen of Egypt', FALSE, 'learning'),
(2, 'When was WW2?', '1939-1945', TRUE, 'mastered'),
(3, 'What is H2O?', 'Water', TRUE, 'mastered'),
(3, 'What is gravity?', 'Force of attraction', FALSE, 'needs_practice'),
(4, 'Who wrote 1984?', 'George Orwell', FALSE, 'learning'),
(5, 'What is a variable?', 'A named storage', TRUE, 'mastered'),
(5, 'What is a loop?', 'Repeated execution', FALSE, 'needs_practice'),
(6, 'Capital of France?', 'Paris', TRUE, 'mastered');

-- Insert quizzes
INSERT INTO quiz (deck_id, question, is_fav, correct_answer, status) VALUES
(1, 'What is 10 / 2?', FALSE, 2, 'needs_practice'),
(2, 'Who discovered America?', TRUE, 1, 'mastered'),
(3, 'What gas do plants need?', FALSE, 3, 'learning'),
(4, 'Author of Pride and Prejudice?', TRUE, 4, 'mastered'),
(5, 'What is an array?', FALSE, 2, 'needs_practice'),
(6, 'Largest continent?', TRUE, 1, 'mastered');

-- Insert quiz options
INSERT INTO quiz_options (quiz_id, first_option, second_option, third_option, fourth_option) VALUES
(1, '2', '5', '10', '20'),
(2, 'Columbus', 'Vespucci', 'Magellan', 'Drake'),
(3, 'Oxygen', 'Nitrogen', 'Carbon Dioxide', 'Helium'),
(4, 'Dickens', 'Bronte', 'Woolf', 'Austen'),
(5, 'Function', 'List of values', 'Class', 'Module'),
(6, 'Asia', 'Africa', 'Europe', 'Australia');