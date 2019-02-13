using System;
using System.Collections.Generic;
using UntisNotifier.Abstractions.Models;
using UntisNotifier.Abstractions.NotifyService;

namespace UntisNotifier.Console
{
    public class ConsoleNotifier : INotifyService
    {
        public bool Notify(IEnumerable<Lesson> lessons)
        {
            var messages = MessageCreator.CreateUserFriendlyMessage(lessons);
            foreach (var message in messages)
            {
                System.Console.WriteLine(message);
            }

            return true;
        }
    }
}