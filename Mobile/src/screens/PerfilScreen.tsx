import React from "react";
import {
  Alert,
  ScrollView,
  StyleSheet,
  Text,
  TouchableOpacity,
  View,
} from "react-native";
import { Ionicons } from "@expo/vector-icons";
import { StatusBar } from "expo-status-bar";

import { useAuth } from "../context/AuthContext";
import { colors } from "../theme/colors";

export function PerfilScreen() {
  const { usuario, logout } = useAuth();

  function sair() {
    Alert.alert("Sair da conta", "Deseja realmente sair?", [
      { text: "Cancelar", style: "cancel" },
      { text: "Sair", style: "destructive", onPress: logout },
    ]);
  }

  return (
    <ScrollView style={styles.flex} contentContainerStyle={styles.container}>
      <StatusBar style="light" />
      <View style={styles.header}>
        <Text style={styles.headerTitulo}>Perfil</Text>
      </View>

      <View style={styles.avatar}>
        <Text style={styles.avatarTexto}>
          {usuario?.nome?.charAt(0)?.toUpperCase() ?? "?"}
        </Text>
      </View>
      <Text style={styles.nome}>{usuario?.nome ?? "Usuário"}</Text>
      <Text style={styles.email}>{usuario?.email ?? ""}</Text>

      <View style={styles.infoCard}>
        <ItemInfo icone="shield-checkmark-outline" rotulo="Perfil" valor={usuario?.perfil ?? "-"} />
        <ItemInfo icone="paw-outline" rotulo="ID do usuário" valor={String(usuario?.id ?? "-")} />
        <ItemInfo
          icone="phone-portrait-outline"
          rotulo="Dispositivo"
          valor="Aplicativo PataFácil"
        />
      </View>

      <TouchableOpacity style={styles.sairBtn} onPress={sair}>
        <Ionicons name="log-out-outline" size={20} color={colors.danger} />
        <Text style={styles.sairTexto}>Sair da conta</Text>
      </TouchableOpacity>

      <Text style={styles.versao}>PataFácil Mobile v1.0.0 · PIM IV</Text>
    </ScrollView>
  );
}

function ItemInfo({ icone, rotulo, valor }: { icone: any; rotulo: string; valor: string }) {
  return (
    <View style={styles.itemInfo}>
      <Ionicons name={icone} size={20} color={colors.primary} style={styles.itemIcone} />
      <View style={styles.itemTextos}>
        <Text style={styles.itemRotulo}>{rotulo}</Text>
        <Text style={styles.itemValor}>{valor}</Text>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  flex: { flex: 1, backgroundColor: colors.background },
  container: {
    alignItems: "center",
    paddingBottom: 40,
  },
  header: {
    alignSelf: "stretch",
    backgroundColor: colors.primary,
    paddingTop: 60,
    paddingBottom: 18,
    paddingHorizontal: 24,
  },
  headerTitulo: {
    color: colors.white,
    fontSize: 24,
    fontWeight: "800",
  },
  avatar: {
    width: 96,
    height: 96,
    borderRadius: 48,
    backgroundColor: colors.primary,
    alignItems: "center",
    justifyContent: "center",
    marginTop: 24,
  },
  avatarTexto: {
    fontSize: 40,
    fontWeight: "800",
    color: colors.white,
  },
  nome: {
    fontSize: 22,
    fontWeight: "800",
    color: colors.text,
    marginTop: 12,
  },
  email: {
    fontSize: 14,
    color: colors.textMuted,
    marginTop: 2,
  },
  infoCard: {
    alignSelf: "stretch",
    marginHorizontal: 20,
    marginTop: 24,
    backgroundColor: colors.card,
    borderRadius: 16,
    paddingHorizontal: 16,
    borderWidth: 1,
    borderColor: colors.border,
  },
  itemInfo: {
    flexDirection: "row",
    alignItems: "center",
    paddingVertical: 14,
    borderBottomWidth: 1,
    borderBottomColor: colors.border,
  },
  itemIcone: {
    marginRight: 12,
  },
  itemTextos: {
    flex: 1,
  },
  itemRotulo: {
    fontSize: 12,
    color: colors.textMuted,
  },
  itemValor: {
    fontSize: 15,
    fontWeight: "700",
    color: colors.text,
    marginTop: 2,
  },
  sairBtn: {
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "center",
    alignSelf: "stretch",
    marginHorizontal: 20,
    marginTop: 20,
    padding: 16,
    backgroundColor: colors.card,
    borderRadius: 14,
    borderWidth: 1,
    borderColor: colors.danger,
  },
  sairTexto: {
    color: colors.danger,
    fontWeight: "700",
    marginLeft: 8,
  },
  versao: {
    marginTop: 24,
    color: colors.textMuted,
    fontSize: 12,
  },
});
