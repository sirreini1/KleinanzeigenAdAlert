using KleinanzeigenAdAlert.core.Telegram;
using KleinanzeigenAdAlert.DB.repositories;
using KleinanzeigenAdAlert.models;
using Microsoft.Extensions.Logging;

namespace KleinanzeigenAdAlert.core.Kleinanzeigen;

public interface IAdMonitoringService
{
    Task CheckForNewAdsPeriodically();
}

public class AdMonitoringService(
    IFlatAdRepository flatAdRepository,
    ITelegramMessageUserService telegramMessageUserService,
    ILogger<AdMonitoringService> logger)
    : IAdMonitoringService
{
    private static readonly Random Random = new();

    public async Task CheckForNewAdsPeriodically()
    {
        while (true)
        {
            logger.LogInformation("Checking for new ads");
            await CheckForNewAds();
            var randomTimeOffset = Random.Next() % 120 - 60;
            if (randomTimeOffset < Config.TimeBetweenSearches.TotalSeconds)
            {
                randomTimeOffset = 0;
            }

            var timeBetweenSearches = Config.TimeBetweenSearches + TimeSpan.FromSeconds(randomTimeOffset);
            logger.LogInformation("Next Search in: {TimeBetweenSearches}",
                timeBetweenSearches);
            await Task.Delay(timeBetweenSearches);
        }
    }

    private async Task CheckForNewAds()
    {
        var searchPairs = flatAdRepository.GetUniqueTelegramUserSearchUrlPairs();

        foreach (var pair in searchPairs)
        {
            Console.WriteLine("Checking for new ads for user: " + pair.TelegramUser);
            var flatAds = await AdExtractor.GetAdsFromUrl(pair.SearchUrl);
            var newAds = flatAdRepository.CheckForNewAds(flatAds, pair.TelegramUser);
            if (newAds.Count != 0)
            {
                Console.WriteLine("New ads found for user: " + pair.TelegramUser);
                foreach (var message in newAds.Select(GetFormattedMessage))
                    _ = telegramMessageUserService.SendMessageToUser(pair.TelegramUser, message);
            }

            flatAdRepository.UpsertFlatAds(newAds, pair.TelegramUser);
        }
    }

    private static string GetFormattedMessage(FlatAd ad)
    {
        var message = $"""
                       description: {ad.Description}
                       price: {ad.Price}
                       published time: {ad.PostedDate}
                       location: {ad.Location}
                       url: {ad.AdUrl}
                       """;

        return message;
    }
}