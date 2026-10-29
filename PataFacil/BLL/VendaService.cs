using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PataFacil.Models;
using PataFacil.DAL;

namespace PataFacil.BLL
{
    public class VendaService
    {
        private readonly IRepositorio<Venda> _vendaRepo;
        private readonly IRepositorio<ItemVenda> _itemVendaRepo;
        private readonly IRepositorio<Estoque> _estoqueRepo;
        private readonly IRepositorio<Produto> _produtoRepo;

        public VendaService(
            IRepositorio<Venda> vendaRepo,
            IRepositorio<ItemVenda> itemVendaRepo,
            IRepositorio<Estoque> estoqueRepo,
            IRepositorio<Produto> produtoRepo)
        {
            _vendaRepo = vendaRepo;
            _itemVendaRepo = itemVendaRepo;
            _estoqueRepo = estoqueRepo;
            _produtoRepo = produtoRepo;
        }

        public async Task<(int VendaId, decimal Total, List<string> Alertas)> RegistrarVendaAsync(Venda venda, List<ItemVenda> itens)
        {
            if (itens == null || itens.Count == 0)
                throw new InvalidOperationException("A venda deve conter itens.");

            // O mesmo produto pode vir em mais de uma linha do carrinho: soma antes de validar o estoque
            var itensAgrupados = itens
                .GroupBy(i => i.IdProduto)
                .Select(g => new ItemVenda { IdProduto = g.Key, Quantidade = g.Sum(i => i.Quantidade) })
                .ToList();

            var alertas = new List<string>();
            decimal totalVenda = 0;
            var idsProdutos = itensAgrupados.Select(i => i.IdProduto).ToList();
            var estoques = (await _estoqueRepo.BuscarAsync(e => idsProdutos.Contains(e.IdProduto))).ToList();

            // 1. Valida os itens, calcula os subtotais e confere o saldo antes de gravar qualquer coisa
            foreach (var item in itensAgrupados)
            {
                if (item.Quantidade <= 0)
                    throw new InvalidOperationException("A quantidade de cada item deve ser maior que zero.");

                var produto = await _produtoRepo.ObterPorIdAsync(item.IdProduto);
                if (produto == null || !produto.Ativo)
                    throw new InvalidOperationException($"Produto ID {item.IdProduto} não encontrado.");

                var estoqueProduto = estoques.FirstOrDefault(e => e.IdProduto == item.IdProduto);
                if (estoqueProduto == null || estoqueProduto.QuantidadeAtual < item.Quantidade)
                    throw new InvalidOperationException($"Estoque insuficiente para o produto '{produto.Nome}'. Disponível: {estoqueProduto?.QuantidadeAtual ?? 0}.");

                item.PrecoUnitario = produto.PrecoVenda;
                item.Subtotal = item.Quantidade * item.PrecoUnitario;
                totalVenda += item.Subtotal;

                // RN15: avisa quando o saldo após a venda atinge o ponto de reposição
                var saldoAposVenda = estoqueProduto.QuantidadeAtual - item.Quantidade;
                if (saldoAposVenda <= estoqueProduto.PontoReposicao)
                    alertas.Add($"ALERTA DE ESTOQUE: O produto '{produto.Nome}' atingiu o ponto de reposição. Quantidade atual: {saldoAposVenda}.");
            }

            // 2. Salva a Venda
            venda.Total = totalVenda;
            venda.DataVenda = DateTime.Now;
            var vendaId = await _vendaRepo.AdicionarAsync(venda);

            // 3. Salva os Itens da Venda. A baixa do estoque (RN15) é feita no banco pela trigger
            //    trg_item_venda_baixa_estoque a cada item inserido — por isso não é repetida aqui.
            foreach (var item in itensAgrupados)
            {
                item.IdVenda = vendaId;
                await _itemVendaRepo.AdicionarAsync(item);
            }

            return (vendaId, totalVenda, alertas);
        }
    }
}
