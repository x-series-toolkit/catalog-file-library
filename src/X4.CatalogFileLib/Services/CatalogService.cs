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

    public Task<CatalogFile> GetCatalogFileAsync(string catalogFilePath, CancellationToken ct = default)
    {
        if (!string.IsNullOrEmpty(catalogFilePath))
        {
            return _catalogFileReader.GetCatalogFileAsync(catalogFilePath, ct);
        }

        _logger?.LogError("Catalog file path not specified.");
        throw new ArgumentException("Catalog file path not specified.", nameof(catalogFilePath));
    }

    public Task<IReadOnlyList<CatalogFile>> GetCatalogFilesByDirectoryAsync(string catalogsDirectoryPath, CancellationToken ct = default, IProgress<ProgressReport>? progress = null)
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
            return Task.FromResult<IReadOnlyList<CatalogFile>>(new List<CatalogFile>());
        }

        IList<Task<CatalogFile>> tasks = catalogFilePaths.Select(catalogFilePath => GetCatalogFileAsync(catalogFilePath, ct)).ToList();

        if (progress == null)
        {
            return Task.WhenAll(tasks).ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    return (IReadOnlyList<CatalogFile>)t.Result;
                }
                _logger?.LogError(t.Exception, "Error while reading catalog files.");
                throw t.Exception;

            }, ct);
        }

        return tasks.WhenAllWithProgress(_ =>
        {
            int completedTasksCount = _.Count(task => task.IsCompleted);
            int tasksCount = tasks.Count();
            progress?.Report(new ProgressReport(completedTasksCount, tasksCount));
        }).ContinueWith(t =>
        {
            if (!t.IsFaulted)
            {
                progress.Report(new ProgressReport(tasks.Count, tasks.Count));
                return t.Result;
            }

            _logger?.LogError(t.Exception, "Error while reading catalog files.");
            throw t.Exception;
        }, ct, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
    }

    public IReadOnlyList<CatalogFile> GetCatalogFilesInDirectory(string catalogsDirectoryPath, IProgress<ProgressReport>? progress = null)
    {
        if (string.IsNullOrEmpty(catalogsDirectoryPath))
        {
            _logger?.LogError("Catalog root directory not specified.");
            throw new ArgumentException("Catalog root directory path is not set.", nameof(catalogsDirectoryPath));
        }

        string[] catalogFilePaths = _fs.Directory.GetFiles(catalogsDirectoryPath, "*.cat");

        if (catalogFilePaths.Length == 0)
        {
            return new List<CatalogFile>();
        }

        var catalogFiles = new List<CatalogFile>();

        for (var index = 0; index < catalogFilePaths.Length; index++)
        {
            string catalogFilePath = catalogFilePaths[index];

            CatalogFile catalogFile = GetCatalogFile(catalogFilePath);
            catalogFiles.Add(catalogFile);

            progress?.Report(new ProgressReport(index + 1, catalogFilePaths.Length));
        }

        return catalogFiles;
    }

    public IReadOnlyList<CatalogFile> GetCatalogFilesInDirectoryParallel(string catalogsDirectoryPath, IProgress<ProgressReport>? progress = null)
    {
        if (string.IsNullOrEmpty(catalogsDirectoryPath))
        {
            _logger?.LogError("Catalog root directory not specified.");
            throw new ArgumentException("Catalog root directory path is not set.", nameof(catalogsDirectoryPath));
        }

        string[] catalogFilePaths = _fs.Directory.GetFiles(catalogsDirectoryPath, "*.cat");

        if (catalogFilePaths.Length == 0)
        {
            return new List<CatalogFile>();
        }

        var catalogFiles = new ConcurrentBag<CatalogFile>();
        var currentProgress = 0;
        Parallel.For(0, catalogFilePaths.Length, i =>
        {
            string catalogFilePath = catalogFilePaths[i];

            CatalogFile catalogFile = GetCatalogFile(catalogFilePath);
            catalogFiles.Add(catalogFile);

            if (progress == null)
            {
                return;
            }

            int totalProgress = Interlocked.Increment(ref currentProgress);
            progress.Report(new ProgressReport(totalProgress, catalogFilePaths.Length));
        });

        return new List<CatalogFile>(catalogFiles).OrderBy(catalogFile => catalogFile.FilePath).ToList();
    }
    
    public IReadOnlyList<CatalogFile> GetCatalogFilesInMultipleDirectory(params string[] catalogDirectoryPaths)
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

        return catalogFiles;
    }
}