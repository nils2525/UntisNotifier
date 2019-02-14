using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace UntisNotifier
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configurator = new Configurator(Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData),
                    "UntisNotifier",
                    "UntisNotifier.json"));
            
            var client = configurator.GetWebUntisClient();
            var isLoggedIn = await client.LoginAsync();
            if (isLoggedIn)
            {
                var lessons = await client.GetChangedLessons();
                if (lessons != null)
                {
                    var tasks = new List<Task>();
                    configurator.Notifiers.ForEach(n => tasks.Add(Task.Factory.StartNew(() => n.Notify(lessons))));
                    await Task.WhenAll(tasks.ToArray());
                }
            }
        }
    }
}