# ETAPA 4 — DESENVOLVIMENTO DA SOLUÇÃO WEB

> **Disciplina:** Desenvolvimento Web com .NET
> **Sistema:** PataFácil — Gestão integrada para Pet Shop e Clínica Veterinária
> **Finalidade:** Conteúdo-base para o capítulo correspondente do PIM IV (formato ABNT)

---

## 4.1 Arquitetura da aplicação

A solução web foi construída seguindo o princípio da **arquitetura em camadas
(arquitetura em três camadas estendida)**, separando responsabilidades e
facilitando a manutenção e a evolução do sistema.

```
┌─────────────────────────────────────────────────────────────┐
│  APRESENTAÇÃO (Frontend)                                    │
│  HTML5 + CSS3 + JavaScript puro (Frontend/)                 │
│  index, portal, dashboard, agendamento, veterinario,        │
│  atendente, estoque, gerente                                │
└───────────────────────────┬─────────────────────────────────┘
                            │ HTTP / JSON (REST)
┌───────────────────────────▼─────────────────────────────────┐
│  API (Controllers/) — Portas de entrada da API              │
│  Auth, Tutores, Pets, Servicos, Funcionarios, Agendamento,  │
│  HistoricoClinico, Estoque, Vendas, Relatorios, ML          │
└───────────────────────────┬─────────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────────┐
│  BLL (Negócios) — Regras de negócio                         │
│  AgendamentoService, VendaService, RelatorioService,        │
│  HistoricoClinicoService, EmailService                      │
└───────────────────────────┬─────────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────────┐
│  DAL (Dados) — Acesso ao banco                              │
│  PataFacilContext (EF Core) + Padrão Repository (IRepositorio)│
│  MySQL                                                  │
└─────────────────────────────────────────────────────────────┘
        │                                        │
        ▼                                        ▼
   Banco de Dados (MySQL)          Microsserviço de IA (Python/FastAPI)
   Tabelas: Tutor, Pet, Funcionario,      3 modelos: Risco de cancelamento,
   Servico, Agendamento, Historico         Previsão de demanda, Segmentação
   Clinico, Produto, Estoque, Venda,      de clientes
   ItemVenda
```

### Camadas da solução

| Camada | Pasta | Responsabilidade |
|---|---|---|
| **Apresentação** | `Frontend/` | Interfaces gráficas (HTML/CSS/JS) consumidas no navegador |
| **API** | `Controllers/` | Recebe requisições HTTP e orquestra as chamadas ao negócio |
| **Negócios** | `BLL/` | Regras de negócio (conflito de horário, fluxo de status, estoque, relatórios) |
| **Dados** | `DAL/` | Acesso ao banco com **Padrão Repository** e Entity Framework Core |
| **Modelos** | `Models/` | Classes que estruturam as tabelas (Orientação a Objetos) |
| **IA (microsserviço)** | `MachineLearning/` | API Python/FastAPI com modelos de Machine Learning |

### Destaques de arquitetura
- **Injeção de Dependência:** os controllers recebem as dependências via
  construtor (ex.: `AgendamentoService`, `IRepositorio<T>`), desacoplando as
  camadas e facilitando testes;
- **Padrão Repository:** uma interface `IRepositorio<T>` (genérica) com
  implementação `Repositorio<T>` centraliza o acesso a dados;
- **Orientação a Objetos:** herança (`Tutor : Pessoa`), polimorfismo
  (`GetPerfil()`) e encapsulamento (ex.: o status do agendamento só muda por
  métodos `Confirmar()`, `Cancelar()`, `Concluir()`);
- **Modularização:** cada módulo de negócio possui seu próprio controller e
  service.

## 4.2 APIs REST

A API segue o padrão **REST**, utilizando os verbos HTTP adequados e
retornando **JSON**. A base da API é `http://localhost:5288/api`.

| Recurso | Método | Rota | Descrição |
|---|---|---|---|
| Autenticação | POST | `/api/auth/login` | Login e geração do Token JWT |
| Tutores | POST | `/api/tutores` | Cadastro de tutor |
| Tutores | GET | `/api/tutores/{id}` | Consulta tutor seguro |
| Tutores | GET | `/api/tutores` | Lista tutores (dados seguros) |
| Pets | POST | `/api/pets` | Cadastra pet |
| Pets | GET | `/api/Pets/tutor/{tutorId}` | Lista pets de um tutor |
| Serviços | GET | `/api/Servicos` | Lista serviços ativos |
| Funcionários | GET | `/api/funcionarios/profissionais` | Lista profissionais (atendente/veterinário) |
| Funcionários | POST/PUT/DELETE | `/api/funcionarios` | Gestão de equipe (Gerente) |
| Agendamento | POST | `/api/agendamento` | Cria agendamento (RN01, RN03, RN10) |
| Agendamento | DELETE | `/api/agendamento/{id}` | Cancela (RN02) |
| Agendamento | GET | `/api/agendamento/dia` | Agenda do dia |
| Agendamento | GET | `/api/agendamento/semana` | Agenda da semana |
| Agendamento | GET | `/api/agendamento/tutor/{id}` | Agendamentos do tutor |
| Agendamento | PATCH | `/api/agendamento/{id}/confirmar` | Confirma (RN04) |
| Agendamento | PATCH | `/api/agendamento/{id}/concluir` | Conclui (RN04) |
| Agendamento | GET | `/api/agendamento/horarios-disponiveis` | Horários livres por data/serviço/profissional |
| Histórico Clínico | POST | `/api/HistoricoClinico` | Registra prontuário |
| Histórico Clínico | GET | `/api/HistoricoClinico/pet/{idPet}` | Histórico do pet |
| Estoque | GET | `/api/estoque/critico` | Produtos abaixo do ponto de reposição |
| Estoque | GET/POST | `/api/estoque/produtos` | Consulta e cadastro de produtos |
| Estoque | POST | `/api/estoque/{id}/entrada` | Reposição de estoque |
| Vendas | POST | `/api/Vendas` | Registra venda (baixa de estoque) |
| Relatórios | GET | `/api/relatorios/receita-mensal` | Receita mensal |
| Relatórios | GET | `/api/relatorios/dashboard-kpis` | KPIs do dashboard |
| IA | GET | `/api/ml/previsao-demanda-hoje` | Previsão de demanda via microsserviço Python |

## 4.3 Autenticação

A autenticação é baseada em **Token JWT (JSON Web Token)**, seguindo o padrão de
segurança da web:

1. O usuário informa **e-mail e senha** em `POST /api/auth/login`;
2. O sistema valida as credenciais e gera um **Token JWT assinado**, contendo as
   *claims* de identificação, e-mail e **papel (role)** do usuário;
3. O token possui **validade de 8 horas** (exigência de segurança RNF02);
4. Nas requisições protegidas, o frontend envia o header
   `Authorization: Bearer <token>`;
5. O middleware de autenticação valida a assinatura e a expiração do token a
   cada requisição.

### Perfis de acesso (Roles)
- **Tutor:** agenda e acompanha seus próprios serviços;
- **Atendente:** gerencia a agenda do dia e vendas;
- **Veterinário:** registra e consulta prontuários;
- **Gerente:** acessa relatórios, KPIs e gestão de equipe.

## 4.4 Segurança

As principais medidas de segurança implementadas:

- **Criptografia de senhas (BCrypt):** as senhas nunca são armazenadas em texto
  puro; são gravadas com *hash* BCrypt (custo 12) no banco;
- **Autorização por papel:** os controllers utilizam `[Authorize(Roles = "...")]`
  para restringir endpoints. Por exemplo, apenas o **Gerente** acessa a gestão
  de funcionários, e relatórios financeiros **recusam (403)** acessos de Tutor;
- **Expiração de token:** sessão limitada a 8 horas, reduzindo a janela de
  exploração de tokens vazados;
- **Proteção de dados sensíveis:** os endpoints de listagem de tutores e
  funcionários retornam apenas dados seguros (nunca *hashes* de senha ou dados
  sensíveis), aplicando o princípio do **menor privilégio**;
- **Validação de entrada:** as regras de negócio validam datas, horários e
  campos obrigatórios antes de persistir (ex.: impedir agendamento no passado,
  em horário fora do expediente ou aos domingos — RN03);
- **CORS configurado:** política de permissão de origem para que o frontend
  possa consumir a API;
- **Tratamento de erros:** mensagens de erro padronizadas sem expor detalhes
  internos do servidor.

## 4.5 Estrutura administrativa

A aplicação web oferece painéis administrativos específicos, cada um com
funcionalidades adequadas ao seu perfil:

| Painel | Arquivo | Funcionalidades |
|---|---|---|
| **Tutor (Portal)** | `portal.html` | Pets, agendamentos, cancelamento online |
| **Agendamento** | `agendamento.html` | Wizard de agendamento (passo a passo) |
| **Dashboard (Visão Geral)** | `dashboard.html` | Indicadores e KPIs gerenciais |
| **Veterinário** | `veterinario.html` | Registro de prontuários clínicos |
| **Atendente** | `atendente.html` | Agenda do dia, check-in |
| **Estoque** | `estoque.html` | Inventário, vendas (carrinho) e alertas de reposição |
| **Gerente** | `gerente.html` | Relatórios financeiros e cadastro de equipe |

Essa separação garante que **cada papel enxergue apenas o que lhe compete**,
reforçando a segurança e a organização administrativa.

## 4.6 Integração entre módulos

A solução integra **módulos internos e sistemas externos** de forma coesa:

1. **Frontend ↔ API:** as telas consomem os endpoints REST via `fetch`,
   enviando o token JWT nas requisições autenticadas;
2. **API ↔ Banco de Dados:** o Entity Framework Core persiste e consulta os
   dados no MySQL por meio do Padrão Repository;
3. **C# ↔ Microsserviço de IA (Python):** o `MLController` chama a API
   FastAPI (`http://127.0.0.1:8000`) via `HttpClientFactory`, obtendo previsões
   de demanda exibidas no dashboard;
4. **Notificações por e-mail:** o `EmailService` envia confirmações de
   agendamento para tutores e equipe, integrando o módulo de comunicação;
5. **Estoque ↔ Vendas:** ao registrar uma venda (`VendaService`), o estoque é
   **abatido automaticamente**; se o saldo atingir o ponto de reposição, o
   sistema dispara o alerta (RN15) exibido para Gerente e Atendente;
6. **Clínica ↔ Agenda:** o histórico clínico vincula-se ao agendamento e ao pet,
   mantendo o prontuário coerente com os atendimentos realizados.

### Fluxo de integração do agendamento (exemplo)
1. Tutor acessa o portal e escolhe pet, serviço e profissional;
2. A API consulta `horarios-disponiveis`, calculando os horários livres a partir
   da **duração do serviço** (RN10) e da agenda do profissional (RN01);
3. O tutor confirma; a API valida expediente (RN03) e cria o agendamento com
   status **Pendente**;
4. A API envia e-mail de confirmação e o agendamento passa a ser visível nos
   painéis do Atendente e do Veterinário;
5. O fluxo segue de forma travada: **Pendente → Confirmado → Concluído** (RN04),
   com cancelamento respeitando a antecedência mínima de 2 horas (RN02).

---

> **Uso na monografia:** este conteúdo corresponde ao capítulo "Desenvolvimento
> da Solução Web" do desenvolvimento do PIM IV. Complemente com diagramas
> (arquitetura, MER de APIs) e referências ABNT antes da entrega final.
