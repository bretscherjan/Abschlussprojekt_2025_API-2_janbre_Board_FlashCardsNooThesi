import { useEffect, useState } from 'react';
import { View, Text, TouchableOpacity, ActivityIndicator, TextInput, Alert, ScrollView } from 'react-native';
import { useLocalSearchParams, router } from 'expo-router';
import AsyncStorage from '@react-native-async-storage/async-storage';
import styles from '../../styles';
import { LinearGradient } from 'expo-linear-gradient';
import useKeyboard from '../../useKeyboard';

export default function EditCardScreen() {
  const isKeyboardVisible = useKeyboard();
  const params = useLocalSearchParams();
  const deckId = params.deckId;
  const [deck, setDeck] = useState(null);
  const [loading, setLoading] = useState(true);
  const [editingCards, setEditingCards] = useState([]);

  useEffect(() => {
    loadDeckDetails();
  }, [deckId]);

  async function loadDeckDetails() {
    try {
      setLoading(true);
      const storedDecks = await AsyncStorage.getItem('decks');

      if (storedDecks) {
        const decks = JSON.parse(storedDecks);
        const foundDeck = decks.find(deck => deck.id === deckId);
        if (foundDeck) {
          setDeck(foundDeck);
          // Erstelle eine bearbeitbare Kopie aller Karten
          setEditingCards([...foundDeck.cards]);
        }
      }
    } catch (error) {
      console.error('Loading Error:', error);
    } finally {
      setLoading(false);
    }
  }

  function navigateBack() {
    router.push(`/deck/${deckId}`);
  }

  function impExpIndex(id) {
    router.push({
      pathname: '../impExp/impExpIndex',
      params: { deckId: id }
    });
  }

  function handleCardChange(index, field, value) {
    const updatedCards = [...editingCards];
    updatedCards[index] = {
      ...updatedCards[index],
      [field]: value
    };
    setEditingCards(updatedCards);
  }

  async function saveChanges() {
    const hasEmptyFields = editingCards.some(card => {
      if (card.type === 'quiz') {
        return (
          !card.question.trim() ||
          !card.options.every(opt => opt.trim()) ||
          card.correctAnswerIndex === undefined
        );
      }
      return !card.question.trim() || !card.answer.trim();
    });
  
    if (hasEmptyFields) {
      Alert.alert('Error', 'All questions and answers are required');
      return;
    }
  
    try {
      let updatedDecks = [];
      const storedDecks = await AsyncStorage.getItem('decks');
      if (storedDecks) {
        updatedDecks = JSON.parse(storedDecks);
      }
  
      const deckExists = updatedDecks.some(d => d.id === deckId);
      if (!deckExists) {
        Alert.alert('Error', 'Deck not found');
        return;
      }
  
      updatedDecks = updatedDecks.map(d => {
        if (d.id === deckId) {
          return { ...d, cards: editingCards };
        }
        return d;
      });
  
      await AsyncStorage.setItem('decks', JSON.stringify(updatedDecks));
      Alert.alert('Success', 'All changes saved successfully');
      await loadDeckDetails(); // Stelle sicher, dass dies asynchron ist
    } catch (error) {
      console.error('Save Error:', error);
      if (error instanceof SyntaxError) {
        Alert.alert('Error', 'Failed to parse stored data');
      } else if (error.message.includes('AsyncStorage')) {
        Alert.alert('Error', 'Storage access failed');
      } else {
        Alert.alert('Error', 'An unexpected error occurred');
      }
    }
  }

  function addNewCard() {
    setEditingCards([
      ...editingCards,
      { type: 'card', fav: true, question: '', answer: '' }
    ]);
  }

  function addNewQuiz() {
    setEditingCards([
      ...editingCards,
      {
        type: 'quiz',
        fav: false,
        question: '',
        options: ['', '', '', ''],
        correctAnswerIndex: 0
      }
    ]);
  }


  function removeCard(index) {
    const updatedCards = editingCards.filter((_, i) => i !== index);
    setEditingCards(updatedCards);
  }

  function renderLoadingState() {
    return (
      <View style={styles.centered}>
        <ActivityIndicator size="large" color="#4a90e2" />
      </View>
    );
  }

/*
  function renderErrorState() {
    return (
      <View style={styles.centered}>
        <Text style={styles.errorText}>Deck not found or has no cards.</Text>
        <TouchableOpacity
          style={styles.backButton}
          onPress={navigateBack}
        >
          <Text style={styles.backButtonText}>Back</Text>
        </TouchableOpacity>
      </View>
    );
  }
*/

  function renderEditCardsForm() {
    return (
      <View style={[styles.container, {justifyContent: 'space-between'}]}>
        <LinearGradient
          colors={deck.gradientColors}
          style={styles.header}
          start={{ x: 0, y: 0 }}
          end={{ x: 1, y: 1 }}
        >
          <Text style={styles.title}>{deck.title}</Text>
          <Text style={styles.subtitle}>Edit {editingCards.length} Cards</Text>
        </LinearGradient>

        <ScrollView style={styles.formEdit}>
          {editingCards.map((card, index) => (
            <View key={index} style={{ marginBottom: 20, borderBottomWidth: 1, borderBottomColor: '#444', paddingBottom: 10 }}>
              <Text style={styles.label}>Card {index + 1} ({card.type})</Text>

              <Text style={styles.label}>Question</Text>
              <TextInput
                style={styles.input}
                value={card.question}
                onChangeText={(text) => handleCardChange(index, 'question', text)}
                placeholder="Enter question"
                placeholderTextColor="#707070"
                multiline
              />

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

              {card.type === 'quiz' && (
                <>
                  <Text style={styles.label}>Options</Text>
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

                  <Text style={styles.label}>Correct Answer (Index)</Text>
                  <TextInput
                    style={styles.input}
                    keyboardType="numeric"
                    value={card.correctAnswerIndex?.toString()}
                    onChangeText={(text) => handleCardChange(index, 'correctAnswerIndex', (parseInt(text)) || 0)}
                    placeholder="Correct option index (0–3)"
                    placeholderTextColor="#707070"
                  />
                </>
              )}

              <TouchableOpacity
                style={[styles.buttonAdd, { backgroundColor: '#ff4d4d', marginTop: 5 }]}
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
            style={[styles.buttonAdd, {marginTop: 10}]}
            onPress={addNewQuiz}
          >
            <Text style={styles.buttonText}>Add New Quiz</Text>
          </TouchableOpacity>

          <TouchableOpacity 
            style={[styles.buttonAdd, {marginTop: 20, backgroundColor: '#4CAF50', bottom: 20}]}
            onPress={saveChanges}
          >
            <Text style={styles.buttonText}>Save All Changes</Text>
          </TouchableOpacity>
        </ScrollView>

        <TouchableOpacity
          style={[styles.backButtonBg, { display: isKeyboardVisible ? 'none' : 'flex' }]}
          onPress={navigateBack}
        >
          <Text style={styles.learnBackButtonText}>Back to Deck</Text>
        </TouchableOpacity>
        <TouchableOpacity
          style={styles.fabUnten}
          onPress={() => impExpIndex(deck.id)}
        >
          <Text style={styles.addButtonText}>↓↑</Text>
      </TouchableOpacity>
      </View>
    );
  }

  if (loading) {
    return renderLoadingState();
  } else if (!deck || !deck.cards || deck.cards.length === 0) {
    return renderEditCardsForm();
  } else {
    return renderEditCardsForm();
  }
}