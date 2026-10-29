using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PataFacil.Models;
using PataFacil.DAL;

namespace PataFacil.BLL
{
    public class HistoricoClinicoService
    {
        private readonly IRepositorio<HistoricoClinico> _historicoRepo;
        private readonly IRepositorio<Agendamento> _agendamentoRepo;
        private readonly IRepositorio<Pet> _petRepo;

        public HistoricoClinicoService(
            IRepositorio<HistoricoClinico> historicoRepo,
            IRepositorio<Agendamento> agendamentoRepo,
            IRepositorio<Pet> petRepo)
        {
            _historicoRepo = historicoRepo;
            _agendamentoRepo = agendamentoRepo;
            _petRepo = petRepo;
        }

        public async Task<int> RegistrarHistoricoAsync(HistoricoClinico historico, int idFuncionario)
        {
            var pet = await _petRepo.ObterPorIdAsync(historico.IdPet);
            if (pet == null)
                throw new ArgumentException("Pet não encontrado.");

            if (string.IsNullOrWhiteSpace(historico.Descricao))
                throw new ArgumentException("A descrição do atendimento é obrigatória.");

            historico.DataAtendimento = DateTime.Now;
            historico.IdFuncionario = idFuncionario; // Quem registra é sempre o profissional autenticado
            if (historico.IdAgendamento <= 0) historico.IdAgendamento = null;

            // 1. Salva o registro clínico no banco
            var id = await _historicoRepo.AdicionarAsync(historico);

            // 2. UC16: Conclui o agendamento automaticamente após a consulta/serviço
            if (historico.IdAgendamento.HasValue)
            {
                var agendamento = await _agendamentoRepo.ObterPorIdAsync(historico.IdAgendamento.Value);

                // Chama o método encapsulado da classe Agendamento (só conclui o que está confirmado — RN04)
                if (agendamento != null && agendamento.Status == StatusAgendamento.CONFIRMADO)
                {
                    agendamento.Concluir();
                    await _agendamentoRepo.AtualizarAsync(agendamento);
                }
            }

            return id;
        }

        // idTutorSolicitante é informado quando quem consulta é um Tutor: ele só vê o histórico dos próprios pets
        public async Task<IEnumerable<HistoricoClinico>> ObterHistoricoPorPetAsync(int idPet, int? idTutorSolicitante = null)
        {
            var pet = await _petRepo.ObterPorIdAsync(idPet);
            if (pet == null)
                throw new ArgumentException("Pet não encontrado.");

            if (idTutorSolicitante.HasValue && pet.TutorId != idTutorSolicitante.Value)
                throw new UnauthorizedAccessException("Este pet não pertence ao tutor autenticado.");

            var historicos = await _historicoRepo.BuscarAsync(h => h.IdPet == idPet);
            return historicos.OrderByDescending(h => h.DataAtendimento);
        }
    }
}
