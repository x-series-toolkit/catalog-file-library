namespace X4.Tests.Sandboxes.Benchmarks.Benchmarks.SplitSpan;

// [MemoryDiagnoser()]
public class BenchMark
{
    private static readonly string _catalogFilePath = @"E:\SteamLibrary\steamapps\common\X4 Foundations\01.cat";
    private static readonly string _fileContent = File.ReadAllText(_catalogFilePath);
    
    // [Benchmark]
    public IList<CatalogFile2> ParseWithSplit()
    {
        string[] catalogFilePaths = Directory.GetFiles(@"E:\SteamLibrary\steamapps\common\X4 Foundations", "*.cat");

        return catalogFilePaths.Select(File.ReadAllText).Select(catalogFileContent => new CatalogFile2(_catalogFilePath, catalogFileContent)).ToList();
    }

    // [Benchmark]
    public IList<CatalogFile> ParseWithSpan()
    {
        string[] catalogFilePaths = Directory.GetFiles(@"E:\SteamLibrary\steamapps\common\X4 Foundations", "*.cat");

        return catalogFilePaths.Select(File.ReadAllText).Select(catalogFileContent => new CatalogFile(_catalogFilePath.AsSpan(), catalogFileContent.AsSpan())).ToList();
    }
}