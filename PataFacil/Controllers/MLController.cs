using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PataFacil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Atendente,Veterinario,Gerente")] // Indicadores internos: somente a equipe
    public class MLController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public MLController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // URL da IA configurável (necessário para o Docker: serviço "ml-api" em vez de 127.0.0.1)
        private string MlUrl => _configuration["MlApiUrl"] ?? "http://127.0.0.1:8000";

        // Rota que o Frontend (dashboard.html) chama para exibir a previsão do dia
        [HttpGet("previsao-demanda-hoje")]
        public Task<IActionResult> PreverDemandaHoje()
        {
            var hoje = DateTime.Today;

            // dia_semana segue o DayOfWeek do C# (0 = domingo). Feriado, promoção e temperatura
            // usam valores padrão, pois o sistema ainda não mantém esses cadastros.
            var dados = new { dia_semana = (int)hoje.DayOfWeek, mes = hoje.Month, feriado = 0, promocao = 0, temperatura = 25.0 };
            return EncaminharParaIaAsync("/api/ml/prever-demanda", dados);
        }

        public class SegmentacaoDTO
        {
            public int FrequenciaAnual { get; set; }
            public double TicketMedio { get; set; }
            public int RecenciaDias { get; set; }
            public int NumPets { get; set; }
        }

        // Classifica um cliente em um dos perfis (Ocasional, Em risco, Fiel, VIP)
        [HttpPost("segmentar-tutor")]
        [Authorize(Roles = "Gerente")]
        public Task<IActionResult> SegmentarTutor([FromBody] SegmentacaoDTO dto)
        {
            var dados = new { frequencia_anual = dto.FrequenciaAnual, ticket_medio = dto.TicketMedio, recencia_dias = dto.RecenciaDias, num_pets = dto.NumPets };
            return EncaminharParaIaAsync("/api/ml/segmentar-tutor", dados);
        }

        private async Task<IActionResult> EncaminharParaIaAsync(string rota, object dados)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(5);

                var response = await client.PostAsJsonAsync($"{MlUrl}{rota}", dados);
                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadAsStringAsync();
                    return Content(resultado, "application/json"); // Devolve para o Frontend o JSON da IA
                }
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                // Cai no retorno 503 abaixo: a IA é um serviço à parte e pode estar fora do ar
            }
            return StatusCode(503, new { erro = "Serviço de IA (Python) indisponível no momento." });
        }
    }
}
