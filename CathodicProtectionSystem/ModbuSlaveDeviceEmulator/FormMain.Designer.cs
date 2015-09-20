namespace ModbuSlaveDevicesNetwork
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this._MenuStripMainWindow = new System.Windows.Forms.MenuStrip();
            this._ToolStripMenuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this._ToolStripMenuItemFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this._ToolStripMenuItemFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._ToolStripMenuItemFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this._ToolStripMenuItemNetwork = new System.Windows.Forms.ToolStripMenuItem();
            this._ToolStripMenuItemNetworkEdit = new System.Windows.Forms.ToolStripMenuItem();
            this._StatusStripMainWindow = new System.Windows.Forms.StatusStrip();
            this._ToolStripMainWindow = new System.Windows.Forms.ToolStrip();
            this._ToolStripButtonStart = new System.Windows.Forms.ToolStripButton();
            this._ToolStripButtonStop = new System.Windows.Forms.ToolStripButton();
            this._SplitContainerMainWindow = new System.Windows.Forms.SplitContainer();
            this._TreeViewNetwork = new System.Windows.Forms.TreeView();
            this._TabControlDeviceProperties = new System.Windows.Forms.TabControl();
            this._TabPageHoldingRegisters = new System.Windows.Forms.TabPage();
            this._DataGridViewHoldingRegisters = new System.Windows.Forms.DataGridView();
            this._TabPageInputRegisters = new System.Windows.Forms.TabPage();
            this._DataGridViewInputRegisters = new System.Windows.Forms.DataGridView();
            this._TabPageCoils = new System.Windows.Forms.TabPage();
            this._DataGridViewCoils = new System.Windows.Forms.DataGridView();
            this._TabPageDiscretesInputs = new System.Windows.Forms.TabPage();
            this._DataGridViewDiscretesInputs = new System.Windows.Forms.DataGridView();
            this._TabPageFiles = new System.Windows.Forms.TabPage();
            this._SplitContainerFiles = new System.Windows.Forms.SplitContainer();
            this._DataGridViewFiles = new System.Windows.Forms.DataGridView();
            this._DataGridViewRecords = new System.Windows.Forms.DataGridView();
            this._MenuStripMainWindow.SuspendLayout();
            this._ToolStripMainWindow.SuspendLayout();
            this._SplitContainerMainWindow.Panel1.SuspendLayout();
            this._SplitContainerMainWindow.Panel2.SuspendLayout();
            this._SplitContainerMainWindow.SuspendLayout();
            this._TabControlDeviceProperties.SuspendLayout();
            this._TabPageHoldingRegisters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewHoldingRegisters)).BeginInit();
            this._TabPageInputRegisters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewInputRegisters)).BeginInit();
            this._TabPageCoils.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewCoils)).BeginInit();
            this._TabPageDiscretesInputs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewDiscretesInputs)).BeginInit();
            this._TabPageFiles.SuspendLayout();
            this._SplitContainerFiles.Panel1.SuspendLayout();
            this._SplitContainerFiles.Panel2.SuspendLayout();
            this._SplitContainerFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewRecords)).BeginInit();
            this.SuspendLayout();
            // 
            // _MenuStripMainWindow
            // 
            this._MenuStripMainWindow.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._ToolStripMenuItemFile,
            this._ToolStripMenuItemNetwork});
            this._MenuStripMainWindow.Location = new System.Drawing.Point(0, 0);
            this._MenuStripMainWindow.Name = "_MenuStripMainWindow";
            this._MenuStripMainWindow.Size = new System.Drawing.Size(904, 28);
            this._MenuStripMainWindow.TabIndex = 0;
            this._MenuStripMainWindow.Text = "menuStrip1";
            // 
            // _ToolStripMenuItemFile
            // 
            this._ToolStripMenuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._ToolStripMenuItemFileSave,
            this._ToolStripMenuItemFileSaveAs,
            this.toolStripSeparator1,
            this._ToolStripMenuItemFileExit});
            this._ToolStripMenuItemFile.Name = "_ToolStripMenuItemFile";
            this._ToolStripMenuItemFile.Size = new System.Drawing.Size(57, 24);
            this._ToolStripMenuItemFile.Text = "Файл";
            // 
            // _ToolStripMenuItemFileSave
            // 
            this._ToolStripMenuItemFileSave.Name = "_ToolStripMenuItemFileSave";
            this._ToolStripMenuItemFileSave.Size = new System.Drawing.Size(178, 24);
            this._ToolStripMenuItemFileSave.Text = "Сохранить";
            this._ToolStripMenuItemFileSave.Click += new System.EventHandler(this.EventHandler_ToolStripMenuItemFileSave_Click);
            // 
            // _ToolStripMenuItemFileSaveAs
            // 
            this._ToolStripMenuItemFileSaveAs.Name = "_ToolStripMenuItemFileSaveAs";
            this._ToolStripMenuItemFileSaveAs.Size = new System.Drawing.Size(178, 24);
            this._ToolStripMenuItemFileSaveAs.Text = "Сохранить как";
            this._ToolStripMenuItemFileSaveAs.Click += new System.EventHandler(this.EventHandler_ToolStripMenuItemFileSaveAs_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(175, 6);
            // 
            // _ToolStripMenuItemFileExit
            // 
            this._ToolStripMenuItemFileExit.Name = "_ToolStripMenuItemFileExit";
            this._ToolStripMenuItemFileExit.Size = new System.Drawing.Size(178, 24);
            this._ToolStripMenuItemFileExit.Text = "Выход";
            this._ToolStripMenuItemFileExit.Click += new System.EventHandler(this.EventHandler_ToolStripMenuItemFileExit_Click);
            // 
            // _ToolStripMenuItemNetwork
            // 
            this._ToolStripMenuItemNetwork.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._ToolStripMenuItemNetworkEdit});
            this._ToolStripMenuItemNetwork.Enabled = false;
            this._ToolStripMenuItemNetwork.Name = "_ToolStripMenuItemNetwork";
            this._ToolStripMenuItemNetwork.Size = new System.Drawing.Size(52, 24);
            this._ToolStripMenuItemNetwork.Text = "Сеть";
            // 
            // _ToolStripMenuItemNetworkEdit
            // 
            this._ToolStripMenuItemNetworkEdit.Name = "_ToolStripMenuItemNetworkEdit";
            this._ToolStripMenuItemNetworkEdit.Size = new System.Drawing.Size(214, 24);
            this._ToolStripMenuItemNetworkEdit.Text = "Конфигурация сети";
            this._ToolStripMenuItemNetworkEdit.Click += new System.EventHandler(this.EventHandler_ToolStripMenuItemNetworkEdit_Click);
            // 
            // _StatusStripMainWindow
            // 
            this._StatusStripMainWindow.Location = new System.Drawing.Point(0, 555);
            this._StatusStripMainWindow.Name = "_StatusStripMainWindow";
            this._StatusStripMainWindow.Size = new System.Drawing.Size(904, 22);
            this._StatusStripMainWindow.TabIndex = 1;
            this._StatusStripMainWindow.Text = "statusStrip1";
            // 
            // _ToolStripMainWindow
            // 
            this._ToolStripMainWindow.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._ToolStripButtonStart,
            this._ToolStripButtonStop});
            this._ToolStripMainWindow.Location = new System.Drawing.Point(0, 28);
            this._ToolStripMainWindow.Name = "_ToolStripMainWindow";
            this._ToolStripMainWindow.Size = new System.Drawing.Size(904, 27);
            this._ToolStripMainWindow.TabIndex = 2;
            this._ToolStripMainWindow.Text = "toolStrip1";
            // 
            // _ToolStripButtonStart
            // 
            this._ToolStripButtonStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._ToolStripButtonStart.Enabled = false;
            this._ToolStripButtonStart.Image = ((System.Drawing.Image)(resources.GetObject("_ToolStripButtonStart.Image")));
            this._ToolStripButtonStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._ToolStripButtonStart.Name = "_ToolStripButtonStart";
            this._ToolStripButtonStart.Size = new System.Drawing.Size(51, 24);
            this._ToolStripButtonStart.Text = "Старт";
            this._ToolStripButtonStart.ToolTipText = "Запускает эмулятор";
            this._ToolStripButtonStart.Click += new System.EventHandler(this.EventHandler_ToolStripButtonStart_Click);
            // 
            // _ToolStripButtonStop
            // 
            this._ToolStripButtonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._ToolStripButtonStop.Enabled = false;
            this._ToolStripButtonStop.Image = ((System.Drawing.Image)(resources.GetObject("_ToolStripButtonStop.Image")));
            this._ToolStripButtonStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._ToolStripButtonStop.Name = "_ToolStripButtonStop";
            this._ToolStripButtonStop.Size = new System.Drawing.Size(46, 24);
            this._ToolStripButtonStop.Text = "Стоп";
            this._ToolStripButtonStop.Click += new System.EventHandler(this.EventHandler_ToolStripButtonStop_Click);
            // 
            // _SplitContainerMainWindow
            // 
            this._SplitContainerMainWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this._SplitContainerMainWindow.Location = new System.Drawing.Point(0, 55);
            this._SplitContainerMainWindow.Name = "_SplitContainerMainWindow";
            // 
            // _SplitContainerMainWindow.Panel1
            // 
            this._SplitContainerMainWindow.Panel1.Controls.Add(this._TreeViewNetwork);
            // 
            // _SplitContainerMainWindow.Panel2
            // 
            this._SplitContainerMainWindow.Panel2.Controls.Add(this._TabControlDeviceProperties);
            this._SplitContainerMainWindow.Size = new System.Drawing.Size(904, 500);
            this._SplitContainerMainWindow.SplitterDistance = 300;
            this._SplitContainerMainWindow.TabIndex = 3;
            // 
            // _TreeViewNetwork
            // 
            this._TreeViewNetwork.CheckBoxes = true;
            this._TreeViewNetwork.Dock = System.Windows.Forms.DockStyle.Fill;
            this._TreeViewNetwork.Enabled = false;
            this._TreeViewNetwork.FullRowSelect = true;
            this._TreeViewNetwork.Location = new System.Drawing.Point(0, 0);
            this._TreeViewNetwork.Name = "_TreeViewNetwork";
            this._TreeViewNetwork.ShowRootLines = false;
            this._TreeViewNetwork.Size = new System.Drawing.Size(300, 500);
            this._TreeViewNetwork.TabIndex = 0;
            this._TreeViewNetwork.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.EventHandler_TreeViewNetwork_AfterSelect);
            // 
            // _TabControlDeviceProperties
            // 
            this._TabControlDeviceProperties.Controls.Add(this._TabPageHoldingRegisters);
            this._TabControlDeviceProperties.Controls.Add(this._TabPageInputRegisters);
            this._TabControlDeviceProperties.Controls.Add(this._TabPageCoils);
            this._TabControlDeviceProperties.Controls.Add(this._TabPageDiscretesInputs);
            this._TabControlDeviceProperties.Controls.Add(this._TabPageFiles);
            this._TabControlDeviceProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this._TabControlDeviceProperties.Enabled = false;
            this._TabControlDeviceProperties.Location = new System.Drawing.Point(0, 0);
            this._TabControlDeviceProperties.Name = "_TabControlDeviceProperties";
            this._TabControlDeviceProperties.SelectedIndex = 0;
            this._TabControlDeviceProperties.Size = new System.Drawing.Size(600, 500);
            this._TabControlDeviceProperties.TabIndex = 0;
            // 
            // _TabPageHoldingRegisters
            // 
            this._TabPageHoldingRegisters.Controls.Add(this._DataGridViewHoldingRegisters);
            this._TabPageHoldingRegisters.Location = new System.Drawing.Point(4, 25);
            this._TabPageHoldingRegisters.Name = "_TabPageHoldingRegisters";
            this._TabPageHoldingRegisters.Padding = new System.Windows.Forms.Padding(3);
            this._TabPageHoldingRegisters.Size = new System.Drawing.Size(592, 471);
            this._TabPageHoldingRegisters.TabIndex = 0;
            this._TabPageHoldingRegisters.Text = "Holding Registers";
            this._TabPageHoldingRegisters.UseVisualStyleBackColor = true;
            // 
            // _DataGridViewHoldingRegisters
            // 
            this._DataGridViewHoldingRegisters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._DataGridViewHoldingRegisters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._DataGridViewHoldingRegisters.Dock = System.Windows.Forms.DockStyle.Fill;
            this._DataGridViewHoldingRegisters.Location = new System.Drawing.Point(3, 3);
            this._DataGridViewHoldingRegisters.MultiSelect = false;
            this._DataGridViewHoldingRegisters.Name = "_DataGridViewHoldingRegisters";
            this._DataGridViewHoldingRegisters.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this._DataGridViewHoldingRegisters.RowTemplate.Height = 24;
            this._DataGridViewHoldingRegisters.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._DataGridViewHoldingRegisters.Size = new System.Drawing.Size(586, 465);
            this._DataGridViewHoldingRegisters.TabIndex = 0;
            // 
            // _TabPageInputRegisters
            // 
            this._TabPageInputRegisters.Controls.Add(this._DataGridViewInputRegisters);
            this._TabPageInputRegisters.Location = new System.Drawing.Point(4, 25);
            this._TabPageInputRegisters.Name = "_TabPageInputRegisters";
            this._TabPageInputRegisters.Padding = new System.Windows.Forms.Padding(3);
            this._TabPageInputRegisters.Size = new System.Drawing.Size(592, 471);
            this._TabPageInputRegisters.TabIndex = 1;
            this._TabPageInputRegisters.Text = "Input Registers";
            this._TabPageInputRegisters.UseVisualStyleBackColor = true;
            // 
            // _DataGridViewInputRegisters
            // 
            this._DataGridViewInputRegisters.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._DataGridViewInputRegisters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._DataGridViewInputRegisters.Dock = System.Windows.Forms.DockStyle.Fill;
            this._DataGridViewInputRegisters.Location = new System.Drawing.Point(3, 3);
            this._DataGridViewInputRegisters.MultiSelect = false;
            this._DataGridViewInputRegisters.Name = "_DataGridViewInputRegisters";
            this._DataGridViewInputRegisters.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this._DataGridViewInputRegisters.RowTemplate.Height = 24;
            this._DataGridViewInputRegisters.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._DataGridViewInputRegisters.Size = new System.Drawing.Size(586, 465);
            this._DataGridViewInputRegisters.TabIndex = 0;
            // 
            // _TabPageCoils
            // 
            this._TabPageCoils.Controls.Add(this._DataGridViewCoils);
            this._TabPageCoils.Location = new System.Drawing.Point(4, 25);
            this._TabPageCoils.Name = "_TabPageCoils";
            this._TabPageCoils.Size = new System.Drawing.Size(592, 471);
            this._TabPageCoils.TabIndex = 2;
            this._TabPageCoils.Text = "Coils";
            this._TabPageCoils.UseVisualStyleBackColor = true;
            // 
            // _DataGridViewCoils
            // 
            this._DataGridViewCoils.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._DataGridViewCoils.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._DataGridViewCoils.Dock = System.Windows.Forms.DockStyle.Fill;
            this._DataGridViewCoils.Location = new System.Drawing.Point(0, 0);
            this._DataGridViewCoils.MultiSelect = false;
            this._DataGridViewCoils.Name = "_DataGridViewCoils";
            this._DataGridViewCoils.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this._DataGridViewCoils.RowTemplate.Height = 24;
            this._DataGridViewCoils.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._DataGridViewCoils.Size = new System.Drawing.Size(592, 471);
            this._DataGridViewCoils.TabIndex = 0;
            // 
            // _TabPageDiscretesInputs
            // 
            this._TabPageDiscretesInputs.Controls.Add(this._DataGridViewDiscretesInputs);
            this._TabPageDiscretesInputs.Location = new System.Drawing.Point(4, 25);
            this._TabPageDiscretesInputs.Name = "_TabPageDiscretesInputs";
            this._TabPageDiscretesInputs.Size = new System.Drawing.Size(592, 471);
            this._TabPageDiscretesInputs.TabIndex = 3;
            this._TabPageDiscretesInputs.Text = "Discretes Inputs";
            this._TabPageDiscretesInputs.UseVisualStyleBackColor = true;
            // 
            // _DataGridViewDiscretesInputs
            // 
            this._DataGridViewDiscretesInputs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._DataGridViewDiscretesInputs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._DataGridViewDiscretesInputs.Dock = System.Windows.Forms.DockStyle.Fill;
            this._DataGridViewDiscretesInputs.Location = new System.Drawing.Point(0, 0);
            this._DataGridViewDiscretesInputs.MultiSelect = false;
            this._DataGridViewDiscretesInputs.Name = "_DataGridViewDiscretesInputs";
            this._DataGridViewDiscretesInputs.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this._DataGridViewDiscretesInputs.RowTemplate.Height = 24;
            this._DataGridViewDiscretesInputs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._DataGridViewDiscretesInputs.Size = new System.Drawing.Size(592, 471);
            this._DataGridViewDiscretesInputs.TabIndex = 0;
            // 
            // _TabPageFiles
            // 
            this._TabPageFiles.Controls.Add(this._SplitContainerFiles);
            this._TabPageFiles.Location = new System.Drawing.Point(4, 25);
            this._TabPageFiles.Name = "_TabPageFiles";
            this._TabPageFiles.Size = new System.Drawing.Size(592, 471);
            this._TabPageFiles.TabIndex = 4;
            this._TabPageFiles.Text = "Files";
            this._TabPageFiles.UseVisualStyleBackColor = true;
            // 
            // _SplitContainerFiles
            // 
            this._SplitContainerFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this._SplitContainerFiles.Location = new System.Drawing.Point(0, 0);
            this._SplitContainerFiles.Name = "_SplitContainerFiles";
            this._SplitContainerFiles.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _SplitContainerFiles.Panel1
            // 
            this._SplitContainerFiles.Panel1.Controls.Add(this._DataGridViewFiles);
            // 
            // _SplitContainerFiles.Panel2
            // 
            this._SplitContainerFiles.Panel2.Controls.Add(this._DataGridViewRecords);
            this._SplitContainerFiles.Size = new System.Drawing.Size(592, 471);
            this._SplitContainerFiles.SplitterDistance = 238;
            this._SplitContainerFiles.TabIndex = 0;
            // 
            // _DataGridViewFiles
            // 
            this._DataGridViewFiles.AllowUserToAddRows = false;
            this._DataGridViewFiles.AllowUserToDeleteRows = false;
            this._DataGridViewFiles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._DataGridViewFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._DataGridViewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this._DataGridViewFiles.Location = new System.Drawing.Point(0, 0);
            this._DataGridViewFiles.MultiSelect = false;
            this._DataGridViewFiles.Name = "_DataGridViewFiles";
            this._DataGridViewFiles.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this._DataGridViewFiles.RowTemplate.Height = 24;
            this._DataGridViewFiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._DataGridViewFiles.Size = new System.Drawing.Size(592, 238);
            this._DataGridViewFiles.TabIndex = 4;
            // 
            // _DataGridViewRecords
            // 
            this._DataGridViewRecords.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._DataGridViewRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._DataGridViewRecords.Dock = System.Windows.Forms.DockStyle.Fill;
            this._DataGridViewRecords.Location = new System.Drawing.Point(0, 0);
            this._DataGridViewRecords.MultiSelect = false;
            this._DataGridViewRecords.Name = "_DataGridViewRecords";
            this._DataGridViewRecords.RowTemplate.Height = 24;
            this._DataGridViewRecords.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._DataGridViewRecords.Size = new System.Drawing.Size(592, 229);
            this._DataGridViewRecords.TabIndex = 0;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 577);
            this.Controls.Add(this._SplitContainerMainWindow);
            this.Controls.Add(this._ToolStripMainWindow);
            this.Controls.Add(this._StatusStripMainWindow);
            this.Controls.Add(this._MenuStripMainWindow);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this._MenuStripMainWindow;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Эмулятор сети Modbus";
            this._MenuStripMainWindow.ResumeLayout(false);
            this._MenuStripMainWindow.PerformLayout();
            this._ToolStripMainWindow.ResumeLayout(false);
            this._ToolStripMainWindow.PerformLayout();
            this._SplitContainerMainWindow.Panel1.ResumeLayout(false);
            this._SplitContainerMainWindow.Panel2.ResumeLayout(false);
            this._SplitContainerMainWindow.ResumeLayout(false);
            this._TabControlDeviceProperties.ResumeLayout(false);
            this._TabPageHoldingRegisters.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewHoldingRegisters)).EndInit();
            this._TabPageInputRegisters.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewInputRegisters)).EndInit();
            this._TabPageCoils.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewCoils)).EndInit();
            this._TabPageDiscretesInputs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewDiscretesInputs)).EndInit();
            this._TabPageFiles.ResumeLayout(false);
            this._SplitContainerFiles.Panel1.ResumeLayout(false);
            this._SplitContainerFiles.Panel2.ResumeLayout(false);
            this._SplitContainerFiles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewRecords)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip _MenuStripMainWindow;
        private System.Windows.Forms.StatusStrip _StatusStripMainWindow;
        private System.Windows.Forms.ToolStrip _ToolStripMainWindow;
        private System.Windows.Forms.ToolStripButton _ToolStripButtonStart;
        private System.Windows.Forms.ToolStripButton _ToolStripButtonStop;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemFile;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemFileSaveAs;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemFileSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemFileExit;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemNetwork;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemNetworkEdit;
        private System.Windows.Forms.SplitContainer _SplitContainerMainWindow;
        private System.Windows.Forms.TreeView _TreeViewNetwork;
        private System.Windows.Forms.TabControl _TabControlDeviceProperties;
        private System.Windows.Forms.TabPage _TabPageHoldingRegisters;
        private System.Windows.Forms.TabPage _TabPageInputRegisters;
        private System.Windows.Forms.TabPage _TabPageCoils;
        private System.Windows.Forms.TabPage _TabPageDiscretesInputs;
        private System.Windows.Forms.TabPage _TabPageFiles;
        private System.Windows.Forms.DataGridView _DataGridViewDiscretesInputs;
        private System.Windows.Forms.DataGridView _DataGridViewHoldingRegisters;
        private System.Windows.Forms.DataGridView _DataGridViewInputRegisters;
        private System.Windows.Forms.DataGridView _DataGridViewCoils;
        private System.Windows.Forms.DataGridView _DataGridViewFiles;
        private System.Windows.Forms.SplitContainer _SplitContainerFiles;
        private System.Windows.Forms.DataGridView _DataGridViewRecords;
    }
}

