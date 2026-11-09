"""Texto do relatório final do PIM IV. A formatação fica em gerar_relatorio.py.

Blocos aceitos: ("h1", título), ("h2", título), ("p", parágrafo), ("alineas", [itens]),
("fig", arquivo ou [arquivos], título, largura_cm), ("quadro", título, cabeçalho, linhas, larguras_cm[, tamanho])
e ("codigo", texto).
"""

TITULO = "PATAFÁCIL: SISTEMA INTEGRADO DE GESTÃO PARA PET SHOP E CLÍNICA VETERINÁRIA"
SUBTITULO = ("Desenvolvimento e implantação de uma solução tecnológica corporativa integrada "
             "para apoiar a transformação digital")

RESUMO = (
    "Este trabalho apresenta o desenvolvimento do PataFácil, uma solução tecnológica corporativa integrada "
    "para apoiar a transformação digital de um pet shop com clínica veterinária de pequeno porte, dando "
    "continuidade ao estudo realizado no PIM III. O objetivo foi construir e validar um sistema que unifica "
    "agenda de serviços, prontuário clínico, estoque, vendas e indicadores gerenciais, acessível por aplicação "
    "web e por aplicativo móvel. A pesquisa é aplicada, de abordagem qualitativa, apoiada em levantamento "
    "bibliográfico e conduzida de forma iterativa e incremental com o framework Scrum. A solução adota "
    "arquitetura em camadas e orientada a objetos: uma API REST em ASP.NET Core concentra as regras de negócio "
    "e é consumida tanto pela aplicação web, em HTML, CSS e JavaScript, quanto pelo aplicativo em React Native; "
    "os dados ficam em um banco MySQL com triggers, procedures, views e índices; e um microsserviço em Python "
    "fornece previsões de risco de cancelamento, de demanda e de segmentação de clientes. A autenticação "
    "utiliza tokens JWT, com senhas protegidas por BCrypt e autorização por perfil. O projeto contempla ainda "
    "o plano de infraestrutura em nuvem com containers e pipeline de integração e entrega contínuas, além de "
    "ações de acessibilidade, diversidade e responsabilidade social. No balcão, o sistema registra a chegada do "
    "cliente e o pagamento, emite automaticamente a ordem de serviço em A4, PDF ou cupom para impressora "
    "térmica e controla o recebimento de mercadorias com nota fiscal, validade, margem de lucro e cadastro de "
    "fornecedores. A validação reuniu 28 testes unitários automatizados, 53 verificações de ponta a ponta sobre "
    "a API e testes diretos dos objetos do banco, todos aprovados. Conclui-se que a solução atende aos objetivos "
    "propostos, elimina os conflitos de agenda, integra as vendas ao estoque e oferece ao gestor informações "
    "para decidir com base em dados."
)
PALAVRAS_CHAVE = "Transformação digital. Sistema integrado. API REST. Aplicativo móvel. Banco de dados. Metodologias ágeis."

ABSTRACT = (
    "This work presents the development of PataFácil, an integrated corporate technology solution designed to "
    "support the digital transformation of a small pet shop with a veterinary clinic, continuing the study "
    "carried out in PIM III. The objective was to build and validate a system that unifies service scheduling, "
    "clinical records, inventory, sales and management indicators, available through a web application and a "
    "mobile application. This is applied research with a qualitative approach, supported by a literature review "
    "and conducted iteratively and incrementally with the Scrum framework. The solution adopts a layered, "
    "object-oriented architecture: a REST API in ASP.NET Core concentrates the business rules and is consumed "
    "both by the web application, written in HTML, CSS and JavaScript, and by the React Native application; "
    "data is stored in a MySQL database with triggers, stored procedures, views and indexes; and a Python "
    "microservice provides predictions of cancellation risk, demand and customer segmentation. Authentication "
    "uses JWT tokens, with passwords protected by BCrypt and role-based authorization. The project also "
    "includes a cloud infrastructure plan with containers and a continuous integration and delivery pipeline, "
    "as well as accessibility, diversity and social responsibility actions. At the front desk, the system records "
    "the customer's arrival and payment, automatically issues the service order as an A4 page, a PDF file or a "
    "thermal printer receipt, and manages the receipt of goods with invoice, expiry date, profit margin and "
    "supplier registration. Validation comprised 28 automated unit tests, 53 end-to-end checks on the API and "
    "direct tests of the database objects, all of which passed. "
    "It is concluded that the solution meets the proposed objectives, eliminates scheduling conflicts, "
    "integrates sales with inventory and gives the manager information to make data-driven decisions."
)
KEYWORDS = "Digital transformation. Integrated system. REST API. Mobile application. Database. Agile methodologies."


# =====================================================================================
INTRODUCAO = [
    ("h1", "1 INTRODUÇÃO"),
    ("p", "A transformação digital tornou-se um dos principais fatores de competitividade das organizações. "
          "Empresas de todos os portes buscam integrar processos, automatizar atividades, ampliar a "
          "disponibilidade dos serviços e oferecer melhor experiência a seus clientes. Nos pequenos negócios "
          "esse movimento ainda encontra rotinas manuais e informações dispersas, que geram retrabalho, perda "
          "de receita e decisões tomadas sem dados."),
    ("p", "Este Projeto Integrado Multidisciplinar (PIM IV) dá continuidade ao PIM III, no qual foram "
          "levantados os requisitos e modelada a solução para a PataFácil, um pet shop com clínica veterinária "
          "de pequeno porte adotado como estudo de caso. A empresa presta serviços de banho e tosa, consultas "
          "e vacinação, além de vender produtos para animais de estimação, e até então controlava a agenda, "
          "os prontuários e o estoque em papel e planilhas."),
    ("p", "O objetivo geral deste trabalho é desenvolver e validar uma solução tecnológica corporativa "
          "integrada para a organização, contemplando o planejamento do negócio, as aplicações web e mobile, "
          "as APIs de integração, a arquitetura de software, o banco de dados, o plano de infraestrutura em "
          "nuvem com práticas de DevOps e o gerenciamento ágil do projeto, sem perder de vista a inclusão, a "
          "acessibilidade e a responsabilidade social."),
    ("p", "Quanto à metodologia, trata-se de uma pesquisa aplicada, de abordagem qualitativa e caráter "
          "exploratório, apoiada em pesquisa bibliográfica (GIL, 2017; MARCONI; LAKATOS, 2017; SEVERINO, "
          "2016) sobre engenharia de software, bancos de dados, computação em nuvem e métodos ágeis. A "
          "construção do sistema seguiu um ciclo iterativo e incremental orientado pelo Scrum (SCHWABER; "
          "SUTHERLAND, 2020), e cada incremento foi verificado por testes automatizados e por testes de "
          "integração executados sobre a aplicação em funcionamento."),
    ("p", "O código-fonte completo da solução, com as aplicações web e mobile, as APIs, o script do banco de "
          "dados, os testes automatizados e os arquivos de implantação, está disponível no repositório público "
          "do projeto: <<https://github.com/uelsam-jpg/PIM-IV-Patafacil>>."),
    ("p", "O trabalho está organizado conforme as etapas do manual do PIM IV. Os capítulos 2 a 4 tratam da "
          "organização, do planejamento da solução e da responsabilidade social. Os capítulos 5 a 8 descrevem "
          "a aplicação web, o aplicativo mobile, a arquitetura de software e o banco de dados. Os capítulos 9 "
          "e 10 apresentam a infraestrutura em nuvem com DevOps e o gerenciamento ágil. Por fim, o capítulo "
          "11 traz a conclusão, seguida das referências e dos apêndices."),
]


# =====================================================================================
def desenvolvimento(dados):
    ano = dados["ano"]
    return [
        # ------------------------------------------------------------------ Etapa 1
        ("h1", "2 CARACTERIZAÇÃO DA ORGANIZAÇÃO"),
        ("p", "A PataFácil é um estabelecimento de cuidados com animais de estimação que reúne, no mesmo "
              "endereço, um pet shop e uma clínica veterinária. Atende tutores de cães, gatos, aves e pequenos "
              "roedores em uma região urbana residencial e opera em uma única loja, de segunda a sábado, das "
              "8h às 19h. Por se tratar de um estudo de caso acadêmico, os dados aqui apresentados descrevem "
              "o cenário analisado no PIM III."),
        ("h2", "2.1 Segmento de atuação"),
        ("p", "A organização atua no setor de comércio e serviços para animais de estimação, combinando o "
              "varejo de produtos com a prestação de serviços estéticos e veterinários. Enquadra-se como "
              "microempresa e concorre com outros estabelecimentos de bairro e com redes especializadas, "
              "frente às quais a agilidade e a proximidade no atendimento são seus principais atributos."),
        ("h2", "2.2 Produtos e serviços"),
        ("p", "O Quadro 1 resume o catálogo cadastrado no sistema: 25 serviços, com a duração padrão usada para "
              "reservar a agenda do profissional, e 51 produtos vendidos no balcão, distribuídos em cinco "
              "categorias."),
        ("quadro", "Serviços e produtos oferecidos pela organização",
         ["Tipo", "Item", "Descrição"],
         [["Serviço", "Banho e tosa (12 itens)", "Banho por porte, tosa na máquina, na tesoura, higiênica e bebê, hidratação, banho medicamentoso e banho para gatos; de 40 a 90 minutos"],
          ["Serviço", "Consultas (4 itens)", "Consulta, retorno, emergência e check-up geriátrico; de 20 a 60 minutos"],
          ["Serviço", "Vacinas e procedimentos (9 itens)", "Vacinas V10, antirrábica, V4 felina e gripe canina, vermifugação, microchipagem, hemograma e limpeza de tártaro; de 15 a 90 minutos"],
          ["Produto", "Alimentos", "Rações secas e úmidas, petiscos e alimentos para aves e roedores"],
          ["Produto", "Medicamentos", "Antipulgas, vermífugos, suplementos e pomadas"],
          ["Produto", "Higiene", "Shampoos, tapetes higiênicos, areias sanitárias e itens de limpeza"],
          ["Produto", "Acessórios e outros", "Coleiras, guias, camas, comedouros, brinquedos, caixas de transporte e placas de identificação"]],
         [2.3, 4.7, 9.0]),
        ("h2", "2.3 Estrutura organizacional"),
        ("p", "A estrutura é funcional e enxuta, como mostra a Figura 1. O gerente responde pela administração, "
              "pelas metas e pela equipe; o atendente cuida do balcão, da agenda e das vendas; o veterinário "
              "realiza os atendimentos clínicos; e o apoio de estoque recebe e repõe as mercadorias. Esses "
              "papéis deram origem aos perfis de acesso do sistema."),
        ("fig", "dia_organograma.png", "Organograma da organização", 13.5),
        ("h2", "2.4 Principais processos"),
        ("p", "Os processos que sustentam a operação, e que orientaram o escopo da solução, são os seguintes:"),
        ("alineas", [
            "agendamento de serviços: o tutor solicita o serviço e a equipe encaixa o horário na agenda do profissional",
            "atendimento clínico: o veterinário atende o animal e registra a consulta no prontuário",
            "atendimento no balcão: o atendente recebe o cliente, acompanha o serviço e recebe o pagamento ao final",
            "venda no balcão: o atendente registra a venda de produtos e recebe o pagamento",
            "reposição de estoque: o responsável confere os saldos, solicita compras aos fornecedores e confere a nota fiscal e a validade no recebimento",
            "fechamento financeiro: o gerente consolida as receitas de serviços e de produtos"]),
        ("h2", "2.5 Problemas identificados"),
        ("p", "O levantamento apontou que a operação manual gerava falhas recorrentes. A agenda em papel "
              "permitia marcar dois serviços para o mesmo profissional no mesmo horário; as faltas dos "
              "clientes não eram previstas e deixavam horários ociosos; os produtos acabavam sem aviso, pois "
              "a venda não baixava o estoque; o histórico clínico ficava disperso em fichas; o tutor dependia "
              "do telefone para agendar; e o gerente não dispunha de indicadores confiáveis para decidir."),
        ("h2", "2.6 Oportunidades de melhoria por meio da tecnologia"),
        ("p", "Cada problema corresponde a uma oportunidade de melhoria: uma agenda que impeça conflitos "
              "automaticamente; canais digitais, web e mobile, que deem autonomia ao tutor; prontuário "
              "eletrônico com lembrete de vacinas; estoque integrado às vendas, com alerta de reposição; "
              "modelos de inteligência artificial para estimar a demanda e o risco de cancelamento; e "
              "relatórios gerenciais em tempo real. Essas oportunidades fundamentam o planejamento descrito "
              "no próximo capítulo."),

        # ------------------------------------------------------------------ Etapa 2
        ("h1", "3 PLANEJAMENTO DA SOLUÇÃO TECNOLÓGICA"),
        ("p", "Dornelas (2018) destaca que empreender é transformar uma oportunidade identificada em um "
              "negócio viável, o que exige definir com clareza o problema, o público e o valor entregue. Este "
              "capítulo apresenta esse planejamento para o PataFácil."),
        ("h2", "3.1 Problema a ser resolvido"),
        ("p", "O problema central é a ausência de uma solução integrada que automatize o fluxo operacional do "
              "estabelecimento, composto por agenda, clínica, estoque e vendas, e que ao mesmo tempo aproxime "
              "o cliente por canais digitais. A fragmentação das informações causa retrabalho, conflitos de "
              "horário, ruptura de estoque e decisões sem base em dados."),
        ("h2", "3.2 Objetivos da solução"),
        ("p", "O objetivo da solução é digitalizar e integrar os processos da organização em uma única "
              "plataforma. Especificamente, pretende-se:"),
        ("alineas", [
            "impedir conflitos de horário para o mesmo profissional (regra de negócio RN01)",
            "permitir que o tutor agende, acompanhe e cancele serviços on-line, respeitando a antecedência mínima de duas horas (RN02) e o horário de funcionamento (RN03)",
            "controlar o ciclo do agendamento em etapas travadas, de pendente a confirmado e a concluído (RN04), com a duração calculada pelo sistema (RN10)",
            "manter o histórico clínico de cada animal em prontuário eletrônico",
            "integrar vendas e estoque, com baixa automática e alerta no ponto de reposição (RN15)",
            "registrar o recebimento de mercadorias com nota fiscal, validade, margem de lucro e fornecedor",
            "emitir a ordem de serviço de cada atendimento, para impressão comum, arquivo PDF ou impressora térmica",
            "oferecer relatórios e indicadores ao gerente e previsões por aprendizado de máquina"]),
        ("h2", "3.3 Público-alvo"),
        ("p", "A solução atende dois públicos. O primeiro são os tutores, clientes do estabelecimento, em "
              "geral moradores da região, usuários de smartphone e que valorizam praticidade. O segundo é a "
              "equipe interna, formada por atendentes, veterinários e gerente, com níveis distintos de "
              "familiaridade com tecnologia, o que motivou interfaces simples e específicas para cada perfil."),
        ("h2", "3.4 Proposta de valor"),
        ("p", "A proposta de valor pode ser resumida em reunir todos os cuidados com o animal em um só lugar. "
              "Para o tutor, significa agendar a qualquer hora, escolher profissional e horário entre opções "
              "realmente disponíveis e consultar o histórico do animal. Para o estabelecimento, significa uma "
              "agenda sem conflitos, estoque sob controle e informação para decidir."),
        ("h2", "3.5 Benefícios esperados"),
        ("p", "O Quadro 2 relaciona os benefícios esperados ao recurso do sistema que os viabiliza."),
        ("quadro", "Benefícios esperados e recursos que os viabilizam",
         ["Benefício", "Como a solução entrega"],
         [["Fim dos conflitos de agenda", "Validação de sobreposição de horários por profissional (RN01)"],
          ["Redução de faltas", "Previsão do risco de cancelamento a cada novo agendamento"],
          ["Menos ruptura de estoque", "Baixa automática na venda e alerta no ponto de reposição (RN15)"],
          ["Melhor experiência do cliente", "Portal web e aplicativo para agendar, acompanhar e cancelar"],
          ["Decisão baseada em dados", "Indicadores do dia, relatório de faturamento e previsão de demanda"],
          ["Prontuário organizado", "Histórico clínico digital por animal, com data da próxima vacina"],
          ["Comprovante para o cliente", "Ordem de serviço gerada automaticamente, em A4, PDF ou cupom térmico"],
          ["Compras sob controle", "Entrada de mercadoria com nota, validade, margem sugerida e fornecedor cadastrado"],
          ["Segurança da informação", "Autenticação JWT, senhas com BCrypt e acesso por perfil"]],
         [5.5, 10.5]),
        ("h2", "3.6 Diferenciais competitivos"),
        ("p", "Em relação às soluções usuais para pequenos estabelecimentos, o PataFácil se diferencia por "
              "quatro aspectos: o uso de inteligência artificial para antecipar comportamentos, e não apenas "
              "registrar dados; a oferta dos canais web e mobile sobre as mesmas APIs, o que garante "
              "consistência; a aplicação das regras de negócio no servidor e no banco de dados, e não só na "
              "interface; e o baixo custo, por empregar apenas tecnologias de código aberto ou gratuitas."),
        ("h2", "3.7 Modelo de negócio e viabilidade"),
        ("p", "O modelo de negócio foi estruturado com o Business Model Canvas (OSTERWALDER; PIGNEUR, 2011) "
              "e está sintetizado no Quadro 3. A solução é oferecida como serviço, por assinatura mensal, o "
              "que dispensa o cliente de manter infraestrutura própria."),
        ("quadro", "Modelo de negócio do PataFácil (Business Model Canvas)",
         ["Bloco", "Definição"],
         [["Segmentos de clientes", "Pet shops e clínicas veterinárias de pequeno e médio porte e seus tutores"],
          ["Proposta de valor", "Gestão integrada, conveniência digital para o tutor e previsões por IA"],
          ["Canais", "Aplicação web, aplicativo mobile e notificações por e-mail"],
          ["Relacionamento", "Autoatendimento no portal e no aplicativo, com suporte ao estabelecimento"],
          ["Fontes de receita", "Assinatura mensal por estabelecimento e módulos adicionais"],
          ["Recursos principais", "Equipe de desenvolvimento, infraestrutura em nuvem e base de dados"],
          ["Atividades principais", "Desenvolvimento, operação da plataforma e suporte"],
          ["Parcerias principais", "Provedor de nuvem e serviço de e-mail transacional"],
          ["Estrutura de custos", "Hospedagem, manutenção evolutiva e atendimento"]],
         [4.5, 11.5]),
        ("p", "A análise de viabilidade é favorável nas três dimensões consideradas. Tecnicamente, as "
              "tecnologias adotadas são consolidadas e bem documentadas. Operacionalmente, o sistema "
              "automatiza rotinas que a equipe já executa, o que reduz a curva de aprendizado. "
              "Financeiramente, não há custo de licenciamento e a hospedagem em nuvem é paga conforme o uso, "
              "de modo que o investimento se concentra no desenvolvimento e tende a ser compensado pela "
              "redução de faltas e de perdas de estoque."),

        # ------------------------------------------------------------------ Etapa 3
        ("h1", "4 RESPONSABILIDADE SOCIAL E DIVERSIDADE"),
        ("p", "A tecnologia não é neutra: as escolhas de projeto podem ampliar ou reduzir desigualdades. "
              "Almeida (2019) mostra que o racismo opera de modo estrutural, reproduzido por práticas "
              "institucionais mesmo sem intenção individual, e Munanga e Gomes (2006) reforçam que superar "
              "essa condição exige ações concretas de valorização da diversidade. Com base nessa compreensão "
              "e na legislação brasileira, a solução incorporou as ações descritas a seguir."),
        ("h2", "4.1 Combate à discriminação"),
        ("p", "O sistema trata todos os usuários de forma igual: serviços, preços e horários são os mesmos "
              "para qualquer tutor, e o cadastro não coleta raça, cor, religião ou renda, em linha com o "
              "princípio da não discriminação do Estatuto da Igualdade Racial (BRASIL, 2010) e com a "
              "minimização de dados prevista na Lei Geral de Proteção de Dados (BRASIL, 2018). Os modelos de "
              "inteligência artificial utilizam apenas variáveis comportamentais, como antecedência e "
              "histórico de cancelamentos, o que evita que uma decisão automatizada reproduza preconceitos."),
        ("h2", "4.2 Valorização da diversidade"),
        ("p", "As mensagens e telas adotam linguagem respeitosa e neutra, e o acesso aos módulos depende "
              "apenas da função exercida. Propõe-se à organização que a identidade visual e os materiais de "
              "divulgação representem a diversidade étnico-racial da população brasileira e que o processo de "
              "contratação valorize essa diversidade, em sintonia com as diretrizes de educação das relações "
              "étnico-raciais instituídas a partir da Lei nº 10.639 (BRASIL, 2003)."),
        ("h2", "4.3 Recursos tecnológicos inclusivos"),
        ("p", "Todas as páginas da aplicação web incorporam o VLibras, conjunto de ferramentas gratuitas do "
              "governo federal que traduz conteúdos digitais para a Língua Brasileira de Sinais (BRASIL, "
              "2026). Os formulários de acesso possuem rótulos descritivos para leitores de tela, e o "
              "aplicativo utiliza componentes nativos compatíveis com os recursos de acessibilidade do "
              "Android e do iOS. O agendamento em passos curtos reduz a carga cognitiva e favorece pessoas "
              "idosas ou com pouca familiaridade com tecnologia."),
        ("h2", "4.4 Ações voltadas à acessibilidade"),
        ("p", "As ações de acessibilidade seguem a Lei Brasileira de Inclusão (BRASIL, 2015) e as diretrizes "
              "WCAG 2.1 (W3C, 2018), organizadas em quatro frentes: comunicação, com a tradução em Libras; "
              "percepção visual, com contraste entre texto e fundo e hierarquia tipográfica clara; navegação, "
              "com fluxos curtos e botões amplos; e conteúdo, com textos diretos e sem jargões. Como evolução, "
              "recomenda-se realizar testes com usuários com deficiência e ampliar os atributos de "
              "acessibilidade nas telas internas."),
        ("h2", "4.5 Utilização da tecnologia para promoção da cidadania"),
        ("p", "Ao permitir que o tutor acompanhe seus agendamentos e o histórico do animal, a solução amplia "
              "o acesso à informação e a transparência na relação de consumo. A oferta simultânea de web e "
              "aplicativo favorece a inclusão digital; o prontuário com lembrete de vacinas estimula a guarda "
              "responsável e o bem-estar animal; o controle de estoque reduz desperdícios; e o modelo de "
              "baixo custo torna ferramentas de gestão acessíveis a pequenos empreendedores."),

        # ------------------------------------------------------------------ Etapa 4
        ("h1", "5 DESENVOLVIMENTO DA SOLUÇÃO WEB"),
        ("p", "A aplicação web é composta por um servidor de APIs em ASP.NET Core (MICROSOFT, 2026a) e por "
              "páginas em HTML5, CSS3 e JavaScript que consomem essas APIs pelo navegador."),
        ("h2", "5.1 Arquitetura da aplicação"),
        ("p", "A solução segue a arquitetura em camadas, que separa a apresentação, as regras de negócio e o "
              "acesso a dados para que cada parte evolua sem afetar as demais (SOMMERVILLE, 2018). A Figura 2 "
              "apresenta as camadas e o sentido das chamadas: as interfaces enviam requisições HTTP aos "
              "controllers, que acionam os serviços de negócio, e estes usam os repositórios para ler e "
              "gravar no MySQL. O microsserviço de inteligência artificial é acionado pela camada de negócio."),
        ("fig", "dia_arquitetura.png", "Arquitetura em camadas da solução", 6.6),
        ("h2", "5.2 APIs REST"),
        ("p", "As APIs adotam o estilo arquitetural REST (FIELDING, 2000): os recursos são identificados por "
              "URLs, manipulados pelos verbos HTTP e representados em JSON. O Quadro 4 lista os principais "
              "endpoints e os perfis autorizados em cada um."),
        ("quadro", "Principais endpoints da API REST",
         ["Método e rota", "Finalidade", "Perfis"],
         [["POST /api/auth/login", "Autentica e devolve o token JWT", "Público"],
          ["POST /api/tutores", "Cadastra um tutor", "Público"],
          ["GET e POST /api/pets", "Lista e cadastra os animais do tutor", "Tutor e equipe"],
          ["GET /api/servicos", "Catálogo de serviços ativos", "Público"],
          ["GET /api/agendamento/horarios-disponiveis", "Horários livres por data, serviço e profissional", "Público"],
          ["POST /api/agendamento", "Cria o agendamento (RN01, RN03 e RN10)", "Tutor e atendente"],
          ["DELETE /api/agendamento/{id}", "Cancela o agendamento (RN02)", "Tutor, atendente e gerente"],
          ["PATCH /api/agendamento/{id}/confirmar e /concluir", "Avança o status (RN04)", "Equipe"],
          ["PATCH /api/agendamento/{id}/chegada e /pagamento", "Registra a chegada do cliente e a baixa do pagamento", "Atendente e gerente"],
          ["GET /api/agendamento/periodo e /em-atendimento", "Agenda do balcão e ordens de serviço por período", "Equipe"],
          ["GET /api/agendamento/tutor/{id}", "Agendamentos do tutor", "Tutor (os próprios) e balcão"],
          ["POST e GET /api/historicoclinico", "Registra e consulta o prontuário", "Veterinário; tutor (leitura)"],
          ["GET e POST /api/estoque/entradas", "Recebimento de mercadoria com nota, validade e preço", "Atendente e gerente"],
          ["GET e POST /api/estoque/fornecedores", "Lista e cadastra fornecedores", "Atendente e gerente"],
          ["GET /api/estoque/produtos e /critico", "Produtos, saldos, validades e estoque crítico", "Atendente e gerente"],
          ["POST /api/vendas", "Registra a venda com baixa de estoque (RN15)", "Atendente e gerente"],
          ["GET /api/relatorios/painel e /receita-mensal", "Indicadores, gráficos do mês e faturamento", "Equipe; gerente (financeiro)"],
          ["GET, PATCH e DELETE /api/emails", "Caixa de e-mail do usuário logado", "Todos os perfis"],
          ["GET /api/ml/previsao-demanda-hoje", "Previsão de demanda obtida da IA", "Equipe"],
          ["GET /health", "Verificação de saúde da API e do banco", "Público"]],
         [6.6, 5.6, 3.8], 9),
        ("h2", "5.3 Autenticação"),
        ("p", "A autenticação utiliza JSON Web Token, padrão que transporta declarações assinadas entre as "
              "partes (JONES; BRADLEY; SAKIMURA, 2015). O usuário informa e-mail e senha; a API confere a "
              "senha com o hash armazenado e emite um token assinado com o algoritmo HMAC-SHA256, válido por "
              "oito horas, que contém o identificador, o e-mail e o perfil do usuário. O cliente envia esse "
              "token no cabeçalho Authorization de cada requisição, e o servidor valida a assinatura e a "
              "validade sem manter sessão, o que favorece a escalabilidade."),
        ("h2", "5.4 Segurança"),
        ("p", "As medidas de segurança foram aplicadas em camadas complementares:"),
        ("alineas", [
            "senhas armazenadas apenas como hash BCrypt de fator 12, inclusive as da equipe inicial",
            "autorização por perfil em cada endpoint: o relatório financeiro, por exemplo, responde com o código 403 a qualquer usuário que não seja o gerente",
            "verificação de propriedade: o tutor só acessa os próprios animais, agendamentos e históricos, mesmo que altere os identificadores da requisição",
            "validação de entrada no servidor, com rejeição de e-mail ou CPF duplicados, e respostas que nunca expõem o hash da senha",
            "mensagem única para e-mail inexistente e senha incorreta, para não revelar quais contas existem",
            "escape dos textos recebidos da API antes da exibição nas páginas, prevenindo a injeção de código"]),
        ("h2", "5.5 Estrutura administrativa"),
        ("p", "Após o login, cada perfil é direcionado ao seu painel e o menu lateral mostra apenas os módulos "
              "permitidos. O tutor acessa o portal, com seus animais e agendamentos, o assistente de "
              "agendamento em passos e a caixa de e-mail. A interface segue um padrão visual único em todas as "
              "telas, com a fonte Inter, própria para leitura em tela, cores sólidas e poucos elementos "
              "decorativos; os ícones ilustrados ficam restritos aos avatares dos animais."),
        ("p", "A visão geral, mostrada na Figura 3, reúne os indicadores do dia, a demanda prevista pela "
              "inteligência artificial e o desempenho do mês comparado ao anterior: faturamento acumulado dia a "
              "dia, os cinco serviços e os cinco produtos mais vendidos, os clientes que mais gastaram, o "
              "movimento previsto para a semana e sugestões de promoção geradas a partir da validade dos lotes, "
              "do estoque parado e do movimento previsto. Cada gráfico tem uma tabela equivalente, para quem "
              "prefere ler os números."),
        ("fig", "web_dashboard.png", "Visão geral da equipe, com indicadores, gráficos do mês e previsão de demanda", 9.5),
        ("p", "O atendente trabalha na agenda do balcão, reproduzida no Apêndice B. A agenda é navegada por dia "
              "ou por semana e organizada em abas que acompanham o atendimento: agendados, em processo, "
              "finalizados e cancelados. Cada cartão mostra o animal, o cliente com telefone e atalho para o "
              "WhatsApp, o serviço, o profissional e um campo de observações da equipe. O botão \"Cliente "
              "chegou\" leva o atendimento para a aba em processo, e \"Finalizar e receber\" registra o valor, "
              "a forma de pagamento e o troco. O balcão também agenda serviços para clientes que chegam sem "
              "cadastro prévio."),
        ("p", "Para cada agendamento o sistema gera automaticamente uma ordem de serviço, numerada pelo próprio "
              "atendimento, que reúne os dados do cliente e do animal, o serviço, o profissional, as "
              "observações e o pagamento. A tela de ordens de serviço, também reproduzida no Apêndice B, lista as ordens do dia, da "
              "semana ou do mês, com busca e filtro por situação."),
        ("p", "Ao abrir uma ordem, escolhe-se o formato, como mostra a Figura 4: folha A4, que pode ser "
              "impressa ou baixada como arquivo PDF para envio ao cliente, ou cupom de 80 mm ou 58 mm para "
              "impressoras térmicas, cuja altura é calculada pelo conteúdo. Um atalho abre o WhatsApp do "
              "cliente com o resumo do atendimento."),
        ("fig", "web_os_a4.png", "Ordem de serviço no formato A4, pronta para impressão ou PDF", 9.5),
        ("p", "Na tela de estoque e vendas, a aba de entrada de mercadoria, apresentada no Apêndice B, registra o recebimento "
              "do fornecedor com número da nota, validade do lote, quantidade, custo e preço de venda. O "
              "sistema sugere o preço pela margem de lucro sobre a venda e mostra o lucro por unidade. O "
              "produto pode ser escolhido no catálogo ou cadastrado na própria entrada, e o fornecedor é "
              "escolhido entre os já cadastrados, com o último fornecedor do produto preenchido "
              "automaticamente, ou cadastrado na hora, com CNPJ, telefone e e-mail. Os alertas do topo indicam "
              "produtos no ponto de reposição e lotes que vencem em até 30 dias."),
        ("p", "O veterinário trabalha no consultório, onde vê a fila de pacientes e registra o prontuário, e o "
              "gerente dispõe ainda do relatório de faturamento e do cadastro da equipe. Todos os perfis têm "
              "uma caixa de e-mail com as mensagens que o sistema enviou, como confirmações e cancelamentos. "
              "As demais telas constam do Apêndice B."),
        ("h2", "5.6 Integração entre módulos"),
        ("p", "Os módulos trocam informações de forma automática. A Figura 5 detalha a criação de um "
              "agendamento: o serviço de negócio valida as regras, grava o registro, consulta o microsserviço "
              "de inteligência artificial para anotar o risco de cancelamento e dispara a notificação por "
              "e-mail, simulada no ambiente de desenvolvimento. A consulta à inteligência artificial tem "
              "tempo limite de três segundos e sua indisponibilidade não impede o agendamento."),
        ("fig", "dia_sequencia.png", "Sequência de integração na criação de um agendamento", 15.5),
        ("p", "De modo análogo, o registro do prontuário conclui o agendamento confirmado na mesma operação, "
              "e cada item de venda aciona, no banco de dados, a baixa do estoque e a geração do alerta de "
              "reposição. A entrada de mercadoria atualiza o saldo, o custo e o preço do produto, registra o "
              "lote com a validade e, se o fornecedor informado ainda não existir, inclui-o no cadastro."),
        ("p", "A validação dessa integração reuniu 28 testes unitários, escritos com xUnit e Moq, que cobrem as "
              "regras de agendamento (RN01, RN02, RN03, RN04 e RN10), a chegada e o pagamento no balcão, a posse "
              "dos dados pelo tutor, as vendas com baixa de estoque (RN15), a entrada de mercadoria com o "
              "cálculo do preço pela margem e o armazenamento de senhas com BCrypt. Os testes dispensam o banco "
              "de dados, pois os repositórios são simulados. Complementarmente, o teste final executou 53 "
              "verificações de ponta a ponta sobre a API em funcionamento, que cobriram as regras de negócio, "
              "as restrições de acesso de cada perfil, o fluxo do balcão, as vendas, a entrada de mercadoria e "
              "o cadastro de fornecedores, e abriu as 15 telas da aplicação web com o perfil correspondente, sem "
              "nenhum erro de execução no navegador; todos os testes foram aprovados."),

        # ------------------------------------------------------------------ Etapa 5
        ("h1", "6 DESENVOLVIMENTO DA SOLUÇÃO MOBILE"),
        ("p", "O aplicativo móvel destina-se ao tutor e foi desenvolvido em React Native (META PLATFORMS, "
              "2026) com a plataforma Expo (EXPO, 2026) e a linguagem TypeScript, o que permite gerar versões "
              "para Android e iOS a partir do mesmo código."),
        ("h2", "6.1 Telas principais"),
        ("p", "O aplicativo possui oito telas. A Figura 6 reúne as principais, capturadas com o "
              "aplicativo em execução e conectado à API. A tela inicial oferece atalhos para as funções mais "
              "usadas; a tela de animais lista os pets do tutor e dá acesso ao cadastro; a tela de consultas "
              "mostra os agendamentos com o respectivo status e a opção de cancelamento."),
        ("fig", ["mob_login.png", "mob_inicio.png", "mob_pets.png", "mob_consultas.png"],
         "Telas de login, início, pets e consultas do aplicativo", 3.5),
        ("p", "O agendamento é conduzido por um assistente de cinco passos: escolha do animal, do serviço e "
              "do profissional, seleção da data e de um dos horários livres informados pela API e, por fim, "
              "o resumo para confirmação, como mostra o Apêndice B."),
        ("h2", "6.2 Fluxo de navegação"),
        ("p", "A navegação combina uma pilha de telas para o acesso e abas inferiores para a área logada, "
              "conforme a Figura 7. Ao abrir o aplicativo, verifica-se se há sessão salva: em caso negativo, "
              "exibem-se as telas de login e de cadastro; em caso positivo, o usuário segue diretamente para "
              "o painel, cujas abas dão acesso a início, pets, agendar, consultas e perfil."),
        ("fig", "dia_navegacao.png", "Fluxo de navegação do aplicativo", 8.0),
        ("h2", "6.3 Autenticação"),
        ("p", "O aplicativo autentica-se no mesmo endpoint da aplicação web. O token JWT e os dados básicos "
              "do usuário são gravados no armazenamento local do aparelho, de modo que a sessão é restaurada "
              "nas aberturas seguintes. Ao sair, esses dados são removidos. Um contexto de autenticação "
              "disponibiliza o token a todas as telas, e o cliente HTTP o anexa às requisições protegidas."),
        ("h2", "6.4 Sincronização de dados"),
        ("p", "Os dados de negócio não são mantidos em cópia local: a fonte única é a API, o que evita "
              "divergências entre os canais. As listas são recarregadas sempre que a tela recebe o foco e "
              "quando o usuário puxa a lista para baixo; após criar ou cancelar um agendamento, a consulta é "
              "refeita. Assim, uma confirmação feita pelo atendente na web aparece no aplicativo na "
              "atualização seguinte. As requisições têm tempo limite de quinze segundos e mensagens de erro "
              "compreensíveis para falhas de conexão."),
        ("h2", "6.5 Integração com APIs"),
        ("p", "A integração foi isolada em uma camada de serviços, com um cliente HTTP comum que monta as "
              "requisições, injeta o token e traduz os erros retornados pelo servidor. O Quadro 5 relaciona "
              "as telas aos endpoints consumidos, os mesmos da aplicação web."),
        ("quadro", "Endpoints consumidos pelo aplicativo",
         ["Tela", "Endpoints"],
         [["Login e cadastro", "POST /api/auth/login; POST /api/tutores"],
          ["Pets e novo pet", "GET /api/pets/tutor/{id}; POST /api/pets"],
          ["Agendar", "GET /api/servicos; GET /api/funcionarios/profissionais; GET /api/agendamento/horarios-disponiveis; POST /api/agendamento"],
          ["Consultas", "GET /api/agendamento/tutor/{id}; DELETE /api/agendamento/{id}"]],
         [4.0, 12.0]),

        # ------------------------------------------------------------------ Etapa 6
        ("h1", "7 ARQUITETURA DE SOFTWARE"),
        ("p", "A arquitetura foi definida para atender simultaneamente à aplicação web e ao aplicativo, com "
              "um núcleo único de regras de negócio exposto por APIs."),
        ("h2", "7.1 Orientação a objetos"),
        ("p", "O domínio foi modelado com os pilares da orientação a objetos, representados no diagrama de "
              "classes da Figura 8. A classe abstrata Pessoa reúne os atributos comuns a tutores e "
              "funcionários (abstração), que são suas especializações (herança). O método GetPerfil é "
              "implementado de forma distinta em cada subclasse (polimorfismo), o que permite ao serviço de "
              "autenticação obter o perfil de qualquer usuário sem conhecer o seu tipo concreto."),
        ("fig", "dia_classes.png", "Diagrama de classes do domínio", 14.0),
        ("p", "O encapsulamento protege as regras mais sensíveis. Na classe Agendamento, o status só pode ser "
              "alterado pelos métodos da própria classe, que verificam a transição permitida, como mostra o "
              "trecho a seguir; desse modo, a regra RN04 vale para qualquer ponto do sistema que manipule um "
              "agendamento."),
        ("codigo", """public StatusAgendamento Status { get; private set; } = StatusAgendamento.PENDENTE;

public void Concluir()
{
    // RN04: o fluxo é travado (Pendente -> Confirmado -> Concluído)
    if (Status != StatusAgendamento.CONFIRMADO)
        throw new InvalidOperationException(
            "Apenas agendamentos confirmados podem ser concluídos.");
    Status = StatusAgendamento.CONCLUIDO;
}"""),
        ("h2", "7.2 Organização em camadas"),
        ("p", "O Quadro 6 descreve as camadas e sua localização no código. Uma requisição percorre sempre o "
              "mesmo caminho, do controller ao serviço e deste ao repositório, sem que uma camada acesse "
              "diretamente outra que não seja a imediatamente inferior. O acesso a dados emprega o padrão "
              "Repository (FOWLER, 2006), por meio de uma interface genérica cuja implementação utiliza o "
              "Entity Framework Core, e as dependências são fornecidas por injeção, o que permitiu testar os "
              "serviços com repositórios simulados."),
        ("quadro", "Camadas da solução e suas responsabilidades",
         ["Camada", "Local no projeto", "Responsabilidade"],
         [["Apresentação", "Frontend e Mobile", "Interfaces do usuário, clientes das APIs"],
          ["API", "PataFacil/Controllers", "Rotas REST, autenticação e autorização"],
          ["Negócio (BLL)", "PataFacil/BLL", "Regras de agendamento, venda, prontuário e relatórios"],
          ["Dados (DAL)", "PataFacil/DAL", "Repositório genérico, contexto e script do banco"],
          ["Modelos", "PataFacil/Models", "Entidades e enumerações do domínio"],
          ["Inteligência artificial", "MachineLearning", "Microsserviço de previsões em Python"]],
         [3.6, 4.4, 8.0]),
        ("h2", "7.3 Modularização"),
        ("p", "O sistema é modularizado por domínio de negócio: agendamento, clínica, estoque e vendas, "
              "relatórios e autenticação possuem controllers e serviços próprios, de modo que uma alteração "
              "em um módulo não exige mexer nos demais. O repositório genérico elimina a repetição do código "
              "de acesso a dados entre as treze entidades persistidas."),
        ("p", "A inteligência artificial constitui um módulo independente, implantado como microsserviço em "
              "Python com FastAPI e scikit-learn (PEDREGOSA et al., 2011) e acessado por HTTP. Ele oferece "
              "três modelos: classificação do risco de cancelamento com floresta aleatória, previsão da "
              "demanda diária com regressão linear e segmentação de clientes com K-Means. Como a organização "
              "ainda não possui histórico digital, os modelos foram treinados com dados sintéticos que "
              "reproduzem o comportamento esperado do negócio; nesse conjunto, o classificador obteve "
              "acurácia de 0,74 e a regressão, coeficiente de determinação de 0,83. Esses valores servem para "
              "validar a integração e deverão ser reavaliados com dados reais."),

        # ------------------------------------------------------------------ Etapa 7
        ("h1", "8 PROJETO DO BANCO DE DADOS"),
        ("p", "O banco de dados foi projetado no modelo relacional e implementado no MySQL 8 (ORACLE, 2026). "
              "O projeto seguiu as etapas clássicas de modelagem conceitual, lógica e física (ELMASRI; "
              "NAVATHE, 2018)."),
        ("h2", "8.1 Modelo entidade-relacionamento"),
        ("p", "A Figura 9 apresenta o modelo entidade-relacionamento (MER) completo, com as quinze tabelas do "
              "banco. Um tutor possui vários animais; cada agendamento associa "
              "um animal, um serviço e um profissional; o histórico clínico pertence a um animal e pode ter "
              "sido originado por um agendamento; cada produto tem um registro de estoque; e uma venda é "
              "composta por itens, cada qual referente a um produto."),
        ("p", "Nas últimas Sprints o modelo ganhou três tabelas: emails, que guarda as "
              "mensagens enviadas pelo sistema para a caixa de e-mail de cada usuário; entradas_estoque, em "
              "que cada recebimento de mercadoria forma um lote com nota fiscal, fornecedor, validade, custo e "
              "preço aplicado, ligado ao produto e ao funcionário que recebeu; e fornecedores, com nome único, "
              "CNPJ, telefone e e-mail. A entrada guarda o nome do fornecedor, o que preserva o histórico "
              "mesmo que o cadastro seja alterado depois; por isso a ligação aparece tracejada no MER."),
        ("fig", "dia_mer.png", "Modelo entidade-relacionamento do banco de dados", 12.5),
        ("h2", "8.2 Modelo lógico"),
        ("p", "No modelo lógico, cada entidade foi convertida em uma relação com chave primária numérica e os "
              "relacionamentos, em chaves estrangeiras, como resume o Quadro 7. As relações estão na terceira "
              "forma normal: não há grupos repetidos nem atributos dependentes de outros que não a chave. A "
              "única redundância intencional é o preço unitário gravado no item de venda, que preserva o "
              "valor praticado mesmo que o preço do produto mude depois."),
        ("quadro", "Modelo lógico: relações, atributos principais e chaves",
         ["Relação", "Atributos principais (PK = chave primária; FK = chave estrangeira)"],
         [["tutores", "Id (PK), Nome, Cpf, Email, Telefone, endereço, SenhaHash, Ativo"],
          ["pets", "Id (PK), Nome, Especie, Raca, Sexo, DataNascimento, PesoKg, TutorId (FK)"],
          ["funcionarios", "Id (PK), Nome, Cpf, Email, Cargo, SenhaHash, DataAdmissao, Ativo"],
          ["servicos", "Id (PK), Nome, Descricao, DuracaoMinutos, Preco, Ativo"],
          ["agendamentos", "Id (PK), DataHoraInicio, DataHoraFim, Status, Observacoes, ObservacoesAtendimento, DataChegada, ValorPago, FormaPagamento, DataPagamento, IdPet (FK), IdServico (FK), IdFuncionario (FK)"],
          ["historicos", "Id (PK), DataAtendimento, Tipo, Descricao, Diagnostico, Prescricao, ProximaVacina, IdPet (FK), IdFuncionario (FK), IdAgendamento (FK, opcional)"],
          ["produtos", "Id (PK), Nome, Categoria, Marca, PrecoCusto, PrecoVenda, Ativo"],
          ["estoques", "Id (PK), QuantidadeAtual, PontoReposicao, IdProduto (FK, único)"],
          ["vendas", "Id (PK), DataVenda, Total, FormaPagamento, IdTutor (FK, opcional), IdFuncionario (FK)"],
          ["itensvenda", "Id (PK), Quantidade, PrecoUnitario, Subtotal, IdVenda (FK), IdProduto (FK)"],
          ["log_agendamento_status", "Id (PK), IdAgendamento (FK), StatusAnterior, StatusNovo, DataMudanca"],
          ["alerta_reposicao", "Id (PK), IdProduto (FK), QuantidadeAtual, PontoReposicao, Resolvido"],
          ["emails", "Id (PK), Destinatario, Assunto, Corpo, DataEnvio, Lida"],
          ["entradas_estoque", "Id (PK), IdProduto (FK), Quantidade, NumeroNota, Fornecedor, Validade, CustoUnitario, PrecoVendaAplicado, DataEntrada, IdFuncionario (FK)"],
          ["fornecedores", "Id (PK), Nome (único), Cnpj, Telefone, Email, DataCadastro, Ativo"]],
         [4.0, 12.0], 9),
        ("h2", "8.3 Modelo físico"),
        ("p", "No modelo físico foram definidos os tipos de dados, as restrições e os índices. Os textos "
              "usam VARCHAR com tamanho adequado ao conteúdo, os valores monetários usam DECIMAL(10,2) e as "
              "enumerações do código são gravadas como inteiros. Além das chaves primárias e estrangeiras, o "
              "esquema declara unicidade para e-mail e CPF e restrições de verificação que impedem, por "
              "exemplo, status fora do domínio, término anterior ao início, saldo de estoque negativo e "
              "entrada de mercadoria com preço de venda abaixo do custo. "
              "Dez índices otimizam as consultas mais frequentes; o índice composto por profissional e "
              "horário de início atende à verificação de conflito de agenda, o que foi confirmado pelo plano "
              "de execução da consulta."),
        ("h2", "8.4 Procedures"),
        ("p", "As cinco stored procedures de negócio do Quadro 8 encapsulam operações que envolvem várias "
              "tabelas e permitem que outras ferramentas, como planilhas e sistemas de relatórios, utilizem o "
              "banco respeitando as mesmas regras da aplicação. Duas procedures auxiliares, sp_garantir_coluna "
              "e sp_recriar_check, atualizam bancos criados por versões anteriores, acrescentando colunas e "
              "restrições sem apagar dados."),
        ("quadro", "Stored procedures implementadas",
         ["Procedure", "Função", "Regras"],
         [["sp_agendar_servico", "Agenda um serviço validando data futura, expediente e conflito de horário; devolve o identificador e uma mensagem", "RN01, RN03, RN10"],
          ["sp_horarios_disponiveis", "Lista os horários livres de um profissional em uma data, em intervalos de 15 minutos", "RN01, RN03"],
          ["sp_registrar_venda", "Registra a venda e seus itens, recebidos em JSON, em uma única transação; desfaz tudo se faltar estoque", "RN15"],
          ["sp_receita_mensal", "Totaliza vendas de produtos, ticket médio e serviços concluídos do mês", "Gestão"],
          ["sp_estoque_critico", "Lista os produtos no ponto de reposição ou abaixo dele", "RN15"]],
         [4.3, 9.2, 2.5]),
        ("h2", "8.5 Triggers"),
        ("p", "As triggers do Quadro 9 garantem, no próprio banco, regras que não podem depender da "
              "aplicação que grava os dados. A baixa de estoque é feita exclusivamente pela trigger de itens "
              "de venda: a camada de negócio valida o saldo e calcula os alertas, mas não repete a baixa, o "
              "que evita o desconto em duplicidade."),
        ("quadro", "Triggers implementadas",
         ["Trigger", "Evento", "Ação"],
         [["trg_agendamento_calcula_fim", "Antes de inserir em agendamentos", "Calcula o término pela duração do serviço (RN10)"],
          ["trg_item_venda_baixa_estoque", "Após inserir em itensvenda", "Baixa o estoque e gera o alerta de reposição (RN15)"],
          ["trg_item_venda_atualiza_total", "Após inserir em itensvenda", "Recalcula o total da venda"],
          ["trg_agendamento_log_status", "Após atualizar agendamentos", "Registra a mudança de status para auditoria (RN04)"],
          ["trg_estoque_resolve_alerta", "Após atualizar estoques", "Encerra os alertas quando a reposição supera o ponto mínimo"]],
         [5.2, 4.6, 6.2]),
        ("h2", "8.6 Script completo e validação"),
        ("p", "O script completo do banco, reproduzido no Apêndice A, cria as quinze tabelas, as cinco "
              "triggers, as sete procedures, as três views e os índices. Ele é aplicado pela própria API ao "
              "iniciar, de modo que a aplicação web, o aplicativo e as APIs utilizam exatamente a estrutura "
              "documentada, e pode ser executado repetidas vezes sem erro. O Quadro 10 resume os testes "
              "realizados diretamente no MySQL 8.0."),
        ("quadro", "Resultados dos testes executados no banco de dados",
         ["Teste", "Resultado obtido"],
         [["Agendamento válido pela procedure", "Criado, com mensagem de sucesso"],
          ["Segundo agendamento sobreposto (RN01)", "Recusado: conflito de horário com outro atendimento"],
          ["Agendamento em domingo ou após as 19h (RN03)", "Recusado, com a mensagem da regra violada"],
          ["Inserção com término incorreto (RN10)", "Trigger corrigiu o término para início mais 30 minutos"],
          ["Mudança de status (RN04)", "Registro da transição de 0 para 1 gravado na auditoria"],
          ["Venda de 12 unidades com saldo 20 e ponto 10 (RN15)", "Saldo 8, total de R$ 180,00 calculado e um alerta aberto"],
          ["Venda acima do saldo", "Recusada, com o saldo inalterado"],
          ["Atualização que deixaria o saldo negativo", "Bloqueada pela restrição de verificação"],
          ["Reposição acima do ponto mínimo", "Alerta encerrado automaticamente"]],
         [7.5, 8.5]),

        # ------------------------------------------------------------------ Etapa 8
        ("h1", "9 INFRAESTRUTURA EM NUVEM E DEVOPS"),
        ("p", "Este capítulo apresenta o plano de execução da infraestrutura para implantar a solução. A "
              "computação em nuvem oferece recursos sob demanda, com elasticidade e pagamento pelo uso (MELL; "
              "GRANCE, 2011), características adequadas a um negócio de pequeno porte, que não dispõe de "
              "equipe para manter servidores. Os artefatos de containers e de automação já fazem parte do "
              "repositório do projeto; a contratação do provedor é a etapa seguinte à entrega."),
        ("h2", "9.1 Arquitetura em nuvem"),
        ("p", "A arquitetura de referência, na Figura 10, utiliza serviços gerenciados. O frontend é "
              "distribuído como conteúdo estático; a API e o microsserviço de inteligência artificial "
              "executam em containers; e o banco de dados é um serviço gerenciado, acessível somente pela "
              "rede interna. O aplicativo e o navegador comunicam-se com a API por HTTPS."),
        ("fig", "dia_nuvem.png", "Arquitetura de referência em nuvem", 12.0),
        ("h2", "9.2 Serviços utilizados"),
        ("p", "O Quadro 11 indica os serviços previstos, tomando a Microsoft Azure como referência pela "
              "integração com a plataforma .NET (MICROSOFT, 2026b). Como a solução é empacotada em "
              "containers, pode ser implantada de forma equivalente em outros provedores."),
        ("quadro", "Serviços de nuvem previstos",
         ["Componente", "Serviço", "Justificativa"],
         [["Frontend", "Azure Static Web Apps", "Hospedagem de arquivos estáticos com CDN e HTTPS"],
          ["API e IA", "Azure Container Apps", "Execução de containers com escala automática"],
          ["Banco de dados", "Azure Database for MySQL", "Serviço gerenciado com backup e atualização automáticos"],
          ["Imagens", "Container Registry", "Registro privado das imagens Docker"],
          ["Segredos", "Azure Key Vault", "Guarda da senha do banco e da chave JWT"],
          ["Observabilidade", "Azure Monitor e Application Insights", "Métricas, logs, rastreamento e alertas"]],
         [3.4, 5.4, 7.2]),
        ("h2", "9.3 Containers"),
        ("p", "Cada componente possui sua imagem Docker (DOCKER, 2026). A imagem da API é construída em dois "
              "estágios: o primeiro compila e publica o projeto e o segundo contém apenas o ambiente de "
              "execução, o que reduz o tamanho e a superfície de ataque. Um arquivo docker-compose sobe o "
              "banco de dados, o microsserviço de inteligência artificial, a API e o servidor web com um "
              "único comando, com verificação de saúde do banco e do microsserviço e com a senha e a chave "
              "JWT fornecidas por variáveis de ambiente. Assim, o ambiente de desenvolvimento reproduz o de "
              "produção."),
        ("h2", "9.4 Pipeline de CI/CD"),
        ("p", "A integração e a entrega contínuas, práticas centrais da cultura DevOps (KIM et al., 2018), "
              "foram automatizadas com o GitHub Actions, como ilustra a Figura 11. A cada envio de código, o "
              "pipeline compila a API, executa os 28 testes unitários e valida o microsserviço em Python. "
              "Somente se essas etapas forem aprovadas, e apenas na ramificação principal, as imagens Docker "
              "são construídas e publicadas no registro, identificadas pelo código da versão, ficando "
              "prontas para a implantação."),
        ("fig", "dia_pipeline.png", "Pipeline de integração e entrega contínuas", 15.8),
        ("h2", "9.5 Monitoramento"),
        ("p", "O monitoramento combina três recursos. A API e o microsserviço expõem endpoints de verificação "
              "de saúde, utilizados pela plataforma para reiniciar instâncias com falha; os registros de "
              "execução são centralizados para consulta; e a ferramenta de observabilidade coleta tempo de "
              "resposta, taxa de erros e uso de recursos. Propõem-se alertas para indisponibilidade do banco "
              "ou da inteligência artificial e para tempo de resposta acima de dois segundos."),
        ("h2", "9.6 Escalabilidade"),
        ("p", "A API não mantém estado entre requisições, pois a identidade do usuário está no token; por "
              "isso, pode ser replicada horizontalmente atrás de um balanceador de carga. O microsserviço de "
              "inteligência artificial pode reduzir-se a zero instâncias nos períodos sem uso, o que diminui "
              "o custo. O banco gerenciado admite ampliação de capacidade e réplicas de leitura para os "
              "relatórios, e o frontend é servido por rede de distribuição de conteúdo."),
        ("h2", "9.7 Segurança"),
        ("p", "Na infraestrutura, a segurança complementa as medidas da aplicação: todo o tráfego externo "
              "utiliza HTTPS; o banco de dados não é exposto à internet; os segredos ficam em cofre e são "
              "injetados como variáveis de ambiente, nunca gravados no código; as imagens contêm apenas o "
              "necessário à execução; e o banco gerenciado realiza cópias de segurança automáticas, com "
              "retenção configurável, para atender à continuidade do negócio."),

        # ------------------------------------------------------------------ Etapa 9
        ("h1", "10 GERENCIAMENTO ÁGIL DO PROJETO"),
        ("p", "O desenvolvimento foi planejado e acompanhado com o Scrum, framework em que o produto é "
              "construído em ciclos curtos, com inspeção e adaptação frequentes (SCHWABER; SUTHERLAND, 2020). "
              "A abordagem é indicada quando os requisitos evoluem ao longo do projeto e se deseja entregar "
              "valor desde cedo (PRESSMAN; MAXIM, 2016)."),
        ("h2", "10.1 Papéis e eventos"),
        ("p", "Foram definidos os três papéis do Scrum: o Product Owner, que representa os interesses da "
              "organização e ordena o backlog; o Scrum Master, que zela pelo método e remove impedimentos; e "
              "os desenvolvedores, responsáveis por backend, frontend, mobile, banco de dados e inteligência "
              "artificial. Cada Sprint, de duas semanas, compreendeu o planejamento, as reuniões diárias de "
              "quinze minutos, a revisão do incremento e a retrospectiva."),
        ("h2", "10.2 Product Backlog"),
        ("p", "O Product Backlog reúne as funcionalidades na forma de histórias de usuário, ordenadas pelo "
              "valor para o negócio. O Quadro 12 apresenta as 24 histórias, com a prioridade e a Sprint em "
              "que cada uma foi planejada. As histórias PB18 a PB24 surgiram nas revisões das Sprints, a partir "
              "do uso do sistema pela equipe do balcão, e foram atendidas na Sprint 7."),
        ("quadro", "Product Backlog",
         ["ID", "História de usuário", "Prior.", "Sprint"],
         [["PB01", "Como usuário, quero me autenticar com segurança", "1", "1"],
          ["PB02", "Como tutor, quero criar minha conta", "1", "1"],
          ["PB03", "Como tutor, quero cadastrar meus pets", "1", "2"],
          ["PB04", "Como tutor, quero agendar serviços em passos simples", "2", "2"],
          ["PB05", "Como estabelecimento, quero impedir conflito de horário (RN01)", "2", "2"],
          ["PB06", "Como estabelecimento, quero agenda no expediente e duração automática (RN03, RN10)", "2", "2"],
          ["PB07", "Como tutor, quero cancelar on-line com antecedência (RN02)", "2", "3"],
          ["PB08", "Como atendente, quero confirmar e concluir em etapas travadas (RN04)", "3", "3"],
          ["PB09", "Como veterinário, quero registrar o prontuário", "3", "3"],
          ["PB10", "Como atendente, quero vender produtos com carrinho", "4", "4"],
          ["PB11", "Como estabelecimento, quero baixa de estoque e alerta de reposição (RN15)", "4", "4"],
          ["PB12", "Como gerente, quero relatórios e indicadores", "5", "5"],
          ["PB13", "Como gerente, quero cadastrar a equipe e controlar acessos", "5", "5"],
          ["PB14", "Como gerente, quero prever demanda e risco de cancelamento", "6", "6"],
          ["PB15", "Como tutor, quero agendar pelo celular", "6", "6"],
          ["PB16", "Como pessoa surda, quero conteúdo em Libras", "7", "6"],
          ["PB17", "Como equipe técnica, quero containers e pipeline", "7", "6"],
          ["PB18", "Como atendente, quero registrar chegada e pagamento com troco", "3", "7"],
          ["PB19", "Como usuário, quero uma caixa de e-mail do sistema", "5", "7"],
          ["PB20", "Como atendente, quero registrar entrada com nota, validade e margem", "4", "7"],
          ["PB21", "Como atendente, quero escolher ou cadastrar o fornecedor na hora", "4", "7"],
          ["PB22", "Como atendente, quero emitir a ordem de serviço (A4, PDF ou térmica)", "3", "7"],
          ["PB23", "Como gerente, quero gráficos do mês e sugestões de promoção", "5", "7"],
          ["PB24", "Como usuário, quero uma interface padronizada e confortável", "6", "7"]],
         [1.4, 11.2, 1.6, 1.8], 9),
        ("h2", "10.3 Sprint Backlog e cronograma das Sprints"),
        ("p", "O trabalho foi distribuído em sete Sprints de duas semanas, de 3 de agosto a 6 de novembro de 2026. No planejamento de cada "
              "Sprint, as histórias selecionadas foram decompostas em tarefas técnicas, que formam o Sprint "
              "Backlog. O Quadro 13 traz o cronograma, com o objetivo e o Sprint Backlog resumido de cada "
              "ciclo."),
        ("quadro", "Cronograma das Sprints e Sprint Backlog resumido",
         ["Sprint", "Período", "Objetivo", "Sprint Backlog (tarefas principais)"],
         [["1", "03/08 a 14/08", "Fundação e autenticação", "Estrutura em camadas; repositório genérico; modelo de dados; login com JWT e BCrypt; cadastro de tutor"],
          ["2", "17/08 a 28/08", "Pets e agenda", "Cadastro de pets; cálculo de horários livres; regras RN01, RN03 e RN10; assistente de agendamento web"],
          ["3", "31/08 a 11/09", "Fluxo clínico", "Máquina de status (RN04); cancelamento (RN02); prontuário; painéis do atendente e do veterinário"],
          ["4", "14/09 a 25/09", "Estoque e vendas", "Produtos e estoque; venda com carrinho; triggers de baixa e de alerta (RN15)"],
          ["5", "28/09 a 09/10", "Gestão", "Indicadores; relatório de faturamento; gestão da equipe; procedures e views"],
          ["6", "12/10 a 23/10", "IA, mobile e DevOps", "Microsserviço de IA; aplicativo; VLibras; containers; pipeline; testes automatizados"],
          ["7", "26/10 a 06/11", "Operação do balcão", "Chegada e pagamento; caixa de e-mail; entrada de mercadoria; fornecedores; ordem de serviço; gráficos do mês; padronização visual; catálogo de demonstração"]],
         [1.3, 2.5, 3.4, 8.8], 9),
        ("h2", "10.4 Quadro Kanban"),
        ("p", "O andamento das tarefas foi acompanhado em um quadro Kanban, que torna o fluxo de trabalho "
              "visível e limita o trabalho em progresso (ANDERSON, 2011). Adotou-se o limite de três itens "
              "na coluna em andamento. O Quadro 14 reproduz a situação do quadro ao final da primeira semana "
              "da Sprint 4."),
        ("quadro", "Quadro Kanban na primeira semana da Sprint 4",
         ["A fazer", "Em andamento (limite 3)", "Em revisão", "Concluído"],
         [["PB12 Relatórios e indicadores\nPB13 Gestão da equipe\nPB14 Previsões por IA\nPB15 Aplicativo mobile\nPB16 Libras\nPB17 Containers e pipeline",
           "PB11 Trigger de baixa de estoque\nPB11 Alerta de reposição",
           "PB10 Venda com carrinho",
           "PB01 a PB09 (Sprints 1 a 3)\nPB10 Cadastro de produtos"]],
         [4.6, 4.2, 3.2, 4.0], 9),
        ("h2", "10.5 Definição das entregas"),
        ("p", "Cada Sprint resultou em um incremento utilizável, descrito no Quadro 15. Uma história só foi "
              "considerada pronta quando atendeu à definição de pronto acordada: código compilando e "
              "integrado; regras de negócio verificadas por teste; funcionamento de ponta a ponta na "
              "interface correspondente; e aceite do Product Owner na revisão da Sprint."),
        ("quadro", "Entregas por Sprint",
         ["Sprint", "Incremento entregue"],
         [["1", "API com autenticação por token e cadastro de tutor"],
          ["2", "Agendamento web sem conflitos de horário, dentro do expediente"],
          ["3", "Ciclo completo de atendimento, do agendamento ao prontuário"],
          ["4", "Vendas integradas ao estoque, com alerta de reposição"],
          ["5", "Painel gerencial com indicadores e relatório de faturamento"],
          ["6", "Solução integrada: web, aplicativo, inteligência artificial, containers e pipeline"],
          ["7", "Balcão completo, do recebimento do cliente à ordem de serviço, e compras com fornecedores cadastrados"]],
         [2.0, 14.0]),
    ]


# =====================================================================================
CONCLUSAO = [
    ("h1", "11 CONCLUSÃO"),
    ("p", "Este trabalho propôs desenvolver e validar uma solução tecnológica integrada para apoiar a "
          "transformação digital de um pet shop com clínica veterinária, em continuidade ao PIM III. O "
          "objetivo foi alcançado: o PataFácil reúne, em uma única plataforma, a agenda de serviços, o "
          "prontuário clínico, o estoque, as vendas e os indicadores gerenciais, com acesso pela web e por "
          "aplicativo móvel sobre as mesmas APIs."),
    ("p", "Os problemas identificados na caracterização da organização receberam resposta direta. O conflito "
          "de horários é impedido pela validação de sobreposição; o tutor ganhou autonomia para agendar e "
          "cancelar; o histórico clínico passou a ser digital; a venda baixa o estoque e alerta a reposição; "
          "o recebimento de mercadorias registra nota, validade e fornecedor; cada atendimento gera sua "
          "ordem de serviço para impressão, PDF ou cupom térmico; e o gerente dispõe de indicadores e de "
          "previsões. A arquitetura em camadas e orientada a objetos "
          "concentrou as regras no servidor e no banco de dados, o que garantiu o mesmo comportamento nos "
          "dois canais, e os testes automatizados e de integração, todos aprovados, sustentam a confiança "
          "no resultado."),
    ("p", "O projeto também evidenciou limites, que orientam os próximos passos. Os modelos de inteligência "
          "artificial foram treinados com dados sintéticos e precisam ser reavaliados quando houver "
          "histórico real; o envio de e-mails está simulado e deve ser ligado a um serviço transacional; e a "
          "infraestrutura em nuvem está planejada e automatizada, mas ainda não contratada. Recomenda-se, "
          "ainda, realizar testes de acessibilidade com usuários e incluir lembretes automáticos de vacinas "
          "e de consultas."),
    ("p", "Conclui-se que a integração dos conhecimentos de empreendedorismo, desenvolvimento web e mobile, "
          "arquitetura de software, banco de dados, computação em nuvem, DevOps e gerenciamento ágil permitiu "
          "entregar uma solução viável, segura e inclusiva, capaz de aumentar a eficiência e a "
          "competitividade de um pequeno negócio, o que reafirma o papel da tecnologia como instrumento de "
          "transformação das organizações."),
]


# =====================================================================================
REFERENCIAS = [
    "ALMEIDA, Silvio Luiz de. **Racismo estrutural**. São Paulo: Sueli Carneiro; Pólen, 2019.",
    "ANDERSON, David J. **Kanban**: mudança evolucionária de sucesso para seu negócio de tecnologia. [S. l.]: Blue Hole Press, 2011.",
    "BRASIL. **Lei nº 10.639, de 9 de janeiro de 2003**. Altera a Lei nº 9.394, de 20 de dezembro de 1996, para incluir no currículo oficial da rede de ensino a obrigatoriedade da temática “História e Cultura Afro-Brasileira”. Brasília, DF: Presidência da República, 2003. Disponível em: https://www.planalto.gov.br/ccivil_03/leis/2003/l10.639.htm. Acesso em: 10 out. 2026.",
    "BRASIL. **Lei nº 12.288, de 20 de julho de 2010**. Institui o Estatuto da Igualdade Racial. Brasília, DF: Presidência da República, 2010. Disponível em: https://www.planalto.gov.br/ccivil_03/_ato2007-2010/2010/lei/l12288.htm. Acesso em: 10 out. 2026.",
    "BRASIL. **Lei nº 13.146, de 6 de julho de 2015**. Institui a Lei Brasileira de Inclusão da Pessoa com Deficiência (Estatuto da Pessoa com Deficiência). Brasília, DF: Presidência da República, 2015. Disponível em: https://www.planalto.gov.br/ccivil_03/_ato2015-2018/2015/lei/l13146.htm. Acesso em: 10 out. 2026.",
    "BRASIL. **Lei nº 13.709, de 14 de agosto de 2018**. Lei Geral de Proteção de Dados Pessoais (LGPD). Brasília, DF: Presidência da República, 2018. Disponível em: https://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm. Acesso em: 10 out. 2026.",
    "BRASIL. Ministério da Gestão e da Inovação em Serviços Públicos. **VLibras**. Brasília, DF, 2026. Disponível em: https://www.gov.br/governodigital/pt-br/acessibilidade-e-usuario/vlibras. Acesso em: 10 out. 2026.",
    "DOCKER. **Docker documentation**. [S. l.], 2026. Disponível em: https://docs.docker.com. Acesso em: 10 out. 2026.",
    "DORNELAS, José. **Empreendedorismo**: transformando ideias em negócios. 7. ed. São Paulo: Empreende, 2018.",
    "ELMASRI, Ramez; NAVATHE, Shamkant B. **Sistemas de banco de dados**. 7. ed. São Paulo: Pearson Education do Brasil, 2018.",
    "EXPO. **Expo documentation**. [S. l.], 2026. Disponível em: https://docs.expo.dev. Acesso em: 10 out. 2026.",
    "FIELDING, Roy Thomas. **Architectural styles and the design of network-based software architectures**. 2000. Tese (Doutorado em Informação e Ciência da Computação) – University of California, Irvine, 2000. Disponível em: https://ics.uci.edu/~fielding/pubs/dissertation/top.htm. Acesso em: 10 out. 2026.",
    "FOWLER, Martin. **Padrões de arquitetura de aplicações corporativas**. Porto Alegre: Bookman, 2006.",
    "GIL, Antonio Carlos. **Como elaborar projetos de pesquisa**. 6. ed. São Paulo: Atlas, 2017.",
    "JONES, Michael; BRADLEY, John; SAKIMURA, Nat. **JSON Web Token (JWT)**: RFC 7519. [S. l.]: Internet Engineering Task Force, 2015. Disponível em: https://www.rfc-editor.org/rfc/rfc7519. Acesso em: 10 out. 2026.",
    "KIM, Gene et al. **Manual de DevOps**: como obter agilidade, confiabilidade e segurança em organizações tecnológicas. Rio de Janeiro: Alta Books, 2018.",
    "MARCONI, Marina de Andrade; LAKATOS, Eva Maria. **Fundamentos de metodologia científica**. 8. ed. São Paulo: Atlas, 2017.",
    "MELL, Peter; GRANCE, Timothy. **The NIST definition of cloud computing**. Gaithersburg: National Institute of Standards and Technology, 2011. (Special Publication 800-145). Disponível em: https://doi.org/10.6028/NIST.SP.800-145. Acesso em: 10 out. 2026.",
    "META PLATFORMS. **React Native documentation**. [S. l.], 2026. Disponível em: https://reactnative.dev/docs/getting-started. Acesso em: 10 out. 2026.",
    "MICROSOFT. **Documentação do ASP.NET Core**. [S. l.], 2026a. Disponível em: https://learn.microsoft.com/pt-br/aspnet/core. Acesso em: 10 out. 2026.",
    "MICROSOFT. **Documentação dos Aplicativos de Contêiner do Azure**. [S. l.], 2026b. Disponível em: https://learn.microsoft.com/pt-br/azure/container-apps. Acesso em: 10 out. 2026.",
    "MUNANGA, Kabengele; GOMES, Nilma Lino. **O negro no Brasil de hoje**. São Paulo: Global, 2006.",
    "ORACLE. **MySQL 8.0 reference manual**. [S. l.], 2026. Disponível em: https://dev.mysql.com/doc/refman/8.0/en. Acesso em: 10 out. 2026.",
    "OSTERWALDER, Alexander; PIGNEUR, Yves. **Business model generation**: inovação em modelos de negócios. Rio de Janeiro: Alta Books, 2011.",
    "PEDREGOSA, Fabian et al. Scikit-learn: machine learning in Python. **Journal of Machine Learning Research**, [s. l.], v. 12, p. 2825-2830, 2011.",
    "PRESSMAN, Roger S.; MAXIM, Bruce R. **Engenharia de software**: uma abordagem profissional. 8. ed. Porto Alegre: AMGH, 2016.",
    "SCHWABER, Ken; SUTHERLAND, Jeff. **O guia do Scrum**: o guia definitivo para o Scrum: as regras do jogo. [S. l.], 2020. Disponível em: https://scrumguides.org/docs/scrumguide/v2020/2020-Scrum-Guide-PortugueseBR-3.0.pdf. Acesso em: 10 out. 2026.",
    "SEVERINO, Antônio Joaquim. **Metodologia do trabalho científico**. 24. ed. São Paulo: Cortez, 2016.",
    "SOMMERVILLE, Ian. **Engenharia de software**. 10. ed. São Paulo: Pearson Education do Brasil, 2018.",
    "W3C. **Web Content Accessibility Guidelines (WCAG) 2.1**. [S. l.]: World Wide Web Consortium, 2018. Disponível em: https://www.w3.org/TR/WCAG21. Acesso em: 10 out. 2026.",
]


# =====================================================================================
APENDICE_TELAS = [
    ("p", "As figuras a seguir complementam os capítulos 5 e 6 e foram capturadas com a aplicação em execução."),
    ("fig", "web_atendente.png", "Agenda do balcão com um atendimento em processo", 12.5),
    ("fig", "web_ordens.png", "Lista de ordens de serviço da semana", 12.5),
    ("fig", "web_entrada.png", "Entrada de mercadoria com alertas de reposição e de validade", 12.5),
    ("fig", ["mob_agendar_1.png", "mob_agendar_2.png", "mob_agendar_4.png", "mob_agendar_5.png"],
         "Passos do assistente de agendamento no aplicativo", 3.5),
    ("fig", "web_login.png", "Tela de login e cadastro", 12.5),
    ("fig", "web_portal.png", "Portal do tutor", 12.5),
    ("fig", "web_agendamento.png", "Assistente de agendamento da aplicação web", 12.5),
    ("fig", "web_veterinario.png", "Consultório veterinário e prontuário eletrônico", 12.5),
    ("fig", "web_estoque.png", "Venda no balcão com carrinho e alertas de estoque", 12.5),
    ("fig", "web_gerente.png", "Painel do gerente com o relatório de faturamento", 12.5),
    ("fig", "web_emails.png", "Caixa de e-mail do usuário", 12.5),
    ("fig", "web_os_termica.png", "Ordem de serviço no formato de cupom para impressora térmica de 80 mm", 12.5),
]
