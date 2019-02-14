using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UntisNotifier.Telegram
{
    /// <summary>
    /// Telegram Clinet
    /// </summary>
    public class TelegramClient
    {
        /// <summary>
        /// Base Url
        /// </summary>
        private const string _defaultBaseUri = "https://api.telegram.org/";
        
        /// <summary>
        /// Base Url
        /// </summary>
        private string _baseUri;

        private HttpClient _httpClient;
        /// <summary>
        /// Token to use with telegram
        /// </summary>
        private string _token;

        /// <summary>
        /// True = Telegram token valid
        /// </summary>
        public bool IsInitialized { get; private set; }


        private TelegramClient()
        {
            _httpClient = new HttpClient();
        }

        private static TelegramClient _instance;
        public static TelegramClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TelegramClient();
                }

                return _instance;
            }
        }


        public async Task<bool> InitClientAsync(string token)
        {
            if (!String.IsNullOrWhiteSpace(token))
            {                
                //Test Token
                var response = await _httpClient.GetAsync(_defaultBaseUri + "bot" + token + "/getme");
                if (response.IsSuccessStatusCode)
                {
                    var content = JObject.Parse(await response.Content.ReadAsStringAsync());
                    //If value "ok" is true, token is valid
                    if (content["ok"]?.ToString() == Boolean.TrueString)
                    {
                        //Set token and base uri
                        _token = token;
                        _baseUri = _defaultBaseUri + "bot" + token + "/";
                        return IsInitialized = true;
                    }
                }
            }
            return IsInitialized = false;
        }

        /// <summary>
        /// Send single message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<bool> SendMessageAsync(string message, int chatID)
        {
            if (IsInitialized && !String.IsNullOrWhiteSpace(message) && chatID > 0)
            {
                var encodedMessage = Uri.EscapeDataString(message);
                var telegramResult = await _httpClient.GetAsync(_baseUri + "sendMessage?chat_id=" + chatID + "&text=" + encodedMessage);
            }
            return false;
        }
    }
}
