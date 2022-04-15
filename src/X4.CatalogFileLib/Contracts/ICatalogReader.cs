namespace X4.CatalogFileLib.Contracts;

public interface ICatalogReader
{
    //IReadOnlyList<string> GetCatalogFiles(string catalogDirectoryPath);
    
    Task<IReadOnlyList<CatalogEntry>> GetCatalogEntriesAsync(string catalogFilePath, IProgress<ProgressReport>? progress = null);
    
    IReadOnlyList<CatalogEntry> GetCatalogEntries(string catalogFilePath, IProgress<ProgressReport>? progress = null);
}