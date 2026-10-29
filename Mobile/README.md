# PataFácil Mobile (PIM IV — Etapa 5: Desenvolvimento Mobile)

Aplicativo móvel do **Tutor** para o sistema PataFácil, construído com
**React Native + Expo** e integrado à API C# (ASP.NET Core) que já existe no
projeto.

O app é o complemento mobile da solução web: o tutor pode **entrar, cadastrar
pets, agendar serviços (wizard), acompanhar e cancelar consultas**, tudo
consumindo as mesmas APIs REST do sistema.

---

## 🛠 Requisitos

- **Node.js** (versão 18 ou superior) — já instalado no ambiente (v21)
- **Expo Go** instalado no celular (Android ou iPhone) para testar sem build
- Backend **C# rodando** (pasta `PataFacil`, com `dotnet run`) e o banco MySQL ativo

---

## 🚀 Como rodar

```bash
# 1. Instalar as dependências (primeira vez)
cd Mobile
npm install

# 2. Subir o servidor de desenvolvimento do Expo
npx expo start
```

Um QR Code aparece no terminal. Abra o **Expo Go** no celular e escaneie o QR Code.

Outras formas de abrir:
```bash
npx expo start --android   # Abre no emulador Android
npx expo start --web       # Abre no navegador (versão web)
```

---

## ⚙️ Configurando o endereço da API

Edite o arquivo `src/config/api.ts`:

| Onde está testando | Valor de `API_URL` |
|---|---|
| Navegador (expo web) | `http://localhost:5288/api` |
| Emulador Android | `http://10.0.2.2:5288/api` |
| Celular físico (Expo Go) | `http://SEU_IP_DA_REDE:5288/api` (veja com `ipconfig`) |

> No celular físico, o celular e o PC precisam estar na **mesma rede Wi-Fi** e a
> porta `5288` liberada no firewall do Windows.

---

## 📱 Telas do aplicativo

| Tela | Descrição |
|---|---|
| **Login** | Autenticação com e-mail e senha (JWT) |
| **Cadastro** | Criação de conta do Tutor |
| **Início** | Painel com atalhos (pets, agendar, consultas) |
| **Pets** | Lista os pets do tutor (atualização por puxar a tela) |
| **Novo Pet** | Cadastro com espécie, sexo, peso e data de nascimento |
| **Agendar** | Wizard em 5 passos: Pet → Serviço → Profissional → Data/Horário → Resumo |
| **Minhas consultas** | Lista agendamentos com status e cancelamento |
| **Perfil** | Dados do usuário e sair da conta |

---

## 🔌 APIs consumidas (integração)

| Método | Endpoint | Uso |
|---|---|---|
| POST | `/api/auth/login` | Login (retorna Token JWT) |
| POST | `/api/tutores` | Cadastro do tutor |
| GET | `/api/Pets/tutor/{id}` | Lista pets do tutor |
| POST | `/api/pets` | Cadastra pet |
| GET | `/api/Servicos` | Lista serviços |
| GET | `/api/funcionarios/profissionais` | Lista profissionais |
| GET | `/api/agendamento/horarios-disponiveis` | Horários livres |
| POST | `/api/agendamento` | Cria agendamento |
| GET | `/api/agendamento/tutor/{id}` | Agendamentos do tutor |
| DELETE | `/api/agendamento/{id}` | Cancela agendamento |

> As requisições autenticadas enviam o header `Authorization: Bearer <token>`
> (mesmo padrão do Frontend Web).

---

## 🧭 Estrutura de pastas

```
Mobile/
├─ App.tsx                      # Entrada: AuthProvider + Navegação
├─ src/
│  ├─ config/api.ts             # Endereço da API
│  ├─ theme/colors.ts           # Paleta de cores (identidade PataFácil)
│  ├─ services/                 # Camada de integração com a API
│  │  ├─ apiClient.ts           # Cliente HTTP com JWT
│  │  ├─ authService.ts         # Login e cadastro
│  │  ├─ petService.ts          # Pets
│  │  └─ agendamentoService.ts  # Serviços, profissionais, agendamentos
│  ├─ context/AuthContext.tsx   # Sessão persistida no dispositivo
│  ├─ navigation/               # Pilhas e abas de navegação
│  ├─ components/               # Botões, inputs, cards reutilizáveis
│  └─ screens/                  # Telas do aplicativo
└─ ...
```

---

## 🗂 Sincronização de dados

- A **sessão** (token + usuário) é salva no dispositivo (AsyncStorage): o usuário
  continua logado ao reabrir o app.
- As listas (pets e agendamentos) são **sincronizadas com a API** ao abrir a tela
  e por **pull-to-refresh** (puxar a tela para baixo), garantindo dados sempre
  atualizados.

---

## 📝 Observação para o PIM IV

Este app cobre a **Etapa 5 – Desenvolvimento da solução Mobile** do manual:
telas principais, fluxo de navegação, autenticação, sincronização de dados e
integração com as APIs.
