import { apiRequest } from "./apiClient";

export interface Servico {
  id: number;
  nome: string;
  descricao: string;
  duracaoMinutos: number;
  preco: number;
  ativo: boolean;
}

/** Cargo: 0 = Veterinario, 1 = Atendente, 2 = Gerente, 3 = Tosador (enum C# Cargo.cs) */
export interface Profissional {
  id: number;
  nome: string;
  cargo: number;
}

/** Status: 0 = PENDENTE, 1 = CONFIRMADO, 2 = CONCLUIDO, 3 = CANCELADO */
export enum StatusAgendamento {
  PENDENTE = 0,
  CONFIRMADO = 1,
  CONCLUIDO = 2,
  CANCELADO = 3,
}

export interface Agendamento {
  id: number;
  dataHoraInicio: string;
  dataHoraFim: string;
  observacoes?: string | null;
  dataCriacao: string;
  idPet: number;
  idServico: number;
  idFuncionario: number;
  status: StatusAgendamento;
}

export interface CriarAgendamentoInput {
  idPet: number;
  idServico: number;
  idFuncionario: number;
  dataHoraInicio: string;
  observacoes: string;
  status: number; // 0 = PENDENTE
}

/** GET /api/Servicos — lista os serviços ativos */
export async function listarServicos(): Promise<Servico[]> {
  return apiRequest<Servico[]>("/Servicos");
}

/** GET /api/funcionarios/profissionais — lista os profissionais disponíveis */
export async function listarProfissionais(): Promise<Profissional[]> {
  return apiRequest<Profissional[]>("/funcionarios/profissionais");
}

/** GET /api/agendamento/horarios-disponiveis?data=...&idServico=...&idFuncionario=... */
export async function listarHorariosDisponiveis(
  data: string, // yyyy-MM-dd
  idServico: number,
  idFuncionario: number
): Promise<string[]> {
  const query = `data=${encodeURIComponent(data)}&idServico=${idServico}&idFuncionario=${idFuncionario}`;
  return apiRequest<string[]>(`/agendamento/horarios-disponiveis?${query}`);
}

/** GET /api/agendamento/tutor/{idTutor} — lista os agendamentos do tutor */
export async function listarAgendamentosDoTutor(idTutor: number, token: string): Promise<Agendamento[]> {
  return apiRequest<Agendamento[]>(`/agendamento/tutor/${idTutor}`, { token });
}

/** POST /api/agendamento — cria um novo agendamento (exige JWT) */
export async function criarAgendamento(
  dados: CriarAgendamentoInput,
  token: string
): Promise<{ Mensagem?: string; mensagem?: string; Id?: number; id?: number }> {
  return apiRequest("/agendamento", {
    method: "POST",
    body: dados,
    token,
  });
}

/** DELETE /api/agendamento/{id} — cancela um agendamento (exige JWT) */
export async function cancelarAgendamento(id: number, token: string): Promise<any> {
  return apiRequest(`/agendamento/${id}`, {
    method: "DELETE",
    token,
  });
}
