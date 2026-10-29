import React from "react";
import { createNativeStackNavigator } from "@react-navigation/native-stack";
import { createBottomTabNavigator } from "@react-navigation/bottom-tabs";
import { Ionicons } from "@expo/vector-icons";

import { useAuth } from "../context/AuthContext";
import { Loading } from "../components/Loading";
import { colors } from "../theme/colors";

import { LoginScreen } from "../screens/LoginScreen";
import { RegisterScreen } from "../screens/RegisterScreen";
import { HomeScreen } from "../screens/HomeScreen";
import { PetsScreen } from "../screens/PetsScreen";
import { AddPetScreen } from "../screens/AddPetScreen";
import { AgendarScreen } from "../screens/AgendarScreen";
import { MeusAgendamentosScreen } from "../screens/MeusAgendamentosScreen";
import { PerfilScreen } from "../screens/PerfilScreen";

import { AuthStackParamList, MainTabsParamList } from "./types";

const AuthStack = createNativeStackNavigator<AuthStackParamList>();
const Tabs = createBottomTabNavigator<MainTabsParamList>();

function AuthNavigator() {
  return (
    <AuthStack.Navigator screenOptions={{ headerShown: false }}>
      <AuthStack.Screen name="Login" component={LoginScreen} />
      <AuthStack.Screen name="Register" component={RegisterScreen} />
    </AuthStack.Navigator>
  );
}

function MainTabsNavigator() {
  return (
    <Tabs.Navigator
      screenOptions={{
        headerShown: false,
        tabBarActiveTintColor: colors.primary,
        tabBarInactiveTintColor: colors.textMuted,
        tabBarStyle: {
          height: 62,
          paddingBottom: 8,
          paddingTop: 6,
        },
      }}
    >
      <Tabs.Screen
        name="Home"
        component={HomeScreen}
        options={{
          title: "Início",
          tabBarIcon: ({ color, size }) => (
            <Ionicons name="home" size={size} color={color} />
          ),
        }}
      />
      <Tabs.Screen
        name="Pets"
        component={PetsScreen}
        options={{
          title: "Pets",
          tabBarIcon: ({ color, size }) => (
            <Ionicons name="paw" size={size} color={color} />
          ),
        }}
      />
      <Tabs.Screen
        name="Agendar"
        component={AgendarScreen}
        options={{
          title: "Agendar",
          tabBarIcon: ({ color, size }) => (
            <Ionicons name="calendar" size={size} color={color} />
          ),
        }}
      />
      <Tabs.Screen
        name="MeusAgendamentos"
        component={MeusAgendamentosScreen}
        options={{
          title: "Consultas",
          tabBarIcon: ({ color, size }) => (
            <Ionicons name="list" size={size} color={color} />
          ),
        }}
      />
      <Tabs.Screen
        name="Perfil"
        component={PerfilScreen}
        options={{
          title: "Perfil",
          tabBarIcon: ({ color, size }) => (
            <Ionicons name="person" size={size} color={color} />
          ),
        }}
      />
      {/* Aba oculta usada para cadastrar novo pet a partir da tela de Pets */}
      <Tabs.Screen
        name="AddPet"
        component={AddPetScreen}
        options={{
          title: "Novo Pet",
          tabBarButton: () => null,
        }}
      />
    </Tabs.Navigator>
  );
}

export function RootNavigator() {
  const { token, loading } = useAuth();

  if (loading) {
    return <Loading />;
  }

  // Autenticado? Mostra o painel do Tutor. Senão, tela de Login.
  return token ? <MainTabsNavigator /> : <AuthNavigator />;
}
