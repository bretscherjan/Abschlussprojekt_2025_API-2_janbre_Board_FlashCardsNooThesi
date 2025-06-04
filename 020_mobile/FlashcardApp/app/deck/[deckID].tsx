import { useEffect, useState } from 'react';
import { View, Text, StyleSheet, TouchableOpacity, ActivityIndicator, Alert, ScrollView, Modal, TextInput } from 'react-native';
import { useLocalSearchParams, router } from 'expo-router';
import AsyncStorage from '@react-native-async-storage/async-storage';
import styles from '../styles';
import { LinearGradient } from 'expo-linear-gradient';

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

export default function DeckDetailScreen() {
  const params = useLocalSearchParams();
  const deckId = params.deckID;
  const [deck, setDeck] = useState(null);
  const [originalDeck, setOriginalDeck] = useState(null);
  const [openModal, setOpenModal] = useState(false);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');

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
        setDeck(foundDeck);
        setOriginalDeck(foundDeck);
      }
    } catch (error) {
      console.error("Error loading deck details:", error);
    } finally {
      setLoading(false);
    }
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

  function handleStartLearning() {
    router.push(`/deck/learn/${deck.id}`);
  }

  function navigateToCreateCards() {
    router.push({
      pathname: '/deck/add-card/editCard',
      params: { deckId: deck.id }
    });
  }

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

        {openModal && (
          <View pointerEvents="box-none" style={styles.modalInputDetail}>
            <View style={styles.modalContainerDetail}>
              <TouchableOpacity
                style={styles.closeModalDetail}
                onPress={() => {
                  setOpenModal(false);
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

        <TouchableOpacity
          style={styles.startButton}
          onPress={handleStartLearning}
        >
          <Text style={styles.buttonText}>Start learning</Text>
        </TouchableOpacity>

        <TouchableOpacity
          style={styles.backButton}
          onPress={navigateBack}
        >
          <Text style={styles.backButtonText}>Back to overview</Text>
        </TouchableOpacity>

        <ScrollView style={{ flex: 1 }}>
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
          style={styles.fabOben}
          onPress={() => setOpenModal(!openModal)}
        >
          <Text style={styles.addButtonText}>🔍︎</Text>
        </TouchableOpacity>
        <TouchableOpacity
          style={styles.fabUnten}
          onPress={navigateToCreateCards}
        >
          <Text style={styles.addButtonText}>✎</Text>
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