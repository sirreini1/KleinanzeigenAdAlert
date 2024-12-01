using System.Text.RegularExpressions;
using KleinanzeigenAdAlert.core.Kleinanzeigen;
using KleinanzeigenAdAlert.DB.repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KleinanzeigenAdAlert.core.Telegram.Commands;

public partial class GenerateApplicationCommand(
    TelegramBotClient bot,
    ILogger<TelegramCommand> logger)
    : TelegramCommand(bot, WatchRegex(), logger)
{
    private readonly TelegramBotClient _bot = bot;

    protected override async Task HandleCommand(Message msg, UpdateType updateType)
    {
        var url = msg.Text!.Split(" ")[1];
        var description = await FullDescriptionExtractor.GetDescriptionFromUrl(url);
        
        await _bot.SendMessage(msg.Chat, description);
    }

    [GeneratedRegex("/genApplication", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex WatchRegex();
}