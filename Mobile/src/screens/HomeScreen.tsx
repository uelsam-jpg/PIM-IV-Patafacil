import React from "react";
import {
  ScrollView,
  StyleSheet,
  Text,
  TouchableOpacity,
  View,
} from "react-native";
import { useNavigation } from "@react-navigation/native";
import { Ionicons } from "@expo/vector-icons";
import { StatusBar } from "expo-status-bar";

import { useAuth } from "../context/AuthContext";
import { colors } from "../theme/colors";
import { MainTabsParamList } from "../navigation/types";

export function HomeScreen() {
  const { usuario, logout } = useAuth();
  const navigation = useNavigation<any>();

  const acoes = [
    {
      titulo: "Meus Pets",
      descricao: "Veja e cadastre seus animais",
      icone: "paw" as const,
      cor: colors.primary,
      aoClicar: () => navigation.navigate("Pets"),
    },
    {
      titulo: "Agendar serviço",
      descricao: "Banho, consulta, vacina e mais",
      icone: "calendar" as const,
      cor: colors.secondary,
      aoClicar: () => navigation.navigate("Agendar"),
    },
    {
      titulo: "Minhas consultas",
      descricao: "Acompanhe e cancele agendamentos",
      icone: "list" as const,
      cor: "#8e44ad",
      aoClicar: () => navigation.navigate("MeusAgendamentos"),
    },
  ];

  return (
    <ScrollView style={styles.flex} contentContainerStyle={styles.container}>
      <StatusBar style="light" />
      <View style={styles.hero}>
        <Text style={styles.saudacao}>Olá, {usuario?.nome?.split(" ")[0]} 👋</Text>
        <Text style={styles.heroSub}>
          Seu painel de cuidados com os pets está pronto.
        </Text>
      </View>

      <Text style={styles.secaoTitulo}>O que deseja fazer?</Text>

      {acoes.map((acao) => (
        <TouchableOpacity
          key={acao.titulo}
          style={styles.card}
          onPress={acao.aoClicar}
          activeOpacity={0.8}
        >
          <View style={[styles.iconeWrapper, { backgroundColor: `${acao.cor}18` }]}>
            <Ionicons name={acao.icone} size={26} color={acao.cor} />
          </View>
          <View style={styles.cardInfo}>
            <Text style={styles.cardTitulo}>{acao.titulo}</Text>
            <Text style={styles.cardDesc}>{acao.descricao}</Text>
          </View>
          <Ionicons name="chevron-forward" size={20} color={colors.textMuted} />
        </TouchableOpacity>
      ))}

      <TouchableOpacity style={styles.sair} onPress={logout}>
        <Ionicons name="log-out-outline" size={20} color={colors.danger} />
        <Text style={styles.sairTexto}>Sair da conta</Text>
      </TouchableOpacity>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  flex: { flex: 1, backgroundColor: colors.background },
  container: {
    paddingBottom: 40,
  },
  hero: {
    backgroundColor: colors.primary,
    paddingHorizontal: 24,
    paddingTop: 70,
    paddingBottom: 34,
    borderBottomLeftRadius: 28,
    borderBottomRightRadius: 28,
  },
  saudacao: {
    color: colors.white,
    fontSize: 26,
    fontWeight: "800",
  },
  heroSub: {
    color: colors.white,
    fontSize: 14,
    marginTop: 6,
    opacity: 0.92,
  },
  secaoTitulo: {
    fontSize: 18,
    fontWeight: "700",
    color: colors.text,
    marginTop: 24,
    marginBottom: 12,
    paddingHorizontal: 24,
  },
  card: {
    flexDirection: "row",
    alignItems: "center",
    backgroundColor: colors.card,
    borderRadius: 16,
    padding: 16,
    marginHorizontal: 24,
    marginBottom: 12,
    borderWidth: 1,
    borderColor: colors.border,
  },
  iconeWrapper: {
    width: 52,
    height: 52,
    borderRadius: 26,
    alignItems: "center",
    justifyContent: "center",
    marginRight: 14,
  },
  cardInfo: {
    flex: 1,
  },
  cardTitulo: {
    fontSize: 16,
    fontWeight: "700",
    color: colors.text,
  },
  cardDesc: {
    fontSize: 13,
    color: colors.textMuted,
    marginTop: 2,
  },
  sair: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    marginTop: 24,
    padding: 14,
  },
  sairTexto: {
    color: colors.danger,
    fontWeight: "700",
    marginLeft: 6,
  },
});
