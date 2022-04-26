using System.IO.Abstractions;
using Microsoft.Extensions.Logging;

namespace X4.Tests.Sandboxes.Benchmarks.Benchmarks.Export;

[MemoryDiagnoser()]
public class ExportSpan
{
    //private const string Path = @"E:\SteamLibrary\steamapps\common\X4 Foundations\extensions\crystal_rarities\subst_01.cat";
    private const string Path = @"E:\SteamLibrary\steamapps\common\X4 Foundations\01.cat";

    private readonly CatalogFile _catalogFile;
    private readonly CatalogAssetExporter _catalogAssetExporter = new();
    private readonly IFileSystem _fs;
    private readonly ILogger<ExportSpan>? _logger;

    public ExportSpan()
    {
        var catalogFileReader = new CatalogFileReader();
        _catalogFile = catalogFileReader.GetCatalogFile(Path);
        _fs = new FileSystem();
    }

    [Benchmark]
    public async Task ExportAssets()
    {
        await _catalogAssetExporter.ExportAssetsAsync(_catalogFile, @"E:\temp\tempx4");
    }

    //[Benchmark]
    //public async Task ExportAssetsPipe()
    //{
    //    await _catalogAssetExporter.ExportAssetsPipe(_catalogFile, @"E:\temp\tempx4pipe");
    //}

    [Benchmark]
    public async Task ExportAssetsArrayPool()
    {
        await _catalogAssetExporter.ExportAssetsArrayPoolAsync(_catalogFile, @"E:\temp\tempx4pool");
    }

    //[Benchmark]
    //public void WithSpanWithStackalloc()
    //{
    //    ExportAssets(_catalogFile, @"E:\temp\tempx4");
    //}

    //[Benchmark]
    //public void WithSpanWithoutStackalloc()
    //{
    //    ExportAssetsOld(_catalogFile, @"E:\temp\tempx4");
    //}

    //public void ExportAssets(CatalogFile catalogFile, string destDirectory)
    //{
    //    if (catalogFile == null)
    //    {
    //        _logger?.LogError("Catalog file is null");
    //        throw new ArgumentNullException(nameof(catalogFile));
    //    }

    //    IFileInfo catalogFileInfo = _fs.FileInfo.FromFileName(catalogFile.FilePath);

    //    if (!catalogFileInfo.Exists)
    //    {
    //        _logger?.LogError("Catalog file does not exist: {CatalogFile}", catalogFile.FilePath);
    //        throw new CatalogFileNotFoundException("Catalog file does not exist", catalogFile.FilePath);
    //    }

    //    if (string.IsNullOrEmpty(destDirectory))
    //    {
    //        _logger?.LogError("Destination directory is null or empty");
    //        throw new ArgumentException("Value cannot be null or empty.", nameof(destDirectory));
    //    }

    //    string catalogFileName = _fs.Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
    //    string datFilePath = _fs.Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");

    //    IFileInfo datFileInfo = _fs.FileInfo.FromFileName(datFilePath);

    //    if (!datFileInfo.Exists)
    //    {
    //        _logger?.LogError("Dat file does not exist: {DatFile}", datFilePath);
    //        throw new DatFileNotFoundException("Dat file does not exist", datFilePath);
    //    }

    //    using Stream stream = datFileInfo.OpenRead();

    //    foreach (CatalogEntry catalogEntry in catalogFile.CatalogEntries)
    //    {
    //        string destFilePath = _fs.Path.Combine(destDirectory, catalogEntry.AssetPath);
    //        IFileInfo destFile = _fs.FileInfo.FromFileName(destFilePath);

    //        if (!destFile.Directory.Exists)
    //        {
    //            destFile.Directory.Create();
    //        }

    //        stream.Seek(catalogEntry.ByteOffset, SeekOrigin.Begin);
    //        Span<byte> newFileData = stackalloc byte[catalogEntry.AssetSize];
    //        int read = stream.Read(newFileData);

    //        if (read != catalogEntry.AssetSize)
    //        {
    //            _logger?.LogError("Could not read asset data from dat file: {DatFile}", datFilePath);
    //            throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
    //        }

    //        //using Stream destStream = _fs.File.Open(destFile.FullName, FileMode.Create);
    //        //destStream.Write(newFileData);
    //        //destStream.Close();
    //    }
    //}

    //public void ExportAssetsOld(CatalogFile catalogFile, string destDirectory)
    //{
    //    if (catalogFile == null)
    //    {
    //        _logger?.LogError("Catalog file is null");
    //        throw new ArgumentNullException(nameof(catalogFile));
    //    }

    //    IFileInfo catalogFileInfo = _fs.FileInfo.FromFileName(catalogFile.FilePath);

    //    if (!catalogFileInfo.Exists)
    //    {
    //        _logger?.LogError("Catalog file does not exist: {CatalogFile}", catalogFile.FilePath);
    //        throw new CatalogFileNotFoundException("Catalog file does not exist", catalogFile.FilePath);
    //    }

    //    if (string.IsNullOrEmpty(destDirectory))
    //    {
    //        _logger?.LogError("Destination directory is null or empty");
    //        throw new ArgumentException("Value cannot be null or empty.", nameof(destDirectory));
    //    }

    //    string catalogFileName = _fs.Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
    //    string datFilePath = _fs.Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");

    //    IFileInfo datFileInfo = _fs.FileInfo.FromFileName(datFilePath);

    //    if (!datFileInfo.Exists)
    //    {
    //        _logger?.LogError("Dat file does not exist: {DatFile}", datFilePath);
    //        throw new DatFileNotFoundException("Dat file does not exist", datFilePath);
    //    }

    //    using Stream stream = datFileInfo.OpenRead();

    //    foreach (CatalogEntry catalogEntry in catalogFile.CatalogEntries)
    //    {
    //        string destFilePath = _fs.Path.Combine(destDirectory, catalogEntry.AssetPath);
    //        IFileInfo destFile = _fs.FileInfo.FromFileName(destFilePath);

    //        if (!destFile.Directory.Exists)
    //        {
    //            destFile.Directory.Create();
    //        }

    //        stream.Seek(catalogEntry.ByteOffset, SeekOrigin.Begin);
    //        var newFileData = new byte[catalogEntry.AssetSize];
    //        int read = stream.Read(newFileData, 0, catalogEntry.AssetSize);

    //        if (read != catalogEntry.AssetSize)
    //        {
    //            _logger?.LogError("Could not read asset data from dat file: {DatFile}", datFilePath);
    //            throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
    //        }

    //        //using Stream destStream = _fs.File.Open(destFile.FullName, FileMode.Create);
    //        //destStream.Write(newFileData);
    //        //destStream.Close();
    //    }
    //}

    //[Benchmark]
    //public void WithSpanOld()
    //{
    //    ExportAssetOld(Path, _catalogFile.CatalogEntries[0], @"E:\temp\tempx.xsd");
    //}

    //public void ExportAsset(string sourceFile, CatalogEntry catalogEntry, string destDirectory, string? destFileName = null)
    //{
    //    IFileInfo catalogFileInfo = _fs.FileInfo.FromFileName(sourceFile);

    //    if (!catalogFileInfo.Exists)
    //    {
    //        _logger?.LogError("Catalog file does not exist: {CatalogFile}", sourceFile);
    //        throw new CatalogFileNotFoundException("Catalog file does not exist", sourceFile);
    //    }

    //    if (catalogEntry == null)
    //    {
    //        _logger?.LogError("Catalog entry is null");
    //        throw new ArgumentNullException(nameof(catalogEntry));
    //    }

    //    if (string.IsNullOrEmpty(destDirectory))
    //    {
    //        _logger?.LogError("Destination directory is null or empty");
    //        throw new ArgumentException("Value cannot be null or empty.", nameof(destDirectory));
    //    }

    //    if (destFileName != null && string.IsNullOrEmpty(destDirectory))
    //    {
    //        _logger?.LogError("Destination file empty");
    //        throw new ArgumentException("Value cannot be empty.", nameof(destFileName));
    //    }

    //    string catalogFileName = _fs.Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
    //    string datFilePath = _fs.Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");

    //    IFileInfo datFileInfo = _fs.FileInfo.FromFileName(datFilePath);

    //    if (!datFileInfo.Exists)
    //    {
    //        _logger?.LogError("Dat file does not exist: {DatFile}", datFilePath);
    //        throw new DatFileNotFoundException("Dat file does not exist", datFilePath);
    //    }

    //    string destFilePath = _fs.Path.Combine(destDirectory, destFileName ?? catalogEntry.AssetPath);
    //    IFileInfo destFile = _fs.FileInfo.FromFileName(destFilePath);

    //    if (destFile.Directory.Exists)
    //    {
    //        destFile.Directory.Create();
    //    }

    //    using Stream stream = datFileInfo.OpenRead();
    //    stream.Seek(catalogEntry.ByteOffset, SeekOrigin.Begin);
    //    Span<byte> newFileData = new byte[catalogEntry.AssetSize];
    //    int read = stream.Read(newFileData);

    //    if (read != catalogEntry.AssetSize)
    //    {
    //        _logger?.LogError("Could not read asset data from dat file: {DatFile}", datFilePath);
    //        throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
    //    }

    //    //using Stream destStream = _fs.File.Open(destFile.FullName, FileMode.Create);
    //    //destStream.Write(newFileData);
    //    //destStream.Close();
    //}

    //public void ExportAssetOld(string sourceFile, CatalogEntry catalogEntry, string destDirectory, string? destFileName = null)
    //{
    //    IFileInfo catalogFileInfo = _fs.FileInfo.FromFileName(sourceFile);

    //    if (!catalogFileInfo.Exists)
    //    {
    //        _logger?.LogError("Catalog file does not exist: {CatalogFile}", sourceFile);
    //        throw new CatalogFileNotFoundException("Catalog file does not exist", sourceFile);
    //    }

    //    if (catalogEntry == null)
    //    {
    //        _logger?.LogError("Catalog entry is null");
    //        throw new ArgumentNullException(nameof(catalogEntry));
    //    }

    //    if (string.IsNullOrEmpty(destDirectory))
    //    {
    //        _logger?.LogError("Destination directory is null or empty");
    //        throw new ArgumentException("Value cannot be null or empty.", nameof(destDirectory));
    //    }

    //    if (destFileName != null && string.IsNullOrEmpty(destDirectory))
    //    {
    //        _logger?.LogError("Destination file empty");
    //        throw new ArgumentException("Value cannot be empty.", nameof(destFileName));
    //    }

    //    string catalogFileName = _fs.Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
    //    string datFilePath = _fs.Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");

    //    IFileInfo datFileInfo = _fs.FileInfo.FromFileName(datFilePath);

    //    if (!datFileInfo.Exists)
    //    {
    //        _logger?.LogError("Dat file does not exist: {DatFile}", datFilePath);
    //        throw new DatFileNotFoundException("Dat file does not exist", datFilePath);
    //    }

    //    string destFilePath = _fs.Path.Combine(destDirectory, destFileName ?? catalogEntry.AssetPath);
    //    IFileInfo destFile = _fs.FileInfo.FromFileName(destFilePath);

    //    if (destFile.Directory.Exists)
    //    {
    //        destFile.Directory.Create();
    //    }

    //    using Stream stream = datFileInfo.OpenRead();
    //    stream.Seek(catalogEntry.ByteOffset, SeekOrigin.Begin);
    //    var newFileData = new byte[catalogEntry.AssetSize];
    //    int read = stream.Read(newFileData, 0, catalogEntry.AssetSize);

    //    if (read != catalogEntry.AssetSize)
    //    {
    //        _logger?.LogError("Could not read asset data from dat file: {DatFile}", datFilePath);
    //        throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
    //    }

    //    //using Stream destStream = _fs.File.Open(destFile.FullName, FileMode.Create);
    //    //destStream.Write(newFileData);
    //    //destStream.Close();
    //}
}