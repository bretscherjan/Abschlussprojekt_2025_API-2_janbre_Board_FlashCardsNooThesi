import { StyleSheet } from 'react-native';

const stylesCreate = StyleSheet.create({
    container: {
      flex: 1,
      backgroundColor: '#2f2f2f',
      padding: 16,
    },
    title: {
      fontSize: 24,
      fontWeight: 'bold',
      marginBottom: 20,
      textAlign: 'center',
      color: '#fff',
    },
    form: {
      backgroundColor: '#1E1E1E',
      padding: 20,
      borderRadius: 12,
    },
    label: {
      fontSize: 16,
      marginBottom: 8,
      color: '#D0D0D0',
    },
    input: {
      backgroundColor: '#2A2A2A',
      color: '#fff',
      borderRadius: 8,
      padding: 12,
      fontSize: 16,
      marginBottom: 20,
    },
    button: {
      backgroundColor: '#dfaa30',
      padding: 16,
      borderRadius: 8,
      alignItems: 'center',
      marginTop: 20,
    },
    buttonText: {
      color: '#2f2f2f',
      fontWeight: 'bold',
      fontSize: 16,
    },
    cancelButton: {
      padding: 16,
      alignItems: 'center',
      marginTop: 10,
    },
    cancelButtonText: {
      color: '#a0a0a0',
      fontSize: 16,
    },
    hint: {
      fontSize: 14,
      color: '#888',
      marginBottom: 10,
      fontStyle: 'italic',
    },
    color: {
      width: 45,
      height: 45,
      borderRadius: 25,
      margin: 5,
      borderWidth: 1,
      borderColor: '#333',
    },
    colorContainer: {
      flexDirection: 'row',
      flexWrap: 'wrap',
      alignItems: 'center',
      marginBottom: 10,
    },
  });

export default stylesCreate;