import React, { useState } from "react";
import {
  Alert,
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  StyleSheet,
  Text,
  View,
} from "react-native";
import { NativeStackScreenProps } from "@react-navigation/native-stack";
import { StatusBar } from "expo-status-bar";

import { AppButton } from "../components/AppButton";
import { AppInput } from "../components/AppInput";
import { useAuth } from "../context/AuthContext";
import { colors } from "../theme/colors";
import { AuthStackParamList } from "../navigation/types";

type Props = NativeStackScreenProps<AuthStackParamList, "Login">;

export function LoginScreen({ navigation }: Props) {
  const { login } = useAuth();
  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
  const [carregando, setCarregando] = useState(false);

  async function entrar() {
    if (!email.trim() || !senha) {
      Alert.alert("Campos obrigatórios", "Informe e-mail e senha.");
      return;
    }
    setCarregando(true);
    try {
      await login(email.trim(), senha);
    } catch (e: any) {
      Alert.alert("Acesso negado", e.message ?? "Falha ao entrar.");
    } finally {
      setCarregando(false);
    }
  }

  return (
    <KeyboardAvoidingView
      style={styles.flex}
      behavior={Platform.OS === "ios" ? "padding" : undefined}
    >
      <StatusBar style="light" />
      <ScrollView
        contentContainerStyle={styles.container}
        keyboardShouldPersistTaps="handled"
      >
        <View style={styles.hero}>
          <Text style={styles.logo}>🐾</Text>
          <Text style={styles.titulo}>PataFácil</Text>
          <Text style={styles.subtitulo}>
            O melhor cuidado para o seu melhor amigo, agora na palma da mão.
          </Text>
        </View>

        <View style={styles.card}>
          <Text style={styles.cardTitle}>Entrar</Text>

          <AppInput
            label="E-mail"
            placeholder="seuemail@exemplo.com"
            keyboardType="email-address"
            autoCapitalize="none"
            value={email}
            onChangeText={setEmail}
          />
          <AppInput
            label="Senha"
            placeholder="Sua senha"
            secureTextEntry
            value={senha}
            onChangeText={setSenha}
            onSubmitEditing={entrar}
          />

          <AppButton title="Entrar" onPress={entrar} loading={carregando} />

          <View style={styles.rodape}>
            <Text style={styles.rodapeTexto}>Ainda não tem conta?</Text>
            <Text
              style={styles.rodapeLink}
              onPress={() => navigation.navigate("Register")}
            >
              Cadastre-se
            </Text>
          </View>
        </View>

        <Text style={styles.aviso}>
          Faça login com a conta de Tutor criada no site, ou cadastre-se agora.
        </Text>
      </ScrollView>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  flex: { flex: 1, backgroundColor: colors.primary },
  container: {
    flexGrow: 1,
    justifyContent: "center",
    padding: 24,
  },
  hero: {
    alignItems: "center",
    marginBottom: 28,
  },
  logo: {
    fontSize: 64,
  },
  titulo: {
    fontSize: 32,
    fontWeight: "800",
    color: colors.white,
    marginTop: 4,
  },
  subtitulo: {
    fontSize: 14,
    color: colors.white,
    textAlign: "center",
    marginTop: 6,
    paddingHorizontal: 10,
    opacity: 0.9,
  },
  card: {
    backgroundColor: colors.card,
    borderRadius: 20,
    padding: 22,
    shadowColor: colors.black,
    shadowOpacity: 0.12,
    shadowRadius: 12,
    shadowOffset: { width: 0, height: 4 },
    elevation: 4,
  },
  cardTitle: {
    fontSize: 20,
    fontWeight: "700",
    color: colors.text,
    marginBottom: 16,
  },
  rodape: {
    flexDirection: "row",
    justifyContent: "center",
    marginTop: 16,
  },
  rodapeTexto: {
    color: colors.textMuted,
  },
  rodapeLink: {
    color: colors.primary,
    fontWeight: "700",
    marginLeft: 4,
  },
  aviso: {
    color: colors.white,
    textAlign: "center",
    marginTop: 20,
    fontSize: 12,
    opacity: 0.85,
  },
});
