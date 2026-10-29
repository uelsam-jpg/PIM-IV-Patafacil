# ETAPA 5 — DESENVOLVIMENTO DA SOLUÇÃO MOBILE

**Disciplina:** Desenvolvimento Mobile
**Sistema:** PataFácil — Gestão integrada para Pet Shop e Clínica Veterinária
**Implementação:** pasta `Mobile/` (React Native + Expo + TypeScript)

> Texto-base desta etapa. A versão final, com as figuras, está no capítulo 6 do
> relatório (`PIM_IV_PataFacil_Relatorio_Final.docx` / `.pdf`).

## 5.1 Telas principais

| Tela | Arquivo | Função |
|---|---|---|
| Login | `LoginScreen.tsx` | Autenticação com e-mail e senha |
| Cadastro | `RegisterScreen.tsx` | Criação da conta do tutor |
| Início | `HomeScreen.tsx` | Atalhos para pets, agendamento e consultas |
| Pets | `PetsScreen.tsx` | Lista os animais do tutor |
| Novo Pet | `AddPetScreen.tsx` | Cadastro de animal (espécie, sexo, peso, nascimento) |
| Agendar | `AgendarScreen.tsx` | Assistente em 5 passos: pet, serviço, profissional, data/hora e resumo |
| Consultas | `MeusAgendamentosScreen.tsx` | Agendamentos com status e cancelamento |
| Perfil | `PerfilScreen.tsx` | Dados do usuário e saída da conta |

As capturas das telas estão em `Documentacao/figuras/mob_*.png`.

## 5.2 Fluxo de navegação

- Ao abrir, o app verifica se há sessão salva no aparelho.
- Sem sessão: pilha de acesso (Login ⇄ Cadastro).
- Com sessão: painel do tutor, com abas inferiores (Início, Pets, Agendar, Consultas, Perfil).
- "Novo Pet" é aberto a partir da aba Pets; "Sair", no Perfil, volta ao Login.

Diagrama: `Documentacao/figuras/dia_navegacao.png`.

## 5.3 Autenticação

- Mesmo endpoint da web: `POST /api/auth/login`.
- O token JWT (validade de 8 horas) e os dados do usuário ficam no AsyncStorage; a sessão
  é restaurada ao reabrir o app e apagada ao sair.
- O `AuthContext` disponibiliza o token às telas e o `apiClient` o envia no cabeçalho
  `Authorization: Bearer <token>`.

## 5.4 Sincronização de dados

- A API é a fonte única dos dados: não há cópia local de pets ou agendamentos.
- As listas são recarregadas quando a tela recebe o foco e ao puxar a lista para baixo
  (pull-to-refresh); após criar ou cancelar um agendamento a consulta é refeita.
- No passo de data/hora, os horários livres são buscados na API ao entrar no passo e a
  cada troca de data.
- Requisições com tempo limite de 15 segundos e mensagens de erro claras.

## 5.5 Integração com APIs

| Tela | Endpoints |
|---|---|
| Login e cadastro | `POST /api/auth/login`, `POST /api/tutores` |
| Pets e novo pet | `GET /api/pets/tutor/{id}`, `POST /api/pets` |
| Agendar | `GET /api/servicos`, `GET /api/funcionarios/profissionais`, `GET /api/agendamento/horarios-disponiveis`, `POST /api/agendamento` |
| Consultas | `GET /api/agendamento/tutor/{id}`, `DELETE /api/agendamento/{id}` |

As regras de negócio (RN01, RN02, RN03, RN04, RN10) são validadas no servidor, portanto
valem igualmente para o aplicativo e para a aplicação web. O tutor só acessa os próprios
pets e agendamentos.
