using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PataFacil.Models;
using PataFacil.DAL;

namespace PataFacil.BLL
{
    // Indicadores do mês para os gráficos da Visão Geral: andamento, comparação com o mês anterior,
    // melhores clientes, serviços e produtos mais vendidos e sugestões de promoção para a semana.
    public class PainelService
    {
        private static readonly CultureInfo PtBr = new CultureInfo("pt-BR");
        private static readonly DayOfWeek[] DiasUteis =
            { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday };

        private readonly IRepositorio<Agendamento> _agendamentoRepo;
        private readonly IRepositorio<Servico> _servicoRepo;
        private readonly IRepositorio<Venda> _vendaRepo;
        private readonly IRepositorio<ItemVenda> _itemVendaRepo;
        private readonly IRepositorio<Produto> _produtoRepo;
        private readonly IRepositorio<Estoque> _estoqueRepo;
        private readonly IRepositorio<EntradaEstoque> _entradaRepo;
        private readonly IRepositorio<Tutor> _tutorRepo;
        private readonly IRepositorio<Pet> _petRepo;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public PainelService(
            IRepositorio<Agendamento> agendamentoRepo, IRepositorio<Servico> servicoRepo,
            IRepositorio<Venda> vendaRepo, IRepositorio<ItemVenda> itemVendaRepo,
            IRepositorio<Produto> produtoRepo, IRepositorio<Estoque> estoqueRepo, IRepositorio<EntradaEstoque> entradaRepo,
            IRepositorio<Tutor> tutorRepo, IRepositorio<Pet> petRepo,
            IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _agendamentoRepo = agendamentoRepo;
            _servicoRepo = servicoRepo;
            _vendaRepo = vendaRepo;
            _itemVendaRepo = itemVendaRepo;
            _produtoRepo = produtoRepo;
            _estoqueRepo = estoqueRepo;
            _entradaRepo = entradaRepo;
            _tutorRepo = tutorRepo;
            _petRepo = petRepo;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // "outubro de 2026" -> "Outubro de 2026" (ToTitleCase deixaria "Outubro De 2026")
        private static string Capitalizar(string texto) => texto.Length == 0 ? texto : char.ToUpper(texto[0], PtBr) + texto[1..];

        // Um atendimento conta no mês em que foi pago (ou na data marcada, se concluído sem baixa no balcão)
        private static DateTime DataDoAtendimento(Agendamento a) => (a.DataPagamento ?? a.DataHoraInicio).Date;

        public async Task<object> ObterPainelAsync(int ano, int mes)
        {
            var inicio = new DateTime(ano, mes, 1);
            var fim = inicio.AddMonths(1);
            var inicioAnterior = inicio.AddMonths(-1);
            var hoje = DateTime.Today;

            var agendamentos = (await _agendamentoRepo.ObterTodosAsync()).ToList();
            var servicos = (await _servicoRepo.ObterTodosAsync()).ToDictionary(s => s.Id);
            var vendas = (await _vendaRepo.ObterTodosAsync()).ToList();
            var itens = (await _itemVendaRepo.ObterTodosAsync()).ToList();
            var produtos = (await _produtoRepo.ObterTodosAsync()).ToDictionary(p => p.Id);
            var estoques = (await _estoqueRepo.ObterTodosAsync()).ToList();
            var entradas = (await _entradaRepo.ObterTodosAsync()).ToList();
            var tutores = (await _tutorRepo.ObterTodosAsync()).ToDictionary(t => t.Id);
            var pets = (await _petRepo.ObterTodosAsync()).ToDictionary(p => p.Id);

            decimal ValorServico(Agendamento a) => a.ValorPago ?? (servicos.TryGetValue(a.IdServico, out var s) ? s.Preco : 0);
            int? TutorDoAgendamento(Agendamento a) => pets.TryGetValue(a.IdPet, out var p) ? p.TutorId : null;

            var concluidos = agendamentos.Where(a => a.Status == StatusAgendamento.CONCLUIDO).ToList();
            List<Agendamento> AtendimentosEntre(DateTime de, DateTime ate) =>
                concluidos.Where(a => DataDoAtendimento(a) >= de && DataDoAtendimento(a) < ate).ToList();
            List<Venda> VendasEntre(DateTime de, DateTime ate) =>
                vendas.Where(v => v.DataVenda >= de && v.DataVenda < ate).ToList();

            var atendMes = AtendimentosEntre(inicio, fim);
            var atendAnt = AtendimentosEntre(inicioAnterior, inicio);
            var vendasMes = VendasEntre(inicio, fim);
            var vendasAnt = VendasEntre(inicioAnterior, inicio);

            object Resumo(List<Agendamento> atend, List<Venda> vend, DateTime de, DateTime ate)
            {
                var receitaServicos = atend.Sum(ValorServico);
                var receitaProdutos = vend.Sum(v => v.Total);
                var transacoes = atend.Count + vend.Count;
                return new
                {
                    receitaServicos,
                    receitaProdutos,
                    receitaTotal = receitaServicos + receitaProdutos,
                    atendimentos = atend.Count,
                    vendas = vend.Count,
                    ticketMedio = transacoes > 0 ? Math.Round((receitaServicos + receitaProdutos) / transacoes, 2) : 0,
                    novosClientes = tutores.Values.Count(t => t.DataCadastro >= de && t.DataCadastro < ate),
                    cancelamentos = agendamentos.Count(a => a.Status == StatusAgendamento.CANCELADO && a.DataHoraInicio >= de && a.DataHoraInicio < ate)
                };
            }

            // Faturamento acumulado dia a dia: mostra o andamento do mês contra o mês anterior
            decimal[] Acumulado(List<Agendamento> atend, List<Venda> vend, DateTime primeiroDia, int dias, DateTime? limite)
            {
                var serie = new decimal[dias];
                decimal soma = 0;
                for (int d = 0; d < dias; d++)
                {
                    var dia = primeiroDia.AddDays(d);
                    soma += atend.Where(a => DataDoAtendimento(a) == dia).Sum(ValorServico) + vend.Where(v => v.DataVenda.Date == dia).Sum(v => v.Total);
                    serie[d] = soma;
                }
                if (limite.HasValue)
                    for (int d = 0; d < dias; d++)
                        if (primeiroDia.AddDays(d) > limite.Value) serie[d] = -1; // dias que ainda não chegaram
                return serie;
            }
            var diasMes = DateTime.DaysInMonth(ano, mes);
            var diasAnt = DateTime.DaysInMonth(inicioAnterior.Year, inicioAnterior.Month);
            var mesCorrente = hoje >= inicio && hoje < fim;

            // Clientes que mais gastaram no mês (serviços pagos + compras de produtos)
            var gastos = new Dictionary<int, (decimal Total, int Visitas, decimal Servicos, decimal Produtos)>();
            void Somar(int idTutor, decimal servico, decimal produto)
            {
                gastos.TryGetValue(idTutor, out var g);
                gastos[idTutor] = (g.Total + servico + produto, g.Visitas + 1, g.Servicos + servico, g.Produtos + produto);
            }
            foreach (var a in atendMes) { var t = TutorDoAgendamento(a); if (t.HasValue) Somar(t.Value, ValorServico(a), 0); }
            foreach (var v in vendasMes.Where(v => v.IdTutor.HasValue)) Somar(v.IdTutor!.Value, 0, v.Total);
            var topClientes = gastos.OrderByDescending(g => g.Value.Total).Take(5).Select(g => new
            {
                nome = tutores.TryGetValue(g.Key, out var t) ? t.Nome : "Cliente",
                total = g.Value.Total,
                servicos = g.Value.Servicos,
                produtos = g.Value.Produtos,
                visitas = g.Value.Visitas
            }).ToList();

            // Serviços mais vendidos (todos os serviços ativos aparecem, mesmo com zero)
            var rankingServicos = servicos.Values.Where(s => s.Ativo).Select(s => new
            {
                nome = s.Nome,
                quantidade = atendMes.Count(a => a.IdServico == s.Id),
                receita = atendMes.Where(a => a.IdServico == s.Id).Sum(ValorServico),
                quantidadeAnterior = atendAnt.Count(a => a.IdServico == s.Id)
            }).OrderByDescending(s => s.quantidade).ThenByDescending(s => s.receita).ToList();

            var idsVendasMes = vendasMes.Select(v => v.Id).ToHashSet();
            var topProdutos = itens.Where(i => idsVendasMes.Contains(i.IdVenda))
                .GroupBy(i => i.IdProduto)
                .Select(g => new { nome = produtos.TryGetValue(g.Key, out var p) ? p.Nome : "Produto", quantidade = g.Sum(i => i.Quantidade), receita = g.Sum(i => i.Subtotal) })
                .OrderByDescending(p => p.quantidade).Take(5).ToList();

            // Movimento médio por dia da semana nas últimas 8 semanas (agendamentos não cancelados)
            var oitoSemanas = hoje.AddDays(-56);
            var movimento = DiasUteis.Select(d => new
            {
                dia = PtBr.DateTimeFormat.GetDayName(d),
                media = Math.Round(agendamentos.Count(a => a.Status != StatusAgendamento.CANCELADO && a.DataHoraInicio >= oitoSemanas && a.DataHoraInicio < hoje.AddDays(1) && a.DataHoraInicio.DayOfWeek == d) / 8.0, 2)
            }).ToList();

            var previsao = await PreverProximaSemanaAsync(hoje);

            var promocoes = SugerirPromocoes(hoje, movimento.Select(m => (m.dia, m.media)).ToList(), previsao,
                rankingServicos.Select(s => (s.nome, s.quantidade)).ToList(),
                produtos, estoques, entradas, itens, vendas, agendamentos, pets, tutores);

            return new
            {
                ano,
                mes,
                nomeMes = Capitalizar(inicio.ToString("MMMM 'de' yyyy", PtBr)),
                nomeMesAnterior = Capitalizar(inicioAnterior.ToString("MMMM", PtBr)),
                mesCorrente,
                atual = Resumo(atendMes, vendasMes, inicio, fim),
                anterior = Resumo(atendAnt, vendasAnt, inicioAnterior, inicio),
                diario = new
                {
                    atual = Acumulado(atendMes, vendasMes, inicio, diasMes, mesCorrente ? hoje : null),
                    anterior = Acumulado(atendAnt, vendasAnt, inicioAnterior, diasAnt, null)
                },
                topClientes,
                servicos = rankingServicos,
                topProdutos,
                movimento,
                previsao,
                promocoes
            };
        }

        // Previsão da IA (microsserviço Python) para os próximos dias de expediente.
        // Se a IA estiver fora do ar, devolve lista vazia e as sugestões usam só o histórico.
        private async Task<List<object>> PreverProximaSemanaAsync(DateTime hoje)
        {
            var resultado = new List<object>();
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(3);
                var url = (_configuration["MlApiUrl"] ?? "http://127.0.0.1:8000") + "/api/ml/prever-demanda";
                for (var dia = hoje.AddDays(1); resultado.Count < 6; dia = dia.AddDays(1))
                {
                    if (dia.DayOfWeek == DayOfWeek.Sunday) continue;
                    var resposta = await client.PostAsJsonAsync(url, new { dia_semana = (int)dia.DayOfWeek, mes = dia.Month, feriado = 0, promocao = 0, temperatura = 25.0 });
                    if (!resposta.IsSuccessStatusCode) return new List<object>();
                    var json = await resposta.Content.ReadFromJsonAsync<JsonElement>();
                    resultado.Add(new
                    {
                        data = dia,
                        dia = PtBr.DateTimeFormat.GetDayName(dia.DayOfWeek),
                        previsto = json.GetProperty("previsao_agendamentos").GetInt32()
                    });
                }
            }
            catch
            {
                return new List<object>();
            }
            return resultado;
        }

        private static List<object> SugerirPromocoes(
            DateTime hoje, List<(string Dia, double Media)> movimento, List<object> previsao,
            List<(string Nome, int Quantidade)> servicos,
            Dictionary<int, Produto> produtos, List<Estoque> estoques, List<EntradaEstoque> entradas,
            List<ItemVenda> itens, List<Venda> vendas, List<Agendamento> agendamentos,
            Dictionary<int, Pet> pets, Dictionary<int, Tutor> tutores)
        {
            var sugestoes = new List<object>();
            var servicoPrincipal = servicos.FirstOrDefault(s => s.Nome.Contains("Banho", StringComparison.OrdinalIgnoreCase)).Nome
                                   ?? servicos.FirstOrDefault().Nome ?? "Banho e Tosa";

            // 1. Dia mais fraco da próxima semana: pela previsão da IA ou, sem ela, pela média das últimas semanas
            string? diaFraco = null;
            string motivoDia = "";
            if (previsao.Count > 0)
            {
                var menor = previsao.Select(p => JsonSerializer.SerializeToElement(p))
                    .OrderBy(p => p.GetProperty("previsto").GetInt32()).First();
                diaFraco = menor.GetProperty("dia").GetString();
                motivoDia = $"A IA prevê {menor.GetProperty("previsto").GetInt32()} atendimentos na {diaFraco}, o menor movimento da próxima semana.";
            }
            else if (movimento.Any(m => m.Media > 0))
            {
                var menor = movimento.OrderBy(m => m.Media).First();
                diaFraco = menor.Dia;
                motivoDia = $"Nas últimas 8 semanas, a {diaFraco} teve em média {menor.Media.ToString("0.#", PtBr)} atendimentos, o dia mais fraco.";
            }
            if (diaFraco != null)
                sugestoes.Add(new
                {
                    tipo = "dia",
                    icone = "📅",
                    titulo = $"{Capitalizar(diaFraco)} do {servicoPrincipal.Split(' ')[0]}",
                    acao = $"15% de desconto em {servicoPrincipal} na {diaFraco}.",
                    motivo = motivoDia
                });

            // 2. Lotes perto de vencer: liquidar antes de perder a mercadoria
            var vencendo = entradas.Where(e => e.Validade.HasValue && e.Validade.Value.Date >= hoje && (e.Validade.Value.Date - hoje).TotalDays <= 30)
                .GroupBy(e => e.IdProduto)
                .Select(g => new { IdProduto = g.Key, Validade = g.Min(e => e.Validade!.Value) })
                .Where(v => estoques.Any(e => e.IdProduto == v.IdProduto && e.QuantidadeAtual > 0))
                .OrderBy(v => v.Validade).Take(2);
            foreach (var v in vencendo)
            {
                if (!produtos.TryGetValue(v.IdProduto, out var p)) continue;
                var dias = (int)(v.Validade.Date - hoje).TotalDays;
                var desconto = dias <= 15 ? 20 : 10;
                var saldo = estoques.First(e => e.IdProduto == v.IdProduto).QuantidadeAtual;
                sugestoes.Add(new
                {
                    tipo = "validade",
                    icone = "⏳",
                    titulo = $"Queima de estoque: {p.Nome}",
                    acao = $"{desconto}% de desconto: de {p.PrecoVenda.ToString("C", PtBr)} por {(p.PrecoVenda * (100 - desconto) / 100).ToString("C", PtBr)}.",
                    motivo = $"Um lote vence em {dias} {(dias == 1 ? "dia" : "dias")} ({v.Validade:dd/MM}) e ainda há {saldo} unidades."
                });
            }

            // 3. Produto parado: bastante estoque e nenhuma venda nos últimos 30 dias
            var vendidosRecentes = itens.Where(i => vendas.Any(v => v.Id == i.IdVenda && v.DataVenda >= hoje.AddDays(-30)))
                .Select(i => i.IdProduto).ToHashSet();
            var parado = estoques.Where(e => produtos.ContainsKey(e.IdProduto) && produtos[e.IdProduto].Ativo
                                            && !vendidosRecentes.Contains(e.IdProduto) && e.QuantidadeAtual >= Math.Max(10, e.PontoReposicao * 2))
                .OrderByDescending(e => e.QuantidadeAtual).FirstOrDefault();
            if (parado != null)
                sugestoes.Add(new
                {
                    tipo = "parado",
                    icone = "📦",
                    titulo = $"Leve 3, pague 2: {produtos[parado.IdProduto].Nome}",
                    acao = $"Monte um combo ou exponha no balcão com destaque.",
                    motivo = $"São {parado.QuantidadeAtual} unidades em estoque e nenhuma venda nos últimos 30 dias."
                });

            // 4. Clientes sumidos: já vieram, mas não voltam há mais de 60 dias
            var ultimaVisita = new Dictionary<int, DateTime>();
            foreach (var a in agendamentos.Where(a => a.Status == StatusAgendamento.CONCLUIDO))
                if (pets.TryGetValue(a.IdPet, out var pet) && (!ultimaVisita.TryGetValue(pet.TutorId, out var u) || a.DataHoraInicio > u))
                    ultimaVisita[pet.TutorId] = a.DataHoraInicio;
            var agendadosFuturos = agendamentos.Where(a => a.DataHoraInicio >= hoje && a.Status != StatusAgendamento.CANCELADO)
                .Select(a => pets.TryGetValue(a.IdPet, out var p) ? p.TutorId : 0).ToHashSet();
            var sumidos = ultimaVisita.Count(u => u.Value < hoje.AddDays(-60) && !agendadosFuturos.Contains(u.Key));
            if (sumidos > 0)
                sugestoes.Add(new
                {
                    tipo = "retorno",
                    icone = "💌",
                    titulo = "Volte a nos visitar",
                    acao = $"Envie um cupom de 10% para {sumidos} {(sumidos == 1 ? "cliente" : "clientes")} pelo WhatsApp.",
                    motivo = $"{(sumidos == 1 ? "Esse cliente não volta" : "Esses clientes não voltam")} há mais de 60 dias e não {(sumidos == 1 ? "tem" : "têm")} nada agendado."
                });

            // 5. Serviço menos procurado no mês: pacote com o mais vendido
            if (servicos.Count >= 2 && servicos.First().Quantidade > servicos.Last().Quantidade)
            {
                var fraco = servicos.Last();
                var forte = servicos.First();
                sugestoes.Add(new
                {
                    tipo = "combo",
                    icone = "🎁",
                    titulo = $"Combo {forte.Nome} + {fraco.Nome}",
                    acao = $"Quem agendar {forte.Nome} ganha 20% em {fraco.Nome} no mesmo dia.",
                    motivo = $"{fraco.Nome} teve {fraco.Quantidade} {(fraco.Quantidade == 1 ? "atendimento" : "atendimentos")} no mês, contra {forte.Quantidade} de {forte.Nome}."
                });
            }

            return sugestoes.Take(4).ToList();
        }
    }
}
