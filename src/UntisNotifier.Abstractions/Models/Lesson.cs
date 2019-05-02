using System;
using System.Collections.Generic;
using System.Text;

namespace UntisNotifier.Abstractions.Models
{
    public class Lesson
    {
        public int ID { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int SchoolHour { get; set; }

        public string Name { get; set; }
        public string FullName { get; set; }

        public LessonStatus LessonStatus { get; set; }

        public string Room { get; set; }
        public bool RoomIsAbnormal { get; set; }

        public string Teacher { get; set; }
        public string FullTeacherName { get; set; }
        public bool TeacherIsAbnormal { get; set; }
    }

    public enum LessonStatus
    {
        Normal,
        Exam,
        Canceled,
        Event
    }
}
