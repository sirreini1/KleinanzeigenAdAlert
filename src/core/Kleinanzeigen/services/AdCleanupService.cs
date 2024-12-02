using KleinanzeigenAdAlert.DB.repositories;
using Microsoft.Extensions.Logging;

namespace KleinanzeigenAdAlert.core.Kleinanzeigen.services;

public interface IAdCleanupService
{
    Task CleanupAdsPeriodically();
}

public class AdCleanupService(
    IFlatAdRepository flatAdRepository,
    ILogger<AdCleanupService> logger)
    : IAdCleanupService
{
    public async Task CleanupAdsPeriodically()
    {
        while (true)
        {
            logger.LogInformation("Cleaning up old ads");
            flatAdRepository.DeleteOldFlatAds();
            await Task.Delay(Config.TimeBetweenCleanups);
        }
    }
}