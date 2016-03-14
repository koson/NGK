namespace NGK.CorrosionMonitoringSystem.Views
{
    partial class MainWindowView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindowView));
            this._StatusStripMain = new System.Windows.Forms.StatusStrip();
            this._ToolStripButtonTotalDevices = new System.Windows.Forms.ToolStripButton();
            this._ToolStripButtonFaultyDevices = new System.Windows.Forms.ToolStripButton();
            this._ToolStripStatusLabelDateTime = new System.Windows.Forms.ToolStripStatusLabel();
            this._PanelTitleRegion = new System.Windows.Forms.Panel();
            this._LabelTilte = new System.Windows.Forms.Label();
            this._PanelSystemButtonsRegion = new System.Windows.Forms.Panel();
            this._TableLayoutPanelButtonsPanel = new System.Windows.Forms.TableLayoutPanel();
            this._ButtonF6 = new System.Windows.Forms.Button();
            this._ButtonF5 = new System.Windows.Forms.Button();
            this._ButtonF3 = new System.Windows.Forms.Button();
            this._ButtonF2 = new System.Windows.Forms.Button();
            this._ButtonF4 = new System.Windows.Forms.Button();
            this._PanelWorkingRegion = new System.Windows.Forms.Panel();
            this._StatusStripMain.SuspendLayout();
            this._PanelTitleRegion.SuspendLayout();
            this._PanelSystemButtonsRegion.SuspendLayout();
            this._TableLayoutPanelButtonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _StatusStripMain
            // 
            this._StatusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._ToolStripButtonTotalDevices,
            this._ToolStripButtonFaultyDevices,
            this._ToolStripStatusLabelDateTime});
            this._StatusStripMain.Location = new System.Drawing.Point(0, 511);
            this._StatusStripMain.Margin = new System.Windows.Forms.Padding(5);
            this._StatusStripMain.Name = "_StatusStripMain";
            this._StatusStripMain.Size = new System.Drawing.Size(884, 26);
            this._StatusStripMain.TabIndex = 3;
            this._StatusStripMain.Text = "SystemStatusStrip";
            // 
            // _ToolStripButtonTotalDevices
            // 
            this._ToolStripButtonTotalDevices.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._ToolStripButtonTotalDevices.Image = ((System.Drawing.Image)(resources.GetObject("_ToolStripButtonTotalDevices.Image")));
            this._ToolStripButtonTotalDevices.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._ToolStripButtonTotalDevices.Name = "_ToolStripButtonTotalDevices";
            this._ToolStripButtonTotalDevices.Size = new System.Drawing.Size(100, 24);
            this._ToolStripButtonTotalDevices.Text = "Total devices";
            this._ToolStripButtonTotalDevices.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this._ToolStripButtonTotalDevices.ToolTipText = "Всего устройств в системе";
            // 
            // _ToolStripButtonFaultyDevices
            // 
            this._ToolStripButtonFaultyDevices.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._ToolStripButtonFaultyDevices.Image = ((System.Drawing.Image)(resources.GetObject("_ToolStripButtonFaultyDevices.Image")));
            this._ToolStripButtonFaultyDevices.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._ToolStripButtonFaultyDevices.Name = "_ToolStripButtonFaultyDevices";
            this._ToolStripButtonFaultyDevices.Size = new System.Drawing.Size(105, 24);
            this._ToolStripButtonFaultyDevices.Text = "Faulty devices";
            this._ToolStripButtonFaultyDevices.ToolTipText = "Количество нейсправных устройств";
            // 
            // _ToolStripStatusLabelDateTime
            // 
            this._ToolStripStatusLabelDateTime.AutoToolTip = true;
            this._ToolStripStatusLabelDateTime.Name = "_ToolStripStatusLabelDateTime";
            this._ToolStripStatusLabelDateTime.Size = new System.Drawing.Size(74, 21);
            this._ToolStripStatusLabelDateTime.Text = "DateTime";
            this._ToolStripStatusLabelDateTime.ToolTipText = "Системное время";
            // 
            // _PanelTitleRegion
            // 
            this._PanelTitleRegion.Controls.Add(this._LabelTilte);
            this._PanelTitleRegion.Dock = System.Windows.Forms.DockStyle.Top;
            this._PanelTitleRegion.Location = new System.Drawing.Point(0, 0);
            this._PanelTitleRegion.Name = "_PanelTitleRegion";
            this._PanelTitleRegion.Size = new System.Drawing.Size(884, 26);
            this._PanelTitleRegion.TabIndex = 2;
            // 
            // _LabelTilte
            // 
            this._LabelTilte.Dock = System.Windows.Forms.DockStyle.Fill;
            this._LabelTilte.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._LabelTilte.Location = new System.Drawing.Point(0, 0);
            this._LabelTilte.Name = "_LabelTilte";
            this._LabelTilte.Size = new System.Drawing.Size(884, 26);
            this._LabelTilte.TabIndex = 0;
            this._LabelTilte.Text = "TITLE";
            this._LabelTilte.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _PanelSystemButtonsRegion
            // 
            this._PanelSystemButtonsRegion.Controls.Add(this._TableLayoutPanelButtonsPanel);
            this._PanelSystemButtonsRegion.Dock = System.Windows.Forms.DockStyle.Right;
            this._PanelSystemButtonsRegion.Location = new System.Drawing.Point(711, 26);
            this._PanelSystemButtonsRegion.Name = "_PanelSystemButtonsRegion";
            this._PanelSystemButtonsRegion.Size = new System.Drawing.Size(173, 485);
            this._PanelSystemButtonsRegion.TabIndex = 5;
            // 
            // _TableLayoutPanelButtonsPanel
            // 
            this._TableLayoutPanelButtonsPanel.AutoSize = true;
            this._TableLayoutPanelButtonsPanel.ColumnCount = 1;
            this._TableLayoutPanelButtonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._TableLayoutPanelButtonsPanel.Controls.Add(this._ButtonF6, 0, 4);
            this._TableLayoutPanelButtonsPanel.Controls.Add(this._ButtonF4, 0, 2);
            this._TableLayoutPanelButtonsPanel.Controls.Add(this._ButtonF5, 0, 3);
            this._TableLayoutPanelButtonsPanel.Controls.Add(this._ButtonF2, 0, 0);
            this._TableLayoutPanelButtonsPanel.Controls.Add(this._ButtonF3, 0, 1);
            this._TableLayoutPanelButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._TableLayoutPanelButtonsPanel.Location = new System.Drawing.Point(0, 0);
            this._TableLayoutPanelButtonsPanel.Name = "_TableLayoutPanelButtonsPanel";
            this._TableLayoutPanelButtonsPanel.RowCount = 5;
            this._TableLayoutPanelButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this._TableLayoutPanelButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this._TableLayoutPanelButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this._TableLayoutPanelButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this._TableLayoutPanelButtonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this._TableLayoutPanelButtonsPanel.Size = new System.Drawing.Size(173, 485);
            this._TableLayoutPanelButtonsPanel.TabIndex = 0;
            // 
            // _ButtonF6
            // 
            this._ButtonF6.Dock = System.Windows.Forms.DockStyle.Fill;
            this._ButtonF6.Location = new System.Drawing.Point(3, 391);
            this._ButtonF6.Name = "_ButtonF6";
            this._ButtonF6.Size = new System.Drawing.Size(167, 91);
            this._ButtonF6.TabIndex = 14;
            this._ButtonF6.Text = "Скрыть панель";
            this._ButtonF6.UseVisualStyleBackColor = true;
            // 
            // _ButtonF5
            // 
            this._ButtonF5.Dock = System.Windows.Forms.DockStyle.Fill;
            this._ButtonF5.Location = new System.Drawing.Point(3, 294);
            this._ButtonF5.Name = "_ButtonF5";
            this._ButtonF5.Size = new System.Drawing.Size(167, 91);
            this._ButtonF5.TabIndex = 13;
            this._ButtonF5.Text = "F5";
            this._ButtonF5.UseVisualStyleBackColor = true;
            // 
            // _ButtonF3
            // 
            this._ButtonF3.Dock = System.Windows.Forms.DockStyle.Fill;
            this._ButtonF3.Location = new System.Drawing.Point(3, 100);
            this._ButtonF3.Name = "_ButtonF3";
            this._ButtonF3.Size = new System.Drawing.Size(167, 91);
            this._ButtonF3.TabIndex = 11;
            this._ButtonF3.Text = "F3";
            this._ButtonF3.UseVisualStyleBackColor = true;
            // 
            // _ButtonF2
            // 
            this._ButtonF2.Dock = System.Windows.Forms.DockStyle.Fill;
            this._ButtonF2.Location = new System.Drawing.Point(3, 3);
            this._ButtonF2.Name = "_ButtonF2";
            this._ButtonF2.Size = new System.Drawing.Size(167, 91);
            this._ButtonF2.TabIndex = 10;
            this._ButtonF2.Text = "Меню";
            this._ButtonF2.UseVisualStyleBackColor = true;
            // 
            // _ButtonF4
            // 
            this._ButtonF4.Dock = System.Windows.Forms.DockStyle.Fill;
            this._ButtonF4.Location = new System.Drawing.Point(3, 197);
            this._ButtonF4.Name = "_ButtonF4";
            this._ButtonF4.Size = new System.Drawing.Size(167, 91);
            this._ButtonF4.TabIndex = 13;
            this._ButtonF4.Text = "F4";
            this._ButtonF4.UseVisualStyleBackColor = true;
            // 
            // _PanelWorkingRegion
            // 
            this._PanelWorkingRegion.Dock = System.Windows.Forms.DockStyle.Fill;
            this._PanelWorkingRegion.Location = new System.Drawing.Point(0, 26);
            this._PanelWorkingRegion.Name = "_PanelWorkingRegion";
            this._PanelWorkingRegion.Size = new System.Drawing.Size(711, 485);
            this._PanelWorkingRegion.TabIndex = 6;
            // 
            // MainWindowView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 537);
            this.Controls.Add(this._PanelWorkingRegion);
            this.Controls.Add(this._PanelSystemButtonsRegion);
            this.Controls.Add(this._StatusStripMain);
            this.Controls.Add(this._PanelTitleRegion);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindowView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TemplateView";
            this.Load += new System.EventHandler(this.EventHandler_MainWindowView_Load);
            this._StatusStripMain.ResumeLayout(false);
            this._StatusStripMain.PerformLayout();
            this._PanelTitleRegion.ResumeLayout(false);
            this._PanelSystemButtonsRegion.ResumeLayout(false);
            this._PanelSystemButtonsRegion.PerformLayout();
            this._TableLayoutPanelButtonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip _StatusStripMain;
        private System.Windows.Forms.ToolStripButton _ToolStripButtonTotalDevices;
        private System.Windows.Forms.ToolStripButton _ToolStripButtonFaultyDevices;
        private System.Windows.Forms.ToolStripStatusLabel _ToolStripStatusLabelDateTime;
        private System.Windows.Forms.Panel _PanelTitleRegion;
        private System.Windows.Forms.Label _LabelTilte;
        private System.Windows.Forms.Button _ButtonF6;
        private System.Windows.Forms.Button _ButtonF5;
        private System.Windows.Forms.Panel _PanelSystemButtonsRegion;
        private System.Windows.Forms.TableLayoutPanel _TableLayoutPanelButtonsPanel;
        private System.Windows.Forms.Button _ButtonF2;
        private System.Windows.Forms.Button _ButtonF3;
        private System.Windows.Forms.Button _ButtonF4;
        private System.Windows.Forms.Panel _PanelWorkingRegion;
    }
}