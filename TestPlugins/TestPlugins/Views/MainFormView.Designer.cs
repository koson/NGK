namespace TestPlugins.Views
{
    partial class MainFormView
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
            this._MenuStrip = new System.Windows.Forms.MenuStrip();
            this._StatusStrip = new System.Windows.Forms.StatusStrip();
            this.PanelTitle = new System.Windows.Forms.Panel();
            this._LabelTiltle = new System.Windows.Forms.Label();
            this._SplitContainer = new System.Windows.Forms.SplitContainer();
            this._ButtonMenu = new System.Windows.Forms.Button();
            this.PanelTitle.SuspendLayout();
            this._SplitContainer.Panel2.SuspendLayout();
            this._SplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // _MenuStrip
            // 
            this._MenuStrip.Location = new System.Drawing.Point(0, 0);
            this._MenuStrip.Name = "_MenuStrip";
            this._MenuStrip.Size = new System.Drawing.Size(644, 24);
            this._MenuStrip.TabIndex = 0;
            this._MenuStrip.Text = "menuStrip1";
            // 
            // _StatusStrip
            // 
            this._StatusStrip.Location = new System.Drawing.Point(0, 510);
            this._StatusStrip.Name = "_StatusStrip";
            this._StatusStrip.Size = new System.Drawing.Size(644, 22);
            this._StatusStrip.TabIndex = 1;
            this._StatusStrip.Text = "statusStrip1";
            // 
            // PanelTitle
            // 
            this.PanelTitle.Controls.Add(this._LabelTiltle);
            this.PanelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelTitle.Location = new System.Drawing.Point(0, 24);
            this.PanelTitle.Name = "PanelTitle";
            this.PanelTitle.Size = new System.Drawing.Size(644, 26);
            this.PanelTitle.TabIndex = 5;
            // 
            // _LabelTiltle
            // 
            this._LabelTiltle.AutoSize = true;
            this._LabelTiltle.Location = new System.Drawing.Point(266, 10);
            this._LabelTiltle.Name = "_LabelTiltle";
            this._LabelTiltle.Size = new System.Drawing.Size(37, 13);
            this._LabelTiltle.TabIndex = 0;
            this._LabelTiltle.Text = "TITLE";
            this._LabelTiltle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _SplitContainer
            // 
            this._SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._SplitContainer.Location = new System.Drawing.Point(0, 50);
            this._SplitContainer.Name = "_SplitContainer";
            // 
            // _SplitContainer.Panel2
            // 
            this._SplitContainer.Panel2.Controls.Add(this._ButtonMenu);
            this._SplitContainer.Size = new System.Drawing.Size(644, 460);
            this._SplitContainer.SplitterDistance = 526;
            this._SplitContainer.TabIndex = 7;
            // 
            // _ButtonMenu
            // 
            this._ButtonMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._ButtonMenu.Location = new System.Drawing.Point(3, 6);
            this._ButtonMenu.Name = "_ButtonMenu";
            this._ButtonMenu.Size = new System.Drawing.Size(108, 23);
            this._ButtonMenu.TabIndex = 7;
            this._ButtonMenu.Text = "Menu";
            this._ButtonMenu.UseVisualStyleBackColor = true;
            // 
            // MainFormView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 532);
            this.Controls.Add(this._SplitContainer);
            this.Controls.Add(this.PanelTitle);
            this.Controls.Add(this._StatusStrip);
            this.Controls.Add(this._MenuStrip);
            this.MainMenuStrip = this._MenuStrip;
            this.Name = "MainFormView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainFormView";
            this.Load += new System.EventHandler(this.EventHandler_MainFormView_Load);
            this.PanelTitle.ResumeLayout(false);
            this.PanelTitle.PerformLayout();
            this._SplitContainer.Panel2.ResumeLayout(false);
            this._SplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip _MenuStrip;
        private System.Windows.Forms.StatusStrip _StatusStrip;
        private System.Windows.Forms.Panel PanelTitle;
        private System.Windows.Forms.Label _LabelTiltle;
        private System.Windows.Forms.SplitContainer _SplitContainer;
        public System.Windows.Forms.Button _ButtonMenu;
    }
}