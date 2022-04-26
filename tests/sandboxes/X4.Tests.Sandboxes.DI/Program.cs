using X4.Tests.Sandboxes.DI.HostedServices;

try
{
    var appInstanceId = Guid.NewGuid();
    string applicationName = typeof(Program).Assembly.GetName().Name ?? "DI Sandbox";

    ConfigureStaticLogger(appInstanceId, applicationName);

    Log.Information("Starting {AppName}", applicationName);

    IHostBuilder hostBuilder = new HostBuilder()
        .UseEnvironment(GetEnv())
        .ConfigureAppConfiguration((context, builder) =>
        {
            builder.SetBasePath(CurrentDirectory)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            if (args is { Length: > 0 })
            {
                builder.AddCommandLine(args);
            }
        })
        .ConfigureServices((context, collection) =>
        {
            collection
                .AddSingleton<IFileSystem, FileSystem>()
                .AddTransient<ICatalogFileReader, CatalogFileReader>()
                .AddTransient<ICatalogAssetExporter, CatalogAssetExporter>()
                .AddTransient<ICatalogService, CatalogService>()
                .AddTransient<ICatalogServiceWithProgress, CatalogServiceWithProgress>()
                .AddLogging();

            collection.AddHostedService<SeqHostedService>();

        })
        .ConfigureLogging((context, builder) =>
        {
            Logger logger = new LoggerConfiguration()
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.WithProperty("AppInstanceId", appInstanceId)
                .Enrich.WithProperty("ApplicationName", applicationName)
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
#if DEBUG
                // Used to filter out potentially bad data due debugging.
                // Very useful when doing Seq dashboards and want to remove logs under debugging session.
                .Enrich.WithProperty("DebuggerAttached", Debugger.IsAttached)
#endif
                .CreateLogger();

            builder.AddSerilog(logger);
        })
        .UseConsoleLifetime();

    IHost host = hostBuilder.Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}