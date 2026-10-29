using System;

namespace PataFacil.Models
{
    // E-mail automático gerado pelo sistema (RF09). Fica guardado para a Caixa de E-mail de cada usuário.
    public class EmailEnviado
    {
        public int Id { get; set; }
        public string Destinatario { get; set; } = string.Empty;
        public string Assunto { get; set; } = string.Empty;
        public string Corpo { get; set; } = string.Empty;
        public DateTime DataEnvio { get; set; } = DateTime.Now;
        public bool Lida { get; set; }
    }
}
