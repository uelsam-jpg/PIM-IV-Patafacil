using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using PataFacil.DAL;
using PataFacil.BLL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Adiciona os Controllers (ativa as rotas da sua API)
builder.Services.AddControllers(options => {
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true; // Impede que o C# bloqueie o JSON por falta do objeto Tutor completo
});

// 2. Configuração do Swagger (Interface visual para testar a API no navegador)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Configurando a conexão com o banco de dados MySQL
var connectionString = builder.Configuration.GetConnectionString("ConexaoMySQL") 
                       ?? "Server=127.0.0.1;Database=patafacil;Uid=root;Pwd=Admin@123.;"; // 127.0.0.1 elimina o delay/lentidão do IPv6

builder.Services.AddDbContext<PataFacilContext>(options =>
    options.UseMySQL(connectionString));

// 4. Injeção de Dependências (Ligando o "Cérebro" do sistema)
builder.Services.AddHttpClient(); // Ativa a injeção do IHttpClientFactory para requisições externas
builder.Services.AddScoped<AgendamentoService>();
builder.Services.AddScoped<HistoricoClinicoService>();
builder.Services.AddScoped<VendaService>();
builder.Services.AddScoped<EstoqueService>();
builder.Services.AddScoped<PainelService>();
builder.Services.AddScoped<RelatorioService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Avisa ao C# qual é a classe concreta (Repositorio) da interface abstrata (IRepositorio)
builder.Services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));

// 5. Permissão de CORS (Necessário para o frontend conversar com o backend sem bloqueio)
builder.Services.AddCors(options => {
    options.AddPolicy("PermitirTudo", policy => {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// 6. Configuração da Autenticação JWT (Requisito RNF02)
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "MinhaChaveSuperSecretaPataFacil2025!");
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// =============== BANCO DE DADOS ===============
// Cria/atualiza o banco a partir do script oficial (DAL/ScriptBancoDeDados.sql): as mesmas tabelas,
// triggers, procedures, views e índices são usados pela API, pela aplicação Web e pelo Mobile.
try
{
    var caminhoScript = Path.Combine(AppContext.BaseDirectory, "DAL", "ScriptBancoDeDados.sql");
    InicializadorBanco.AplicarScript(connectionString, caminhoScript);
    System.Console.WriteLine("Banco de dados verificado/criado com sucesso!");

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<PataFacilContext>();

    // --- INÍCIO DA INJEÇÃO DE DADOS PADRÃO ---
    // Garante que o frontend tenha o que exibir no Passo 2 e 3 do Agendamento
    if (!dbContext.Servicos.Any())
    {
        dbContext.Servicos.AddRange(
            new PataFacil.Models.Servico { Nome = "Banho e Tosa", Descricao = "Banho completo", DuracaoMinutos = 60, Preco = 80.00m, Ativo = true },
            new PataFacil.Models.Servico { Nome = "Consulta Veterinária", Descricao = "Avaliação clínica", DuracaoMinutos = 30, Preco = 150.00m, Ativo = true },
            new PataFacil.Models.Servico { Nome = "Vacina V10", Descricao = "Dose anual", DuracaoMinutos = 15, Preco = 100.00m, Ativo = true }
        );
        dbContext.SaveChanges();
    }

    // Equipe inicial. A senha de demonstração (123456) é gravada em hash BCrypt, nunca em texto puro.
    var equipePadrao = new[]
    {
        new PataFacil.Models.Funcionario { Nome = "Dr. PataFácil", Cpf = "00000000000", Email = "vet@patafacil.com", Cargo = PataFacil.Models.Cargo.Veterinario },
        new PataFacil.Models.Funcionario { Nome = "Ana Atendente", Cpf = "11111111111", Email = "atendente@patafacil.com", Cargo = PataFacil.Models.Cargo.Atendente },
        new PataFacil.Models.Funcionario { Nome = "Gerente Silva", Cpf = "22222222222", Email = "gerente@patafacil.com", Cargo = PataFacil.Models.Cargo.Gerente },
        // Profissionais de Banho e Tosa: o tutor escolhe um deles ao agendar um serviço de Pet Shop
        new PataFacil.Models.Funcionario { Nome = "Carla Mendes", Cpf = "33333333333", Email = "carla.tosa@patafacil.com", Cargo = PataFacil.Models.Cargo.Tosador },
        new PataFacil.Models.Funcionario { Nome = "Bruno Costa", Cpf = "44444444444", Email = "bruno.tosa@patafacil.com", Cargo = PataFacil.Models.Cargo.Tosador },
        new PataFacil.Models.Funcionario { Nome = "Júlia Rocha", Cpf = "55555555555", Email = "julia.tosa@patafacil.com", Cargo = PataFacil.Models.Cargo.Tosador }
    };
    foreach (var funcionario in equipePadrao)
    {
        if (dbContext.Funcionarios.Any(f => f.Email == funcionario.Email)) continue;
        funcionario.SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456", 12);
        funcionario.DataAdmissao = System.DateTime.Now;
        funcionario.Ativo = true;
        dbContext.Funcionarios.Add(funcionario);
    }
    dbContext.SaveChanges();

    // Bancos criados por versões anteriores guardavam a senha da equipe em texto puro: converte para BCrypt
    foreach (var funcionario in dbContext.Funcionarios.ToList().Where(f => !f.SenhaHash.StartsWith("$2")))
    {
        funcionario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(funcionario.SenhaHash, 12);
    }
    dbContext.SaveChanges();

    if (!dbContext.Produtos.Any())
    {
        var p1 = new PataFacil.Models.Produto { Nome = "Ração Golden 15kg", Categoria = PataFacil.Models.CategoriaProdutoEnum.Alimento, UnidadeMedida = "UN", PrecoCusto = 105.00m, PrecoVenda = 150.00m, Ativo = true };
        var p2 = new PataFacil.Models.Produto { Nome = "Shampoo Pet", Categoria = PataFacil.Models.CategoriaProdutoEnum.Higiene, UnidadeMedida = "UN", PrecoCusto = 18.00m, PrecoVenda = 35.00m, Ativo = true };
        var p3 = new PataFacil.Models.Produto { Nome = "Bolinha de Borracha", Categoria = PataFacil.Models.CategoriaProdutoEnum.Acessorio, UnidadeMedida = "UN", PrecoCusto = 6.00m, PrecoVenda = 15.00m, Ativo = true };
        
        dbContext.Produtos.AddRange(p1, p2, p3);
        dbContext.SaveChanges();
        
        // Criar estoque inicial. O Shampoo Pet foi configurado para disparar o Alerta Crítico (RF15)!
        dbContext.Estoques.AddRange(
            new PataFacil.Models.Estoque { IdProduto = p1.Id, QuantidadeAtual = 10, PontoReposicao = 5 },
            new PataFacil.Models.Estoque { IdProduto = p2.Id, QuantidadeAtual = 3, PontoReposicao = 5 }, 
            new PataFacil.Models.Estoque { IdProduto = p3.Id, QuantidadeAtual = 20, PontoReposicao = 10 }
        );
        dbContext.SaveChanges();
    }

    CatalogoDemonstracao.Criar(dbContext);
    DadosDemonstracao.Criar(dbContext);
    // --- FIM DA INJEÇÃO ---
}
catch (System.Exception ex)
{
    System.Console.WriteLine($"[ERRO DE BANCO DE DADOS] Verifique se o MySQL está no ar e a senha em appsettings.json: {ex.Message}");
}
// ==============================================
// 7. Habilita a interface visual do Swagger no modo de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("PermitirTudo"); // Ativa a política de CORS
app.UseHttpsRedirection();
app.UseAuthentication(); // Ativa o suporte a JWT
app.UseAuthorization();
app.MapControllers();

// Verificação de saúde para o orquestrador/monitoramento: responde 200 se a API alcança o banco
app.MapGet("/health", async (PataFacilContext db) =>
    await db.Database.CanConnectAsync()
        ? Results.Ok(new { status = "ok" })
        : Results.Json(new { status = "banco indisponivel" }, statusCode: 503));

// Inicia a aplicação
app.Run();
