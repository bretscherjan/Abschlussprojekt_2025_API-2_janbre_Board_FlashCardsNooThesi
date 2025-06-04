import { useState, useEffect } from 'react';
import { View, Text, TextInput, StyleSheet, TouchableOpacity, Alert } from 'react-native';
import { router, useLocalSearchParams, Redirect } from 'expo-router';
import AsyncStorage from '@react-native-async-storage/async-storage';
import styles from './styles';
import { goBack } from 'expo-router/build/global-state/routing';
import useKeyboard from './useKeyboard';

export default function CreateDeckScreen() {
  const isKeyboardVisible = useKeyboard();
  const params = useLocalSearchParams();
  const deckId = params.deckId || params.id; // Handle both parameter names

  const [title, setTitle] = useState('');
  const [gradient, setGradient] = useState([]);
  const [existingCards, setExistingCards] = useState([]);
  const isColorSelected = (color) => {
    switch (color) {
      case 'p': return gradient[0] === 'rgba(156, 39, 176, 0.8)';
      case 'b': return gradient[0] === 'rgba(33, 150, 243, 0.8)';
      case 'green': return gradient[0] === 'rgba(76, 175, 80, 0.8)';
      case 'y': return gradient[0] === 'rgba(223, 170, 48, 0.8)';
      case 'r': return gradient[0] === 'rgba(255, 82, 82, 0.8)';
      case 'gray': return gradient[0] === 'rgba(167, 167, 167, 0.8)';
      default: return false;
    }
  };





  // Clear state when component mounts to prevent state persistence issues
  useEffect(() => {
    const resetState = () => {
      setTitle('');
      setGradient([]);
      setExistingCards([]);
    };

    // If we're not editing, reset all state
    if (!deckId) {
      resetState();
    } else if (deckId) {
      loadDeckData();
    }
  }, [deckId]);

  async function loadDeckData() {
    try {
      const storedDecks = await AsyncStorage.getItem('decks');
      if (storedDecks) {
        const decks = JSON.parse(storedDecks);
        const deck = decks.find(d => d.id === deckId);

        if (deck) {
          setTitle(deck.title);
          setGradient(deck.gradientColors || []);
          setExistingCards(deck.cards || []);
        }
      }
    } catch (error) {
      console.error('Loading Error:', error);
    }
  }

  function onTitleChange(newText) {
    setTitle(newText);
  }

  function goBack() {
    // Clear navigation stack and reset params when going back
    router.replace({
      pathname: '/',
      params: {deckId: null }
    });
  }

  function goHome() {
    saveDeck();
    router.push('/');
  }

  function addCards() {
    saveDeck();
    router.push({
      pathname: '/deck/add-card/editCard',
      params: { deckId: deckId }
    });
  }

  function setGradientColor(color) {
    switch (color) {
      case 'p':
        setGradient(['rgba(156, 39, 176, 0.8)', 'rgba(76, 0, 0, 0.3)']);
        break;
      case 'b':
        setGradient(['rgba(33, 150, 243, 0.8)', 'rgba(0, 43, 78, 0.3)']);
        break;
      case 'green':
        setGradient(['rgba(76, 175, 80, 0.8)', 'rgba(0, 74, 2, 0.3)']);
        break;
      case 'y':
        setGradient(['rgba(223, 170, 48, 0.8)', 'rgba(77, 62, 0, 0.3)']);
        break;
      case 'r':
        setGradient(['rgba(255, 82, 82, 0.8)', 'rgba(76, 0, 0, 0.3)']);
        break;
      case 'gray':
        setGradient(['rgba(167, 167, 167, 0.8)', 'rgba(67, 67, 67, 0.3)']);
        break;
      default:
        setGradient(['rgba(255, 255, 255, 0.4)', 'rgba(255, 255, 255, 0.3)']);
        break;
    }
  }

  async function saveDeck() {
    if (!title.trim()) {
      Alert.alert('Error', 'Please provide a deck title.');
      return;
    }

    try {
      const storedDecks = await AsyncStorage.getItem('decks');
      let decks = storedDecks ? JSON.parse(storedDecks) : [];

      const updatedDecks = decks.map(deck => {
        if (deck.id === deckId) {
          return {
            ...deck,
            title: title.trim(),
            gradientColors: gradient,
            cards: existingCards || []
          };
        }
        return deck;
      });

      await AsyncStorage.setItem('decks', JSON.stringify(updatedDecks));
    } catch (error) {
      console.error('Error saving deck:', error);
    }
  }



  return (
    <View style={[styles.container, { justifyContent: 'space-between' }]}>
      <Text style={[styles.title, {top: '2.5%'}]}>
        {'Edit deck'}
      </Text>

      <View style={[styles.form, { top: '40%' }]}>
        <Text style={styles.label}>Deck title</Text>
        <TextInput
          style={styles.input}
          value={title}
          onChangeText={onTitleChange}
          placeholder="ex. Math, English..."
          placeholderTextColor="#707070"
          maxLength={50}
        />

        <Text style={styles.label}>Pick a color:</Text>
        <View style={styles.colorContainer}>
          <TouchableOpacity onPress={() => setGradientColor('p')}>
            <View style={[
              styles.color, 
              { backgroundColor: 'rgba(156, 39, 176, 0.7)' },
              isColorSelected('p') && { opacity: 1, borderWidth: 5, borderColor: 'black' }
            ]} />
          </TouchableOpacity>

          <TouchableOpacity onPress={() => setGradientColor('b')}>
            <View style={[
              styles.color, 
              { backgroundColor: 'rgba(33, 150, 243, 0.7)' },
              isColorSelected('b') && { opacity: 1, borderWidth: 5, borderColor: 'black' }
            ]} />
          </TouchableOpacity>

          <TouchableOpacity onPress={() => setGradientColor('green')}>
            <View style={[
              styles.color, 
              { backgroundColor: 'rgba(76, 175, 80, 0.7)' },
              isColorSelected('green') && { opacity: 1, borderWidth: 5, borderColor: 'black' }
            ]} />
          </TouchableOpacity>

          <TouchableOpacity onPress={() => setGradientColor('y')}>
            <View style={[
              styles.color, 
              { backgroundColor: 'rgba(223, 170, 48, 0.7)' },
              isColorSelected('y') && { opacity: 1, borderWidth: 5, borderColor: 'black' }
            ]} />
          </TouchableOpacity>

          <TouchableOpacity onPress={() => setGradientColor('r')}>
            <View style={[
              styles.color, 
              { backgroundColor: 'rgba(255, 82, 82, 0.7)' },
              isColorSelected('r') && { opacity: 1, borderWidth: 5, borderColor: 'black' }
            ]} />
          </TouchableOpacity>

          <TouchableOpacity onPress={() => setGradientColor('gray')}>
            <View style={[
              styles.color, 
              { backgroundColor: 'rgba(167, 167, 167, 0.7)' },
              isColorSelected('gray') && { opacity: 1, borderWidth: 5, borderColor: 'black' }
            ]} />
          </TouchableOpacity>

        </View>

        <TouchableOpacity style={styles.buttonAdd} onPress={goHome}>
          <Text style={styles.buttonText}>
            {'Save changes and go home'}
          </Text>
        </TouchableOpacity>

        <TouchableOpacity style={styles.buttonAdd} onPress={addCards}>
          <Text style={styles.buttonText}>
            {'Save changes and add cards'}
          </Text>
        </TouchableOpacity>
      </View>

      <TouchableOpacity
        style={[styles.backButtonBg, { display: isKeyboardVisible ? 'none' : 'flex' }]}
        onPress={goBack}
      >
        <Text style={styles.learnBackButtonText}>Cancel</Text>
      </TouchableOpacity>
    </View>
    );
  }
