import { StyleSheet } from 'react-native';

const stylesIndex = StyleSheet.create({
    container: {
      flex: 1,
      backgroundColor: '#2f2f2f',
      padding: 16,
      justifyContent: 'space-between',
    },
    title: {
      fontSize: 24,
      fontWeight: 'bold',
      marginBottom: 20,
      textAlign: 'center',
      color: '#fff',
    },
    deckGrid: {
      paddingBottom: 20,
    },
    deckCard: {
      flex: 1,
      height: 120,
      margin: 3,
      padding: 5,
      borderRadius: 12,
      justifyContent: 'space-between',
    },
    deckTitle: {
      fontSize: 18,
      fontWeight: 'bold',
      color: '#fff',
      left: 10,
      top: 10,
    },
    addButton: {
      backgroundColor: '#dfaa30',
      padding: 16,
      borderRadius: 10,
      alignItems: 'center',
      margin: 20,
    },
    addButtonText: {
      color: '#2f2f2f',
      fontWeight: 'bold',
      fontSize: 30,
    },
    emptyText: {
      textAlign: 'center',
      marginTop: 40,
      fontSize: 16,
      color: '#888',
    },
    cardCount: {
      fontSize: 14,
      color: '#fff',
      opacity: 0.8,
      left: 10,
      bottom: 10,
    },
    delete: {
      position: 'absolute',
      top: 8,
      right: 8,
      color: '#ff4d4d',
      fontSize: 20,
    },
    safeArea: {
      flex: 1,
      backgroundColor: '#2f2f2f',
    },
    fab: {
      position: 'absolute',
      right: 16,
      bottom: 16,
      width: 56,
      height: 56,
      borderRadius: 28,
      backgroundColor: '#dfaa30',
      justifyContent: 'center',
      alignItems: 'center',
      shadowColor: '#000',
      shadowOffset: { width: 0, height: 2 },
      shadowOpacity: 0.3,
      shadowRadius: 3.84,
      elevation: 5,
    },
  });

export default stylesIndex;