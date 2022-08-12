using System.Net.NetworkInformation;

namespace SwitchHosts;

public class Worker : BackgroundService
{
    private readonly SwitchHostService _switchHostService;
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger, SwitchHostService switchHostService)
    {
        _logger = logger;
        _switchHostService = switchHostService;
        NetworkChange.NetworkAddressChanged += new
            NetworkAddressChangedEventHandler(AddressChangedCallback);
    }

    private void AddressChangedCallback(object? sender, EventArgs e)
    {
        string executeResult = _switchHostService.Execute();
        _logger.LogInformation("{executeResult}", executeResult);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            string executeResult = _switchHostService.Execute();
            _logger.LogInformation("{executeResult}", executeResult);

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(300000, stoppingToken);
        }
    }
}
