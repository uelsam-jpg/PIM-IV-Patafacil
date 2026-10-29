/**
 * Cliente HTTP genérico que injeta automaticamente o Token JWT
 * em toda requisição autenticada (mesmo padrão do Frontend Web).
 */
import { API_URL, TIMEOUT_MS } from "../config/api";

export class ApiError extends Error {
  status: number;
  constructor(status: number, message: string) {
    super(message);
    this.status = status;
  }
}

/** Extrai a mensagem de erro que o C# retorna (mensagem / erro / title / errors) */
function extrairMensagemErro(data: any): string {
  if (!data) return "Erro desconhecido retornado pelo servidor.";
  if (typeof data === "string") return data;
  if (data.mensagem) return data.mensagem;
  if (data.erro) return data.erro;
  if (data.Erro) return data.Erro;
  if (data.title) return data.title;
  if (data.errors) {
    // Erros automáticos de validação do ASP.NET (ex: campo obrigatório)
    const valores: any[] = Object.values(data.errors);
    return valores.flat().join("\n");
  }
  return "Erro desconhecido retornado pelo servidor.";
}

interface RequestOptions {
  method?: "GET" | "POST" | "PUT" | "DELETE" | "PATCH";
  body?: any;
  token?: string | null;
  /** Envia o corpo como JSON (padrão true) */
  json?: boolean;
}

/** Função principal: faz a requisição e devolve o JSON já tratado. */
export async function apiRequest<T = any>(
  path: string,
  { method = "GET", body, token, json = true }: RequestOptions = {}
): Promise<T> {
  const controller = new AbortController();
  const timeout = setTimeout(() => controller.abort(), TIMEOUT_MS);

  const headers: Record<string, string> = {};
  if (json && body !== undefined) headers["Content-Type"] = "application/json";
  if (token) headers["Authorization"] = `Bearer ${token}`;

  try {
    const response = await fetch(`${API_URL}${path}`, {
      method,
      headers,
      body: json && body !== undefined ? JSON.stringify(body) : undefined,
      signal: controller.signal,
    });

    let data: any = null;
    const texto = await response.text();
    if (texto) {
      try {
        data = JSON.parse(texto);
      } catch {
        data = texto;
      }
    }

    if (!response.ok) {
      throw new ApiError(response.status, extrairMensagemErro(data));
    }

    return data as T;
  } catch (e: any) {
    if (e.name === "AbortError") {
      throw new ApiError(0, "A requisição demorou demais. Verifique se a API está no ar.");
    }
    if (e instanceof ApiError) throw e;
    throw new ApiError(0, "Falha de comunicação com o servidor. Verifique sua conexão.");
  } finally {
    clearTimeout(timeout);
  }
}
