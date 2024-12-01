using System.Text.RegularExpressions;
using KleinanzeigenAdAlert.core.Kleinanzeigen;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;


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
        var application = await GenerateApplicationText(description);
        await _bot.SendMessage(msg.Chat, application);
    }

    [GeneratedRegex("/genApplication", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex WatchRegex();
    
    private static async Task<string> GenerateApplicationText(string description)
    {
        var promptBase = await LoadPrompt();
        ChatClient client = new(model: "gpt-4o-mini", apiKey: Config.OpenAIKey);
        var fullPrompt = promptBase + description;
        ChatCompletion completion = await client.CompleteChatAsync(fullPrompt);
        return completion.Content.FirstOrDefault()?.Text ?? "Error";
    }
    
    private static async Task<string> LoadPrompt()
    {
        return await File.ReadAllTextAsync(Config.PromptFilePath);
    }
}