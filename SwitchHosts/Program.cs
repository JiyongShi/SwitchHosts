using SwitchHosts;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "SwitchHosts";
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<SwitchHostService>();
        services.AddSingleton<GithubHostsService>();

        services.AddHostedService<SwitchHostWorker>();
        services.AddHostedService<GitHubHostsWorker>();
    })
    .ConfigureLogging((context, logging) =>
    {
        // See: https://github.com/dotnet/runtime/issues/47303
        logging.AddConfiguration(
            context.Configuration.GetSection("Logging"));
    })
    .Build();

await host.RunAsync();
