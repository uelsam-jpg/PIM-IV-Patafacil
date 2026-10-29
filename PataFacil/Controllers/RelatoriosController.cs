using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using PataFacil.BLL;

namespace PataFacil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Rota base: /api/relatorios
    public class RelatoriosController : ControllerBase
    {
        private readonly RelatorioService _relatorioService;
        private readonly PainelService _painelService;

        public RelatoriosController(RelatorioService relatorioService, PainelService painelService)
        {
            _relatorioService = relatorioService;
            _painelService = painelService;
        }

        // Gráficos da Visão Geral: andamento do mês, comparação com o anterior, rankings e promoções
        [HttpGet("painel")]
        [Authorize(Roles = "Atendente,Gerente")]
        public async Task<IActionResult> ObterPainel([FromQuery] int ano, [FromQuery] int mes)
        {
            try
            {
                if (mes < 1 || mes > 12 || ano < 2000)
                {
                    ano = DateTime.Now.Year;
                    mes = DateTime.Now.Month;
                }
                return Ok(await _painelService.ObterPainelAsync(ano, mes));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = "Erro ao montar o painel: " + ex.Message });
            }
        }

        [HttpGet("receita-mensal")]
        [Authorize(Roles = "Gerente")] // Dados financeiros: somente o Gerente (demais perfis recebem 403)
        public async Task<IActionResult> ObterReceitaMensal([FromQuery] int ano, [FromQuery] int mes)
        {
            try
            {
                // Validação de fallback: se não passar o mês/ano na URL, pega o atual
                if (mes < 1 || mes > 12 || ano < 2000)
                {
                    ano = DateTime.Now.Year;
                    mes = DateTime.Now.Month;
                }

                var relatorio = await _relatorioService.GerarRelatorioMensalAsync(ano, mes);
                return Ok(relatorio);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = "Erro ao gerar relatório: " + ex.Message });
            }
        }

        [HttpGet("dashboard-kpis")]
        [Authorize(Roles = "Atendente,Veterinario,Gerente")]
        public async Task<IActionResult> ObterKpisDashboard()
        {
            try
            {
                var kpis = await _relatorioService.ObterKpisDashboardAsync();
                return Ok(kpis);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = "Erro ao obter KPIs: " + ex.Message });
            }
        }
    }
}