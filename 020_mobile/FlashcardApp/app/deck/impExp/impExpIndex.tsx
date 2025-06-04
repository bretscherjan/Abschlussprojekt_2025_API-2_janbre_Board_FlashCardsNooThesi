import React from 'react';
import { useEffect, useState } from 'react';
import { Platform, View, Text, StyleSheet, TouchableOpacity, ActivityIndicator, Alert, ScrollView, Modal, TextInput } from 'react-native';
import { useLocalSearchParams, router } from 'expo-router';
import AsyncStorage from '@react-native-async-storage/async-storage';
import styles from '../../styles';
import { LinearGradient } from 'expo-linear-gradient';
import * as FileSystem from 'expo-file-system';
import * as Sharing from 'expo-sharing';
import * as DocumentPicker from 'expo-document-picker';

const DeckDetailScreen = () => {
    const params = useLocalSearchParams();
    const deckId = params.deckId;
    const [deck, setDeck] = useState(null);
    const [data, setData] = useState(null);
    const [originalDeck, setOriginalDeck] = useState(null);
    const [openModalSearch, setOpenModalSearch] = useState(false);
    const [openModalExport, setOpenModalExport] = useState(false);
    const [openModalImport, setOpenModalImport] = useState(false);
    const [loading, setLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');

    interface Card {
        id: string;
        type: 'card' | 'quiz';
        fav: boolean;
        options?: string[];
        correctAnswerIndex?: number;
    }

    interface Deck {
        id: string;
        title: string;
        cards: Card[];
        gradientColors: string[];
    }

    useEffect(function () {
        loadDeckDetails();
    }, [deckId]);

    async function loadDeckDetails() {
        try {
            setLoading(true);
            const storedDecks = await AsyncStorage.getItem('decks');

            if (storedDecks) {
                const decks = JSON.parse(storedDecks);
                const foundDeck = decks.find(d => d.id === deckId);

                if (foundDeck) {
                    setDeck(foundDeck);
                    setOriginalDeck(foundDeck);
                } else {
                    setDeck(null);
                }
            } else {
                setDeck(null);
            }
        } catch (error) {
            console.error('Loading Error:', error);
            setDeck(null);
        } finally {
            setLoading(false);
        }
    }

      // Neue Funktion zum Umschalten und Speichern des Favoriten-Status
  async function toggleFavorite(cardIndex: number) {
    if (!deck) return;
  
    try {
      const storedDecks = await AsyncStorage.getItem('decks');
      if (storedDecks) {
        const decks: Deck[] = JSON.parse(storedDecks);
        const deckIndex = decks.findIndex(d => d.id === deck.id);
        if (deckIndex === -1) return;
  
        // Ändere den fav-Status der Karte am gegebenen Index
        decks[deckIndex].cards[cardIndex].fav = !decks[deckIndex].cards[cardIndex].fav;
  
        await AsyncStorage.setItem('decks', JSON.stringify(decks));
        setDeck(decks[deckIndex]);
        setOriginalDeck(decks[deckIndex]);
      }
    } catch (error) {
      console.error("Fehler beim Speichern des Favoriten-Status:", error);
    }
  }

    function handleSearch(text) {
        setSearchTerm(text);

        if (!originalDeck) return;

        if (text === '') {
            setDeck(originalDeck);
            return;
        }

        const lowerCaseText = text.toLowerCase();

        // Filtere Karten basierend auf Suchtext
        const filteredCards = originalDeck.cards.filter(card => {
            if (card.type === 'card') {
                return card.question.toLowerCase().includes(lowerCaseText) ||
                    card.answer.toLowerCase().includes(lowerCaseText);
            } else if (card.type === 'quiz') {
                return card.question.toLowerCase().includes(lowerCaseText) ||
                    card.options.some(opt => opt.toLowerCase().includes(lowerCaseText));
            }
            return false;
        });

        // Erstelle eine Kopie des Decks mit den gefilterten Karten
        setDeck({
            ...originalDeck,
            cards: filteredCards
        });
    }

    function navigateBack() {
        router.push(`/`);
    }

    async function resetFavorites() {
        try {
            const updatedCards = deck!.cards.map((card: Card) => ({ ...card, fav: false }));
            const updatedDeck: Deck = { ...deck!, cards: updatedCards };
            setDeck(updatedDeck);

            const storedDecks = await AsyncStorage.getItem('decks');
            if (storedDecks) {
                const decks: Deck[] = JSON.parse(storedDecks);
                const updatedDecks = decks.map(d => d.id === updatedDeck.id ? updatedDeck : d);
                await AsyncStorage.setItem('decks', JSON.stringify(updatedDecks));
            }
        } catch (error) {
            console.error("Error resetting favorites:", error);
        }
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Export Cards
    ////////////////////////////////////////////////////////////////////////////////////////////////////////

    async function handleExportCards(onlyMarkedCards, isJSON) {
        let exportData = [];

        if (onlyMarkedCards) {
            exportData = deck.cards.filter(card => card.fav);
        } else {
            exportData = [...deck.cards];
        }

        const exportDataWithFavFalse = exportData.map(card => ({
            ...card,
            fav: false
        }));

        if (isJSON) {
            await saveJsonFile(exportDataWithFavFalse, false);
        } else {
            const csvString = convertToCSV(exportDataWithFavFalse, false);
            await saveCsvFile(csvString, false);
        }
    }



    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Export Deck
    ////////////////////////////////////////////////////////////////////////////////////////////////////////


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


    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Data editing
    ////////////////////////////////////////////////////////////////////////////////////////////////////////


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



    const saveCsvFile = async (csvString, isDeck) => {
        try {
            let fileName;
            const isSharingAvailable = await Sharing.isAvailableAsync();
            if (!isSharingAvailable) {
                Alert.alert("Fehler", "Das Teilen von Dateien wird auf diesem Gerät nicht unterstützt");
                return;
            }

            if (isDeck) {
                fileName = `${deck?.title || 'deck'}_deck.csv`;
            } else {
                fileName = `${deck?.title || 'cards'}_cards.csv`
            }

            const fileUri = `${FileSystem.documentDirectory}${fileName}`;

            await FileSystem.writeAsStringAsync(fileUri, csvString, {
                encoding: FileSystem.EncodingType.UTF8,
            });

            await Sharing.shareAsync(fileUri, {
                mimeType: 'application/csv',
                dialogTitle: 'Karten exportieren',
                UTI: 'public.csv',
            });

            console.log("CSV erfolgreich exportiert:", fileUri);
        } catch (error) {
            console.error("Fehler beim Exportieren:", error);
            Alert.alert("Fehler", "Beim Exportieren ist ein Fehler aufgetreten");
        }
    };







    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Import Cards
    ////////////////////////////////////////////////////////////////////////////////////////////////////////

    const handleImportData = async (isJson: boolean) => {
        try {
            const result = await DocumentPicker.getDocumentAsync({
                type: isJson ? ['application/json'] : ['text/csv'],
                copyToCacheDirectory: true,
            });

            if (result.canceled) {
                Alert.alert("Import Cancelled", "No file was selected.");
                return;
            }

            const { uri, mimeType } = result.assets[0];
            const fileContent = await FileSystem.readAsStringAsync(uri, {
                encoding: FileSystem.EncodingType.UTF8,
            });

            let importedData;

            if (isJson) {
                // Parse JSON
                try {
                    importedData = JSON.parse(fileContent);
                } catch (error) {
                    Alert.alert("Error", "Invalid JSON format.");
                    return;
                }

                // Validate JSON structure
                if (Array.isArray(importedData)) {
                    // Prüfe, ob es ein Array von Karten ist
                    const isValid = importedData.every(item => {
                        if (typeof item !== 'object' || item === null) return false;
                        if (!item.type || !['card', 'quiz'].includes(item.type)) return false;
                        if (typeof item.fav !== 'boolean') return false;
                        if (!item.question || typeof item.question !== 'string') return false;

                        if (item.type === 'card') {
                            if (!item.answer || typeof item.answer !== 'string') return false;
                            // Keine weiteren Properties erlaubt
                            return Object.keys(item).every(key => ['type', 'fav', 'question', 'answer'].includes(key));
                        }

                        if (item.type === 'quiz') {
                            if (!Array.isArray(item.options) || item.options.length === 0 || !item.options.every(opt => typeof opt === 'string')) return false;
                            if (typeof item.correctAnswerIndex !== 'number' || !Number.isInteger(item.correctAnswerIndex) || item.correctAnswerIndex < 0 || item.correctAnswerIndex >= item.options.length) return false;
                            // Keine weiteren Properties erlaubt
                            return Object.keys(item).every(key => ['type', 'fav', 'question', 'options', 'correctAnswerIndex'].includes(key));
                        }

                        return false;
                    });

                    if (!isValid) {
                        Alert.alert("Error", "Invalid card format in JSON file.");
                        return;
                    }

                    // Update deck with imported cards
                    setDeck(prevDeck => ({
                        ...prevDeck,
                        cards: [...(prevDeck?.cards || []), ...importedData]
                    }));
                    setOriginalDeck(prevDeck => ({
                        ...prevDeck,
                        cards: [...(prevDeck?.cards || []), ...importedData]
                    }));

                    // Update AsyncStorage
                    const storedDecks = await AsyncStorage.getItem('decks');
                    const decks = storedDecks ? JSON.parse(storedDecks) : [];
                    const deckIndex = decks.findIndex(d => d.id === deckId);
                    if (deckIndex !== -1) {
                        decks[deckIndex].cards = [...decks[deckIndex].cards, ...importedData];
                        await AsyncStorage.setItem('decks', JSON.stringify(decks));
                    }

                } else {
                    // Wenn es KEIN Array ist, dann ist es KEIN gültiges Karten-JSON
                    Alert.alert("Error", "JSON muss ein Array von Karten sein, kein Deck-Objekt.");
                    return;
                }
            } else {
                // Parse CSV
                const lines = fileContent.split('\n').filter(line => line.trim() !== '');
                if (lines.length < 1) {
                    Alert.alert("Error", "Empty CSV file.");
                    return;
                }

                const header = lines[0].split(';').map(h => h.trim());
                const expectedHeader = ["type", "question", "correctAnswerIndex", "answer", "options", "fav"];
                const isCardImport = header.every((h, i) => h === expectedHeader[i]);

                if (!isCardImport) {
                    Alert.alert("Error", "Invalid CSV header format.");
                    return;
                }

                const importedCards = [];
                for (let i = 1; i < lines.length; i++) {
                    const row = lines[i].split(';').map(cell => cell.trim());
                    if (row.length !== expectedHeader.length) continue;

                    const card = {
                        type: row[0],
                        question: row[1],
                        correctAnswerIndex: row[2] ? parseInt(row[2]) : undefined,
                        answer: row[3],
                        options: row[4] ? row[4].split(',').map(opt => opt.trim()) : undefined,
                        fav: row[5].toLowerCase() === 'true'
                    };

                    if (!['card', 'quiz'].includes(card.type)) continue;
                    if (!card.question) continue;
                    if (card.type === 'card' && !card.answer) continue;
                    if (card.type === 'quiz') {
                        if (!card.options || !card.options.every(opt => typeof opt === 'string')) continue;
                        if (isNaN(card.correctAnswerIndex) || card.correctAnswerIndex < 0 || card.correctAnswerIndex >= card.options.length) continue;
                    }

                    importedCards.push(card);
                }

                if (importedCards.length === 0) {
                    Alert.alert("Error", "No valid cards found in CSV.");
                    return;
                }

                // Update deck with imported cards
                setDeck(prevDeck => ({
                    ...prevDeck,
                    cards: [...(prevDeck?.cards || []), ...importedCards]
                }));
                setOriginalDeck(prevDeck => ({
                    ...prevDeck,
                    cards: [...(prevDeck?.cards || []), ...importedCards]
                }));

                // Update AsyncStorage
                const storedDecks = await AsyncStorage.getItem('decks');
                const decks = storedDecks ? JSON.parse(storedDecks) : [];
                const deckIndex = decks.findIndex(d => d.id === deckId);
                if (deckIndex !== -1) {
                    decks[deckIndex].cards = [...decks[deckIndex].cards, ...importedCards];
                    await AsyncStorage.setItem('decks', JSON.stringify(decks));
                }
            }

            Alert.alert("Success", "Data imported successfully.");
            setOpenModalImport(false);
        } catch (error) {
            console.error("Import error:", error);
            Alert.alert("Error", "An error occurred during import.");
        }
    };


    function renderLoadingState() {
        return (
            <View style={styles.centered}>
                <ActivityIndicator size="large" color="#4a90e2" />
            </View>
        );
    }

    function renderErrorState() {
        return (
            <View style={styles.centered}>
                <Text style={styles.errorText}>Deck not found.</Text>
                <TouchableOpacity
                    style={styles.backButton}
                    onPress={navigateBack}
                >
                    <Text style={styles.backButtonText}>Back</Text>
                </TouchableOpacity>
            </View>
        );
    }

    function renderDeckDetails() {
        return (
            <View style={styles.container}>

                <ScrollView style={{ flex: 1 }}>
                    <LinearGradient
                        colors={deck.gradientColors}
                        style={styles.header}
                        start={{ x: 0, y: 0 }}
                        end={{ x: 1, y: 1 }}
                    >
                        <Text style={styles.title}>{deck.title}</Text>
                        <Text style={styles.subtitle}>
                            {deck.cards.filter(card => card.type === 'card').length} Cards
                        </Text>
                        <Text style={styles.subtitle}>
                            {deck.cards.filter(card => card.type === 'quiz').length} Quiz
                        </Text>
                    </LinearGradient>

                    {openModalSearch && (
                        <View pointerEvents="box-none" style={styles.modalInputDetail}>
                            <View style={styles.modalContainerDetail}>
                                <TouchableOpacity
                                    style={styles.closeModalDetail}
                                    onPress={() => {
                                        setOpenModalSearch(false);
                                        setSearchTerm('');
                                        setDeck(originalDeck);
                                    }}
                                >
                                    <Text style={{ fontSize: 18, color: '#fff' }}>✕</Text>
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
                    )}




                    {/* Export Modal Cards */}
                    {openModalExport && (
                        <Modal
                            animationType="slide"
                            transparent={true}
                            visible={openModalExport !== null}
                            onRequestClose={() => {
                                setOpenModalExport(null);
                            }}
                        >
                            <View style={styles.centeredView}>
                                <View style={styles.modalView}>
                                    <Text style={styles.modalText}>Deck Settings</Text>
                                    <TouchableOpacity
                                        style={[styles.button, styles.buttonAdd]}
                                        onPress={() => {
                                            handleExportCards(false, true);
                                        }}
                                    >
                                        <Text style={styles.textStyle}>Export cards as JSON</Text>
                                    </TouchableOpacity>

                                    <TouchableOpacity
                                        style={[styles.button, styles.buttonAdd]}
                                        onPress={() => {
                                            handleExportCards(true, true);
                                        }}
                                    >
                                        <Text style={styles.textStyle}>Export marked cards as JSON</Text>
                                    </TouchableOpacity>

                                    <TouchableOpacity
                                        style={[styles.button, styles.buttonAdd]}
                                        onPress={() => {
                                            handleExportDeck(true);
                                        }}
                                    >
                                        <Text style={styles.textStyle}>Export deck as JSON</Text>
                                    </TouchableOpacity>

                                    <TouchableOpacity
                                        style={[styles.button, styles.buttonAdd]}
                                        onPress={() => {
                                            handleExportCards(false, false);
                                        }}
                                    >
                                        <Text style={styles.textStyle}>Export cards as CSV</Text>
                                    </TouchableOpacity>

                                    <TouchableOpacity
                                        style={[styles.button, styles.buttonAdd]}
                                        onPress={() => {
                                            handleExportCards(true, false);
                                        }}
                                    >
                                        <Text style={styles.textStyle}>Export marked cards as CSV</Text>
                                    </TouchableOpacity>

                                    <TouchableOpacity
                                        style={[styles.button, styles.buttonAdd]}
                                        onPress={() => {
                                            handleExportDeck(false);
                                        }}
                                    >
                                        <Text style={styles.textStyle}>Export deck as CSV</Text>
                                    </TouchableOpacity>

                                    <TouchableOpacity
                                        style={styles.backButton}
                                        onPress={() => { setOpenModalExport(!openModalExport); }}
                                    >
                                        <Text style={styles.backButtonText}>Cancel</Text>
                                    </TouchableOpacity>
                                </View>
                            </View>
                        </Modal>
                    )}


                    {/* Import Modal */}
                    {openModalImport && (
                        <Modal
                            animationType="slide"
                            transparent={true}
                            visible={openModalImport !== null}
                            onRequestClose={() => {
                                setOpenModalImport(null);
                            }}
                        >
                            <View style={styles.centeredView}>
                                <View style={styles.modalView}>
                                    <Text style={styles.modalText}>Deck Settings</Text>
                                    <TouchableOpacity
                                        style={[styles.button, styles.buttonAdd]}
                                        onPress={() => {
                                            handleImportData(true);
                                        }}
                                    >
                                        <Text style={styles.textStyle}>Import JSON cards</Text>
                                    </TouchableOpacity>

                                    <TouchableOpacity
                                        style={[styles.button, styles.buttonAdd]}
                                        onPress={() => {
                                            handleImportData(false);
                                        }}
                                    >
                                        <Text style={styles.textStyle}>Import CSV cards</Text>
                                    </TouchableOpacity>

                                    <TouchableOpacity
                                        style={styles.backButton}
                                        onPress={() => { setOpenModalImport(!openModalImport); }}
                                    >
                                        <Text style={styles.backButtonText}>Cancel</Text>
                                    </TouchableOpacity>
                                </View>
                            </View>
                        </Modal>
                    )}

                    <TouchableOpacity
                        style={styles.startButton}
                        onPress={() => { setOpenModalExport(!openModalExport); }}
                    >
                        <Text style={styles.buttonText}>Export Data ↑</Text>
                    </TouchableOpacity>

                    <TouchableOpacity
                        style={styles.startButton}
                        onPress={() => { setOpenModalImport(!openModalImport); }}
                    >
                        <Text style={styles.buttonText}>Import Data ↓</Text>
                    </TouchableOpacity>

                    <TouchableOpacity
                        style={styles.backButton}
                        onPress={navigateBack}
                    >
                        <Text style={styles.backButtonText}>Back to overview</Text>
                    </TouchableOpacity>

                    {deck.cards?.length > 0 ? (
                        deck.cards.map((card, index) => (
                            <View key={index} style={styles.ListCardContainer}>
                                {card.type === 'card' && (
                                    <>
                                        <TouchableOpacity
                                            style={styles.favIconButton}
                                            onPress={() => toggleFavorite(index)}
                                        >
                                            {card.fav === true && (
                                                <Text style={styles.favIcon}>★</Text>
                                            )}
                                            {card.fav === false && (
                                                <Text style={styles.favIcon}>☆</Text>
                                            )}
                                        </TouchableOpacity>
                                        <Text style={styles.listQuestion}>{card.question}</Text>
                                        <Text style={styles.listAnswer}>{card.answer}</Text>
                                    </>
                                )}
                                {card.type === 'quiz' && (
                                    <>
                                        <TouchableOpacity
                                            style={styles.favIconButton}
                                            onPress={() => toggleFavorite(index)}
                                        >
                                            {card.fav === true && (
                                                <Text style={styles.favIcon}>★</Text>
                                            )}
                                            {card.fav === false && (
                                                <Text style={styles.favIcon}>☆</Text>
                                            )}
                                        </TouchableOpacity>
                                        <Text style={styles.listQuestion}>{card.question}</Text>
                                        {card.options.map((option, optIndex) => (
                                            <Text key={optIndex} style={styles.listOption}>{option}</Text>
                                        ))}
                                        <Text style={styles.listCorrectAnswer}>
                                            Correct Answer: {card.options[card.correctAnswerIndex]}
                                        </Text>
                                    </>
                                )}
                            </View>
                        ))
                    ) : (
                        <View style={styles.centered}>
                            <Text style={styles.errorText}>No cards found</Text>
                        </View>
                    )}
                    <TouchableOpacity
                        style={styles.backButton}
                        onPress={resetFavorites}
                    >
                        <Text style={styles.backButtonText}>Reset all Favorites</Text>
                    </TouchableOpacity>
                </ScrollView>

                <TouchableOpacity
                    style={styles.fabUnten}
                    onPress={() => setOpenModalSearch(!openModalSearch)}
                >
                    <Text style={styles.addButtonText}>🔍︎</Text>
                </TouchableOpacity>
            </View>
        );
    }

    if (loading) {
        return renderLoadingState();
    } else if (!deck) {
        return renderErrorState();
    } else {
        return renderDeckDetails();
    }
}

export default DeckDetailScreen;