"""Gera o roteiro de apresentação do PIM IV (material de estudo da dupla para a banca).

Uso (a partir da raiz do projeto):
    python Documentacao/gerar_roteiro.py

As referências de linha ("MOSTRAR NA TELA") apontam para o código atual: ao mudar os arquivos
citados, confira as linhas e gere o PDF de novo.
"""
from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle
from reportlab.lib.units import cm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import (KeepTogether, PageBreak, Paragraph, SimpleDocTemplate, Spacer, Table,
                                TableStyle)

RAIZ = Path(__file__).resolve().parent.parent
SAIDA = RAIZ / "Roteiro_Apresentacao_PataFacil.pdf"

# ---------------------------------------------------------------- fontes e cores
FONTES = Path(r"C:\Windows\Fonts")
pdfmetrics.registerFont(TTFont("Arial", str(FONTES / "arial.ttf")))
pdfmetrics.registerFont(TTFont("Arial-Bold", str(FONTES / "arialbd.ttf")))
pdfmetrics.registerFont(TTFont("Arial-Italic", str(FONTES / "ariali.ttf")))
pdfmetrics.registerFont(TTFont("Arial-BoldItalic", str(FONTES / "arialbi.ttf")))
pdfmetrics.registerFont(TTFont("Consolas", str(FONTES / "consola.ttf")))
pdfmetrics.registerFontFamily("Arial", normal="Arial", bold="Arial-Bold", italic="Arial-Italic", boldItalic="Arial-BoldItalic")

TINTA = colors.HexColor("#1f2937")
CINZA = colors.HexColor("#4b5563")
CLARO = colors.HexColor("#6b7280")
LINHA = colors.HexColor("#e5e7eb")
FUNDO_CAIXA = colors.HexColor("#f3f4f6")
ALUNO = {
    1: (colors.HexColor("#1d4ed8"), colors.HexColor("#eef4ff"), colors.HexColor("#2563eb")),
    2: (colors.HexColor("#047857"), colors.HexColor("#ecfdf5"), colors.HexColor("#059669")),
}

def estilo(nome, **kw):
    base = dict(fontName="Arial", fontSize=10, leading=14, textColor=TINTA)
    base.update(kw)
    return ParagraphStyle(nome, **base)

E = {
    "capa_titulo": estilo("capa_titulo", fontName="Arial-Bold", fontSize=30, leading=36, alignment=TA_CENTER),
    "capa_sub": estilo("capa_sub", fontSize=13, leading=20, textColor=CINZA, alignment=TA_CENTER),
    "capa_rodape": estilo("capa_rodape", fontSize=12, leading=16, textColor=CINZA, alignment=TA_CENTER),
    "h1": estilo("h1", fontName="Arial-Bold", fontSize=20, leading=26, spaceAfter=10),
    "h2": estilo("h2", fontName="Arial-Bold", fontSize=13.5, leading=18, spaceAfter=2),
    "rotulo": estilo("rotulo", fontName="Arial-Bold", fontSize=7.5, leading=10, textColor=CLARO),
    "fala": estilo("fala", fontName="Arial-Italic", fontSize=10, leading=15),
    "texto": estilo("texto", fontSize=10, leading=14.5),
    "caixa": estilo("caixa", fontSize=9, leading=13, textColor=CINZA),
    "tabela": estilo("tabela", fontSize=8.8, leading=12),
    "tabela_cab": estilo("tabela_cab", fontName="Arial-Bold", fontSize=8.8, leading=12, textColor=CINZA),
    "item": estilo("item", fontSize=9.5, leading=13.5, leftIndent=12, bulletIndent=2),
}

LARGURA = A4[0] - 4 * cm


def rodape(canvas, doc):
    if doc.page == 1:
        return
    canvas.saveState()
    canvas.setFont("Arial", 7.5)
    canvas.setFillColor(CLARO)
    canvas.drawString(2 * cm, 1.2 * cm, "PataFácil · PIM IV · Roteiro de apresentação")
    canvas.drawRightString(A4[0] - 2 * cm, 1.2 * cm, f"Página {doc.page}")
    canvas.restoreState()


# ---------------------------------------------------------------- blocos
def caixa_mostrar(texto):
    conteudo = [Paragraph("MOSTRAR NA TELA", E["rotulo"]), Spacer(1, 3)]
    conteudo += [Paragraph(t, E["caixa"]) for t in texto.split("\n")]
    t = Table([[conteudo]], colWidths=[LARGURA - 0.9 * cm])
    t.setStyle(TableStyle([("BACKGROUND", (0, 0), (-1, -1), FUNDO_CAIXA),
                           ("LEFTPADDING", (0, 0), (-1, -1), 9), ("RIGHTPADDING", (0, 0), (-1, -1), 9),
                           ("TOPPADDING", (0, 0), (-1, -1), 7), ("BOTTOMPADDING", (0, 0), (-1, -1), 8)]))
    return t


def topico(aluno, titulo, fala, mostrar):
    """Um tópico da fala: barra lateral na cor do aluno, o que falar e o que mostrar."""
    cor = ALUNO[aluno][2]
    itens = [Paragraph(titulo, E["h2"]), Paragraph("O QUE FALAR", E["rotulo"]), Spacer(1, 1),
             Paragraph(f"“{fala}”", E["fala"]), Spacer(1, 6), caixa_mostrar(mostrar)]
    t = Table([[itens]], colWidths=[LARGURA - 0.3 * cm])
    t.setStyle(TableStyle([("LINEBEFORE", (0, 0), (0, -1), 2.6, cor),
                           ("LEFTPADDING", (0, 0), (-1, -1), 10), ("RIGHTPADDING", (0, 0), (-1, -1), 0),
                           ("TOPPADDING", (0, 0), (-1, -1), 2), ("BOTTOMPADDING", (0, 0), (-1, -1), 4)]))
    return [KeepTogether(t), Spacer(1, 10)]


def faixa_aluno(aluno, titulo, tempo):
    escuro, fundo, cor = ALUNO[aluno]
    rotulo = Paragraph(f"ALUNO {aluno}", estilo("fa", fontName="Arial-Bold", fontSize=8, textColor=colors.white))
    tit = Paragraph(titulo, estilo("ft", fontName="Arial-Bold", fontSize=12, leading=15))
    tmp = Paragraph(tempo, estilo("fm", fontName="Arial-Bold", fontSize=8.5, textColor=escuro))
    t = Table([[rotulo, tit, tmp]], colWidths=[2.2 * cm, LARGURA - 4.6 * cm, 2.4 * cm])
    t.setStyle(TableStyle([("BACKGROUND", (0, 0), (0, 0), cor), ("BACKGROUND", (1, 0), (-1, 0), fundo),
                           ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
                           ("TOPPADDING", (0, 0), (-1, -1), 9), ("BOTTOMPADDING", (0, 0), (-1, -1), 9),
                           ("LEFTPADDING", (0, 0), (-1, -1), 8)]))
    return [t, Spacer(1, 10)]


def tabela(cabecalho, linhas, larguras, cor_linha=None):
    dados = [[Paragraph(c, E["tabela_cab"]) for c in cabecalho]]
    dados += [[Paragraph(str(c), E["tabela"]) for c in linha] for linha in linhas]
    t = Table(dados, colWidths=larguras, repeatRows=1)
    estilo_t = [("BACKGROUND", (0, 0), (-1, 0), FUNDO_CAIXA), ("VALIGN", (0, 0), (-1, -1), "TOP"),
                ("LINEBELOW", (0, 0), (-1, -1), 0.5, LINHA),
                ("TOPPADDING", (0, 0), (-1, -1), 5), ("BOTTOMPADDING", (0, 0), (-1, -1), 6)]
    if cor_linha:
        for i, linha in enumerate(linhas, start=1):
            estilo_t.append(("BACKGROUND", (0, i), (-1, i), cor_linha(linha)))
    t.setStyle(TableStyle(estilo_t))
    return t


def lista(itens):
    return [Paragraph(i, E["item"], bulletText="•") for i in itens]


# ---------------------------------------------------------------- conteúdo
def capa():
    capa_alunos = Table([
        [Paragraph("ALUNO 1", estilo("c1", fontName="Arial-Bold", fontSize=8, textColor=ALUNO[1][2])),
         Paragraph("Abertura, arquitetura, Orientação a Objetos, segurança, banco de dados e estoque", E["tabela"])],
        [Paragraph("ALUNO 2", estilo("c2", fontName="Arial-Bold", fontSize=8, textColor=ALUNO[2][2])),
         Paragraph("Regras de negócio, agendamento, balcão e ordem de serviço, Inteligência Artificial, testes e encerramento", E["tabela"])],
    ], colWidths=[3 * cm, 10.5 * cm])
    capa_alunos.setStyle(TableStyle([("LINEBELOW", (0, 0), (-1, -1), 0.5, LINHA), ("VALIGN", (0, 0), (-1, -1), "MIDDLE"),
                                     ("TOPPADDING", (0, 0), (-1, -1), 8), ("BOTTOMPADDING", (0, 0), (-1, -1), 8)]))
    return [Spacer(1, 6.5 * cm), Paragraph("PataFácil", E["capa_titulo"]), Spacer(1, 8),
            Paragraph("Roteiro de apresentação do PIM IV", E["capa_sub"]),
            Paragraph("Análise e Desenvolvimento de Sistemas", E["capa_sub"]), Spacer(1, 2.2 * cm),
            capa_alunos, Spacer(1, 2 * cm),
            Paragraph("Banca: 2 professores · Duração prevista: cerca de 20 minutos + perguntas", E["capa_rodape"]),
            PageBreak()]


def divisao():
    corpo = [Paragraph("1. Como a apresentação está dividida", E["h1"]),
             Paragraph("Cada aluno apresenta uma metade do sistema e conduz a demonstração ao vivo da sua parte. Os "
                       "tempos são uma sugestão para cerca de 20 minutos; ajustem ao tempo que a banca der. Troquem "
                       "Aluno 1 e Aluno 2 pelos nomes de vocês.", E["texto"]), Spacer(1, 8)]
    linhas = [
        ["1", "Aluno 1", "Abertura: problema, objetivo e perfis de usuário", "2 min"],
        ["2", "Aluno 1", "Arquitetura e camadas (Controller, BLL, DAL, Models)", "3 min"],
        ["3", "Aluno 1", "Orientação a Objetos no código", "2 min"],
        ["4", "Aluno 1", "Segurança (JWT, perfis, BCrypt) + demonstração de login e erro 403", "2 min"],
        ["5", "Aluno 1", "Banco de dados e estoque (triggers, entrada de mercadoria, fornecedores) + demonstração", "3 min"],
        ["6", "Aluno 2", "Regras de negócio e o fluxo de agendamento", "2 min"],
        ["7", "Aluno 2", "Demonstração ao vivo: tutor agenda, balcão recebe o cliente, dá baixa no pagamento e emite a ordem de serviço", "3 min"],
        ["8", "Aluno 2", "Inteligência Artificial (3 modelos)", "2 min"],
        ["9", "Aluno 2", "Testes, DevOps, mobile e acessibilidade", "1 min"],
        ["10", "Aluno 2", "Limitações, melhorias futuras e encerramento", "1 min"],
    ]
    corpo.append(tabela(["Ordem", "Quem", "Assunto", "Tempo"], linhas, [1.4 * cm, 1.9 * cm, LARGURA - 5.1 * cm, 1.8 * cm],
                        lambda l: ALUNO[1][1] if l[1] == "Aluno 1" else ALUNO[2][1]))
    corpo += [Spacer(1, 14), Paragraph("Antes de começar (checklist)", E["h2"]), Spacer(1, 4)]
    corpo += lista([
        "MySQL, API de IA (porta 8000) e API em C# (porta 5288) rodando. Teste http://localhost:5288/health e http://localhost:8000/health.",
        "Navegador aberto em Frontend/index.html (aperte Ctrl+F5 para carregar a versão mais recente) e uma segunda aba no Swagger (http://localhost:5288/swagger).",
        "O sistema já cria uma cliente de demonstração com pets e agendamentos: cliente@patafacil.com (Maria Souza, pets Thor e Mel), e um catálogo com 25 serviços, 51 produtos e 6 fornecedores. Usem esses dados para não perder tempo.",
        "VS Code aberto com os arquivos que serão mostrados: Program.cs, AgendamentoService.cs, Agendamento.cs, Pessoa.cs, AuthController.cs, EstoqueService.cs, ScriptBancoDeDados.sql e ml_api.py.",
        "Usuários de teste, todos com senha 123456: gerente@patafacil.com, atendente@patafacil.com (Ana, abre direto na agenda), vet@patafacil.com, carla.tosa@patafacil.com (Banho e Tosa) e cliente@patafacil.com.",
        "Combinar quem opera o computador: quem está falando é quem mexe na tela.",
    ])
    corpo += [Spacer(1, 12), Paragraph("Dicas", E["h2"]), Spacer(1, 4)]
    corpo += lista([
        "Ensaiem pelo menos duas vezes juntos, cronometrando.",
        "Se algo der erro na demonstração, mantenham a calma, expliquem o que deveria acontecer e mostrem o código da regra.",
        "Falem olhando para a banca, não para a tela.",
        "Não decorem palavra por palavra: entendam a ideia de cada bloco e falem com as suas palavras.",
    ])
    return corpo + [PageBreak()]


def aluno1():
    c = [Paragraph("2. Roteiro do Aluno 1", E["h1"])]
    c += faixa_aluno(1, "Abertura, arquitetura, OO, segurança, banco e estoque", "≈ 12 min")
    c += topico(1, "2.1 Abertura",
        "Bom dia, professores. Nosso projeto é o <b>PataFácil</b>, um sistema de gestão para pet shop e clínica "
        "veterinária. O problema que resolvemos é a desorganização da operação: horários marcados em duplicidade, "
        "clientes que faltam sem avisar, estoque controlado no papel e nenhum comprovante do atendimento. O sistema "
        "cuida de <b>agendamentos, balcão de atendimento, ordem de serviço, prontuário clínico, estoque, compras e "
        "vendas, relatórios</b> e tem uma parte de <b>Inteligência Artificial</b> que prevê faltas, demanda e o perfil "
        "dos clientes. Há quatro perfis: o <b>Tutor</b>, que é o dono do pet, e a equipe, formada por <b>Atendente, "
        "Veterinário e Gerente</b>, além dos profissionais de banho e tosa. Eu vou apresentar a estrutura técnica do "
        "sistema e o meu colega apresenta as regras de negócio, o balcão e a IA.",
        "Tela de login do sistema (Frontend/index.html).")
    c += topico(1, "2.2 Arquitetura",
        "O sistema tem quatro partes. O <b>frontend web</b> em HTML, CSS e JavaScript, e um <b>aplicativo mobile</b> em "
        "React Native com Expo, para o tutor. As duas interfaces conversam com a mesma <b>API em C# ASP.NET Core</b>, "
        "que é a única que acessa o <b>banco MySQL</b>. A API também chama um <b>microsserviço de IA em Python</b> com "
        "FastAPI. Ou seja: nenhuma tela acessa o banco diretamente. Tudo passa pela API, que aplica as regras e a "
        "segurança.",
        "Desenhar ou mostrar o esquema:\n<font name='Consolas'>Frontend / Mobile -&gt; API C# -&gt; MySQL</font>\n"
        "<font name='Consolas'>API C# -&gt; API de IA (Python)</font>")
    c += topico(1, "2.3 Camadas do backend",
        "Dentro da API usamos <b>arquitetura em camadas</b>. Os <b>Controllers</b> recebem as requisições HTTP e "
        "verificam quem pode chamar cada rota. A <b>BLL</b>, a camada de negócio, tem as regras, por exemplo o "
        "AgendamentoService e o EstoqueService. A <b>DAL</b> faz o acesso ao banco com o <b>padrão Repository</b> e o "
        "Entity Framework. E os <b>Models</b> são as classes que representam as tabelas. Cada camada só conhece a de "
        "baixo, então dá para mudar uma sem quebrar as outras. Tudo é ligado por <b>injeção de dependência</b>, "
        "configurada no Program.cs.",
        "VS Code: as pastas Controllers, BLL, DAL e Models; depois <b>Program.cs</b>, linhas 31 a 40 (registro dos "
        "serviços e do Repositorio genérico).")
    c += topico(1, "2.4 Orientação a Objetos",
        "Aplicamos os quatro pilares. <b>Abstração e herança</b>: a classe Pessoa é abstrata, guarda nome, CPF e "
        "e-mail, e Tutor e Funcionario herdam dela. <b>Polimorfismo</b>: Pessoa declara o método GetPerfil e cada "
        "filha responde de um jeito; no login tratamos tutor e funcionário como Pessoa. <b>Encapsulamento</b>: na "
        "classe Agendamento o Status tem private set. Ninguém altera o status diretamente; é preciso chamar "
        "Confirmar, Cancelar, Concluir, RegistrarChegada ou RegistrarPagamento, e cada método valida se a mudança é "
        "permitida. Por fim, o Repositorio genérico funciona para qualquer entidade, sem repetir código.",
        "Models/Pessoa.cs (classe abstrata) e Models/Agendamento.cs, linha 29 (private set) e métodos Confirmar, "
        "Cancelar, Concluir, RegistrarChegada (linha 66) e RegistrarPagamento (linha 82).")
    c += topico(1, "2.5 Segurança + demonstração",
        "O acesso é feito com <b>token JWT</b>. Ao fazer login, a API gera um token assinado que vale 8 horas e "
        "carrega o id e o perfil do usuário. Cada rota diz quais perfis podem usá-la; um tutor que tenta acessar "
        "uma tela de gerente recebe <b>erro 403</b>. O tutor também só enxerga os próprios pets e agendamentos. As "
        "senhas são guardadas com hash <b>BCrypt</b>, nunca em texto puro. E a mensagem de erro do login é a mesma "
        "para e-mail inexistente e senha errada, para não revelar quem tem cadastro.",
        "1) Login como gerente@patafacil.com e mostrar a Visão Geral. 2) No Swagger, chamar uma rota gerencial sem "
        "token ou com token de tutor e mostrar o 401/403. 3) Mostrar <b>AuthController.cs</b>, linhas 56 e 74 "
        "(mensagem única e expiração de 8 horas).")
    c += topico(1, "2.6 Banco de dados e estoque + demonstração",
        "O banco é MySQL. O script ScriptBancoDeDados.sql é aplicado sozinho quando a API sobe e cria <b>15 tabelas</b>, "
        "além de triggers, procedures, views e índices; pode ser executado de novo sem apagar dados. Um exemplo: a "
        "trigger <b>trg_item_venda_baixa_estoque</b> diminui o estoque a cada item vendido, que é a <b>RN15</b>. "
        "Colocamos isso no banco para a baixa acontecer mesmo se alguém inserir uma venda por fora da API. Quando o "
        "estoque chega no ponto de reposição, aparece um alerta para o gerente e o atendente. Na outra ponta, a "
        "<b>entrada de mercadoria</b> registra a nota fiscal, a validade do lote, o custo e o preço; o sistema sugere "
        "o preço pela margem de lucro sobre a venda, e o banco recusa preço abaixo do custo. O produto pode ser "
        "cadastrado na própria entrada, e o <b>fornecedor</b> é escolhido entre os cadastrados, já vem preenchido com "
        "o último fornecedor daquele produto, ou é cadastrado na hora.",
        "1) estoque.html: mostrar os alertas de reposição e de validade no topo. 2) Fazer uma venda e mostrar o estoque "
        "diminuindo. 3) Aba Entrada de Mercadoria: escolher um produto, mostrar o fornecedor preenchido e o cálculo da "
        "margem; clicar em <b>+ Novo fornecedor</b> e mostrar a janela de cadastro. 4) No VS Code, mostrar a trigger "
        "no ScriptBancoDeDados.sql (linha 351) e a tabela fornecedores (linha 266).")
    c.append(Paragraph("<b>Passagem para o Aluno 2:</b> “Essa é a base técnica do sistema. Agora o meu colega vai "
                       "mostrar como as regras de negócio funcionam na prática, o balcão e a parte de Inteligência "
                       "Artificial.”", E["caixa"]))
    return c + [PageBreak()]


def aluno2():
    c = [Paragraph("3. Roteiro do Aluno 2", E["h1"])]
    c += faixa_aluno(2, "Regras de negócio, balcão, IA, testes e encerramento", "≈ 9 min")
    c += topico(2, "3.1 Regras de negócio",
        "O coração do sistema é o agendamento, e ele segue regras que o sistema não deixa quebrar. <b>RN01</b>: nunca "
        "dois serviços com o mesmo profissional no mesmo horário. <b>RN02</b>: o tutor só cancela pela internet com "
        "pelo menos 2 horas de antecedência; a equipe pode cancelar a qualquer momento. <b>RN03</b>: atendimento das "
        "8h às 19h, fechado aos domingos. <b>RN04</b>: o status segue Pendente, depois Confirmado, depois Concluído, "
        "sem pular nem voltar etapa. <b>RN10</b>: o usuário escolhe só o início; o fim é calculado somando a duração "
        "do serviço.",
        "Tabela de regras de negócio do relatório ou o GUIA_DE_EXECUCAO.txt, seção 5.")
    c += topico(2, "3.2 O fluxo de criação de um agendamento",
        "Quando um agendamento chega, o AgendamentoService faz nesta ordem: busca o serviço para saber a duração; "
        "recusa datas no passado; calcula o horário de fim; recusa domingo e horários fora do expediente; se quem "
        "pede é um tutor, confere se o pet é dele; e verifica conflito de horário. Para o conflito usamos a fórmula "
        "clássica de <b>sobreposição de intervalos</b>: início do novo menor que o fim do existente, e fim do novo "
        "maior que o início do existente. Se passar em tudo, salva, consulta a IA e envia e-mail de confirmação. A "
        "IA e o e-mail ficam dentro de try/catch: se a IA estiver fora do ar, o agendamento é feito mesmo assim. A IA "
        "é um apoio, não um ponto de falha.",
        "BLL/AgendamentoService.cs: linhas 102 e 103 (RN10), linhas 105 a 115 (RN03), linhas 124 a 136 (RN01, a "
        "fórmula), linhas 145 a 181 (chamada à IA com tempo limite de 3 segundos).")
    c += topico(2, "3.3 Demonstração ao vivo: do agendamento à ordem de serviço",
        "Vou mostrar como cliente. Escolho o pet e um serviço de banho e tosa; o sistema lista os três profissionais "
        "de banho e tosa e só os horários livres, de 15 em 15 minutos. Confirmo, e o cliente recebe o e-mail de "
        "confirmação. Agora entro como a atendente Ana: o sistema abre direto na agenda. Navego pelo dia ou pela "
        "semana e vejo o pet, o cliente e o telefone com WhatsApp, e posso anotar observações sobre o pet. Quando o "
        "cliente chega, clico em <b>Cliente chegou</b> e ele vai para a aba Em processo. Quando o pet fica pronto, "
        "clico em <b>Finalizar e receber</b>, informo o valor e a forma de pagamento, e o atendimento é concluído. Se "
        "eu tentar receber antes de registrar a chegada, o sistema bloqueia, porque essa regra está dentro da própria "
        "classe Agendamento. Todo atendimento tem a sua <b>ordem de serviço</b>, gerada automaticamente: posso "
        "imprimir em A4, baixar em PDF para mandar ao cliente ou imprimir o cupom numa impressora térmica de 80 ou "
        "58 milímetros.",
        "1) Login como cliente@patafacil.com e agendar em agendamento.html. 2) Mostrar a Caixa de E-mail do cliente "
        "com a confirmação. 3) Login como atendente@patafacil.com: agenda abre sozinha; mostrar Dia/Semana e as setas. "
        "4) Escrever uma observação, clicar em Cliente chegou, ir em Em processo e Finalizar e receber (escolher "
        "Dinheiro e mostrar o troco). 5) No mesmo cartão, clicar em <b>Ordem de serviço</b>: mostrar A4, trocar para "
        "Térmica 80 mm e clicar em Baixar PDF. 6) Mostrar a tela Ordens de Serviço com o filtro por situação.")
    c += topico(2, "3.4 Inteligência Artificial",
        "A IA é um microsserviço em Python com FastAPI e scikit-learn, com três modelos. O primeiro é um <b>Random "
        "Forest</b> que prevê o risco de o cliente faltar, usando a antecedência, os cancelamentos anteriores, o "
        "serviço, o dia da semana e se o tutor é novo; ele tem <b>74% de acurácia</b>. O segundo é uma <b>Regressão "
        "Linear</b> que prevê o movimento do dia, com <b>R² de 0,83</b>, e aparece na Visão Geral. O terceiro é o "
        "<b>K-Means</b>, que agrupa os clientes em quatro perfis: Ocasionais, Em Risco, Fiéis e VIP. Como o pet shop "
        "ainda não tem histórico real suficiente, os modelos são treinados com <b>dados sintéticos</b> que seguem o "
        "comportamento esperado do negócio. Quando houver dados reais, basta trocar as funções que geram os dados "
        "pela leitura do banco.",
        "1) http://localhost:8000/health mostrando os 3 modelos e as métricas. 2) dashboard.html com a demanda "
        "prevista e o gráfico de movimento da semana. 3) MachineLearning/ml_api.py: as funções gerar_dados_* e o "
        "treino dos 3 modelos.")
    c += topico(2, "3.5 Testes, DevOps, mobile e acessibilidade",
        "Temos <b>28 testes automatizados</b> com xUnit e Moq que cobrem as regras RN01, RN02, RN03, RN04, RN10 e "
        "RN15, a chegada e o pagamento no balcão, a entrada de mercadoria com o cálculo da margem, além do BCrypt. "
        "Eles rodam sem banco, porque os repositórios são simulados; isso só é possível graças à injeção de "
        "dependência. Temos também Dockerfiles, docker-compose e um pipeline de CI no GitHub Actions. O tutor tem um "
        "aplicativo mobile em React Native que usa a mesma API. E todas as páginas têm o <b>VLibras</b>, que traduz o "
        "conteúdo para Libras, com uma interface padronizada e de leitura confortável.",
        "Pasta Tests/ (AgendamentoServiceTests.cs e EstoqueServiceTests.cs), o resultado do dotnet test e o ícone do "
        "VLibras em qualquer tela.")
    c += topico(2, "3.6 Limitações e encerramento",
        "Como melhorias futuras: treinar a IA com dados reais do banco; proteger a verificação de conflito também com "
        "uma trava no banco, para o caso de duas pessoas agendarem no mesmo segundo; ligar o envio de e-mails a um "
        "serviço real; e, em produção, guardar a chave do JWT numa variável de ambiente e restringir o CORS. "
        "Concluindo: o PataFácil organiza a agenda, impede erros de marcação, acompanha o cliente do balcão até a "
        "ordem de serviço, controla compras e estoque automaticamente e usa IA para antecipar faltas e demanda. "
        "Obrigado. Estamos à disposição para perguntas.",
        "Voltar para a Visão Geral ou para a tela de login.")
    return c + [PageBreak()]


def perguntas():
    c = [Paragraph("4. Perguntas prováveis da banca", E["h1"]),
         Paragraph("Quem é indicado na coluna “Responde” começa a resposta; o outro aluno pode complementar. Admitir "
                   "uma limitação e explicar como resolvê-la costuma ser mais bem avaliado do que fugir da pergunta.",
                   E["texto"]), Spacer(1, 8)]
    linhas = [
        ["Qual a diferença entre Controller e Service?", "Aluno 1", "O Controller cuida do HTTP e das permissões; o Service tem as regras de negócio. O Controller só repassa o pedido."],
        ["Por que usar o padrão Repository?", "Aluno 1", "Isola o acesso ao banco: o Service não sabe que é MySQL. Um único Repositorio&lt;T&gt; atende todas as entidades, e nos testes ele é trocado por um simulado."],
        ["Onde está o polimorfismo?", "Aluno 1", "Pessoa.GetPerfil() é abstrato; Tutor e Funcionario implementam. No login os dois são tratados como Pessoa."],
        ["Como funciona o JWT?", "Aluno 1", "No login a API gera um token assinado com uma chave secreta, contendo id e perfil, válido por 8 horas. Cada requisição envia o token e a API confere a assinatura e o perfil."],
        ["Por que a baixa de estoque é uma trigger e não código C#?", "Aluno 1", "Garante a RN15 mesmo se a venda for gravada por outro caminho, e evita baixar duas vezes."],
        ["Como o preço de venda é sugerido?", "Aluno 1", "Pela margem sobre a venda: preço = custo ÷ (1 − margem). Com custo de R$ 100 e margem de 45%, o preço é R$ 181,82, e o lucro de R$ 81,82 é 45% do preço. Somar 45% ao custo daria só 31% de margem."],
        ["Por que a entrada guarda o nome do fornecedor e não só o id?", "Aluno 1", "Para preservar o histórico da nota como ela foi recebida. O cadastro de fornecedores tem nome único, e um nome digitado direto na entrada entra no cadastro automaticamente."],
        ["A chave do JWT está no appsettings.json. Isso é seguro?", "Aluno 1", "Para desenvolvimento, sim. Em produção iria para variável de ambiente ou cofre de segredos, e o CORS seria restrito ao domínio do site."],
        ["E se duas pessoas agendarem o mesmo horário no mesmo segundo?", "Aluno 2", "A checagem da RN01 é feita antes de salvar, então em tese as duas poderiam passar. A melhoria é usar transação ou trava no banco."],
        ["Como a ordem de serviço é gerada? Ela tem tabela própria?", "Aluno 2", "Não precisa: a OS é montada a partir do agendamento e numerada pelo id dele, então sempre existe e nunca fica desatualizada. A impressão usa o tamanho de página do CSS (A4 ou bobina de 80/58 mm) e o PDF é gerado no próprio navegador."],
        ["Os dados da IA são reais?", "Aluno 2", "Não, são sintéticos, porque o pet shop ainda não tem histórico. As regras de geração imitam o negócio; com dados reais, basta trocar as funções gerar_dados_*."],
        ["74% de acurácia é bom?", "Aluno 2", "Para um modelo de apoio que só coloca uma etiqueta e não bloqueia nada, é aceitável. Com dados reais a tendência é melhorar."],
        ["Por que a IA está em Python e não em C#?", "Aluno 2", "O scikit-learn é o padrão de mercado para Machine Learning. Como microsserviço separado, se a IA cair o resto do sistema continua funcionando."],
        ["O que acontece se a IA estiver desligada?", "Aluno 2", "Nada quebra: a chamada tem tempo limite de 3 segundos e fica dentro de try/catch. O agendamento é salvo sem a etiqueta de risco."],
        ["Como vocês garantem que as regras funcionam?", "Aluno 2", "Com os 28 testes automatizados (xUnit e Moq), que rodam sem banco e cobrem as principais regras."],
        ["Como o sistema impede receber um pagamento duas vezes?", "Aluno 2", "Chegada e pagamento são métodos da classe Agendamento (encapsulamento). O pagamento exige a chegada registrada e recusa uma segunda baixa."],
    ]
    c.append(tabela(["Pergunta", "Responde", "Resposta curta"], linhas, [4.6 * cm, 2 * cm, LARGURA - 6.6 * cm],
                    lambda l: ALUNO[1][1] if l[1] == "Aluno 1" else ALUNO[2][1]))
    return c


def main():
    doc = SimpleDocTemplate(str(SAIDA), pagesize=A4, leftMargin=2 * cm, rightMargin=2 * cm, topMargin=1.8 * cm,
                            bottomMargin=2 * cm, title="PataFácil - Roteiro de apresentação do PIM IV",
                            author="Equipe PataFácil")
    doc.build(capa() + divisao() + aluno1() + aluno2() + perguntas(), onFirstPage=rodape, onLaterPages=rodape)
    print("Roteiro gerado:", SAIDA)


if __name__ == "__main__":
    main()
