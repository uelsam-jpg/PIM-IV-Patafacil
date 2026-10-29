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
import { cadastrarTutor } from "../services/authService";
import { colors } from "../theme/colors";
import { AuthStackParamList } from "../navigation/types";

type Props = NativeStackScreenProps<AuthStackParamList, "Register">;

export function RegisterScreen({ navigation }: Props) {
  const [nome, setNome] = useState("");
  const [cpf, setCpf] = useState("");
  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
  const [telefone, setTelefone] = useState("");
  const [carregando, setCarregando] = useState(false);

  async function cadastrar() {
    if (!nome.trim() || !cpf.trim() || !email.trim() || !senha) {
      Alert.alert("Campos obrigatórios", "Preencha nome, CPF, e-mail e senha.");
      return;
    }
    if (cpf.replace(/\D/g, "").length !== 11) {
      Alert.alert("CPF inválido", "Digite um CPF com 11 números.");
      return;
    }
    // Mesma regra da API: senha com pelo menos 6 caracteres
    if (senha.length < 6) {
      Alert.alert("Senha muito curta", "A senha deve ter pelo menos 6 caracteres.");
      return;
    }

    setCarregando(true);
    try {
      await cadastrarTutor({
        nome: nome.trim(),
        cpf: cpf.replace(/\D/g, ""),
        email: email.trim(),
        senhaHash: senha,
        telefone: telefone.trim() || "(00) 00000-0000",
        // Campos de endereço com padrão (podem ser completados depois no site)
        logradouro: "A preencher",
        numero: "S/N",
        bairro: "A preencher",
        cidade: "A preencher",
        uf: "SP",
        cep: "00000-000",
      });
      Alert.alert("Sucesso!", "Conta criada. Faça login para entrar.", [
        { text: "OK", onPress: () => navigation.goBack() },
      ]);
    } catch (e: any) {
      Alert.alert("Falha ao cadastrar", e.message ?? "Tente novamente.");
    } finally {
      setCarregando(false);
    }
  }

  return (
    <KeyboardAvoidingView
      style={styles.flex}
      behavior={Platform.OS === "ios" ? "padding" : undefined}
    >
      <StatusBar style="dark" />
      <ScrollView
        contentContainerStyle={styles.container}
        keyboardShouldPersistTaps="handled"
      >
        <View style={styles.header}>
          <Text style={styles.headerTitle}>Criar conta</Text>
          <Text style={styles.headerSub}>Cadastre-se para cuidar do seu pet</Text>
        </View>

        <View style={styles.card}>
          <AppInput label="Nome completo" placeholder="Maria Silva" value={nome} onChangeText={setNome} />
          <AppInput
            label="CPF"
            placeholder="Somente números"
            keyboardType="number-pad"
            maxLength={14}
            value={cpf}
            onChangeText={setCpf}
          />
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
            placeholder="Crie uma senha"
            secureTextEntry
            value={senha}
            onChangeText={setSenha}
          />
          <AppInput
            label="Telefone (opcional)"
            placeholder="(11) 99999-9999"
            keyboardType="phone-pad"
            value={telefone}
            onChangeText={setTelefone}
          />

          <AppButton title="Cadastrar" onPress={cadastrar} loading={carregando} />

          <View style={styles.rodape}>
            <Text style={styles.rodapeTexto}>Já tem conta?</Text>
            <Text
              style={styles.rodapeLink}
              onPress={() => navigation.goBack()}
            >
              Fazer login
            </Text>
          </View>
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  flex: { flex: 1, backgroundColor: colors.background },
  container: {
    flexGrow: 1,
    padding: 24,
    paddingTop: 60,
  },
  header: {
    marginBottom: 20,
  },
  headerTitle: {
    fontSize: 26,
    fontWeight: "800",
    color: colors.text,
  },
  headerSub: {
    fontSize: 14,
    color: colors.textMuted,
    marginTop: 4,
  },
  card: {
    backgroundColor: colors.card,
    borderRadius: 20,
    padding: 22,
    shadowColor: colors.black,
    shadowOpacity: 0.08,
    shadowRadius: 10,
    shadowOffset: { width: 0, height: 3 },
    elevation: 3,
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
});
