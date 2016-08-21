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
            this._ButtonMenu = new System.Windows.Forms.Button();
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
            // _ButtonMenu
            // 
            this._ButtonMenu.Location = new System.Drawing.Point(569, 27);
            this._ButtonMenu.Name = "_ButtonMenu";
            this._ButtonMenu.Size = new System.Drawing.Size(75, 23);
            this._ButtonMenu.TabIndex = 2;
            this._ButtonMenu.Text = "Menu";
            this._ButtonMenu.UseVisualStyleBackColor = true;
            // 
            // MainFormView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 532);
            this.Controls.Add(this._ButtonMenu);
            this.Controls.Add(this._StatusStrip);
            this.Controls.Add(this._MenuStrip);
            this.MainMenuStrip = this._MenuStrip;
            this.Name = "MainFormView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainFormView";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip _MenuStrip;
        private System.Windows.Forms.StatusStrip _StatusStrip;
        public System.Windows.Forms.Button _ButtonMenu;
    }
}