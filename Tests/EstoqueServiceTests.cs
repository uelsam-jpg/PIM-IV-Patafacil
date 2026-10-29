using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using Xunit;
using PataFacil.Models;
using PataFacil.DAL;
using PataFacil.BLL;

namespace PataFacil.Tests
{
    public class EstoqueServiceTests
    {
        private readonly Mock<IRepositorio<Produto>> _produtoRepoMock = new Mock<IRepositorio<Produto>>();
        private readonly Mock<IRepositorio<Estoque>> _estoqueRepoMock = new Mock<IRepositorio<Estoque>>();
        private readonly Mock<IRepositorio<EntradaEstoque>> _entradaRepoMock = new Mock<IRepositorio<EntradaEstoque>>();
        private readonly EstoqueService _estoqueService;

        // "Banco de dados falso": ração com 10 unidades
        private readonly Produto _racao = new Produto { Id = 1, Nome = "Ração Golden 15kg", PrecoCusto = 100m, PrecoVenda = 150m, Ativo = true };
        private readonly List<Estoque> _estoques = new List<Estoque>();
        private readonly List<Produto> _produtos = new List<Produto>();

        public EstoqueServiceTests()
        {
            _produtos.Add(_racao);
            _estoques.Add(new Estoque { Id = 1, IdProduto = 1, QuantidadeAtual = 10, PontoReposicao = 5 });

            _produtoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(_racao);
            _produtoRepoMock
                .Setup(r => r.BuscarAsync(It.IsAny<Expression<Func<Produto, bool>>>()))
                .ReturnsAsync((Expression<Func<Produto, bool>> filtro) => _produtos.FindAll(new Predicate<Produto>(filtro.Compile())));
            _produtoRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<Produto>())).ReturnsAsync(2);
            _estoqueRepoMock
                .Setup(r => r.BuscarAsync(It.IsAny<Expression<Func<Estoque, bool>>>()))
                .ReturnsAsync((Expression<Func<Estoque, bool>> filtro) => _estoques.FindAll(new Predicate<Estoque>(filtro.Compile())));
            _entradaRepoMock.Setup(r => r.AdicionarAsync(It.IsAny<EntradaEstoque>())).ReturnsAsync(77);

            _estoqueService = new EstoqueService(_produtoRepoMock.Object, _estoqueRepoMock.Object, _entradaRepoMock.Object);
        }

        private static EntradaMercadoriaDTO EntradaValida(int? idProduto = 1) => new EntradaMercadoriaDTO
        {
            IdProduto = idProduto, Quantidade = 20, NumeroNota = "NF-1234", CustoUnitario = 110m, PrecoVenda = 200m,
            Validade = DateTime.Today.AddMonths(6)
        };

        [Fact]
        public void T24_PrecoPorMargem_45PorCento_NaoEMarkup()
        {
            // Custo R$ 100 com margem de 45%: preço R$ 181,82 (lucro de R$ 81,82 = 45% do preço).
            // Um markup de 45% daria R$ 145,00.
            Assert.Equal(181.82m, EstoqueService.PrecoPorMargem(100m, EstoqueService.MargemSugerida));
            Assert.NotEqual(145m, EstoqueService.PrecoPorMargem(100m, 0.45m));
        }

        [Fact]
        public async Task T25_RegistrarEntrada_ProdutoExistente_SomaEstoqueEAtualizaCustoEPreco()
        {
            var (idEntrada, idProduto, saldo) = await _estoqueService.RegistrarEntradaAsync(EntradaValida(), idFuncionario: 2);

            Assert.Equal(77, idEntrada);
            Assert.Equal(1, idProduto);
            Assert.Equal(30, saldo); // 10 que havia + 20 recebidos
            Assert.Equal(110m, _racao.PrecoCusto);
            Assert.Equal(200m, _racao.PrecoVenda);
            _entradaRepoMock.Verify(r => r.AdicionarAsync(It.Is<EntradaEstoque>(e =>
                e.NumeroNota == "NF-1234" && e.Quantidade == 20 && e.IdFuncionario == 2 && e.Validade == DateTime.Today.AddMonths(6))), Times.Once);
        }

        [Fact]
        public async Task T26_RegistrarEntrada_ProdutoNovo_CadastraProdutoEEstoque()
        {
            var dto = EntradaValida(idProduto: null);
            dto.NovoProduto = new NovoProdutoEntradaDTO { Nome = "Coleira Antipulgas", Categoria = CategoriaProdutoEnum.Medicamento, PontoReposicao = 3 };

            var (_, idProduto, saldo) = await _estoqueService.RegistrarEntradaAsync(dto, idFuncionario: 2);

            Assert.Equal(2, idProduto);
            Assert.Equal(20, saldo);
            _produtoRepoMock.Verify(r => r.AdicionarAsync(It.Is<Produto>(p => p.Nome == "Coleira Antipulgas")), Times.Once);
            _estoqueRepoMock.Verify(r => r.AdicionarAsync(It.Is<Estoque>(e => e.PontoReposicao == 3)), Times.Once);
        }

        [Fact]
        public async Task T27_RegistrarEntrada_ProdutoVencidoOuPrecoAbaixoDoCusto_LancaExcecao()
        {
            var vencido = EntradaValida();
            vencido.Validade = DateTime.Today.AddDays(-1);
            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => _estoqueService.RegistrarEntradaAsync(vencido, 2));
            Assert.Contains("vencido", excecao.Message);

            var prejuizo = EntradaValida();
            prejuizo.PrecoVenda = 90m; // abaixo do custo de R$ 110
            await Assert.ThrowsAsync<InvalidOperationException>(() => _estoqueService.RegistrarEntradaAsync(prejuizo, 2));

            var semNota = EntradaValida();
            semNota.NumeroNota = "  ";
            await Assert.ThrowsAsync<InvalidOperationException>(() => _estoqueService.RegistrarEntradaAsync(semNota, 2));

            _entradaRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<EntradaEstoque>()), Times.Never);
        }

        [Fact]
        public async Task T28_RegistrarEntrada_NovoProdutoComNomeRepetido_LancaExcecao()
        {
            var dto = EntradaValida(idProduto: null);
            dto.NovoProduto = new NovoProdutoEntradaDTO { Nome = "ração golden 15kg" };

            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => _estoqueService.RegistrarEntradaAsync(dto, 2));
            Assert.Contains("Já existe", excecao.Message);
        }
    }
}
