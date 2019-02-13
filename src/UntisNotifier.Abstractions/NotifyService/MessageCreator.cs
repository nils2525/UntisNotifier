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
            var cancelledLessons = lessons.Where(l => l.Status.Cancelled == true && l.Date.Date >= DateTime.Today.Date).ToList();
            //All non default lessons
            var nonDefaultLessons = lessons.Where(l => l.Status.Standard == null && l.Date.Date >= DateTime.Today.Date).ToList();

            var messages = new List<string>();
            
            //Write user-friendly message
            foreach (var cancelledLesson in cancelledLessons)
            {
                var messageString = "";
                if (cancelledLesson.Date.Date == DateTime.Today.Date)
                {
                    messageString = "Heute";
                }
                else
                {
                    messageString = "Am " + cancelledLesson.Date.ToString("dd.MM.yyyy");
                }

                messageString = messageString + " entfällt die " + cancelledLesson.Hour + " Std. (Fach " + cancelledLesson.Elements.Where(c => c.Type == ElementType.Subject).FirstOrDefault().LongName + ")";
                messages.Add(messageString);
            }

            //Write user-friendly message
            foreach (var nonDefaultLesson in nonDefaultLessons)
            {
                var messageString = "";
                if (nonDefaultLesson.Date.Date == DateTime.Today.Date)
                {
                    messageString = "Heute";
                }
                else
                {
                    messageString = "Am " + nonDefaultLesson.Date.ToString("dd.MM.yyyy");
                }

                messageString = messageString + " wird die " + nonDefaultLesson.Hour + " Std. (Fach " + nonDefaultLesson.Elements.Where(c => c.Type == ElementType.Subject).FirstOrDefault().LongName + ") vertreten";
                messages.Add(messageString);
            }

            return messages;
        }
    }
}