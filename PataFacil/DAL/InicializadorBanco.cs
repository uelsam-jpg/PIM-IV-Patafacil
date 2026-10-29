using System;
using System.IO;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace PataFacil.DAL
{
    // Cria o banco a partir do ScriptBancoDeDados.sql, para que a API, a aplicação Web e o
    // Mobile usem exatamente a estrutura do script (tabelas, triggers, procedures, views e índices).
    public static class InicializadorBanco
    {
        public static void AplicarScript(string connectionString, string caminhoScript)
        {
            var builder = new MySqlConnectionStringBuilder(connectionString) { AllowUserVariables = true };
            var nomeBanco = builder.Database;
            if (!Regex.IsMatch(nomeBanco, "^[A-Za-z0-9_]+$"))
                throw new InvalidOperationException("Nome de banco inválido na string de conexão.");

            // Conecta sem banco selecionado: na primeira execução ele ainda não existe
            builder.Database = string.Empty;
            using var conexao = new MySqlConnection(builder.ConnectionString);
            conexao.Open();

            using (var comando = conexao.CreateCommand())
            {
                comando.CommandText = $"CREATE DATABASE IF NOT EXISTS `{nomeBanco}` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci";
                comando.ExecuteNonQuery();
            }
            conexao.ChangeDatabase(nomeBanco);

            // O cabeçalho CREATE DATABASE / USE do script serve à execução manual (mysql < script);
            // aqui vale o banco da string de conexão.
            var sql = File.ReadAllText(caminhoScript);
            sql = Regex.Replace(sql, @"^\s*CREATE\s+DATABASE\b[^;]*;", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            sql = Regex.Replace(sql, @"^\s*USE\s+\w+\s*;", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // MySqlScript entende a diretiva DELIMITER usada nas triggers e procedures
            new MySqlScript(conexao, sql).Execute();
        }
    }
}
