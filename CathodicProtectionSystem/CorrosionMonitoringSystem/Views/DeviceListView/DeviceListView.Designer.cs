namespace NGK.CorrosionMonitoringSystem.View
{
    partial class DeviceListView
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
            this._DataGridViewDevices = new System.Windows.Forms.DataGridView();
            this._SplitContainerMain.Panel1.SuspendLayout();
            this._SplitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewDevices)).BeginInit();
            this.SuspendLayout();
            // 
            // _SplitContainerMain
            // 
            // 
            // _SplitContainerMain.Panel1
            // 
            this._SplitContainerMain.Panel1.Controls.Add(this._DataGridViewDevices);
            // 
            // _DataGridViewDevices
            // 
            this._DataGridViewDevices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._DataGridViewDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this._DataGridViewDevices.Location = new System.Drawing.Point(5, 5);
            this._DataGridViewDevices.Name = "_DataGridViewDevices";
            this._DataGridViewDevices.RowTemplate.Height = 24;
            this._DataGridViewDevices.Size = new System.Drawing.Size(394, 398);
            this._DataGridViewDevices.TabIndex = 1;
            // 
            // DeviceListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ButtonF3IsAccessible = true;
            this.ButtonF4IsAccessible = true;
            this.ButtonF5IsAccessible = true;
            this.ClientSize = new System.Drawing.Size(543, 408);
            this.Name = "DeviceListView";
            this.Text = "Список устройств";
            this.Load += new System.EventHandler(this.EventHandler_DeviceListView_Load);
            this._SplitContainerMain.Panel1.ResumeLayout(false);
            this._SplitContainerMain.Panel1.PerformLayout();
            this._SplitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewDevices)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _DataGridViewDevices;
    }
}