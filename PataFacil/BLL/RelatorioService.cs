using System;
using System.Linq;
using System.Threading.Tasks;
using PataFacil.Models;
using PataFacil.DAL;

namespace PataFacil.BLL
{
    // Classe auxiliar (DTO) para formatar a resposta do relatório
    public class RelatorioFaturamentoDTO
    {
        public int Mes { get; set; }
        public int Ano { get; set; }
        public decimal TotalVendasProdutos { get; set; }
        public decimal TotalServicos { get; set; }
        public decimal FaturamentoTotal => TotalVendasProdutos + TotalServicos;
    }

    public class RelatorioService
    {
        private readonly IRepositorio<Venda> _vendaRepo;
        private readonly IRepositorio<Agendamento> _agendamentoRepo;
        private readonly IRepositorio<Servico> _servicoRepo;
        private readonly IRepositorio<ItemVenda> _itemVendaRepo;

        public RelatorioService(
            IRepositorio<Venda> vendaRepo,
            IRepositorio<Agendamento> agendamentoRepo,
            IRepositorio<Servico> servicoRepo,
            IRepositorio<ItemVenda> itemVendaRepo)
        {
            _vendaRepo = vendaRepo;
            _agendamentoRepo = agendamentoRepo;
            _servicoRepo = servicoRepo;
            _itemVendaRepo = itemVendaRepo;
        }

        public async Task<RelatorioFaturamentoDTO> GerarRelatorioMensalAsync(int ano, int mes)
        {
            // 1. Calcula o faturamento de produtos vendidos no mês
            var vendas = await _vendaRepo.ObterTodosAsync();
            var vendasDoMes = vendas.Where(v => v.DataVenda.Year == ano && v.DataVenda.Month == mes);
            decimal totalProdutos = vendasDoMes.Sum(v => v.Total);

            // 2. Calcula o faturamento de serviços concluídos no mês
            var agendamentos = await _agendamentoRepo.ObterTodosAsync();
            var servicos = await _servicoRepo.ObterTodosAsync();

            var agendamentosConcluidos = agendamentos.Where(a => 
                a.Status == StatusAgendamento.CONCLUIDO && 
                a.DataHoraInicio.Year == ano && 
                a.DataHoraInicio.Month == mes);

            // Vale o que foi recebido no balcão; sem baixa registrada, o preço de tabela do serviço
            decimal totalServicos = agendamentosConcluidos.Sum(a => a.ValorPago ?? servicos.FirstOrDefault(s => s.Id == a.IdServico)?.Preco ?? 0);

            return new RelatorioFaturamentoDTO
            {
                Ano = ano,
                Mes = mes,
                TotalVendasProdutos = totalProdutos,
                TotalServicos = totalServicos
            };
        }

        public async Task<object> ObterKpisDashboardAsync()
        {
            var hoje = DateTime.Today;
            
            var vendas = await _vendaRepo.ObterTodosAsync();
            var agendamentos = await _agendamentoRepo.ObterTodosAsync();
            var itensVenda = await _itemVendaRepo.ObterTodosAsync();

            // 1. Clientes atendidos no dia (Baseado em agendamentos concluídos hoje)
            int clientesAtendidos = agendamentos
                .Count(a => a.DataHoraInicio.Date == hoje && a.Status == StatusAgendamento.CONCLUIDO);

            // 2. Forma de pagamento mais utilizada (vendas de produtos + serviços pagos no balcão)
            string formaPagamentoMaisUsada = vendas
                .Select(v => v.FormaPagamento)
                .Concat(agendamentos.Where(a => a.FormaPagamento.HasValue).Select(a => a.FormaPagamento!.Value))
                .GroupBy(forma => forma)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key.ToString())
                .FirstOrDefault() ?? "Nenhuma";

            // 3. Total de produtos vendidos no mês
            var vendasDoMesIds = vendas
                .Where(v => v.DataVenda.Year == hoje.Year && v.DataVenda.Month == hoje.Month)
                .Select(v => v.Id)
                .ToHashSet();

            int produtosVendidosNoMes = itensVenda
                .Where(i => vendasDoMesIds.Contains(i.IdVenda))
                .Sum(i => i.Quantidade);

            return new 
            {
                ClientesAtendidosHoje = clientesAtendidos,
                FormaPagamentoFavorita = formaPagamentoMaisUsada,
                ProdutosVendidosMes = produtosVendidosNoMes
            };
        }
    }
}