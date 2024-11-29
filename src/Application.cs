using KleinanzeigenAdAlert.core.Kleinanzeigen;

namespace KleinanzeigenAdAlert;

public class Application(IAdMonitoringService adMonitoringService)
{
    public async Task StartApp()
    {
        await adMonitoringService.CheckForNewAdsPeriodically();
    }
}