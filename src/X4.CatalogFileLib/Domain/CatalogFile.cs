namespace X4.CatalogFileLib.Domain;

public class CatalogFile
{
    private readonly List<CatalogEntry> _catalogEntries = new();

    public CatalogFile(ReadOnlySpan<char> catalogFilePath, ReadOnlySpan<char> fileContent)
    {
        if (catalogFilePath == null || catalogFilePath.IsEmpty)
        {
            throw new ArgumentException("Value cannot be null or empty.", nameof(catalogFilePath));
        }

        if (fileContent == null || fileContent.IsEmpty)
        {
            throw new ArgumentException("Value cannot be null or empty.", nameof(fileContent));
        }

        
        long byteOffset = 0;
        int index = fileContent.IndexOf('\n');
        
        do
        {
            if (index == -1)
            {
                if (!fileContent.IsEmpty)
                {
                    _catalogEntries.Add(new CatalogEntry(fileContent, byteOffset));
                }

                break;
            }

            var line = fileContent.Slice(0, index);
            fileContent = fileContent.Slice(index + 1);
            index = fileContent.IndexOf('\n');

            var catalogEntry = new CatalogEntry(line, byteOffset);
            _catalogEntries.Add(catalogEntry);
            byteOffset += catalogEntry.AssetSize;

        } while (index != -1);

        FilePath = catalogFilePath.ToString();
        Size = byteOffset;
    }

    public string FilePath { get; }

    public long Size { get; }

    public IReadOnlyList<CatalogEntry> CatalogEntries => _catalogEntries;
}