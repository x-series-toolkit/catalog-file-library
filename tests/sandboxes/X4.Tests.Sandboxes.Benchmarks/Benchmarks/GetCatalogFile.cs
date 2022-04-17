namespace X4.Tests.Sandboxes.Benchmarks.Benchmarks;

public class GetCatalogFile
{
    private readonly CatalogService _catalogService = new(new CatalogFileReader(), new CatalogAssetExporter());
    private const string CatalogFilePath = @"E:\SteamLibrary\steamapps\common\X4 Foundations\01.cat";

    [Benchmark]
    public CatalogFile GetCatalogFile_Benchmark()
    {
        return _catalogService.GetCatalogFile(CatalogFilePath);
    }

    [Benchmark]
    public async Task<CatalogFile> GetCatalogFileAsync_Benchmark()
    {
        return await _catalogService.GetCatalogFileAsync(CatalogFilePath);
    }
}