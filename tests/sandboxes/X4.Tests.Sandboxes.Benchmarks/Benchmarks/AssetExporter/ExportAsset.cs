namespace X4.Tests.Sandboxes.Benchmarks.Benchmarks.AssetExporter;

[MemoryDiagnoser()]
public class ExportAsset
{
    //private const string Path = @"E:\SteamLibrary\steamapps\common\X4 Foundations\extensions\crystal_rarities\subst_01.cat";
    private const string Path = @"E:\SteamLibrary\steamapps\common\X4 Foundations\01.cat";

    private readonly CatalogFile _catalogFile;
    private readonly CatalogAssetExporter _catalogAssetExporter = new();

    public ExportAsset()
    {
        var catalogFileReader = new CatalogFileReader();
        _catalogFile = catalogFileReader.GetCatalogFile(Path);
    }

    [Benchmark]
    public void ExportAssetAlloc()
    {
        _catalogAssetExporter.ExportAsset(_catalogFile.FilePath, _catalogFile.CatalogEntries[0], @"E:\temp\tempx4");
    }

    [Benchmark]
    public void ExportAssetSubStream()
    {
        _catalogAssetExporter.ExportAssetSubStream(_catalogFile.FilePath, _catalogFile.CatalogEntries[0], @"E:\temp\tempx4");
    }
}