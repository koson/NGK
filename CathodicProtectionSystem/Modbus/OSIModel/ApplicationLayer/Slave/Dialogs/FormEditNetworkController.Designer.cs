namespace Modbus.OSIModel.ApplicationLayer.Slave.Dialogs
{
    partial class FormEditNetworkController
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
            components = new System.ComponentModel.Container();
            _MenuStripMain = new System.Windows.Forms.MenuStrip();
            _StatusStripMain = new System.Windows.Forms.StatusStrip();
            _SplitContainerMain = new System.Windows.Forms.SplitContainer();
            _DataGridViewDevicesList = new System.Windows.Forms.DataGridView();
            _ContextMenuStripDevicesList = new System.Windows.Forms.ContextMenuStrip(components);
            _ToolStripMenuItemAddDevice = new System.Windows.Forms.ToolStripMenuItem();
            _ToolStripMenuItemRemoveDevice = new System.Windows.Forms.ToolStripMenuItem();
            _TabControlDevice = new System.Windows.Forms.TabControl();
            _TabPageHoldingRegisters = new System.Windows.Forms.TabPage();
            _DataGridViewHoldingRegisters = new System.Windows.Forms.DataGridView();
            _ContextMenuStripHoldingRegisters = new System.Windows.Forms.ContextMenuStrip(components);
            _ToolStripMenuItemAddHoldingRegister = new System.Windows.Forms.ToolStripMenuItem();
            _ToolStripMenuItemRemoveHoldingRegister = new System.Windows.Forms.ToolStripMenuItem();
            _TabPageInputRegisters = new System.Windows.Forms.TabPage();
            _DataGridViewInputRegisters = new System.Windows.Forms.DataGridView();
            _ContextMenuStripInputRegisters = new System.Windows.Forms.ContextMenuStrip(components);
            _ToolStripMenuItemAddInputRegister = new System.Windows.Forms.ToolStripMenuItem();
            _ToolStripMenuItemRemoveInputRegister = new System.Windows.Forms.ToolStripMenuItem();
            _TabPageCoils = new System.Windows.Forms.TabPage();
            _DataGridViewCoils = new System.Windows.Forms.DataGridView();
            _ContextMenuStripCoils = new System.Windows.Forms.ContextMenuStrip(components);
            _ToolStripMenuItemAddCoil = new System.Windows.Forms.ToolStripMenuItem();
            _ToolStripMenuItemRemoveCoil = new System.Windows.Forms.ToolStripMenuItem();
            _TabPageDiscretesInputs = new System.Windows.Forms.TabPage();
            _DataGridViewDiscretesInputs = new System.Windows.Forms.DataGridView();
            _ContextMenuStripDiscretesInputs = new System.Windows.Forms.ContextMenuStrip(components);
            _ToolStripMenuItemAddDiscreteInput = new System.Windows.Forms.ToolStripMenuItem();
            _ToolStripMenuItemRemoveDiscreteInput = new System.Windows.Forms.ToolStripMenuItem();
            _TabPageFiles = new System.Windows.Forms.TabPage();
            _SplitContainerFiles = new System.Windows.Forms.SplitContainer();
            _DataGridViewFiles = new System.Windows.Forms.DataGridView();
            _ContextMenuStripFiles = new System.Windows.Forms.ContextMenuStrip(components);
            _ToolStripMenuItemAddFile = new System.Windows.Forms.ToolStripMenuItem();
            _ToolStripMenuItemRemoveFile = new System.Windows.Forms.ToolStripMenuItem();
            _DataGridViewRecords = new System.Windows.Forms.DataGridView();
            _ContextMenuStripRecords = new System.Windows.Forms.ContextMenuStrip(components);
            _ToolStripMenuItemAddRecord = new System.Windows.Forms.ToolStripMenuItem();
            _ToolStripMenuItemRemoveRecord = new System.Windows.Forms.ToolStripMenuItem();
            _SplitContainerMain.Panel1.SuspendLayout();
            _SplitContainerMain.Panel2.SuspendLayout();
            _SplitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_DataGridViewDevicesList)).BeginInit();
            _ContextMenuStripDevicesList.SuspendLayout();
            _TabControlDevice.SuspendLayout();
            _TabPageHoldingRegisters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_DataGridViewHoldingRegisters)).BeginInit();
            _ContextMenuStripHoldingRegisters.SuspendLayout();
            _TabPageInputRegisters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_DataGridViewInputRegisters)).BeginInit();
            _ContextMenuStripInputRegisters.SuspendLayout();
            _TabPageCoils.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_DataGridViewCoils)).BeginInit();
            _ContextMenuStripCoils.SuspendLayout();
            _TabPageDiscretesInputs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_DataGridViewDiscretesInputs)).BeginInit();
            _ContextMenuStripDiscretesInputs.SuspendLayout();
            _TabPageFiles.SuspendLayout();
            _SplitContainerFiles.Panel1.SuspendLayout();
            _SplitContainerFiles.Panel2.SuspendLayout();
            _SplitContainerFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_DataGridViewFiles)).BeginInit();
            _ContextMenuStripFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_DataGridViewRecords)).BeginInit();
            _ContextMenuStripRecords.SuspendLayout();
            SuspendLayout();
            // 
            // _MenuStripMain
            // 
            _MenuStripMain.Location = new System.Drawing.Point(0, 0);
            _MenuStripMain.Name = "_MenuStripMain";
            _MenuStripMain.Size = new System.Drawing.Size(880, 24);
            _MenuStripMain.TabIndex = 0;
            _MenuStripMain.Text = "menuStrip1";
            // 
            // _StatusStripMain
            // 
            _StatusStripMain.Location = new System.Drawing.Point(0, 434);
            _StatusStripMain.Name = "_StatusStripMain";
            _StatusStripMain.Size = new System.Drawing.Size(880, 22);
            _StatusStripMain.TabIndex = 1;
            _StatusStripMain.Text = "statusStrip1";
            // 
            // _SplitContainerMain
            // 
            _SplitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            _SplitContainerMain.Location = new System.Drawing.Point(0, 24);
            _SplitContainerMain.Name = "_SplitContainerMain";
            // 
            // _SplitContainerMain.Panel1
            // 
            _SplitContainerMain.Panel1.Controls.Add(_DataGridViewDevicesList);
            // 
            // _SplitContainerMain.Panel2
            // 
            _SplitContainerMain.Panel2.Controls.Add(_TabControlDevice);
            _SplitContainerMain.Size = new System.Drawing.Size(880, 410);
            _SplitContainerMain.SplitterDistance = 292;
            _SplitContainerMain.TabIndex = 3;
            // 
            // _DataGridViewDevicesList
            // 
            _DataGridViewDevicesList.AllowUserToAddRows = false;
            _DataGridViewDevicesList.AllowUserToDeleteRows = false;
            _DataGridViewDevicesList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _DataGridViewDevicesList.ContextMenuStrip = _ContextMenuStripDevicesList;
            _DataGridViewDevicesList.Dock = System.Windows.Forms.DockStyle.Fill;
            _DataGridViewDevicesList.Location = new System.Drawing.Point(0, 0);
            _DataGridViewDevicesList.Name = "_DataGridViewDevicesList";
            _DataGridViewDevicesList.RowTemplate.Height = 24;
            _DataGridViewDevicesList.Size = new System.Drawing.Size(292, 410);
            _DataGridViewDevicesList.TabIndex = 0;
            // 
            // _ContextMenuStripDevicesList
            // 
            _ContextMenuStripDevicesList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _ToolStripMenuItemAddDevice,
            _ToolStripMenuItemRemoveDevice});
            _ContextMenuStripDevicesList.Name = "_ContextMenuStripTreeViewNetwork";
            _ContextMenuStripDevicesList.Size = new System.Drawing.Size(146, 52);
            // 
            // _ToolStripMenuItemAddDevice
            // 
            _ToolStripMenuItemAddDevice.Name = "_ToolStripMenuItemAddDevice";
            _ToolStripMenuItemAddDevice.Size = new System.Drawing.Size(145, 24);
            _ToolStripMenuItemAddDevice.Text = "Добавить";
            _ToolStripMenuItemAddDevice.Click += new System.EventHandler(EventHandler_ToolStripMenuItemAddDevice_Click);
            // 
            // _ToolStripMenuItemRemoveDevice
            // 
            _ToolStripMenuItemRemoveDevice.Name = "_ToolStripMenuItemRemoveDevice";
            _ToolStripMenuItemRemoveDevice.Size = new System.Drawing.Size(145, 24);
            _ToolStripMenuItemRemoveDevice.Text = "Удалить";
            _ToolStripMenuItemRemoveDevice.Click += new System.EventHandler(EventHandler_ToolStripMenuItemRemoveDevice_Click);
            // 
            // _TabControlDevice
            // 
            _TabControlDevice.Controls.Add(_TabPageHoldingRegisters);
            _TabControlDevice.Controls.Add(_TabPageInputRegisters);
            _TabControlDevice.Controls.Add(_TabPageCoils);
            _TabControlDevice.Controls.Add(_TabPageDiscretesInputs);
            _TabControlDevice.Controls.Add(_TabPageFiles);
            _TabControlDevice.Dock = System.Windows.Forms.DockStyle.Fill;
            _TabControlDevice.Location = new System.Drawing.Point(0, 0);
            _TabControlDevice.Name = "_TabControlDevice";
            _TabControlDevice.SelectedIndex = 0;
            _TabControlDevice.Size = new System.Drawing.Size(584, 410);
            _TabControlDevice.TabIndex = 0;
            // 
            // _TabPageHoldingRegisters
            // 
            _TabPageHoldingRegisters.Controls.Add(_DataGridViewHoldingRegisters);
            _TabPageHoldingRegisters.Location = new System.Drawing.Point(4, 25);
            _TabPageHoldingRegisters.Name = "_TabPageHoldingRegisters";
            _TabPageHoldingRegisters.Padding = new System.Windows.Forms.Padding(3);
            _TabPageHoldingRegisters.Size = new System.Drawing.Size(576, 381);
            _TabPageHoldingRegisters.TabIndex = 0;
            _TabPageHoldingRegisters.Text = "Holding Registers";
            _TabPageHoldingRegisters.UseVisualStyleBackColor = true;
            // 
            // _DataGridViewHoldingRegisters
            // 
            _DataGridViewHoldingRegisters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _DataGridViewHoldingRegisters.ContextMenuStrip = _ContextMenuStripHoldingRegisters;
            _DataGridViewHoldingRegisters.Dock = System.Windows.Forms.DockStyle.Fill;
            _DataGridViewHoldingRegisters.Location = new System.Drawing.Point(3, 3);
            _DataGridViewHoldingRegisters.MultiSelect = false;
            _DataGridViewHoldingRegisters.Name = "_DataGridViewHoldingRegisters";
            _DataGridViewHoldingRegisters.RowTemplate.Height = 24;
            _DataGridViewHoldingRegisters.Size = new System.Drawing.Size(570, 375);
            _DataGridViewHoldingRegisters.TabIndex = 0;
            // 
            // _ContextMenuStripHoldingRegisters
            // 
            _ContextMenuStripHoldingRegisters.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _ToolStripMenuItemAddHoldingRegister,
            _ToolStripMenuItemRemoveHoldingRegister});
            _ContextMenuStripHoldingRegisters.Name = "_ContextMenuStripHoldingRegisters";
            _ContextMenuStripHoldingRegisters.Size = new System.Drawing.Size(204, 52);
            // 
            // _ToolStripMenuItemAddHoldingRegister
            // 
            _ToolStripMenuItemAddHoldingRegister.Name = "_ToolStripMenuItemAddHoldingRegister";
            _ToolStripMenuItemAddHoldingRegister.Size = new System.Drawing.Size(203, 24);
            _ToolStripMenuItemAddHoldingRegister.Text = "Добавить регистр";
            _ToolStripMenuItemAddHoldingRegister.Click += new System.EventHandler(EventHandler_ToolStripMenuItemAddHoldingRegister_Click);
            // 
            // _ToolStripMenuItemRemoveHoldingRegister
            // 
            _ToolStripMenuItemRemoveHoldingRegister.Name = "_ToolStripMenuItemRemoveHoldingRegister";
            _ToolStripMenuItemRemoveHoldingRegister.Size = new System.Drawing.Size(203, 24);
            _ToolStripMenuItemRemoveHoldingRegister.Text = "Удалить регистр";
            _ToolStripMenuItemRemoveHoldingRegister.Click += new System.EventHandler(EventHandler_ToolStripMenuItemRemoveHoldingRegister_Click);
            // 
            // _TabPageInputRegisters
            // 
            _TabPageInputRegisters.Controls.Add(_DataGridViewInputRegisters);
            _TabPageInputRegisters.Location = new System.Drawing.Point(4, 25);
            _TabPageInputRegisters.Name = "_TabPageInputRegisters";
            _TabPageInputRegisters.Padding = new System.Windows.Forms.Padding(3);
            _TabPageInputRegisters.Size = new System.Drawing.Size(576, 381);
            _TabPageInputRegisters.TabIndex = 1;
            _TabPageInputRegisters.Text = "Input Registers";
            _TabPageInputRegisters.UseVisualStyleBackColor = true;
            // 
            // _DataGridViewInputRegisters
            // 
            _DataGridViewInputRegisters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _DataGridViewInputRegisters.ContextMenuStrip = _ContextMenuStripInputRegisters;
            _DataGridViewInputRegisters.Dock = System.Windows.Forms.DockStyle.Fill;
            _DataGridViewInputRegisters.Location = new System.Drawing.Point(3, 3);
            _DataGridViewInputRegisters.Name = "_DataGridViewInputRegisters";
            _DataGridViewInputRegisters.RowTemplate.Height = 24;
            _DataGridViewInputRegisters.Size = new System.Drawing.Size(570, 375);
            _DataGridViewInputRegisters.TabIndex = 0;
            // 
            // _ContextMenuStripInputRegisters
            // 
            _ContextMenuStripInputRegisters.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _ToolStripMenuItemAddInputRegister,
            _ToolStripMenuItemRemoveInputRegister});
            _ContextMenuStripInputRegisters.Name = "_ContextMenuStripInputRegisters";
            _ContextMenuStripInputRegisters.Size = new System.Drawing.Size(204, 52);
            // 
            // _ToolStripMenuItemAddInputRegister
            // 
            _ToolStripMenuItemAddInputRegister.Name = "_ToolStripMenuItemAddInputRegister";
            _ToolStripMenuItemAddInputRegister.Size = new System.Drawing.Size(203, 24);
            _ToolStripMenuItemAddInputRegister.Text = "Добавить регистр";
            _ToolStripMenuItemAddInputRegister.Click += new System.EventHandler(EventHandler_ToolStripMenuItemAddInputRegister_Click);
            // 
            // _ToolStripMenuItemRemoveInputRegister
            // 
            _ToolStripMenuItemRemoveInputRegister.Name = "_ToolStripMenuItemRemoveInputRegister";
            _ToolStripMenuItemRemoveInputRegister.Size = new System.Drawing.Size(203, 24);
            _ToolStripMenuItemRemoveInputRegister.Text = "Удалить регистр";
            _ToolStripMenuItemRemoveInputRegister.Click += new System.EventHandler(EventHandler_ToolStripMenuItemRemoveInputRegister_Click);
            // 
            // _TabPageCoils
            // 
            _TabPageCoils.Controls.Add(_DataGridViewCoils);
            _TabPageCoils.Location = new System.Drawing.Point(4, 25);
            _TabPageCoils.Name = "_TabPageCoils";
            _TabPageCoils.Size = new System.Drawing.Size(576, 381);
            _TabPageCoils.TabIndex = 2;
            _TabPageCoils.Text = "Coils";
            _TabPageCoils.UseVisualStyleBackColor = true;
            // 
            // _DataGridViewCoils
            // 
            _DataGridViewCoils.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _DataGridViewCoils.ContextMenuStrip = _ContextMenuStripCoils;
            _DataGridViewCoils.Dock = System.Windows.Forms.DockStyle.Fill;
            _DataGridViewCoils.Location = new System.Drawing.Point(0, 0);
            _DataGridViewCoils.Name = "_DataGridViewCoils";
            _DataGridViewCoils.RowTemplate.Height = 24;
            _DataGridViewCoils.Size = new System.Drawing.Size(576, 381);
            _DataGridViewCoils.TabIndex = 0;
            // 
            // _ContextMenuStripCoils
            // 
            _ContextMenuStripCoils.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _ToolStripMenuItemAddCoil,
            _ToolStripMenuItemRemoveCoil});
            _ContextMenuStripCoils.Name = "_ContextMenuStripCoils";
            _ContextMenuStripCoils.Size = new System.Drawing.Size(278, 52);
            // 
            // _ToolStripMenuItemAddCoil
            // 
            _ToolStripMenuItemAddCoil.Name = "_ToolStripMenuItemAddCoil";
            _ToolStripMenuItemAddCoil.Size = new System.Drawing.Size(277, 24);
            _ToolStripMenuItemAddCoil.Text = "Добавить дискр. вход/выход";
            _ToolStripMenuItemAddCoil.Click += new System.EventHandler(EventHandler_ToolStripMenuItemAddCoil_Click);
            // 
            // _ToolStripMenuItemRemoveCoil
            // 
            _ToolStripMenuItemRemoveCoil.Name = "_ToolStripMenuItemRemoveCoil";
            _ToolStripMenuItemRemoveCoil.Size = new System.Drawing.Size(277, 24);
            _ToolStripMenuItemRemoveCoil.Text = "Удалить дискр. вход/выход";
            _ToolStripMenuItemRemoveCoil.Click += new System.EventHandler(EventHandler_ToolStripMenuItemRemoveCoil_Click);
            // 
            // _TabPageDiscretesInputs
            // 
            _TabPageDiscretesInputs.Controls.Add(_DataGridViewDiscretesInputs);
            _TabPageDiscretesInputs.Location = new System.Drawing.Point(4, 25);
            _TabPageDiscretesInputs.Name = "_TabPageDiscretesInputs";
            _TabPageDiscretesInputs.Size = new System.Drawing.Size(576, 381);
            _TabPageDiscretesInputs.TabIndex = 3;
            _TabPageDiscretesInputs.Text = "Discretes Inputs";
            _TabPageDiscretesInputs.UseVisualStyleBackColor = true;
            // 
            // _DataGridViewDiscretesInputs
            // 
            _DataGridViewDiscretesInputs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _DataGridViewDiscretesInputs.ContextMenuStrip = _ContextMenuStripDiscretesInputs;
            _DataGridViewDiscretesInputs.Dock = System.Windows.Forms.DockStyle.Fill;
            _DataGridViewDiscretesInputs.Location = new System.Drawing.Point(0, 0);
            _DataGridViewDiscretesInputs.Name = "_DataGridViewDiscretesInputs";
            _DataGridViewDiscretesInputs.RowTemplate.Height = 24;
            _DataGridViewDiscretesInputs.Size = new System.Drawing.Size(576, 381);
            _DataGridViewDiscretesInputs.TabIndex = 0;
            // 
            // _ContextMenuStripDiscretesInputs
            // 
            _ContextMenuStripDiscretesInputs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _ToolStripMenuItemAddDiscreteInput,
            _ToolStripMenuItemRemoveDiscreteInput});
            _ContextMenuStripDiscretesInputs.Name = "_ContextMenuStripDiscretesInputs";
            _ContextMenuStripDiscretesInputs.Size = new System.Drawing.Size(269, 52);
            // 
            // _ToolStripMenuItemAddDiscreteInput
            // 
            _ToolStripMenuItemAddDiscreteInput.Name = "_ToolStripMenuItemAddDiscreteInput";
            _ToolStripMenuItemAddDiscreteInput.Size = new System.Drawing.Size(268, 24);
            _ToolStripMenuItemAddDiscreteInput.Text = "Добавить дискретный вход";
            _ToolStripMenuItemAddDiscreteInput.Click += new System.EventHandler(EventHandler_ToolStripMenuItemAddDiscreteInput_Click);
            // 
            // _ToolStripMenuItemRemoveDiscreteInput
            // 
            _ToolStripMenuItemRemoveDiscreteInput.Name = "_ToolStripMenuItemRemoveDiscreteInput";
            _ToolStripMenuItemRemoveDiscreteInput.Size = new System.Drawing.Size(268, 24);
            _ToolStripMenuItemRemoveDiscreteInput.Text = "Удалить дискретный вход";
            _ToolStripMenuItemRemoveDiscreteInput.Click += new System.EventHandler(EventHandler_ToolStripMenuItemRemoveDiscreteInput_Click);
            // 
            // _TabPageFiles
            // 
            _TabPageFiles.Controls.Add(_SplitContainerFiles);
            _TabPageFiles.Location = new System.Drawing.Point(4, 25);
            _TabPageFiles.Name = "_TabPageFiles";
            _TabPageFiles.Size = new System.Drawing.Size(576, 381);
            _TabPageFiles.TabIndex = 4;
            _TabPageFiles.Text = "Files";
            _TabPageFiles.UseVisualStyleBackColor = true;
            // 
            // _SplitContainerFiles
            // 
            _SplitContainerFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            _SplitContainerFiles.Location = new System.Drawing.Point(0, 0);
            _SplitContainerFiles.Name = "_SplitContainerFiles";
            _SplitContainerFiles.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _SplitContainerFiles.Panel1
            // 
            _SplitContainerFiles.Panel1.Controls.Add(_DataGridViewFiles);
            // 
            // _SplitContainerFiles.Panel2
            // 
            _SplitContainerFiles.Panel2.Controls.Add(_DataGridViewRecords);
            _SplitContainerFiles.Size = new System.Drawing.Size(576, 381);
            _SplitContainerFiles.SplitterDistance = 194;
            _SplitContainerFiles.TabIndex = 0;
            // 
            // _DataGridViewFiles
            // 
            _DataGridViewFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _DataGridViewFiles.ContextMenuStrip = _ContextMenuStripFiles;
            _DataGridViewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            _DataGridViewFiles.Location = new System.Drawing.Point(0, 0);
            _DataGridViewFiles.Name = "_DataGridViewFiles";
            _DataGridViewFiles.RowTemplate.Height = 24;
            _DataGridViewFiles.Size = new System.Drawing.Size(576, 194);
            _DataGridViewFiles.TabIndex = 0;
            // 
            // _ContextMenuStripFiles
            // 
            _ContextMenuStripFiles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _ToolStripMenuItemAddFile,
            _ToolStripMenuItemRemoveFile});
            _ContextMenuStripFiles.Name = "_ContextMenuStripFiles";
            _ContextMenuStripFiles.Size = new System.Drawing.Size(185, 52);
            // 
            // _ToolStripMenuItemAddFile
            // 
            _ToolStripMenuItemAddFile.Name = "_ToolStripMenuItemAddFile";
            _ToolStripMenuItemAddFile.Size = new System.Drawing.Size(184, 24);
            _ToolStripMenuItemAddFile.Text = "Добавить файл";
            _ToolStripMenuItemAddFile.Click += new System.EventHandler(EventHandler_ToolStripMenuItemAddFile_Click);
            // 
            // _ToolStripMenuItemRemoveFile
            // 
            _ToolStripMenuItemRemoveFile.Name = "_ToolStripMenuItemRemoveFile";
            _ToolStripMenuItemRemoveFile.Size = new System.Drawing.Size(184, 24);
            _ToolStripMenuItemRemoveFile.Text = "Удалить файл";
            _ToolStripMenuItemRemoveFile.Click += new System.EventHandler(EventHandler_ToolStripMenuItemRemoveFile_Click);
            // 
            // _DataGridViewRecords
            // 
            _DataGridViewRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _DataGridViewRecords.ContextMenuStrip = _ContextMenuStripRecords;
            _DataGridViewRecords.Dock = System.Windows.Forms.DockStyle.Fill;
            _DataGridViewRecords.Location = new System.Drawing.Point(0, 0);
            _DataGridViewRecords.Name = "_DataGridViewRecords";
            _DataGridViewRecords.RowTemplate.Height = 24;
            _DataGridViewRecords.Size = new System.Drawing.Size(576, 183);
            _DataGridViewRecords.TabIndex = 0;
            // 
            // _ContextMenuStripRecords
            // 
            _ContextMenuStripRecords.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _ToolStripMenuItemAddRecord,
            _ToolStripMenuItemRemoveRecord});
            _ContextMenuStripRecords.Name = "_ContextMenuStripRecords";
            _ContextMenuStripRecords.Size = new System.Drawing.Size(198, 52);
            // 
            // _ToolStripMenuItemAddRecord
            // 
            _ToolStripMenuItemAddRecord.Name = "_ToolStripMenuItemAddRecord";
            _ToolStripMenuItemAddRecord.Size = new System.Drawing.Size(197, 24);
            _ToolStripMenuItemAddRecord.Text = "Добавить запись";
            _ToolStripMenuItemAddRecord.Click += new System.EventHandler(EventHandler_ToolStripMenuItemAddRecord_Click);
            // 
            // _ToolStripMenuItemRemoveRecord
            // 
            _ToolStripMenuItemRemoveRecord.Name = "_ToolStripMenuItemRemoveRecord";
            _ToolStripMenuItemRemoveRecord.Size = new System.Drawing.Size(197, 24);
            _ToolStripMenuItemRemoveRecord.Text = "Удалить запись";
            _ToolStripMenuItemRemoveRecord.Click += new System.EventHandler(EventHandler_ToolStripMenuItemRemoveRecord_Click);
            // 
            // FormEditNetworkController
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new System.Drawing.Size(880, 456);
            Controls.Add(_SplitContainerMain);
            Controls.Add(_StatusStripMain);
            Controls.Add(_MenuStripMain);
            MainMenuStrip = _MenuStripMain;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormEditNetworkController";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Редактор сети Modbus";
            Load += new System.EventHandler(EventHandler_FormEditNetworkController_Load);
            FormClosed += new System.Windows.Forms.FormClosedEventHandler(EventHandler_FormEditNetworkController_FormClosed);
            FormClosing += new System.Windows.Forms.FormClosingEventHandler(EventHandler_FormEditNetworkController_FormClosing);
            _SplitContainerMain.Panel1.ResumeLayout(false);
            _SplitContainerMain.Panel2.ResumeLayout(false);
            _SplitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(_DataGridViewDevicesList)).EndInit();
            _ContextMenuStripDevicesList.ResumeLayout(false);
            _TabControlDevice.ResumeLayout(false);
            _TabPageHoldingRegisters.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(_DataGridViewHoldingRegisters)).EndInit();
            _ContextMenuStripHoldingRegisters.ResumeLayout(false);
            _TabPageInputRegisters.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(_DataGridViewInputRegisters)).EndInit();
            _ContextMenuStripInputRegisters.ResumeLayout(false);
            _TabPageCoils.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(_DataGridViewCoils)).EndInit();
            _ContextMenuStripCoils.ResumeLayout(false);
            _TabPageDiscretesInputs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(_DataGridViewDiscretesInputs)).EndInit();
            _ContextMenuStripDiscretesInputs.ResumeLayout(false);
            _TabPageFiles.ResumeLayout(false);
            _SplitContainerFiles.Panel1.ResumeLayout(false);
            _SplitContainerFiles.Panel2.ResumeLayout(false);
            _SplitContainerFiles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(_DataGridViewFiles)).EndInit();
            _ContextMenuStripFiles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(_DataGridViewRecords)).EndInit();
            _ContextMenuStripRecords.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip _MenuStripMain;
        private System.Windows.Forms.StatusStrip _StatusStripMain;
        private System.Windows.Forms.SplitContainer _SplitContainerMain;
        private System.Windows.Forms.ContextMenuStrip _ContextMenuStripDevicesList;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemAddDevice;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemRemoveDevice;
        private System.Windows.Forms.DataGridView _DataGridViewDevicesList;
        private System.Windows.Forms.TabControl _TabControlDevice;
        private System.Windows.Forms.TabPage _TabPageHoldingRegisters;
        private System.Windows.Forms.DataGridView _DataGridViewHoldingRegisters;
        private System.Windows.Forms.TabPage _TabPageInputRegisters;
        private System.Windows.Forms.TabPage _TabPageCoils;
        private System.Windows.Forms.TabPage _TabPageDiscretesInputs;
        private System.Windows.Forms.TabPage _TabPageFiles;
        private System.Windows.Forms.DataGridView _DataGridViewInputRegisters;
        private System.Windows.Forms.DataGridView _DataGridViewCoils;
        private System.Windows.Forms.DataGridView _DataGridViewDiscretesInputs;
        private System.Windows.Forms.SplitContainer _SplitContainerFiles;
        private System.Windows.Forms.DataGridView _DataGridViewFiles;
        private System.Windows.Forms.DataGridView _DataGridViewRecords;
        private System.Windows.Forms.ContextMenuStrip _ContextMenuStripHoldingRegisters;
        private System.Windows.Forms.ContextMenuStrip _ContextMenuStripInputRegisters;
        private System.Windows.Forms.ContextMenuStrip _ContextMenuStripCoils;
        private System.Windows.Forms.ContextMenuStrip _ContextMenuStripDiscretesInputs;
        private System.Windows.Forms.ContextMenuStrip _ContextMenuStripFiles;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemAddHoldingRegister;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemRemoveHoldingRegister;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemAddInputRegister;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemRemoveInputRegister;
        private System.Windows.Forms.ContextMenuStrip _ContextMenuStripRecords;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemAddDiscreteInput;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemRemoveDiscreteInput;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemAddFile;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemRemoveFile;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemAddRecord;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemRemoveRecord;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemAddCoil;
        private System.Windows.Forms.ToolStripMenuItem _ToolStripMenuItemRemoveCoil;
    }
}