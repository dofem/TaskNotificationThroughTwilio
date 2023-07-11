using TaskSchedulerWindowService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IUtility,Utility>();
        services.AddSingleton<INotification, Notification>();
    })
    .Build();

await host.RunAsync();
