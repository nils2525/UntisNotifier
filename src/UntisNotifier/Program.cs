using System;
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
                    //ToDo: Notify müsste eingentlich in verschiedenen threads laufen
                    configurator.Notifiers.ForEach(n => n.Notify(lessons));
                }
            }
        }
    }
}