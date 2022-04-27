namespace X4.Tests.Sandboxes.Benchmarks.Benchmarks.AssetExporter;

[MemoryDiagnoser()]
public class Comparison
{
    private const string PathSmall = @"F:\SteamLibrary\steamapps\common\X4 Foundations\extensions\crystal_rarities\subst_01.cat";
    private const string PathBig = @"F:\SteamLibrary\steamapps\common\X4 Foundations\01.cat";

    private static readonly IFileSystem Fs = new FileSystem();
    private static readonly CatalogFileReader CatalogFileReader = new();
    private static readonly CatalogFile CatalogFileSmall = CatalogFileReader.GetCatalogFile(PathSmall);
    private static readonly CatalogFile CatalogFileLarge = CatalogFileReader.GetCatalogFile(PathBig);

    private CatalogFile _catalogFile;
    
    //[Params("Small_600KB", "Large_5GB")]
    //public string FileSize { get; set; }
    
    //[GlobalSetup]
    //public void GlobalSetup()
    //{
    //    _catalogFile = FileSize == "Large_5GB" ? CatalogFileLarge : CatalogFileSmall;
    //}

    public Comparison()
    {
        _catalogFile = CatalogFileSmall;
    }

    [Benchmark]
    public void ExportAssetsUnoptimized_Benchmark()
    {
        ExportAssetsUnoptimized(_catalogFile, @"E:\temp\");
    }

    [Benchmark]
    public async Task ExportAssetsUnoptimizedAsync_Benchmark()
    {
        await ExportAssetsUnoptimizedAsync(_catalogFile, @"E:\temp\");
    }

    [Benchmark]
    public void ExportAssetsWithArrayPool_Benchmark()
    {
        ExportAssetsWithArrayPool(_catalogFile, @"E:\temp\");
    }

    [Benchmark]
    public async Task ExportAssetsWithArrayPoolAsync_Benchmark()
    {
        await ExportAssetsWithArrayPoolAsync(_catalogFile, @"E:\temp\");
    }

    [Benchmark]
    public void ExportAssetsSubStream_Benchmark()
    {
        ExportAssetsSubStream(_catalogFile, @"E:\temp\");
    }

    [Benchmark]
    public async Task ExportAssetsSubStreamAsync_Benchmark()
    {
        await ExportAssetsSubStreamAsync(_catalogFile, @"E:\temp\");
    }
    
    private static void ExportAssetsUnoptimized(CatalogFile catalogFile, string destDirectory)
    {
        IFileInfo catalogFileInfo = Fs.FileInfo.FromFileName(catalogFile.FilePath);
        string catalogFileName = Fs.Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
        string datFilePath = Fs.Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");
        IFileInfo datFileInfo = Fs.FileInfo.FromFileName(datFilePath);

        using Stream stream = datFileInfo.OpenRead();

        foreach (CatalogEntry catalogEntry in catalogFile.CatalogEntries)
        {
            string destFilePath = Fs.Path.Combine(destDirectory, catalogEntry.AssetPath);
            IFileInfo destFile = Fs.FileInfo.FromFileName(destFilePath);

            if (!destFile.Directory.Exists)
            {
                // destFile.Directory.Create();
            }

            stream.Seek(catalogEntry.ByteOffset, SeekOrigin.Begin);
            var newFileData = new byte[catalogEntry.AssetSize];
            int read = stream.Read(newFileData, 0, catalogEntry.AssetSize);

            if (read != catalogEntry.AssetSize)
            {
                throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
            }

            // using Stream destStream = Fs.File.Open(destFile.FullName, FileMode.Create);
            using var destStream = new MemoryStream();
            destStream.Write(newFileData);
            destStream.Close();
        }
    }

    private static void ExportAssetsWithArrayPool(CatalogFile catalogFile, string destDirectory)
    {
        IFileInfo catalogFileInfo = Fs.FileInfo.FromFileName(catalogFile.FilePath);
        string catalogFileName = Fs.Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
        string datFilePath = Fs.Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");
        IFileInfo datFileInfo = Fs.FileInfo.FromFileName(datFilePath);

        ArrayPool<byte> bufferPool = ArrayPool<byte>.Shared;

        using Stream stream = datFileInfo.OpenRead();
        foreach (CatalogEntry catalogEntry in catalogFile.CatalogEntries)
        {
            string destFilePath = Fs.Path.Combine(destDirectory, catalogEntry.AssetPath);
            IFileInfo destFile = Fs.FileInfo.FromFileName(destFilePath);

            if (!destFile.Directory.Exists)
            {
                //destFile.Directory.Create();
            }

            stream.Seek(catalogEntry.ByteOffset, SeekOrigin.Begin);
            byte[] newFileData = bufferPool.Rent(catalogEntry.AssetSize);
            //Span<byte> newFileData = buffer.AsSpan().Slice(0, catalogEntry.AssetSize);
            int read = stream.Read(newFileData, 0, catalogEntry.AssetSize);

            if (read != catalogEntry.AssetSize)
            {
                throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
            }

            // using Stream destStream = Fs.File.Open(destFile.FullName, FileMode.Create);
            using Stream destStream = new MemoryStream();
            destStream.Write(newFileData, 0, catalogEntry.AssetSize);
            destStream.Close();
            bufferPool.Return(newFileData);
        }
    }

    private static void ExportAssetsSubStream(CatalogFile catalogFile, string destDirectory)
    {
        IFileInfo catalogFileInfo = Fs.FileInfo.FromFileName(catalogFile.FilePath);
        string catalogFileName = Fs.Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
        string datFilePath = Fs.Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");
        IFileInfo datFileInfo = Fs.FileInfo.FromFileName(datFilePath);

        using Stream stream = datFileInfo.OpenRead();
        foreach (CatalogEntry catalogEntry in catalogFile.CatalogEntries)
        {
            string destFilePath = Fs.Path.Combine(destDirectory, catalogEntry.AssetPath);
            IFileInfo destFile = Fs.FileInfo.FromFileName(destFilePath);

            if (!destFile.Directory.Exists)
            {
                //destFile.Directory.Create();
            }

            using var subStream = new SubStream(stream, catalogEntry.ByteOffset, catalogEntry.AssetSize);
            //using Stream destStream = Fs.File.Open(destFile.FullName, FileMode.Create);
            using Stream destStream = new MemoryStream();
            subStream.CopyTo(destStream);
            destStream.Close();
        }
    }

    private static async Task ExportAssetsUnoptimizedAsync(CatalogFile catalogFile, string destDirectory, CancellationToken ct = default)
    {
        IFileInfo catalogFileInfo = Fs.FileInfo.FromFileName(catalogFile.FilePath);
        string catalogFileName = Fs.Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
        string datFilePath = Fs.Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");
        IFileInfo datFileInfo = Fs.FileInfo.FromFileName(datFilePath);

        await using Stream stream = datFileInfo.OpenRead();

        foreach (CatalogEntry catalogEntry in catalogFile.CatalogEntries)
        {
            string destFilePath = Fs.Path.Combine(destDirectory, catalogEntry.AssetPath);
            IFileInfo destFile = Fs.FileInfo.FromFileName(destFilePath);

            if (!destFile.Directory.Exists)
            {
                // destFile.Directory.Create();
            }

            stream.Seek(catalogEntry.ByteOffset, SeekOrigin.Begin);
            var newFileData = new byte[catalogEntry.AssetSize];
            int read = await stream.ReadAsync(newFileData, 0, catalogEntry.AssetSize, ct);

            if (read != catalogEntry.AssetSize)
            {
                throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
            }

            // using Stream destStream = Fs.File.Open(destFile.FullName, FileMode.Create);
            await using var destStream = new MemoryStream();
            await destStream.WriteAsync(newFileData, 0, catalogEntry.AssetSize, ct);
            destStream.Close();
        }
    }    
    
    private static async Task ExportAssetsWithArrayPoolAsync(CatalogFile catalogFile, string destDirectory, CancellationToken ct = default)
    {
        IFileInfo catalogFileInfo = Fs.FileInfo.FromFileName(catalogFile.FilePath);
        string catalogFileName = Fs.Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
        string datFilePath = Fs.Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");
        IFileInfo datFileInfo = Fs.FileInfo.FromFileName(datFilePath);

        ArrayPool<byte> bufferPool = ArrayPool<byte>.Shared;

        await using Stream stream = datFileInfo.OpenRead();
        foreach (CatalogEntry catalogEntry in catalogFile.CatalogEntries)
        {
            string destFilePath = Fs.Path.Combine(destDirectory, catalogEntry.AssetPath);
            IFileInfo destFile = Fs.FileInfo.FromFileName(destFilePath);

            if (!destFile.Directory.Exists)
            {
                //destFile.Directory.Create();
            }

            stream.Seek(catalogEntry.ByteOffset, SeekOrigin.Begin);
            byte[] newFileData = bufferPool.Rent(catalogEntry.AssetSize);
            //Memory<byte> newFileData = buffer.AsMemory().Slice(0, catalogEntry.AssetSize);
            int read = await stream.ReadAsync(newFileData, 0, catalogEntry.AssetSize, ct);

            if (read != catalogEntry.AssetSize)
            {
                throw new DatFileReadException("Could not read asset data from dat file", datFilePath);
            }

            // using Stream destStream = Fs.File.Open(destFile.FullName, FileMode.Create);
            await using Stream destStream = new MemoryStream();
            await destStream.WriteAsync(newFileData, 0, catalogEntry.AssetSize, ct);
            destStream.Close();
            bufferPool.Return(newFileData);
        }
    }

    private static async Task ExportAssetsSubStreamAsync(CatalogFile catalogFile, string destDirectory, CancellationToken ct = default)
    {
        IFileInfo catalogFileInfo = Fs.FileInfo.FromFileName(catalogFile.FilePath);
        string catalogFileName = Fs.Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
        string datFilePath = Fs.Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");
        IFileInfo datFileInfo = Fs.FileInfo.FromFileName(datFilePath);

        await using Stream stream = datFileInfo.OpenRead();
        foreach (CatalogEntry catalogEntry in catalogFile.CatalogEntries)
        {
            string destFilePath = Fs.Path.Combine(destDirectory, catalogEntry.AssetPath);
            IFileInfo destFile = Fs.FileInfo.FromFileName(destFilePath);

            if (!destFile.Directory.Exists)
            {
                //destFile.Directory.Create();
            }

            await using var subStream = new SubStream(stream, catalogEntry.ByteOffset, catalogEntry.AssetSize);
            //using Stream destStream = Fs.File.Open(destFile.FullName, FileMode.Create);
            await using Stream destStream = new MemoryStream();
            await subStream.CopyToAsync(destStream, ct);
            destStream.Close();
        }
    }
}