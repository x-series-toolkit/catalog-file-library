namespace X4.Tests.Sandboxes.Benchmarks.Benchmarks.SplitSpan;

public class CatalogFile2
{
    private readonly List<CatalogEntry2> _catalogEntries = new();

    public CatalogFile2(string catalogFilePath, string fileContent)
    {
        if (string.IsNullOrEmpty(catalogFilePath))
        {
            throw new ArgumentNullException(nameof(catalogFilePath));
        }

        if (string.IsNullOrEmpty(fileContent))
        {
            throw new ArgumentException("Value cannot be null or empty.", nameof(fileContent));
        }


        string[] lines = fileContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        long byteOffset = 0;

        foreach (string line in lines)
        {
            var catalogEntry = new CatalogEntry2(line, byteOffset);

            _catalogEntries.Add(catalogEntry);

            byteOffset += catalogEntry.AssetSize;
        }

        FilePath = catalogFilePath.ToString();
        Size = byteOffset;
    }


    public string FilePath { get; }

    public long Size { get; }

    public IReadOnlyList<CatalogEntry2> CatalogEntries => _catalogEntries;
}