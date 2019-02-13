using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UntisNotifier
{
    public class Configurator
    {

        private JObject _config;
        
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