namespace X4.Tests.Sandboxes.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class GetCatalogFilesInMultipleDir
{
    private static readonly CatalogService CatalogService = new(new CatalogFileReader(), new CatalogAssetExporter());
    private static readonly CatalogServiceWithProgress CatalogServiceWithProgress = new(CatalogService);

    private readonly string[] _catalogDirPaths =
    {
        @"E:\SteamLibrary\steamapps\common\X4 Foundations",
        @"E:\SteamLibrary\steamapps\common\X4 Foundations\extensions\ego_dlc_split",
        @"D:\temp\x4-temp", 
        @"D:\temp\x4-temp\crystal_rarities",
        @"D:\temp\x4-temp\ego_dlc_terran"
    };

    [Benchmark]
    public IImmutableList<CatalogFile> GetCatalogFilesInMultipleDirectory_Benchmark()
    {
        return CatalogService.GetCatalogFilesInMultipleDirectory(_catalogDirPaths);
    }

    [Benchmark]
    public IImmutableList<CatalogFile> GetCatalogFilesInMultipleDirectoryWithProgress_Benchmark()
    {
        return CatalogServiceWithProgress.GetCatalogFilesInMultipleDirectory(new ConsoleProgress<ProgressReport>(report =>
        {
            (int completed, int total) = report;
            double percent = Math.Round(completed / (double)total * 100);
            Console.WriteLine($"{percent}%");
        }), _catalogDirPaths);
    }

    [Benchmark]
    public IImmutableList<CatalogFile> GetCatalogFilesInMultipleDirectoryParallel_Benchmark()
    {
        return CatalogService.GetCatalogFilesInMultipleDirectoryParallel(_catalogDirPaths);
    }

    [Benchmark]
    public IImmutableList<CatalogFile> GetCatalogFilesInMultipleDirectoryParallelWithProgress_Benchmark()
    {
        return CatalogServiceWithProgress.GetCatalogFilesInMultipleDirectoryParallel(new ConsoleProgress<ProgressReport>(report =>
        {
            (int completed, int total) = report;
            double percent = Math.Round(completed / (double)total * 100);
            Console.WriteLine($"{percent}%");
        }), _catalogDirPaths);
    }

    [Benchmark]
    public async Task<IImmutableList<CatalogFile>> GetCatalogFilesInMultipleDirectoryAsync_Benchmark()
    {
        return await CatalogService.GetCatalogFilesInMultipleDirectoryAsync(catalogDirectoryPaths:_catalogDirPaths);
    }

    [Benchmark]
    public Task<IImmutableList<CatalogFile>> GetCatalogFilesInMultipleDirectoryAsyncWithProgress_Benchmark()
    {
        return CatalogServiceWithProgress.GetCatalogFilesInMultipleDirectoryAsync(new ConsoleProgress<ProgressReport>(report =>
        {
            (int completed, int total) = report;
            double percent = Math.Round(completed / (double)total * 100);
            Console.WriteLine($"{percent}%");
        }), catalogDirectoryPaths: _catalogDirPaths);
    }
}