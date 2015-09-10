namespace NGK.CorrosionMonitoringSystem.Forms
{
    partial class FormNetworkControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this._TreeViewSystem = new System.Windows.Forms.TreeView();
            this._PropertyGridViewer = new System.Windows.Forms.PropertyGrid();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this._TreeViewSystem);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this._PropertyGridViewer);
            this.splitContainerMain.Size = new System.Drawing.Size(655, 472);
            this.splitContainerMain.SplitterDistance = 218;
            this.splitContainerMain.TabIndex = 0;
            // 
            // _TreeViewSystem
            // 
            this._TreeViewSystem.Dock = System.Windows.Forms.DockStyle.Fill;
            this._TreeViewSystem.Location = new System.Drawing.Point(0, 0);
            this._TreeViewSystem.Name = "_TreeViewSystem";
            this._TreeViewSystem.Size = new System.Drawing.Size(218, 472);
            this._TreeViewSystem.TabIndex = 0;
            // 
            // _PropertyGridViewer
            // 
            this._PropertyGridViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._PropertyGridViewer.Location = new System.Drawing.Point(0, 0);
            this._PropertyGridViewer.Name = "_PropertyGridViewer";
            this._PropertyGridViewer.Size = new System.Drawing.Size(433, 472);
            this._PropertyGridViewer.TabIndex = 0;
            // 
            // FormNetworkControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 472);
            this.Controls.Add(this.splitContainerMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "FormNetworkControl";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormNetworkControl";
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.TreeView _TreeViewSystem;
        private System.Windows.Forms.PropertyGrid _PropertyGridViewer;
    }
}