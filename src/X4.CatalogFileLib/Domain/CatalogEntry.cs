namespace X4.CatalogFileLib.Domain;

public class CatalogEntry
{
    public CatalogEntry(string catalogFilePath, string assetRow, long byteOffset)
    {
        if (assetRow == null)
        {
            throw new ArgumentNullException(nameof(assetRow));
        }

        string[] columns = assetRow.Split(' ');

        string assetPath;
        int assetSize;
        int timeStamp;
        string md5Hash;

        switch (columns.Length)
        {
            case 4:
                assetPath = columns[0];
                int.TryParse(columns[1], out assetSize);
                int.TryParse(columns[2], out timeStamp);
                md5Hash = columns[3];
                break;
            //Possible spaces in file path. try to fix
            case > 4:
                assetPath = string.Join(" ", columns.Take(columns.Length - 3));

                int.TryParse(columns[columns.Length - 3], out assetSize);
                int.TryParse(columns[columns.Length - 2], out timeStamp);
                md5Hash = columns[columns.Length - 1];
                break;
            //The row is missing columns. Can't recover
            default:
                throw new CorruptedCatalogFileException($"The row is missing columns: {assetRow}");
        }

        CatalogFilePath = catalogFilePath;
        AssetPath = assetPath;
        AssetSize = assetSize;
        TimeStamp = timeStamp;
        ByteOffset = byteOffset;
        Md5Hash = md5Hash;
    }

    public string CatalogFilePath { get; }
    
    public string AssetPath { get; }
    
    public int AssetSize { get; }
    
    public int TimeStamp { get; }
    
    public long ByteOffset { get; }
    
    public string Md5Hash { get; }
}