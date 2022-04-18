namespace X4.CatalogFileLib.Contracts;

public interface ICatalogServiceWithProgress : ICatalogService
{
    IImmutableList<CatalogFile> GetCatalogFiles(IProgress<ProgressReport> progress, params string[] catalogFilePaths);

    IImmutableList<CatalogFile> GetCatalogFilesInDirectory(string catalogsDirectoryPath, IProgress<ProgressReport> progress);

    IImmutableList<CatalogFile> GetCatalogFilesInDirectoryParallel(string catalogsDirectoryPath, IProgress<ProgressReport> progress);

    IImmutableList<CatalogFile> GetCatalogFilesInMultipleDirectory(IProgress<ProgressReport> progress, params string[] catalogDirectoryPaths);

    IImmutableList<CatalogFile> GetCatalogFilesInMultipleDirectoryParallel(IProgress<ProgressReport> progress, params string[] catalogDirectoryPaths);

    Task<IImmutableList<CatalogFile>> GetCatalogFilesAsync(IProgress<ProgressReport> progress, CancellationToken ct = default, params string[] catalogFilePaths);

    Task<IImmutableList<CatalogFile>> GetCatalogFilesInDirectoryAsync(string catalogsDirectoryPath, IProgress<ProgressReport> progress, CancellationToken ct = default);

    Task<IImmutableList<CatalogFile>> GetCatalogFilesInMultipleDirectoryAsync(IProgress<ProgressReport> progress, CancellationToken ct = default, params string[] catalogDirectoryPaths);
}