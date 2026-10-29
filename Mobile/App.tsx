/**
 * PataFácil Mobile — Aplicativo do Tutor (PIM IV)
 *
 * Ponto de entrada do app: envolve tudo com o provedor de autenticação
 * e a navegação (Login/Cadastro quando deslogado; painel do Tutor quando logado).
 */
import React from "react";
import { NavigationContainer } from "@react-navigation/native";
import { SafeAreaProvider } from "react-native-safe-area-context";

import { AuthProvider } from "./src/context/AuthContext";
import { RootNavigator } from "./src/navigation/RootNavigator";

export default function App() {
  return (
    <SafeAreaProvider>
      <AuthProvider>
        <NavigationContainer>
          <RootNavigator />
        </NavigationContainer>
      </AuthProvider>
    </SafeAreaProvider>
  );
}
