using X4.CatalogFileLib.Streams;

namespace X4.CatalogFileLib.Services.Components;

public class CatalogAssetExporter : ICatalogAssetExporter
{
    private readonly IFileSystem _fs;
    private readonly ILogger<CatalogAssetExporter>? _logger;

    public CatalogAssetExporter(IFileSystem? fileSystem = null, ILogger<CatalogAssetExporter>? logger = null)
    {
        _fs = fileSystem ?? new FileSystem();
        _logger = logger;
    }

    public void ExportAsset(string sourceFile, CatalogEntry catalogEntry, string destDirectory, string? destFileName = null)
    {
        IFileInfo catalogFileInfo = _fs.FileInfo.FromFileName(sourceFile);

        if (!catalogFileInfo.Exists)
        {
            _logger?.LogError("Catalog file does not exist: {CatalogFile}", sourceFile);
            throw new CatalogFileNotFoundException("Catalog file does not exist", sourceFile);
        }

        if (catalogEntry == null)
        {
            _logger?.LogError("Catalog entry is null");
            throw new ArgumentNullException(nameof(catalogEntry));
        }

        if (string.IsNullOrEmpty(destDirectory))
        {
            _logger?.LogError("Destination directory is null or empty");
            throw new ArgumentException("Value cannot be null or empty.", nameof(destDirectory));
        }

        if (destFileName != null && string.IsNullOrEmpty(destDirectory))
        {
            _logger?.LogError("Destination file empty");
            throw new ArgumentException("Value cannot be empty.", nameof(destFileName));
        }

        string catalogFileName = _fs.Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
        string datFilePath = _fs.Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");

        IFileInfo datFileInfo = _fs.FileInfo.FromFileName(datFilePath);

        if (!datFileInfo.Exists)
        {
            _logger?.LogError("Dat file does not exist: {DatFile}", datFilePath);
            throw new DatFileNotFoundException("Dat file does not exist", datFilePath);
        }

        string destFilePath = _fs.Path.Combine(destDirectory, destFileName ?? catalogEntry.AssetPath);
        IFileInfo destFile = _fs.FileInfo.FromFileName(destFilePath);

        if (destFile.Directory.Exists)
        {
            destFile.Directory.Create();
        }

        using Stream stream = datFileInfo.OpenRead();
        stream.Seek(catalogEntry.ByteOffset, SeekOrigin.Begin);
        var newFileData = new byte[catalogEntry.AssetSize];
        int read = stream.Read(newFileData, 0, catalogEntry.AssetSize);

        if (read != catalogEntry.AssetSize)
        {
            _logger?.LogError("Could not read asset data from dat file: {DatFile}", datFilePath);
            throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
        }

        //using Stream destStream = _fs.File.Open(destFile.FullName, FileMode.Create);
        //destStream.Write(newFileData);
        //destStream.Close();

        using Stream destStream = new MemoryStream();
        destStream.Write(newFileData);
        destStream.Close();
    }

    public void ExportAssetSubStream(string sourceFile, CatalogEntry catalogEntry, string destDirectory, string? destFileName = null)
    {
        IFileInfo catalogFileInfo = _fs.FileInfo.FromFileName(sourceFile);

        if (!catalogFileInfo.Exists)
        {
            _logger?.LogError("Catalog file does not exist: {CatalogFile}", sourceFile);
            throw new CatalogFileNotFoundException("Catalog file does not exist", sourceFile);
        }

        if (catalogEntry == null)
        {
            _logger?.LogError("Catalog entry is null");
            throw new ArgumentNullException(nameof(catalogEntry));
        }

        if (string.IsNullOrEmpty(destDirectory))
        {
            _logger?.LogError("Destination directory is null or empty");
            throw new ArgumentException("Value cannot be null or empty.", nameof(destDirectory));
        }

        if (destFileName != null && string.IsNullOrEmpty(destDirectory))
        {
            _logger?.LogError("Destination file empty");
            throw new ArgumentException("Value cannot be empty.", nameof(destFileName));
        }

        string catalogFileName = _fs.Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
        string datFilePath = _fs.Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");

        IFileInfo datFileInfo = _fs.FileInfo.FromFileName(datFilePath);

        if (!datFileInfo.Exists)
        {
            _logger?.LogError("Dat file does not exist: {DatFile}", datFilePath);
            throw new DatFileNotFoundException("Dat file does not exist", datFilePath);
        }

        string destFilePath = _fs.Path.Combine(destDirectory, destFileName ?? catalogEntry.AssetPath);
        IFileInfo destFile = _fs.FileInfo.FromFileName(destFilePath);

        if (destFile.Directory.Exists)
        {
            destFile.Directory.Create();
        }

        using Stream stream = datFileInfo.OpenRead();
        using var subStream = new SubStream(stream, catalogEntry.ByteOffset, catalogEntry.AssetSize);
        //using Stream destStream = _fs.File.Open(destFile.FullName, FileMode.Create);
        using Stream destStream = new MemoryStream();
        subStream.CopyTo(destStream);
        destStream.Close();
    }

    public async Task ExportAssetAsync(string sourceFile, CatalogEntry catalogEntry, string destDirectory, string? destFileName = null, CancellationToken ct = default)
    {
        IFileInfo catalogFileInfo = _fs.FileInfo.FromFileName(sourceFile);

        if (!catalogFileInfo.Exists)
        {
            _logger?.LogError("Catalog file does not exist: {CatalogFile}", sourceFile);
            throw new CatalogFileNotFoundException("Catalog file does not exist", sourceFile);
        }

        if (catalogEntry == null)
        {
            _logger?.LogError("Catalog entry is null");
            throw new ArgumentNullException(nameof(catalogEntry));
        }

        if (string.IsNullOrEmpty(destDirectory))
        {
            _logger?.LogError("Destination directory is null or empty");
            throw new ArgumentException("Value cannot be null or empty.", nameof(destDirectory));
        }

        if (destFileName != null && string.IsNullOrEmpty(destDirectory))
        {
            _logger?.LogError("Destination file empty");
            throw new ArgumentException("Value cannot be empty.", nameof(destFileName));
        }

        string catalogFileName = _fs.Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
        string datFilePath = _fs.Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");

        IFileInfo datFileInfo = _fs.FileInfo.FromFileName(datFilePath);

        if (!datFileInfo.Exists)
        {
            _logger?.LogError("Dat file does not exist: {DatFile}", datFilePath);
            throw new DatFileNotFoundException("Dat file does not exist", datFilePath);
        }

        string destFilePath = _fs.Path.Combine(destDirectory, destFileName ?? catalogEntry.AssetPath);
        IFileInfo destFile = _fs.FileInfo.FromFileName(destFilePath);

        if (destFile.Directory.Exists)
        {
            destFile.Directory.Create();
        }

#if NET461 || NETSTANDARD2_0
        using Stream stream = datFileInfo.OpenRead();

        stream.Seek(catalogEntry.ByteOffset, SeekOrigin.Begin);
        var newFileData = new byte[catalogEntry.AssetSize];
        int read = await stream.ReadAsync(newFileData, 0, catalogEntry.AssetSize, ct);

        if (read != catalogEntry.AssetSize)
        {
            _logger?.LogError("Could not read asset data from dat file: {DatFile}", datFilePath);
            throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
        }


        using Stream destStream = _fs.File.Open(destFile.FullName, FileMode.Create);
        await destStream.WriteAsync(newFileData, 0, catalogEntry.AssetSize, ct);
        destStream.Close();
#else
        await using Stream stream = datFileInfo.OpenRead();
        stream.Seek(catalogEntry.ByteOffset, SeekOrigin.Begin);
        var newFileData = new byte[catalogEntry.AssetSize];
        int read = await stream.ReadAsync(newFileData, 0, catalogEntry.AssetSize, ct);

        if (read != catalogEntry.AssetSize)
        {
            _logger?.LogError("Could not read asset data from dat file: {DatFile}", datFilePath);
            throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
        }

        await using Stream destStream = _fs.File.Open(destFile.FullName, FileMode.Create);
        await destStream.WriteAsync(newFileData, 0, catalogEntry.AssetSize, ct);
        destStream.Close();
#endif
    }

    public async Task ExportAssetsAsync(CatalogFile catalogFile, string destDirectory, CancellationToken ct = default)
    {
        if (catalogFile == null)
        {
            _logger?.LogError("Catalog file is null");
            throw new ArgumentNullException(nameof(catalogFile));
        }

        IFileInfo catalogFileInfo = _fs.FileInfo.FromFileName(catalogFile.FilePath);

        if (!catalogFileInfo.Exists)
        {
            _logger?.LogError("Catalog file does not exist: {CatalogFile}", catalogFile.FilePath);
            throw new CatalogFileNotFoundException("Catalog file does not exist", catalogFile.FilePath);
        }

        if (string.IsNullOrEmpty(destDirectory))
        {
            _logger?.LogError("Destination directory is null or empty");
            throw new ArgumentException("Value cannot be null or empty.", nameof(destDirectory));
        }

        string catalogFileName = _fs.Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
        string datFilePath = _fs.Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");

        IFileInfo datFileInfo = _fs.FileInfo.FromFileName(datFilePath);

        if (!datFileInfo.Exists)
        {
            _logger?.LogError("Dat file does not exist: {DatFile}", datFilePath);
            throw new DatFileNotFoundException("Dat file does not exist", datFilePath);
        }

        ArrayPool<byte> bufferPool = ArrayPool<byte>.Shared;

#if NET461 || NETSTANDARD2_0
        using Stream stream = datFileInfo.OpenRead();
        foreach (CatalogEntry catalogEntry in catalogFile.CatalogEntries)
        {
            string destFilePath = _fs.Path.Combine(destDirectory, catalogEntry.AssetPath);
            IFileInfo destFile = _fs.FileInfo.FromFileName(destFilePath);

            if (!destFile.Directory.Exists)
            {
                //destFile.Directory.Create();
            }

            stream.Seek(catalogEntry.ByteOffset, SeekOrigin.Begin);

            byte[] newFileData = bufferPool.Rent(catalogEntry.AssetSize);
            int read = await stream.ReadAsync(newFileData, 0, catalogEntry.AssetSize, ct);

            if (read != catalogEntry.AssetSize)
            {
                _logger?.LogError("Could not read asset data from dat file: {DatFile}", datFilePath);
                throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
            }

            using Stream destStream = _fs.File.Open(destFile.FullName, FileMode.Create);

            await destStream.WriteAsync(newFileData, 0, catalogEntry.AssetSize, ct);
            destStream.Close();
#else
        await using Stream stream = datFileInfo.OpenRead();
        foreach (CatalogEntry catalogEntry in catalogFile.CatalogEntries)
        {
            string destFilePath = _fs.Path.Combine(destDirectory, catalogEntry.AssetPath);
            IFileInfo destFile = _fs.FileInfo.FromFileName(destFilePath);

            if (!destFile.Directory.Exists)
            {
                destFile.Directory.Create();
            }

            stream.Seek(catalogEntry.ByteOffset, SeekOrigin.Begin);

            byte[] newFileData = bufferPool.Rent(catalogEntry.AssetSize);
            Memory<byte> memory = newFileData.AsMemory()[..catalogEntry.AssetSize];
            int read = await stream.ReadAsync(memory, ct);

            if (read != catalogEntry.AssetSize)
            {
                _logger?.LogError("Could not read asset data from dat file: {DatFile}", datFilePath);
                throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
            }

            //await using Stream destStream = _fs.File.Open(destFile.FullName, FileMode.Create);
            await using Stream destStream = new MemoryStream();
            await destStream.WriteAsync(memory, ct);
            destStream.Close();
#endif
            bufferPool.Return(newFileData);
        }
    }

    public void ExportAssetsSubStream(CatalogFile catalogFile, string destDirectory)
    {
        if (catalogFile == null)
        {
            _logger?.LogError("Catalog file is null");
            throw new ArgumentNullException(nameof(catalogFile));
        }

        IFileInfo catalogFileInfo = _fs.FileInfo.FromFileName(catalogFile.FilePath);

        if (!catalogFileInfo.Exists)
        {
            _logger?.LogError("Catalog file does not exist: {CatalogFile}", catalogFile.FilePath);
            throw new CatalogFileNotFoundException("Catalog file does not exist", catalogFile.FilePath);
        }

        if (string.IsNullOrEmpty(destDirectory))
        {
            _logger?.LogError("Destination directory is null or empty");
            throw new ArgumentException("Value cannot be null or empty.", nameof(destDirectory));
        }

        string catalogFileName = _fs.Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
        string datFilePath = _fs.Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");

        IFileInfo datFileInfo = _fs.FileInfo.FromFileName(datFilePath);

        if (!datFileInfo.Exists)
        {
            _logger?.LogError("Dat file does not exist: {DatFile}", datFilePath);
            throw new DatFileNotFoundException("Dat file does not exist", datFilePath);
        }

        using Stream stream = datFileInfo.OpenRead();
        foreach (CatalogEntry catalogEntry in catalogFile.CatalogEntries)
        {
            string destFilePath = _fs.Path.Combine(destDirectory, catalogEntry.AssetPath);
            IFileInfo destFile = _fs.FileInfo.FromFileName(destFilePath);

            if (!destFile.Directory.Exists)
            {
                //destFile.Directory.Create();
            }

            using var subStream = new SubStream(stream, catalogEntry.ByteOffset, catalogEntry.AssetSize);
            //using Stream destStream = _fs.File.Open(destFile.FullName, FileMode.Create);
            using Stream destStream = new MemoryStream();
            subStream.CopyTo(destStream);
            destStream.Close();
        }
    }

    public async Task ExportAssetsSubStreamAsync(CatalogFile catalogFile, string destDirectory, CancellationToken ct = default)
    {
        if (catalogFile == null)
        {
            _logger?.LogError("Catalog file is null");
            throw new ArgumentNullException(nameof(catalogFile));
        }

        IFileInfo catalogFileInfo = _fs.FileInfo.FromFileName(catalogFile.FilePath);

        if (!catalogFileInfo.Exists)
        {
            _logger?.LogError("Catalog file does not exist: {CatalogFile}", catalogFile.FilePath);
            throw new CatalogFileNotFoundException("Catalog file does not exist", catalogFile.FilePath);
        }

        if (string.IsNullOrEmpty(destDirectory))
        {
            _logger?.LogError("Destination directory is null or empty");
            throw new ArgumentException("Value cannot be null or empty.", nameof(destDirectory));
        }

        string catalogFileName = _fs.Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
        string datFilePath = _fs.Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");

        IFileInfo datFileInfo = _fs.FileInfo.FromFileName(datFilePath);

        if (!datFileInfo.Exists)
        {
            _logger?.LogError("Dat file does not exist: {DatFile}", datFilePath);
            throw new DatFileNotFoundException("Dat file does not exist", datFilePath);
        }

        await using Stream stream = datFileInfo.OpenRead();
        foreach (CatalogEntry catalogEntry in catalogFile.CatalogEntries)
        {
            string destFilePath = _fs.Path.Combine(destDirectory, catalogEntry.AssetPath);
            IFileInfo destFile = _fs.FileInfo.FromFileName(destFilePath);

            if (!destFile.Directory.Exists)
            {
                //destFile.Directory.Create();
            }

            await using var subStream = new SubStream(stream, catalogEntry.ByteOffset, catalogEntry.AssetSize);
            //using Stream destStream = _fs.File.Open(destFile.FullName, FileMode.Create);
            await using Stream destStream = new MemoryStream();
            await subStream.CopyToAsync(destStream, ct);
            destStream.Close();
        }
    }

    public void ExportAssets(CatalogFile catalogFile, string destDirectory)
    {
        if (catalogFile == null)
        {
            _logger?.LogError("Catalog file is null");
            throw new ArgumentNullException(nameof(catalogFile));
        }

        IFileInfo catalogFileInfo = _fs.FileInfo.FromFileName(catalogFile.FilePath);

        if (!catalogFileInfo.Exists)
        {
            _logger?.LogError("Catalog file does not exist: {CatalogFile}", catalogFile.FilePath);
            throw new CatalogFileNotFoundException("Catalog file does not exist", catalogFile.FilePath);
        }

        if (string.IsNullOrEmpty(destDirectory))
        {
            _logger?.LogError("Destination directory is null or empty");
            throw new ArgumentException("Value cannot be null or empty.", nameof(destDirectory));
        }

        string catalogFileName = _fs.Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
        string datFilePath = _fs.Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");

        IFileInfo datFileInfo = _fs.FileInfo.FromFileName(datFilePath);

        if (!datFileInfo.Exists)
        {
            _logger?.LogError("Dat file does not exist: {DatFile}", datFilePath);
            throw new DatFileNotFoundException("Dat file does not exist", datFilePath);
        }

        ArrayPool<byte> bufferPool = ArrayPool<byte>.Shared;

        using Stream stream = datFileInfo.OpenRead();
        foreach (CatalogEntry catalogEntry in catalogFile.CatalogEntries)
        {
            string destFilePath = _fs.Path.Combine(destDirectory, catalogEntry.AssetPath);
            IFileInfo destFile = _fs.FileInfo.FromFileName(destFilePath);

            if (!destFile.Directory.Exists)
            {
                //destFile.Directory.Create();
            }

            stream.Seek(catalogEntry.ByteOffset, SeekOrigin.Begin);
            byte[] buffer = bufferPool.Rent(catalogEntry.AssetSize);
            Span<byte> newFileData = buffer.AsSpan().Slice(0, catalogEntry.AssetSize);
            int read = stream.Read(newFileData);

            if (read != catalogEntry.AssetSize)
            {
                _logger?.LogError("Could not read asset data from dat file: {DatFile}", datFilePath);
                throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
            }

            //using Stream destStream = _fs.File.Open(destFile.FullName, FileMode.Create);
            using Stream destStream = new MemoryStream();
            destStream.Write(newFileData);
            destStream.Close();
            bufferPool.Return(buffer);
        }
    }
}