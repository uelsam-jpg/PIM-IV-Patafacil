# ETAPA 2 — PLANEJAMENTO DA SOLUÇÃO TECNOLÓGICA

> **Disciplina:** Empreendedorismo em TI
> **Sistema:** PataFácil — Gestão integrada para Pet Shop e Clínica Veterinária
> **Finalidade:** Conteúdo-base para o capítulo correspondente do PIM IV (formato ABNT)

---

## 2.1 Problema a ser resolvido

Pequenos e médios pet shops e clínicas veterinárias ainda gerenciam seus
processos de forma **manual e fragmentada**: agenda em papel ou planilhas,
controle de estoque sem automatização, prontuários clínicos espalhados e
atendimento ao cliente restrito ao telefone ou presencialmente. Essa realidade
gera uma série de problemas concretos:

- **Choque de horários:** sem uma agenda centralizada, dois serviços podem ser
  marcados para o mesmo profissional no mesmo intervalo, causando atrasos e
  perda de receita;
- **Faltas (no-show) não previstas:** o estabelecimento não tem como estimar
  quais tutores têm maior chance de faltar, comprometendo o aproveitamento da
  agenda;
- **Falhas no estoque:** produtos importantes (rações, medicamentos, itens de
  higiene) esgotam sem aviso prévio, e as vendas não são integradas à baixa
  automática de estoque;
- **Histórico clínico disperso:** informações sobre vacinas, exames e
  prescrições dos pets ficam perdidas em papel, dificultando o acompanhamento;
- **Pouca transparência para o cliente:** o tutor não acompanha o status dos
  agendamentos nem tem autonomia para agendar ou cancelar serviços online;
- **Decisões sem dados:** o gerente não dispõe de indicadores financeiros e de
  demanda em tempo real para planejar equipe, promoções e compras.

O problema central, portanto, é a **ausência de uma solução tecnológica
integrada** que automatize o fluxo operacional (agenda, clínica, estoque e
vendas) e, ao mesmo tempo, aproxime o cliente (tutor) do estabelecimento por
meio de canais digitais web e mobile.

## 2.2 Objetivos da solução

### Objetivo geral
Desenvolver e implantar um sistema corporativo integrado — **PataFácil** — que
digitalize e automatize os processos de um pet shop e clínica veterinária,
unificando agenda de serviços, histórico clínico, controle de estoque, vendas e
relatórios gerenciais, com acesso por aplicação web e aplicativo mobile, e apoio
de inteligência artificial para previsão de demandas e comportamento de clientes.

### Objetivos específicos
- Automatizar o agendamento de serviços, garantindo que **não existam conflitos
  de horário** para o mesmo profissional (Regra de Negócio RN01);
- Permitir que o **tutor agende, acompanhe e cancele serviços online**, com
  regras claras de antecedência (RN02) e de horário de funcionamento (RN03);
- Controlar o **fluxo do agendamento** de forma travada: Pendente → Confirmado
  → Concluído (RN04), impedindo inversões acidentais de etapa;
- Manter o **histórico clínico** do pet (consultas, vacinas, exames, prescrições)
  organizado e acessível aos veterinários;
- Integrar **vendas e estoque**, com baixa automática do saldo e alerta de
  ponto de reposição (RN15);
- Gerar **relatórios financeiros e KPIs** em tempo real para o gerente;
- Oferecer um **aplicativo mobile** que complemente a experiência do tutor,
  consumindo as mesmas APIs REST do sistema;
- Utilizar **aprendizado de máquina** para prever risco de cancelamento,
  prever demanda diária e segmentar clientes.

## 2.3 Público-alvo

O público-alvo da solução é composto por **dois grupos principais**:

1. **Clientes (Tutores):** donos de cães, gatos, aves e outros pets que desejam
   conveniência — agendar banho e tosa, consultas e vacinas, acompanhar o
   histórico do animal e se comunicar com a clínica por canais digitais
   (web e mobile).
2. **Colaboradores do estabelecimento:**
   - **Atendente:** gerencia a agenda do dia, realiza check-in e atende vendas
     no balcão;
   - **Veterinário:** registra prontuários e conduz atendimentos clínicos;
   - **Gerente:** acompanha indicadores, relatórios financeiros e cadastra a
     equipe.

O perfil sociodemográfico típico é de tutores de **classe média urbana**, com
acesso a smartphones, que valorizam praticidade, agilidade e o bem-estar dos
animais. Para a equipe, o sistema atende usuários com perfis técnicos variados,
por isso a interface foi projetada de forma simples, objetiva e acessível.

## 2.4 Proposta de valor

A proposta de valor do PataFácil pode ser resumida em: **"Todos os cuidados com
o seu pet em um só lugar — agendamento, histórico e acompanhamento — com
processos internos automatizados para o estabelecimento."**

Para o **tutor**:
- Agendar serviços a qualquer hora, pelo computador ou celular;
- Escolher profissional, data e horário com **horários reais disponíveis**
  (sem fila e sem retrabalho);
- Acompanhar o status do agendamento e cancelar com antecedência;
- Manter o histórico clínico do pet sempre acessível.

Para o **estabelecimento**:
- Agenda inteligente que **impede conflitos de horário** automaticamente;
- **Inteligência artificial** que prevê risco de faltas e dias de maior demanda;
- Estoque integrado às vendas, com alerta automático de reposição;
- Relatórios financeiros e **KPIs em tempo real** para decisão gerencial;
- Redução de retrabalho, erros manuais e custo operacional.

## 2.5 Benefícios esperados

| Benefício | Como o sistema entrega |
|---|---|
| Redução de conflitos de agenda | Regra RN01 valida sobreposição de horários por profissional |
| Menos faltas (no-show) | ML prevê risco de cancelamento e antecipa avisos |
| Menos ruptura de estoque | Baixa automática + alerta de ponto de reposição (RN15) |
| Melhor experiência do cliente | Portal do tutor + app mobile (agendar, cancelar, acompanhar) |
| Decisão baseada em dados | KPIs e relatórios mensais em tempo real |
| Prontuário organizado | Histórico clínico digital por pet |
| Segurança da informação | Login com JWT, senhas com BCrypt e perfis de acesso |
| Redução de custo operacional | Automação de tarefas repetitivas e eliminação de papel |

## 2.6 Diferenciais competitivos

- **Integração de inteligência artificial:** o sistema não se limita a registrar
  dados; ele **prevê comportamento** (risco de cancelamento, demanda diária e
  segmentação de clientes), o que é raro em soluções de pequeno porte;
- **Omnicanalidade:** web e mobile compartilhando as **mesmas APIs REST**, o
  que garante consistência de dados entre os canais;
- **Regras de negócio fortes:** conflito de horário, fluxo travado de status e
  janela de cancelamento são garantidos pelo **backend**, e não apenas pela
  interface;
- **Arquitetura em camadas (BLL/DAL/Controllers/Models):** facilita manutenção,
  testes e evolução do sistema;
- **Baixo custo de implantação:** tecnologias open source (ASP.NET Core, MySQL,
  Python/FastAPI, React Native/Expo) e execução local simples;
- **Acessibilidade integrada:** plugin de **Libras (VLibras)** e preocupação com
  acessibilidade nos canais digitais.

## 2.7 Modelo de negócio e análise de viabilidade

### 2.7.1 Modelo de negócio (Canvas resumido)
- **Segmentos de clientes:** tutores de pets e equipe interna (atendente,
  veterinário, gerente);
- **Proposta de valor:** gestão integrada + conveniência digital + IA preditiva;
- **Canais:** aplicação web, aplicativo mobile, e-mail (confirmações);
- **Relacionamento:** autoatendimento no portal/app + atendimento presencial;
- **Fontes de receita:** mensalidade SaaS do sistema e/ou venda da licença de
  implantação, com possibilidade de módulos adicionais (IA e relatórios);
- **Recursos-chave:** infraestrutura em nuvem, equipe de desenvolvimento,
  base de dados de agendamentos e vendas;
- **Parcerias-chave:** fornecedores de infraestrutura em nuvem, provedores de
  e-mail transacional;
- **Estrutura de custos:** hospedagem, desenvolvimento e manutenção.

### 2.7.2 Análise de viabilidade
- **Viabilidade técnica:** as tecnologias adotadas (ASP.NET Core, MySQL, Python,
  React Native) são consolidadas no mercado, com ampla documentação e custo
  zero de licenciamento;
- **Viabilidade operacional:** o sistema automatiza processos já existentes,
  exigindo baixa curva de aprendizado das equipes;
- **Viabilidade financeira:** o custo principal é o desenvolvimento inicial; a
  operação em nuvem reduz investimentos em hardware próprio. O ganho com
  redução de faltas e otimização de estoque tende a se pagar em poucos meses
  para estabelecimentos de médio movimento.

---

> **Uso na monografia:** este conteúdo corresponde ao capítulo "Planejamento da
> Solução Tecnológica" do desenvolvimento do PIM IV. Adapte os nomes, dados da
> empresa e referências bibliográficas às normas ABNT antes da entrega final.
