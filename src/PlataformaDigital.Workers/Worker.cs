namespace PlataformaDigital.Workers;

public class ConsolidationWorker(ILogger<ConsolidationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Consolidation worker heartbeat: {time}", DateTimeOffset.UtcNow);
            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }
}
