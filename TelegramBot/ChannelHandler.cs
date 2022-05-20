using System.Net;
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
        private readonly bool _repeatSuggestionToSameUser;
        private Dictionary<string, uint> _alreadySuggested = new(10000);
        private List<DictionaryItem> _rulesDictionary = null;
        private uint lastSuggested = 0;

        public ChannelHandler(CancellationTokenSource cts, bool repeatSuggestionToSameUser)
        {
            //var rules = File.ReadAllText("Dictionaries.json");
            //var rules = new WebClient().DownloadString("https://drive.google.com/uc?export=download&id=1f6PJtPOuc31oRYcKp9AdMVf2apWi7acU");
            //_rulesDictionary = JsonSerializer.Deserialize<List<DictionaryItem>>(rules);

            var rules = File.ReadAllText("Dictionaries.ru.json");
            _rulesDictionary = JsonSerializer.Deserialize<List<DictionaryItem>>(rules);

            rules = File.ReadAllText("Dictionaries.ua.json");
            _rulesDictionary.AddRange(JsonSerializer.Deserialize<List<DictionaryItem>>(rules));

            _repeatSuggestionToSameUser = repeatSuggestionToSameUser;
            var botClient = new TelegramBotClient("5304162311:AAG4ngGCK5Kaf9BglhztXAsTl4xt2eF1S1U");
            var receiverOptions = new ReceiverOptions();
            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cts.Token);
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
            if (update.Message.ReplyToMessage != null)
                return;

            //var rules = new WebClient().DownloadString("https://drive.google.com/uc?export=download&id=1f6PJtPOuc31oRYcKp9AdMVf2apWi7acU");

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text.ToLower();
            messageText = messageText.Replace("ё", "е");

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
            Suggestion message = null;

            foreach (var rule in _rulesDictionary)
            {
                if (rule.NotKeywords.Any(_ => messageText.Contains(_)))
                    continue;

                if (rule.MandatoryKeywords.Any() && rule.MandatoryKeywords.All(_ => messageText.Contains(_)))
                    message = new Suggestion(rule);

                if (message != null)
                    break;

                var matchCount = 0;
                foreach (var keyword in rule.OptionalKeywords)
                {
                    if (messageText.Contains(keyword))
                        matchCount++;
                    if (matchCount >= rule.AtLeastCount)
                    {
                        message = new Suggestion(rule);
                        break;
                    }
                }
            }

            if (message != null)
            {
                if (!_repeatSuggestionToSameUser && update.Message.From != null &&
                    _alreadySuggested.ContainsKey(message.RuleName + "_" + update.Message.From.Id))
                    return;

                try
                {
                    await botClient.SendTextMessageAsync(chatId, message.Message,
                        replyToMessageId: update.Message.MessageId,
                        disableWebPagePreview: true,
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken);

                    _alreadySuggested.TryAdd(message.RuleName + "_" + update.Message.From.Id, lastSuggested++);
                    if (lastSuggested > 9000)
                    {
                        var asl = _alreadySuggested.ToList();
                        var maxVal = asl.Max(x => x.Value) - 100;

                        _alreadySuggested = asl.Where(_ => _.Value > maxVal)
                            .ToDictionary(key => key.Key, val => (uint)0);
                        lastSuggested = 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message} - {ex?.InnerException}");

                    return;
                }

            }
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
