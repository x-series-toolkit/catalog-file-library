//const string Path = @"E:\SteamLibrary\steamapps\common\X4 Foundations\extensions\crystal_rarities\subst_01.cat";

//using System.Buffers;
//using System.IO.Pipelines;
//using System.Text;

//const string catPath = @"E:\SteamLibrary\steamapps\common\X4 Foundations\extensions\crystal_rarities\ext_01.cat";

//var catalogFileReader = new CatalogFileReader();
//CatalogFile catalogFile = catalogFileReader.GetCatalogFile(catPath);

//FileInfo catalogFileInfo = new FileInfo(catalogFile.FilePath);

//string catalogFileName = Path.GetFileNameWithoutExtension(catalogFileInfo.Name);
//string datFilePath = Path.Combine(catalogFileInfo.DirectoryName, $"{catalogFileName}.dat");

//FileInfo datFileInfo = new FileInfo(datFilePath);

//using Stream stream = datFileInfo.OpenRead();

//foreach (CatalogEntry catalogEntry in catalogFile.CatalogEntries)
//{
//    string destFilePath = Path.Combine(@"E:\temp\tempx4", catalogEntry.AssetPath);
//    FileInfo destFile = new FileInfo(destFilePath);

//    if (!destFile.Directory.Exists)
//    {
//        destFile.Directory.Create();
//    }

//    stream.Position = catalogEntry.ByteOffset;

//    var reader = PipeReader.Create(stream);

//    while (true)
//    {
//        ReadResult readResult = await reader.ReadAsync();

//        //while (reader.TryRead(out var result))
//        //{
//        ReadOnlySequence<byte> buffer = readResult.Buffer;

//        //while (buffer.Length > 0)
//        //{

//        if (buffer.Length >= catalogEntry.AssetSize)
//        {
//            ReadOnlySequence<byte> entry = buffer.Slice(0, catalogEntry.AssetSize);
//            await using Stream destStream = File.Open(destFile.FullName, FileMode.Create);
//            foreach (ReadOnlyMemory<byte> mem in entry)
//            {
//                await destStream.WriteAsync(mem);
//            }

//            destStream.Close();

//            //string entryString = Encoding.UTF8.GetString(entry.ToArray());

//            //Console.WriteLine(entryString);

//            //buffer = buffer.Slice(catalogEntry.AssetSize);
//            //reader.AdvanceTo(buffer.Start, buffer.End);
//            break;
//        }

//        reader.AdvanceTo(buffer.Start, buffer.End);
//        //}
//    }


//    //}
//}




//CatalogAssetExporter catalogAssetExporter = new();

//var catalogFileReader = new CatalogFileReader();
//CatalogFile catalogFile = catalogFileReader.GetCatalogFile(Path);

//catalogAssetExporter.ExportAssets(catalogFile, @"E:\temp\tempx4");

// BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

using X4.Tests.Sandboxes.Benchmarks.Benchmarks.Export;

BenchmarkRunner.Run<ExportSpan>();