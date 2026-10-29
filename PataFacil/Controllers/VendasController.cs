using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using PataFacil.Models;
using PataFacil.BLL;
using PataFacil.Controllers;
using System.Linq;
using System.Collections.Generic;

namespace PataFacil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Atendente,Gerente")]
    public class VendasController : ControllerBase
    {
        private readonly VendaService _vendaService;

        public VendasController(VendaService vendaService)
        {
            _vendaService = vendaService;
        }

        public class NovaVendaDTO
        {
            public int? IdTutor { get; set; } // Opcional: venda de balcão sem cliente cadastrado
            public int FormaPagamento { get; set; }
            public List<NovoItemVendaDTO> Itens { get; set; } = new List<NovoItemVendaDTO>();
        }

        public class NovoItemVendaDTO { public int IdProduto { get; set; } public int Quantidade { get; set; } }

        [HttpPost]
        public async Task<IActionResult> RegistrarVenda([FromBody] NovaVendaDTO dto)
        {
            try
            {
                if (dto.Itens == null || !dto.Itens.Any()) return BadRequest(new { Erro = "A venda deve conter itens." });
                if (!Enum.IsDefined(typeof(FormaPagamentoEnum), dto.FormaPagamento)) return BadRequest(new { Erro = "Forma de pagamento inválida." });

                var novaVenda = new Venda
                {
                    IdTutor = dto.IdTutor > 0 ? dto.IdTutor : null,
                    IdFuncionario = User.ObterId(), // O vendedor é sempre o usuário autenticado
                    FormaPagamento = (FormaPagamentoEnum)dto.FormaPagamento
                };
                var itens = dto.Itens.Select(i => new ItemVenda { IdProduto = i.IdProduto, Quantidade = i.Quantidade }).ToList();

                // Validação de estoque, cálculo do total e alertas de reposição ficam na regra de negócio (RF06, RF07, RN15)
                var (idVenda, total, alertas) = await _vendaService.RegistrarVendaAsync(novaVenda, itens);

                return Ok(new { Mensagem = "Venda registrada com sucesso!", IdVenda = idVenda, Total = total, Alertas = alertas });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }
    }
}
