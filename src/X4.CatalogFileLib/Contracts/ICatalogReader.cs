namespace X4.CatalogFileLib.Contracts;

public interface ICatalogReader
{
    IReadOnlyList<string> GetCatalogFiles(string catalogDirectoryPath);
    
    Task<IReadOnlyList<CatalogEntry>> GetFilesInCatalogAsync(string path, IProgress<ProgressReport>? progress = null);
    
    IReadOnlyList<CatalogEntry> GetFilesInCatalog(string path, IProgress<ProgressReport>? progress = null);
}