import React, { useCallback, useMemo, useState } from "react";
import {
  Alert,
  FlatList,
  RefreshControl,
  StyleSheet,
  Text,
  TouchableOpacity,
  View,
} from "react-native";
import { useFocusEffect } from "@react-navigation/native";
import { Ionicons } from "@expo/vector-icons";
import { StatusBar } from "expo-status-bar";

import { EmptyState } from "../components/EmptyState";
import { useAuth } from "../context/AuthContext";
import { colors } from "../theme/colors";
import { listarPetsDoTutor, Pet } from "../services/petService";
import {
  Agendamento,
  cancelarAgendamento,
  listarAgendamentosDoTutor,
  listarServicos,
  Servico,
  StatusAgendamento,
} from "../services/agendamentoService";

const STATUS_INFO: Record<number, { rotulo: string; cor: string; fundo: string }> = {
  [StatusAgendamento.PENDENTE]: { rotulo: "Pendente", cor: colors.warning, fundo: "#fff7e6" },
  [StatusAgendamento.CONFIRMADO]: { rotulo: "Confirmado", cor: colors.primary, fundo: colors.primaryLight },
  [StatusAgendamento.CONCLUIDO]: { rotulo: "Concluído", cor: colors.success, fundo: "#e8f5e9" },
  [StatusAgendamento.CANCELADO]: { rotulo: "Cancelado", cor: colors.danger, fundo: "#fdecea" },
};

function formatarDataHora(iso: string): string {
  const d = new Date(iso);
  return d.toLocaleDateString("pt-BR", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  }) + " às " + d.toLocaleTimeString("pt-BR", { hour: "2-digit", minute: "2-digit" });
}

export function MeusAgendamentosScreen() {
  const { token, usuario } = useAuth();
  const [agendamentos, setAgendamentos] = useState<Agendamento[]>([]);
  const [pets, setPets] = useState<Pet[]>([]);
  const [servicos, setServicos] = useState<Servico[]>([]);
  const [carregando, setCarregando] = useState(true);
  const [atualizando, setAtualizando] = useState(false);
  const [cancelando, setCancelando] = useState<number | null>(null);

  const carregarTudo = useCallback(
    async (modo: "inicial" | "refresh" = "inicial") => {
      if (!token || !usuario) return;
      if (modo === "inicial") setCarregando(true);
      else setAtualizando(true);
      try {
        const [agend, petsDados, servicosDados] = await Promise.all([
          listarAgendamentosDoTutor(usuario.id, token),
          listarPetsDoTutor(usuario.id, token),
          listarServicos(),
        ]);
        setAgendamentos(agend);
        setPets(petsDados);
        setServicos(servicosDados);
      } catch (e: any) {
        Alert.alert("Erro", e.message ?? "Não foi possível carregar os agendamentos.");
      } finally {
        setCarregando(false);
        setAtualizando(false);
      }
    },
    [token, usuario]
  );

  useFocusEffect(
    useCallback(() => {
      carregarTudo("inicial");
    }, [carregarTudo])
  );

  const mapaPets = useMemo(
    () => Object.fromEntries(pets.map((p) => [p.id, p.nome])),
    [pets]
  );
  const mapaServicos = useMemo(
    () => Object.fromEntries(servicos.map((s) => [s.id, s.nome])),
    [servicos]
  );

  function confirmarCancelamento(ag: Agendamento) {
    Alert.alert(
      "Cancelar agendamento?",
      `Deseja cancelar o agendamento de ${mapaPets[ag.idPet] ?? "seu pet"}?`,
      [
        { text: "Não", style: "cancel" },
        { text: "Sim, cancelar", style: "destructive", onPress: () => cancelar(ag) },
      ]
    );
  }

  async function cancelar(ag: Agendamento) {
    if (!token) return;
    setCancelando(ag.id);
    try {
      await cancelarAgendamento(ag.id, token);
      Alert.alert("Cancelado!", "Agendamento cancelado com sucesso.");
      await carregarTudo("refresh");
    } catch (e: any) {
      Alert.alert("Não foi possível cancelar", e.message ?? "Tente novamente.");
    } finally {
      setCancelando(null);
    }
  }

  if (carregando) {
    return (
      <View style={styles.flex}>
        <Header />
        <View style={styles.center}>
          <Text style={styles.carregandoTexto}>Carregando agendamentos...</Text>
        </View>
      </View>
    );
  }

  return (
    <View style={styles.flex}>
      <StatusBar style="light" />
      <Header />
      <FlatList
        data={agendamentos}
        keyExtractor={(item) => String(item.id)}
        contentContainerStyle={styles.lista}
        refreshControl={
          <RefreshControl
            refreshing={atualizando}
            onRefresh={() => carregarTudo("refresh")}
            colors={[colors.primary]}
          />
        }
        ListEmptyComponent={
          <EmptyState
            icon="📅"
            title="Nenhum agendamento"
            subtitle="Agende um serviço na aba Agendar."
          />
        }
        renderItem={({ item }) => {
          const status = STATUS_INFO[item.status] ?? STATUS_INFO[StatusAgendamento.PENDENTE];
          const podeCancelar =
            item.status === StatusAgendamento.PENDENTE ||
            item.status === StatusAgendamento.CONFIRMADO;
          return (
            <View style={styles.card}>
              <View style={styles.cardHeader}>
                <Text style={styles.cardPet}>{mapaPets[item.idPet] ?? "Pet"}</Text>
                <View style={[styles.statusChip, { backgroundColor: status.fundo }]}>
                  <Text style={[styles.statusTexto, { color: status.cor }]}>{status.rotulo}</Text>
                </View>
              </View>
              <Text style={styles.cardServico}>{mapaServicos[item.idServico] ?? "Serviço"}</Text>
              <Text style={styles.cardData}>{formatarDataHora(item.dataHoraInicio)}</Text>
              {podeCancelar && (
                <TouchableOpacity
                  style={styles.cancelarBtn}
                  onPress={() => confirmarCancelamento(item)}
                  disabled={cancelando === item.id}
                >
                  <Ionicons name="close-circle-outline" size={18} color={colors.danger} />
                  <Text style={styles.cancelarTexto}>
                    {cancelando === item.id ? "Cancelando..." : "Cancelar"}
                  </Text>
                </TouchableOpacity>
              )}
            </View>
          );
        }}
      />
    </View>
  );
}

function Header() {
  return (
    <View style={styles.header}>
      <Text style={styles.headerTitulo}>Minhas consultas</Text>
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
  card: {
    backgroundColor: colors.card,
    borderRadius: 14,
    padding: 16,
    marginBottom: 12,
    borderWidth: 1,
    borderColor: colors.border,
  },
  cardHeader: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
  },
  cardPet: {
    fontSize: 17,
    fontWeight: "800",
    color: colors.text,
  },
  statusChip: {
    borderRadius: 12,
    paddingHorizontal: 10,
    paddingVertical: 4,
  },
  statusTexto: {
    fontSize: 12,
    fontWeight: "700",
  },
  cardServico: {
    fontSize: 15,
    color: colors.text,
    marginTop: 6,
  },
  cardData: {
    fontSize: 13,
    color: colors.textMuted,
    marginTop: 2,
  },
  cancelarBtn: {
    flexDirection: "row",
    alignItems: "center",
    marginTop: 12,
    alignSelf: "flex-start",
  },
  cancelarTexto: {
    color: colors.danger,
    fontWeight: "700",
    marginLeft: 4,
  },
});
