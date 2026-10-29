using Microsoft.EntityFrameworkCore;
using PataFacil.Models;

namespace PataFacil.DAL
{
    public class PataFacilContext : DbContext
    {
        public PataFacilContext(DbContextOptions<PataFacilContext> options) : base(options)
        {
        }

        // Mapeamento das nossas classes para as tabelas do MySQL
        public DbSet<Tutor> Tutores { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<HistoricoClinico> Historicos { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Estoque> Estoques { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }
        public DbSet<EmailEnviado> Emails { get; set; }
        public DbSet<EntradaEstoque> EntradasEstoque { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Nomes de tabela em minúsculas, iguais aos do ScriptBancoDeDados.sql.
            // No Linux (Docker) o MySQL diferencia maiúsculas de minúsculas em nomes de tabela.
            modelBuilder.Entity<Tutor>().ToTable("tutores");
            modelBuilder.Entity<Pet>().ToTable("pets");
            modelBuilder.Entity<Funcionario>().ToTable("funcionarios");
            modelBuilder.Entity<Servico>().ToTable("servicos");
            modelBuilder.Entity<Agendamento>().ToTable("agendamentos");
            modelBuilder.Entity<HistoricoClinico>().ToTable("historicos");
            modelBuilder.Entity<Produto>().ToTable("produtos");
            modelBuilder.Entity<Estoque>().ToTable("estoques");
            modelBuilder.Entity<Venda>().ToTable("vendas");
            modelBuilder.Entity<ItemVenda>().ToTable("itensvenda");
            modelBuilder.Entity<EmailEnviado>().ToTable("emails");
            modelBuilder.Entity<EntradaEstoque>().ToTable("entradas_estoque");
            modelBuilder.Entity<Fornecedor>().ToTable("fornecedores");
        }
    }
}
