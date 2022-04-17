namespace X4.CatalogFileLib.Services.Components;

public class CatalogAssetExporter : ICatalogAssetExporter
{
    private readonly IFileSystem _fs;
    private readonly ILogger<CatalogAssetExporter>? _logger;

    public CatalogAssetExporter(IFileSystem? fileSystem = null, ILogger<CatalogAssetExporter>? logger = null)
    {
        _fs = fileSystem ?? new FileSystem();
        _logger = logger;
    }

    public void ExportAsset(CatalogEntry catalogEntry, string destFileName)
    {
        IFileInfo catalogFileInfo = _fs.FileInfo.FromFileName("catalogEntry.CatalogFilePath");

        if (!catalogFileInfo.Exists)
        {
            _logger?.LogError("Catalog file does not exist: {CatalogFile}", "catalogEntry.CatalogFilePath");
            throw new CatalogFileNotFoundException("Catalog file does not exist", "catalogEntry.CatalogFilePath");
        }

        string catalogFileName = _fs.Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
        string datFilePath = _fs.Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");

        IFileInfo datFileInfo = _fs.FileInfo.FromFileName(datFilePath);

        if (!datFileInfo.Exists)
        {
            _logger?.LogError("Dat file does not exist: {DatFile}", datFilePath);
            throw new DatFileNotFoundException("Dat file does not exist", datFilePath);
        }

        using Stream stream = datFileInfo.OpenRead();
        stream.Seek(catalogEntry.ByteOffset, SeekOrigin.Begin);
        var newFileData = new byte[catalogEntry.AssetSize];
        int read = stream.Read(newFileData, 0, catalogEntry.AssetSize);

        if (read != catalogEntry.AssetSize)
        {
            _logger?.LogError("Could not read asset data from dat file: {DatFile}", datFilePath);
            throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
        }

        using Stream destStream = _fs.File.Open(destFileName, FileMode.Create);
        destStream.Write(newFileData, 0, catalogEntry.AssetSize);
        destStream.Close();
    }
}