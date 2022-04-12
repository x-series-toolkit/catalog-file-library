namespace X4.CatalogFileLib;

public class CatalogReader : ICatalogReader
{
    private readonly IFileSystem _fileSystem;

    public CatalogReader(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public IReadOnlyList<string> GetCatalogFiles(string catalogDirectoryPath)
    {
        return _fileSystem.Directory.GetFiles(catalogDirectoryPath, "*.cat");
    }

    public async Task<IReadOnlyList<CatalogEntry>> GetFilesInCatalogAsync(string path, IProgress<ProgressReport>? progress = null)
    {
        IFileInfo catalogFile = _fileSystem.FileInfo.FromFileName(path);

        if (!catalogFile.Exists)
        {
            throw new Exception($"No such cat file: {path}");
        }

        if (catalogFile.Extension != ".cat")
        {
            throw new Exception("Invalid cat file extension. Should be .cat");
        }

        using StreamReader? reader = catalogFile.OpenText();
        string fileContent = await reader.ReadToEndAsync();
        string[] assets = fileContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        return ParseCatalogFile(path, assets, progress);
    }

    public IReadOnlyList<CatalogEntry> GetFilesInCatalog(string path, IProgress<ProgressReport>? progress = null)
    {
        IFileInfo catalogFile = _fileSystem.FileInfo.FromFileName(path);

        if (!catalogFile.Exists)
        {
            throw new Exception($"No such cat file: {path}");
        }

        if (catalogFile.Extension != ".cat")
        {
            throw new Exception("Invalid cat file extension. Should be .cat");
        }

        using StreamReader? reader = catalogFile.OpenText();
        string[] assets = _fileSystem.File.ReadAllLines(path);

        return ParseCatalogFile(path, assets, progress);
    }

    private static IReadOnlyList<CatalogEntry> ParseCatalogFile(string path, IReadOnlyList<string> assets, IProgress<ProgressReport>? progress = null)
    {
        var entries = new List<CatalogEntry>();
        long byteOffset = 0;

        for (var index = 0; index < assets.Count; index++)
        {
            string asset = assets[index];
            string[] columns = asset.Split(' ');

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
                    throw new Exception($"File {path} has invalid format");
            }

            var catalogEntry = new CatalogEntry(path, assetPath, assetSize, timeStamp, byteOffset, md5Hash);
            entries.Add(catalogEntry);

            byteOffset += assetSize;
            progress?.Report(new ProgressReport(assetPath, index, assets.Count));
        }

        return entries;
    }
}