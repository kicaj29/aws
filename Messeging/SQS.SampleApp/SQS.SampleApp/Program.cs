using System;
using System.Threading.Tasks;

namespace SQS.SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            var client = new SQSClient();
            client.Init();
            await client.SendMessageAsync("I am a message!");
            //await client.ReceiveMessage();
        }

    }
}
