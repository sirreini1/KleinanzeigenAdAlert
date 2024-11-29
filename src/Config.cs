namespace KleinanzeigenAdAlert;

public static class Config
{
    public const string SearchBaseUrl = "https://www.kleinanzeigen.de";
    public static TimeSpan TimeBetweenSearches = TimeSpan.FromMinutes(5);
    public static readonly string TelegramBotToken = Environment.GetEnvironmentVariable("TOKEN") ?? "someToken";
}