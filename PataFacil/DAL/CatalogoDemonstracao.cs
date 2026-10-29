using System;
using System.Collections.Generic;
using System.Linq;
using PataFacil.Models;

namespace PataFacil.DAL
{
    // Catálogo de exemplo (serviços e produtos com estoque e lotes), para o sistema já parecer
    // uma loja em funcionamento. Só insere o que ainda não existe pelo nome, então pode rodar
    // em todo início sem duplicar nada nem mexer no que a equipe cadastrou.
    public static class CatalogoDemonstracao
    {
        // O sistema separa Pet Shop de Veterinária pelo nome: serviços com "banho" ou "tosa"
        // são atendidos pelos profissionais de Banho e Tosa; os demais, pelo veterinário.
        private static readonly (string Nome, string Descricao, int Minutos, decimal Preco)[] Servicos =
        {
            ("Banho Simples - Porte Pequeno", "Banho, secagem, escovação e perfume (até 10 kg)", 40, 50.00m),
            ("Banho Simples - Porte Médio", "Banho, secagem, escovação e perfume (10 a 25 kg)", 50, 65.00m),
            ("Banho Simples - Porte Grande", "Banho, secagem, escovação e perfume (acima de 25 kg)", 60, 85.00m),
            ("Banho e Tosa Higiênica", "Banho com tosa das patas, região íntima e rosto", 60, 75.00m),
            ("Tosa na Máquina", "Tosa completa na máquina, com banho", 60, 90.00m),
            ("Tosa na Tesoura", "Tosa completa na tesoura, com banho", 90, 120.00m),
            ("Tosa Bebê", "Tosa curta e uniforme, com banho", 75, 95.00m),
            ("Banho com Hidratação de Pelagem", "Banho com máscara hidratante para pelos secos", 75, 95.00m),
            ("Banho Medicamentoso", "Banho com shampoo prescrito pelo veterinário", 60, 90.00m),
            ("Banho para Gatos", "Banho e secagem com manejo para felinos", 50, 80.00m),
            ("Banho + Corte de Unhas e Limpeza de Ouvidos", "Banho completo com corte de unhas e limpeza de ouvidos", 50, 70.00m),

            ("Retorno de Consulta", "Reavaliação em até 30 dias após a consulta", 20, 60.00m),
            ("Consulta de Emergência", "Atendimento prioritário sem agendamento prévio", 40, 250.00m),
            ("Vacina Antirrábica", "Dose anual contra raiva", 15, 70.00m),
            ("Vacina V4 Felina", "Polivalente para gatos", 15, 110.00m),
            ("Vacina Gripe Canina", "Proteção contra tosse dos canis", 15, 95.00m),
            ("Vermifugação", "Aplicação de vermífugo conforme o peso", 15, 40.00m),
            ("Aplicação de Medicamento", "Injeção ou medicação prescrita", 15, 35.00m),
            ("Microchipagem", "Implante de microchip de identificação", 20, 120.00m),
            ("Exame de Sangue (Hemograma)", "Coleta e envio para o laboratório", 20, 90.00m),
            ("Check-up Geriátrico", "Consulta completa para pets idosos com exames básicos", 60, 290.00m),
            ("Limpeza de Tártaro", "Profilaxia dentária com ultrassom", 90, 380.00m)
        };

        private record ItemCatalogo(string Nome, CategoriaProdutoEnum Categoria, string Marca, decimal Custo, decimal Venda,
            int Quantidade, int PontoReposicao, int? DiasValidade);

        // Alguns itens começam abaixo do ponto de reposição ou perto de vencer, para os alertas aparecerem
        private static readonly ItemCatalogo[] Produtos =
        {
            new("Ração Premier Adulto Raças Médias 15kg", CategoriaProdutoEnum.Alimento, "Premier Pet", 165.00m, 289.90m, 12, 4, 300),
            new("Ração Golden Gatos Castrados Frango 10,1kg", CategoriaProdutoEnum.Alimento, "Golden", 118.00m, 199.90m, 9, 4, 280),
            new("Ração Royal Canin Mini Adult 7,5kg", CategoriaProdutoEnum.Alimento, "Royal Canin", 205.00m, 359.90m, 6, 3, 330),
            new("Ração Pedigree Adulto Carne e Vegetais 10,1kg", CategoriaProdutoEnum.Alimento, "Pedigree", 72.00m, 129.90m, 15, 5, 240),
            new("Ração Whiskas Adulto Peixe 2,7kg", CategoriaProdutoEnum.Alimento, "Whiskas", 32.00m, 56.90m, 18, 6, 25),
            new("Ração Hill's Science Diet Filhote 6kg", CategoriaProdutoEnum.Alimento, "Hill's", 190.00m, 334.90m, 4, 3, 360),
            new("Sachê Whiskas Carne ao Molho 85g", CategoriaProdutoEnum.Alimento, "Whiskas", 1.60m, 3.49m, 120, 30, 200),
            new("Sachê Pedigree Frango ao Molho 100g", CategoriaProdutoEnum.Alimento, "Pedigree", 1.75m, 3.79m, 96, 30, 210),
            new("Petisco Bifinho Keldog Carne 65g", CategoriaProdutoEnum.Alimento, "Keldog", 4.20m, 8.90m, 40, 10, 150),
            new("Petisco Dreamies Gatos Queijo 40g", CategoriaProdutoEnum.Alimento, "Dreamies", 4.80m, 9.90m, 35, 10, 12),
            new("Osso de Couro Natural 10cm", CategoriaProdutoEnum.Alimento, "Pet Bone", 3.10m, 7.90m, 50, 15, 365),
            new("Alpiste Mix para Pássaros 500g", CategoriaProdutoEnum.Alimento, "Zootekna", 5.50m, 11.90m, 22, 8, 180),
            new("Ração para Hamster Nutrópica 300g", CategoriaProdutoEnum.Alimento, "Nutrópica", 9.80m, 19.90m, 3, 5, 200),

            new("Antipulgas Bravecto Cães 10 a 20kg", CategoriaProdutoEnum.Medicamento, "MSD", 148.00m, 259.90m, 8, 3, 540),
            new("Antipulgas NexGard Cães 4 a 10kg (1 tablete)", CategoriaProdutoEnum.Medicamento, "Boehringer", 52.00m, 92.90m, 14, 5, 480),
            new("Simparic Cães 10,1 a 20kg (1 comprimido)", CategoriaProdutoEnum.Medicamento, "Zoetis", 62.00m, 109.90m, 7, 3, 420),
            new("Coleira Antipulgas Seresto Cães até 8kg", CategoriaProdutoEnum.Medicamento, "Elanco", 145.00m, 249.90m, 5, 2, 720),
            new("Antipulgas Frontline Plus Gatos (pipeta)", CategoriaProdutoEnum.Medicamento, "Boehringer", 38.00m, 69.90m, 10, 4, 450),
            new("Vermífugo Drontal Plus Cães 10kg (2 comprimidos)", CategoriaProdutoEnum.Medicamento, "Elanco", 28.00m, 49.90m, 20, 6, 600),
            new("Vermífugo Milbemax Gatos (2 comprimidos)", CategoriaProdutoEnum.Medicamento, "Elanco", 30.00m, 54.90m, 2, 4, 500),
            new("Suplemento Ômega 3 Organnact 30 cápsulas", CategoriaProdutoEnum.Medicamento, "Organnact", 34.00m, 62.90m, 9, 3, 365),
            new("Pomada Cicatrizante Vetaglós 50g", CategoriaProdutoEnum.Medicamento, "Vetnil", 18.00m, 34.90m, 6, 3, 18),

            new("Shampoo Neutro Sanol Dog 500ml", CategoriaProdutoEnum.Higiene, "Sanol", 11.00m, 22.90m, 25, 8, 720),
            new("Condicionador Pet Society Hydrate 500ml", CategoriaProdutoEnum.Higiene, "Pet Society", 22.00m, 42.90m, 10, 4, 720),
            new("Tapete Higiênico Super Secão 30 unidades", CategoriaProdutoEnum.Higiene, "Super Secão", 39.00m, 74.90m, 16, 6, null),
            new("Areia Sanitária Pipicat Classic 4kg", CategoriaProdutoEnum.Higiene, "Pipicat", 9.50m, 19.90m, 30, 10, null),
            new("Areia de Sílica Kelcat 1,8kg", CategoriaProdutoEnum.Higiene, "Kelco", 18.00m, 34.90m, 4, 6, null),
            new("Lenço Umedecido Pet 100 unidades", CategoriaProdutoEnum.Higiene, "Petlike", 9.00m, 18.90m, 20, 6, 540),
            new("Kit Escova e Creme Dental Pet", CategoriaProdutoEnum.Higiene, "Petclean", 12.00m, 24.90m, 12, 4, 600),
            new("Colônia Pet Baby 120ml", CategoriaProdutoEnum.Higiene, "Pet Society", 14.00m, 29.90m, 11, 4, 720),
            new("Eliminador de Odores Sanol 500ml", CategoriaProdutoEnum.Higiene, "Sanol", 12.50m, 26.90m, 8, 4, null),
            new("Saquinho Coletor de Fezes (4 rolos)", CategoriaProdutoEnum.Higiene, "Chalesco", 6.00m, 14.90m, 28, 8, null),

            new("Coleira de Nylon Ajustável M", CategoriaProdutoEnum.Acessorio, "Chalesco", 9.00m, 24.90m, 15, 5, null),
            new("Guia Retrátil 5m até 20kg", CategoriaProdutoEnum.Acessorio, "Flexi", 48.00m, 99.90m, 6, 2, null),
            new("Peitoral Air Mesh P", CategoriaProdutoEnum.Acessorio, "Zee.Dog", 55.00m, 119.90m, 4, 2, null),
            new("Comedouro Inox Antiderrapante 750ml", CategoriaProdutoEnum.Acessorio, "Chalesco", 14.00m, 32.90m, 18, 6, null),
            new("Fonte Bebedouro Elétrica para Gatos 2L", CategoriaProdutoEnum.Acessorio, "Petlon", 85.00m, 169.90m, 3, 2, null),
            new("Cama Pet Redonda M", CategoriaProdutoEnum.Acessorio, "Fábrica Pet", 62.00m, 139.90m, 5, 2, null),
            new("Arranhador para Gatos com Plataforma", CategoriaProdutoEnum.Acessorio, "São Pet", 70.00m, 149.90m, 4, 2, null),
            new("Brinquedo Kong Classic M", CategoriaProdutoEnum.Acessorio, "Kong", 52.00m, 109.90m, 7, 3, null),
            new("Mordedor de Corda 3 Nós", CategoriaProdutoEnum.Acessorio, "Jambo", 8.00m, 19.90m, 22, 8, null),
            new("Bolinha com Guizo para Gatos (3 unidades)", CategoriaProdutoEnum.Acessorio, "Jambo", 4.50m, 12.90m, 30, 10, null),
            new("Caixa de Transporte N.2", CategoriaProdutoEnum.Acessorio, "Plast Pet", 58.00m, 119.90m, 5, 2, null),
            new("Moletom Pet P", CategoriaProdutoEnum.Acessorio, "Pet Fashion", 22.00m, 49.90m, 8, 3, null),
            new("Gaiola para Hamster Completa", CategoriaProdutoEnum.Acessorio, "Jel Plast", 95.00m, 189.90m, 2, 2, null),
            new("Escova Rasqueadeira", CategoriaProdutoEnum.Acessorio, "Chalesco", 12.00m, 27.90m, 14, 4, null),
            new("Cortador de Unhas Pet", CategoriaProdutoEnum.Acessorio, "Chalesco", 11.00m, 24.90m, 9, 3, null),

            new("Placa de Identificação Gravada", CategoriaProdutoEnum.Outro, "PataFácil", 8.00m, 25.00m, 40, 10, null)
        };

        private static readonly Dictionary<CategoriaProdutoEnum, (string Fornecedor, string Nota)> Notas = new()
        {
            [CategoriaProdutoEnum.Alimento] = ("Distribuidora Pet Alimentos", "000184522"),
            [CategoriaProdutoEnum.Medicamento] = ("VetFarma Distribuidora", "000093117"),
            [CategoriaProdutoEnum.Higiene] = ("Higipet Comercial", "000051840"),
            [CategoriaProdutoEnum.Acessorio] = ("Acessórios Pet Brasil", "000027365"),
            [CategoriaProdutoEnum.Outro] = ("Gravações Rápidas", "000003311")
        };

        public static void Criar(PataFacilContext db)
        {
            CriarProdutosEServicos(db);

            // Fornecedores que só existiam no texto das entradas passam a fazer parte do cadastro
            var cadastrados = db.Fornecedores.Select(f => f.Nome.ToLower()).ToHashSet();
            var nomes = db.EntradasEstoque.Where(e => e.Fornecedor != "").Select(e => e.Fornecedor.Trim()).Distinct().ToList();
            foreach (var nome in nomes.Where(n => n.Length >= 2).GroupBy(n => n.ToLower()).Select(g => g.First()))
                if (!cadastrados.Contains(nome.ToLower()))
                    db.Fornecedores.Add(new Fornecedor { Nome = nome, DataCadastro = DateTime.Now, Ativo = true });
            db.SaveChanges();
        }

        private static void CriarProdutosEServicos(PataFacilContext db)
        {
            var servicosExistentes = db.Servicos.Select(s => s.Nome.ToLower()).ToHashSet();
            foreach (var s in Servicos.Where(s => !servicosExistentes.Contains(s.Nome.ToLower())))
                db.Servicos.Add(new Servico { Nome = s.Nome, Descricao = s.Descricao, DuracaoMinutos = s.Minutos, Preco = s.Preco, Ativo = true });
            db.SaveChanges();

            var produtosExistentes = db.Produtos.Select(p => p.Nome.ToLower()).ToHashSet();
            var novos = Produtos.Where(p => !produtosExistentes.Contains(p.Nome.ToLower())).ToList();
            if (novos.Count == 0) return;

            var recebedor = db.Funcionarios.Where(f => f.Ativo).OrderBy(f => f.Cargo == Cargo.Atendente ? 0 : 1).ThenBy(f => f.Id).FirstOrDefault();
            var dataEntrada = DateTime.Today.AddDays(-10).AddHours(9);

            foreach (var item in novos)
            {
                var produto = new Produto
                {
                    Nome = item.Nome, Categoria = item.Categoria, Marca = item.Marca, UnidadeMedida = "UN",
                    PrecoCusto = item.Custo, PrecoVenda = item.Venda, Ativo = true
                };
                db.Produtos.Add(produto);
                db.SaveChanges();

                db.Estoques.Add(new Estoque { IdProduto = produto.Id, QuantidadeAtual = item.Quantidade, PontoReposicao = item.PontoReposicao, UltimaAtualizacao = dataEntrada });

                // Lote de entrada com a nota do fornecedor, para a validade e o histórico aparecerem
                if (recebedor != null)
                {
                    var (fornecedor, nota) = Notas[item.Categoria];
                    db.EntradasEstoque.Add(new EntradaEstoque
                    {
                        IdProduto = produto.Id, Quantidade = item.Quantidade, NumeroNota = nota, Fornecedor = fornecedor,
                        Validade = item.DiasValidade.HasValue ? DateTime.Today.AddDays(item.DiasValidade.Value) : null,
                        CustoUnitario = item.Custo, PrecoVendaAplicado = item.Venda,
                        DataEntrada = dataEntrada, IdFuncionario = recebedor.Id
                    });
                }
            }
            db.SaveChanges();
        }
    }
}
