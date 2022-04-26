namespace X4.Tests.Sandboxes.WindowsFormsApp
{
    partial class CatalogBrowser
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.prgCatalogLoad = new System.Windows.Forms.ProgressBar();
            this.btnLoad = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.treeCatalogFiles = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // prgCatalogLoad
            // 
            this.prgCatalogLoad.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.prgCatalogLoad.Location = new System.Drawing.Point(0, 427);
            this.prgCatalogLoad.Name = "prgCatalogLoad";
            this.prgCatalogLoad.Size = new System.Drawing.Size(800, 23);
            this.prgCatalogLoad.TabIndex = 0;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(677, 12);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(111, 23);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(12, 12);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(659, 23);
            this.txtPath.TabIndex = 2;
            // 
            // treeCatalogFiles
            // 
            this.treeCatalogFiles.Location = new System.Drawing.Point(12, 41);
            this.treeCatalogFiles.Name = "treeCatalogFiles";
            this.treeCatalogFiles.Size = new System.Drawing.Size(776, 380);
            this.treeCatalogFiles.TabIndex = 3;
            // 
            // CatalogBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.treeCatalogFiles);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.prgCatalogLoad);
            this.Name = "CatalogBrowser";
            this.Text = "CatalogBrowser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ProgressBar prgCatalogLoad;
        private Button btnLoad;
        private TextBox txtPath;
        private TreeView treeCatalogFiles;
    }
}