// See https://aka.ms/new-console-template for more information

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using var cts = new CancellationTokenSource();
Console.WriteLine("Hello, World!");


//var urlString = "https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}";
//var apiToken = "5304162311:AAG4ngGCK5Kaf9BglhztXAsTl4xt2eF1S1U";
var chatId = "@YorickTest";
//var text = "Hello world!";
//urlString = string.Format(urlString, apiToken, chatId, text);
//var request = WebRequest.Create(urlString);
//var rs = request.GetResponse().GetResponseStream();
//var reader = new StreamReader(rs);
//var line = "";
//var sb = new StringBuilder();
//while (line != null)
//{
//    line = reader.ReadLine();
//    if (line != null)
//        sb.Append(line);
//}

//var response = sb.ToString();
//Console.WriteLine(response);

var botClient = new TelegramBotClient("5304162311:AAG4ngGCK5Kaf9BglhztXAsTl4xt2eF1S1U");
var receiverOptions = new ReceiverOptions();
botClient.StartReceiving(
    HandleUpdateAsync,
    HandleErrorAsync,
    receiverOptions,
    cts.Token);


var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();


async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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
        "You said:\n" + messageText,
        replyToMessageId: update.Message.MessageId,
        cancellationToken: cancellationToken);
}

Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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

//var message = await botClient.SendTextMessageAsync(
//    chatId,
//    "Trying *all the parameters* of `sendMessage` method",
//    ParseMode.MarkdownV2,
//    disableNotification: true,
//    replyToMessageId: update.Message.MessageId,
//    replyMarkup: new InlineKeyboardMarkup(
//        InlineKeyboardButton.WithUrl(
//            "Check sendMessage method",
//            "https://core.telegram.org/bots/api#sendmessage")),
//    cancellationToken: cancellationToken);

//var me = await botClient.GetMeAsync();
//Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
