/**
 * CONFIGURAÇÃO DA API PataFácil (Backend C# ASP.NET Core)
 *
 * ATENÇÃO — Como testar no celular:
 * - Emulador Android: use http://10.0.2.2:5288/api  (10.0.2.2 = "localhost" do PC visto do emulador)
 * - Celular físico (Expo Go): use o IP da sua máquina na rede local, ex: http://192.168.0.10:5288/api
 *   (descubra com `ipconfig` no Windows e certifique-se de que o firewall permite a porta 5288)
 * - Navegador Web (expo start --web): use http://localhost:5288/api
 *
 * O backend deve estar rodando na pasta PataFacil com:  dotnet run
 */
export const API_URL = "http://localhost:5288/api";

/** Tempo máximo de espera para cada requisição (evita travamento do app) */
export const TIMEOUT_MS = 15000;
