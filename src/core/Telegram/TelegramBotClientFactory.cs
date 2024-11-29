using Telegram.Bot;

namespace KleinanzeigenAdAlert.core.Telegram;

public static class TelegramBotClientFactory
{
    public static TelegramBotClient CreateBotClient()
    {
        var botToken = Config.TelegramBotToken;
        if (botToken is null) throw new Exception("No telegram bot token found");

        var cts = new CancellationTokenSource();
        return new TelegramBotClient(botToken, cancellationToken: cts.Token);
    }
}