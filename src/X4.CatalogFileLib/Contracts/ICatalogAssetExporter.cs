namespace X4.CatalogFileLib.Contracts;

public interface ICatalogAssetExporter
{
    void ExportAsset(CatalogEntry catalogEntry, string destFileName);
}