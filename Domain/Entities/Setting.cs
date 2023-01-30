using System.Collections.Generic;

namespace Domain.Entities
{
    public class Setting
    {
        public class Secret
        {
            public string Chave { get; set; }
        }

        public class EmailSettings
        {
            public const string SectionName = "Email";
            public string NomeUsuarioEmail { get; set; }
            public string Host { get; set; }
            public int Port { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class MailRequest
        {
            public string ToEmail { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
        }
    }
}
