namespace NGK.Plugins.Views
{
    partial class DevicesList
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
            this._DataGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this._DataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // _DataGridView
            // 
            this._DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._DataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._DataGridView.Location = new System.Drawing.Point(0, 0);
            this._DataGridView.Name = "_DataGridView";
            this._DataGridView.RowTemplate.Height = 24;
            this._DataGridView.Size = new System.Drawing.Size(671, 450);
            this._DataGridView.TabIndex = 0;
            // 
            // DevicesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._DataGridView);
            this.Name = "DevicesView";
            this.Size = new System.Drawing.Size(671, 450);
            ((System.ComponentModel.ISupportInitialize)(this._DataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _DataGridView;
    }
}
