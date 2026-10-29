using Microsoft.AspNetCore.Mvc;
using PataFacil.Models;
using PataFacil.DAL;
using PataFacil.BLL;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System;

namespace PataFacil.Controllers
{
    [Route("api/[controller]")] // Rota base: /api/estoque
    [ApiController]
    public class EstoqueController : ControllerBase
    {
        private readonly IRepositorio<Estoque> _estoqueRepo;
        private readonly IRepositorio<Produto> _produtoRepo;
        private readonly IRepositorio<EntradaEstoque> _entradaRepo;
        private readonly IRepositorio<Funcionario> _funcionarioRepo;
        private readonly IRepositorio<Fornecedor> _fornecedorRepo;
        private readonly EstoqueService _estoqueService;

        public EstoqueController(IRepositorio<Estoque> estoqueRepo, IRepositorio<Produto> produtoRepo,
            IRepositorio<EntradaEstoque> entradaRepo, IRepositorio<Funcionario> funcionarioRepo,
            IRepositorio<Fornecedor> fornecedorRepo, EstoqueService estoqueService)
        {
            _estoqueRepo = estoqueRepo;
            _produtoRepo = produtoRepo;
            _entradaRepo = entradaRepo;
            _funcionarioRepo = funcionarioRepo;
            _fornecedorRepo = fornecedorRepo;
            _estoqueService = estoqueService;
        }

        [HttpGet("critico")]
        [Authorize(Roles = "Gerente,Atendente")]
        public async Task<IActionResult> GetCritico()
        {
            var estoques = await _estoqueRepo.ObterTodosAsync();
            var produtos = await _produtoRepo.ObterTodosAsync();

            var criticos = estoques.Where(e => e.QuantidadeAtual <= e.PontoReposicao).Select(e => new {
                NomeProduto = produtos.FirstOrDefault(p => p.Id == e.IdProduto)?.Nome,
                QuantidadeAtual = e.QuantidadeAtual,
                PontoReposicao = e.PontoReposicao
            });
            return Ok(criticos);
        }

        [HttpGet("produtos")]
        [Authorize(Roles = "Gerente,Atendente")]
        public async Task<IActionResult> GetProdutos()
        {
            var produtos = await _produtoRepo.ObterTodosAsync();
            var estoques = await _estoqueRepo.ObterTodosAsync();
            var hoje = DateTime.Today;
            var entradas = (await _entradaRepo.ObterTodosAsync()).ToList();
            var validades = entradas.Where(e => e.Validade != null && e.Validade >= hoje)
                .GroupBy(e => e.IdProduto)
                .ToDictionary(g => g.Key, g => g.Min(e => e.Validade));
            // Fornecedor da compra mais recente, para já vir preenchido na próxima entrada
            var ultimosFornecedores = entradas.Where(e => !string.IsNullOrWhiteSpace(e.Fornecedor))
                .GroupBy(e => e.IdProduto)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(e => e.DataEntrada).First().Fornecedor);

            var resultado = produtos.Where(p => p.Ativo).OrderBy(p => p.Nome).Select(p => {
                var estoque = estoques.FirstOrDefault(e => e.IdProduto == p.Id);
                return new {
                    Id = p.Id,
                    Nome = p.Nome,
                    Categoria = p.Categoria,
                    Marca = p.Marca,
                    PrecoCusto = p.PrecoCusto,
                    PrecoVenda = p.PrecoVenda,
                    QuantidadeEstoque = estoque?.QuantidadeAtual ?? 0,
                    PontoReposicao = estoque?.PontoReposicao ?? 0,
                    // Validade mais próxima entre os lotes recebidos que ainda não venceram
                    ProximaValidade = validades.TryGetValue(p.Id, out var validade) ? validade : null,
                    UltimoFornecedor = ultimosFornecedores.GetValueOrDefault(p.Id)
                };
            });
            return Ok(resultado);
        }

        // Fornecedores cadastrados, com quantas entregas cada um já fez (os mais usados primeiro).
        // Nomes antigos que só existem nas entradas também aparecem.
        [HttpGet("fornecedores")]
        [Authorize(Roles = "Gerente,Atendente")]
        public async Task<IActionResult> ListarFornecedores()
        {
            var entregas = (await _entradaRepo.BuscarAsync(e => e.Fornecedor != ""))
                .GroupBy(e => e.Fornecedor.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => (Quantidade: g.Count(), Ultima: (DateTime?)g.Max(e => e.DataEntrada)), StringComparer.OrdinalIgnoreCase);
            var cadastrados = (await _fornecedorRepo.BuscarAsync(f => f.Ativo)).ToList();

            var lista = cadastrados.Select(f => new {
                    f.Id, f.Nome, f.Cnpj, f.Telefone, f.Email,
                    Entregas = entregas.TryGetValue(f.Nome, out var e) ? e.Quantidade : 0,
                    UltimaEntrega = entregas.TryGetValue(f.Nome, out var u) ? u.Ultima : null
                })
                .Concat(entregas.Where(e => !cadastrados.Any(f => string.Equals(f.Nome, e.Key, StringComparison.OrdinalIgnoreCase)))
                    .Select(e => new { Id = 0, Nome = e.Key, Cnpj = "", Telefone = "", Email = "", Entregas = e.Value.Quantidade, UltimaEntrega = e.Value.Ultima }))
                .OrderByDescending(f => f.Entregas).ThenBy(f => f.Nome);
            return Ok(lista);
        }

        [HttpPost("fornecedores")]
        [Authorize(Roles = "Gerente,Atendente")]
        public async Task<IActionResult> CadastrarFornecedor([FromBody] NovoFornecedorDTO dto)
        {
            try
            {
                var f = await _estoqueService.CadastrarFornecedorAsync(dto);
                return Ok(new { f.Id, f.Nome, f.Cnpj, f.Telefone, f.Email, Entregas = 0, UltimaEntrega = (DateTime?)null });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        // Sugestão de preço pela margem de lucro (padrão 45% sobre a venda)
        [HttpGet("preco-sugerido")]
        [Authorize(Roles = "Gerente,Atendente")]
        public IActionResult PrecoSugerido([FromQuery] decimal custo, [FromQuery] decimal? margem)
        {
            try
            {
                var m = margem.HasValue ? margem.Value / 100m : EstoqueService.MargemSugerida;
                return Ok(new { Custo = custo, Margem = m * 100, PrecoVenda = EstoqueService.PrecoPorMargem(custo, m) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        // Entrada de mercadoria: produto existente ou cadastro de um novo, com nota, validade, custo e preço
        [HttpPost("entradas")]
        [Authorize(Roles = "Gerente,Atendente")]
        public async Task<IActionResult> RegistrarEntradaMercadoria([FromBody] EntradaMercadoriaDTO dto)
        {
            try
            {
                var (idEntrada, idProduto, saldo) = await _estoqueService.RegistrarEntradaAsync(dto, User.ObterId());
                return Ok(new { Mensagem = "Entrada registrada e estoque atualizado.", IdEntrada = idEntrada, IdProduto = idProduto, SaldoAtual = saldo });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        // Últimas entradas recebidas (histórico da aba de entrada)
        [HttpGet("entradas")]
        [Authorize(Roles = "Gerente,Atendente")]
        public async Task<IActionResult> ListarEntradas()
        {
            var entradas = (await _entradaRepo.ObterTodosAsync()).OrderByDescending(e => e.DataEntrada).Take(30).ToList();
            var produtos = (await _produtoRepo.ObterTodosAsync()).ToDictionary(p => p.Id, p => p.Nome);
            var funcionarios = (await _funcionarioRepo.ObterTodosAsync()).ToDictionary(f => f.Id, f => f.Nome);

            return Ok(entradas.Select(e => new {
                e.Id, e.DataEntrada, e.NumeroNota, e.Fornecedor, e.Quantidade, e.Validade, e.CustoUnitario, e.PrecoVendaAplicado,
                Produto = produtos.GetValueOrDefault(e.IdProduto, "—"),
                RecebidoPor = funcionarios.GetValueOrDefault(e.IdFuncionario, "—")
            }));
        }

        public class NovoProdutoDTO { public string Nome { get; set; } = string.Empty; public decimal PrecoVenda { get; set; } public int PontoReposicao { get; set; } public int QuantidadeInicial { get; set; } }
        public class EntradaEstoqueDTO { public int Quantidade { get; set; } }

        // Rotas antigas, mantidas por compatibilidade. A tela usa POST /api/estoque/entradas.
        [HttpPost("produtos")]
        [HttpPost("/api/produtos")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> CadastrarProduto([FromBody] NovoProdutoDTO dto)
        {
            var idProduto = await _produtoRepo.AdicionarAsync(new Produto { Nome = dto.Nome, PrecoVenda = dto.PrecoVenda, Ativo = true });
            await _estoqueRepo.AdicionarAsync(new Estoque {
                IdProduto = idProduto,
                QuantidadeAtual = dto.QuantidadeInicial,
                PontoReposicao = dto.PontoReposicao,
                UltimaAtualizacao = DateTime.Now
            });
            return Ok(new { Mensagem = "Produto cadastrado com sucesso!" });
        }

        [HttpPost("{idProduto}/entrada")]
        [Authorize(Roles = "Gerente,Atendente")]
        public async Task<IActionResult> RegistrarEntrada(int idProduto, [FromBody] EntradaEstoqueDTO dto)
        {
            var estoque = (await _estoqueRepo.ObterTodosAsync()).FirstOrDefault(e => e.IdProduto == idProduto);
            if (estoque == null) return NotFound("Estoque do produto não encontrado.");

            estoque.QuantidadeAtual += dto.Quantidade;
            estoque.UltimaAtualizacao = DateTime.Now;
            await _estoqueRepo.AtualizarAsync(estoque);

            return Ok(new { Mensagem = "Estoque atualizado!" });
        }
    }
}
