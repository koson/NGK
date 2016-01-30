namespace NGK.CorrosionMonitoringSystem.View
{
    partial class PivotTableView
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
            this._DataGridViewPivotTable = new System.Windows.Forms.DataGridView();
            this._SplitContainerMain.Panel1.SuspendLayout();
            this._SplitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewPivotTable)).BeginInit();
            this.SuspendLayout();
            // 
            // _SplitContainerMain
            // 
            // 
            // _SplitContainerMain.Panel1
            // 
            this._SplitContainerMain.Panel1.Controls.Add(this._DataGridViewPivotTable);
            this._SplitContainerMain.Size = new System.Drawing.Size(670, 436);
            this._SplitContainerMain.SplitterDistance = 550;
            // 
            // _DataGridViewPivotTable
            // 
            this._DataGridViewPivotTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._DataGridViewPivotTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._DataGridViewPivotTable.Location = new System.Drawing.Point(5, 5);
            this._DataGridViewPivotTable.Name = "_DataGridViewPivotTable";
            this._DataGridViewPivotTable.RowTemplate.Height = 24;
            this._DataGridViewPivotTable.Size = new System.Drawing.Size(540, 426);
            this._DataGridViewPivotTable.TabIndex = 1;
            // 
            // PivotTableView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ButtonF3IsAccessible = true;
            this.ButtonF4IsAccessible = true;
            this.ButtonF5IsAccessible = true;
            this.ClientSize = new System.Drawing.Size(670, 436);
            this.Name = "PivotTableView";
            this.Text = "Параметры системы";
            this.Load += new System.EventHandler(this.EventHandler_PivotTableView_Load);
            this._SplitContainerMain.Panel1.ResumeLayout(false);
            this._SplitContainerMain.Panel1.PerformLayout();
            this._SplitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._DataGridViewPivotTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _DataGridViewPivotTable;
    }
}