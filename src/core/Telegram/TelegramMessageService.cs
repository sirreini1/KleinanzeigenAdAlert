// src/core/TelegramHandler.cs

using KleinanzeigenAdAlert.DB.repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace KleinanzeigenAdAlert.core.Telegram;

public interface ITelegramMessageUserService
{
    Task SendMessageToUser(string userId, string message);
}

public class TelegramMessageUserUserService(
    IUserPairChatIdRepository userPairChatIdRepository,
    TelegramBotClient bot,
    ILogger<TelegramMessageUserUserService> logger)
    : ITelegramMessageUserService
{
    public async Task SendMessageToUser(string userId, string message)
    {
        var userChatIdPair = userPairChatIdRepository.FindChatByUserId(userId);
        if (userChatIdPair?.ChatId != null)
            await bot.SendTextMessageAsync(userChatIdPair.ChatId, message);
        else
            logger.LogError("User not found, could not send message");
    }
}