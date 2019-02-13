using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UntisNotifier.Abstractions.Models;

namespace UntisNotifier.WebUntis
{
    public class Client
    {
        
        private const string InvalidCredentialsError = "\"loginError\":\"Invalid user name and/or password\"";
        
        /// <summary>
        /// Untis Username
        /// </summary>
        private readonly string _username;
        /// <summary>
        /// Password
        /// </summary>
        private readonly string _password;
        /// <summary>
        /// School name
        /// </summary>
        private readonly string _school;

        /// <summary>
        /// Cookies, send with every request
        /// </summary>
        private CookieContainer _cookies;
        /// <summary>
        /// Http client
        /// </summary>
        private HttpClient _client;
        /// <summary>
        /// Client handler (includes cookies)
        /// </summary>
        private HttpClientHandler _clientHandler;

        /// <summary>
        /// Used to get correct url's
        /// </summary>
        private Urls _urls;

        /// <summary>
        /// True = logged in
        /// </summary>
        public bool IsLoggedIn { get; private set; } = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="school"></param>
        /// <param name="baseUrl">WebUntis Url without any query e.g. https://mese.webuntis.com/WebUntis</param>
        public Client(string username, string password, string school, string baseUrl)
        {
            _username = username;
            _password = password;
            _school = school;
            _urls = new Urls(baseUrl);

            //Init http client
            _cookies = new CookieContainer();
            _clientHandler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = _cookies
            };
            _client = new HttpClient(_clientHandler);
        }

        /// <summary>
        /// Login with credentials
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LoginAsync()
        {
            var bodyContent = new Dictionary<string, string>()
            {
                {"school", _school },
                {"j_username", _username },
                {"j_password", _password },
                {"token", "" }
            };

            HttpResponseMessage response;
            using (var content = new FormUrlEncodedContent(bodyContent))
            {
                //Send login request
                response = await _client.PostAsync(_urls.LoginUri, content);
            }
            var res = await response.Content.ReadAsStringAsync();

            //the website will return a successful status code if login failed
            //so we got the opportunity to look in returned html for error --> "loginError":"Invalid user name and/or password" (InvalidCredentialsError)
            if (res != null && !res.Contains(InvalidCredentialsError))
            {
                //Logged in!
                return IsLoggedIn = true;
            }
            return IsLoggedIn = false;
        }

        /// <summary>
        /// Get Lessons for current week
        /// </summary>
        /// <returns></returns>
        public async Task<List<Lesson>> GetWeeklyLessonsAsync()
        {
            if (IsLoggedIn)
            {
                //parsed DateString for today
                var currentWeekDate = DateTime.Now.ToString("yyyy-MM-dd");

                //Get raw data
                var request = await _client.GetAsync(_urls.WeeklyDataUri + $"?elementType=1&elementId=972&date={currentWeekDate}&formatId=1");
                if (request.IsSuccessStatusCode)
                {
                    //Parse data
                    var jsonString = await request.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(jsonString);

                    var rawLessons = jsonObject["data"]["result"]["data"]["elementPeriods"]["972"];
                    var parsedLessons = JsonConvert.DeserializeObject<List<Lesson>>(rawLessons.ToString());

                    var rawElements = jsonObject["data"]["result"]["data"]["elements"];
                    var parsedElements = JsonConvert.DeserializeObject<List<Element>>(rawElements.ToString());


                    foreach (var lesson in parsedLessons)
                    {
                        foreach (var element in lesson.Elements)
                        {
                            var fullElement = parsedElements.Where(e => element.ID == e.ID).FirstOrDefault();
                            if (fullElement != null)
                            {
                                element.Name = fullElement.Name;
                                element.LongName = fullElement.LongName;
                            }
                        }
                    }

                    return parsedLessons;
                }
            }
            return null;
        }

        /// <summary>
        /// Get all lessons that are not normal
        /// </summary>
        /// <param name="lessons"></param>
        /// <returns></returns>
        public async Task<List<Lesson>> GetChangedLessons(List<Lesson> lessons = null)
        {
            if (lessons == null)
            {
                lessons = await GetWeeklyLessonsAsync();
                if (lessons == null)
                {
                    //Something went wrong...
                    return null;
                }
            }

            return lessons
                .Where(l => (l.Status.Cancelled  == true || l.Status.Standard == null) && 
                            l.Date.Date >= DateTime.Today.Date).ToList();
        }
    }
}
