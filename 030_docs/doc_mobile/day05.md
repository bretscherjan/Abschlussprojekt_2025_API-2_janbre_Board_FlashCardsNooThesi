# Tag 5

### 1. Deck löschen
- Mit 'onLongPress' auf ein Deck wird ein Alert aufgerufen
```
onLongPress={() => infoAlert(item.title, item.id, cardCount)}
```
```
  const infoAlert = (title, id, cardCount) =>
    Alert.alert(title, cardCount + ' cards in this deck.', [
      {
        text: 'DELETE 🗑️',
        onPress: () => removeItem(id),
      },
      {
        text: 'EDIT ✏️',
        onPress: () => rename(id),
      },
      {text: 'OK ✅', onPress: () => console.log('OK Pressed')},
    ]);
```
- In diesem Alert gibt es die Option DELETE um das gewählte Objekt zu löschen
- Die remove Funktion um das Objekt aus dem Speicher zu löschen.
```
  async function removeItem(id) {
    try {
      // 1. Hole aktuelle Decks aus AsyncStorage
      const storedDecks = await AsyncStorage.getItem('decks');
      if (!storedDecks) return;

      // 2. Filtere das Deck mit der ID heraus
      const updatedDecks = JSON.parse(storedDecks).filter(deck => deck.id !== id);

      // 3. Speichere die aktualisierte Liste zurück
      await AsyncStorage.setItem('decks', JSON.stringify(updatedDecks));

      // 4. Aktualisiere den State, um die UI zu refreshen
      setDecks(updatedDecks);

      console.log('Deleted deck:', id);
    } catch (error) {
      console.log('Delete Error:', error);
    }
  }
```

### 2. Farbe ändern

- Dem json die neue Kategorie für gradientColors hinzufügen, um die beiden Farben für den Farbverlauf zu speichern
```
const newDeck = {
  id: Date.now().toString(),
  title: title.trim(),
  alt: title.trim(),
  gradientColors: gradient.length >= 2 ? gradient : ['rgba(0, 0, 0, 0.3)', 'rgba(255, 255, 255, 0.5)'],
  cards: [
    {
      question: 'What is React Native?',
      answer: 'A framework for app development with JavaScript.'
    },
    {
      question: 'What does useState do?',
      answer: 'It stores local states in a component.'
    },
    {
      question: 'What is AsyncStorage for?',
      answer: 'For storing data locally on the device.'
    }
  ]
};
```
- Die verschiedenen, möglichen Farbauswahlen darstellen
```
<TouchableOpacity onPress={() => setGradientColor('p')}>
  <View style={[
    styles.color,
    { backgroundColor: 'rgba(156, 39, 176, 0.7)' },
    isColorSelected('p') && { opacity: 1, borderWidth: 5, borderColor: 'black' }
  ]} />
</TouchableOpacity>
```
- isColorSelected macht einen dickeren Rahmen bei der ausgewählten Farbe
- Der Switch um 2 passende Farben für den Verlauf zu erstellen
```
function setGradientColor(color) {
  switch (color) {
    case 'p':
      setGradient(['rgba(156, 39, 176, 0.8)', 'rgba(76, 0, 0, 0.3)']);
      break;
    case 'b':
      setGradient(['rgba(33, 150, 243, 0.8)', 'rgba(0, 43, 78, 0.3)']);
      break;
    case 'green':
      setGradient(['rgba(76, 175, 80, 0.8)', 'rgba(0, 74, 2, 0.3)']);
      break;
    case 'y':
      setGradient(['rgba(223, 170, 48, 0.8)', 'rgba(77, 62, 0, 0.3)']);
      break;
    case 'r':
      setGradient(['rgba(255, 82, 82, 0.8)', 'rgba(76, 0, 0, 0.3)']);
      break;
    case 'gray':
      setGradient(['rgba(167, 167, 167, 0.8)', 'rgba(67, 67, 67, 0.3)']);
      break;
    default:
      setGradient(['rgba(255, 255, 255, 0.4)', 'rgba(255, 255, 255, 0.3)']);
      break;
  }
}
```
- Man kann nur die vorgegebenen Farben auswählen, und auch nur immer den gleichen Farbverlauf

# Fazit

Die delete Funktion habe ich bereits an einem anderen Tag umgesetzt. Ich bin zufrieden mit meinen Fortschritten. Es lief ziemlich alles wie geplant und gab keine weiteren Schwierigkeiten. Ich habe nun noch ein paar routen zwischen den Seiten eingebaut, sodass man nun z.B. ein Deck erstellen kann und danach gleich zur Seite um Karten hinzuzufügen kommt. Es macht mir immernoch spass mit react native zu arbeiten. Ich finde vor allem toll, dass ich immer wieder meine Vortschritte sehe in der App.
Ich konnte nun auch mit expo go arbeiten. Die Lösung ist, dass ich im Casette WLAN sein muss und die App via --tunnel mode starten muss.



