import { useEffect, useState } from 'react';
import { View, Text, TouchableOpacity, ActivityIndicator, Dimensions, Alert } from 'react-native';
import { useLocalSearchParams, router } from 'expo-router';
import AsyncStorage from '@react-native-async-storage/async-storage';
import styles from '../../styles';
import { LinearGradient } from 'expo-linear-gradient';
import Animated, {
  useSharedValue,
  withTiming,
  useAnimatedStyle,
  Easing,
  interpolate,
} from 'react-native-reanimated';

export default function LearnDeckScreen() {
  const params = useLocalSearchParams();
  const deckId = params.learnDeckID;
  const [deck, setDeck] = useState(null);
  const [loading, setLoading] = useState(true);
  const [currentCardIndex, setCurrentCardIndex] = useState(0);
  const [isFlipped, setIsFlipped] = useState(false);

  // Animation shared value
  const spin = useSharedValue(0);

  useEffect(function () {
    loadDeckDetails();
  }, [deckId]);

  async function loadDeckDetails() {
    try {
      setLoading(true);
      // Lade alle Decks
      var storedDecks = await AsyncStorage.getItem('decks');

      if (storedDecks) {
        var decks = JSON.parse(storedDecks);
        // Finde das Deck mit der passenden ID
        var foundDeck = decks.find(deck => deck.id === deckId);
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

  function nextCard() {
    if (currentCardIndex < deck.cards.length - 1) {
      // Zurücksetzen der Kartenansicht, falls sie umgedreht war
      if (isFlipped) {
        flipCard();
      }
      setCurrentCardIndex(currentCardIndex + 1);
    }
  }

  function prevCard() {
    if (currentCardIndex > 0) {
      // Zurücksetzen der Kartenansicht, falls sie umgedreht war
      if (isFlipped) {
        flipCard();
      }
      setCurrentCardIndex(currentCardIndex - 1);
    }
  }

  function flipCard() {
    setIsFlipped(!isFlipped);
    spin.value = withTiming(isFlipped ? 0 : 1, {
      duration: 500,
      easing: Easing.bezier(0.25, 0.1, 0.25, 1),
    });
  }

  function checkAnswer(selectedOption) {
    const card = deck.cards[currentCardIndex];
    if (card.type === 'quiz') {
      if (parseInt(selectedOption) === parseInt(card.correctAnswerIndex)) {
        Alert.alert('Correct!', 'Well done!', [{ text: 'OK' }]);
      } else {
        Alert.alert('Wrong answer', 'Try again!', [{ text: 'OK' }]);
      }
    }
  }

  // Animierte Stile für die Vorderseite
  const frontAnimatedStyle = useAnimatedStyle(() => {
    const rotateY = interpolate(spin.value, [0, 1], [0, 180]);
    return {
      transform: [{ rotateY: `${rotateY}deg` }],
      backfaceVisibility: 'hidden',
    };
  });

  // Animierte Stile für die Rückseite
  const backAnimatedStyle = useAnimatedStyle(() => {
    const rotateY = interpolate(spin.value, [0, 1], [180, 360]);
    return {
      transform: [{ rotateY: `${rotateY}deg` }],
      backfaceVisibility: 'hidden',
    };
  });

  // Render Funktionen für verschiedene Zustände
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
        <Text style={styles.errorText}>No cards found.</Text>
        <TouchableOpacity
          style={styles.backButton}
          onPress={navigateBack}
        >
          <Text style={styles.backButtonText}>Back</Text>
        </TouchableOpacity>
      </View>
    );
  }

  function renderLearningCards() {
    const card = deck.cards[currentCardIndex];
    const LastCard = currentCardIndex === deck.cards.length - 1;
    const FirstCard = currentCardIndex === 0;

    return (
      <View style={styles.container}>
        <LinearGradient
          colors={deck.gradientColors}
          style={styles.header}
          start={{ x: 0, y: 0 }}
          end={{ x: 1, y: 1 }}
        >
          <Text style={styles.title}>{deck.title}</Text>
          <Text style={styles.subtitle}>Card {currentCardIndex + 1} of {deck.cards.length}</Text>
        </LinearGradient>

        <View style={styles.cardContainer}>

          {card.type === 'card' && (
            <>
                <TouchableOpacity onPress={flipCard} activeOpacity={0.9} style={styles.cardWrapper}>
                  {/* Vorderseite (Frage) */}
                  <Animated.View style={[styles.card, frontAnimatedStyle]}>
                    <Text style={styles.cardTitle}>Question: {currentCardIndex + 1}</Text>
                    <Text style={styles.cardText}>{card.question}</Text>
                    <Text style={styles.hint}>Click to flip</Text>
                  </Animated.View>

                  {/* Rückseite (Antwort) */}
                  <Animated.View style={[styles.card, styles.cardBack, backAnimatedStyle]}>
                    <Text style={styles.cardTitle}>Answer:</Text>
                    <Text style={styles.cardText}>{card.answer}</Text>
                    <Text style={styles.hint}>Click to flip</Text>
                  </Animated.View>
                </TouchableOpacity>
            </>
          )}
          {card.type === 'quiz' && (
            <>
              <View style={styles.card}>
                <Text style={styles.cardTitle}>Question: {currentCardIndex + 1}</Text>
                <Text style={styles.cardText}>{card.question}</Text>
                <View style={styles.title}>
                  {card.options.map((option, index) => (
                    <TouchableOpacity key={index} style={styles.quizButton} onPress={() => { checkAnswer(card.options.indexOf(option)); }}>
                      <Text style={styles.quizButtonText}>{option}</Text>
                    </TouchableOpacity>
                  ))}
                </View>
              </View>
            </>
          )}


        </View>

        <View style={styles.navigationContainer}>
          <TouchableOpacity
            style={[styles.navButton, FirstCard && styles.disabledButton, {backgroundColor: '#696969'}]}
            onPress={prevCard}
            disabled={FirstCard}
          >
            <Text style={styles.navButtonText}>← Back</Text>
          </TouchableOpacity>

          <TouchableOpacity
            style={[styles.navButton, LastCard && styles.disabledButton, {backgroundColor: '#696969'}]}
            onPress={nextCard}
            disabled={LastCard}
          >
            <Text style={styles.navButtonText}>Foreward →</Text>
          </TouchableOpacity>
        </View>

        <TouchableOpacity
          style={styles.backButtonBg}
          onPress={navigateBack}
        >
          <Text style={styles.learnBackButtonText}>Quit learning</Text>
        </TouchableOpacity>
      </View>
    );
  }

  // Hauptrender-Funktion
  if (loading) {
    return renderLoadingState();
  } else if (!deck || !deck.cards || deck.cards.length === 0) {
    return renderErrorState();
  } else {
    return renderLearningCards();
  }
}