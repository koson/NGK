namespace MvpApplication2.View
{
    partial class MainScreenView
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
            this._CheckBox = new System.Windows.Forms.CheckBox();
            this._Button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _CheckBox
            // 
            this._CheckBox.AutoSize = true;
            this._CheckBox.Location = new System.Drawing.Point(12, 12);
            this._CheckBox.Name = "_CheckBox";
            this._CheckBox.Size = new System.Drawing.Size(256, 21);
            this._CheckBox.TabIndex = 0;
            this._CheckBox.Text = "Разрешить выполнение комманды";
            this._CheckBox.UseVisualStyleBackColor = true;
            this._CheckBox.CheckedChanged += new System.EventHandler(this.EventHandler_CheckBox_CheckedChanged);
            // 
            // _Button
            // 
            this._Button.Location = new System.Drawing.Point(12, 39);
            this._Button.Name = "_Button";
            this._Button.Size = new System.Drawing.Size(95, 31);
            this._Button.TabIndex = 1;
            this._Button.Text = "button1";
            this._Button.UseVisualStyleBackColor = true;
            // 
            // MainScreenView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 323);
            this.Controls.Add(this._Button);
            this.Controls.Add(this._CheckBox);
            this.Name = "MainScreenView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainScreenView";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox _CheckBox;
        private System.Windows.Forms.Button _Button;
    }
}