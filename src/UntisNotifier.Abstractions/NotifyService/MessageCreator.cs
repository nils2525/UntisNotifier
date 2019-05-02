using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UntisNotifier.Abstractions.Models;

namespace UntisNotifier.Abstractions.NotifyService
{
    public class MessageCreator
    {
        public static IEnumerable<string> CreateUserFriendlyMessage(IEnumerable<Lesson> lessons)
        {
            //All cancalled lessons
            var cancelledLessons = lessons.Where(l => l.LessonStatus == LessonStatus.Canceled).ToList();
            //All non default lessons
            var nonDefaultLessons = lessons.Where(l => (l.RoomIsAbnormal || l.TeacherIsAbnormal) && l.LessonStatus != LessonStatus.Canceled).ToList();
            //exams
            var exams = lessons.Where(c => c.LessonStatus == LessonStatus.Exam).ToList();

            var messages = new List<string>();
            
            //Write user-friendly message
            foreach (var cancelledLesson in cancelledLessons)
            {
                var messageString = "";
                if (cancelledLesson.StartTime.Date == DateTime.Today.Date)
                {
                    messageString = "Heute";
                }
                else
                {
                    messageString = "Am " + cancelledLesson.StartTime.ToString("dd.MM.yyyy");
                }

                messageString = messageString + " entfällt die " + cancelledLesson.SchoolHour + " Std. (Fach " + cancelledLesson.FullName + ")";
                messages.Add(messageString);
            }

            //Write user-friendly message
            foreach (var nonDefaultLesson in nonDefaultLessons)
            {
                var messageString = "";
                if (nonDefaultLesson.StartTime.Date == DateTime.Today.Date)
                {
                    messageString = "Heute";
                }
                else
                {
                    messageString = "Am " + nonDefaultLesson.StartTime.ToString("dd.MM.yyyy");
                }

                messageString = messageString + " findet die " + nonDefaultLesson.SchoolHour + " Std. (" + nonDefaultLesson.Name + ") in Raum " + nonDefaultLesson.Room + (false && !String.IsNullOrWhiteSpace(nonDefaultLesson.RoomFullName) ? (" (" +  nonDefaultLesson.RoomFullName + ")") : "") + " bei " + nonDefaultLesson.Teacher + " statt.";
                messages.Add(messageString);
            }

            foreach(var exam in exams)
            {
                var messageString = "";
                if (exam.StartTime.Date == DateTime.Today.Date)
                {
                    messageString = "Heute";
                }
                else
                {
                    messageString = "Am " + exam.StartTime.ToString("dd.MM.yyyy");
                }

                messageString = messageString + " wird ein/e Klausur/Test im Fach " + exam.Name + " in der " + exam.SchoolHour + "geschrieben.";
                messages.Add(messageString);
            }

            return messages;
        }
    }
}