namespace NGK.Plugins.Views
{
    partial class SystemEventsLogArchive
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
            this._PanelInformation = new System.Windows.Forms.Panel();
            this._DataGridViewSystemEventsLog = new System.Windows.Forms.DataGridView();
            this._LabelTotalPagesCaption = new System.Windows.Forms.Label();
            this._LabelPageNumberCaption = new System.Windows.Forms.Label();
            this._LabelTotalPages = new System.Windows.Forms.Label();
            this._LabelPageNumber = new System.Windows.Forms.Label();
            this._PanelInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewSystemEventsLog)).BeginInit();
            this.SuspendLayout();
            // 
            // _PanelInformation
            // 
            this._PanelInformation.Controls.Add(this._LabelPageNumber);
            this._PanelInformation.Controls.Add(this._LabelTotalPages);
            this._PanelInformation.Controls.Add(this._LabelPageNumberCaption);
            this._PanelInformation.Controls.Add(this._LabelTotalPagesCaption);
            this._PanelInformation.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._PanelInformation.Location = new System.Drawing.Point(0, 284);
            this._PanelInformation.Name = "_PanelInformation";
            this._PanelInformation.Size = new System.Drawing.Size(360, 24);
            this._PanelInformation.TabIndex = 0;
            // 
            // _DataGridViewSystemEventsLog
            // 
            this._DataGridViewSystemEventsLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._DataGridViewSystemEventsLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this._DataGridViewSystemEventsLog.Location = new System.Drawing.Point(0, 0);
            this._DataGridViewSystemEventsLog.Name = "_DataGridViewSystemEventsLog";
            this._DataGridViewSystemEventsLog.Size = new System.Drawing.Size(360, 284);
            this._DataGridViewSystemEventsLog.TabIndex = 1;
            // 
            // _LabelTotalPagesCaption
            // 
            this._LabelTotalPagesCaption.AutoSize = true;
            this._LabelTotalPagesCaption.Location = new System.Drawing.Point(3, 3);
            this._LabelTotalPagesCaption.Name = "_LabelTotalPagesCaption";
            this._LabelTotalPagesCaption.Size = new System.Drawing.Size(84, 13);
            this._LabelTotalPagesCaption.TabIndex = 0;
            this._LabelTotalPagesCaption.Text = "Всего страниц:";
            // 
            // _LabelPageNumberCaption
            // 
            this._LabelPageNumberCaption.AutoSize = true;
            this._LabelPageNumberCaption.Location = new System.Drawing.Point(131, 3);
            this._LabelPageNumberCaption.Name = "_LabelPageNumberCaption";
            this._LabelPageNumberCaption.Size = new System.Drawing.Size(58, 13);
            this._LabelPageNumberCaption.TabIndex = 1;
            this._LabelPageNumberCaption.Text = "Страница:";
            // 
            // _LabelTotalPages
            // 
            this._LabelTotalPages.AutoSize = true;
            this._LabelTotalPages.Location = new System.Drawing.Point(84, 3);
            this._LabelTotalPages.Name = "_LabelTotalPages";
            this._LabelTotalPages.Size = new System.Drawing.Size(13, 13);
            this._LabelTotalPages.TabIndex = 2;
            this._LabelTotalPages.Text = "0";
            // 
            // _LabelPageNumber
            // 
            this._LabelPageNumber.AutoSize = true;
            this._LabelPageNumber.Location = new System.Drawing.Point(186, 3);
            this._LabelPageNumber.Name = "_LabelPageNumber";
            this._LabelPageNumber.Size = new System.Drawing.Size(13, 13);
            this._LabelPageNumber.TabIndex = 3;
            this._LabelPageNumber.Text = "0";
            // 
            // SystemEventsLogArchive
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._DataGridViewSystemEventsLog);
            this.Controls.Add(this._PanelInformation);
            this.Name = "SystemEventsLogArchive";
            this.Size = new System.Drawing.Size(360, 308);
            this._PanelInformation.ResumeLayout(false);
            this._PanelInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewSystemEventsLog)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _PanelInformation;
        private System.Windows.Forms.DataGridView _DataGridViewSystemEventsLog;
        private System.Windows.Forms.Label _LabelTotalPagesCaption;
        private System.Windows.Forms.Label _LabelPageNumberCaption;
        private System.Windows.Forms.Label _LabelPageNumber;
        private System.Windows.Forms.Label _LabelTotalPages;

    }
}
