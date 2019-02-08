using System;
using System.Threading.Tasks;

namespace UntisNotifier
{
    class Program
    {
        //args constructed
        //args[0] username
        //args[1] password
        //args[2] school
        //args[3] --notifiers
        //args[3+n] {notifier name}
        static async Task Main(string[] args)
        {
            var client = new WebUntis.Client(args[0], args[1], args[2], "https://mese.webuntis.com/WebUntis");
            var isLoggedIn = await client.LoginAsync();
            if (isLoggedIn)
            {
                var lessons = await client.GetChangedLessons();
            }
        }
    }
}