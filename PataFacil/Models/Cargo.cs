namespace PataFacil.Models
{
    // Enum exigido na Seção 8.2 do Modelo Lógico para o Funcionario
    public enum Cargo
    {
        Veterinario = 0,
        Atendente = 1,
        Gerente = 2,
        Tosador = 3 // Profissional de Banho e Tosa: é ele quem atende os serviços de Pet Shop
    }
}
