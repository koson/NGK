namespace NGK.CorrosionMonitoringSystem.Views
{
    partial class NavigationMenuView
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
            this._ÒableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._ButtonExit = new System.Windows.Forms.Button();
            this._ÒableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _ÒableLayoutPanel
            // 
            this._ÒableLayoutPanel.ColumnCount = 1;
            this._ÒableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._ÒableLayoutPanel.Controls.Add(this._ButtonExit, 0, 0);
            this._ÒableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._ÒableLayoutPanel.Location = new System.Drawing.Point(5, 5);
            this._ÒableLayoutPanel.Name = "_ÒableLayoutPanel";
            this._ÒableLayoutPanel.RowCount = 1;
            this._ÒableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._ÒableLayoutPanel.Size = new System.Drawing.Size(226, 99);
            this._ÒableLayoutPanel.TabIndex = 1;
            // 
            // _ButtonExit
            // 
            this._ButtonExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._ButtonExit.Dock = System.Windows.Forms.DockStyle.Fill;
            this._ButtonExit.Location = new System.Drawing.Point(3, 3);
            this._ButtonExit.Name = "_ButtonExit";
            this._ButtonExit.Size = new System.Drawing.Size(220, 93);
            this._ButtonExit.TabIndex = 1;
            this._ButtonExit.Text = "Âûõîä";
            this._ButtonExit.UseVisualStyleBackColor = true;
            // 
            // NavigationMenuView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(236, 109);
            this.Controls.Add(this._ÒableLayoutPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NavigationMenuView";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ìåíþ";
            this.Load += new System.EventHandler(this.EventHandler_NavigationMenuForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EventHandler_NavigationMenuView_FormClosed);
            this._ÒableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _ÒableLayoutPanel;
        private System.Windows.Forms.Button _ButtonExit;

    }
}