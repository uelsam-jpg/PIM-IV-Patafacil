"""Gera o diagrama de classes e o MER do relatório com o Graphviz (dot).

Uso (a partir da raiz do projeto):
    python Documentacao/diagramas/gerar_diagramas.py

Saída: Documentacao/figuras/dia_classes.png e dia_mer.png. Requer o Graphviz instalado.
"""
import html
import shutil
import subprocess
from pathlib import Path

PASTA = Path(__file__).resolve().parent
FIGURAS = PASTA.parent / "figuras"
DOT = shutil.which("dot") or r"C:\Program Files\Graphviz\bin\dot.exe"

BORDA, CABECALHO, LINHA = "#20b2aa", "#e6f6f5", "#4b5563"


def caixa(titulo, secoes, estereotipo=None):
    """Tabela HTML do Graphviz: cabeçalho e uma ou mais seções de linhas (atributos, métodos)."""
    topo = (f'<FONT POINT-SIZE="10">«{estereotipo}»</FONT><BR/>' if estereotipo else "") + f"<B>{html.escape(titulo)}</B>"
    linhas = [f'<TR><TD BGCOLOR="{CABECALHO}" CELLPADDING="6">{topo}</TD></TR>']
    for secao in secoes:
        if not secao:
            continue
        itens = "<BR ALIGN=\"LEFT\"/>".join(html.escape(i) for i in secao) + '<BR ALIGN="LEFT"/>'
        linhas.append(f'<TR><TD ALIGN="LEFT" BALIGN="LEFT" CELLPADDING="6">{itens}</TD></TR>')
    return ('<<TABLE BORDER="1" CELLBORDER="1" CELLSPACING="0" COLOR="' + BORDA + '" BGCOLOR="white">'
            + "".join(linhas) + "</TABLE>>")


def cabecalho_grafo(nome, rankdir, extra=""):
    return (f'digraph {nome} {{\n  graph [rankdir={rankdir}, bgcolor="white", nodesep=0.45, ranksep=0.9, pad=0.3, dpi=220 {extra}];\n'
            f'  node [shape=plain, fontname="Arial", fontsize=13];\n'
            f'  edge [fontname="Arial", fontsize=11, color="{LINHA}", fontcolor="{LINHA}", arrowsize=0.8];\n')


# ------------------------------------------------------------------ diagrama de classes
CLASSES = {
    "Pessoa": ([["+int Id", "+string Nome", "+string Cpf", "+string Email", "+bool Ativo"], ["+GetPerfil() : string  {abstrato}"]], "abstract"),
    "Tutor": ([["+string Telefone", "+string SenhaHash", "+DateTime DataCadastro"], ["+GetPerfil() : string"]], None),
    "Funcionario": ([["+Cargo Cargo", "+string SenhaHash", "+DateTime DataAdmissao"], ["+GetPerfil() : string"]], None),
    "Pet": ([["+string Nome", "+Especie Especie", "+string Raca", "+decimal PesoKg"]], None),
    "Servico": ([["+string Nome", "+int DuracaoMinutos", "+decimal Preco"]], None),
    "Agendamento": ([["+DateTime DataHoraInicio", "+DateTime DataHoraFim", "-StatusAgendamento Status", "-DateTime? DataChegada",
                      "-decimal? ValorPago", "-FormaPagamentoEnum? FormaPagamento"],
                     ["+Confirmar()", "+Cancelar()", "+Concluir()", "+RegistrarChegada()", "+RegistrarPagamento(valor, forma)"]], None),
    "HistoricoClinico": ([["+TipoAtendimentoEnum Tipo", "+string Descricao", "+string Diagnostico", "+DateTime? ProximaVacina"]], None),
    "Venda": ([["+DateTime DataVenda", "+decimal Total", "+FormaPagamentoEnum FormaPagamento"]], None),
    "ItemVenda": ([["+int Quantidade", "+decimal PrecoUnitario", "+decimal Subtotal"]], None),
    "Produto": ([["+string Nome", "+CategoriaProdutoEnum Categoria", "+decimal PrecoCusto", "+decimal PrecoVenda"]], None),
    "Estoque": ([["+int QuantidadeAtual", "+int PontoReposicao"]], None),
    "EntradaEstoque": ([["+int Quantidade", "+string NumeroNota", "+string Fornecedor", "+DateTime? Validade",
                         "+decimal CustoUnitario", "+decimal PrecoVendaAplicado"]], None),
    "Fornecedor": ([["+string Nome", "+string Cnpj", "+string Telefone", "+string Email"]], None),
    "EmailEnviado": ([["+string Destinatario", "+string Assunto", "+DateTime DataEnvio", "+bool Lida"]], None),
}
# (origem, destino, rótulo, mult. origem, mult. destino, estilo)
RELACOES_CLASSES = [
    ("Tutor", "Pet", "possui", "1", "0..*", "assoc"),
    ("Pet", "Agendamento", "", "1", "0..*", "assoc"),
    ("Servico", "Agendamento", "", "1", "0..*", "assoc"),
    ("Funcionario", "Agendamento", "atende", "1", "0..*", "assoc"),
    ("Pet", "HistoricoClinico", "", "1", "0..*", "assoc"),
    ("Agendamento", "HistoricoClinico", "gera", "0..1", "0..*", "assoc"),
    ("Funcionario", "Venda", "registra", "1", "0..*", "assoc"),
    ("Venda", "ItemVenda", "", "1", "1..*", "composicao"),
    ("Produto", "ItemVenda", "", "1", "0..*", "assoc"),
    ("Produto", "Estoque", "", "1", "1", "assoc"),
    ("Produto", "EntradaEstoque", "recebe", "1", "0..*", "assoc"),
    ("Funcionario", "EntradaEstoque", "confere", "1", "0..*", "assoc"),
    ("Fornecedor", "EntradaEstoque", "fornece", "1", "0..*", "dependencia"),
]


def diagrama_classes():
    dot = [cabecalho_grafo("Classes", "LR")]
    for nome, (secoes, estereotipo) in CLASSES.items():
        dot.append(f'  {nome} [label={caixa(nome, secoes, estereotipo)}];')
    # Herança: seta vazada apontando para a classe base
    for filha in ("Tutor", "Funcionario"):
        dot.append(f'  {filha} -> Pessoa [arrowhead=onormal, arrowsize=1.2, constraint=false];')
    dot.append('  Pessoa -> Tutor [style=invis]; Pessoa -> Funcionario [style=invis];')
    for a, b, rotulo, ma, mb, estilo in RELACOES_CLASSES:
        attrs = [f'taillabel="{ma}"', f'headlabel="{mb}"', 'labeldistance=1.6']
        if rotulo:
            attrs.append(f'label=" {rotulo} "')
        if estilo == "composicao":
            attrs += ["dir=both", "arrowtail=diamond", "arrowhead=vee"]
        elif estilo == "dependencia":
            attrs += ["style=dashed", "arrowhead=vee"]
        else:
            attrs.append("arrowhead=vee")
        dot.append(f'  {a} -> {b} [{", ".join(attrs)}];')
    dot.append('  { rank=same; Fornecedor; EmailEnviado; }')
    dot.append("}")
    return "\n".join(dot)


# ------------------------------------------------------------------ MER
TABELAS = {
    "tutores": ["Id (PK)", "Nome", "Cpf (UK)", "Email (UK)", "Telefone", "SenhaHash"],
    "pets": ["Id (PK)", "Nome", "Especie", "Raca", "PesoKg", "TutorId (FK)"],
    "funcionarios": ["Id (PK)", "Nome", "Email (UK)", "Cargo", "SenhaHash"],
    "servicos": ["Id (PK)", "Nome", "DuracaoMinutos", "Preco"],
    "agendamentos": ["Id (PK)", "DataHoraInicio", "DataHoraFim", "Status", "DataChegada", "ValorPago", "FormaPagamento",
                     "IdPet (FK)", "IdServico (FK)", "IdFuncionario (FK)"],
    "log_agendamento_status": ["Id (PK)", "IdAgendamento (FK)", "StatusAnterior", "StatusNovo", "DataMudanca"],
    "historicos": ["Id (PK)", "Tipo", "Descricao", "ProximaVacina", "IdPet (FK)", "IdFuncionario (FK)", "IdAgendamento (FK)"],
    "vendas": ["Id (PK)", "DataVenda", "Total", "FormaPagamento", "IdTutor (FK)", "IdFuncionario (FK)"],
    "itensvenda": ["Id (PK)", "Quantidade", "PrecoUnitario", "Subtotal", "IdVenda (FK)", "IdProduto (FK)"],
    "produtos": ["Id (PK)", "Nome", "Categoria", "PrecoCusto", "PrecoVenda"],
    "estoques": ["Id (PK)", "QuantidadeAtual", "PontoReposicao", "IdProduto (FK, UK)"],
    "alerta_reposicao": ["Id (PK)", "IdProduto (FK)", "QuantidadeAtual", "Resolvido"],
    "entradas_estoque": ["Id (PK)", "Quantidade", "NumeroNota", "Fornecedor", "Validade", "CustoUnitario",
                         "IdProduto (FK)", "IdFuncionario (FK)"],
    "fornecedores": ["Id (PK)", "Nome (UK)", "Cnpj", "Telefone", "Email"],
    "emails": ["Id (PK)", "Destinatario", "Assunto", "DataEnvio", "Lida"],
}
# (lado "um", lado "muitos", rótulo, cardinalidade do lado um, do lado muitos, tracejado)
RELACOES_MER = [
    ("tutores", "pets", "possui", "1", "N", False),
    ("pets", "agendamentos", "agenda", "1", "N", False),
    ("servicos", "agendamentos", "define", "1", "N", False),
    ("funcionarios", "agendamentos", "atende", "1", "N", False),
    ("agendamentos", "log_agendamento_status", "registra", "1", "N", False),
    ("pets", "historicos", "tem", "1", "N", False),
    ("funcionarios", "historicos", "registra", "1", "N", False),
    ("agendamentos", "historicos", "origina", "0..1", "N", False),
    ("tutores", "vendas", "compra", "0..1", "N", False),
    ("funcionarios", "vendas", "registra", "1", "N", False),
    ("vendas", "itensvenda", "contém", "1", "1..N", False),
    ("produtos", "itensvenda", "vendido em", "1", "N", False),
    ("produtos", "estoques", "saldo", "1", "1", False),
    ("produtos", "alerta_reposicao", "alerta", "1", "N", False),
    ("produtos", "entradas_estoque", "recebe", "1", "N", False),
    ("funcionarios", "entradas_estoque", "confere", "1", "N", False),
    ("fornecedores", "entradas_estoque", "fornece (pelo nome)", "1", "N", True),
]


def diagrama_mer():
    dot = [cabecalho_grafo("MER", "TB")]
    for nome, colunas in TABELAS.items():
        dot.append(f'  {nome} [label={caixa(nome.upper(), [colunas])}];')
    for a, b, rotulo, ca, cb, tracejado in RELACOES_MER:
        estilo = ', style=dashed' if tracejado else ''
        dot.append(f'  {a} -> {b} [label=" {rotulo} ", taillabel="{ca}", headlabel="{cb}", labeldistance=1.8, '
                   f'arrowhead=crow, arrowtail=tee, dir=both{estilo}];')
    dot.append('  { rank=same; fornecedores; emails; }')
    dot.append("}")
    return "\n".join(dot)


def renderizar(nome, fonte):
    arquivo_dot = PASTA / f"{nome}.dot"
    arquivo_dot.write_text(fonte, encoding="utf-8")
    saida = FIGURAS / f"{nome}.png"
    subprocess.run([DOT, "-Tpng", str(arquivo_dot), "-o", str(saida)], check=True)
    print("gerado:", saida)


if __name__ == "__main__":
    renderizar("dia_classes", diagrama_classes())
    renderizar("dia_mer", diagrama_mer())
