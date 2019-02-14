using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using UntisNotifier.Abstractions.Models;
using UntisNotifier.Abstractions.NotifyService;

/*
 Note for using gmail allow less secure apps has to be activated
 https://support.google.com/accounts/answer/6010255?hl=de
 Config:
"email" : {
  "active" : "true",
  "fromEmail" : "email",
  "toEmail" : "email",
  "password" : "password",
  "smtpServer" : "server",
  "smtpPort" : "port"
}
 */

namespace UntisNotifier.Email
{
    public class EmailNotifier : INotifyService
    {
        private readonly MailAddress _from;
        private readonly MailAddress _to;
        private readonly string _password;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly SmtpClient _smtpClient;

        public EmailNotifier(MailAddress from, MailAddress to, string password, string smtpServer, int smtpPort)
        {
            _from = from;
            _to = to;
            _password = password;
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpClient = CreateSmtpClient();
        }

        public bool Notify(IEnumerable<Lesson> lessons)
        {
            using (var message = new MailMessage(_from, _to)
            {
                Subject = "UntisNotifier",
                Body = MessageCreator
                    .CreateUserFriendlyMessage(lessons)
                    .Aggregate((longest, next) => longest + Environment.NewLine + next)
            })
            {
                try
                {
                    _smtpClient.Send(message);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                    return false;
                }
            }
            return true;
        }

        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient
            {
                Host = _smtpServer,
                Port = _smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_from.Address, _password)
            };
        }
    }
}