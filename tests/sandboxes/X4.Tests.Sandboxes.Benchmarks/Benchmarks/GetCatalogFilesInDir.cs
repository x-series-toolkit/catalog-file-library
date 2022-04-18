namespace X4.Tests.Sandboxes.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class GetCatalogFilesInDir
{
    private static readonly CatalogService CatalogService = new(new CatalogFileReader(), new CatalogAssetExporter());
    private static readonly CatalogServiceWithProgress CatalogServiceWithProgress = new(CatalogService);
    private const string CatalogDirPath = @"E:\SteamLibrary\steamapps\common\X4 Foundations";

    [Benchmark]
    public IImmutableList<CatalogFile> GetCatalogFilesInDirectory_Benchmark()
    {
        return CatalogService.GetCatalogFilesInDirectory(CatalogDirPath);
    }

    [Benchmark]
    public IImmutableList<CatalogFile> GetCatalogFilesInDirectoryWithProgress_Benchmark()
    {
        return CatalogServiceWithProgress.GetCatalogFilesInDirectory(CatalogDirPath, new ConsoleProgress<ProgressReport>(report =>
        {
            (int completed, int total) = report;
            double percent = Math.Round(completed / (double)total * 100);
            Console.WriteLine($"{percent}%");
        }));
    }

    [Benchmark]
    public IImmutableList<CatalogFile> GetCatalogFilesInDirectoryParallel_Benchmark()
    {
        return CatalogService.GetCatalogFilesInDirectoryParallel(CatalogDirPath);
    }

    [Benchmark]
    public IImmutableList<CatalogFile> GetCatalogFilesInDirectoryParallelWithProgress_Benchmark()
    {
        return CatalogServiceWithProgress.GetCatalogFilesInDirectoryParallel(CatalogDirPath, new ConsoleProgress<ProgressReport>(report =>
        {
            (int completed, int total) = report;
            double percent = Math.Round(completed / (double)total * 100);
            Console.WriteLine($"{percent}%");
        }));
    }

    [Benchmark]
    public async Task<IImmutableList<CatalogFile>> GetCatalogFilesInDirectoryAsync_Benchmark()
    {
        return await CatalogService.GetCatalogFilesInDirectoryAsync(CatalogDirPath);
    }

    [Benchmark]
    public async Task<IImmutableList<CatalogFile>> GetCatalogFilesInDirectoryAsyncWithProgress_Benchmark()
    {
        return await CatalogServiceWithProgress.GetCatalogFilesInDirectoryAsync(CatalogDirPath, new ConsoleProgress<ProgressReport>(report =>
        {
            (int completed, int total) = report;
            double percent = Math.Round(completed / (double)total * 100);
            Console.WriteLine($"{percent}%");
        }));
    }
}