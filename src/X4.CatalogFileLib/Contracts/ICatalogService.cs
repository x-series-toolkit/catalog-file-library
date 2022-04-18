namespace X4.CatalogFileLib.Contracts;

public interface ICatalogService
{
    CatalogFile GetCatalogFile(string catalogFilePath);
    
    IImmutableList<CatalogFile> GetCatalogFiles(params string[] catalogFilePaths);

    IImmutableList<CatalogFile> GetCatalogFilesInDirectory(string catalogsDirectoryPath);

    IImmutableList<CatalogFile> GetCatalogFilesInDirectoryParallel(string catalogsDirectoryPath);

    IImmutableList<CatalogFile> GetCatalogFilesInMultipleDirectory(params string[] catalogDirectoryPaths);

    IImmutableList<CatalogFile> GetCatalogFilesInMultipleDirectoryParallel(params string[] catalogDirectoryPaths);

    Task<CatalogFile> GetCatalogFileAsync(string catalogFilePath, CancellationToken ct = default);
    
    Task<IImmutableList<CatalogFile>> GetCatalogFilesAsync(CancellationToken ct = default, params string[] catalogFilePaths);

    Task<IImmutableList<CatalogFile>> GetCatalogFilesInDirectoryAsync(string catalogsDirectoryPath, CancellationToken ct = default);

    Task<IImmutableList<CatalogFile>> GetCatalogFilesInMultipleDirectoryAsync(CancellationToken ct = default, params string[] catalogDirectoryPaths);
}