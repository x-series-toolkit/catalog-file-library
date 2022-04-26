namespace X4.Tests.Sandboxes.DI;

internal static class StaticLogger
{
    public static void ConfigureStaticLogger(Guid appId, string? appName)
    {
        var builder = new ConfigurationBuilder();

        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{GetEnv()}.json", true, true)
            .AddEnvironmentVariables();

        IConfigurationRoot configuration = builder.Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
            .Enrich.WithProperty("AppInstanceId", appId)
            .Enrich.WithProperty("ApplicationName", appName)
            .Enrich.WithProperty("Environment", GetEnv())
            .CreateLogger();
    }
}