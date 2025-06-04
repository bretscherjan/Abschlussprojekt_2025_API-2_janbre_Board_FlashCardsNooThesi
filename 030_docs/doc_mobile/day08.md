# Tag 8

# Objektorientierte Prinzipien in der Flashcard-App

## 1. Die vier Grundprinzipien der Objektorientierung

### Kapselung
- Daten und Methoden, die zusammengehören, werden in einer Klasse "verpackt".
- Der Zugriff auf Methoden in Klassen wird kontrolliert (private/public).

### Abstraktion
- Komplexität wird versteckt, indem nur relevante Details gezeigt werden.
- Klassen können abstrakte Methoden definieren, die erst in Kind-Klassen ausgefüllt werden.
- Mit dem Schlüsselwort ```abstract``` wird eine Klasse nur als übergeortnete Klasse verwenden, und man kann keine Objekte daraus Instanziieren.

### Vererbung
- Eine Klasse kann Eigenschaften und Methoden einer anderen Klasse übernehmen.
- Vermeidet Code-Dopplungen und Verallgemeinert alles.

### Polymorphie
- Objekte können je nach Kontext unterschiedliche Formen annehmen.
- Methoden heissen gleich, verhalten sich aber anders (z. B. Tier.sound() gibt bei Hund "Wuff", bei Katze "Miau" aus).

## 2. Analyse der Anwendung von OOP-Prinzipien im Projekt

### 2.1 Kapselung

#### Wo zeigt sich dieses Prinzip in der App?
Im Code der App wird Kapselung vorallem in den lokalen States verwendet.

```javascript
const [title, setTitle] = useState('');
const [gradient, setGradient] = useState([]);
```

Die Daten (wie `title` und `gradient`) werden durch State-Variablen gekapselt und können nur über die entsprechenden Setter-Funktionen (`setTitle`, `setGradient`) modifiziert werden.

#### Konkrete Beispiele:
1. Die Deck-Erstellung in `create.tsx` kapselt alle relevanten Zustände und Funktionen:
   ```javascript
   const [title, setTitle] = useState('');
   const [gradient, setGradient] = useState([]);

   function onTitleChange(newText) {
     setTitle(newText);
   }

   function setGradientColor(color){
    // ...
   }

   async function saveDeck() {
     // ...
   }
   ```

2. In `_layout.tsx` wird der Keyboard-Status gekapselt:
   ```javascript
   const [isKeyboardVisible, setKeyboardVisible] = useState(false);

   useEffect(() => {
     const keyboardDidShowListener = Keyboard.addListener(
       'keyboardDidShow',
       () => {
         setKeyboardVisible(true);
       }
     );
     // ...
   }, []);
   ```

#### Verbesserungsmöglichkeiten:
- Eine bessere Trennung zwischen Frontend und Backend würde den Code um einiges lesbarer machen. Zum Beispiel könnte man die Logik vom Async Storage an anderen Orten, als beim Frontend verwalten.

### 2.2 Abstraktion

#### Wo zeigt sich dieses Prinzip in der App?

```javascript
function renderDeck(renderItem) {
  var item = renderItem.item;
  // ...
  return (
    <TouchableOpacity
      style={[styles.deckCard]}
      onPress={function () { navigateToDeck(item.id); }}
      // ...
    >
      // ...
    </TouchableOpacity>
  );
}
```

Die komplexe Implementierung der Deck-Karte wird hinter einer einfachen Funktion abstrahiert.

#### Konkrete Beispiele:
1. Die Abstraktion der Navigation durch Expo Router:
   ```javascript
   function navigateToDeck(id) {
     router.push(`/deck/${id}`);
   }

   function editCards(id) {
     router.push({
       pathname: '/deck/add-card/editCard',
       params: { deckId: id }
     });
   }
   ```

2. Die Abstraktion der Card-Renderlogik in `[learnDeckID].tsx`:
   ```javascript
   function renderLearningCards() {
    // ...
   }

   if (loading) {
     return renderLoadingState();
   } else if (!deck || !deck.cards || deck.cards.length === 0) {
     return renderErrorState();
   } else {
     return renderLearningCards();
   }
   ```

#### Verbesserungsmöglichkeiten:
- Separate Komponenten für Card, Deck, und Quiz könnten erstellt werden, um die Abstraktion zu verbessern.

### 2.3 Vererbung

#### Wo zeigt sich dieses Prinzip in der App?

1. Die Vererbung von Styles in alle Codeteile durch das zentrale `styles.js`:
   ```javascript
   import styles from '../styles';

   <View style={styles.container}>
   ```

2. Die "Vererbung" von Eigenschaften durch Props:
   ```javascript
   <Tabs.Screen
     name="index"
     options={{
       tabBarIcon: ({ color }) => (
         <FontAwesome name="home" size={32} color={color} />
       ),
     }}
   />
   ```

#### Verbesserungsmöglichkeiten:
- Implementierung einer Basis-Komponente für Kartentypen (FlashCard), von der spezifische Kartentypen (QuizCard, Card) ableiten könnten.

### 2.4 Polymorphie

#### Wo zeigt sich dieses Prinzip in der App?
Die unterschiedliche Darstellung und Verhalten von Karten basierend auf ihrem Typ:

```javascript
{card.type === 'card' && (
    // ...
)}
{card.type === 'quiz' && (
    // ...
)}
```

#### Konkrete Beispiele:
1. Unterschiedliches Rendering und Verhalten von Karten basierend auf ihrem Typ:
   ```javascript
   {card.type === 'card' && (
     <>
       <TouchableOpacity onPress={flipCard} activeOpacity={0.9} style={styles.cardWrapper}>
         {/* Vorderseite und Rückseite der Karte */}
       </TouchableOpacity>
     </>
   )}
   {card.type === 'quiz' && (
     <>
       <View style={styles.card}>
         <Text style={styles.cardTitle}>Question: {currentCardIndex + 1}</Text>
         <Text style={styles.cardText}>{card.question}</Text>
         <View style={styles.title}>
           {card.options.map((option, index) => (
             <TouchableOpacity key={index} style={styles.quizButton} onPress={() => { checkAnswer(card.options.indexOf(option)); }}>
               <Text style={styles.quizButtonText}>{option}</Text>
             </TouchableOpacity>
           ))}
         </View>
       </View>
     </>
   )}
   ```

2. Unterschiedliches Verhalten der Render-Funktionen basierend auf dem Zustand der App:
   ```javascript
   if (loading) {
     return renderLoadingState();
   } else if (!deck) {
     return renderErrorState();
   } else {
     return renderDeckDetails();
   }
   ```

#### Verbesserungsmöglichkeiten:
- Die Files addCard.tsx und editCard.tsx sind sehr ähnlich. Es wäre schöner, wenn die edit und add Funktion im gleichen File funktionieren würde.

## 3. Konkrete Beispiele im Projekt

### 3.1 Deck- und Card-Objekte

Die Hauptdatenstrukturen in der App sind:

1. **Deck-Objekte in JSON Format**:
   ```javascript
   const newDeck = {
     id: Date.now().toString(),
     title: title.trim(),
     alt: title.trim(),
     gradientColors: gradient.length >= 2 ? gradient : getRandomColor(),
     cards: [
       // Karten-Objekte
     ]
   };
   ```

2. **Card-Objekte** mit zwei Typen:
   ```javascript
   // Standard-Karte
   {
     type: 'card',
     question: 'Was heisst Bonjour?',
     answer: 'Guten Tag'
   }

   // Quiz-Karte
   {
     type: 'quiz',
     question: 'Was ist die Hauptstadt der Schweiz?',
     options: ['Zürich', 'Bern', 'Genf', 'Lausanne'],
     correctAnswerIndex: 1
   }
   ```

### 3.2 Komponenten
- **Wiederverwendbare UI-Elemente**:
  - **Deck-Karten** (`index.tsx`): Render-Funktion für Deck-Ansicht mit Gradienten und Kartenanzahl.
  - **Modale Dialoge** (`index.tsx`): Einstellungs-Modal für Decks (Löschen/Bearbeiten).
  - **Karten-Animation** (`[learnDeckID].tsx`): Flip-Effekt für Lernkarten mit `react-native-reanimated`.
  - **Formulare** (`editCard.tsx`): Dynamische Eingabefelder für Karten/Quizze.

- **State-Management**:
  - Lokale States wie `decks`, `selectedDeck`, `currentCardIndex` steuern die Darstellung.


### 3.3 Props Schnittstellen

Props (Properties) werden als Schnittstellen zwischen Komponenten verwendet:

```javascript
<Tabs.Screen
  name="index"
  options={{
    tabBarIcon: ({ color }) => (
      <FontAwesome name="home" size={32} color={color} />
    ),
  }}
/>
```

Hier werden Optionen als Props übergeben, die das Verhalten und Aussehen der Tab-Komponente definieren.

### 3.4 Speicherlogik, Router und dynamische Views
- **AsyncStorage**:
  - **Datenstruktur**: Decks werden als JSON-Array unter dem Schlüssel `'decks'` gespeichert.
  - **CRUD-Operationen**:
    - **Create**: `create.tsx` erstellt neue Decks mit `AsyncStorage.setItem()`.
    - **Read**: `index.tsx` lädt Decks via `AsyncStorage.getItem()`.
    - **Update**: `editdeck.tsx` aktualisiert Titel/Farbe eines Decks.
    - **Delete**: `index.tsx` filtert Decks heraus und speichert sie neu.
- **Expo Router**:
  - **Tabs**: `_layout.tsx` definiert zwei Tabs (Home/Create) mit Icons.
  - **Stack-Navigation**: Dynamische Routes wie `/[deckID]` oder `/deck/learn/[learnDeckID]`.
  - **Parameter-Weitergabe**:
    - Beispiel: `router.push(/deck/${id})` (Home → Deck-Detail).
    - Oder mit Parametern: `router.push({ pathname: '/editdeck', params: { deckId } })`.
- **Dynamische Views**:
  - `[deckID].tsx`: Zeigt Deck-Details basierend auf URL-Parametern.
  - `[learnDeckID].tsx`: Rendert Lernkarten oder Quizze je nach `card.type`.
  - **Bedingtes Rendering**:
    - Leere States (`decks.length === 0`), Lade-Indikatoren (`loading`), Fehleransichten.



## 4. Verbesserungsmöglichkeiten

### 4.1 Klarere Trennung von Logik und Darstellung

1. **Custom Hooks für Datenoperationen**:
   ```javascript
   function useDeckOperations() {
     const loadDeck = async (id) => { /* ... */ };
     const saveDeck = async (deck) => { /* ... */ };
     const deleteDeck = async (id) => { /* ... */ };

     return { loadDeck, saveDeck, deleteDeck };
   }
   ```

2. **Frontend und Backend separieren**:
   - Es wäre übersichtlicher, wenn das Backend vom Frontend getrennt wäre.
   - Zum Beispiel die SaveDeck-Funktion in create.tsx könnte man in eine andere Datei auslagern, inder nur das Backend läuft.

### 4.2 Basisklassen oder gemeinsame Typen

1. **Gemeinsame Interface für Karten**:
   ```javascript
   interface BaseCard {
     type: string;
     question: string;
   }

   interface StandardCard extends BaseCard {
     type: 'card';
     answer: string;
   }

   interface QuizCard extends BaseCard {
     type: 'quiz';
     options: string[];
     correctAnswerIndex: number;
   }
   ```

2. **Base Component für Karten**:
   ```javascript
   function BaseCard({ question, renderContent }) {
     return (
       <View style={styles.card}>
         <Text style={styles.cardTitle}>Question:</Text>
         <Text style={styles.cardText}>{question}</Text>
         {renderContent()}
       </View>
     );
   }
   ```

### 4.3 Klassisch objektorientierter Aufbau

Ein klassisch objektorientierter Aufbau könnte so aussehen:

1. **Model-Klassen**:
   ```javascript
   class Deck {
     constructor(id, title, gradientColors = []) {
       this.id = id;
       this.title = title;
       this.alt = title;
       this.gradientColors = gradientColors;
       this.cards = [];
     }

     addCard(card) {
       this.cards.push(card);
     }

     removeCard(index) {
       this.cards.splice(index, 1);
     }
   }

   class Card {
     constructor(question) {
       this.question = question;
       this.type = null;
     }
   }

   class StandardCard extends Card {
     constructor(question, answer) {
       super(question);
       this.type = 'card';
       this.answer = answer;
     }
   }

   class QuizCard extends Card {
     constructor(question, options, correctAnswerIndex) {
       super(question);
       this.type = 'quiz';
       this.options = options;
       this.correctAnswerIndex = correctAnswerIndex;
     }
   }
   ```

2. **Service-Klassen**:
   ```javascript
   class DeckService {
     static async getAll() {
       // ..
     }

     static async getById(id) {
       // ..
     }

     static async save(deck) {
       // ..
     }

     static async delete(id) {
       // ..
     }
   }
   ```

3. **View-Komponenten**:
   ```javascript
   function DeckView({ deck, onCardEdit, onDeckEdit }) {
     // ..
   }

   function CardView({ card, onFlip, onAnswer }) {
     // ..
   }
   ```

## 5. Fazit

Meine Flashcard-App verwendet bereits einige objektorientierte Konzepte, welche auch gut ersichtlich sind. Es gibt jedoch auch viele verbesserungsmöglichkeiten, welche ich noch nicht ganz sauber umgesetzt habe. Ich versuche nun noch den Code etwas übersichtlicher und besser zu gestallten, sodass ich schneller Fehler finden kann. Grundsätzlich ist die App aber ziemlich gut und erfüllt ihren Zweck.


