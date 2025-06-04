# Tag 2

### 1. Themenbereiche
- **Styling**
- **visuellen Gestaltung**
- **dynamischen Parametern**
- **useFocusEffect**

### 2. Styles und Logik Trennen
- Für jedes Typescript File (Bsp. index.tsx) ein separates styles.ts File erstellen.
- Der Style-Code aus den .tsx Dateien in .ts Dateien kopieren.
- Den Style richtig exportieren.
    ```export default styles;```
- Den Style in .tsx importieren.
    ```import styles from './stylesIndex';```

### 3. Grunddesign anpassen
- passende Farben suchen (Grau #2f2f2f & Gelb #dfaa30)
- Design nach diesen Farben auf allen Pages gleich abstimmen
- Detail anpassung: Der Button in index.tsx immer unten am Bildschirm darstellen.

### 4. Farbverlauf
- Farbverlauf mit LinearGradient
- LinearGradient installieren:
```npx expo install expo-linear-gradient```
- LinearGradient importieren:
```import { LinearGradient } from 'expo-linear-gradient';```
- Neuer Hook für die Farbwerte für den Verlauf
```const [gradient, setGradient] = useState<string[]>([]);```
- Funktion für die Farben für einen passenden Verlauf
```
function setGradientColor(color) {
    switch (color) {
        case 'p':
            setGradient(['rgba(156, 39, 176, 0.8)', 'rgba(76, 0, 0, 0.3)']);
            break;
    }
}
```
- Neuer Wert im Deck für die Farben
```gradientColors: gradient.length >= 2 ? gradient : ['rgba(0, 0, 0, 0.3)', 'rgba(255, 255, 255, 0.5)'],```
- Die Farben Abrufen und den Farbverlauf von oben-links (x0,y0) nach unten-rechts (x1,y1) darstellen
```
<LinearGradient
    colors={deck.gradientColors}
    style={styles.header}
    start={{ x: 0, y: 0 }}
    end={{ x: 1, y: 1 }}
>
    <Text style={styles.title}>{deck.title}</Text>
    <Text style={styles.cardCount}>{deck.cards.length} Karten</Text>
</LinearGradient>
```

### 5. OnLongPress beschreibung
- Die onLongPress-Funktion verwenden um einen Alert zu senden.
```
onLongPress={() => {
        Alert.alert(item.alt);
    }
}
```

### 6. Fragen anzeigen
- Neuer Ordner (/app/deck/learn) und neue Datei ([learnDeckID].tsx) erstellen
- Hooks einbinden und Variablen definieren
```
const params = useLocalSearchParams();
const deckId = params.learnDeckID;
const [deck, setDeck] = useState(null);
const [loading, setLoading] = useState(true);
const [currentCardIndex, setCurrentCardIndex] = useState(0);
const [isFlipped, setIsFlipped] = useState(false);
```
- Aktuelles Deck aus den URL-Parametern auslesen
```
async function loadDeckDetails() {
  try {
    setLoading(true);
    var storedDecks = await AsyncStorage.getItem('decks');
    if (storedDecks) {
      var decks = JSON.parse(storedDecks);
      var foundDeck = decks.find(deck => deck.id === deckId);
      setDeck(foundDeck);
    } else {
      setDeck(null);
    }

  } catch (error) {
    console.error('Fehler beim Laden der Deck-Details:', error);
    setDeck(null);
  } finally {
    setLoading(false);
  }
}
```
- Karten aus dem ausgewählten Deck anzeigen.
```
<TouchableOpacity onPress={flipCard} activeOpacity={0.9}>
  <Animated.View style={[styles.card]}>
    <Text style={styles.cardTitle}>Frage: {currentCardIndex + 1}</Text>
    <Text style={styles.cardText}>{card.question}</Text>
    <Text style={styles.flipHint}>Tippen zum Umdrehen</Text>
  </Animated.View>

  <Animated.View style={[styles.card, styles.cardBack]}>
    <Text style={styles.cardTitle}>Antwort:</Text>
    <Text style={styles.cardText}>{card.answer}</Text>
    <Text style={styles.flipHint}>Tippen zum Umdrehen</Text>
  </Animated.View>
</TouchableOpacity>
```
- Zwischen verschiedenen Karten wechseln
```
function nextCard() {
  if (currentCardIndex < deck.cards.length - 1) {
    if (isFlipped) {
      flipCard();
    }
    setCurrentCardIndex(currentCardIndex + 1);
  }
}

function prevCard() {
  if (currentCardIndex > 0) {
    if (isFlipped) {
      flipCard();
    }
    setCurrentCardIndex(currentCardIndex - 1);
  }
}
```




# Fazit

Ich habe nicht alle Vorgaben fürs Design eingehalten, welche im Moodle standen. Ich möchte diese App so designen, dass sie mir gefällt. Deshalb habe ich bei einigen Punkten meine Vorstellung umgesetzt. Ich konnte heute gut selbstständig arbeiten und kam auch sehr gut voran. Ich habe sehr viel Zeit für den Farbverlauf verbraucht. Ich bin nun jedoch sehr zufrieden und habe eine weitere Funktion hinzugefügt, mit der man die Farbe für das jedes neue Deck wählen kann. Ein weiterer komplexer schritt war das Anzeigen der einzelnen Karten. Ich bin da noch nicht ganz fertig, wie ich es mir vorgestellt habe. Das schwierigste und wichtigste habe ich nun jedoch bereits. Ich möchte bei diesen Karten eine ähnliche Animation wie bei Quizzlet. An dieser Animation werde ich noch zu Hause oder morgen Arbeiten.



Tutorial für rotating card:

https://www.google.com/search?q=react+native+rotating+card+animation&sca_esv=393159a26b253a41&sxsrf=AHTn8zqZvR6s0iXnp5xNN6GPEPmauIJW8g%3A1744293227009&ei=a833Z6kh4Y327w_crLa5DA&oq=react+native+rotating+card&gs_lp=Egxnd3Mtd2l6LXNlcnAiGnJlYWN0IG5hdGl2ZSByb3RhdGluZyBjYXJkKgIIATIFECEYoAEyBRAhGKABMgUQIRifBTIFECEYnwUyBRAhGJ8FMgUQIRifBUjpQFAAWNUxcAB4AZABAJgBtwGgAdoSqgEEMTcuObgBAcgBAPgBAZgCGqACsxPCAgwQIxiABBgTGCcYigXCAgoQIxiABBgnGIoFwgIKEAAYgAQYQxiKBcICERAuGIAEGLEDGNEDGIMBGMcBwgILEAAYgAQYsQMYgwHCAgoQLhiABBhDGIoFwgIEECMYJ8ICEBAAGIAEGLEDGEMYgwEYigXCAggQLhiABBixA8ICCBAAGIAEGLEDwgIFEAAYgATCAggQABiABBjLAcICBhAAGBYYHsICBhAAGA0YHsICCRAAGIAEGBMYDcICCBAAGBMYDRgewgIIEAAYExgWGB7CAgoQABgTGAgYDRgewgIFEAAY7wWYAwCSBwUxNi4xMKAHzbQBsgcFMTYuMTC4B7MT&sclient=gws-wiz-serp#fpstate=ive&vld=cid:9436ba15,vid:8z4zdCObixs,st:0

