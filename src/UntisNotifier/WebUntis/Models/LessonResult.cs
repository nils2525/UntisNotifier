using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace UntisNotifier.WebUntis.Models
{
    public class LessonResult
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("lessonId")]
        public int LessonId { get; set; }

        [JsonProperty("date")]
        public int DateInt { get; set; }

        [JsonIgnore]
        public DateTime Date
        {
            get
            {
                var provider = CultureInfo.InvariantCulture;
                return DateTime.ParseExact(DateInt.ToString(), "yyyyMMdd", provider);
            }
        }

        public int Hour
        {
            get
            {
                switch (StartTime)
                {
                    case 750:
                        return 1;
                    case 835:
                        return 2;
                    case 940:
                        return 3;
                    case 1025:
                        return 4;
                    case 1130:
                        return 5;
                    case 1215:
                        return 6;
                    case 1315:
                        return 7;
                    case 1400:
                        return 8;
                    default:
                        return -1;

                }
            }
        }

        [JsonProperty("startTime")]
        public int StartTime { get; set; }

        [JsonProperty("endTime")]
        public int EndTime { get; set; }

        [JsonProperty("hasInfo")]
        public bool HasInfo { get; set; }

        [JsonProperty("priority")]
        public int Priority { get; set; }

        [JsonProperty("is")]
        public LessonStatus Status { get; set; }

        [JsonProperty("elements")]
        public List<Element> Elements { get; set; }
    }

    public class LessonStatus
    {
        [JsonProperty("standard")]
        public bool? Standard { get; set; }

        [JsonProperty("cancelled")]
        public bool? Cancelled { get; set; }

        [JsonProperty("event")]
        public bool Event { get; set; }

        [JsonProperty("roomSubstitution")]
        public bool? RoomSubstitution { get; set; }

        [JsonProperty("exam")]
        public bool? Exam { get; set; }

        [JsonProperty("substitution")]
        public bool? Substitution { get; set; }
    }

    public class Element
    {
        [JsonProperty("type")]
        public ElementType Type { get; set; }

        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("longName")]
        public string LongName { get; set; }

        [JsonProperty("state")]
        public ElementState State { get; set; }
    }

    public enum ElementState
    {
        Regular,
        Substituted
    }

    public enum ElementType
    {
        None,
        Class,
        Teacher,
        Subject,
        Student
    }
}

