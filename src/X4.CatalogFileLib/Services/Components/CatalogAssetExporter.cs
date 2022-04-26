using System.Buffers;

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
        Span<byte> newFileData = stackalloc byte[catalogEntry.AssetSize];
        int read = stream.Read(newFileData);

        if (read != catalogEntry.AssetSize)
        {
            _logger?.LogError("Could not read asset data from dat file: {DatFile}", datFilePath);
            throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
        }

        using Stream destStream = _fs.File.Open(destFile.FullName, FileMode.Create);
        destStream.Write(newFileData);
        destStream.Close();
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

        //void SaveFile(Stream fileStream, CatalogEntry catalogEntry, string datFile, IFileSystemInfo fileInfo)
        //{
        //    fileStream.Seek(catalogEntry.ByteOffset, SeekOrigin.Begin);
        //    Span<byte> newFileData = stackalloc byte[catalogEntry.AssetSize];
        //    int read = fileStream.Read(newFileData);

        //    if (read != catalogEntry.AssetSize)
        //    {
        //        _logger?.LogError("Could not read asset data from dat file: {DatFile}", datFile);
        //        throw new DatFileReadException("Could not read asset data from dat file", datFile);
        //    }

        //    //using Stream destStream = _fs.File.Open(fileInfo.FullName, FileMode.Create);
        //    //destStream.Write(newFileData);
        //    //destStream.Close();
        //}

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
            var newFileData = new byte[catalogEntry.AssetSize];
            int read = await stream.ReadAsync(newFileData, 0, catalogEntry.AssetSize, ct);

            if (read != catalogEntry.AssetSize)
            {
                _logger?.LogError("Could not read asset data from dat file: {DatFile}", datFilePath);
                throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
            }

            //await using Stream destStream = _fs.File.Open(destFile.FullName, FileMode.Create);
            //destStream.Write(newFileData);
            //destStream.Close();

            await using Stream destStream = new MemoryStream();
            await destStream.WriteAsync(newFileData, 0, catalogEntry.AssetSize, ct);
            destStream.Close();
        }
    }

    public async Task ExportAssetsArrayPoolAsync(CatalogFile catalogFile, string destDirectory, CancellationToken ct = default)
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

        var bufferPool = ArrayPool<byte>.Shared;
        
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
            
            var newFileData = bufferPool.Rent(catalogEntry.AssetSize);
            Memory<byte> memory = newFileData.AsMemory().Slice(0, catalogEntry.AssetSize);
            int read = await stream.ReadAsync(memory, ct);

            if (read != catalogEntry.AssetSize)
            {
                _logger?.LogError("Could not read asset data from dat file: {DatFile}", datFilePath);
                throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
            }

            await using Stream destStream = new MemoryStream();
            await destStream.WriteAsync(memory, ct);
            destStream.Close();

            bufferPool.Return(newFileData);
            //await using Stream destStream = _fs.File.Open(destFile.FullName, FileMode.Create);
            //destStream.Write(newFileData);
            //destStream.Close();
        }
    }

    public async Task ExportAssetsPipe(CatalogFile catalogFile, string destDirectory, CancellationToken ct = default)
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
                destFile.Directory.Create();
            }

            stream.Position = catalogEntry.ByteOffset;
            var reader = PipeReader.Create(stream);

            while (true)
            {
                ReadResult readResult = await reader.ReadAsync(ct);

                ReadOnlySequence<byte> buffer = readResult.Buffer;

                if (buffer.Length >= catalogEntry.AssetSize)
                {
                    ReadOnlySequence<byte> entry = buffer.Slice(0, catalogEntry.AssetSize);
                    //await using Stream destStream = new MemoryStream();
                    ////await using Stream destStream = File.Open(destFile.FullName, FileMode.Create);
                    //foreach (ReadOnlyMemory<byte> mem in entry)
                    //{
                    //    await destStream.WriteAsync(mem, ct);
                    //}

                    //destStream.Close();
                    break;
                }
                reader.AdvanceTo(buffer.Start, buffer.End);
            }
        }
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
#else
        await using Stream stream = datFileInfo.OpenRead();
#endif

        stream.Seek(catalogEntry.ByteOffset, SeekOrigin.Begin);
        var newFileData = new byte[catalogEntry.AssetSize];
        int read = await stream.ReadAsync(newFileData, 0, catalogEntry.AssetSize, ct);

        if (read != catalogEntry.AssetSize)
        {
            _logger?.LogError("Could not read asset data from dat file: {DatFile}", datFilePath);
            throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
        }

#if NET461 || NETSTANDARD2_0
        using Stream destStream = _fs.File.Open(destFile.FullName, FileMode.Create);
#else
        await using Stream destStream = _fs.File.Open(destFile.FullName, FileMode.Create);
#endif
        await destStream.WriteAsync(newFileData, 0, catalogEntry.AssetSize, ct);
        destStream.Close();
    }
}