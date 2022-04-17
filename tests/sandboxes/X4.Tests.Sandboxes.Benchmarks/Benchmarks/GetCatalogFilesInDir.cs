namespace X4.Tests.Sandboxes.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class GetCatalogFilesInDir
{
    private readonly CatalogService _catalogService = new(new CatalogFileReader(), new CatalogAssetExporter());
    private const string CatalogDirPath = @"E:\SteamLibrary\steamapps\common\X4 Foundations";


    [Benchmark]
    public IReadOnlyList<CatalogFile> GetCatalogFilesByDirectory_Benchmark()
    {
        return _catalogService.GetCatalogFilesByDirectory(CatalogDirPath);
    }

    [Benchmark]
    public IReadOnlyList<CatalogFile> GetCatalogFilesByDirectoryWithProgress_Benchmark()
    {
        return _catalogService.GetCatalogFilesByDirectory(CatalogDirPath, progress: new ConsoleProgress<ProgressReport>(report =>
        {
            (int completed, int total) = report;
            double percent = Math.Round(completed / (double)total * 100);
            System.Console.WriteLine($"{percent}%");
        }));
    }

    [Benchmark]
    public async Task<IReadOnlyList<CatalogFile>> GetCatalogFilesByDirectoryAsync_Benchmark()
    {
        return await _catalogService.GetCatalogFilesByDirectoryAsync(CatalogDirPath);
    }

    [Benchmark]
    public async Task<IReadOnlyList<CatalogFile>> GetCatalogFilesByDirectoryAsyncWithProgress_Benchmark()
    {
        return await _catalogService.GetCatalogFilesByDirectoryAsync(CatalogDirPath,default, new ConsoleProgress<ProgressReport>(report =>
        {
            (int completed, int total) = report;
            double percent = Math.Round(completed / (double)total * 100);
            System.Console.WriteLine($"{percent}%");
        }));
    }
}