namespace NGK.Plugins.Views
{
    partial class SystemEventsLog
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
            this._DataGridViewSystemEventsLog = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewSystemEventsLog)).BeginInit();
            this.SuspendLayout();
            // 
            // _DataGridViewSystemEventsLog
            // 
            this._DataGridViewSystemEventsLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._DataGridViewSystemEventsLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this._DataGridViewSystemEventsLog.Location = new System.Drawing.Point(0, 0);
            this._DataGridViewSystemEventsLog.Name = "_DataGridViewSystemEventsLog";
            this._DataGridViewSystemEventsLog.Size = new System.Drawing.Size(360, 308);
            this._DataGridViewSystemEventsLog.TabIndex = 0;
            // 
            // SystemEnventsLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._DataGridViewSystemEventsLog);
            this.Name = "SystemEnventsLog";
            this.Size = new System.Drawing.Size(360, 308);
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewSystemEventsLog)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _DataGridViewSystemEventsLog;
    }
}
