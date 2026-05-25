using HomeAssistant_Status.Services;

namespace HomeAssistant_Status;

public class Worker(IWebServerStandAlone webServerStandAlone) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await webServerStandAlone.Start();
    }
}