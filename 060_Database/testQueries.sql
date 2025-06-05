-- Benutzer erstellen
INSERT INTO users (username, email, password) 
VALUES ('max_mustermann', 'max@example.com', 'secure123');

-- Benutzer Login
SELECT id, username FROM users 
WHERE email = 'max@example.com' AND password = 'secure123';

-- Benutzer aktualisieren
UPDATE users 
SET username = 'new_username', email = 'new@example.com' 
WHERE id = 1;

-- Benutzer löschen
DELETE FROM users WHERE id = 1;




-- Deck erstellen
INSERT INTO decks (title, alt, is_private, creator_id) 
VALUES ('Mathe Basics', 'Deck für Grundrechenarten', false, 1);

-- Farben hinzufügen
INSERT INTO deck_colors (deck_id, start_color, end_color)
VALUES (
  (SELECT id FROM decks ORDER BY id DESC LIMIT 1),
  '#FF0000',
  '#00FF00'
);

-- alle Decks eines Benutzers anzeigen
SELECT d.id, d.title, d.alt, dc.start_color, dc.end_color 
FROM decks d
LEFT JOIN deck_colors dc ON d.id = dc.deck_id
WHERE d.creator_id = 1 OR d.id IN (
    SELECT deck_id FROM collaborators WHERE user_id = 1
);

-- Deck aktualisieren
UPDATE decks 
SET title = 'Updated Title', is_private = true 
WHERE id = 1;

-- Deck löschen
DELETE FROM decks WHERE id = 1;



-- Karte hinzufügen
INSERT INTO cards (deck_id, question, answer, is_fav) 
VALUES ((SELECT id FROM decks ORDER BY id DESC LIMIT 1), 'Was ist 2+2?', '4', true);

-- Quiz hinzufügen
-- Quiz-Frage erstellen
INSERT INTO quiz (deck_id, question, correct_answer) 
VALUES ((SELECT id FROM decks ORDER BY id DESC LIMIT 1), 'Was ist die Hauptstadt von Deutschland?', 1);

-- Optionen hinzufügen
INSERT INTO quiz_options (quiz_id, first_option, second_option, third_option, fourth_option) 
VALUES ((SELECT id FROM quiz ORDER BY id DESC LIMIT 1), 'Berlin', 'München', 'Hamburg', 'Köln');

-- alle Karten eines Decks anzeigen
-- Einfache Karten
SELECT id, question, answer, is_fav FROM cards WHERE deck_id = 1;

-- Quizze
SELECT q.id, q.question, qo.first_option, qo.second_option, qo.third_option, qo.fourth_option, q.correct_answer 
FROM quiz q
JOIN quiz_options qo ON q.id = qo.quiz_id
WHERE q.deck_id = 1;

-- Favoriten markieren
UPDATE cards SET is_fav = true WHERE id = 1;

-- karte/quiz löschen
DELETE FROM cards WHERE id = 1;
-- Für Quizze: Zuerst Optionen löschen (falls CASCADE nicht aktiv)
DELETE FROM quiz_options WHERE quiz_id = 1;
DELETE FROM quiz WHERE id = 1;



-- mitarbeiter hinzufügen
INSERT INTO collaborators (deck_id, user_id, can_edit) 
VALUES (1, 2, true);

-- Folgen
INSERT INTO follows (follower_id, followed_id) 
VALUES (1, 2);

-- alle mitarbeiter eines Decks anzeigen
SELECT collaborator.username AS username_collaborator, d.title AS deck_title, creator.username AS username_creator
FROM collaborators c
JOIN users collaborator ON c.user_id = collaborator.id
JOIN decks d ON c.deck_id = d.id
JOIN users creator ON d.creator_id = creator.id
WHERE c.deck_id = 1;


-- Follower Statistik
-- Anzahl Follower pro Benutzer
SELECT u.username, COUNT(f.follower_id) AS follower_count
FROM users u
LEFT JOIN follows f ON u.id = f.followed_id
GROUP BY u.id;


-- anzahl decks/karten pro benutzer
SELECT 
    u.username,
    COUNT(DISTINCT d.id) AS deck_count,
    COUNT(DISTINCT c.id) + COUNT(DISTINCT q.id) AS card_count
FROM users u
LEFT JOIN decks d ON u.id = d.creator_id
LEFT JOIN cards c ON d.id = c.deck_id
LEFT JOIN quiz q ON d.id = q.deck_id
WHERE u.id = 1
GROUP BY u.id;




SELECT 
    d.id AS deck_id,
    d.title AS deck_name,
    'card' AS card_type,
    c.question,
    c.answer AS correct_answer,
    NULL AS options
FROM 
    decks d
JOIN 
    cards c ON d.id = c.deck_id

UNION ALL

SELECT 
    d.id AS deck_id,
    d.title AS deck_name,
    'quiz' AS card_type,
    q.question,
    -- Zeigt die richtige Antwort als Text (nicht nur Index)
    CASE q.correct_answer
        WHEN 1 THEN qo.first_option
        WHEN 2 THEN qo.second_option
        WHEN 3 THEN qo.third_option
        WHEN 4 THEN qo.fourth_option
    END AS correct_answer,
    -- Kombiniert alle Optionen in einem JSON-Format
    JSON_OBJECT(
        'option1', qo.first_option,
        'option2', qo.second_option,
        'option3', qo.third_option,
        'option4', qo.fourth_option
    ) AS options
FROM 
    decks d
JOIN 
    quiz q ON d.id = q.deck_id
JOIN 
    quiz_options qo ON q.id = qo.quiz_id
ORDER BY 
    deck_id, card_type;

