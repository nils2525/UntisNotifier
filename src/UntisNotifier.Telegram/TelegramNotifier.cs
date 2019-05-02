using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using UntisNotifier.Abstractions.Models;
using UntisNotifier.Abstractions.NotifyService;

/*
 "telegram" : {
	  "active" : "true",
      "token" : "token",
      "chatId" : "chatId"
}
 */

namespace UntisNotifier.Telegram
{
    public class TelegramNotifier : INotifyService
    {
        private readonly string _token;
        private readonly int _chatId;

        public TelegramNotifier(string token, int chatId)
        {
            _token = token;
            _chatId = chatId;
        }
        
        public bool Notify(IEnumerable<Lesson> lessons)
        {
            //Login to telegram if not initialized
            if (!TelegramClient.Instance.IsInitialized)
            {
                var result = Task.Run(() => TelegramClient.Instance.InitClientAsync(_token)).Result;
                if (!result)
                {
                    return false;
                }
            }

            //Create message
            var messages = MessageCreator.CreateUserFriendlyMessage(lessons);
            foreach(var message in messages)
            {
                //Send message to telegram client
                Task.Run(() => TelegramClient.Instance.SendMessageAsync(message, _chatId)).Wait();
            }
            return true;
        }
    }
}
