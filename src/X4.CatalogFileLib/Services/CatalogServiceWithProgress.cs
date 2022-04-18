namespace X4.CatalogFileLib.Services;

public class CatalogServiceWithProgress : ICatalogServiceWithProgress
{
    private readonly ICatalogService _catalogService;
    private readonly IFileSystem _fs;
    private readonly ILogger<CatalogServiceWithProgress>? _logger;

    public CatalogServiceWithProgress(
        ICatalogService catalogService, 
        IFileSystem? fileSystem = null,
        ILogger<CatalogServiceWithProgress>? logger = null)
    {
        _catalogService = catalogService;
        _fs = fileSystem ?? new FileSystem();
        _logger = logger;
    }

    public CatalogFile GetCatalogFile(string catalogFilePath)
    {
        return _catalogService.GetCatalogFile(catalogFilePath);
    }

    public IImmutableList<CatalogFile> GetCatalogFiles(params string[] catalogFilePaths)
    {
        return _catalogService.GetCatalogFiles(catalogFilePaths);
    }

    public IImmutableList<CatalogFile> GetCatalogFiles(IProgress<ProgressReport> progress, params string[] catalogFilePaths)
    {
        if (catalogFilePaths is { Length: 0 })
        {
            _logger?.LogError("No catalog files specified.");
            throw new ArgumentException("No catalog files specified.", nameof(catalogFilePaths));
        }

        var catalogFiles = new List<CatalogFile>(catalogFilePaths.Length);

        for (var index = 0; index < catalogFilePaths.Length; index++)
        {
            string catalogFilePath = catalogFilePaths[index];
            CatalogFile catalogFile = GetCatalogFile(catalogFilePath);
            catalogFiles.Add(catalogFile);

            progress.Report(new ProgressReport(index + 1, catalogFilePaths.Length));
        }

        return catalogFiles.ToImmutableList();
    }

    public IImmutableList<CatalogFile> GetCatalogFilesInDirectory(string catalogsDirectoryPath)
    {
        return _catalogService.GetCatalogFilesInDirectory(catalogsDirectoryPath);
    }

    public IImmutableList<CatalogFile> GetCatalogFilesInDirectory(string catalogsDirectoryPath, IProgress<ProgressReport> progress)
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

        var catalogFiles = new List<CatalogFile>();

        for (var index = 0; index < catalogFilePaths.Length; index++)
        {
            string catalogFilePath = catalogFilePaths[index];

            CatalogFile catalogFile = GetCatalogFile(catalogFilePath);
            catalogFiles.Add(catalogFile);

            progress.Report(new ProgressReport(index + 1, catalogFilePaths.Length));
        }

        return catalogFiles.ToImmutableList();
    }

    public IImmutableList<CatalogFile> GetCatalogFilesInDirectoryParallel(string catalogsDirectoryPath)
    {
        return _catalogService.GetCatalogFilesInMultipleDirectoryParallel(catalogsDirectoryPath);
    }

    public IImmutableList<CatalogFile> GetCatalogFilesInDirectoryParallel(string catalogsDirectoryPath, IProgress<ProgressReport> progress)
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
        var currentProgress = 0;
        Parallel.For(0, catalogFilePaths.Length, i =>
        {
            string catalogFilePath = catalogFilePaths[i];

            CatalogFile catalogFile = GetCatalogFile(catalogFilePath);
            catalogFiles.Add(catalogFile);

            int totalProgress = Interlocked.Increment(ref currentProgress);
            progress.Report(new ProgressReport(totalProgress, catalogFilePaths.Length));
        });

        return catalogFiles.OrderBy(catalogFile => catalogFile.FilePath).ToImmutableList();
    }

    public IImmutableList<CatalogFile> GetCatalogFilesInMultipleDirectory(params string[] catalogDirectoryPaths)
    {
        return _catalogService.GetCatalogFilesInMultipleDirectory(catalogDirectoryPaths);
    }

    public IImmutableList<CatalogFile> GetCatalogFilesInMultipleDirectory(IProgress<ProgressReport> progress, params string[] catalogDirectoryPaths)
    {
        if (catalogDirectoryPaths is { Length: 0 })
        {
            _logger?.LogError("No catalog directories specified.");
            throw new ArgumentException("No catalog directories specified.", nameof(catalogDirectoryPaths));
        }

        var catalogFiles = new List<CatalogFile>();

        for (var i = 0; i < catalogDirectoryPaths.Length; i++)
        {
            string catalogDirectoryPath = catalogDirectoryPaths[i];
            int index = i;
            catalogFiles.AddRange(GetCatalogFilesInDirectory(catalogDirectoryPath,
                new Progress<ProgressReport>(
                    report =>
                    {
                        (int completed, int total) = report;
                        if (total == completed)
                        {
                            progress.Report(new ProgressReport(index + 1, catalogDirectoryPaths.Length));
                        }
                    })));
        }

        return catalogFiles.ToImmutableList();
    }

    public IImmutableList<CatalogFile> GetCatalogFilesInMultipleDirectoryParallel(params string[] catalogDirectoryPaths)
    {
        return _catalogService.GetCatalogFilesInMultipleDirectoryParallel(catalogDirectoryPaths);
    }

    public IImmutableList<CatalogFile> GetCatalogFilesInMultipleDirectoryParallel(IProgress<ProgressReport> progress, params string[] catalogDirectoryPaths)
    {
        if (catalogDirectoryPaths is { Length: 0 })
        {
            _logger?.LogError("No catalog directories specified.");
            throw new ArgumentException("No catalog directories specified.", nameof(catalogDirectoryPaths));
        }

        var catalogFiles = new ConcurrentBag<IImmutableList<CatalogFile>>();
        var currentProgress = 0;

        Parallel.For(0, catalogDirectoryPaths.Length, i =>
        {
            string catalogDirectoryPath = catalogDirectoryPaths[i];

            var catalogFilesInDirectory = GetCatalogFilesInDirectory(catalogDirectoryPath);
            catalogFiles.Add(catalogFilesInDirectory);

            int totalProgress = Interlocked.Increment(ref currentProgress);
            progress.Report(new ProgressReport(totalProgress, catalogDirectoryPaths.Length));
        });

        return catalogFiles.SelectMany(list => list).OrderBy(file => file.FilePath).ToImmutableList();
    }

    public Task<CatalogFile> GetCatalogFileAsync(string catalogFilePath, CancellationToken ct = default)
    {
        return _catalogService.GetCatalogFileAsync(catalogFilePath, ct);
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

    public async Task<IImmutableList<CatalogFile>> GetCatalogFilesAsync(IProgress<ProgressReport> progress, CancellationToken ct = default, params string[] catalogFilePaths)
    {
        if (catalogFilePaths is { Length: 0 })
        {
            _logger?.LogError("No catalog files specified.");
            throw new ArgumentException("No catalog files specified.", nameof(catalogFilePaths));
        }

        var tasks = catalogFilePaths.Select(s => GetCatalogFileAsync(s, ct)).ToList();

        var files = (await tasks.WhenAllWithProgress(_ =>
        {
            int completedTasksCount = _.Count(task => task.IsCompleted);
            int tasksCount = tasks.Count;
            progress.Report(new ProgressReport(completedTasksCount, tasksCount));
        }).ContinueWith(t =>
        {
            if (!t.IsFaulted)
            {
                progress.Report(new ProgressReport(tasks.Count, tasks.Count));
                return t.Result;
            }

            _logger?.LogError(t.Exception, "Error while reading catalog files.");
            throw t.Exception ?? new Exception("Error while reading catalog files.");
        }, ct)).ToImmutableList();

        return files;
    }

    public Task<IImmutableList<CatalogFile>> GetCatalogFilesInDirectoryAsync(string catalogsDirectoryPath, CancellationToken ct = default)
    {
        return _catalogService.GetCatalogFilesInDirectoryAsync(catalogsDirectoryPath, ct);
    }

    public Task<IImmutableList<CatalogFile>> GetCatalogFilesInDirectoryAsync(string catalogsDirectoryPath, IProgress<ProgressReport> progress, CancellationToken ct = default)
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

        return tasks.WhenAllWithProgress(_ =>
        {
            int completedTasksCount = _.Count(task => task.IsCompleted);
            int tasksCount = tasks.Count;
            progress.Report(new ProgressReport(completedTasksCount, tasksCount));
        }).ContinueWith(t =>
        {
            if (!t.IsFaulted)
            {
                progress.Report(new ProgressReport(tasks.Count, tasks.Count));
                return (IImmutableList<CatalogFile>)t.Result.ToImmutableList();
            }

            _logger?.LogError(t.Exception, "Error while reading catalog files.");
            throw t.Exception ?? new Exception("Error while reading catalog files.");
        }, ct);
    }

    public Task<IImmutableList<CatalogFile>> GetCatalogFilesInMultipleDirectoryAsync(CancellationToken ct = default, params string[] catalogDirectoryPaths)
    {
        return _catalogService.GetCatalogFilesInMultipleDirectoryAsync(ct, catalogDirectoryPaths);
    }

    public async Task<IImmutableList<CatalogFile>> GetCatalogFilesInMultipleDirectoryAsync(IProgress<ProgressReport> progress, CancellationToken ct = default, params string[] catalogDirectoryPaths)
    {
        if (catalogDirectoryPaths is { Length: 0 })
        {
            _logger?.LogError("No catalog directories specified.");
            throw new ArgumentException("No catalog directories specified.", nameof(catalogDirectoryPaths));
        }

        var tasks = catalogDirectoryPaths
            .Select(catalogDirectoryPath => GetCatalogFilesInDirectoryAsync(catalogDirectoryPath, ct))
            .ToList();

        var files = (await tasks.WhenAllWithProgress(_ =>
        {
            int completedTasksCount = _.Count(task => task.IsCompleted);
            int tasksCount = tasks.Count;
            progress.Report(new ProgressReport(completedTasksCount, tasksCount));
        }).ContinueWith(t =>
        {
            if (!t.IsFaulted)
            {
                progress.Report(new ProgressReport(tasks.Count, tasks.Count));
                return t.Result;
            }

            _logger?.LogError(t.Exception, "Error while reading catalog files.");
            throw t.Exception ?? new Exception("Error while reading catalog files.");
        }, ct)).SelectMany(list => list).ToImmutableList();

        return files;
    }
}