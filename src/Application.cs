using KleinanzeigenAdAlert.core.Kleinanzeigen;
using KleinanzeigenAdAlert.core.Kleinanzeigen.services;

namespace KleinanzeigenAdAlert;

public class Application(IAdMonitoringService adMonitoringService, IAdCleanupService adCleanupService)
{
    public async Task StartApp()
    {
        var scanForNewAdsTask = adMonitoringService.CheckForNewAdsPeriodically();
        var cleanUpTask = adCleanupService.CleanupAdsPeriodically();
        Task.WaitAll(scanForNewAdsTask, cleanUpTask);
    }
}