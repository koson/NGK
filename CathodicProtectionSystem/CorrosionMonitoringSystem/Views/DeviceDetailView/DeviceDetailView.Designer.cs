namespace NGK.CorrosionMonitoringSystem.View
{
    partial class DeviceDetailView
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
            this._DataGridViewParametersViewer = new System.Windows.Forms.DataGridView();
            this._SplitContainerMain.Panel1.SuspendLayout();
            this._SplitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewParametersViewer)).BeginInit();
            this.SuspendLayout();
            // 
            // _SplitContainerMain
            // 
            // 
            // _SplitContainerMain.Panel1
            // 
            this._SplitContainerMain.Panel1.Controls.Add(this._DataGridViewParametersViewer);
            // 
            // _DataGridViewParametersViewer
            // 
            this._DataGridViewParametersViewer.AllowUserToAddRows = false;
            this._DataGridViewParametersViewer.AllowUserToDeleteRows = false;
            this._DataGridViewParametersViewer.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._DataGridViewParametersViewer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._DataGridViewParametersViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._DataGridViewParametersViewer.Location = new System.Drawing.Point(5, 5);
            this._DataGridViewParametersViewer.MultiSelect = false;
            this._DataGridViewParametersViewer.Name = "_DataGridViewParametersViewer";
            this._DataGridViewParametersViewer.ReadOnly = true;
            this._DataGridViewParametersViewer.RowHeadersVisible = false;
            this._DataGridViewParametersViewer.RowTemplate.Height = 24;
            this._DataGridViewParametersViewer.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._DataGridViewParametersViewer.Size = new System.Drawing.Size(394, 398);
            this._DataGridViewParametersViewer.TabIndex = 1;
            // 
            // DeviceDetailView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ButtonF3IsAccessible = true;
            this.ButtonF4IsAccessible = true;
            this.ButtonF5IsAccessible = true;
            this.ClientSize = new System.Drawing.Size(543, 408);
            this.Name = "DeviceDetailView";
            this.Text = "Устройство";
            this._SplitContainerMain.Panel1.ResumeLayout(false);
            this._SplitContainerMain.Panel1.PerformLayout();
            this._SplitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewParametersViewer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _DataGridViewParametersViewer;
    }
}