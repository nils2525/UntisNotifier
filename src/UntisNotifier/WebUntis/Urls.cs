using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UntisNotifier.WebUntis
{
    public class Urls
    {
        private readonly string _baseUrl;

        public Urls(string baseUrl)
        {
            _baseUrl = baseUrl;

            if (!_baseUrl.EndsWith("/"))
            {
                _baseUrl = _baseUrl + "/";
            }
        }
        
        public string LoginUri => _baseUrl + "j_spring_security_check";

        public string WeeklyDataUri => _baseUrl + "api/public/timetable/weekly/data";
    }
}
