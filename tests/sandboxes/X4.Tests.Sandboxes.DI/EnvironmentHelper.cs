namespace X4.Tests.Sandboxes.DI;

internal static class EnvironmentHelper
{
    public static string GetEnv() =>
        GetEnvironmentVariable("DOTNET_ENVIRONMENT") ??
        GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";
}