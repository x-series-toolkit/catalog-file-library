namespace X4.CatalogFileLib.Contracts;

public interface ICatalogAssetExporter
{
    void ExportAsset(string sourceFile, CatalogEntry catalogEntry, string destDirectory, string? destFileName = null);
    
    Task ExportAssetAsync(string sourceFile, CatalogEntry catalogEntry, string destDirectory, string? destFileName = null, CancellationToken ct = default);
}