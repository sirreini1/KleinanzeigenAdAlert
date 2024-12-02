namespace KleinanzeigenAdAlert;

public static class Config
{
    public const string SearchBaseUrl = "https://www.kleinanzeigen.de";
    public static TimeSpan TimeBetweenSearches = TimeSpan.FromSeconds(30);
    public static TimeSpan TimeBetweenCleanups = TimeSpan.FromDays(1);
    public static TimeSpan MaxAgeOfAdToConsider = TimeSpan.FromDays(4);
    public static readonly string TelegramBotToken = Environment.GetEnvironmentVariable("TOKEN") ?? "someToken";
    public static readonly string OpenAIKey = Environment.GetEnvironmentVariable("OPENAI_KEY") ?? "";

    public static readonly string PromptFilePath =
        Environment.GetEnvironmentVariable("PROMPT_FILE_PATH") ??
        @"prompt.txt";
}