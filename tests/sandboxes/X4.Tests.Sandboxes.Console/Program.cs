using X4.CatalogFileLib;
using X4.CatalogFileLib.Services.Components;
using X4.Tests.Sandboxes.Console;

var catalogReader = new CatalogReader();

var catalogEntries = await catalogReader
    .GetCatalogEntriesAsync(@"F:\SteamLibrary\steamapps\common\X4 Foundations\extensions\crystal_rarities\ext_01.cat",
        new ConsoleProgress<ProgressReport>(report =>
        {
            (string assetPath, int current, int total) = report;
            double percent = Math.Round(current / (double)total * 100);
            Console.WriteLine($"{current} - {assetPath} - {percent}");
        }));

var catalogAssetExporter = new CatalogAssetExporter();

catalogAssetExporter.ExportAsset(catalogEntries[0], $"F:\\Temp\\{Path.GetFileName(catalogEntries[0].AssetPath)}");

Console.ReadLine();