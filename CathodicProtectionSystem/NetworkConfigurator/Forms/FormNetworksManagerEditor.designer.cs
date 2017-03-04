namespace NGK.NetworkConfigurator
{
    partial class NetworksManagerEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetworksManagerEditor));
            this._StatusStripMain = new System.Windows.Forms.StatusStrip();
            this._MenuStripMain = new System.Windows.Forms.MenuStrip();
            this._SplitContainerMain = new System.Windows.Forms.SplitContainer();
            this._SplitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // _StatusStripMain
            // 
            this._StatusStripMain.Location = new System.Drawing.Point(0, 374);
            this._StatusStripMain.Name = "_StatusStripMain";
            this._StatusStripMain.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this._StatusStripMain.Size = new System.Drawing.Size(632, 22);
            this._StatusStripMain.TabIndex = 0;
            this._StatusStripMain.Text = "_StatusStripMain";
            // 
            // _MenuStripMain
            // 
            this._MenuStripMain.Location = new System.Drawing.Point(0, 0);
            this._MenuStripMain.Name = "_MenuStripMain";
            this._MenuStripMain.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this._MenuStripMain.Size = new System.Drawing.Size(632, 24);
            this._MenuStripMain.TabIndex = 1;
            this._MenuStripMain.Text = "menuStrip1";
            // 
            // _SplitContainerMain
            // 
            this._SplitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this._SplitContainerMain.Location = new System.Drawing.Point(0, 24);
            this._SplitContainerMain.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._SplitContainerMain.Name = "_SplitContainerMain";
            this._SplitContainerMain.Size = new System.Drawing.Size(632, 350);
            this._SplitContainerMain.SplitterDistance = 210;
            this._SplitContainerMain.SplitterWidth = 3;
            this._SplitContainerMain.TabIndex = 2;
            // 
            // NetworksManagerEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 396);
            this.Controls.Add(this._SplitContainerMain);
            this.Controls.Add(this._StatusStripMain);
            this.Controls.Add(this._MenuStripMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this._MenuStripMain;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "NetworksManagerEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Конфигуратор";
            this._SplitContainerMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip _StatusStripMain;
        private System.Windows.Forms.MenuStrip _MenuStripMain;
        private System.Windows.Forms.SplitContainer _SplitContainerMain;
    }
}