namespace X4.Tests.Sandboxes.Benchmarks.Benchmarks.AssetExporter;

[MemoryDiagnoser()]
public class ExportAssets
{
    private const string PathSmall = @"E:\SteamLibrary\steamapps\common\X4 Foundations\extensions\crystal_rarities\subst_01.cat";
    private const string PathBig = @"E:\SteamLibrary\steamapps\common\X4 Foundations\01.cat";

    private static readonly CatalogFileReader CatalogFileReader = new();
    private static readonly CatalogFile CatalogFileSmall = CatalogFileReader.GetCatalogFile(PathSmall);
    private static readonly CatalogFile CatalogFileLarge = CatalogFileReader.GetCatalogFile(PathBig);
    private static readonly CatalogAssetExporter CatalogAssetExporter = new();

    private CatalogFile _catalogFile;
    
    [Params("Small_600KB", "Large_5GB")]
    public string FileSize { get; set; }
    
    [GlobalSetup]
    public void GlobalSetup()
    {
        _catalogFile = FileSize == "Large_5GB" ? CatalogFileLarge : CatalogFileSmall;
    }

    [Benchmark]
    public void ExportAssetsArrayPool()
    {
        CatalogAssetExporter.ExportAssets(_catalogFile, @"E:\temp\tempx4");
    }

    [Benchmark]
    public void ExportAssetSubStream()
    {
        CatalogAssetExporter.ExportAssetsSubStream(_catalogFile, @"E:\temp\tempx4");
    }

    [Benchmark]
    public async Task ExportAssetArrayPoolAsync()
    {
        await CatalogAssetExporter.ExportAssetsAsync(_catalogFile, @"E:\temp\tempx4");
    }

    [Benchmark]
    public async Task ExportAssetSubStreamAsync()
    {
        await CatalogAssetExporter.ExportAssetsSubStreamAsync(_catalogFile, @"E:\temp\tempx4");
    }
}