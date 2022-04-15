namespace X4.CatalogFileLib.Services.Components;

public class CatalogReader : ICatalogReader
{
    private readonly IFileSystem _fs;
    private readonly ILogger<CatalogReader>? _logger;

    public CatalogReader(IFileSystem? fileSystem = null, ILogger<CatalogReader>? logger = null)
    {
        _fs = fileSystem ?? new FileSystem();
        _logger = logger;
    }

    public async Task<IReadOnlyList<CatalogEntry>> GetCatalogEntriesAsync(string catalogFilePath, IProgress<ProgressReport>? progress = null)
    {
        IFileInfo catalogFile = GetCatalogFileInfo(catalogFilePath);

        _logger?.LogTrace("Reading catalog file: {CatalogFile}", catalogFilePath);
        using StreamReader? reader = catalogFile.OpenText();
        string fileContent = await reader.ReadToEndAsync();
        string[] rows = fileContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        var entries = new List<CatalogEntry>();
        long byteOffset = 0;

        for (var index = 0; index < rows.Length; index++)
        {
            string row = rows[index];

            var catalogEntry = new CatalogEntry(catalogFile.FullName, row, byteOffset);

            entries.Add(catalogEntry);

            byteOffset += catalogEntry.AssetSize;
            progress?.Report(new ProgressReport(catalogEntry.AssetPath, index, rows.Length));
        }

        return entries;
    }

    public IReadOnlyList<CatalogEntry> GetCatalogEntries(string catalogFilePath, IProgress<ProgressReport>? progress = null)
    {
        IFileInfo catalogFile = GetCatalogFileInfo(catalogFilePath);

        _logger?.LogTrace("Reading catalog file: {CatalogFile}", catalogFilePath);
        string[] rows = _fs.File.ReadAllLines(catalogFilePath);

        var entries = new List<CatalogEntry>();
        long byteOffset = 0;

        for (var index = 0; index < rows.Length; index++)
        {
            string row = rows[index];

            var catalogEntry = new CatalogEntry(catalogFile.FullName, row, byteOffset);

            entries.Add(catalogEntry);

            byteOffset += catalogEntry.AssetSize;
            progress?.Report(new ProgressReport(catalogEntry.AssetPath, index, rows.Length));
        }

        return entries;
    }

    private IFileInfo GetCatalogFileInfo(string path)
    {
        IFileInfo catalogFile = _fs.FileInfo.FromFileName(path);

        if (!catalogFile.Exists)
        {
            _logger?.LogError("Catalog file does not exist: {CatalogFile}", path);
            throw new CatalogFileNotFoundException("Catalog file does not exist", path);
        }

        if (catalogFile.Extension == ".cat")
        {
            return catalogFile;
        }
        
        _logger?.LogError("The specified file is not a catalog file: {CatalogFile}", path);
        throw new InvalidCatalogFileException("File is not a catalog file");
    }
}