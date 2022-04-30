using System.Text;
using TelegramBot;

namespace RefugeesBot
{
    public class Programm
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            bool rememberUser = args.Any(_ => _ == "debug");

            Console.WriteLine($"Bot run in [debug={rememberUser}] mode.");

            var cts = new CancellationTokenSource();
            new ChannelHandler(cts, rememberUser);
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Start listening ");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();

        }
    }
}
