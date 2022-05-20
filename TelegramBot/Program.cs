using System.Text;
using TelegramBot;

namespace RefugeesBot
{
    public class Programm
    {
        private static async Task Main(string[] args)
        {
            
            
            
            Console.WriteLine("Hello World!");
            bool repeatSuggestionToSameUser = args.Any(_ => _ == "debug");

            Console.WriteLine($"Bot run in [debug={repeatSuggestionToSameUser}] mode.");

            var cts = new CancellationTokenSource();
            new ChannelHandler(cts, repeatSuggestionToSameUser);
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Start listening ");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();

        }
    }
}
