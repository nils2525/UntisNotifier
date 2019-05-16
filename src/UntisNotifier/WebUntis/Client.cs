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
using UntisNotifier.Data;
using UntisNotifier.WebUntis.Models;

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
            System.Console.WriteLine("Login was not successfully");
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
                if(!String.IsNullOrWhiteSpace(Program.WeekDate))
                {
                    currentWeekDate = Program.WeekDate;
                }

                //Get raw data
                var request = await _client.GetAsync(_urls.WeeklyDataUri + $"?elementType=1&elementId=972&date={currentWeekDate}&formatId=1");
                if (request.IsSuccessStatusCode)
                {
                    //Parse data
                    var jsonString = await request.Content.ReadAsStringAsync();
                    var jsonObject = JObject.Parse(jsonString);

                    var rawLessons = jsonObject["data"]["result"]["data"]["elementPeriods"]["972"];
                    var parsedLessons = JsonConvert.DeserializeObject<List<LessonResult>>(rawLessons.ToString());

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
                    return ParseLessons(parsedLessons);
                }
            }
            return null;
        }

        /// <summary>
        /// Get all lessons that are not normal
        /// </summary>
        /// <param name="lessons"></param>
        /// <returns></returns>
        public async Task<List<Lesson>> GetAbnormalLessons(List<Lesson> lessons = null, bool onlyNewChanges = false, bool updateDatabase = true)
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

            var result = new List<Lesson>();
            using (var context = new DatabaseContext())
            {
                if (onlyNewChanges)
                {
                    result = lessons.Where(c => LessonHasChanged(c, context) ?? (c.LessonStatus != Abstractions.Models.LessonStatus.Normal || c.RoomIsAbnormal || c.TeacherIsAbnormal)).ToList();
                }
                else
                {
                    result = lessons.Where(c => c.LessonStatus != Abstractions.Models.LessonStatus.Normal || c.RoomIsAbnormal || c.TeacherIsAbnormal).ToList();
                }

                if (updateDatabase)
                {
                    UpdateLessonsInDB(lessons);
                }
            }

            return result;
        }


        private bool? LessonHasChanged(Lesson lesson, DatabaseContext context)
        {
            var dbLesson = context.Lessons.Where(c => c.ID == lesson.ID).FirstOrDefault();
            if (dbLesson != null)
            {
                return dbLesson.LessonStatus != lesson.LessonStatus ||
                    dbLesson.RoomIsAbnormal != lesson.RoomIsAbnormal ||
                    dbLesson.TeacherIsAbnormal != lesson.TeacherIsAbnormal;
            }
            return null;
        }

        private List<Lesson> ParseLessons(List<LessonResult> lessonResults)
        {
            var result = new List<Lesson>();
            foreach (var lesson in lessonResults)
            {
                result.Add(ParseLesson(lesson));
            }
            return result;
        }
        private Lesson ParseLesson(LessonResult lessonResult)
        {
            var startTimeString = lessonResult.StartTime.ToString().PadLeft(4, '0');
            var startHours = Int32.Parse(startTimeString.Remove(2, 2));
            var startMinutes = Int32.Parse(startTimeString.Remove(0, 2));

            var endTimeString = lessonResult.EndTime.ToString().PadLeft(4, '0');
            var endHours = Int32.Parse(endTimeString.Remove(2, 2));
            var endMinutes = Int32.Parse(endTimeString.Remove(0, 2));


            var subject = lessonResult.Elements.Where(c => c.Type == ElementType.Subject).First();
            var teacher = lessonResult.Elements.Where(c => c.Type == ElementType.Teacher).First();
            var room = lessonResult.Elements.Where(c => c.Type == ElementType.Student).First();

            Abstractions.Models.LessonStatus status = Abstractions.Models.LessonStatus.Normal;
                       
            if (lessonResult.Status.Cancelled == true)
            {
                status = Abstractions.Models.LessonStatus.Canceled;
            }
            else if (lessonResult.Status.Exam == true)
            {
                status = Abstractions.Models.LessonStatus.Exam;
            }
            else if (lessonResult.Status.Event)
            {
                status = Abstractions.Models.LessonStatus.Event;
            }
            else if (teacher.State == ElementState.Regular || lessonResult.Status.Standard == true)
            {
                status = Abstractions.Models.LessonStatus.Normal;
            }

            var lesson = new Lesson()
            {
                StartTime = lessonResult.Date.AddMinutes((startHours * 60) + startMinutes),
                EndTime = lessonResult.Date.AddMinutes((endHours * 60) + endMinutes),
                Name = subject.Name,
                FullName = subject.LongName,
                Teacher = teacher.Name,
                FullTeacherName = teacher.LongName,
                TeacherIsAbnormal = teacher.State == ElementState.Substituted,
                Room = room.Name,
                RoomFullName = room.LongName,
                RoomIsAbnormal = lessonResult.Status.RoomSubstitution ?? room.State == ElementState.Substituted,
                SchoolHour = lessonResult.Hour,
                LessonStatus = status,
                ID = lessonResult.ID
            };

            return lesson;
        }

        private void UpdateLessonsInDB(List<Lesson> lessons)
        {
            using (var context = new Data.DatabaseContext())
            {
                foreach (var lesson in lessons)
                {

                    if (!context.Lessons.Any(c => c.ID == lesson.ID))
                    {
                        context.Lessons.Add(lesson);
                    }
                    else
                    {
                        context.Lessons.Update(lesson);
                    }
                }
                context.SaveChanges();
            }
        }
    }
}