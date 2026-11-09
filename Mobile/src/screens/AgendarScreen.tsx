import React, { useCallback, useEffect, useState } from "react";
import {
  Alert,
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
import { Ionicons } from "@expo/vector-icons";
import { StatusBar } from "expo-status-bar";

import { AppButton } from "../components/AppButton";
import { AppInput } from "../components/AppInput";
import { EmptyState } from "../components/EmptyState";
import { useAuth } from "../context/AuthContext";
import { colors } from "../theme/colors";
import { listarPetsDoTutor, Pet } from "../services/petService";
import {
  criarAgendamento,
  listarHorariosDisponiveis,
  listarProfissionais,
  listarServicos,
  Profissional,
  Servico,
} from "../services/agendamentoService";

const PASSOS = ["Pet", "Serviço", "Profissional", "Data/Hora", "Resumo"];

/** Formata uma data como yyyy-MM-dd para enviar à API */
function formatarDataAPI(data: Date): string {
  const mes = String(data.getMonth() + 1).padStart(2, "0");
  const dia = String(data.getDate()).padStart(2, "0");
  return `${data.getFullYear()}-${mes}-${dia}`;
}

/** Formata um horário ISO em HH:mm */
function formatarHora(iso: string): string {
  const d = new Date(iso);
  return `${String(d.getHours()).padStart(2, "0")}:${String(d.getMinutes()).padStart(2, "0")}`;
}

/** "segunda-feira, 09 de novembro" -> "Segunda-feira, 09 de novembro" */
function maiusculaInicial(texto: string): string {
  return texto.charAt(0).toUpperCase() + texto.slice(1);
}

function formatarDataExtensa(data: Date): string {
  return data.toLocaleDateString("pt-BR", {
    weekday: "long",
    day: "2-digit",
    month: "long",
    year: "numeric",
  });
}

export function AgendarScreen() {
  const { token, usuario } = useAuth();

  const [passo, setPasso] = useState(0);

  const [pets, setPets] = useState<Pet[]>([]);
  const [servicos, setServicos] = useState<Servico[]>([]);
  const [profissionais, setProfissionais] = useState<Profissional[]>([]);

  // Seleções do usuário
  const [petSelecionado, setPetSelecionado] = useState<Pet | null>(null);
  const [servicoSelecionado, setServicoSelecionado] = useState<Servico | null>(null);
  const [profissionalSelecionado, setProfissionalSelecionado] = useState<Profissional | null>(null);
  const [dataSelecionada, setDataSelecionada] = useState<Date>(new Date());
  const [horarioSelecionado, setHorarioSelecionado] = useState<string | null>(null);
  const [observacoes, setObservacoes] = useState("");

  // Horários disponíveis
  const [horarios, setHorarios] = useState<string[]>([]);
  const [buscandoHorarios, setBuscandoHorarios] = useState(false);

  const [mostrarPickerData, setMostrarPickerData] = useState(false);
  const [salvando, setSalvando] = useState(false);
  const [carregandoInicial, setCarregandoInicial] = useState(true);

  // Categoria do serviço: Pet Shop (banho/tosa → profissional de Banho e Tosa) ou Veterinária (→ Veterinário)
  const isPetShop = servicoSelecionado?.nome.toLowerCase().includes("banho") ||
    servicoSelecionado?.nome.toLowerCase().includes("tosa") || false;

  const profissionaisFiltrados = profissionais.filter((f) =>
    isPetShop ? f.cargo === 3 : f.cargo === 0
  );

  // Carrega pets, serviços e profissionais uma única vez
  useEffect(() => {
    (async () => {
      try {
        const [petsDados, servicosDados, profsDados] = await Promise.all([
          token && usuario ? listarPetsDoTutor(usuario.id, token) : Promise.resolve([]),
          listarServicos(),
          listarProfissionais(),
        ]);
        setPets(petsDados);
        setServicos(servicosDados);
        setProfissionais(profsDados);
      } catch (e: any) {
        Alert.alert("Erro ao carregar dados", e.message ?? "Verifique se a API está no ar.");
      } finally {
        setCarregandoInicial(false);
      }
    })();
  }, [token, usuario]);

  // Busca horários quando data/serviço/profissional mudam
  const buscarHorarios = useCallback(
    async (data: Date) => {
      if (!servicoSelecionado || !profissionalSelecionado) return;
      setBuscandoHorarios(true);
      setHorarioSelecionado(null);
      try {
        const lista = await listarHorariosDisponiveis(
          formatarDataAPI(data),
          servicoSelecionado.id,
          profissionalSelecionado.id
        );
        setHorarios(lista);
      } catch (e: any) {
        Alert.alert("Erro", e.message ?? "Não foi possível buscar horários.");
        setHorarios([]);
      } finally {
        setBuscandoHorarios(false);
      }
    },
    [servicoSelecionado, profissionalSelecionado]
  );

  // Ao chegar no passo de data/hora, já carrega os horários da data exibida (hoje, por padrão)
  useEffect(() => {
    if (passo === 3) buscarHorarios(dataSelecionada);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [passo, buscarHorarios]);

  function avancar() {
    if (passo === 0 && !petSelecionado) {
      Alert.alert("Selecione", "Escolha o pet.");
      return;
    }
    if (passo === 1 && !servicoSelecionado) {
      Alert.alert("Selecione", "Escolha o serviço.");
      return;
    }
    if (passo === 2 && !profissionalSelecionado) {
      Alert.alert("Selecione", "Escolha o profissional.");
      return;
    }
    if (passo === 3 && !horarioSelecionado) {
      Alert.alert("Selecione", "Escolha um horário disponível.");
      return;
    }
    if (passo === 4) {
      confirmar();
      return;
    }
    setPasso(passo + 1);
  }

  function voltar() {
    if (passo === 0) return;
    setPasso(passo - 1);
  }

  function aoMudarData(event: DateTimePickerEvent, data?: Date) {
    // No Android o picker fecha sozinho; no iOS fica como modal
    if (Platform.OS === "android") setMostrarPickerData(false);
    if (data) {
      setDataSelecionada(data);
      setHorarioSelecionado(null);
      buscarHorarios(data);
    }
  }

  async function confirmar() {
    if (!token || !petSelecionado || !servicoSelecionado || !profissionalSelecionado || !horarioSelecionado) {
      return;
    }
    setSalvando(true);
    try {
      await criarAgendamento(
        {
          idPet: petSelecionado.id,
          idServico: servicoSelecionado.id,
          idFuncionario: profissionalSelecionado.id,
          dataHoraInicio: horarioSelecionado,
          observacoes: observacoes.trim(),
          status: 0, // PENDENTE
        },
        token
      );
      Alert.alert("Sucesso!", "Agendamento realizado com sucesso!", [
        {
          text: "OK",
          onPress: () => {
            // Reinicia o wizard
            setPasso(0);
            setPetSelecionado(null);
            setServicoSelecionado(null);
            setProfissionalSelecionado(null);
            setHorarioSelecionado(null);
            setObservacoes("");
          },
        },
      ]);
    } catch (e: any) {
      Alert.alert("Erro ao agendar", e.message ?? "Verifique os dados e tente novamente.");
    } finally {
      setSalvando(false);
    }
  }

  if (carregandoInicial) {
    return (
      <View style={styles.flex}>
        <Header />
        <View style={styles.center}>
          <Text style={styles.carregandoTexto}>Carregando dados...</Text>
        </View>
      </View>
    );
  }

  return (
    <View style={styles.flex}>
      <StatusBar style="light" />
      <Header />
      <ScrollView contentContainerStyle={styles.container}>
        {/* Indicador de progresso do wizard */}
        <View style={styles.progresso}>
          {PASSOS.map((p, i) => (
            <View key={p} style={styles.progressoItem}>
              <View style={[styles.bolinha, i <= passo && styles.bolinhaAtiva]}>
                <Text style={[styles.bolinhaTexto, i <= passo && styles.bolinhaTextoAtivo]}>
                  {i + 1}
                </Text>
              </View>
              {i < PASSOS.length - 1 && (
                <View style={[styles.linha, i < passo && styles.linhaAtiva]} />
              )}
            </View>
          ))}
        </View>
        <Text style={styles.passoTitulo}>{PASSOS[passo]}</Text>

        {/* PASSO 0 — PET */}
        {passo === 0 && (
          <>
            {pets.length === 0 ? (
              <EmptyState
                icon="🐶"
                title="Você ainda não tem pets"
                subtitle="Cadastre um pet na aba Pets antes de agendar."
              />
            ) : (
              pets.map((pet) => (
                <TouchableOpacity
                  key={pet.id}
                  style={[styles.optionCard, petSelecionado?.id === pet.id && styles.optionCardAtivo]}
                  onPress={() => setPetSelecionado(pet)}
                >
                  <Text style={styles.optionEmoji}>
                    {["🐶", "🐱", "🐦", "🐹", "🐾"][pet.especie] ?? "🐾"}
                  </Text>
                  <Text style={styles.optionTexto}>{pet.nome}</Text>
                  {petSelecionado?.id === pet.id && (
                    <Ionicons name="checkmark-circle" size={22} color={colors.primary} />
                  )}
                </TouchableOpacity>
              ))
            )}
          </>
        )}

        {/* PASSO 1 — SERVIÇO */}
        {passo === 1 && (
          <>
            <Text style={styles.categoria}>
              {servicoSelecionado ? "Escolhido" : "Escolha um serviço"}
            </Text>
            {servicos.map((s) => (
              <TouchableOpacity
                key={s.id}
                style={[styles.optionCard, servicoSelecionado?.id === s.id && styles.optionCardAtivo]}
                onPress={() => setServicoSelecionado(s)}
              >
                <View style={styles.optionInfo}>
                  <Text style={styles.optionTexto}>{s.nome}</Text>
                  <Text style={styles.optionSub}>
                    {s.duracaoMinutos} min · R$ {s.preco.toFixed(2).replace(".", ",")}
                  </Text>
                </View>
                {servicoSelecionado?.id === s.id && (
                  <Ionicons name="checkmark-circle" size={22} color={colors.primary} />
                )}
              </TouchableOpacity>
            ))}
          </>
        )}

        {/* PASSO 2 — PROFISSIONAL */}
        {passo === 2 && (
          <>
            <Text style={styles.categoria}>
              {isPetShop ? "Pet Shop — Banho e Tosa" : "Veterinária — Veterinários"}
            </Text>
            {profissionaisFiltrados.length === 0 ? (
              <EmptyState title="Nenhum profissional disponível" />
            ) : (
              profissionaisFiltrados.map((f) => (
                <TouchableOpacity
                  key={f.id}
                  style={[styles.optionCard, profissionalSelecionado?.id === f.id && styles.optionCardAtivo]}
                  onPress={() => setProfissionalSelecionado(f)}
                >
                  <View style={styles.avatar}>
                    <Text style={styles.avatarTexto}>{f.nome.charAt(0).toUpperCase()}</Text>
                  </View>
                  <Text style={styles.optionTexto}>{f.nome}</Text>
                  {profissionalSelecionado?.id === f.id && (
                    <Ionicons name="checkmark-circle" size={22} color={colors.primary} />
                  )}
                </TouchableOpacity>
              ))
            )}
          </>
        )}

        {/* PASSO 3 — DATA E HORÁRIO */}
        {passo === 3 && (
          <>
            <TouchableOpacity
              style={styles.dateButton}
              onPress={() => setMostrarPickerData(true)}
            >
              <Ionicons name="calendar-outline" size={22} color={colors.primary} />
              <Text style={styles.dateButtonTexto}>{maiusculaInicial(formatarDataExtensa(dataSelecionada))}</Text>
            </TouchableOpacity>

            {mostrarPickerData && (
              <DateTimePicker
                value={dataSelecionada}
                mode="date"
                minimumDate={new Date()}
                display={Platform.OS === "ios" ? "spinner" : "default"}
                onChange={aoMudarData}
              />
            )}

            {buscandoHorarios ? (
              <Text style={styles.carregandoTexto}>Buscando horários...</Text>
            ) : (
              <>
                {horarios.length === 0 ? (
                  <EmptyState
                    title="Nenhum horário disponível"
                    subtitle="Tente outra data."
                  />
                ) : (
                  <View style={styles.horariosGrid}>
                    {horarios.map((h) => {
                      const ativo = horarioSelecionado === h;
                      return (
                        <TouchableOpacity
                          key={h}
                          style={[styles.horarioChip, ativo && styles.horarioChipAtivo]}
                          onPress={() => setHorarioSelecionado(h)}
                        >
                          <Text style={[styles.horarioTexto, ativo && styles.horarioTextoAtivo]}>
                            {formatarHora(h)}
                          </Text>
                        </TouchableOpacity>
                      );
                    })}
                  </View>
                )}
              </>
            )}
          </>
        )}

        {/* PASSO 4 — RESUMO */}
        {passo === 4 && (
          <View style={styles.resumoCard}>
            <ResumoLinha rotulo="Pet" valor={petSelecionado?.nome ?? "-"} />
            <ResumoLinha rotulo="Serviço" valor={servicoSelecionado?.nome ?? "-"} />
            <ResumoLinha
              rotulo="Profissional"
              valor={`${profissionalSelecionado?.nome ?? "-"} (${isPetShop ? "Banho e Tosa" : "Veterinário"})`}
            />
            <ResumoLinha
              rotulo="Data"
              valor={`${formatarDataExtensa(dataSelecionada)} às ${horarioSelecionado ? formatarHora(horarioSelecionado) : "-"}`}
            />
            <AppInput
              label="Observações (opcional)"
              placeholder="Sintomas, recados..."
              multiline
              numberOfLines={3}
              value={observacoes}
              onChangeText={setObservacoes}
              style={styles.obsInput}
            />
          </View>
        )}

        {/* Navegação do wizard */}
        <View style={styles.nav}>
          {passo > 0 && (
            <AppButton title="Voltar" variant="outline" onPress={voltar} style={styles.navBtn} />
          )}
          <AppButton
            title={passo === 4 ? "Confirmar agendamento" : "Continuar"}
            onPress={avancar}
            loading={salvando || buscandoHorarios}
            style={[styles.navBtn, passo > 0 ? styles.navBtnPrincipal : undefined]}
          />
        </View>
      </ScrollView>
    </View>
  );
}

function Header() {
  return (
    <View style={styles.header}>
      <Text style={styles.headerTitulo}>Agendar serviço</Text>
      <Text style={styles.headerSub}>Wizard passo a passo</Text>
    </View>
  );
}

function ResumoLinha({ rotulo, valor }: { rotulo: string; valor: string }) {
  return (
    <View style={styles.resumoLinha}>
      <Text style={styles.resumoRotulo}>{rotulo}</Text>
      <Text style={styles.resumoValor}>{valor}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  flex: { flex: 1, backgroundColor: colors.background },
  center: { flex: 1, alignItems: "center", justifyContent: "center" },
  carregandoTexto: { color: colors.textMuted, fontSize: 15, marginTop: 16 },
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
  headerSub: {
    color: colors.white,
    fontSize: 13,
    opacity: 0.9,
    marginTop: 2,
  },
  container: {
    padding: 20,
    paddingBottom: 40,
  },
  progresso: {
    flexDirection: "row",
    alignItems: "center",
    marginBottom: 16,
  },
  progressoItem: {
    flexDirection: "row",
    alignItems: "center",
    flex: 1,
  },
  bolinha: {
    width: 30,
    height: 30,
    borderRadius: 15,
    borderWidth: 2,
    borderColor: colors.border,
    backgroundColor: colors.card,
    alignItems: "center",
    justifyContent: "center",
  },
  bolinhaAtiva: {
    borderColor: colors.primary,
    backgroundColor: colors.primary,
  },
  bolinhaTexto: {
    color: colors.textMuted,
    fontWeight: "700",
  },
  bolinhaTextoAtivo: {
    color: colors.white,
  },
  linha: {
    flex: 1,
    height: 2,
    backgroundColor: colors.border,
    marginHorizontal: 4,
  },
  linhaAtiva: {
    backgroundColor: colors.primary,
  },
  passoTitulo: {
    fontSize: 20,
    fontWeight: "800",
    color: colors.text,
    marginBottom: 16,
  },
  categoria: {
    fontSize: 14,
    fontWeight: "600",
    color: colors.textMuted,
    marginBottom: 10,
  },
  optionCard: {
    flexDirection: "row",
    alignItems: "center",
    backgroundColor: colors.card,
    borderRadius: 14,
    padding: 16,
    marginBottom: 10,
    borderWidth: 1.5,
    borderColor: colors.border,
  },
  optionCardAtivo: {
    borderColor: colors.primary,
    backgroundColor: colors.primaryLight,
  },
  optionEmoji: {
    fontSize: 24,
    marginRight: 12,
  },
  avatar: {
    width: 32,
    height: 32,
    borderRadius: 16,
    backgroundColor: colors.primary,
    alignItems: "center",
    justifyContent: "center",
    marginRight: 12,
  },
  avatarTexto: {
    color: colors.white,
    fontWeight: "700",
    fontSize: 15,
  },
  optionInfo: {
    flex: 1,
  },
  optionTexto: {
    fontSize: 16,
    fontWeight: "700",
    color: colors.text,
    flex: 1,
  },
  optionSub: {
    fontSize: 13,
    color: colors.textMuted,
    marginTop: 2,
  },
  dateButton: {
    flexDirection: "row",
    alignItems: "center",
    backgroundColor: colors.card,
    borderRadius: 14,
    borderWidth: 1.5,
    borderColor: colors.primary,
    padding: 16,
    marginBottom: 20,
  },
  dateButtonTexto: {
    fontSize: 16,
    fontWeight: "700",
    color: colors.text,
    marginLeft: 10,
  },
  horariosGrid: {
    flexDirection: "row",
    flexWrap: "wrap",
    marginTop: 8,
  },
  horarioChip: {
    borderWidth: 1.5,
    borderColor: colors.border,
    borderRadius: 12,
    paddingHorizontal: 18,
    paddingVertical: 12,
    marginRight: 10,
    marginBottom: 10,
    backgroundColor: colors.card,
  },
  horarioChipAtivo: {
    borderColor: colors.primary,
    backgroundColor: colors.primary,
  },
  horarioTexto: {
    fontSize: 15,
    fontWeight: "700",
    color: colors.text,
  },
  horarioTextoAtivo: {
    color: colors.white,
  },
  resumoCard: {
    backgroundColor: colors.card,
    borderRadius: 16,
    padding: 18,
    borderWidth: 1,
    borderColor: colors.border,
  },
  resumoLinha: {
    flexDirection: "row",
    justifyContent: "space-between",
    paddingVertical: 10,
    borderBottomWidth: 1,
    borderBottomColor: colors.border,
  },
  resumoRotulo: {
    color: colors.textMuted,
    fontSize: 14,
  },
  resumoValor: {
    color: colors.text,
    fontSize: 14,
    fontWeight: "700",
    flexShrink: 1,
    textAlign: "right",
    marginLeft: 12,
  },
  obsInput: {
    marginTop: 12,
    height: 90,
    textAlignVertical: "top",
  },
  nav: {
    flexDirection: "row",
    marginTop: 28,
  },
  navBtn: {
    flex: 1,
    marginHorizontal: 4,
  },
  navBtnPrincipal: {
    flex: 2,
  },
});
