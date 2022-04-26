using X4.CatalogFileLib;
using X4.CatalogFileLib.Services;
using X4.CatalogFileLib.Services.Components;

namespace X4.Tests.Sandboxes.WindowsFormsApp;

public partial class CatalogBrowser : Form
{
    private readonly CatalogServiceWithProgress _catalogService;

    public CatalogBrowser()
    {
        InitializeComponent();

        _catalogService = new CatalogServiceWithProgress(new CatalogService(new CatalogFileReader(), new CatalogAssetExporter()));
        txtPath.Text = @"E:\SteamLibrary\steamapps\common\X4 Foundations";
    }
        
    private async void btnLoad_Click(object sender, EventArgs e)
    {
        treeCatalogFiles.Nodes.Clear();
        prgCatalogLoad.Value = 0;

        int currentThreadManagedThreadId = Thread.CurrentThread.ManagedThreadId;
        SynchronizationContext synchronizationContext = SynchronizationContext.Current;

        var treeNodes = await GetNodes();

        currentThreadManagedThreadId = Thread.CurrentThread.ManagedThreadId;
        synchronizationContext = SynchronizationContext.Current;

        treeCatalogFiles.Nodes.AddRange(treeNodes);

        GC.Collect();
    }

    private async Task<TreeNode[]> GetNodes()
    {
        var catalogFiles = await _catalogService
            .GetCatalogFilesInDirectoryAsync(txtPath.Text,
                new Progress<ProgressReport>(
                    report =>
                    {
                        (int completed, int total) = report;
                        double percent = Math.Round(completed / (double)total * 100);
                        prgCatalogLoad.Value = (int)percent;
                    }));

        var treeNodes = catalogFiles.Select(file => new TreeNode(Path.GetFileName(file.FilePath),
            file.CatalogEntries.Select(entry => new TreeNode(entry.AssetPath)).ToArray())).ToArray();

        return treeNodes;
    }
}