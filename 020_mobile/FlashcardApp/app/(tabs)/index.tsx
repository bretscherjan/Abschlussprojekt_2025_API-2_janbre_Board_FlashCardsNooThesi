import { useEffect, useState } from 'react';
import { View, Text, StyleSheet, TouchableOpacity, FlatList, Button, Alert, Modal, RefreshControl, ScrollView, TextInput } from 'react-native';
import { router, useFocusEffect, Redirect } from 'expo-router';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { useCallback } from 'react';
import styles from '../styles';
import { LinearGradient } from 'expo-linear-gradient';
import FontAwesome from '@expo/vector-icons/FontAwesome';

// Typ-Definition für ein Deck als Kommentar, da wir JavaScript ohne TypeScript verwenden
// Ein Deck hat: id (String), title (String), color (String), cards (Array von Objekten mit question und answer)

export default function HomeScreen() {
  const [openModal, setOpenModal] = useState(null);
  const [selectedDeck, setSelectedDeck] = useState(null);
  const [decks, setDecks] = useState([]);
  const [originalDeck, setOriginalDeck] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');

  // Lade Decks, wenn der Screen in Fokus kommt
  useFocusEffect(
    useCallback(function () {
      loadDecks();
    }, [])
  );

  async function loadDecks() {
    try {
      const storedDecks = await AsyncStorage.getItem('decks');
      if (storedDecks) {
        const parsedDecks = JSON.parse(storedDecks);
        setDecks(parsedDecks);
        setOriginalDeck(parsedDecks);
      }
    } catch (error) {
      console.error('Loading Error:', error);
    }
  }

  function handleSearch(text) {
    setSearchTerm(text);

    if (!originalDeck) return;

    if (text === '') {
      setDecks(originalDeck);
      return;
    }

    const lowerCaseText = text.toLowerCase();

    // Korrigierte Filterlogik
    const filteredDecks = originalDeck.filter(deck => {
      return deck.title.toLowerCase().includes(lowerCaseText);
    });

    setDecks(filteredDecks);
  }

  // Gehe zum ausgewählten Deck
  function navigateToDeck(id) {
    router.push(`/deck/${id}`);
  }

  function editCards(id) {
    router.push({
      pathname: '/deck/add-card/editCard',
      params: { deckId: id }
    });
  }

  function impExpIndex(id) {
    router.push({
      pathname: '/deck/impExp/impExpIndex',
      params: { deckId: id }
    });
  }

  // Not used! This redirect is used in _layout.tsx
  function navigateToCreate() {
    router.push('/create');
  }

  const infoAlert = (title, id, cardCount) =>
    Alert.alert(title, 'You have ' + cardCount + ' cards in this deck. You really want to delete this deck?', [
      { text: 'EXIT', onPress: () => console.log('EXIT Pressed') },
      {
        text: 'DELETE',
        onPress: () => removeItem(id),
      },
    ]);

  function rename(id) {
    router.replace({
      pathname: '/editdeck',
      params: { deckId: id }
    });
  }

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

  // Rendere jedes Deck als Karte
  function renderDeck(renderItem) {
    var item = renderItem.item;

    var cardCount = item.cards ? item.cards.length : 0;
    if (item.cards) {
      cardCount = item.cards.length;
    }

    return (
      <TouchableOpacity
        style={[styles.deckCard]}
        onPress={function () { navigateToDeck(item.id); }}
        accessibilityLabel={item.alt}
        onLongPress={() => setSelectedDeck(item)}>
        <LinearGradient
          colors={item.gradientColors}
          style={styles.deckCard}
          start={{ x: 0, y: 0 }}
          end={{ x: 1, y: 1 }}
        >
          <Text style={styles.deckTitle}>{item.title}</Text>
          <Text style={styles.cardCount}>{cardCount} Cards</Text>
        </LinearGradient>
      </TouchableOpacity>

    );
  }

  // Extrahiere ID für FlatList keyExtractor
  function getItemKey(item) {
    return item.id;
  }

  // Hilfsfunktion, um den Inhalt je nach Deck-Anzahl zu rendern
  function renderContent() {
    if (decks.length === 0) {
      return (
        <Text style={styles.emptyText}>No decks available. Create a deck!</Text>
      );
    } else {
      return (
        <FlatList
          data={decks}
          renderItem={renderDeck}
          keyExtractor={getItemKey}
          numColumns={2}
          contentContainerStyle={styles.deckGrid}
        />
      );
    }
  }

  return (
    <View style={styles.indexContainer}>
      <Text style={styles.title}>Learning decks</Text>

      {/* Inhalt basierend auf vorhandenen Decks */}
      {renderContent()}

      {openModal && (
        <View pointerEvents="box-none" style={styles.modalInputIndex}>
          <View style={styles.modalContainerIndex}>
            <TextInput
              placeholder="Search for a deck"
              placeholderTextColor="#999"
              style={styles.searchInputIndex}
              value={searchTerm}
              onChangeText={handleSearch}
              autoFocus={true}
            />
            <TouchableOpacity
              style={styles.closeModalIndex}
              onPress={() => {
                setOpenModal(false);
                setSearchTerm('');
                setDecks(originalDeck);}}
            >
              <Text style={{ fontSize: 18, color: '#fff' }}>✕</Text>
            </TouchableOpacity>
          </View>
        </View>
      )}


      {/* Modal innerhalb des return Statements */}
      {selectedDeck && (
        <Modal
          animationType="slide"
          transparent={true}
          visible={selectedDeck !== null}
          onRequestClose={() => {
            setSelectedDeck(null);
          }}
        >
          <View style={styles.centeredView}>
            <View style={styles.modalView}>
              <Text style={styles.modalText}>Deck Settings</Text>
      
              <TouchableOpacity
                style={[styles.button, styles.buttonAdd]}
                onPress={() => {
                  rename(selectedDeck.id);
                  setSelectedDeck(null);
                }}
              >
                <Text style={styles.textStyle}>Edit deck title & color ✎</Text>
              </TouchableOpacity>
      
              <TouchableOpacity
                style={[styles.button, styles.buttonAdd]}
                onPress={() => {
                  editCards(selectedDeck.id);
                  setSelectedDeck(null);
                }}
              >
                <Text style={styles.textStyle}>Edit cards ✎</Text>
              </TouchableOpacity>
      
              <TouchableOpacity
                style={[styles.button, styles.buttonAdd]}
                onPress={() => {
                  impExpIndex(selectedDeck.id);
                  setSelectedDeck(null);
                }}
              >
                <Text style={styles.textStyle}>Import & Export cards ↓↑</Text>
              </TouchableOpacity>
      
              <TouchableOpacity
                style={[styles.button, styles.buttonAdd]}
                onPress={() => {
                  infoAlert(selectedDeck.title, selectedDeck.id, selectedDeck.cards.length);
                  setSelectedDeck(null)}}
              >
                <Text style={styles.textStyle}>Delete deck ✖</Text>
              </TouchableOpacity>
      
              <TouchableOpacity
                style={[styles.backButton]}
                onPress={() => { setSelectedDeck(null)}}
              >
                <Text style={styles.backButtonText}>Close deck-settings</Text>
              </TouchableOpacity>
            </View>
          </View>
        </Modal>
      )}
      <TouchableOpacity
        style={styles.fabUnten}
        onPress={() => setOpenModal(!openModal)}
      >
        <Text style={styles.addButtonText}>🔍︎</Text>
      </TouchableOpacity>
    </View>
  );
}