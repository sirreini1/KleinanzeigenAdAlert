using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KleinanzeigenAdAlert.core.Telegram.Commands;

public abstract class TelegramCommand
{

    protected TelegramCommand(TelegramBotClient bot, Regex commandRegex, ILogger<TelegramCommand> logger)
    {

        bot.OnMessage += async (message, updateType) =>
        {
            if (commandRegex.IsMatch(message.Text!))
            {
                try
                {
                    await HandleCommand(message, updateType);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error while handling command {CommandString}", commandRegex.ToString());
                    await bot.SendMessage(message.Chat, "An error occurred while processing the command");
                    throw;
                }
            }
        };
    }
    
    protected abstract Task HandleCommand(Message msg, UpdateType updateType);
}