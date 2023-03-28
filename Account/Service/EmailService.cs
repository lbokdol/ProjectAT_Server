using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MailKit.Net.Smtp;
using MimeKit;

namespace AccountSpace.Service
{
    public class EmailService
    {
        private string _smtpHost;
        private int _smtpPort;
        private string _email;
        private string _password;

        public EmailService(string smtpHost, int smtpPort, string email, string password)
        {
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
            _email = email;
            _password = password;
        }

        public void SendEmail(string to, string username, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Bokdol2Studio", _email));
            message.To.Add(new MailboxAddress(Encoding.UTF8, username, to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                client.Connect(_smtpHost, _smtpPort, false);
                client.Authenticate(_email, _password);
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
