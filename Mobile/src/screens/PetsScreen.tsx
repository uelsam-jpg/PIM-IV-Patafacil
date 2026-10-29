import React, { useCallback, useState } from "react";
import {
  Alert,
  FlatList,
  RefreshControl,
  StyleSheet,
  Text,
  TouchableOpacity,
  View,
} from "react-native";
import { useFocusEffect, useNavigation } from "@react-navigation/native";
import { Ionicons } from "@expo/vector-icons";
import { StatusBar } from "expo-status-bar";

import { useAuth } from "../context/AuthContext";
import { PetCard } from "../components/PetCard";
import { EmptyState } from "../components/EmptyState";
import { AppButton } from "../components/AppButton";
import { listarPetsDoTutor, Pet } from "../services/petService";
import { colors } from "../theme/colors";

export function PetsScreen() {
  const { token, usuario } = useAuth();
  const navigation = useNavigation<any>();

  const [pets, setPets] = useState<Pet[]>([]);
  const [carregando, setCarregando] = useState(true);
  const [atualizando, setAtualizando] = useState(false);

  // Sincroniza os dados sempre que a tela ganha foco (após cadastrar um pet, por exemplo)
  const carregarPets = useCallback(
    async (modo: "inicial" | "refresh" = "inicial") => {
      if (!token || !usuario) return;
      if (modo === "inicial") setCarregando(true);
      else setAtualizando(true);
      try {
        const dados = await listarPetsDoTutor(usuario.id, token);
        setPets(dados);
      } catch (e: any) {
        Alert.alert("Erro", e.message ?? "Não foi possível carregar os pets.");
      } finally {
        setCarregando(false);
        setAtualizando(false);
      }
    },
    [token, usuario]
  );

  useFocusEffect(
    useCallback(() => {
      carregarPets("inicial");
    }, [carregarPets])
  );

  if (carregando) {
    return (
      <View style={styles.flex}>
        <Header titulo="Meus Pets" />
        <View style={styles.center}>
          <Text style={styles.carregandoTexto}>Carregando seus pets...</Text>
        </View>
      </View>
    );
  }

  return (
    <View style={styles.flex}>
      <StatusBar style="light" />
      <Header titulo="Meus Pets" />
      <FlatList
        data={pets}
        keyExtractor={(item) => String(item.id)}
        contentContainerStyle={styles.lista}
        refreshControl={
          <RefreshControl
            refreshing={atualizando}
            onRefresh={() => carregarPets("refresh")}
            colors={[colors.primary]}
          />
        }
        ListEmptyComponent={
          <EmptyState
            icon="🐶"
            title="Nenhum pet cadastrado"
            subtitle="Cadastre seu primeiro pet para agendar serviços."
          />
        }
        renderItem={({ item }) => <PetCard pet={item} />}
      />
      <View style={styles.footer}>
        <AppButton
          title="+ Cadastrar novo pet"
          onPress={() => navigation.navigate("AddPet")}
        />
      </View>
    </View>
  );
}

function Header({ titulo }: { titulo: string }) {
  return (
    <View style={styles.header}>
      <Text style={styles.headerTitulo}>{titulo}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  flex: { flex: 1, backgroundColor: colors.background },
  center: { flex: 1, alignItems: "center", justifyContent: "center" },
  carregandoTexto: { color: colors.textMuted, fontSize: 15 },
  header: {
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
  lista: {
    padding: 20,
    flexGrow: 1,
  },
  footer: {
    padding: 20,
    backgroundColor: colors.card,
    borderTopWidth: 1,
    borderTopColor: colors.border,
  },
});
