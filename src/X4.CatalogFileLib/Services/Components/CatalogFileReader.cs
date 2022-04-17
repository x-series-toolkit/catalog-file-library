namespace X4.CatalogFileLib.Services.Components;

public class CatalogFileReader : ICatalogFileReader
{
    private readonly IFileSystem _fs;
    private readonly ILogger<CatalogFileReader>? _logger;

    public CatalogFileReader(IFileSystem? fileSystem = null, ILogger<CatalogFileReader>? logger = null)
    {
        _fs = fileSystem ?? new FileSystem();
        _logger = logger;
    }

    public CatalogFile GetCatalogFile(string catalogFilePath)
    {
        IFileInfo catalogFileInfo = _fs.FileInfo.FromFileName(catalogFilePath);

        if (!catalogFileInfo.Exists)
        {
            _logger?.LogError("Catalog file does not exist: {CatalogFile}", catalogFilePath);
            throw new CatalogFileNotFoundException("Catalog file does not exist", catalogFilePath);
        }

        if (catalogFileInfo.Extension != ".cat")
        {
            _logger?.LogError("The specified file is not a catalog file: {CatalogFile}", catalogFilePath);
            throw new InvalidCatalogFileException("File is not a catalog file");
        }

        _logger?.LogTrace("Reading catalog file: {CatalogFile}", catalogFilePath);

        string fileContent = _fs.File.ReadAllText(catalogFilePath);

        var catalogFile = new CatalogFile(catalogFileInfo.FullName.AsSpan(), fileContent.AsSpan());

        return catalogFile;
    }

    public async Task<CatalogFile> GetCatalogFileAsync(string catalogFilePath, CancellationToken ct = default)
    {
        IFileInfo catalogFileInfo = _fs.FileInfo.FromFileName(catalogFilePath);

        if (!catalogFileInfo.Exists)
        {
            _logger?.LogError("Catalog file does not exist: {CatalogFile}", catalogFilePath);
            throw new CatalogFileNotFoundException("Catalog file does not exist", catalogFilePath);
        }

        if (catalogFileInfo.Extension != ".cat")
        {
            _logger?.LogError("The specified file is not a catalog file: {CatalogFile}", catalogFilePath);
            throw new InvalidCatalogFileException("File is not a catalog file");
        }

        _logger?.LogTrace("Reading catalog file: {CatalogFile}", catalogFilePath);

        using StreamReader reader = catalogFileInfo.OpenText();
        string fileContent = await reader.ReadToEndAsync().WaitAsync(ct);

        var catalogFile = new CatalogFile(catalogFileInfo.FullName.AsSpan(), fileContent.AsSpan());

        return catalogFile;
    }
}