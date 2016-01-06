namespace MdiApplication
{
    partial class ChildOneForm
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
            this._ButtonExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _ButtonExit
            // 
            this._ButtonExit.Location = new System.Drawing.Point(23, 12);
            this._ButtonExit.Name = "_ButtonExit";
            this._ButtonExit.Size = new System.Drawing.Size(75, 32);
            this._ButtonExit.TabIndex = 0;
            this._ButtonExit.Text = "Закрыть";
            this._ButtonExit.UseVisualStyleBackColor = true;
            this._ButtonExit.Click += new System.EventHandler(this._ButtonExit_Click);
            // 
            // ChildOneForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 268);
            this.Controls.Add(this._ButtonExit);
            this.Name = "ChildOneForm";
            this.Text = "ChildOneForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _ButtonExit;
    }
}