namespace X4.CatalogFileLib.Contracts;

public interface ICatalogFileReader
{
    CatalogFile GetCatalogFile(string catalogFilePath);
    
    Task<CatalogFile> GetCatalogFileAsync(string catalogFilePath, CancellationToken ct = default);
}