using System.Security.Claims;

namespace PataFacil.Controllers
{
    // Leitura das claims do Token JWT (quem é o usuário e qual o seu perfil)
    public static class UsuarioAutenticado
    {
        public static int ObterId(this ClaimsPrincipal usuario)
        {
            return int.TryParse(usuario.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
        }

        // Devolve o Id quando o usuário é Tutor; null para a equipe (que não tem restrição de dono)
        public static int? ObterIdSeTutor(this ClaimsPrincipal usuario)
        {
            return usuario.IsInRole("Tutor") ? usuario.ObterId() : null;
        }
    }
}
