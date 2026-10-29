using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PataFacil.BLL;
using PataFacil.Controllers;

namespace PataFacil.API.Controllers
{
    // Rota curta /api/historico, usada pelas telas Web (portal do tutor e painel do veterinário).
    // As ações, as regras e a autorização são as mesmas de /api/historicoclinico.
    [Route("api/historico")]
    public class HistoricoController : HistoricoClinicoController
    {
        public HistoricoController(HistoricoClinicoService historicoService) : base(historicoService)
        {
        }
    }
}
