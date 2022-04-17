namespace X4.CatalogFileLib.Domain;

public class CatalogEntry
{
    public CatalogEntry(ReadOnlySpan<char> assetLine, long byteOffset)
    {
        if (assetLine == null || assetLine.IsEmpty)
        {
            throw new ArgumentException("Value cannot be null or empty.", nameof(assetLine));
        }

        int index = assetLine.LastIndexOf(' ') + 1;

        Md5Hash = assetLine.Slice(index, assetLine.Length - index).ToString();

        assetLine = assetLine.Slice(0, index - 1);
        index = assetLine.LastIndexOf(' ') + 1;
#if NETSTANDARD2_0 || NET461
        int timeStamp = int.Parse(assetLine.Slice(index, assetLine.Length - index).ToString());
        DateUtc = UnixTimestampToDateTime(timeStamp);
#else
        int timeStamp = int.Parse(assetLine.Slice(index, assetLine.Length - index));
        DateUtc = UnixTimestampToDateTime(timeStamp);
#endif
        assetLine = assetLine.Slice(0, index - 1);
        index = assetLine.LastIndexOf(' ') + 1;

#if NETSTANDARD2_0 || NET461
        AssetSize = int.Parse(assetLine.Slice(index, assetLine.Length - index).ToString());
#else
        AssetSize = int.Parse(assetLine.Slice(index, assetLine.Length - index));
#endif
        AssetPath = assetLine.Slice(0, index - 1).ToString();

        ByteOffset = byteOffset;
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