CREATE TABLE follows (
  follower_id BIGINT NOT NULL,
  followed_id BIGINT NOT NULL,
  created_at DATETIME DEFAULT GETDATE(),
  PRIMARY KEY (follower_id, followed_id),
);

CREATE TABLE decks (
  id BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  title VARCHAR(55) NOT NULL,
  alt VARCHAR(55),
  is_private BIT NOT NULL DEFAULT 1,
  creator_id BIGINT NOT NULL,
  created_at DATETIME DEFAULT GETDATE(),
);

CREATE TABLE collaborators (
  deck_id BIGINT NOT NULL,
  user_id BIGINT NOT NULL,
  can_edit BIT NOT NULL DEFAULT 0,
  PRIMARY KEY (deck_id, user_id),
);

CREATE TABLE deck_colors (
  deck_id BIGINT PRIMARY KEY,
  start_color VARCHAR(7) NOT NULL,
  end_color VARCHAR(7) NOT NULL,
);

CREATE TABLE cards (
  id BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  deck_id BIGINT NOT NULL,
  question VARCHAR(55) NOT NULL,
  answer VARCHAR(55) NOT NULL,
  is_fav BIT DEFAULT 0,
  status VARCHAR(20) DEFAULT 'needs_practice',
  created_at DATETIME DEFAULT GETDATE(),
);

CREATE TABLE quiz (
  id BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,
  deck_id BIGINT NOT NULL,
  question VARCHAR(55) NOT NULL,
  is_fav BIT DEFAULT 0,
  correct_answer SMALLINT NOT NULL CHECK (correct_answer BETWEEN 1 AND 4),
  status VARCHAR(20) DEFAULT 'needs_practice',
  created_at DATETIME DEFAULT GETDATE(),
);

CREATE TABLE quiz_options (
  quiz_id BIGINT NOT NULL PRIMARY KEY,
  first_option VARCHAR(55) NOT NULL,
  second_option VARCHAR(55) NOT NULL,
  third_option VARCHAR(55) NOT NULL,
  fourth_option VARCHAR(55) NOT NULL,
);



ALTER TABLE follows ADD FOREIGN KEY (follower_id) REFERENCES users (id) ON DELETE CASCADE;
ALTER TABLE follows ADD FOREIGN KEY (followed_id) REFERENCES users (id) ON DELETE CASCADE;
ALTER TABLE decks ADD FOREIGN KEY (creator_id) REFERENCES users (id) ON DELETE CASCADE;
ALTER TABLE collaborators ADD FOREIGN KEY (deck_id) REFERENCES decks (id) ON DELETE CASCADE;
ALTER TABLE collaborators ADD FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE;
ALTER TABLE deck_colors ADD FOREIGN KEY (deck_id) REFERENCES decks (id) ON DELETE CASCADE;
ALTER TABLE cards ADD FOREIGN KEY (deck_id) REFERENCES decks (id) ON DELETE CASCADE;
ALTER TABLE quiz ADD FOREIGN KEY (deck_id) REFERENCES decks (id) ON DELETE CASCADE;
ALTER TABLE quiz_options ADD FOREIGN KEY (quiz_id) REFERENCES quiz (id) ON DELETE CASCADE;