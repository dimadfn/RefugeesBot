using TelegramBot;

namespace RefugeesBot
{
    public class Programm
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            var cts = new CancellationTokenSource();
            new ChannelHandler(cts);

            Console.WriteLine("Start listening ");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();

        }
    }
}
