namespace X4.CatalogFileLib;

public record CatalogEntry(string FileName, string AssetPath, int Size, int Timestamp, long ByteOffset, string Md5Hash);

public record ProgressReport(string AssetPath, int Current, int Total);