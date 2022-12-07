namespace SwitchHosts;

public class GitHubHostsWorker : BackgroundService
{
    private readonly GithubHostsService _githubHostsService;
    private readonly ILogger<GitHubHostsWorker> _logger;

    public GitHubHostsWorker(ILogger<GitHubHostsWorker> logger, GithubHostsService githubHostsService)
    {
        _logger = logger;
        _githubHostsService = githubHostsService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            string executeResult = _githubHostsService.Execute();
            _logger.LogInformation("{executeResult}", executeResult);

            _logger.LogInformation("SwitchHostWorker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(600000, stoppingToken);
        }
    }
}
