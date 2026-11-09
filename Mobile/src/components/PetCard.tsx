import React from "react";
import { StyleSheet, Text, View } from "react-native";
import { colors } from "../theme/colors";
import { Especie, Pet, Sexo } from "../services/petService";

const EMOJI_ESPECIE: Record<number, string> = {
  [Especie.Cachorro]: "🐶",
  [Especie.Gato]: "🐱",
  [Especie.Ave]: "🐦",
  [Especie.Roedor]: "🐹",
  [Especie.Outro]: "🐾",
};

export function petEmoji(especie: Especie): string {
  return EMOJI_ESPECIE[especie] ?? "🐾";
}

interface Props {
  pet: Pet;
}

export function PetCard({ pet }: Props) {
  return (
    <View style={styles.card}>
      <View style={styles.avatar}>
        <Text style={styles.avatarText}>{petEmoji(pet.especie)}</Text>
      </View>
      <View style={styles.info}>
        <Text style={styles.nome}>{pet.nome}</Text>
        <Text style={styles.detalhes}>
          {pet.raca ? `${pet.raca} · ` : ""}
          {pet.sexo === Sexo.Macho ? "Macho" : "Fêmea"}
          {pet.pesoKg ? ` · ${String(pet.pesoKg).replace(".", ",")} kg` : ""}
        </Text>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  card: {
    flexDirection: "row",
    alignItems: "center",
    backgroundColor: colors.card,
    borderRadius: 14,
    padding: 14,
    marginBottom: 10,
    borderWidth: 1,
    borderColor: colors.border,
  },
  avatar: {
    width: 52,
    height: 52,
    borderRadius: 26,
    backgroundColor: colors.primaryLight,
    alignItems: "center",
    justifyContent: "center",
    marginRight: 14,
  },
  avatarText: {
    fontSize: 26,
  },
  info: {
    flex: 1,
  },
  nome: {
    fontSize: 17,
    fontWeight: "700",
    color: colors.text,
  },
  detalhes: {
    fontSize: 13,
    color: colors.textMuted,
    marginTop: 2,
  },
});
