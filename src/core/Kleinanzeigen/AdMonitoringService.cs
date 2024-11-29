using KleinanzeigenAdAlert.core.Telegram;
using KleinanzeigenAdAlert.DB.repositories;
using KleinanzeigenAdAlert.models;

namespace KleinanzeigenAdAlert.core.Kleinanzeigen;

public interface IAdMonitoringService
{
    Task CheckForNewAdsPeriodically();
}

public class AdMonitoringService(
    IFlatAdRepository flatAdRepository,
    ITelegramMessageUserService telegramMessageUserService)
    : IAdMonitoringService
{
    public async Task CheckForNewAdsPeriodically()
    {
        while (true)
        {
            await CheckForNewAds();
            await Task.Delay(Config.TimeBetweenSearches);
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
                       location: {ad.Location}
                       url: {ad.AdUrl}
                       """;

        return message;
    }
}