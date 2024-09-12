using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Converters;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Helpers;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.Passport;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Extensions.Polling;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;

// 
//      ПЕРЕМЕННЫЕ ДЛЯ ИСПОЛЬЗОВАНИЯ В КОДЕ
// 

var botClient = new TelegramBotClient("hidden");

var me = await botClient.GetMeAsync();

var _start = new ReplyKeyboardMarkup(
    new KeyboardButton[][]
    {
        new KeyboardButton[] {"Message ✍️", "Sticker 🗽", "Image 🌅"},
        new KeyboardButton[] {"Audio 🎧", "Voice 🎙"},
        new KeyboardButton[] {"Video 🎞", "Video Note 📹"},
        new KeyboardButton[] {"Album"},
        new KeyboardButton[] {"Document 📄", "GIF"},
        new KeyboardButton[] {"Poll 📊", "Interview"}
    }
)
{
    ResizeKeyboard = true
};

using var cts = new CancellationTokenSource();

// 
//      НАЧИНАЕМ РАБОТУ БОТА
// 

botClient.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cts.Token);

Console.WriteLine($"Start listening for @{me.Username}");

// Останавливает бот при нажатии Enter
Console.ReadLine();
cts.Cancel();


// 
//      МЕТОДЫ
// 

Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _                                       => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Type != UpdateType.Message)
        return;
    if (update.Message.Type != MessageType.Text)
        return;

    var chatId = update.Message.Chat.Id;
    
    Console.WriteLine($"Received a '{update.Message.Text}' message in chat {chatId}.");

    switch (update.Message.Text)
    {
        case "/start":
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "*HEY!* 🤙\n\n Приветсвую тебя, мой дорогой друг, в своём тестовом боте 🤪, который покажет что я могу сделать для тебя. 😏 \n\n Сейчас у тебя снизу 👇 есть кнопки. Выбири то, что ты хочешь получить, тем самым посмотрев работоспособность бота!\n\n _P.S. если нет кнопки -- отправляй /start_",
                parseMode: ParseMode.Markdown,
                replyToMessageId: update.Message.MessageId,
                replyMarkup: _start
            );
            break;

        case "Message ✍️":
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Ну а что ты еще ожидал от обычного сообщения))",
                replyMarkup: new ReplyKeyboardMarkup ("Главное меню")
            );
            break;

        case "Sticker 🗽":
            await botClient.SendStickerAsync(
                chatId: chatId,
                sticker: "https://github.com/TelegramBots/book/raw/master/src/docs/sticker-fred.webp",
                replyMarkup: new ReplyKeyboardMarkup ("Главное меню")
            );
            break;

        case "Image 🌅":
            await botClient.SendPhotoAsync(
                chatId: chatId,
                photo: System.IO.File.OpenRead("src/photo.jpg"),
                caption: "Это я. Скажи, красивый?))",
                replyMarkup: new ReplyKeyboardMarkup ("Главное меню")
            );
            break;

        case "Audio 🎧":
            await botClient.SendAudioAsync(
                chatId: chatId,
                audio: System.IO.File.OpenRead("src/01 - River.mp3"),
                caption: "Крутая песня",
                replyMarkup: new ReplyKeyboardMarkup ("Главное меню")
            );
            break;

        case "Voice 🎙":
            await botClient.SendVoiceAsync(
                chatId: chatId,
                voice: System.IO.File.OpenRead("src/Record.ogg"),
                replyMarkup: new ReplyKeyboardMarkup ("Главное меню"),
                duration: 10
            );
            break;

        case "Video 🎞":
            await botClient.SendVideoAsync(
                chatId: chatId,
                video: System.IO.File.OpenRead("src/video.mp4"),
                replyMarkup: new ReplyKeyboardMarkup ("Главное меню")
            );
            break;

        case "Video Note 📹":
            await botClient.SendVideoNoteAsync(
                chatId: chatId,
                videoNote: System.IO.File.OpenRead("src/note.mp4"),
                length: 360,
                replyMarkup: new ReplyKeyboardMarkup ("Главное меню")
            );
            break;

        case "Album":
            await botClient.SendMediaGroupAsync(
                chatId: chatId,
                media: new IAlbumInputMedia[]
                {
                    new InputMediaPhoto("https://cdn.pixabay.com/photo/2017/06/20/19/22/fuchs-2424369_640.jpg"),
                    new InputMediaPhoto("https://cdn.pixabay.com/photo/2017/04/11/21/34/giraffe-2222908_640.jpg")
                }
            );
            break;

        case "Document 📄":
            InputOnlineFile inputOnlineFile = new InputOnlineFile(System.IO.File.OpenRead("src/text.txt"), "text.txt");
            await botClient.SendDocumentAsync(
                chatId: chatId,
                document: inputOnlineFile,
                replyMarkup: new ReplyKeyboardMarkup ("Главное меню")
            );
            break;

        case "GIF":
            await botClient.SendAnimationAsync(
                chatId: chatId,
                animation: "https://raw.githubusercontent.com/TelegramBots/book/master/src/docs/video-waves.mp4",
                replyMarkup: new ReplyKeyboardMarkup ("Главное меню")
            );
            break;

        case "Poll 📊":
            await botClient.SendPollAsync(
                chatId: chatId,
                question: "Я долго писал этого бота?",
                options: new []
                {
                    "<2","2<x<4", ">4"
                },
                correctOptionId: 1,
                replyMarkup: new ReplyKeyboardMarkup ("Главное меню")
            );
            break;
        
        case "Главное меню":
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Выбери что-то из пунктов ниже",
                parseMode: ParseMode.Markdown,
                replyToMessageId: update.Message.MessageId,
                replyMarkup: _start
            );
            break;

        case "Interview":
            await botClient.SendVideoAsync(
                chatId: chatId,
                video: System.IO.File.OpenRead("src/inter.mp4"),
                replyMarkup: new ReplyKeyboardMarkup ("Главное меню")
            );
            break;

        default:
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Прости, в боте еще не поддержана функция общения. Это очень сложно, так что извиняй! Выбери что-то из списка ниже",
                replyMarkup: _start
            );
            break;
    }
}
