namespace NGK.CAN.DataLinkLayer.CanPort.Design.Controls
{
    partial class IXXATCanPortTuner
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeViewAdapters = new System.Windows.Forms.TreeView();
            this.propertyGridAdapter = new System.Windows.Forms.PropertyGrid();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewAdapters);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGridAdapter);
            this.splitContainer1.Size = new System.Drawing.Size(453, 348);
            this.splitContainer1.SplitterDistance = 151;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeViewAdapters
            // 
            this.treeViewAdapters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewAdapters.Location = new System.Drawing.Point(0, 0);
            this.treeViewAdapters.Name = "treeViewAdapters";
            this.treeViewAdapters.Size = new System.Drawing.Size(151, 348);
            this.treeViewAdapters.TabIndex = 0;
            // 
            // propertyGridAdapter
            // 
            this.propertyGridAdapter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridAdapter.Location = new System.Drawing.Point(0, 0);
            this.propertyGridAdapter.Name = "propertyGridAdapter";
            this.propertyGridAdapter.Size = new System.Drawing.Size(298, 348);
            this.propertyGridAdapter.TabIndex = 0;
            // 
            // IXXATCanPortTuner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "IXXATCanPortTuner";
            this.Size = new System.Drawing.Size(453, 348);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeViewAdapters;
        private System.Windows.Forms.PropertyGrid propertyGridAdapter;

    }
}
