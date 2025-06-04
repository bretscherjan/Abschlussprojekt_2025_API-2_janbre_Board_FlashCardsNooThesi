# Tag 6

### 1. Automatische Farbe
- Ich setze die default color hier ```gradientColors: gradient.length >= 2 ? gradient : getRandomColor(),```. wenn im array gradient weniger als 2 Fraben sind, dann wird die Funktion getRandomColor ausgeführt.
- In dieser Funktion wird eine Random Farbkombination ausgewählt und zurückgegeben
```
function getRandomColor() {
  const colors = [
    ['rgba(156, 39, 176, 0.8)', 'rgba(76, 0, 0, 0.3)'], // Purple gradient
    ['rgba(33, 150, 243, 0.8)', 'rgba(0, 43, 78, 0.3)'], // Blue gradient
    ['rgba(76, 175, 80, 0.8)', 'rgba(0, 74, 2, 0.3)'], // Green gradient
    ['rgba(223, 170, 48, 0.8)', 'rgba(77, 62, 0, 0.3)'], // Yellow gradient
    ['rgba(255, 82, 82, 0.8)', 'rgba(76, 0, 0, 0.3)'], // Red gradient
    ['rgba(167, 167, 167, 0.8)', 'rgba(67, 67, 67, 0.3)'] // Gray gradient
  ];
  return colors[Math.floor(Math.random() * colors.length)];
}
```

### 2. Modal für Deck-Settings
- Zuerst Modal von react-native```import { Modal } from 'react-native';``` importieren.
- Einen Hook für die Visibility erstellen ```const [modalVisible, setModalVisible] = useState(false);```
- Das Modal in den Code einbinden
```
<TouchableOpacity
  style={[styles.deckCard]}
  onPress={function () { navigateToDeck(item.id); }}
  onLongPress={() => setModalVisible(true)}
  accessibilityLabel={item.alt}>
    <Modal
    animationType="slide"
    transparent={true}
    visible={modalVisible}
    onRequestClose={() => {
        Alert.alert('Modal has been closed.');
        setModalVisible(!modalVisible);
    }}>
        <View style={styles.centeredView}>
            <View style={styles.modalView}>
            <Text style={styles.modalText}>Deck Settings</Text>
            <TouchableOpacity
                style={[styles.button, styles.buttonAdd]}
                onPress={() => rename(item.id)}>
                <Text style={styles.textStyle}>Edit deck title & color ✏️</Text>
            </TouchableOpacity>
            <TouchableOpacity
                style={[styles.button, styles.buttonAdd]}
                onPress={() => editCards(item.id)}>
                <Text style={styles.textStyle}>Edit cards ✏️</Text>
            </TouchableOpacity>
            <TouchableOpacity
                style={[styles.button, styles.buttonAdd]}
                onPress={() => infoAlert(item.title, item.id, cardCount)}>
                <Text style={styles.textStyle}>Delete deck ❌</Text>
            </TouchableOpacity>
            <TouchableOpacity
                style={[styles.backButton]}
                onPress={() => setModalVisible(!modalVisible)}>
                <Text style={styles.backButtonText}>Close deck-settings</Text>
            </TouchableOpacity>
            </View>
        </View>
    </Modal>
</TouchableOpacity>
```
- onLongPress auf das Deck wird das Modal sichtbar.
- Verschiedene Buttons, welche auf verschiedene Pfade weiterleiten für das Bearbeiten des Decks, der Karten und um das Deck zu löschen.
- Mit dem Close deck-settings Button wird das Modal geschlossen (visibility = false).

### 3. Karten bearbeiten
- addCard.tsx wurde ausserbetrieb genommen. Anstelle gibt es nun editCard.tsx. Hier kann man Karten hinzufügen, löschen und bearbeiten.
- Alle Karten untereinander darstellen mit ScrollView.
- Es werden hier alle Daten aus dem Array editingCards dargestellt. Für jede Karte hat es einen Button um sie zu löschen. Ganz unten hat es einen um alles zu speichern oder eine neue Karte hinzuzufügen.
```
<ScrollView style={styles.formEdit}>
  {editingCards.map((card, index) => (
    <View key={index} style={{ marginBottom: 20, borderBottomWidth: 1, borderBottomColor: '#444', paddingBottom: 10 }}>
      <Text style={styles.label}>Card {index + 1}</Text>

      <Text style={styles.label}>Question</Text>
      <TextInput
        style={styles.input}
        value={card.question}
        onChangeText={(text) => handleCardChange(index, 'question', text)}
        placeholder="Enter question"
        placeholderTextColor="#707070"
        multiline
      />
      <Text style={styles.label}>Answer</Text>
      <TextInput
        style={styles.input}
        value={card.answer}
        onChangeText={(text) => handleCardChange(index, 'answer', text)}
        placeholder="Enter answer"
        placeholderTextColor="#707070"
        multiline
      />
      <TouchableOpacity 
        style={[styles.buttonAdd, {backgroundColor: '#ff4d4d', marginTop: 5}]}
        onPress={() => removeCard(index)}
      >
        <Text style={styles.buttonText}>Remove Card</Text>
      </TouchableOpacity>
    </View>
  ))}
  <TouchableOpacity 
    style={[styles.buttonAdd, {marginTop: 10}]}
    onPress={addNewCard}
  >
    <Text style={styles.buttonText}>Add New Card</Text>
  </TouchableOpacity>
  <TouchableOpacity 
    style={[styles.buttonAdd, {marginTop: 20, backgroundColor: '#4CAF50', bottom: 20}]}
    onPress={saveChanges}
  >
    <Text style={styles.buttonText}>Save All Changes</Text>
  </TouchableOpacity>
</ScrollView>
```
- Die änderungen werden gespeichert. Mit setEditingCards werden die neuen Karten gespeichert.
```
  function handleCardChange(index, field, value) {
    const updatedCards = [...editingCards];
    updatedCards[index] = {
      ...updatedCards[index],
      [field]: value
    };
    setEditingCards(updatedCards);
  }
```

# Fazit
Ich finde Modale sind sehr tolle Elemente für eine solche App. Ich habe nicht alles wie im Auftrag beschrieben gleich umgesetzt, ich habe jedoch alles ähnlich gemacht, sodass es mir gefällt. Ich bin zufrieden mit der App und bin heute auch gut weitergekommen.

