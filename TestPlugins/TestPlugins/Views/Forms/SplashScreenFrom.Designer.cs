namespace TestPlugins.Views
{
    partial class SplashScreenFrom
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
            this._LabelOutput = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _LabelOutput
            // 
            this._LabelOutput.AutoSize = true;
            this._LabelOutput.Location = new System.Drawing.Point(12, 48);
            this._LabelOutput.Name = "_LabelOutput";
            this._LabelOutput.Size = new System.Drawing.Size(65, 13);
            this._LabelOutput.TabIndex = 0;
            this._LabelOutput.Text = "LabelOutput";
            // 
            // SplashScreenView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 333);
            this.Controls.Add(this._LabelOutput);
            this.Name = "SplashScreenView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SplashScreenView";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _LabelOutput;
    }
}