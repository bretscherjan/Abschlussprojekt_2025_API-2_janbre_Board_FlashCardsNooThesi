import { StyleSheet } from 'react-native';

const stylesDeck = StyleSheet.create({
    container: {
      flex: 1,
      backgroundColor: '#2f2f2f',
    },
    centered: {
      flex: 1,
      justifyContent: 'center',
      alignItems: 'center',
      padding: 20,
    },
    errorText: {
      fontSize: 18,
      color: '#a0a0a0',
      marginBottom: 20,
    },
    header: {
      alignItems: 'center',
      justifyContent: 'center',
      padding: 20,
      paddingTop: 100,
      paddingBottom: 30,
      borderBottomLeftRadius: 20,
      borderBottomRightRadius: 20,
      shadowColor: '#000',
      shadowOffset: {
        width: 0,
        height: 4,
      },
    },
    title: {
      fontSize: 24,
      fontWeight: 'bold',
      color: '#fff',
      marginBottom: 8,
    },
    cardCount: {
      fontSize: 16,
      color: '#fff',
      opacity: 0.9,
    },
    startButton: {
      backgroundColor: '#dfaa30',
      margin: 20,
      padding: 16,
      borderRadius: 10,
      alignItems: 'center',
    },
    buttonText: {
      color: '#2f2f2f',
      fontWeight: 'bold',
      fontSize: 16,
    },
    backButton: {
      padding: 16,
      alignItems: 'center',
    },
    backButtonText: {
      color: '#a0a0a0',
      fontSize: 16,
    },
  });

export default stylesDeck;