namespace X4.Tests.Sandboxes.Benchmarks.Benchmarks.SplitSpan;

public class CatalogEntry2
{
    public CatalogEntry2(string assetLine, long byteOffset)
    {
        if (string.IsNullOrEmpty(assetLine))
        {
            throw new ArgumentException("Value cannot be null or empty.", nameof(assetLine));
        }


        string[] columns = assetLine.Split(' ');

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
                throw new CorruptedCatalogFileException($"The line is missing columns: {assetLine}");
        }

        AssetPath = assetPath;
        AssetSize = assetSize;
        DateUtc = UnixTimestampToDateTime(timeStamp);
        ByteOffset = byteOffset;
        Md5Hash = md5Hash;
    }

    public string AssetPath { get; }
    
    public int AssetSize { get; }
    
    public DateTime DateUtc { get; }
    
    public long ByteOffset { get; }
    
    public string Md5Hash { get; }

    private static DateTime UnixTimestampToDateTime(double unixTime)
    {
        var unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        var unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
        return new DateTime(unixStart.Ticks + unixTimeStampInTicks, DateTimeKind.Utc);
    }
}