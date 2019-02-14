using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UntisNotifier.Abstractions.NotifyService;
using UntisNotifier.Console;
using UntisNotifier.Telegram;

namespace UntisNotifier
{
    public class Configurator
    {

        private JObject _config;

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
            
            InitNotifiers();
        }

        private void InitNotifiers()
        {
            InitConsoleNotifier();
            InitTelegramNotifier();
        }

        private void InitConsoleNotifier()
        {
            if (_config.ContainsKey("notifiers") && _config["notifiers"].ToObject<JObject>().ContainsKey("Console"))
            {
                Notifiers.Add(new ConsoleNotifier());
            }
        }
        private void InitTelegramNotifier()
        {
            if (_config.ContainsKey("notifiers") && _config["notifiers"].ToObject<JObject>().ContainsKey("Telegram"))
            {
                Notifiers.Add(new TelegramNotifier());
            }
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