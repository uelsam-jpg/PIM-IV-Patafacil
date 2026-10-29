using System;

namespace PataFacil.Models
{
    public enum StatusAgendamento
    {
        PENDENTE,
        CONFIRMADO,
        CONCLUIDO,
        CANCELADO
    }

    public class Agendamento
    {
        public int Id { get; set; }
        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }
        public string? Observacoes { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public int IdPet { get; set; }
        public int IdServico { get; set; }
        public int IdFuncionario { get; set; }

        // Anotações internas da equipe sobre o pet agendado (o tutor não vê)
        public string? ObservacoesAtendimento { get; set; }

        // Encapsulamento: Status só pode ser alterado por métodos específicos (Regra RN04)
        public StatusAgendamento Status { get; private set; } = StatusAgendamento.PENDENTE;

        // Balcão: chegada do cliente e baixa do pagamento, também alteradas só pelos métodos abaixo
        public DateTime? DataChegada { get; private set; }
        public decimal? ValorPago { get; private set; }
        public FormaPagamentoEnum? FormaPagamento { get; private set; }
        public DateTime? DataPagamento { get; private set; }

        public void Confirmar()
        {
            if (Status != StatusAgendamento.PENDENTE)
                throw new InvalidOperationException("Apenas agendamentos pendentes podem ser confirmados.");

            Status = StatusAgendamento.CONFIRMADO;
        }

        public void Cancelar()
        {
            if (Status == StatusAgendamento.CONCLUIDO)
                throw new InvalidOperationException("Agendamentos concluídos não podem ser cancelados.");
            if (Status == StatusAgendamento.CANCELADO)
                throw new InvalidOperationException("Este agendamento já está cancelado.");

            Status = StatusAgendamento.CANCELADO;
        }

        public void Concluir()
        {
            // RN04: o fluxo é travado (Pendente -> Confirmado -> Concluído), sem pular a confirmação
            if (Status != StatusAgendamento.CONFIRMADO)
                throw new InvalidOperationException("Apenas agendamentos confirmados podem ser concluídos.");

            Status = StatusAgendamento.CONCLUIDO;
        }

        // O cliente chegou ao balcão: o atendimento entra "em processo".
        // Um agendamento ainda pendente é confirmado na chegada, mantendo a ordem da RN04.
        public void RegistrarChegada()
        {
            if (Status == StatusAgendamento.CANCELADO)
                throw new InvalidOperationException("Este agendamento está cancelado.");
            if (Status == StatusAgendamento.CONCLUIDO)
                throw new InvalidOperationException("Este agendamento já foi concluído.");
            if (DataChegada.HasValue)
                throw new InvalidOperationException("A chegada deste cliente já foi registrada.");

            if (Status == StatusAgendamento.PENDENTE)
                Confirmar();

            DataChegada = DateTime.Now;
        }

        // Baixa no balcão: registra o valor e a forma de pagamento e conclui o atendimento
        public void RegistrarPagamento(decimal valor, FormaPagamentoEnum forma)
        {
            if (Status == StatusAgendamento.CANCELADO)
                throw new InvalidOperationException("Este agendamento está cancelado.");
            if (DataPagamento.HasValue)
                throw new InvalidOperationException("O pagamento deste atendimento já foi registrado.");
            if (!DataChegada.HasValue && Status != StatusAgendamento.CONCLUIDO)
                throw new InvalidOperationException("Registre a chegada do cliente antes de receber o pagamento.");
            if (valor <= 0)
                throw new InvalidOperationException("O valor pago deve ser maior que zero.");
            if (!Enum.IsDefined(forma))
                throw new InvalidOperationException("Forma de pagamento inválida.");

            ValorPago = valor;
            FormaPagamento = forma;
            DataPagamento = DateTime.Now;

            // O veterinário pode já ter concluído ao registrar o prontuário
            if (Status == StatusAgendamento.CONFIRMADO)
                Concluir();
        }
    }
}
