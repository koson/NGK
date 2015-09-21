namespace NGK.MeasuringDeviceTech.Classes.MeasuringDevice.UITypeEditors
{
    partial class Channel4_20Control
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
            this.numericUpDownValue = new System.Windows.Forms.NumericUpDown();
            this.checkBoxChannelStatus = new System.Windows.Forms.CheckBox();
            this.buttonOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownValue)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDownValue
            // 
            this.numericUpDownValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericUpDownValue.Location = new System.Drawing.Point(0, 0);
            this.numericUpDownValue.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.numericUpDownValue.Maximum = new decimal(new int[] {
            655350,
            0,
            0,
            0});
            this.numericUpDownValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownValue.Name = "numericUpDownValue";
            this.numericUpDownValue.Size = new System.Drawing.Size(115, 20);
            this.numericUpDownValue.TabIndex = 3;
            this.numericUpDownValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownValue.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // checkBoxChannelStatus
            // 
            this.checkBoxChannelStatus.AutoSize = true;
            this.checkBoxChannelStatus.Location = new System.Drawing.Point(2, 25);
            this.checkBoxChannelStatus.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBoxChannelStatus.Name = "checkBoxChannelStatus";
            this.checkBoxChannelStatus.Size = new System.Drawing.Size(75, 17);
            this.checkBoxChannelStatus.TabIndex = 2;
            this.checkBoxChannelStatus.Text = "Включить";
            this.checkBoxChannelStatus.UseVisualStyleBackColor = true;
            this.checkBoxChannelStatus.CheckedChanged += new System.EventHandler(this.checkBoxChannelStatus_CheckedChanged);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(78, 23);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(35, 19);
            this.buttonOk.TabIndex = 4;
            this.buttonOk.Text = "ОК";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // Channel4_20Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.numericUpDownValue);
            this.Controls.Add(this.checkBoxChannelStatus);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Channel4_20Control";
            this.Size = new System.Drawing.Size(115, 45);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Channel4_20UITypeEditor_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDownValue;
        private System.Windows.Forms.CheckBox checkBoxChannelStatus;
        private System.Windows.Forms.Button buttonOk;
    }
}
