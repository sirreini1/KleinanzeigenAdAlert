using System.Text.RegularExpressions;
using KleinanzeigenAdAlert.core.Kleinanzeigen;
using KleinanzeigenAdAlert.DB.repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KleinanzeigenAdAlert.core.Telegram.Commands;

public partial class WatchCommand(
    TelegramBotClient bot,
    IFlatAdRepository flatAdRepository,
    ILogger<TelegramCommand> logger)
    : TelegramCommand(bot, WatchRegex(), logger)
{
    private readonly TelegramBotClient _bot = bot;

    protected override async Task HandleCommand(Message msg, UpdateType updateType)
    {
        var telegramUser = msg.From!.Id.ToString();
        var url = msg.Text!.Split(" ")[1];

        var flatAds = await AdExtractor.GetAdsFromUrl(url);
        if (flatAdRepository.EntriesForURlAndUserExist(url, telegramUser))
        {
            await _bot.SendMessage(msg.Chat, "Already watching ads for this url",
                linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true });
        }
        else
        {
            flatAdRepository.UpsertFlatAds(flatAds, telegramUser);
            Console.WriteLine("Starting to watch ads from " + url);
            await _bot.SendMessage(msg.Chat, "Starting to watch ads from " + url,
                linkPreviewOptions: new LinkPreviewOptions { IsDisabled = true });
        }
    }

    [GeneratedRegex("/watch", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex WatchRegex();
}