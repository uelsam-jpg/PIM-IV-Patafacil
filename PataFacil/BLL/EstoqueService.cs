using System;
using System.Linq;
using System.Threading.Tasks;
using PataFacil.Models;
using PataFacil.DAL;

namespace PataFacil.BLL
{
    // Dados de um recebimento de mercadoria. Quando IdProduto é nulo, NovoProduto descreve o item a cadastrar.
    public class EntradaMercadoriaDTO
    {
        public int? IdProduto { get; set; }
        public NovoProdutoEntradaDTO? NovoProduto { get; set; }
        public int Quantidade { get; set; }
        public string NumeroNota { get; set; } = string.Empty;
        public string? Fornecedor { get; set; }
        public DateTime? Validade { get; set; }
        public decimal CustoUnitario { get; set; }
        public decimal PrecoVenda { get; set; }
    }

    public class NovoProdutoEntradaDTO
    {
        public string Nome { get; set; } = string.Empty;
        public CategoriaProdutoEnum Categoria { get; set; } = CategoriaProdutoEnum.Outro;
        public string? Marca { get; set; }
        public int PontoReposicao { get; set; } = 5;
    }

    public class NovoFornecedorDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string? Cnpj { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
    }

    public class EstoqueService
    {
        // Margem de lucro sugerida sobre o preço de venda (não é markup): lucro / preço = 45%
        public const decimal MargemSugerida = 0.45m;

        private readonly IRepositorio<Produto> _produtoRepo;
        private readonly IRepositorio<Estoque> _estoqueRepo;
        private readonly IRepositorio<EntradaEstoque> _entradaRepo;
        private readonly IRepositorio<Fornecedor>? _fornecedorRepo;

        public EstoqueService(IRepositorio<Produto> produtoRepo, IRepositorio<Estoque> estoqueRepo, IRepositorio<EntradaEstoque> entradaRepo,
            IRepositorio<Fornecedor>? fornecedorRepo = null)
        {
            _produtoRepo = produtoRepo;
            _estoqueRepo = estoqueRepo;
            _entradaRepo = entradaRepo;
            _fornecedorRepo = fornecedorRepo;
        }

        public async Task<Fornecedor> CadastrarFornecedorAsync(NovoFornecedorDTO dto)
        {
            if (_fornecedorRepo == null) throw new InvalidOperationException("Cadastro de fornecedores indisponível.");

            var nome = (dto.Nome ?? string.Empty).Trim();
            if (nome.Length < 2)
                throw new InvalidOperationException("Informe o nome do fornecedor.");
            if (nome.Length > 120)
                throw new InvalidOperationException("O nome do fornecedor pode ter no máximo 120 caracteres.");

            var cnpj = new string((dto.Cnpj ?? string.Empty).Where(char.IsDigit).ToArray());
            if (cnpj.Length > 0 && cnpj.Length != 14)
                throw new InvalidOperationException("O CNPJ deve ter 14 dígitos.");

            var email = (dto.Email ?? string.Empty).Trim();
            if (email.Length > 0 && (!email.Contains('@') || email.Length > 120))
                throw new InvalidOperationException("E-mail do fornecedor inválido.");

            var telefone = (dto.Telefone ?? string.Empty).Trim();
            if (telefone.Length > 20)
                throw new InvalidOperationException("Telefone do fornecedor inválido.");

            if ((await _fornecedorRepo.BuscarAsync(f => f.Nome.ToLower() == nome.ToLower())).Any())
                throw new InvalidOperationException($"O fornecedor \"{nome}\" já está cadastrado. Selecione-o na lista.");
            if (cnpj.Length > 0 && (await _fornecedorRepo.BuscarAsync(f => f.Cnpj == cnpj)).Any())
                throw new InvalidOperationException("Já existe um fornecedor com este CNPJ.");

            var fornecedor = new Fornecedor { Nome = nome, Cnpj = cnpj, Telefone = telefone, Email = email, DataCadastro = DateTime.Now, Ativo = true };
            fornecedor.Id = await _fornecedorRepo.AdicionarAsync(fornecedor);
            return fornecedor;
        }

        // Nome digitado direto na entrada: entra no cadastro para aparecer nas próximas
        private async Task GarantirFornecedorAsync(string nome)
        {
            if (_fornecedorRepo == null || nome.Length < 2) return;
            if ((await _fornecedorRepo.BuscarAsync(f => f.Nome.ToLower() == nome.ToLower())).Any()) return;
            await _fornecedorRepo.AdicionarAsync(new Fornecedor { Nome = nome, DataCadastro = DateTime.Now, Ativo = true });
        }

        // Preço de venda para uma margem sobre a venda: preço = custo / (1 - margem).
        // Ex.: custo R$ 100 com margem de 45% -> R$ 181,82 (o lucro de R$ 81,82 é 45% do preço).
        // Com markup de 45% seria R$ 145,00, que dá só 31% de margem.
        public static decimal PrecoPorMargem(decimal custo, decimal margem)
        {
            if (custo <= 0) throw new ArgumentException("O custo deve ser maior que zero.");
            if (margem < 0 || margem >= 1) throw new ArgumentException("A margem deve ficar entre 0% e 99%.");
            return Math.Round(custo / (1 - margem), 2, MidpointRounding.AwayFromZero);
        }

        public async Task<(int IdEntrada, int IdProduto, int SaldoAtual)> RegistrarEntradaAsync(EntradaMercadoriaDTO dto, int idFuncionario)
        {
            var numeroNota = (dto.NumeroNota ?? string.Empty).Trim();
            if (numeroNota.Length == 0)
                throw new InvalidOperationException("Informe o número da nota fiscal.");
            if (numeroNota.Length > 30)
                throw new InvalidOperationException("O número da nota pode ter no máximo 30 caracteres.");
            if (dto.Quantidade <= 0)
                throw new InvalidOperationException("A quantidade recebida deve ser maior que zero.");
            if (dto.CustoUnitario <= 0)
                throw new InvalidOperationException("Informe o valor de compra (custo unitário).");
            if (dto.PrecoVenda <= 0)
                throw new InvalidOperationException("Informe o preço de venda.");
            if (dto.PrecoVenda < dto.CustoUnitario)
                throw new InvalidOperationException("O preço de venda não pode ser menor que o custo de compra.");
            if (dto.Validade.HasValue && dto.Validade.Value.Date < DateTime.Today)
                throw new InvalidOperationException("A validade informada já passou: o produto está vencido.");

            Produto produto;
            Estoque? estoque;
            if (dto.IdProduto.HasValue)
            {
                produto = await _produtoRepo.ObterPorIdAsync(dto.IdProduto.Value)
                    ?? throw new InvalidOperationException("Produto não encontrado.");
                var idProduto = produto.Id;
                estoque = (await _estoqueRepo.BuscarAsync(e => e.IdProduto == idProduto)).FirstOrDefault();
            }
            else
            {
                var novo = dto.NovoProduto ?? throw new InvalidOperationException("Escolha um produto ou cadastre um novo.");
                var nome = (novo.Nome ?? string.Empty).Trim();
                if (nome.Length < 2)
                    throw new InvalidOperationException("Informe o nome do novo produto.");
                if (novo.PontoReposicao < 0)
                    throw new InvalidOperationException("O ponto de reposição não pode ser negativo.");
                if ((await _produtoRepo.BuscarAsync(p => p.Ativo && p.Nome.ToLower() == nome.ToLower())).Any())
                    throw new InvalidOperationException($"Já existe um produto chamado \"{nome}\". Selecione-o na lista.");

                produto = new Produto
                {
                    Nome = nome,
                    Categoria = novo.Categoria,
                    Marca = (novo.Marca ?? string.Empty).Trim(),
                    UnidadeMedida = "UN",
                    Ativo = true
                };
                produto.Id = await _produtoRepo.AdicionarAsync(produto);
                estoque = new Estoque { IdProduto = produto.Id, QuantidadeAtual = 0, PontoReposicao = novo.PontoReposicao };
                estoque.Id = await _estoqueRepo.AdicionarAsync(estoque);
            }

            // O custo e o preço passam a valer a partir desta compra
            produto.PrecoCusto = dto.CustoUnitario;
            produto.PrecoVenda = dto.PrecoVenda;
            await _produtoRepo.AtualizarAsync(produto);

            if (estoque == null)
            {
                estoque = new Estoque { IdProduto = produto.Id, QuantidadeAtual = dto.Quantidade, PontoReposicao = 5, UltimaAtualizacao = DateTime.Now };
                await _estoqueRepo.AdicionarAsync(estoque);
            }
            else
            {
                estoque.QuantidadeAtual += dto.Quantidade;
                estoque.UltimaAtualizacao = DateTime.Now;
                await _estoqueRepo.AtualizarAsync(estoque);
            }

            var nomeFornecedor = (dto.Fornecedor ?? string.Empty).Trim();
            await GarantirFornecedorAsync(nomeFornecedor);

            var idEntrada = await _entradaRepo.AdicionarAsync(new EntradaEstoque
            {
                IdProduto = produto.Id,
                Quantidade = dto.Quantidade,
                NumeroNota = numeroNota,
                Fornecedor = nomeFornecedor,
                Validade = dto.Validade?.Date,
                CustoUnitario = dto.CustoUnitario,
                PrecoVendaAplicado = dto.PrecoVenda,
                DataEntrada = DateTime.Now,
                IdFuncionario = idFuncionario
            });

            return (idEntrada, produto.Id, estoque.QuantidadeAtual);
        }
    }
}
