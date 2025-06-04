# Tag 7

### 1. Neue Array struktur
```javascript
{
  id: Date.now().toString(),
  title: title.trim(),
  alt: title.trim(),
  gradientColors: gradient.length >= 2 ? gradient : getRandomColor(),
  cards: [
    { type: 'card', question: 'Was heisst Bonjour?', answer: 'Guten Tag' },
    {
      type: 'quiz',
      question: 'Was ist die Hauptstadt der Schweiz?',
      options: ['Zürich', 'Bern', 'Genf', 'Basel'],
      correctAnswerIndex: 1
    }
  ]
}
```
Diese neue Arraystruktur erlaubt im selben Deck auch Quizfragen zu haben. Jede neue Frage ist entweder type 'card' oder 'quiz'.


### 2. Anzahl Quiz und Karten anzeigen
```
<Text style={styles.subtitle}>
  {deck.cards.filter(card => card.type === 'card').length} Cards
</Text>
<Text style={styles.subtitle}>
  {deck.cards.filter(card => card.type === 'quiz').length} Quiz
</Text>
```
Ich frage bei der Detailierten Deckansicht die anzahl der Karten und der Quiz Inhalte einzel ab.


### 3. Quiz hinzufügen & zwischen Quiz und Karte unterscheiden beim Input
```
<TouchableOpacity
  style={[styles.buttonAdd, {marginTop: 10}]}
  onPress={addNewQuiz}
>
  <Text style={styles.buttonText}>Add New Quiz</Text>
</TouchableOpacity>
```
Es gab einen neuen Button um ein Quiz hinzuzufügen.

```
function addNewQuiz() {
  setEditingCards([
    ...editingCards,
    {
      type: 'quiz',
      question: '',
      options: ['', '', '', ''],
      correctAnswerIndex: 0
    }
  ]);
}
```
Zum aktuellen Deck wird ein neues Array hinzugefügt mit leeren Werten. Diesem Deck wird beim Speichern die Werte hinzugefügt.

```
{editingCards.map((card, index) => (
```
Eine View pro Karte im Deck(Frage-Antwort oder Quiz)
```
  <View 
    key={index} 
    style={{ 
      marginBottom: 20, 
      borderBottomWidth: 1, 
      borderBottomColor: '#444', 
      paddingBottom: 10 
    }}
  >
```
Überschrift mit Kartennummer und Typ
```
    <Text style={styles.label}>Card {index + 1} ({card.type})</Text>
```
Eingabefeld für die Frage
```
    <Text style={styles.label}>Question</Text>
    <TextInput
      style={styles.input}
      value={card.question}
      onChangeText={(text) => handleCardChange(index, 'question', text)}
      placeholder="Enter question"
      placeholderTextColor="#707070"
      multiline
    />
```
Wenn der Kartentyp "card" ist (Frage-Antwort-Karte)
```
    {card.type === 'card' && (
      <>
        <Text style={styles.label}>Answer</Text>
        <TextInput
          style={styles.input}
          value={card.answer}
          onChangeText={(text) => handleCardChange(index, 'answer', text)}
          placeholder="Enter answer"
          placeholderTextColor="#707070"
          multiline
        />
      </>
    )}
```
Wenn der Kartentyp "quiz" ist (Multiple-Choice-Karte)
```
    {card.type === 'quiz' && (
      <>
        <Text style={styles.label}>Options</Text>
```
Alle Antwortmöglichkeiten durchgehen
```
        {card.options.map((opt, optIdx) => (
          <TextInput
            key={optIdx}
            style={styles.input}
            value={opt}
            placeholder={`Option ${optIdx + 1}`}
            onChangeText={(text) => {
              const newOptions = [...card.options];
              newOptions[optIdx] = text;
              handleCardChange(index, 'options', newOptions);
            }}
            placeholderTextColor="#707070"
          />
        ))}
```
Eingabefeld für den Index der richtigen Antwort
```
        <Text style={styles.label}>Correct Answer (Index)</Text>
        <TextInput
          style={styles.input}
          keyboardType="numeric"
          value={card.correctAnswerIndex?.toString()}
          onChangeText={(text) =>
            handleCardChange(index, 'correctAnswerIndex', (parseInt(text)) || 0)
          }
          placeholder="Correct option index (0–3)"
          placeholderTextColor="#707070"
        />
      </>
    )}
```
Entfernen-Button für die Karte
```
    <TouchableOpacity
      style={[styles.buttonAdd, { backgroundColor: '#ff4d4d', marginTop: 5 }]}
      onPress={() => removeCard(index)}
    >
      <Text style={styles.buttonText}>Remove Card</Text>
    </TouchableOpacity>
  </View>
))}
```
Nach jeder eingabe wird die Funktion handleCardChange() aufgerufen um die Daten zu speichern. Dies ist gleichgeblieben.



### 4. Lernen mit dem Quiz
```
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
Auch hier wird wenn der type === quiz ist, wird es so dargestellt. Wenn man auf eine auswahl clickt, wird die Funktion checkAnswer() aufgerufen.

```
function checkAnswer(selectedOption) {
  const card = deck.cards[currentCardIndex];
  if (card.type === 'quiz') {
    if (parseInt(selectedOption) === parseInt(card.correctAnswerIndex)) {
      Alert.alert('Correct!', 'Well done!', [{ text: 'OK' }]);
    } else {
      Alert.alert('Wrong answer', 'Try again!', [{ text: 'OK' }]);
    }
  }
}
```


# Fazit
Ich hatte zu Beginn einige Probleme beim updaten von von der Expoversion. Ich habe viel Zeit für dies verloren. Ich habe ein Problem mit meiner package.json Datei gehabt. Ich bekam danach hilfe von Reto und es funktionierte danach schnell. Alle weiteren Aufträge konnte ich danach gut Lösen. Ich habe das mit den Karten lernen bereits früher mal gemacht. Deswegen wurde ich heute auch fertig mit allen Aufträgen. Ich bin sehr zufrieden mit der Applikation und werde diese bestimmt noch weiter ausbauen, auch nach dem offiziellen Auftrag im ZLI.


