using System;
using System.Threading.Tasks;

namespace UntisNotifier
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new WebUntis.Client("user", "password", "c-severing-bk", "https://mese.webuntis.com/WebUntis");
            await client.LoginAsync();
        }
    }
}
