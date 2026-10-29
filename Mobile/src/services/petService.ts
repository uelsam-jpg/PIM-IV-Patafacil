import { apiRequest } from "./apiClient";

/** Enum Espécie (mesmos valores numéricos do enum C# Models/Pet.cs) */
export enum Especie {
  Cachorro = 0,
  Gato = 1,
  Ave = 2,
  Roedor = 3,
  Outro = 4,
}

/** Enum Sexo (mesmos valores do enum C# Models/Pet.cs) */
export enum Sexo {
  Macho = 0,
  Femea = 1,
}

export interface Pet {
  id: number;
  nome: string;
  especie: Especie;
  raca: string;
  sexo: Sexo;
  dataNascimento: string | null;
  pesoKg: number;
  fotoUrl: string;
  observacoes: string;
  ativo: boolean;
  tutorId: number;
}

export interface CriarPetInput {
  nome: string;
  especie: Especie;
  raca: string;
  sexo: Sexo;
  dataNascimento: string;
  pesoKg: number;
  observacoes: string;
  ativo: boolean;
  tutorId: number;
}

/** GET /api/Pets/tutor/{tutorId} — exige Token JWT */
export async function listarPetsDoTutor(tutorId: number, token: string): Promise<Pet[]> {
  return apiRequest<Pet[]>(`/Pets/tutor/${tutorId}`, { token });
}

/** POST /api/pets — cadastra um novo pet */
export async function criarPet(dados: CriarPetInput, token: string): Promise<{ id: number; mensagem: string }> {
  return apiRequest("/pets", {
    method: "POST",
    body: dados,
    token,
  });
}
