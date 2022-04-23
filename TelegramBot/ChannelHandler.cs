using System.Text;
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
            Console.OutputEncoding = Encoding.UTF8;
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
            var messageText = update.Message.Text.ToLower();
            messageText = messageText.Replace("ё", "е");

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
            var message = string.Empty;

            foreach (var rule in _rulesDictionary)
            {
                if (rule.NotKeywords.Any(_ => messageText.Contains(_)))
                    break;

                if (rule.MandatoryKeywords.Any() && rule.MandatoryKeywords.All(_ => messageText.Contains(_)))
                    message = GetMessage(rule);

                if (message != string.Empty)
                    break;

                var matchCount = 0;
                foreach (var keyword in rule.OptionalKeywords)
                {
                    if (messageText.Contains(keyword))
                        matchCount++;
                    if (matchCount >= rule.AtLeastCount)
                    {
                        message = GetMessage(rule);
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(message))
                await botClient.SendTextMessageAsync(chatId, message,
                    replyToMessageId: update.Message.MessageId,
                    //parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);
        }

        private string GetMessage(DictionaryItem msg)
        {
            return "Добрый день! Возможно вам поможет закреплённый документ, раздел " + msg.Message + " " + msg.Url;
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
