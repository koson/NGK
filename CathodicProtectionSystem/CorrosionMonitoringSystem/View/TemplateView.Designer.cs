namespace NGK.CorrosionMonitoringSystem.View
{
    partial class TemplateView
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
            this._SplitContainerMain = new System.Windows.Forms.SplitContainer();
            this._StatusStripMain = new System.Windows.Forms.StatusStrip();
            this._ButtonF6 = new System.Windows.Forms.Button();
            this._ButtonF5 = new System.Windows.Forms.Button();
            this._ButtonF4 = new System.Windows.Forms.Button();
            this._ButtonF3 = new System.Windows.Forms.Button();
            this._ButtonF2 = new System.Windows.Forms.Button();
            this._SplitContainerMain.Panel1.SuspendLayout();
            this._SplitContainerMain.Panel2.SuspendLayout();
            this._SplitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // _SplitContainerMain
            // 
            this._SplitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this._SplitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this._SplitContainerMain.IsSplitterFixed = true;
            this._SplitContainerMain.Location = new System.Drawing.Point(0, 0);
            this._SplitContainerMain.Name = "_SplitContainerMain";
            // 
            // _SplitContainerMain.Panel1
            // 
            this._SplitContainerMain.Panel1.Controls.Add(this._StatusStripMain);
            this._SplitContainerMain.Panel1.Padding = new System.Windows.Forms.Padding(5);
            // 
            // _SplitContainerMain.Panel2
            // 
            this._SplitContainerMain.Panel2.Controls.Add(this._ButtonF6);
            this._SplitContainerMain.Panel2.Controls.Add(this._ButtonF5);
            this._SplitContainerMain.Panel2.Controls.Add(this._ButtonF4);
            this._SplitContainerMain.Panel2.Controls.Add(this._ButtonF3);
            this._SplitContainerMain.Panel2.Controls.Add(this._ButtonF2);
            this._SplitContainerMain.Panel2.Margin = new System.Windows.Forms.Padding(5);
            this._SplitContainerMain.Panel2.Padding = new System.Windows.Forms.Padding(5);
            this._SplitContainerMain.Size = new System.Drawing.Size(543, 408);
            this._SplitContainerMain.SplitterDistance = 404;
            this._SplitContainerMain.TabIndex = 1;
            // 
            // _StatusStripMain
            // 
            this._StatusStripMain.Location = new System.Drawing.Point(5, 381);
            this._StatusStripMain.Margin = new System.Windows.Forms.Padding(5);
            this._StatusStripMain.Name = "_StatusStripMain";
            this._StatusStripMain.Size = new System.Drawing.Size(394, 22);
            this._StatusStripMain.TabIndex = 0;
            this._StatusStripMain.Text = "SystemStatusStrip";
            // 
            // _ButtonF6
            // 
            this._ButtonF6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._ButtonF6.Location = new System.Drawing.Point(8, 279);
            this._ButtonF6.Name = "_ButtonF6";
            this._ButtonF6.Size = new System.Drawing.Size(119, 50);
            this._ButtonF6.TabIndex = 4;
            this._ButtonF6.Text = "Скрыть панель";
            this._ButtonF6.UseVisualStyleBackColor = true;
            // 
            // _ButtonF5
            // 
            this._ButtonF5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._ButtonF5.Location = new System.Drawing.Point(8, 223);
            this._ButtonF5.Name = "_ButtonF5";
            this._ButtonF5.Size = new System.Drawing.Size(119, 50);
            this._ButtonF5.TabIndex = 3;
            this._ButtonF5.Text = "F5";
            this._ButtonF5.UseVisualStyleBackColor = true;
            // 
            // _ButtonF4
            // 
            this._ButtonF4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._ButtonF4.Location = new System.Drawing.Point(8, 167);
            this._ButtonF4.Name = "_ButtonF4";
            this._ButtonF4.Size = new System.Drawing.Size(119, 50);
            this._ButtonF4.TabIndex = 2;
            this._ButtonF4.Text = "F4";
            this._ButtonF4.UseVisualStyleBackColor = true;
            // 
            // _ButtonF3
            // 
            this._ButtonF3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._ButtonF3.Location = new System.Drawing.Point(8, 111);
            this._ButtonF3.Name = "_ButtonF3";
            this._ButtonF3.Size = new System.Drawing.Size(119, 50);
            this._ButtonF3.TabIndex = 1;
            this._ButtonF3.Text = "F3";
            this._ButtonF3.UseVisualStyleBackColor = true;
            // 
            // _ButtonF2
            // 
            this._ButtonF2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._ButtonF2.Location = new System.Drawing.Point(8, 55);
            this._ButtonF2.Name = "_ButtonF2";
            this._ButtonF2.Size = new System.Drawing.Size(119, 50);
            this._ButtonF2.TabIndex = 0;
            this._ButtonF2.Text = "Меню";
            this._ButtonF2.UseVisualStyleBackColor = true;
            // 
            // TemplateView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 408);
            this.Controls.Add(this._SplitContainerMain);
            this.Name = "TemplateView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TemplateView";
            this.Load += new System.EventHandler(this.TemplateView_Load);
            this.Resize += new System.EventHandler(this.EventHandler_TemplateView_Resize);
            this._SplitContainerMain.Panel1.ResumeLayout(false);
            this._SplitContainerMain.Panel1.PerformLayout();
            this._SplitContainerMain.Panel2.ResumeLayout(false);
            this._SplitContainerMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.SplitContainer _SplitContainerMain;
        private System.Windows.Forms.Button _ButtonF2;
        private System.Windows.Forms.Button _ButtonF6;
        private System.Windows.Forms.Button _ButtonF5;
        private System.Windows.Forms.Button _ButtonF4;
        private System.Windows.Forms.Button _ButtonF3;
        private System.Windows.Forms.StatusStrip _StatusStripMain;
    }
}