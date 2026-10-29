using Xunit;

namespace PataFacil.Tests
{
    public class AuthTests
    {
        [Fact]
        public void T07_ValidarSenha_ComHashCorreto_RetornaTrue()
        {
            // Simula o comportamento do AuthService / AuthController
            string senhaPlana = "senhaSegura123";
            string senhaHash = BCrypt.Net.BCrypt.HashPassword(senhaPlana, 12);
            
            bool valido = BCrypt.Net.BCrypt.Verify(senhaPlana, senhaHash);
            Assert.True(valido);
        }

        [Fact]
        public void T08_ValidarSenha_ComHashIncorreto_RetornaFalse()
        {
            string senhaPlana = "senhaSegura123";
            string senhaErrada = "senhaErrada999";
            string senhaHash = BCrypt.Net.BCrypt.HashPassword(senhaPlana, 12);
            
            bool valido = BCrypt.Net.BCrypt.Verify(senhaErrada, senhaHash);
            Assert.False(valido);
        }
    }
}