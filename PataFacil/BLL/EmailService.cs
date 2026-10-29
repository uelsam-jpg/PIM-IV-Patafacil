using System;
using System.Threading.Tasks;
using PataFacil.DAL;
using PataFacil.Models;

namespace PataFacil.BLL
{
    public interface IEmailService
    {
        Task EnviarEmailAsync(string destinatario, string assunto, string corpo);
    }

    public class EmailService : IEmailService
    {
        private readonly IRepositorio<EmailEnviado> _emailRepo;

        public EmailService(IRepositorio<EmailEnviado> emailRepo)
        {
            _emailRepo = emailRepo;
        }

        public async Task EnviarEmailAsync(string destinatario, string assunto, string corpo)
        {
            // Simulador de envio de e-mail (RF09) - Em um projeto real, usaríamos o SmtpClient aqui.
            // A mensagem fica gravada no banco e aparece na Caixa de E-mail do destinatário.
            await _emailRepo.AdicionarAsync(new EmailEnviado
            {
                Destinatario = destinatario.Trim().ToLower(),
                Assunto = assunto,
                Corpo = corpo,
                DataEnvio = DateTime.Now
            });

            Console.WriteLine($"📧 [E-MAIL AUTOMÁTICO] Para: {destinatario} | Assunto: {assunto}");
        }
    }
}
