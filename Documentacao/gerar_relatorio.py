"""Gera o relatório final do PIM IV (Word + PDF) no padrão ABNT pedido pelo manual da UNIP.

Uso (a partir da raiz do projeto):
    python Documentacao/gerar_relatorio.py

Os dados do aluno ficam em Documentacao/dados_aluno.json e o texto em conteudo_relatorio.py.
O PDF é exportado pelo Microsoft Word, que também atualiza o sumário e as listas.
"""
import json
import shutil
import subprocess
import sys
from pathlib import Path

from docx import Document
from docx.enum.section import WD_SECTION
from docx.enum.table import WD_TABLE_ALIGNMENT
from docx.enum.text import WD_ALIGN_PARAGRAPH, WD_BREAK, WD_LINE_SPACING
from docx.oxml import OxmlElement
from docx.oxml.ns import qn
from docx.shared import Cm, Pt, RGBColor

import conteudo_relatorio as conteudo

PASTA = Path(__file__).resolve().parent
RAIZ = PASTA.parent
FIGURAS = PASTA / "figuras"
SAIDA_DOCX = RAIZ / "PIM_IV_PataFacil_Relatorio_Final.docx"
SAIDA_PDF = RAIZ / "PIM_IV_PataFacil_Relatorio_Final.pdf"

FONTE = "Arial"
LARGURA_UTIL_CM = 16.0  # A4 (21 cm) menos as margens esquerda (3 cm) e direita (2 cm)


# ---------------------------------------------------------------- utilidades de formatação
def definir_fonte(run, tamanho=12, negrito=None, italico=None, nome=FONTE):
    run.font.name = nome
    run._element.rPr.rFonts.set(qn("w:eastAsia"), nome)
    run.font.size = Pt(tamanho)
    run.font.color.rgb = RGBColor(0, 0, 0)
    if negrito is not None:
        run.font.bold = negrito
    if italico is not None:
        run.font.italic = italico


def formatar(par, alinhamento=WD_ALIGN_PARAGRAPH.JUSTIFY, entrelinhas=1.5, recuo_cm=0.0, antes=0, depois=0, esquerda_cm=0.0):
    fmt = par.paragraph_format
    par.alignment = alinhamento
    fmt.line_spacing = entrelinhas
    fmt.first_line_indent = Cm(recuo_cm) if recuo_cm else None
    fmt.left_indent = Cm(esquerda_cm) if esquerda_cm else None
    fmt.space_before = Pt(antes)
    fmt.space_after = Pt(depois)
    return par


def escrever(par, texto, tamanho=12, negrito=None, nome=FONTE):
    """Escreve o texto no parágrafo; trechos entre ** ** saem em negrito e entre __ __ em itálico."""
    import re

    for parte in re.split(r"(\*\*.+?\*\*|__.+?__)", texto):
        if not parte:
            continue
        if parte.startswith("**"):
            definir_fonte(par.add_run(parte[2:-2]), tamanho, True, nome=nome)
        elif parte.startswith("__"):
            definir_fonte(par.add_run(parte[2:-2]), tamanho, negrito, True, nome=nome)
        else:
            definir_fonte(par.add_run(parte), tamanho, negrito, nome=nome)
    return par


def campo(par, instrucao, texto_inicial="1", tamanho=12, negrito=None):
    """Insere um campo do Word (PAGE, SEQ, TOC...) que o Word recalcula ao abrir/exportar."""
    for tipo in ("begin", "instr", "separate", "texto", "end"):
        run = par.add_run()
        definir_fonte(run, tamanho, negrito)
        if tipo == "instr":
            el = OxmlElement("w:instrText")
            el.set(qn("xml:space"), "preserve")
            el.text = instrucao
            run._element.append(el)
        elif tipo == "texto":
            run.text = texto_inicial
        else:
            el = OxmlElement("w:fldChar")
            el.set(qn("w:fldCharType"), tipo)
            run._element.append(el)


def sombrear(celula, cor_hex):
    tc_pr = celula._element.get_or_add_tcPr()
    shd = OxmlElement("w:shd")
    shd.set(qn("w:val"), "clear")
    shd.set(qn("w:color"), "auto")
    shd.set(qn("w:fill"), cor_hex)
    tc_pr.append(shd)


def repetir_cabecalho(linha):
    tr_pr = linha._tr.get_or_add_trPr()
    el = OxmlElement("w:tblHeader")
    el.set(qn("w:val"), "true")
    tr_pr.append(el)


def manter_com_proximo(par):
    par.paragraph_format.keep_with_next = True
    return par


# ---------------------------------------------------------------- construção do documento
class Relatorio:
    def __init__(self, dados):
        self.dados = dados
        self.doc = Document()
        self._configurar_estilos()
        secao = self.doc.sections[0]
        self._configurar_pagina(secao)
        # A capa não entra na contagem: começando em 0, a folha de rosto é a página 1
        self._numeracao_inicial(secao, 0)

    # ---- página e estilos
    @staticmethod
    def _configurar_pagina(secao):
        secao.page_width, secao.page_height = Cm(21), Cm(29.7)
        secao.top_margin, secao.left_margin = Cm(3), Cm(3)
        secao.bottom_margin, secao.right_margin = Cm(2), Cm(2)
        secao.header_distance = Cm(2)
        secao.footer_distance = Cm(1.25)

    @staticmethod
    def _numeracao_inicial(secao, inicio):
        pg = OxmlElement("w:pgNumType")
        pg.set(qn("w:start"), str(inicio))
        secao._sectPr.append(pg)

    def _configurar_estilos(self):
        estilos = self.doc.styles
        normal = estilos["Normal"]
        normal.font.name = FONTE
        normal.element.rPr.rFonts.set(qn("w:eastAsia"), FONTE)
        normal.font.size = Pt(12)

        # Títulos: mesma fonte do texto, em preto (as seções numeradas alimentam o sumário)
        for nome, caixa_alta in (("Heading 1", True), ("Heading 2", False), ("Heading 3", False)):
            estilo = estilos[nome]
            estilo.font.name = FONTE
            r_fonts = estilo.element.rPr.get_or_add_rFonts()
            for atributo in ("w:ascii", "w:hAnsi", "w:eastAsia", "w:cs"):
                r_fonts.set(qn(atributo), FONTE)
            for tema in ("w:asciiTheme", "w:hAnsiTheme", "w:eastAsiaTheme", "w:cstheme"):
                if r_fonts.get(qn(tema)) is not None:
                    del r_fonts.attrib[qn(tema)]
            estilo.font.size = Pt(12)
            estilo.font.bold = True
            estilo.font.italic = False
            estilo.font.all_caps = caixa_alta
            estilo.font.color.rgb = RGBColor(0, 0, 0)
            fmt = estilo.paragraph_format
            fmt.alignment = WD_ALIGN_PARAGRAPH.LEFT
            fmt.line_spacing = 1.5
            fmt.space_before = Pt(0 if nome == "Heading 1" else 12)
            fmt.space_after = Pt(12)
            fmt.keep_with_next = True
            fmt.page_break_before = nome == "Heading 1"

        # Sumário e listas: fonte do texto, entrelinhas simples
        for nome in ("TOC 1", "TOC 2", "TOC 3", "Table of Figures"):
            try:
                estilo = estilos[nome]
            except KeyError:
                estilo = estilos.add_style(nome, 1)
            estilo.font.name = FONTE
            estilo.font.size = Pt(12)
            estilo.font.bold = nome == "TOC 1"
            estilo.font.all_caps = nome == "TOC 1"
            estilo.paragraph_format.space_after = Pt(6)
            estilo.paragraph_format.line_spacing = 1.0

    # ---- blocos básicos
    def paragrafo(self, texto, **kw):
        par = self.doc.add_paragraph()
        opcoes = dict(recuo_cm=1.5)
        opcoes.update(kw)
        tamanho = opcoes.pop("tamanho", 12)
        negrito = opcoes.pop("negrito", None)
        formatar(par, **opcoes)
        escrever(par, texto, tamanho, negrito)
        return par

    def linhas_em_branco(self, quantidade, entrelinhas=1.5):
        for _ in range(quantidade):
            formatar(self.doc.add_paragraph(), entrelinhas=entrelinhas)

    def quebra_de_pagina(self):
        self.doc.add_paragraph().add_run().add_break(WD_BREAK.PAGE)

    def titulo_pre_textual(self, texto):
        """Título centralizado e sem número: não entra no sumário (resumo, listas, sumário)."""
        par = self.doc.add_paragraph()
        formatar(par, WD_ALIGN_PARAGRAPH.CENTER, depois=18)
        par.paragraph_format.page_break_before = True
        escrever(par, texto, negrito=True)

    def secao(self, nivel, texto):
        par = self.doc.add_heading(level=nivel)
        definir_fonte(par.add_run(texto), 12, True)
        return par

    def alineas(self, itens):
        letras = "abcdefghijklmnopqrstuvwxyz"
        for i, item in enumerate(itens):
            final = "." if i == len(itens) - 1 else ";"
            par = self.doc.add_paragraph()
            formatar(par, esquerda_cm=2.0)
            par.paragraph_format.first_line_indent = Cm(-0.5)
            escrever(par, f"{letras[i]}) {item}{final}")

    def codigo(self, texto, tamanho=9, manter_junto=True):
        for linha in texto.strip("\n").split("\n"):
            par = self.doc.add_paragraph()
            formatar(par, WD_ALIGN_PARAGRAPH.LEFT, entrelinhas=1.0, esquerda_cm=1.0)
            if manter_junto:
                manter_com_proximo(par)
            definir_fonte(par.add_run(linha if linha else " "), tamanho, nome="Courier New")
        formatar(self.doc.add_paragraph(), entrelinhas=1.0)

    # ---- ilustrações e quadros (título acima, fonte abaixo — ABNT)
    def _legenda(self, rotulo, titulo):
        par = self.doc.add_paragraph()
        formatar(par, WD_ALIGN_PARAGRAPH.CENTER, entrelinhas=1.0, antes=6, depois=4)
        manter_com_proximo(par)
        escrever(par, f"{rotulo} ", 10, True)
        campo(par, f"SEQ {rotulo} \\* ARABIC", "1", 10, True)
        escrever(par, f" – {titulo}", 10, True)

    def _fonte(self, texto=None):
        par = self.doc.add_paragraph()
        formatar(par, WD_ALIGN_PARAGRAPH.CENTER, entrelinhas=1.0, antes=4, depois=12)
        autoria = "pelos autores" if len(self.dados["alunos"]) > 1 else "pelo autor"
        escrever(par, texto or f"Fonte: elaborado {autoria} ({self.dados['ano']}).", 10)

    def figura(self, arquivos, titulo, largura_cm=15.0, fonte=None):
        """Uma figura com uma ou mais imagens lado a lado (largura_cm é a de cada imagem)."""
        if isinstance(arquivos, str):
            arquivos = [arquivos]
        self._legenda("Figura", titulo)
        par = self.doc.add_paragraph()
        formatar(par, WD_ALIGN_PARAGRAPH.CENTER, entrelinhas=1.0)
        manter_com_proximo(par)
        for i, arquivo in enumerate(arquivos):
            if i:
                par.add_run("  ")
            par.add_run().add_picture(str(FIGURAS / arquivo), width=Cm(largura_cm))
        self._fonte(fonte)

    def quadro(self, titulo, cabecalho, linhas, larguras_cm, tamanho=10, fonte=None):
        self._legenda("Quadro", titulo)
        tabela = self.doc.add_table(rows=1, cols=len(cabecalho))
        tabela.style = "Table Grid"
        tabela.alignment = WD_TABLE_ALIGNMENT.CENTER
        tabela.autofit = False

        def preencher(celula, texto, negrito=None, centro=False):
            par = celula.paragraphs[0]
            formatar(par, WD_ALIGN_PARAGRAPH.CENTER if centro else WD_ALIGN_PARAGRAPH.LEFT, entrelinhas=1.0, antes=2, depois=2)
            escrever(par, str(texto), tamanho, negrito)

        for i, texto in enumerate(cabecalho):
            celula = tabela.rows[0].cells[i]
            preencher(celula, texto, True, True)
            sombrear(celula, "D9D9D9")
        repetir_cabecalho(tabela.rows[0])

        for linha in linhas:
            celulas = tabela.add_row().cells
            for i, texto in enumerate(linha):
                preencher(celulas[i], texto)

        ultima = len(tabela.rows) - 1
        for n, linha in enumerate(tabela.rows):
            # Linhas não se partem entre páginas e o quadro acompanha a sua legenda
            nao_partir = OxmlElement("w:cantSplit")
            nao_partir.set(qn("w:val"), "true")
            linha._tr.get_or_add_trPr().append(nao_partir)
            for i, largura in enumerate(larguras_cm):
                linha.cells[i].width = Cm(largura)
                if n < ultima and len(tabela.rows) <= 12:
                    for par in linha.cells[i].paragraphs:
                        manter_com_proximo(par)
        self._fonte(fonte)

    # ---- elementos pré-textuais
    def capa(self):
        d = self.dados
        for texto in ("UNIVERSIDADE PAULISTA – UNIP", "Projeto Integrado Multidisciplinar",
                      "Curso Superior de Tecnologia em", "Análise e Desenvolvimento de Sistemas"):
            escrever(formatar(self.doc.add_paragraph(), WD_ALIGN_PARAGRAPH.CENTER), texto, negrito=True)
        self.linhas_em_branco(4)
        for aluno in d["alunos"]:
            escrever(formatar(self.doc.add_paragraph(), WD_ALIGN_PARAGRAPH.CENTER), f"{aluno['nome']} – {aluno['ra']}".upper())
        self.linhas_em_branco(max(1, 6 - len(d["alunos"])))
        escrever(formatar(self.doc.add_paragraph(), WD_ALIGN_PARAGRAPH.CENTER), conteudo.TITULO, negrito=True)
        escrever(formatar(self.doc.add_paragraph(), WD_ALIGN_PARAGRAPH.CENTER), conteudo.SUBTITULO)
        self.linhas_em_branco(max(1, 9 - len(d["alunos"])))
        escrever(formatar(self.doc.add_paragraph(), WD_ALIGN_PARAGRAPH.CENTER), d["cidade"].upper())
        escrever(formatar(self.doc.add_paragraph(), WD_ALIGN_PARAGRAPH.CENTER), d["ano"])

    def folha_de_rosto(self):
        d = self.dados
        self.quebra_de_pagina()
        for aluno in d["alunos"]:
            escrever(formatar(self.doc.add_paragraph(), WD_ALIGN_PARAGRAPH.CENTER), f"{aluno['nome']} – {aluno['ra']}".upper())
        self.linhas_em_branco(max(1, 7 - len(d["alunos"])))
        escrever(formatar(self.doc.add_paragraph(), WD_ALIGN_PARAGRAPH.CENTER), conteudo.TITULO, negrito=True)
        escrever(formatar(self.doc.add_paragraph(), WD_ALIGN_PARAGRAPH.CENTER), conteudo.SUBTITULO)
        escrever(formatar(self.doc.add_paragraph(), WD_ALIGN_PARAGRAPH.CENTER),
                 "Projeto Integrado Multidisciplinar em Análise e Desenvolvimento de Sistemas")
        self.linhas_em_branco(3)
        natureza = ("Projeto Integrado Multidisciplinar para obtenção do título de tecnólogo em Análise e "
                    "Desenvolvimento de Sistemas, apresentado à Universidade Paulista – UNIP.")
        escrever(formatar(self.doc.add_paragraph(), entrelinhas=1.0, esquerda_cm=8.0), natureza)
        formatar(self.doc.add_paragraph(), entrelinhas=1.0)
        escrever(formatar(self.doc.add_paragraph(), WD_ALIGN_PARAGRAPH.LEFT, entrelinhas=1.0, esquerda_cm=8.0),
                 f"{d.get('orientador_rotulo', 'Orientador(a)')}: {d['orientador']}")
        self.linhas_em_branco(max(1, 8 - len(d["alunos"])))
        escrever(formatar(self.doc.add_paragraph(), WD_ALIGN_PARAGRAPH.CENTER), d["cidade"].upper())
        escrever(formatar(self.doc.add_paragraph(), WD_ALIGN_PARAGRAPH.CENTER), d["ano"])

    def resumo(self, titulo, texto, rotulo, palavras):
        self.titulo_pre_textual(titulo)
        escrever(formatar(self.doc.add_paragraph(), entrelinhas=1.0, depois=18), texto)
        par = formatar(self.doc.add_paragraph(), WD_ALIGN_PARAGRAPH.LEFT, entrelinhas=1.0)
        escrever(par, f"**{rotulo}:** {palavras}")

    def lista(self, titulo, rotulo):
        self.titulo_pre_textual(titulo)
        campo(self.doc.add_paragraph(), f'TOC \\h \\z \\c "{rotulo}"', "Lista atualizada pelo Word ao exportar o PDF.")

    def sumario(self):
        self.titulo_pre_textual("SUMÁRIO")
        campo(self.doc.add_paragraph(), 'TOC \\o "1-2" \\h \\z \\u', "Sumário atualizado pelo Word ao exportar o PDF.")

    def iniciar_parte_textual(self):
        """Nova seção do Word: a partir da Introdução o número da página aparece no canto superior direito."""
        secao = self.doc.add_section(WD_SECTION.NEW_PAGE)
        self._configurar_pagina(secao)
        # A nova seção herda o "começar em 0" da primeira: remove, para a contagem continuar
        for pg in secao._sectPr.findall(qn("w:pgNumType")):
            secao._sectPr.remove(pg)
        secao.header.is_linked_to_previous = False
        par = secao.header.paragraphs[0]
        formatar(par, WD_ALIGN_PARAGRAPH.RIGHT, entrelinhas=1.0)
        campo(par, "PAGE", "1", 10)

    def referencias(self, itens):
        par = self.secao(1, "REFERÊNCIAS")
        par.alignment = WD_ALIGN_PARAGRAPH.CENTER
        for item in itens:
            # ABNT: alinhadas à esquerda, entrelinhas simples e separadas por uma linha em branco
            escrever(formatar(self.doc.add_paragraph(), WD_ALIGN_PARAGRAPH.LEFT, entrelinhas=1.0, depois=12), item)

    def apendice(self, titulo):
        # Título sem estilo de seção: pelo manual da UNIP, apêndices não entram no sumário
        par = self.doc.add_paragraph()
        formatar(par, WD_ALIGN_PARAGRAPH.CENTER, depois=12)
        par.paragraph_format.page_break_before = True
        manter_com_proximo(par)
        escrever(par, titulo.upper(), negrito=True)

    # ---- interpretação dos blocos de conteúdo
    def renderizar(self, blocos):
        for bloco in blocos:
            tipo, args = bloco[0], bloco[1:]
            if tipo == "h1":
                self.secao(1, args[0])
            elif tipo == "h2":
                self.secao(2, args[0])
            elif tipo == "p":
                self.paragrafo(args[0])
            elif tipo == "alineas":
                self.alineas(args[0])
            elif tipo == "fig":
                self.figura(*args)
            elif tipo == "quadro":
                self.quadro(*args)
            elif tipo == "codigo":
                self.codigo(*args)
            else:
                raise ValueError(f"Bloco desconhecido: {tipo}")


def main():
    dados = json.loads((PASTA / "dados_aluno.json").read_text(encoding="utf-8"))
    rel = Relatorio(dados)

    # Pré-textuais
    rel.capa()
    rel.folha_de_rosto()
    rel.resumo("RESUMO", conteudo.RESUMO, "Palavras-chave", conteudo.PALAVRAS_CHAVE)
    rel.resumo("ABSTRACT", conteudo.ABSTRACT, "Keywords", conteudo.KEYWORDS)
    rel.lista("LISTA DE FIGURAS", "Figura")
    rel.lista("LISTA DE QUADROS", "Quadro")
    rel.sumario()

    # Textuais (Introdução, Desenvolvimento e Conclusão)
    rel.iniciar_parte_textual()
    primeiro = rel.secao(1, conteudo.INTRODUCAO[0][1])
    primeiro.paragraph_format.page_break_before = False  # a quebra de seção já abriu a página
    rel.renderizar(conteudo.INTRODUCAO[1:])
    rel.renderizar(conteudo.desenvolvimento(dados))
    rel.renderizar(conteudo.CONCLUSAO)

    # Pós-textuais
    rel.referencias(conteudo.REFERENCIAS)
    rel.apendice("APÊNDICE A – SCRIPT COMPLETO DO BANCO DE DADOS")
    rel.paragrafo("Script executado pela aplicação na inicialização (arquivo PataFacil/DAL/ScriptBancoDeDados.sql). "
                  "Contém as tabelas, as triggers, as procedures, as views e os índices utilizados pelas APIs, "
                  "pela aplicação web e pelo aplicativo mobile.")
    rel.codigo((RAIZ / "PataFacil" / "DAL" / "ScriptBancoDeDados.sql").read_text(encoding="utf-8"), tamanho=7.5, manter_junto=False)
    rel.apendice("APÊNDICE B – TELAS COMPLEMENTARES DA APLICAÇÃO WEB")
    rel.renderizar(conteudo.APENDICE_TELAS)

    rel.doc.save(SAIDA_DOCX)
    print("Word gerado:", SAIDA_DOCX)

    if "--sem-pdf" in sys.argv:
        return
    # PowerShell 7 (pwsh) se houver; senão, o Windows PowerShell que vem com o sistema
    shell = shutil.which("pwsh") or "powershell"
    resultado = subprocess.run(
        [shell, "-NoProfile", "-ExecutionPolicy", "Bypass", "-File", str(PASTA / "converter_pdf.ps1"),
         "-Docx", str(SAIDA_DOCX), "-Pdf", str(SAIDA_PDF)],
        capture_output=True, text=True)
    print(resultado.stdout.strip() or resultado.stderr.strip())
    if resultado.returncode == 0:
        print("PDF gerado:", SAIDA_PDF)
    else:
        print("Não foi possível exportar o PDF automaticamente: abra o .docx no Word, pressione Ctrl+T e F9 "
              "para atualizar o sumário e use Arquivo > Exportar > PDF.")


if __name__ == "__main__":
    main()
