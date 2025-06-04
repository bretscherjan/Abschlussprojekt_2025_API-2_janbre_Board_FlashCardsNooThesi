# Tag 7

### 1. Suchfunktion in index.tsx & [deckID].tsx

Eine Suchfunktion, um nach Deck-Titel oder Fragen und Antworten von Karten zu suchen.
In beiden Dateien funktioniert die Suchlogik ziemlich ähnlich.

```javascript
{
  openModal && (
    <View pointerEvents="box-none" style={styles.modalInputDetail}>
      <View style={styles.modalContainerDetail}>
        <TouchableOpacity
          style={styles.closeModalDetail}
          onPress={() => {
            setOpenModal(false);
            setSearchTerm("");
            setDeck(originalDeck);
          }}
        >
          <Text style={{ fontSize: 18, color: "#fff" }}>✕</Text>
        </TouchableOpacity>
        <TextInput
          placeholder="Search for a card"
          placeholderTextColor="#999"
          style={styles.searchInputDetail}
          value={searchTerm}
          onChangeText={handleSearch}
          autoFocus={true}
        />
      </View>
    </View>
  );
}
```

Hier wird ein 'Modal' angezeigt. Es ist kein richtiges Modal, weil ich möchte, dass man den Hintergrund immernoch betätigen kann. Bei einem Modal ist dies nicht möglich. Hier wird mit `pointerEvents="box-none"` dies ermöglicht. Wenn sich der Text im Inputfeld ändert, wird diese Funktion aufgerufen:

```javascript
function handleSearch(text) {
  setSearchTerm(text);

  if (!originalDeck) return;
  if (text === "") {
    setDeck(originalDeck);
    return;
  }
  const lowerCaseText = text.toLowerCase();

  // Filtere Karten basierend auf Suchtext
  const filteredCards = originalDeck.cards.filter((card) => {
    if (card.type === "card") {
      return (
        card.question.toLowerCase().includes(lowerCaseText) ||
        card.answer.toLowerCase().includes(lowerCaseText)
      );
    } else if (card.type === "quiz") {
      return (
        card.question.toLowerCase().includes(lowerCaseText) ||
        card.options.some((opt) => opt.toLowerCase().includes(lowerCaseText))
      );
    }
    return false;
  });
  // Erstelle eine Kopie des Decks mit den gefilterten Karten
  setDeck({
    ...originalDeck,
    cards: filteredCards,
  });
}
```

Hier wird der Input aus dem Textfeld mit den Karten-Fragen und Antworten verglichen. Dies alles in `toLowerCase()` damit die Grossschreibung ignoriert wird. Der Hook deck wird nun mit nur den richtigen Karten befüllt. Im Hook `originalDeck` ist das deck gespeichert, wie es am Anfang ist, bevor gesucht wurde. Wenn kein Suchbegriff eingegeben wurde, wird dies angezeigt.

### 2. Export von Karten und Decks

Es gibt viele verschiedene Möglichkeiten, wie man Daten exportieren kann. Man kann entweder ein gesamtes Deck exportieren, beidem der Titel und die Farben und alle Decks gespeichert sind. Dies kann man entweder als JSON oder als CSV exportieren. Weiter kann man die Karten exportieren hier auch wieder entweder JSON oder CSV. Hier kann man jedoch noch unterscheiden, ob man alle Karten aus dem Deck oder nur die markierten (fav = true) exportieren möchte. Insgesamt gibt es also 6 verschiedene Exportmöglichkeiten.

Der ganze Import/Export läuft über eine Page, welche ähnlich aussieht, wie die Übersichtsseite vom Deck. Man hat alle Karten unten aufgelistet und zwei Buttons für Import und Export. Es öffnet sich ein Modal mit den verschiedenen Möglichkeiten. Es wird immer die selbe Funktion aufgerufen, einfach mit unterschiedlichen Parametern.

```javascript
  async function handleExportCards(onlyMarkedCards, isJSON) {
    let exportData = [];
    if (onlyMarkedCards) {
      exportData = deck.cards.filter((card) => card.fav);
    } else {
      exportData = [...deck.cards];
    }
    const exportDataWithFavFalse = exportData.map((card) => ({
      ...card,
      fav: false,
    }));
    if (isJSON) {
      await saveJsonFile(exportDataWithFavFalse, false);
    } else {
      const csvString = convertToCSV(exportDataWithFavFalse, false);
      await saveCsvFile(csvString, false);
    }
  }
```
Hier wird je nach Parameter etwas anderes mitgegeben oder abgefragt. Hier wird mit dem Parameter onlyMarkedCards und isJSON zwischen den Zuständen unterscheidet. Wenn man ein Deck exportieren möchte, wird die Funktion handleExportDeck aufgerufen. Der unterschied ist, dass hier auch die Deckdetails exportiert werden.

```javascript
    async function handleExportDeck(isJSON: boolean) {
        let dataToExport;

        dataToExport = { ...deck, cards: deck.cards.map(card => ({ ...card, fav: false })) };

        if (isJSON) {
            await saveJsonFile(dataToExport, true);
        } else {
            const csvString = convertToCSV(dataToExport, true);
            await saveCsvFile(csvString, true);
        }
    }
```
Wenn man als JSON speichert, wird das JsonFile gedownloaded.

```javascript
    const saveJsonFile = async (data, isDeck) => {
        try {

            let fileName;

            const isSharingAvailable = await Sharing.isAvailableAsync();
            if (!isSharingAvailable) {
                Alert.alert("Fehler", "Das Teilen von Dateien wird auf diesem Gerät nicht unterstützt");
                return;
            }

            if (isDeck) {
                fileName = `${deck?.title || 'deck'}_deck.json`;
            } else {
                fileName = `${deck?.title || 'cards'}_cards.json`
            }

            const fileUri = `${FileSystem.documentDirectory}${fileName}`;
            const jsonString = JSON.stringify(data, null, 2);

            await FileSystem.writeAsStringAsync(fileUri, jsonString, {
                encoding: FileSystem.EncodingType.UTF8,
            });

            await Sharing.shareAsync(fileUri, {
                mimeType: 'application/json',
                dialogTitle: 'exportieren',
                UTI: 'public.json',
            });

            console.log("JSON erfolgreich exportiert:", fileUri);
        } catch (error) {
            console.error("Fehler beim Exportieren:", error);
            Alert.alert("Fehler", "Beim Exportieren ist ein Fehler aufgetreten");
        }
    };
```
Für CSV ist es fast das selbe. Der einzige unterschied ist der Name. Auch hier wird unterschieden, ob man das Deck oder nur die Karten speichert.
Für den CSV-Export wird zuerst die wird die convertToCSV funktion zuerst noch aufgerufen, um von JSON in CSV umzuwandeln. Hier kommen auch zwei unterschiedliche Dateien heraus, je nachdem, wieviele Informationen darin vorhanden sind.
```javascript
    function convertToCSV(data, isDeck) {
        if (data.length === 0) return '';


        if (isDeck) {

            const header = [
                "id",
                "title",
                "alt",
                "gradientColor1",
                "gradientColor2",
                "cardType",
                "fav",
                "question",
                "answer",
                "options",
                "correctAnswerIndex",
            ];
            const csvRows = [header.join(";")];

            const { id, title, alt, gradientColors, cards } = data;
            const gradientColor1 = gradientColors?.[0] || "";
            const gradientColor2 = gradientColors?.[1] || "";

            cards.forEach((card: any) => {
                let row = [id, title, alt, gradientColor1, gradientColor2, card.type, card.fav, card.question, card.answer];
                if (card.type === 'quiz') {
                row.push(card.options ? card.options.join(",") : "");
                row.push(card.correctAnswerIndex !== undefined ? card.correctAnswerIndex : "");
                } else {
                row.push("");
                row.push("");
                }
                csvRows.push(row.join(";"));
            });

            return csvRows.join("\n");

        } else {

            const header = "type;question;correctAnswerIndex;answer;options;fav";

            const csvRows = data.map(item => {
                const fields = {
                    type: item.type || '',
                    question: item.question || '',
                    correctAnswerIndex: item.correctAnswerIndex ?? '',
                    answer: item.answer ?? '',
                    options: item.options ? `${item.options.join(',')}` : '',
                    fav: false
                };

                return [
                    fields.type,
                    fields.question,
                    fields.correctAnswerIndex,
                    fields.answer,
                    fields.options,
                    fields.fav
                ].join(';');
            });

            return header + '\n' + csvRows.join('\n');
        }
    }
```

### 3. Import von Karten und Decks
Für den Import kann man auch entweder einzelne Karten oder ganze Decks importieren. Man hat bei create.tsx die Möglichkeit, ganze Decks richtig importieren, mit Farbe, Titel usw. Bei impExpIndex.tsx kann man einzelne Karten importieren.

Ich wollte dies mit expo-document-picker umsetzen. Auf dem Localhost hat das zu beginn gut funktioniert, jedoch ist die Mobile-Applikation immer beim dieser Funktion abgestürtzt:
```javascript
  const handleImportData = async (isJson: boolean) => {
      const result = await DocumentPicker.getDocumentAsync({
          type: ['text/csv', 'application/json'],
          copyToCacheDirectory: true,
      });
  };
```
Dies ist eine ganz grundsätzliche Funktion mit dem expo Document Picker, welche ein Fenster öffnen sollte, und alle Dateien vom Typ .json angezeigt werden.

### 4. Karten als Favorit markieren



# Fazit


## Erweiterungen
- Vortschritte abspeichern
- Statistik über Vortschritte
- Schreiben von Voci
- Multiplejoise button farbe ändern

- Account auf cloud mit gespeicherten voci
- selber entscheiden ob lokal oder in cloud
- Web UI um cloud-Voci zu lernen

