using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using UntisNotifier.Abstractions.Models;
using UntisNotifier.Abstractions.NotifyService;

namespace UntisNotifier.Telegram
{
    public class TelegramNotifier : INotifyService
    {
        public bool Notify(IEnumerable<Lesson> lessons)
        {
            //Login to telegram if not initialized
            if (!TelegramClient.Instance.IsInitialized)
            {
                var result = Task.Run(() => TelegramClient.Instance.InitClientAsync("")).Result;
                if (!result)
                {
                    return false;
                }
            }

            //Create message
            var messages = MessageCreator.CreateUserFriendlyMessage(lessons);
            var finalMessage = String.Join("\n", messages);

            //Send message to telegram client
            return Task.Run(() => TelegramClient.Instance.SendMessageAsync(finalMessage, 9999999)).Result;
        }
    }
}
