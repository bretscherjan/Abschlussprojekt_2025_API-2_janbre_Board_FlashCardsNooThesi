# Tag 4

### 1. Navigation mit Tabs

- Neue Ordnerstruktur erstllen:
- ![image](https://github.com/user-attachments/assets/8e6772cf-7c35-4377-b3e9-e72a4e5555d1)


- _layout.tsx anpassen für die Bottom Navigation
Die Farben definieren und den Header ausblenden
```
screenOptions={{
  headerShown: false,
  tabBarShowLabel: false,
  tabBarActiveTintColor: 'rgba(223, 170, 48, 0.7)',
  tabBarInactiveTintColor: 'rgba(223, 170, 48, 0.3)',
  tabBarStyle: {
    backgroundColor: 'rgba(30, 30, 30, 1)',
    borderTopColor: 'rgba(223, 170, 48, 0.1)',
    height: 56,
    paddingTop: 14,
  },
}}
```
Beispiel vom Home-Icon
```
<Tabs.Screen
  name="index"
  options={{
    tabBarIcon: ({ color }) => (
      <FontAwesome name="home" size={32} color={color} />
    ),
  }}
/>
```
Grundsätzliche Struktur vom _layout.tsx
```
// app/(tabs)/_layout.tsx
import { Tabs } from 'expo-router';
import FontAwesome from '@expo/vector-icons/FontAwesome';
export default function TabLayout() {
  return (
    <Tabs
      screenOptions={{ ... }} >
      <Tabs.Screen
        name="index"
        options={{ ... }} />
    </Tabs>
  );
}
```
- Die 2 _layout.tsx sind, damit nicht alle verfügbaren Dateien in der Bottom Navigation angezeigt werden, sondern nur Home und Create.

### 2. Style anpassen
- Ich habe zuvor für jede Seite ein einzelnes style.ts File verwendet. Nun habe ich alle zusammengefasst, dass ich ein Stylesheet habe. Dies hat den Vorteil, dass der Style über die gesamte App gleich bleibt und konsistent alles gleich bleibt.

### 3. Karten hinzufügen
- Im Ordner /deck/add-card/ eine Datei addCard.tsx anlegen
- Eine Route zwischen [deckID].tsx und addCard.tsx erstellen
- Zwei input Felder für die Frage und die Antwort für die Karte
```
<TextInput
  style={styles.input}
  value={question}
  onChangeText={setQuestion}
  placeholder="Enter your question here"
  placeholderTextColor="#707070"
  multiline
/>
```
- Der Input wird in Hooks gespeichert
```
const [question, setQuestion] = useState('');
const [answer, setAnswer] = useState('');
```
- Speichere die Kartendaten im Deck
```
try {
   // Load current decks
   const storedDecks = await AsyncStorage.getItem('decks');
   if (storedDecks) {
     const decks = JSON.parse(storedDecks);
     // Find and update the specific deck
     const updatedDecks = decks.map(d => {
       if (d.id === deckId) {
         // Create a new card
         const newCard = {
           question: question.trim(),
           answer: answer.trim()
         };
         // Add the new card to the deck's cards array
         return {
           ...d,
           cards: [...(d.cards || []), newCard]
         };
       }
       return d;
     });
     // Save updated decks back to storage
     await AsyncStorage.setItem('decks', JSON.stringify(updatedDecks));
     // Go back to the deck detail screen
     router.back();
   }
```

# Fazit

Es nervt mich immernoch, dass ich nicht mit Expo Go arbeiten kann. Ich habe bereits viel Zeit investiert, um mit dem Handy zu verbinden, jedoch leider ohne erfolg. Ich konzentrierte mich heute (zu lange) auf das Swipen der Karten konzentriert. Diese Funktion war nicht Teil des Auftrages. Es hat nicht funktioniert. Ich habe somit 1/4 Tag verschwendet. Im weiteren Verlauf befasste ich mich mit den Aufgaben und löste alle Aufträge von heute. Ich war den ganzen Tag gut beschäftigt und mir wurde nie langweilig. Mein Perfektionismus kam mir wieder einmal in die Quere. Ich bin lange im Detail geschwommen und habe somit wieder viel Zeit verloren. Die AddCard Funktion war eine grosse Herausforderung für mich. Es funktioniert nun und ich bin sehr zufrieden mit dem Ergebnis.
