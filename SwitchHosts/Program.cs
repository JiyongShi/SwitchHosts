using SwitchHosts;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "SwitchHosts";
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<SwitchHostService>();

        services.AddHostedService<Worker>();
    })
    .ConfigureLogging((context, logging) =>
    {
        // See: https://github.com/dotnet/runtime/issues/47303
        logging.AddConfiguration(
            context.Configuration.GetSection("Logging"));
    })
    .Build();

await host.RunAsync();
