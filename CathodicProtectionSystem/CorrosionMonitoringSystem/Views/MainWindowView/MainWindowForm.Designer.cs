namespace NGK.CorrosionMonitoringSystem.Views
{
    partial class MainWindowForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindowForm));
            this._StatusStripMain = new System.Windows.Forms.StatusStrip();
            this._ToolStripStatusLabelDateTime = new System.Windows.Forms.ToolStripStatusLabel();
            this._PanelTitleRegion = new System.Windows.Forms.Panel();
            this._LabelTilte = new System.Windows.Forms.Label();
            this._PanelFunctionalButtonsPanel = new System.Windows.Forms.Panel();
            this._PanelWorkingRegion = new System.Windows.Forms.Panel();
            this._StatusStripMain.SuspendLayout();
            this._PanelTitleRegion.SuspendLayout();
            this.SuspendLayout();
            // 
            // _StatusStripMain
            // 
            this._StatusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._ToolStripStatusLabelDateTime});
            this._StatusStripMain.Location = new System.Drawing.Point(0, 414);
            this._StatusStripMain.Margin = new System.Windows.Forms.Padding(4);
            this._StatusStripMain.Name = "_StatusStripMain";
            this._StatusStripMain.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this._StatusStripMain.Size = new System.Drawing.Size(663, 22);
            this._StatusStripMain.TabIndex = 3;
            this._StatusStripMain.Text = "SystemStatusStrip";
            // 
            // _ToolStripStatusLabelDateTime
            // 
            this._ToolStripStatusLabelDateTime.AutoToolTip = true;
            this._ToolStripStatusLabelDateTime.Name = "_ToolStripStatusLabelDateTime";
            this._ToolStripStatusLabelDateTime.Size = new System.Drawing.Size(58, 17);
            this._ToolStripStatusLabelDateTime.Text = "DateTime";
            this._ToolStripStatusLabelDateTime.ToolTipText = "Системное время";
            // 
            // _PanelTitleRegion
            // 
            this._PanelTitleRegion.Controls.Add(this._LabelTilte);
            this._PanelTitleRegion.Dock = System.Windows.Forms.DockStyle.Top;
            this._PanelTitleRegion.Location = new System.Drawing.Point(0, 0);
            this._PanelTitleRegion.Margin = new System.Windows.Forms.Padding(2);
            this._PanelTitleRegion.Name = "_PanelTitleRegion";
            this._PanelTitleRegion.Size = new System.Drawing.Size(663, 21);
            this._PanelTitleRegion.TabIndex = 2;
            // 
            // _LabelTilte
            // 
            this._LabelTilte.Dock = System.Windows.Forms.DockStyle.Fill;
            this._LabelTilte.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._LabelTilte.Location = new System.Drawing.Point(0, 0);
            this._LabelTilte.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._LabelTilte.Name = "_LabelTilte";
            this._LabelTilte.Size = new System.Drawing.Size(663, 21);
            this._LabelTilte.TabIndex = 0;
            this._LabelTilte.Text = "TITLE";
            this._LabelTilte.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _PanelFunctionalButtonsPanel
            // 
            this._PanelFunctionalButtonsPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this._PanelFunctionalButtonsPanel.Location = new System.Drawing.Point(533, 21);
            this._PanelFunctionalButtonsPanel.Margin = new System.Windows.Forms.Padding(2);
            this._PanelFunctionalButtonsPanel.Name = "_PanelFunctionalButtonsPanel";
            this._PanelFunctionalButtonsPanel.Size = new System.Drawing.Size(130, 393);
            this._PanelFunctionalButtonsPanel.TabIndex = 5;
            this._PanelFunctionalButtonsPanel.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.EventHandler_PanelFunctionalButtonsPanel_ControlAdded);
            this._PanelFunctionalButtonsPanel.Resize += new System.EventHandler(this.EventHandler_PanelFunctionalButtonsPanel_Resize);
            // 
            // _PanelWorkingRegion
            // 
            this._PanelWorkingRegion.Dock = System.Windows.Forms.DockStyle.Fill;
            this._PanelWorkingRegion.Location = new System.Drawing.Point(0, 21);
            this._PanelWorkingRegion.Margin = new System.Windows.Forms.Padding(2);
            this._PanelWorkingRegion.Name = "_PanelWorkingRegion";
            this._PanelWorkingRegion.Size = new System.Drawing.Size(533, 393);
            this._PanelWorkingRegion.TabIndex = 6;
            // 
            // MainWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 436);
            this.Controls.Add(this._PanelWorkingRegion);
            this.Controls.Add(this._PanelFunctionalButtonsPanel);
            this.Controls.Add(this._StatusStripMain);
            this.Controls.Add(this._PanelTitleRegion);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainWindowForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TemplateView";
            this.Load += new System.EventHandler(this.EventHandler_MainWindowView_Load);
            this._StatusStripMain.ResumeLayout(false);
            this._StatusStripMain.PerformLayout();
            this._PanelTitleRegion.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripStatusLabel _ToolStripStatusLabelDateTime;
        private System.Windows.Forms.Panel _PanelTitleRegion;
        private System.Windows.Forms.Label _LabelTilte;
        private System.Windows.Forms.Panel _PanelWorkingRegion;
        public System.Windows.Forms.StatusStrip _StatusStripMain;
        private System.Windows.Forms.Panel _PanelFunctionalButtonsPanel;
    }
}