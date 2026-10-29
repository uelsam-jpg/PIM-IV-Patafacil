using System;

namespace PataFacil.Models
{
    public enum TipoAtendimentoEnum { Consulta, Vacina, Cirurgia, Retorno, Exame }

    public class HistoricoClinico
    {
        public int Id { get; set; }
        public DateTime DataAtendimento { get; set; } = DateTime.Now;
        public TipoAtendimentoEnum Tipo { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string? Diagnostico { get; set; }
        public string? Prescricao { get; set; }
        public decimal PesoRegistro { get; set; }
        
        public DateTime? ProximaVacina { get; set; }

        // Chaves Estrangeiras
        public int IdPet { get; set; }
        public int IdFuncionario { get; set; }
        public int? IdAgendamento { get; set; } // Opcional: nem todo registro clínico nasce de um agendamento
    }
}