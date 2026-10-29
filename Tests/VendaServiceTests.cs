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
    public class VendaServiceTests
    {
        private readonly Mock<IRepositorio<Venda>> _vendaRepoMock = new Mock<IRepositorio<Venda>>();
        private readonly Mock<IRepositorio<ItemVenda>> _itemVendaRepoMock = new Mock<IRepositorio<ItemVenda>>();
        private readonly Mock<IRepositorio<Estoque>> _estoqueRepoMock = new Mock<IRepositorio<Estoque>>();
        private readonly Mock<IRepositorio<Produto>> _produtoRepoMock = new Mock<IRepositorio<Produto>>();
        private readonly VendaService _vendaService;

        public VendaServiceTests()
        {
            // Produto 1: Ração a R$ 150,00 — 10 em estoque, ponto de reposição 5
            _produtoRepoMock.Setup(repo => repo.ObterPorIdAsync(1)).ReturnsAsync(new Produto { Id = 1, Nome = "Ração", PrecoVenda = 150m });
            var estoques = new List<Estoque> { new Estoque { IdProduto = 1, QuantidadeAtual = 10, PontoReposicao = 5 } };
            _estoqueRepoMock
                .Setup(repo => repo.BuscarAsync(It.IsAny<Expression<Func<Estoque, bool>>>()))
                .ReturnsAsync((Expression<Func<Estoque, bool>> filtro) => estoques.FindAll(new Predicate<Estoque>(filtro.Compile())));
            _vendaRepoMock.Setup(repo => repo.AdicionarAsync(It.IsAny<Venda>())).ReturnsAsync(50);

            _vendaService = new VendaService(_vendaRepoMock.Object, _itemVendaRepoMock.Object, _estoqueRepoMock.Object, _produtoRepoMock.Object);
        }

        [Fact]
        public async Task T17_RegistrarVenda_ComEstoque_CalculaTotalESubtotal()
        {
            // Arrange
            var itens = new List<ItemVenda> { new ItemVenda { IdProduto = 1, Quantidade = 2 } };

            // Act
            var (vendaId, total, alertas) = await _vendaService.RegistrarVendaAsync(new Venda(), itens);

            // Assert
            Assert.Equal(50, vendaId);
            Assert.Equal(300m, total);
            Assert.Empty(alertas); // Sobram 8, acima do ponto de reposição
            _itemVendaRepoMock.Verify(repo => repo.AdicionarAsync(It.Is<ItemVenda>(i => i.IdVenda == 50 && i.Subtotal == 300m && i.PrecoUnitario == 150m)), Times.Once);
        }

        [Fact]
        public async Task T18_RegistrarVenda_AtingindoPontoDeReposicao_GeraAlerta()
        {
            // Arrange: vender 5 de 10 deixa o saldo em 5, igual ao ponto de reposição (RN15)
            var itens = new List<ItemVenda> { new ItemVenda { IdProduto = 1, Quantidade = 5 } };

            // Act
            var (_, _, alertas) = await _vendaService.RegistrarVendaAsync(new Venda(), itens);

            // Assert
            var alerta = Assert.Single(alertas);
            Assert.Contains("ponto de reposição", alerta);
        }

        [Fact]
        public async Task T19_RegistrarVenda_SemEstoqueSuficiente_NaoGravaNada()
        {
            // Arrange: o mesmo produto em duas linhas do carrinho soma 11, acima dos 10 disponíveis
            var itens = new List<ItemVenda>
            {
                new ItemVenda { IdProduto = 1, Quantidade = 6 },
                new ItemVenda { IdProduto = 1, Quantidade = 5 }
            };

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => _vendaService.RegistrarVendaAsync(new Venda(), itens));
            Assert.Contains("Estoque insuficiente", excecao.Message);
            _vendaRepoMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Venda>()), Times.Never);
            _itemVendaRepoMock.Verify(repo => repo.AdicionarAsync(It.IsAny<ItemVenda>()), Times.Never);
        }
    }
}
