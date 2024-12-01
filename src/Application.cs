using KleinanzeigenAdAlert.core.Kleinanzeigen;

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