namespace NGK.CorrosionMonitoringSystem.View
{
    partial class SplashScreenView
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
            this._LabelOutputInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _LabelOutputInfo
            // 
            this._LabelOutputInfo.AutoSize = true;
            this._LabelOutputInfo.BackColor = System.Drawing.Color.Transparent;
            this._LabelOutputInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this._LabelOutputInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._LabelOutputInfo.ForeColor = System.Drawing.Color.Cyan;
            this._LabelOutputInfo.Location = new System.Drawing.Point(0, 0);
            this._LabelOutputInfo.Name = "_LabelOutputInfo";
            this._LabelOutputInfo.Padding = new System.Windows.Forms.Padding(5);
            this._LabelOutputInfo.Size = new System.Drawing.Size(100, 30);
            this._LabelOutputInfo.TabIndex = 0;
            this._LabelOutputInfo.Text = "Loading...";
            // 
            // SplashScreenView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::NGK.CorrosionMonitoringSystem.Properties.Resources.Logo;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(517, 268);
            this.Controls.Add(this._LabelOutputInfo);
            this.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashScreenView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Splash Screen";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.EventHandler_SplashScreen_Load);
            this.Shown += new System.EventHandler(this.BootstrapperView_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _LabelOutputInfo;
    }
}