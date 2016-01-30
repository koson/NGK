namespace NGK.CorrosionMonitoringSystem.View
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._SplitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // LogViewerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ButtonF3IsAccessible = true;
            this.ButtonF4IsAccessible = true;
            this.ButtonF5IsAccessible = true;
            this.ClientSize = new System.Drawing.Size(543, 408);
            this.Name = "LogViewerView";
            this.Text = "Журнал событий";
            this._SplitContainerMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}