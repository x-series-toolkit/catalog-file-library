namespace X4.CatalogFileLib.Contracts;

public interface ICatalogService
{
    CatalogFile GetCatalogFile(string catalogFilePath);
    
    Task<CatalogFile> GetCatalogFileAsync(string catalogFilePath, CancellationToken ct = default);
    
    Task<IReadOnlyList<CatalogFile>> GetCatalogFilesByDirectoryAsync(string catalogsDirectoryPath, CancellationToken ct = default, IProgress<ProgressReport>? progress = null);
    
    IReadOnlyList<CatalogFile> GetCatalogFilesByDirectory(string catalogsDirectoryPath, IProgress<ProgressReport>? progress = null);
}