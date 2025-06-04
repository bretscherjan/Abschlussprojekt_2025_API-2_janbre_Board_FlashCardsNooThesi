# Tag 2

### 1. Themenbereiche
- **AsyncStorage**
- **FlatList**
- **useEffect**
- **useFocusEffect**

### 2. AsyncStorage installieren
- Paketinstallation:
  ```bash
  npx expo install @react-native-async-storage/async-storage
  ```

### 3. Startseite (`index.tsx`) erweitern

- **FlatList** für zweispaltiges Grid eingebaut:
  ```tsx
  <FlatList
    data={decks}
    renderItem={renderDeck}
    keyExtractor={(item) => item.id}
    numColumns={2}
  />
  ```

- **useFocusEffect** implementiert, um Daten beim Fokussieren zu laden:
  ```tsx
  useFocusEffect(
    useCallback(() => {
      loadDecks();
    }, [])
  );
  ```

- **AsyncStorage-Zugriff** eingebaut:
  ```tsx
  async function loadDecks() {
    try {
      const storedDecks = await AsyncStorage.getItem('decks');
      if (storedDecks) {
        setDecks(JSON.parse(storedDecks));
      }
    } catch (error) {
      console.error('Fehler beim Laden der Decks:', error);
    }
  }
  ```

- **UI**: Deck-Karten mit ansprechendem Styling gestaltet.

### 4. Create-Seite (`create.tsx`) implementieren

- **Formular** mit TextInput für den Deck-Titel erstellt:
  ```tsx
  <TextInput
    style={styles.input}
    value={title}
    onChangeText={onTitleChange}
    placeholder="z.B. Mathematik, Englisch..."
    maxLength={50}
  />
  ```

- **Zufällige Farbauswahl** für Deck:
  ```tsx
  function getRandomColor() {
    const randomIndex = Math.floor(Math.random() * COLORS.length);
    return COLORS[randomIndex];
  }
  ```

- **Validierung** der Eingabe:
  ```tsx
  if (!title.trim()) {
    Alert.alert('Fehler', 'Bitte gib einen Titel für das Deck ein.');
    return;
  }
  ```

- **AsyncStorage-Integration** zum Speichern von Decks:
  ```tsx
  const storedDecks = await AsyncStorage.getItem('decks');
  // Decks aktualisieren
  await AsyncStorage.setItem('decks', JSON.stringify(decks));
  ```

### 5. Deck-Detailseite (`[deckID].tsx`) aktualisiert

- **Datenladen** mit `useEffect` und `AsyncStorage`:
  ```tsx
  useEffect(() => {
    loadDeckDetails();
  }, [deckId]);

  async function loadDeckDetails() {
    const storedDecks = await AsyncStorage.getItem('decks');
  }
  ```

### 6. Datenstruktur

- **Deck**:
  - ID
  - Titel
  - Farbe
  - Cards-Array

- **Card**:
  - Frage
  - Antwort

### 7. Navigation zwischen Screens

- Navigation von der Startseite zur Create-Seite
- Navigation von der Startseite zu Deck-Details
- **Zurück-Navigation** implementiert

## Fazit

Der Start war heute ziemlich zäh. Ich konnte wieder nicht mit meinem Smartphone connecten. Deshalb musste ich mich nochmals etwas mit Android Studio befassen. Ich habe nun viel Erfahrung sammeln können und fühle mich wohl bei der Arbeit mit Android Studio. Ich bin heute gut am Projekt weiter gekommen und verstehe Typescript nun auch schon viel besser. Ich finde dieses Projekt toll.