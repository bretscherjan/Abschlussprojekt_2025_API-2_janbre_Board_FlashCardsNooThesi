import { Tabs } from 'expo-router';
import FontAwesome from '@expo/vector-icons/FontAwesome';
import React, { useState, useEffect } from 'react';
import { StatusBar } from 'expo-status-bar';
import { SafeAreaProvider, SafeAreaView } from 'react-native-safe-area-context';
import { Keyboard } from 'react-native';
import styles from '../styles';

export default function TabLayout() {
  const [isKeyboardVisible, setKeyboardVisible] = useState(false);

  useEffect(() => {
    const keyboardDidShowListener = Keyboard.addListener(
      'keyboardDidShow',
      () => {
        setKeyboardVisible(true);
      }
    );
    const keyboardDidHideListener = Keyboard.addListener(
      'keyboardDidHide',
      () => {
        setKeyboardVisible(false);
      }
    );

    // Aufräumen
    return () => {
      keyboardDidShowListener.remove();
      keyboardDidHideListener.remove();
    };
  }, []);

  return (
    <SafeAreaProvider>
      <SafeAreaView style={styles.safeArea} edges={['top', 'bottom']}>
        <StatusBar style="light" hidden={false} backgroundColor="#171717" />
        <Tabs
          screenOptions={{
            headerShown: false,
            tabBarShowLabel: false,
            tabBarActiveTintColor: 'rgba(223, 170, 48, 0.7)',
            tabBarInactiveTintColor: 'rgba(223, 170, 48, 0.3)',
            tabBarStyle: {
              backgroundColor: 'rgba(30, 30, 30, 1)',
              borderTopColor: 'rgba(223, 170, 48, 0.1)',
              height: 75,
              paddingTop: 17,
              display: isKeyboardVisible ? 'none' : 'flex',
            },
          }}
        >
          <Tabs.Screen
            name="index"
            options={{
              tabBarIcon: ({ color }) => (
                <FontAwesome name="home" size={32} color={color} />
              ),
            }}
          />
          <Tabs.Screen
            name="create"
            options={{
              tabBarIcon: ({ color }) => (
                <FontAwesome name="plus" size={32} color={color} />
              ),
            }}
          />
        </Tabs>
      </SafeAreaView>
    </SafeAreaProvider>
  );
}