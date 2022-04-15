namespace X4.CatalogFileLib.Services;

public class CatalogService
{
    private readonly ICatalogReader _catalogReader;
    private readonly ICatalogAssetExporter _catalogAssetExporter;
    private readonly IFileSystem _fs;
    private readonly ILogger<CatalogService>? _logger;

    public CatalogService(
        ICatalogReader catalogReader, 
        ICatalogAssetExporter catalogAssetExporter,
        IFileSystem? fileSystem = null,
        ILogger<CatalogService>? logger = null)
    {
        _catalogReader = catalogReader;
        _catalogAssetExporter = catalogAssetExporter;
        _fs = fileSystem ?? new FileSystem();
        _logger = logger;
    }

    public IReadOnlyList<string> GetCatalogFilePaths(string catalogsDirectoryPath)
    {
        if (string.IsNullOrEmpty(catalogsDirectoryPath))
        {
            _logger?.LogError("Catalog root directory not specified");
            throw new ArgumentNullException(nameof(catalogsDirectoryPath), "Catalog root directory path is not set.");
        }

        return _fs.Directory.GetFiles(catalogsDirectoryPath, "*.cat");
    }

    public Task<IReadOnlyList<CatalogEntry>> GetCatalogEntriesByDirectoryAsync(string catalogsDirectoryPath, IProgress<ProgressReport>? progress = null)
    {
        return GetCatalogEntriesByDirectoryInternal(catalogsDirectoryPath, progress, true);
    }

    public IReadOnlyList<CatalogEntry> GetCatalogEntriesByDirectory(string catalogsDirectoryPath, IProgress<ProgressReport>? progress = null)
    {
        return GetCatalogEntriesByDirectoryInternal(catalogsDirectoryPath, progress, false).GetAwaiter().GetResult();
    }

    private async Task<IReadOnlyList<CatalogEntry>> GetCatalogEntriesByDirectoryInternal(string catalogsDirectoryPath, IProgress<ProgressReport>? progress = null, bool sync = true)
    {
        if (string.IsNullOrEmpty(catalogsDirectoryPath))
        {
            _logger?.LogError("Catalog root directory not specified.");
            throw new ArgumentNullException(nameof(catalogsDirectoryPath), "Catalog root directory path is not set.");
        }
        
        IReadOnlyList<string> catalogFilePaths = GetCatalogFilePaths(catalogsDirectoryPath);

        if (catalogFilePaths.Count == 0)
        {
            return new List<CatalogEntry>();
        }

        var catalogEntries = new List<CatalogEntry>();

        for (var index = 0; index < catalogFilePaths.Count; index++)
        {
            string catalogFile = catalogFilePaths[index];
            IReadOnlyList<CatalogEntry> catalogEntriesByFile = await GetCatalogEntriesInternal(catalogFile, sync);

            catalogEntries.AddRange(catalogEntriesByFile);

            progress?.Report(new ProgressReport(catalogFile, index + 1, catalogFilePaths.Count));
        }

        return catalogEntries;
    }

    private async Task<IReadOnlyList<CatalogEntry>> GetCatalogEntriesInternal(string catalogFilePath, bool sync = true)
    {
        if (string.IsNullOrEmpty(catalogFilePath))
        {
            _logger?.LogError("Catalog file path not specified.");
            throw new ArgumentNullException(nameof(catalogFilePath), "Catalog file path not specified.");
        }

        if (!_fs.File.Exists(catalogFilePath))
        {
            _logger?.LogError("Catalog file does not exist: {CatalogFile}", catalogFilePath);
            throw new CatalogFileNotFoundException("Catalog file does not exist", catalogFilePath);
        }
        
        IReadOnlyList<CatalogEntry> catalogEntriesByFile;
        if (sync)
        {
            catalogEntriesByFile = _catalogReader.GetCatalogEntries(catalogFilePath);
        }
        else
        {
            catalogEntriesByFile = await _catalogReader.GetCatalogEntriesAsync(catalogFilePath);
        }

        return catalogEntriesByFile;
    }
}