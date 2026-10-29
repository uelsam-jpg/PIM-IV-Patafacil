import { apiRequest } from "./apiClient";

export interface Usuario {
  id: number;
  nome: string;
  email: string;
  perfil: string;
}

export interface LoginResponse {
  token: string;
  usuario: Usuario;
}

export interface CadastroTutorInput {
  nome: string;
  cpf: string;
  email: string;
  senhaHash: string; // o campo é "SenhaHash" no C#; o binding aceita camelCase
  telefone: string;
  logradouro: string;
  numero: string;
  bairro: string;
  cidade: string;
  uf: string;
  cep: string;
}

/** POST /api/auth/login */
export async function login(email: string, senha: string): Promise<LoginResponse> {
  return apiRequest<LoginResponse>("/auth/login", {
    method: "POST",
    body: { email, senha },
  });
}

/** POST /api/tutores — cria a conta do Tutor */
export async function cadastrarTutor(dados: CadastroTutorInput): Promise<{ id: number; mensagem: string }> {
  return apiRequest("/tutores", {
    method: "POST",
    body: dados,
  });
}
