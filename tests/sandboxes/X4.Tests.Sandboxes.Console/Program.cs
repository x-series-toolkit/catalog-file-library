using System.IO.Abstractions;
using X4.CatalogFileLib;
using X4.Tests.Sandboxes.Console;

var catalogReader = new CatalogReader(new FileSystem());

var catalogFiles = catalogReader.GetCatalogFiles(@"E:\SteamLibrary\steamapps\common\X4 Foundations");

var catalogEntries = await catalogReader
    .GetFilesInCatalogAsync(@"E:\SteamLibrary\steamapps\common\X4 Foundations\01.cat",
        new ConsoleProgress<ProgressReport>(report =>
        {
            (string assetPath, int current, int total) = report;
            double percent = Math.Round(current / (double)total * 100);
            Console.WriteLine($"{current} - {assetPath} - {percent}");
        }));

Console.ReadLine();