using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UntisNotifier.Abstractions.NotifyService;
using UntisNotifier.Console;
using UntisNotifier.Email;
using UntisNotifier.Telegram;

namespace UntisNotifier
{
    public class Configurator
    {
        private JObject _config;
        private JObject _notifiers;

        public List<INotifyService> Notifiers { get; private set; } = new List<INotifyService>();

        public Configurator(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));
            }

            var readJson = File.ReadAllText(filePath);
            if (readJson == null)
            {
                throw new ArgumentNullException(nameof(readJson));
            }

            _config = JsonConvert.DeserializeObject<JObject>(readJson);

            if (_config.ContainsKey("notifiers"))
            {
                _notifiers = _config["notifiers"].ToObject<JObject>();
            }

            if (_notifiers != null)
            {
                InitNotifiers();
            }
        }

        private void InitNotifiers()
        {
            InitConsoleNotifier();
            InitEmailNotifier();
            InitTelegramNotifier();
        }

        private void InitConsoleNotifier()
        {
            if (IsNotifierActive("console"))
            {
                Notifiers.Add(new ConsoleNotifier());
            }
        }

        private void InitTelegramNotifier()
        {
            if (!IsNotifierActive("telegram")) return;

            if (_notifiers["telegram"] is JObject telegram)
            {
                Notifiers.Add(new TelegramNotifier(
                    telegram["token"].Value<string>(),
                    telegram["chatId"].Value<int>()
                    ));
            }
        }


        private void InitEmailNotifier()
        {
            if (!IsNotifierActive("email")) return;

            if (_notifiers["email"] is JObject notifier)
                Notifiers.Add(new EmailNotifier(
                    new MailAddress(notifier["fromEmail"].Value<string>()),
                    new MailAddress(notifier["toEmail"].Value<string>()),
                    notifier["password"].Value<string>(),
                    notifier["smtpServer"].Value<string>(),
                    notifier["smtpPort"].Value<int>()
                ));
        }

        private bool IsNotifierActive(string notifier)
        {
            return _notifiers.ContainsKey((notifier)) &&
                   ((JObject) _notifiers[notifier]).SelectToken("active").Value<bool>();
        }

        public WebUntis.Client GetWebUntisClient()
        {
            var userObj = _config["user"];
            return new WebUntis.Client(
                userObj["name"].ToString(),
                userObj["password"].ToString(),
                userObj["school"].ToString(),
                "https://mese.webuntis.com/WebUntis");
        }
    }
}