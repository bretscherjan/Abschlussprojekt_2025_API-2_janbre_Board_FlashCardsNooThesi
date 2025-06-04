import React from 'react';
import { useState, useEffect } from 'react';
import { View, Text, TextInput, StyleSheet, TouchableOpacity, Alert, RefreshControl } from 'react-native';
import { router, useLocalSearchParams, Redirect } from 'expo-router';
import AsyncStorage from '@react-native-async-storage/async-storage';
import * as DocumentPicker from 'expo-document-picker';
import * as FileSystem from 'expo-file-system';
import styles from '../styles';
import useKeyboard from '../useKeyboard';

export default function CreateDeckScreen() {
  const isKeyboardVisible = useKeyboard();
  const [title, setTitle] = useState('');
  const [gradient, setGradient] = useState([]);

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

  function onTitleChange(newText) {
    setTitle(newText);
  }

  function goBack() {
    router.replace('/');
  }

  function goHome() {
    saveDeck();
    router.replace('/');
  }

  async function addCards() {
    const newDeck = await saveDeck();
    router.replace({
      pathname: '/deck/add-card/editCard',
      params: { deckId: newDeck.id }
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

      const newDeck = {
        id: Date.now().toString(),
        title: title.trim(),
        alt: title.trim(),
        gradientColors: gradient.length >= 2 ? gradient : getRandomColor(),
        cards: []
      };

      decks.push(newDeck);
      await AsyncStorage.setItem('decks', JSON.stringify(decks));
      return newDeck;
    } catch (error) {
      console.error('Error saving deck:', error);
      Alert.alert('Error', 'Failed to save deck.');
    }
  }

  async function handleImportDeck(isJson: boolean) {
    try {
      const result = await DocumentPicker.getDocumentAsync({
        type: isJson ? ['application/json'] : ['text/csv'],
        copyToCacheDirectory: true,
      });

      if (result.canceled) {
        Alert.alert('Import Cancelled', 'No file was selected.');
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
          Alert.alert('Error', 'Invalid JSON format.');
          return;
        }

        // Validate JSON deck structure
        if (!importedData.id || typeof importedData.id !== 'string') {
          Alert.alert('Error', 'Invalid or missing deck ID in JSON.');
          return;
        }
        if (!importedData.title || typeof importedData.title !== 'string') {
          Alert.alert('Error', 'Invalid or missing deck title in JSON.');
          return;
        }
        if (!importedData.alt || typeof importedData.alt !== 'string') {
          Alert.alert('Error', 'Invalid or missing deck alt in JSON.');
          return;
        }
        if (!Array.isArray(importedData.gradientColors) || importedData.gradientColors.length < 2) {
          Alert.alert('Error', 'Invalid or missing gradientColors in JSON.');
          return;
        }
        if (!Array.isArray(importedData.cards)) {
          Alert.alert('Error', 'Invalid or missing cards array in JSON.');
          return;
        }

        // Validate cards
        const isValidDeck = importedData.cards.every(card => {
          if (!card.type || !['card', 'quiz'].includes(card.type)) return false;
          if (!card.question || typeof card.question !== 'string') return false;
          if (card.type === 'card' && (!card.answer || typeof card.answer !== 'string')) return false;
          if (card.type === 'quiz') {
            if (!Array.isArray(card.options) || !card.options.every(opt => typeof opt === 'string')) return false;
            if (typeof card.correctAnswerIndex !== 'number' || card.correctAnswerIndex < 0 || card.correctAnswerIndex >= card.options.length) return false;
          }
          if (typeof card.fav !== 'boolean') return false;
          return true;
        });

        if (!isValidDeck) {
          Alert.alert('Error', 'Invalid card format in JSON file.');
          return;
        }
      } else {
        // Parse CSV
        const lines = fileContent.split('\n').filter(line => line.trim() !== '');
        if (lines.length < 1) {
          Alert.alert('Error', 'Empty CSV file.');
          return;
        }

        const header = lines[0].split(';').map(h => h.trim());
        const expectedHeader = [
          'id',
          'title',
          'alt',
          'gradientColor1',
          'gradientColor2',
          'cardType',
          'fav',
          'question',
          'answer',
          'options',
          'correctAnswerIndex'
        ];
        const isValidHeader = header.every((h, i) => h === expectedHeader[i]);

        if (!isValidHeader) {
          Alert.alert('Error', 'Invalid CSV header format.');
          return;
        }

        const importedCards = [];
        let deckId, deckTitle, deckAlt, deckGradientColors;

        for (let i = 1; i < lines.length; i++) {
          const row = lines[i].split(';').map(cell => cell.trim());
          if (row.length !== expectedHeader.length) continue;

          if (!deckId) deckId = row[0];
          if (!deckTitle) deckTitle = row[1];
          if (!deckAlt) deckAlt = row[2];
          if (!deckGradientColors) deckGradientColors = [row[3], row[4]];

          const card = {
            type: row[5],
            fav: row[6].toLowerCase() === 'true',
            question: row[7],
            answer: row[8] || undefined,
            options: row[9] ? row[9].split(',').map(opt => opt.trim()) : undefined,
            correctAnswerIndex: row[10] ? parseInt(row[10]) : undefined
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
          Alert.alert('Error', 'No valid cards found in CSV.');
          return;
        }
        if (!deckId || !deckTitle || !deckAlt || !deckGradientColors) {
          Alert.alert('Error', 'Missing deck metadata in CSV.');
          return;
        }

        importedData = {
          id: deckId,
          title: deckTitle,
          alt: deckAlt,
          gradientColors: deckGradientColors,
          cards: importedCards
        };
      }

      // Save the imported deck to AsyncStorage
      try {
        const storedDecks = await AsyncStorage.getItem('decks');
        let decks = storedDecks ? JSON.parse(storedDecks) : [];

        // Check if deck ID already exists
        if (decks.some(d => d.id === importedData.id)) {
          importedData.id = Date.now().toString(); // Assign new ID to avoid conflicts
        }

        decks.push(importedData);
        await AsyncStorage.setItem('decks', JSON.stringify(decks));
        Alert.alert('Success', 'Deck imported successfully.');
        router.replace('/');
      } catch (error) {
        console.error('Error saving imported deck:', error);
        Alert.alert('Error', 'Failed to save imported deck.');
      }
    } catch (error) {
      console.error('Import error:', error);
      Alert.alert('Error', 'An error occurred during import.');
    }
  }

  return (
    <View style={[styles.container, { justifyContent: 'space-between' }]}>
      <Text style={[styles.title, {top: '2.5%'}]}>
        {'Create a new Deck'}
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
            {'Create deck and go Home'}
          </Text>
        </TouchableOpacity>

        <TouchableOpacity style={styles.buttonAdd} onPress={addCards}>
          <Text style={styles.buttonText}>
            {'Create deck and add cards'}
          </Text>
        </TouchableOpacity>

        <View style={styles.separator} />

        <TouchableOpacity style={styles.buttonAdd} onPress={() => handleImportDeck(true)}>
          <Text style={styles.buttonText}>
            {'Import deck as JSON'}
          </Text>
        </TouchableOpacity>

        <TouchableOpacity style={styles.buttonAdd} onPress={() => handleImportDeck(false)}>
          <Text style={styles.buttonText}>
            {'Import deck as CSV'}
          </Text>
        </TouchableOpacity>
      </View>

      <TouchableOpacity
        style={[styles.backButtonBg, { display: isKeyboardVisible ? 'none' : 'flex' }]}
        onPress={goBack}
      >
        <Text style={styles.learnBackButtonText}>cancel</Text>
      </TouchableOpacity>
    </View>
  );
}