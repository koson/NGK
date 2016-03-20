namespace NGK.CorrosionMonitoringSystem.Views.LogViewerView
{
    partial class LogViewerView
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
            this._DataGridViewLog = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewLog)).BeginInit();
            this.SuspendLayout();
            // 
            // _DataGridViewLog
            // 
            this._DataGridViewLog.AllowUserToAddRows = false;
            this._DataGridViewLog.AllowUserToDeleteRows = false;
            this._DataGridViewLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._DataGridViewLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this._DataGridViewLog.Location = new System.Drawing.Point(0, 0);
            this._DataGridViewLog.Name = "_DataGridViewLog";
            this._DataGridViewLog.ReadOnly = true;
            this._DataGridViewLog.RowTemplate.Height = 24;
            this._DataGridViewLog.Size = new System.Drawing.Size(150, 150);
            this._DataGridViewLog.TabIndex = 0;
            // 
            // LogViewerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._DataGridViewLog);
            this.Name = "LogViewerView";
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewLog)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _DataGridViewLog;
    }
}
