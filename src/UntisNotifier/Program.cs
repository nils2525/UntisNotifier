using System;
using System.IO;
using System.Threading.Tasks;
using UntisNotifier.Abstractions.NotifyService;

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
            }
        }
    }
}