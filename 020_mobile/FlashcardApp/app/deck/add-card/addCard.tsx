//////////////////////////////////// NOT USED /////////////////////////////////////////////
// This file is not used in the app. editCard.tsx is used instead.
//////////////////////////////////// NOT USED /////////////////////////////////////////////





import { useEffect, useState } from 'react';
import { View, Text, TouchableOpacity, ActivityIndicator, TextInput, ScrollView } from 'react-native';
import { useLocalSearchParams, router } from 'expo-router';
import AsyncStorage from '@react-native-async-storage/async-storage';
import styles from '../../styles';
import { LinearGradient } from 'expo-linear-gradient';

export default function AddCardScreen() {
  const params = useLocalSearchParams();
  const deckId = params.deckId; // Changed from deckID to deckId to match URL parameter
  const [deck, setDeck] = useState(null);
  const [loading, setLoading] = useState(true);
  const [question, setQuestion] = useState('');
  const [answer, setAnswer] = useState('');

  useEffect(() => {
    loadDeckDetails();
  }, [deckId]);

  async function loadDeckDetails() {
    try {
      setLoading(true);
      // Load all decks
      const storedDecks = await AsyncStorage.getItem('decks');

      if (storedDecks) {
        const decks = JSON.parse(storedDecks);
        // Find the deck with the matching ID
        const foundDeck = decks.find(deck => deck.id === deckId);
        setDeck(foundDeck);
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

  function navigateBack() {
    router.push(`/deck/${deckId}`);
  }

  async function saveCard() {
    if (!question.trim() || !answer.trim()) {
      // Alert user that both fields are required
      alert('Please enter both question and answer');
      return;
    }

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
        router.push(`/deck/${deckId}`);
      }
    } catch (error) {
      console.error('Save Error:', error);
      alert('Failed to save card. Please try again.');
    }
  }

  // Render functions for different states
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

  function renderAddCardForm() {
    return (
      <View style={[styles.container, {justifyContent: 'space-between',}]}>
        <LinearGradient
          colors={deck.gradientColors}
          style={styles.header}
          start={{ x: 0, y: 0 }}
          end={{ x: 1, y: 1 }}
        >
          <Text style={styles.title}>{deck.title}</Text>
          <Text style={styles.subtitle}>Add New Card</Text>
        </LinearGradient>

        <ScrollView style={styles.form}>
          <Text style={styles.label}>Question</Text>
          <TextInput
            style={styles.input}
            value={question}
            onChangeText={setQuestion}
            placeholder="Enter your question here"
            placeholderTextColor="#707070"
            multiline
          />

          <Text style={styles.label}>Answer</Text>
          <TextInput
            style={styles.input}
            value={answer}
            onChangeText={setAnswer}
            placeholder="Enter the answer here"
            placeholderTextColor="#707070"
            multiline
          />

          <TouchableOpacity style={styles.buttonAdd} onPress={saveCard}>
            <Text style={styles.buttonText}>Save Card</Text>
          </TouchableOpacity>
        </ScrollView>

        <TouchableOpacity
          style={[styles.backButton, { bottom: 3 }]}
          onPress={navigateBack}
        >
          <Text style={[styles.learnBackButtonText]}>Cancel</Text>
        </TouchableOpacity>
      </View>
    );
  }

  // Main render function
  if (loading) {
    return renderLoadingState();
  } else if (!deck) {
    return renderErrorState();
  } else {
    return renderAddCardForm();
  }
}