namespace PluginA.Views
{
    partial class TestControl
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
            this._Button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _Button
            // 
            this._Button.BackColor = System.Drawing.Color.Red;
            this._Button.Dock = System.Windows.Forms.DockStyle.Fill;
            this._Button.Location = new System.Drawing.Point(0, 0);
            this._Button.Name = "_Button";
            this._Button.Size = new System.Drawing.Size(278, 212);
            this._Button.TabIndex = 0;
            this._Button.Text = "button1";
            this._Button.UseVisualStyleBackColor = false;
            // 
            // TestControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._Button);
            this.Name = "TestControl";
            this.Size = new System.Drawing.Size(278, 212);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button _Button;

    }
}
