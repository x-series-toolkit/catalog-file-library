namespace X4.Tests.Sandboxes.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class Parallelism
{
    private readonly CatalogService _catalogService = new(new CatalogFileReader(), new CatalogAssetExporter());
    private readonly string[] _catalogFilePaths = Directory.GetFiles(@"E:\SteamLibrary\steamapps\common\X4 Foundations", "*.cat");

    [Benchmark]
    public async Task<CatalogFile[]> GetCatalogFilesWhenAll_Benchmark()
    {
        IList<Task<CatalogFile>> tasks = _catalogFilePaths.Select(catalogFilePath => _catalogService.GetCatalogFileAsync(catalogFilePath)).ToList();

        return await Task.WhenAll(tasks.ToArray());
    }

#if NET6_0
    [Benchmark]
    public async Task<IList<CatalogFile>> GetCatalogFilesParallelForEachAsync_Benchmark()
    {
        IList<CatalogFile> catalogFiles = new List<CatalogFile>();

        await Parallel.ForEachAsync(_catalogFilePaths, async (s, token) =>
        {
            CatalogFile catalogFileAsync = await _catalogService.GetCatalogFileAsync(s, token);
            catalogFiles.Add(catalogFileAsync);
        });

        return catalogFiles;
    }
#endif

    [Benchmark]
    public IList<CatalogFile> GetCatalogFilesParallelForEach_Benchmark()
    {
        List<CatalogFile> catalogFiles = new List<CatalogFile>();

        Parallel.ForEach(_catalogFilePaths, path =>
        {
            CatalogFile catalogFileAsync = _catalogService.GetCatalogFile(path);
            catalogFiles.Add(catalogFileAsync);
        });

        return catalogFiles.ToList();
    }

    [Benchmark]
    public async Task<IList<CatalogFile>> GetCatalogFilesParallelForEachAsyncOwnImp_Benchmark()
    {
        var catalogFiles = new List<CatalogFile>();

        await _catalogFilePaths.ParallelForEachAsync(Environment.ProcessorCount, async (s, token) =>
        {
            CatalogFile catalogFileAsync = await _catalogService.GetCatalogFileAsync(s, token);
            catalogFiles.Add(catalogFileAsync);
        });

        return catalogFiles.ToList();
    }

    [Benchmark]
    public async Task<IList<CatalogFile>> GetCatalogFilesParallelForEachAsyncOwnImp100_Benchmark()
    {
        var catalogFiles = new List<CatalogFile>();

        await _catalogFilePaths.ParallelForEachAsync(100, async (s, token) =>
        {
            CatalogFile catalogFileAsync = await _catalogService.GetCatalogFileAsync(s, token);
            catalogFiles.Add(catalogFileAsync);
        });

        return catalogFiles.ToList();
    }
}