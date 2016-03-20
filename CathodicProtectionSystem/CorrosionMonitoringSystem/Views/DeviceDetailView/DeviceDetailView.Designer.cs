namespace NGK.CorrosionMonitoringSystem.Views.DeviceDetailView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._DataGridViewDetail = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewDetail)).BeginInit();
            this.SuspendLayout();
            // 
            // _DataGridViewDetail
            // 
            this._DataGridViewDetail.AllowUserToAddRows = false;
            this._DataGridViewDetail.AllowUserToDeleteRows = false;
            this._DataGridViewDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._DataGridViewDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this._DataGridViewDetail.Location = new System.Drawing.Point(0, 0);
            this._DataGridViewDetail.Name = "_DataGridViewDetail";
            this._DataGridViewDetail.ReadOnly = true;
            this._DataGridViewDetail.RowTemplate.Height = 24;
            this._DataGridViewDetail.Size = new System.Drawing.Size(227, 205);
            this._DataGridViewDetail.TabIndex = 0;
            // 
            // DeviceDetailView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._DataGridViewDetail);
            this.Name = "DeviceDetailView";
            this.Size = new System.Drawing.Size(227, 205);
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewDetail)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _DataGridViewDetail;
    }
}
