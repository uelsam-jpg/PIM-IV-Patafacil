<!-- ============================================================
     MONOGRAFIA PIM IV — FORMATO ABNT
     ------------------------------------------------------------
     INSTRUÇÕES DE FORMATAÇÃO (NBR 14724 / ABNT):
     • Papel A4, margens: superior/esquerda 3,0 cm; inferior/direita 2,0 cm
     • Fonte Arial ou Times New Roman, tamanho 12 (citações longas: 10)
     • Espaçamento entrelinhas 1,5; parágrafos com recuo de 1,5 cm
     • Texto justificado; numeração no canto superior direito a partir
       da Introdução
     • Títulos em negrito/caixa alta, conforme hierarquia
     • Substitua os textos entre [COLCHETES] pelos seus dados
     • Expanda cada capítulo usando os arquivos da pasta Documentacao/
     ============================================================ -->

---

# **UNIVERSIDADE PAULISTA — UNIP**

## Projeto Integrado Multidisciplinar

### Curso Superior de Tecnologia em Análise e Desenvolvimento de Sistemas

<br>

**[NOME DO ALUNO — RA]**

<br>

# **SISTEMA INTEGRADO DE GESTÃO PARA PET SHOP E CLÍNICA VETERINÁRIA — PATAFÁCIL**

## **Desenvolvimento e Implantação de uma Solução Tecnológica Corporativa Integrada para a Transformação Digital**

<br>

**[CIDADE]**

**[ANO]**

---

# **FOLHA DE ROSTO**

**[NOME DO ALUNO — RA]**

# **SISTEMA INTEGRADO DE GESTÃO PARA PET SHOP E CLÍNICA VETERINÁRIA — PATAFÁCIL**

**Subtítulo (se houver)**

Projeto Integrado Multidisciplinar em Análise e Desenvolvimento de Sistemas

Projeto Integrado Multidisciplinar para obtenção do título de tecnólogo em Análise e Desenvolvimento de Sistemas, apresentado à Universidade Paulista — UNIP.

**Orientador(a): [NOME DO ORIENTADOR]**

<br>

**[CIDADE]**

**[ANO]**

---

# **RESUMO**

O presente trabalho aborda o desenvolvimento do **PataFácil**, um sistema
corporativo integrado para pet shop e clínica veterinária, cujo objetivo é
apoiar a transformação digital de uma pequena organização do ramo de cuidados
com animais de estimação. A solução foi planejada a partir da caracterização da
empresa e da identificação de seus principais problemas operacionais, como a
gestão manual da agenda, o controle deficiente de estoque e a ausência de
indicadores gerenciais. O sistema foi desenvolvido em arquitetura em camadas,
com backend em ASP.NET Core, banco de dados MySQL, aplicação web em HTML/CSS/JS,
aplicativo mobile em React Native e um microsserviço de inteligência artificial
em Python, responsável por prever risco de cancelamento, demanda diária e
segmentar clientes. O projeto contempla ainda o gerenciamento ágil do
desenvolvimento com Scrum, a definição de infraestrutura em nuvem e práticas de
DevOps, além de ações de responsabilidade social e acessibilidade. Os
resultados demonstram que a solução integrada automatiza os processos do
estabelecimento, reduz conflitos de agenda, previne faltas e apoia a tomada de
decisão com base em dados. Conclui-se que a tecnologia empregada atende aos
objetivos propostos e contribui para a modernização e competitividade da
organização.

**Palavras-chave:** Sistema integrado. Pet shop. Veterinária. Transformação
digital. Arquitetura de software. Inteligência artificial.

---

# **ABSTRACT**

This work addresses the development of **PataFácil**, an integrated corporate
system for a pet shop and veterinary clinic, whose purpose is to support the
digital transformation of a small organization in the pet care sector. The
solution was planned based on the characterization of the company and the
identification of its main operational problems, such as manual scheduling
management, deficient inventory control and the absence of management
indicators. The system was developed in a layered architecture, with a backend
in ASP.NET Core, MySQL database, web application in HTML/CSS/JS, mobile
application in React Native and a machine learning microservice in Python,
responsible for predicting cancellation risk, daily demand and segmenting
customers. The project also includes agile management with Scrum, cloud
infrastructure definition and DevOps practices, as well as social
responsibility and accessibility actions. The results show that the integrated
solution automates the establishment's processes, reduces schedule conflicts,
prevents no-shows and supports data-driven decision making. It is concluded
that the technology employed meets the proposed objectives and contributes to
the modernization and competitiveness of the organization.

**Keywords:** Integrated system. Pet shop. Veterinary. Digital transformation.
Software architecture. Artificial intelligence.

---

# **SUMÁRIO**

1. **INTRODUÇÃO** ................................................................................. X
2. **CARACTERIZAÇÃO DA ORGANIZAÇÃO** .................................................. X
3. **PLANEJAMENTO DA SOLUÇÃO TECNOLÓGICA** ........................................ X
4. **RESPONSABILIDADE SOCIAL E DIVERSIDADE** ....................................... X
5. **DESENVOLVIMENTO DA SOLUÇÃO WEB** ................................................. X
6. **DESENVOLVIMENTO DA SOLUÇÃO MOBILE** ............................................ X
7. **ARQUITETURA DE SOFTWARE** .............................................................. X
8. **PROJETO DO BANCO DE DADOS** ........................................................... X
9. **INFRAESTRUTURA EM NUVEM E DEVOPS** .............................................. X
10. **GERENCIAMENTO ÁGIL DO PROJETO** .................................................. X
11. **CONCLUSÃO** ...................................................................................... X
**REFERÊNCIAS** .............................................................................................. X

---

# **1 INTRODUÇÃO**

## 1.1 Contextualização

A transformação digital tornou-se um dos principais fatores de competitividade
das organizações. Empresas de diferentes segmentos buscam soluções capazes de
integrar processos, automatizar atividades, ampliar a disponibilidade dos
serviços, garantir a segurança das informações e oferecer melhor experiência
aos usuários. Nesse contexto, o setor de cuidados com animais de estimação, em
crescimento no Brasil, ainda depende, em grande parte, de processos manuais e
fragmentados, o que gera retrabalho, perda de receita e insatisfação dos
clientes.

## 1.2 Objetivo geral

Desenvolver e implantar o sistema integrado **PataFácil** para digitalizar e
automatizar os processos de um pet shop e clínica veterinária, unificando agenda
de serviços, histórico clínico, controle de estoque, vendas e relatórios
gerenciais, com acesso por aplicação web e aplicativo mobile, apoiado por
inteligência artificial para previsão de demandas e comportamento de clientes.

## 1.3 Objetivos específicos

- Automatizar o agendamento de serviços, garantindo que não existam conflitos
  de horário para o mesmo profissional (RN01);
- Permitir que o tutor agende, acompanhe e cancele serviços online, com regras
  de antecedência e expediente (RN02 e RN03);
- Controlar o fluxo do agendamento de forma travada (RN04);
- Manter o histórico clínico do pet organizado e acessível;
- Integrar vendas e estoque com baixa automática e alertas de reposição (RN15);
- Gerar relatórios financeiros e KPIs em tempo real;
- Oferecer aplicativo mobile integrado às mesmas APIs;
- Utilizar aprendizado de máquina para previsão de cancelamento, demanda e
  segmentação de clientes.

## 1.4 Metodologia

O trabalho foi desenvolvido com base em pesquisa bibliográfica sobre metodologia
científica (Severino, Lakatos e Marconi, Gil), engenharia de software,
arquitetura em camadas, bancos de dados e metodologias ágeis. A solução foi
construída de forma iterativa e incremental, utilizando o framework Scrum, com
validação das funcionalidades a cada Sprint. As tecnologias adotadas — ASP.NET
Core, MySQL, JavaScript, React Native e Python — foram selecionadas por sua
consolidação no mercado e custo zero de licenciamento.

## 1.5 Estrutura do trabalho

Este trabalho está organizado em onze capítulos: a introdução; a caracterização
da organização; o planejamento da solução tecnológica; a responsabilidade social
e diversidade; o desenvolvimento das soluções web e mobile; a arquitetura de
software; o projeto do banco de dados; a infraestrutura em nuvem e DevOps; o
gerenciamento ágil do projeto; e, por fim, a conclusão e as referências.

---

# **2 CARACTERIZAÇÃO DA ORGANIZAÇÃO** *(Etapa 1)*

## 2.1 Apresentação da empresa

A PataFácil é um estabelecimento do ramo de cuidados com animais de estimação,
que atua como pet shop e clínica veterinária de pequeno/médio porte, oferecendo
serviços de estética e bem-estar (banho, tosa) e serviços clínicos (consultas,
vacinas e exames) para cães, gatos, aves e outros animais.

## 2.2 Segmento de atuação

A organização atua no segmento de comércio e serviços voltados a animais de
estimação (*pet care*), atendendo tutores de cães, gatos, aves e outros animais
de companhia, em estrutura de micro/pequena empresa.

## 2.3 Produtos e serviços

Os serviços oferecidos incluem banho e tosa (60 min), consulta veterinária
(30 min) e vacina V10 (15 min). No varejo, a empresa comercializa rações,
medicamentos, produtos de higiene e acessórios.

## 2.4 Estrutura organizacional

A estrutura é funcional e enxuta, composta por gerente, atendente, veterinário
e apoio de estoque/compras, com responsabilidades bem definidas.

## 2.5 Principais processos

Os principais processos são: agendamento de serviços, atendimento clínico,
venda no balcão, reposição de estoque e fechamento financeiro.

## 2.6 Problemas identificados

Foram identificados problemas como agenda manual com risco de choque de
horários, faltas não previstas, controle de estoque deficiente, prontuário em
papel, atendimento limitado e gestão sem dados.

## 2.7 Oportunidades de melhoria

As oportunidades de melhoria incluem automação da agenda, canal digital para o
cliente, histórico clínico digital, estoque integrado às vendas, inteligência
artificial e gestão orientada a dados.

> *Texto completo: `Documentacao/Etapa01_Caracterizacao_da_Organizacao.md`*

---

# **3 PLANEJAMENTO DA SOLUÇÃO TECNOLÓGICA** *(Etapa 2)*

## 3.1 Problema a ser resolvido

O problema central é a ausência de uma solução tecnológica integrada que
automatize o fluxo operacional do estabelecimento e aproxime o cliente por meio
de canais digitais, eliminando retrabalho, conflitos de agenda e falta de dados
para decisão.

## 3.2 Objetivos da solução

Desenvolver um sistema integrado que automatize agenda, clínica, estoque e
vendas, com relatórios gerenciais, aplicação web e mobile, apoiados por
inteligência artificial.

## 3.3 Público-alvo

O público-alvo é composto pelos tutores de animais de estimação (clientes) e
pela equipe do estabelecimento (atendente, veterinário e gerente).

## 3.4 Proposta de valor

"Todos os cuidados com o seu pet em um só lugar", com agendamento e histórico
acessíveis e processos internos automatizados, reduzindo faltas e otimizando o
estoque.

## 3.5 Benefícios esperados

Redução de conflitos de agenda, menos no-show, menos ruptura de estoque, melhor
experiência do cliente, decisão baseada em dados, prontuário organizado e
segurança da informação.

## 3.6 Diferenciais competitivos

Integração de inteligência artificial, omnicanalidade (web e mobile com as
mesmas APIs), regras de negócio fortes no backend, arquitetura em camadas, baixo
custo de implantação e acessibilidade integrada.

## 3.7 Modelo de negócio e viabilidade

O modelo segue o padrão SaaS para o estabelecimento, com receita por assinatura
e possibilidade de módulos adicionais. A viabilidade técnica, operacional e
financeira foi considerada positiva, dado o baixo custo das tecnologias e os
ganhos operacionais gerados.

> *Texto completo: `Documentacao/Etapa02_Planejamento_Solucao_Tecnologica.md`*

---

# **4 RESPONSABILIDADE SOCIAL E DIVERSIDADE** *(Etapa 3)*

## 4.1 Combate à discriminação

O sistema foi projetado para ser um ambiente livre de preconceitos, com
igualdade de acesso aos serviços, linguagem inclusiva e dados sem vieses — o
módulo de IA utiliza apenas dados comportamentais, jamais atributos pessoais.

## 4.2 Valorização da diversidade

O sistema contempla diferentes perfis de usuários, representatividade visual,
aceitação de diversas espécies e raças de animais e gestão de equipe sem
critérios discriminatórios.

## 4.3 Recursos tecnológicos inclusivos

Inclui o plugin VLibras para tradução em Língua Brasileira de Sinais (Libras),
componentes de acessibilidade no mobile (leitores de tela), cores com contraste
adequado, formulários com rótulos claros e interface simplificada.

## 4.4 Ações voltadas à acessibilidade

Adota acessibilidade de comunicação, visual, navegação e conteúdo, com melhoria
contínua segundo as diretrizes WCAG.

## 4.5 Tecnologia para promoção da cidadania

Promove transparência com o cliente, inclusão digital, consumo consciente,
bem-estar animal e democratização da gestão para pequenos estabelecimentos.

> *Texto completo: `Documentacao/Etapa03_Responsabilidade_Social_Diversidade.md`*

---

# **5 DESENVOLVIMENTO DA SOLUÇÃO WEB** *(Etapa 4)*

## 5.1 Arquitetura da aplicação

A solução web utiliza arquitetura em camadas (Apresentação, API, BLL, DAL e
Models), com injeção de dependência e padrão Repository. O frontend em
HTML/CSS/JS consome as APIs REST do backend ASP.NET Core.

## 5.2 APIs REST

Foram desenvolvidos endpoints REST para autenticação, tutores, pets, serviços,
funcionários, agendamentos, histórico clínico, estoque, vendas, relatórios e
previsão de demanda por inteligência artificial.

## 5.3 Autenticação

A autenticação é baseada em Token JWT com validade de 8 horas e perfis de acesso
(Tutor, Atendente, Veterinário, Gerente), controlados por autorização por papel.

## 5.4 Segurança

Senhas criptografadas com BCrypt, autorização por papel, expiração de token,
proteção de dados sensíveis, validação de entrada, CORS e tratamento de erros.

## 5.5 Estrutura administrativa

A aplicação conta com painéis específicos: portal do tutor, wizard de
agendamento, dashboard de indicadores, painel do veterinário, balcão do
atendente, controle de estoque e painel do gerente.

## 5.6 Integração entre módulos

Os módulos integram frontend, API, banco, microsserviço de IA e notificações
por e-mail, com baixa automática de estoque e fluxo de agendamento validado
pelas regras de negócio.

> *Texto completo: `Documentacao/Etapa04_Desenvolvimento_Solucao_Web.md`*

---

# **6 DESENVOLVIMENTO DA SOLUÇÃO MOBILE** *(Etapa 5)*

## 6.1 Telas principais

O aplicativo mobile, desenvolvido em React Native com Expo, contempla as telas
de login, cadastro, início, pets, cadastro de pet, agendamento (wizard),
minhas consultas e perfil.

## 6.2 Fluxo de navegação

A navegação utiliza pilha e abas inferiores: o usuário deslogado acessa login e
cadastro; após autenticação, acessa o painel do tutor com as abas Início, Pets,
Agendar, Consultas e Perfil.

## 6.3 Autenticação

O aplicativo autentica no mesmo endpoint da API (`/api/auth/login`), armazenando
o token JWT no dispositivo (AsyncStorage) para manter a sessão do usuário.

## 6.4 Sincronização de dados

As listas de pets e agendamentos são sincronizadas com a API ao abrir as telas e
por atualização manual (pull-to-refresh), garantindo dados atualizados.

## 6.5 Integração com APIs

O mobile consome as mesmas APIs REST do sistema web, assegurando consistência
de regras de negócio e dados entre os canais.

> *Implementação: pasta `Mobile/`*

---

# **7 ARQUITETURA DE SOFTWARE** *(Etapa 6)*

## 7.1 Orientação a objetos

O sistema aplica abstração (classe `Pessoa`), herança (`Tutor` e `Funcionario`
herdam de `Pessoa`), polimorfismo (`GetPerfil()`) e encapsulamento (status do
agendamento protegido por métodos).

## 7.2 Organização em camadas

As camadas de apresentação, API, negócios (BLL), dados (DAL) e modelos garantem
separação de responsabilidades e baixo acoplamento.

## 7.3 Modularização

O sistema é modularizado por domínio de negócio, com microsserviço de
inteligência artificial isolado e reuso de código via Repository genérico.

> *Texto completo: `Documentacao/Etapa06_Arquitetura_de_Software.md`*

---

# **8 PROJETO DO BANCO DE DADOS** *(Etapa 7)*

## 8.1 Modelo lógico e físico

O banco MySQL é composto por 10 tabelas principais, com modelo lógico e físico
documentados e diagrama MER, além de tabelas de apoio de auditoria e alertas.

## 8.2 Procedures

Foram criadas procedures para agendamento com validação de conflito (RN01) e
expediente (RN03), horários disponíveis, registro de venda, receita mensal e
estoque crítico.

## 8.3 Triggers

Triggers calculam automaticamente o fim do agendamento (RN10), baixam o estoque
e geram alertas de reposição (RN15), atualizam o total da venda e registram a
auditoria de status (RN04).

## 8.4 Script completo

O script completo do banco, com procedures, triggers, views e índices, está
disponível em `PataFacil/DAL/ScriptBancoDeDados.sql` e foi validado no MySQL 8.

> *Texto completo: `Documentacao/Etapa07_Projeto_Banco_de_Dados.md`*

---

# **9 INFRAESTRUTURA EM NUVEM E DEVOPS** *(Etapa 8)*

## 9.1 Arquitetura em nuvem e serviços

A solução prevê implantação em nuvem com frontend estático, API em containers,
microsserviço de IA e banco de dados gerenciado (referência: Microsoft Azure),
com CDN e balanceamento de carga.

## 9.2 Containers e pipeline CI/CD

As aplicações são empacotadas em Docker (docker-compose) e o pipeline GitHub
Actions automatiza o build, a validação e a publicação das imagens.

## 9.3 Monitoramento, escalabilidade e segurança

São previstos monitoramento com métricas e alertas, escalabilidade horizontal
das APIs e segurança com segredos em cofre, JWT, BCrypt e HTTPS.

> *Texto completo: `Documentacao/Etapa08_Infraestrutura_Nuvem_DevOps.md`*

---

# **10 GERENCIAMENTO ÁGIL DO PROJETO** *(Etapa 9)*

## 10.1 Metodologia e papéis

O projeto foi conduzido com Scrum, definindo Product Owner, Scrum Master e Time
de Desenvolvimento, com cerimônias de planejamento, daily, review e
retrospectiva.

## 10.2 Product Backlog e Sprints

O Product Backlog foi priorizado em 17 histórias de usuário, organizadas em 6
Sprints de 2 semanas: fundação e autenticação; pets e agenda; fluxo clínico;
estoque e vendas; gestão e relatórios; e IA, mobile e DevOps.

## 10.3 Quadro Kanban e definição de pronto

O progresso foi acompanhado em quadro Kanban e cada entrega seguiu a definição
de pronto (código compilando, regras validadas, integração funcionando e
documentação atualizada).

> *Texto completo: `Documentacao/Etapa09_Gerenciamento_Agil_Projeto.md`*

---

# **11 CONCLUSÃO**

O presente trabalho apresentou o desenvolvimento do sistema integrado PataFácil,
uma solução tecnológica corporativa para apoiar a transformação digital de um
pet shop e clínica veterinária. A partir da caracterização da organização e da
identificação de seus principais problemas, foi possível planejar e implementar
uma solução completa, composta por aplicação web, aplicativo mobile, banco de
dados otimizado, relatórios gerenciais e um microsserviço de inteligência
artificial.

Os resultados obtidos demonstram que a solução atende aos objetivos propostos: a
automação da agenda elimina conflitos de horário; as regras de negócio
garantem o fluxo correto dos agendamentos; a integração entre vendas e estoque
evita rupturas; e os relatórios e a inteligência artificial apoiam a tomada de
decisão gerencial. Além disso, a arquitetura em camadas e orientada a objetos
proporcionou modularidade e facilidade de manutenção, enquanto as práticas de
DevOps e o gerenciamento ágil garantiram qualidade e entregas incrementais.

Conclui-se que a tecnologia empregada é viável e contribui significativamente
para a modernização, a eficiência operacional e a competitividade da
organização, reafirmando a importância da transformação digital no setor de
cuidados com animais de estimação.

---

# **REFERÊNCIAS**

GIL, Antônio Carlos. **Como elaborar projetos de pesquisa**. 6. ed. São Paulo:
Atlas, 2017.

LAKATOS, Eva Maria; MARCONI, Marina de Andrade. **Fundamentos de metodologia
científica**. 8. ed. São Paulo: Atlas, 2017.

MICROSOFT. **Documentação do ASP.NET Core**. Disponível em:
https://learn.microsoft.com/aspnet/core. Acesso em: 30 ago. 2026.

MICROSOFT. **Documentação do Entity Framework Core**. Disponível em:
https://learn.microsoft.com/ef/core. Acesso em: 30 ago. 2026.

REACT NATIVE. **Documentação oficial**. Disponível em:
https://reactnative.dev. Acesso em: 30 ago. 2026.

SCIKIT-LEARN. **Documentação oficial**. Disponível em:
https://scikit-learn.org. Acesso em: 30 ago. 2026.

SEVERINO, Antônio Joaquim. **Metodologia do trabalho científico**. 24. ed.
São Paulo: Cortez, 2016.

UNIVERSIDADE PAULISTA. **Guia de normalização para apresentação de trabalhos
acadêmicos da Universidade Paulista**. São Paulo: UNIP, [ano].

W3C. **Web Content Accessibility Guidelines (WCAG)**. Disponível em:
https://www.w3.org/WAI/standards-guidelines/wcag/. Acesso em: 30 ago. 2026.
