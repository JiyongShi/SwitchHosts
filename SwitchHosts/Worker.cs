namespace SwitchHosts;

public class Worker : BackgroundService
{
    private readonly SwitchHostService _switchHostService;
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger, SwitchHostService switchHostService)
    {
        _logger = logger;
        _switchHostService = switchHostService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            string executeResult = await _switchHostService.ExecuteAsync();
            _logger.LogInformation("{executeResult}", executeResult);


            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(300000, stoppingToken);
        }
    }
}
