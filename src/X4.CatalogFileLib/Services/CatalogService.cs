namespace X4.CatalogFileLib.Services;

public class CatalogService : ICatalogService
{
    private readonly ICatalogFileReader _catalogFileReader;
    private readonly ICatalogAssetExporter _catalogAssetExporter;
    private readonly IFileSystem _fs;
    private readonly ILogger<CatalogService>? _logger;

    public CatalogService(
        ICatalogFileReader catalogFileReader,
        ICatalogAssetExporter catalogAssetExporter,
        IFileSystem? fileSystem = null,
        ILogger<CatalogService>? logger = null)
    {
        _catalogFileReader = catalogFileReader;
        _catalogAssetExporter = catalogAssetExporter;
        _fs = fileSystem ?? new FileSystem();
        _logger = logger;
    }

    public CatalogFile GetCatalogFile(string catalogFilePath)
    {
        if (!string.IsNullOrEmpty(catalogFilePath))
        {
            return _catalogFileReader.GetCatalogFile(catalogFilePath);
        }

        _logger?.LogError("Catalog file path not specified.");
        throw new ArgumentException("Catalog file path not specified.", nameof(catalogFilePath));
    }

    public IImmutableList<CatalogFile> GetCatalogFiles(params string[] catalogFilePaths)
    {
        if (catalogFilePaths is not { Length: 0 })
        {
            return catalogFilePaths.Select(GetCatalogFile).ToImmutableList();
        }

        _logger?.LogError("No catalog files specified.");
        throw new ArgumentException("No catalog files specified.", nameof(catalogFilePaths));
    }

    public IImmutableList<CatalogFile> GetCatalogFilesInDirectory(string catalogsDirectoryPath)
    {
        if (string.IsNullOrEmpty(catalogsDirectoryPath))
        {
            _logger?.LogError("Catalog root directory not specified.");
            throw new ArgumentException("Catalog root directory path is not set.", nameof(catalogsDirectoryPath));
        }

        string[] catalogFilePaths = _fs.Directory.GetFiles(catalogsDirectoryPath, "*.cat");

        if (catalogFilePaths.Length == 0)
        {
            return ImmutableList.Create<CatalogFile>();
        }

        var catalogFiles = catalogFilePaths.Select(GetCatalogFile).ToImmutableList();

        return catalogFiles;
    }

    public IImmutableList<CatalogFile> GetCatalogFilesInDirectoryParallel(string catalogsDirectoryPath)
    {
        if (string.IsNullOrEmpty(catalogsDirectoryPath))
        {
            _logger?.LogError("Catalog root directory not specified.");
            throw new ArgumentException("Catalog root directory path is not set.", nameof(catalogsDirectoryPath));
        }

        string[] catalogFilePaths = _fs.Directory.GetFiles(catalogsDirectoryPath, "*.cat");

        if (catalogFilePaths.Length == 0)
        {
            return ImmutableList.Create<CatalogFile>();
        }

        var catalogFiles = new ConcurrentBag<CatalogFile>();

        Parallel.ForEach(catalogFilePaths, catalogFilePath =>
        {
            CatalogFile catalogFile = GetCatalogFile(catalogFilePath);
            catalogFiles.Add(catalogFile);
        });

        return catalogFiles.OrderBy(catalogFile => catalogFile.FilePath).ToImmutableList();
    }

    public IImmutableList<CatalogFile> GetCatalogFilesInMultipleDirectory(params string[] catalogDirectoryPaths)
    {
        if (catalogDirectoryPaths is { Length: 0 })
        {
            _logger?.LogError("No catalog directories specified.");
            throw new ArgumentException("No catalog directories specified.", nameof(catalogDirectoryPaths));
        }

        var catalogFiles = new List<CatalogFile>();

        foreach (string catalogDirectoryPath in catalogDirectoryPaths)
        {
            catalogFiles.AddRange(GetCatalogFilesInDirectory(catalogDirectoryPath));
        }

        return catalogFiles.ToImmutableList();
    }

    public IImmutableList<CatalogFile> GetCatalogFilesInMultipleDirectoryParallel(params string[] catalogDirectoryPaths)
    {
        if (catalogDirectoryPaths is { Length: 0 })
        {
            _logger?.LogError("No catalog directories specified.");
            throw new ArgumentException("No catalog directories specified.", nameof(catalogDirectoryPaths));
        }

        var catalogFiles = new List<CatalogFile>();

        Parallel.ForEach(catalogDirectoryPaths, catalogDirectoryPath =>
        {
            catalogFiles.AddRange(GetCatalogFilesInDirectory(catalogDirectoryPath));
        });

        return catalogFiles.ToImmutableList();
    }

    public Task<CatalogFile> GetCatalogFileAsync(string catalogFilePath, CancellationToken ct = default)
    {
        if (!string.IsNullOrEmpty(catalogFilePath))
        {
            return _catalogFileReader.GetCatalogFileAsync(catalogFilePath, ct);
        }

        _logger?.LogError("Catalog file path not specified.");
        throw new ArgumentException("Catalog file path not specified.", nameof(catalogFilePath));
    }

    public Task<IImmutableList<CatalogFile>> GetCatalogFilesAsync(CancellationToken ct = default, params string[] catalogFilePaths)
    {
        if (catalogFilePaths is { Length: 0 })
        {
            _logger?.LogError("No catalog files specified.");
            throw new ArgumentException("No catalog files specified.", nameof(catalogFilePaths));
        }

        var tasks = catalogFilePaths.Select(s => GetCatalogFileAsync(s, ct));

        return Task.WhenAll(tasks).ContinueWith(t =>
        {
            if (!t.IsFaulted)
            {
                return (IImmutableList<CatalogFile>)t.Result.ToImmutableList();
            }

            _logger?.LogError(t.Exception, "Error while reading catalog files.");
            throw t.Exception ?? new Exception("Error while reading catalog files.");

        }, ct);
    }

    public Task<IImmutableList<CatalogFile>> GetCatalogFilesInDirectoryAsync(string catalogsDirectoryPath, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(catalogsDirectoryPath))
        {
            _logger?.LogError("Catalog root directory not specified.");
            throw new ArgumentException("Catalog root directory path is not set.", nameof(catalogsDirectoryPath));
        }

        string[] catalogFilePaths = _fs.Directory.GetFiles(catalogsDirectoryPath, "*.cat");

        if (catalogFilePaths.Length == 0)
        {
            _logger?.LogInformation("No catalog files found in: {Directory}", catalogsDirectoryPath);
            return Task.FromResult<IImmutableList<CatalogFile>>(new List<CatalogFile>().ToImmutableList());
        }

        IList<Task<CatalogFile>> tasks = catalogFilePaths.Select(catalogFilePath => GetCatalogFileAsync(catalogFilePath, ct)).ToList();

        return Task.WhenAll(tasks).ContinueWith(t =>
        {
            if (!t.IsFaulted)
            {
                return (IImmutableList<CatalogFile>)t.Result.ToImmutableList();
            }
            _logger?.LogError(t.Exception, "Error while reading catalog files.");
            throw t.Exception ?? new Exception("Error while reading catalog files.");

        }, ct);
    }

    public async Task<IImmutableList<CatalogFile>> GetCatalogFilesInMultipleDirectoryAsync(CancellationToken ct = default, params string[] catalogDirectoryPaths)
    {
        if (catalogDirectoryPaths is { Length: 0 })
        {
            _logger?.LogError("No catalog directories specified.");
            throw new ArgumentException("No catalog directories specified.", nameof(catalogDirectoryPaths));
        }

        var tasks = catalogDirectoryPaths
            .Select(catalogDirectoryPath => GetCatalogFilesInDirectoryAsync(catalogDirectoryPath, ct))
            .ToList();

        var files = (await Task.WhenAll(tasks).ContinueWith(t =>
        {
            if (!t.IsFaulted)
            {
                return t.Result;
            }
            _logger?.LogError(t.Exception, "Error while reading catalog files.");
            throw t.Exception ?? new Exception("Error while reading catalog files.");

        }, ct)).SelectMany(list => list).ToList();

        return files.ToImmutableList();
    }
}