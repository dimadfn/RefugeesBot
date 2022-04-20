using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace TelegramBot
{
    public class ChannelHandler
    {
        private readonly List<DictionaryItem> _rulesDictionary;

        public ChannelHandler(CancellationTokenSource cts)
        {
            var botClient = new TelegramBotClient("5304162311:AAG4ngGCK5Kaf9BglhztXAsTl4xt2eF1S1U");
            var receiverOptions = new ReceiverOptions();
            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cts.Token);
            var rules = File.ReadAllText("Dictionaries.json");
            _rulesDictionary = JsonSerializer.Deserialize<List<DictionaryItem>>(rules);
            //var me = await botClient.GetMeAsync();
        }


        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Type != UpdateType.Message)
                return;
            // Only process text messages
            if (update.Message!.Type != MessageType.Text)
                return;

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");


            // Echo received message text
            var sentMessage = await botClient.SendTextMessageAsync(
                chatId,
                "Добрый день! Возможно вам поможет закреплённый документ, раздел " + _rulesDictionary[0].Message +
                "   https://docs.google.com/document/d/16lVkIc58Hw288B8NXHGQYXCaavRkHERcd3_3uKjFSaQ/",
//                "You said:\n" + messageText,
                replyToMessageId: update.Message.MessageId,
                //parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
