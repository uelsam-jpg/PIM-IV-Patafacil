import React, { useState } from "react";
import {
  Alert,
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  StyleSheet,
  Text,
  TouchableOpacity,
  View,
} from "react-native";
import DateTimePicker, {
  DateTimePickerEvent,
} from "@react-native-community/datetimepicker";
import { useNavigation } from "@react-navigation/native";
import { Ionicons } from "@expo/vector-icons";
import { StatusBar } from "expo-status-bar";

import { AppButton } from "../components/AppButton";
import { AppInput } from "../components/AppInput";
import { useAuth } from "../context/AuthContext";
import { colors } from "../theme/colors";
import { criarPet, Especie, Sexo } from "../services/petService";

/** Formata a data de nascimento no mesmo padrão do Frontend Web: "yyyy-MM-ddT00:00:00" */
function formatarNascimento(data: Date): string {
  const mes = String(data.getMonth() + 1).padStart(2, "0");
  const dia = String(data.getDate()).padStart(2, "0");
  return `${data.getFullYear()}-${mes}-${dia}T00:00:00`;
}

const ESPECIES: { valor: Especie; rotulo: string; emoji: string }[] = [
  { valor: Especie.Cachorro, rotulo: "Cachorro", emoji: "🐶" },
  { valor: Especie.Gato, rotulo: "Gato", emoji: "🐱" },
  { valor: Especie.Ave, rotulo: "Ave", emoji: "🐦" },
  { valor: Especie.Roedor, rotulo: "Roedor", emoji: "🐹" },
  { valor: Especie.Outro, rotulo: "Outro", emoji: "🐾" },
];

export function AddPetScreen() {
  const { token, usuario } = useAuth();
  const navigation = useNavigation<any>();

  const [nome, setNome] = useState("");
  const [especie, setEspecie] = useState<Especie>(Especie.Cachorro);
  const [raca, setRaca] = useState("");
  const [sexo, setSexo] = useState<Sexo>(Sexo.Macho);
  const [peso, setPeso] = useState("");
  const [dataNascimento, setDataNascimento] = useState<Date>(() => {
    const d = new Date();
    d.setFullYear(d.getFullYear() - 1); // padrão: 1 ano atrás
    return d;
  });
  const [mostrarPicker, setMostrarPicker] = useState(false);
  const [observacoes, setObservacoes] = useState("");
  const [salvando, setSalvando] = useState(false);

  function aoMudarData(event: DateTimePickerEvent, data?: Date) {
    if (Platform.OS === "android") setMostrarPicker(false);
    if (data) setDataNascimento(data);
  }

  async function salvar() {
    if (!nome.trim()) {
      Alert.alert("Obrigatório", "Informe o nome do pet.");
      return;
    }
    if (!token || !usuario) {
      Alert.alert("Sessão expirada", "Faça login novamente.");
      return;
    }
    setSalvando(true);
    try {
      await criarPet(
        {
          nome: nome.trim(),
          especie,
          raca: raca.trim(),
          sexo,
          dataNascimento: formatarNascimento(dataNascimento),
          pesoKg: peso ? parseFloat(peso.replace(",", ".")) : 0,
          observacoes: observacoes.trim(),
          ativo: true,
          tutorId: usuario.id,
        },
        token
      );
      Alert.alert("Sucesso!", "Pet cadastrado com sucesso!", [
        { text: "OK", onPress: () => navigation.goBack() },
      ]);
    } catch (e: any) {
      Alert.alert("Erro ao cadastrar", e.message ?? "Tente novamente.");
    } finally {
      setSalvando(false);
    }
  }

  return (
    <KeyboardAvoidingView
      style={styles.flex}
      behavior={Platform.OS === "ios" ? "padding" : undefined}
    >
      <StatusBar style="light" />
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.voltar}>
          <Ionicons name="arrow-back" size={24} color={colors.white} />
        </TouchableOpacity>
        <Text style={styles.headerTitulo}>Novo Pet</Text>
      </View>

      <ScrollView contentContainerStyle={styles.container} keyboardShouldPersistTaps="handled">
        <AppInput label="Nome do pet *" placeholder="Ex: Rex" value={nome} onChangeText={setNome} />

        <Text style={styles.label}>Espécie</Text>
        <View style={styles.chips}>
          {ESPECIES.map((e) => {
            const ativo = especie === e.valor;
            return (
              <TouchableOpacity
                key={e.valor}
                style={[styles.chip, ativo && styles.chipAtivo]}
                onPress={() => setEspecie(e.valor)}
              >
                <Text style={styles.chipTexto}>
                  {e.emoji} {e.rotulo}
                </Text>
              </TouchableOpacity>
            );
          })}
        </View>

        <AppInput label="Raça" placeholder="Ex: Golden Retriever" value={raca} onChangeText={setRaca} />

        <Text style={styles.label}>Sexo</Text>
        <View style={styles.chips}>
          <TouchableOpacity
            style={[styles.chip, sexo === Sexo.Macho && styles.chipAtivo]}
            onPress={() => setSexo(Sexo.Macho)}
          >
            <Text style={styles.chipTexto}>♂ Macho</Text>
          </TouchableOpacity>
          <TouchableOpacity
            style={[styles.chip, sexo === Sexo.Femea && styles.chipAtivo]}
            onPress={() => setSexo(Sexo.Femea)}
          >
            <Text style={styles.chipTexto}>♀ Fêmea</Text>
          </TouchableOpacity>
        </View>

        <AppInput
          label="Peso (kg)"
          placeholder="Ex: 12,5"
          keyboardType="decimal-pad"
          value={peso}
          onChangeText={setPeso}
        />

        <Text style={styles.label}>Data de nascimento</Text>
        <TouchableOpacity style={styles.dateButton} onPress={() => setMostrarPicker(true)}>
          <Ionicons name="calendar-outline" size={20} color={colors.primary} />
          <Text style={styles.dateButtonTexto}>
            {dataNascimento.toLocaleDateString("pt-BR")}
          </Text>
        </TouchableOpacity>
        {mostrarPicker && (
          <DateTimePicker
            value={dataNascimento}
            mode="date"
            maximumDate={new Date()}
            display={Platform.OS === "ios" ? "spinner" : "default"}
            onChange={aoMudarData}
          />
        )}

        <AppInput
          label="Observações"
          placeholder="Alergias, temperamento, etc."
          multiline
          numberOfLines={3}
          value={observacoes}
          onChangeText={setObservacoes}
          style={styles.multiline}
        />

        <AppButton title="Salvar pet" onPress={salvar} loading={salvando} style={{ marginTop: 8 }} />
      </ScrollView>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  flex: { flex: 1, backgroundColor: colors.background },
  header: {
    flexDirection: "row",
    alignItems: "center",
    backgroundColor: colors.primary,
    paddingTop: 60,
    paddingBottom: 18,
    paddingHorizontal: 16,
  },
  voltar: {
    marginRight: 12,
  },
  headerTitulo: {
    color: colors.white,
    fontSize: 22,
    fontWeight: "800",
  },
  container: {
    padding: 20,
    paddingBottom: 40,
  },
  label: {
    fontSize: 14,
    fontWeight: "600",
    color: colors.text,
    marginBottom: 6,
  },
  chips: {
    flexDirection: "row",
    flexWrap: "wrap",
    marginBottom: 14,
  },
  chip: {
    borderWidth: 1.5,
    borderColor: colors.border,
    borderRadius: 20,
    paddingHorizontal: 14,
    paddingVertical: 8,
    marginRight: 8,
    marginBottom: 8,
    backgroundColor: colors.card,
  },
  chipAtivo: {
    borderColor: colors.primary,
    backgroundColor: colors.primaryLight,
  },
  chipTexto: {
    color: colors.text,
    fontWeight: "600",
  },
  dateButton: {
    flexDirection: "row",
    alignItems: "center",
    backgroundColor: colors.card,
    borderRadius: 12,
    borderWidth: 1.5,
    borderColor: colors.border,
    padding: 14,
    marginBottom: 14,
  },
  dateButtonTexto: {
    fontSize: 16,
    color: colors.text,
    marginLeft: 10,
    fontWeight: "600",
  },
  multiline: {
    height: 90,
    textAlignVertical: "top",
  },
});
