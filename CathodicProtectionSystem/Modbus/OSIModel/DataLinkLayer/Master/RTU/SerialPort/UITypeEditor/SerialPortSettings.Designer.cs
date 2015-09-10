namespace Modbus.OSIModel.DataLinkLayer.Master.RTU.SerialPort.UITypeEditor
{
    partial class SerialPortSettings
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxParity = new System.Windows.Forms.ComboBox();
            this.comboBoxStopBits = new System.Windows.Forms.ComboBox();
            this.labelParity = new System.Windows.Forms.Label();
            this.labelStopBits = new System.Windows.Forms.Label();
            this.labelDataBits = new System.Windows.Forms.Label();
            this.comboBoxDataBits = new System.Windows.Forms.ComboBox();
            this.comboBoxBautRate = new System.Windows.Forms.ComboBox();
            this.labelBautRate = new System.Windows.Forms.Label();
            this.labelSerialPort = new System.Windows.Forms.Label();
            this.comboBoxPortName = new System.Windows.Forms.ComboBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.comboBoxParity);
            this.groupBox1.Controls.Add(this.comboBoxStopBits);
            this.groupBox1.Controls.Add(this.labelParity);
            this.groupBox1.Controls.Add(this.labelStopBits);
            this.groupBox1.Controls.Add(this.labelDataBits);
            this.groupBox1.Controls.Add(this.comboBoxDataBits);
            this.groupBox1.Controls.Add(this.comboBoxBautRate);
            this.groupBox1.Controls.Add(this.labelBautRate);
            this.groupBox1.Controls.Add(this.labelSerialPort);
            this.groupBox1.Controls.Add(this.comboBoxPortName);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(209, 185);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Настройки порта";
            // 
            // comboBoxParity
            // 
            this.comboBoxParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxParity.FormattingEnabled = true;
            this.comboBoxParity.Location = new System.Drawing.Point(94, 121);
            this.comboBoxParity.Name = "comboBoxParity";
            this.comboBoxParity.Size = new System.Drawing.Size(109, 24);
            this.comboBoxParity.TabIndex = 12;
            this.comboBoxParity.SelectedIndexChanged += new System.EventHandler(this.comboBoxParity_SelectedIndexChanged);
            // 
            // comboBoxStopBits
            // 
            this.comboBoxStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStopBits.FormattingEnabled = true;
            this.comboBoxStopBits.Location = new System.Drawing.Point(94, 151);
            this.comboBoxStopBits.Name = "comboBoxStopBits";
            this.comboBoxStopBits.Size = new System.Drawing.Size(109, 24);
            this.comboBoxStopBits.TabIndex = 11;
            this.comboBoxStopBits.SelectedIndexChanged += new System.EventHandler(this.comboBoxStopBits_SelectedIndexChanged);
            // 
            // labelParity
            // 
            this.labelParity.AutoSize = true;
            this.labelParity.Location = new System.Drawing.Point(6, 124);
            this.labelParity.Name = "labelParity";
            this.labelParity.Size = new System.Drawing.Size(64, 17);
            this.labelParity.TabIndex = 10;
            this.labelParity.Text = "Паритет";
            // 
            // labelStopBits
            // 
            this.labelStopBits.AutoSize = true;
            this.labelStopBits.Location = new System.Drawing.Point(6, 154);
            this.labelStopBits.Name = "labelStopBits";
            this.labelStopBits.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelStopBits.Size = new System.Drawing.Size(81, 17);
            this.labelStopBits.TabIndex = 9;
            this.labelStopBits.Text = "Стоп биты:";
            // 
            // labelDataBits
            // 
            this.labelDataBits.AutoSize = true;
            this.labelDataBits.Location = new System.Drawing.Point(6, 94);
            this.labelDataBits.Name = "labelDataBits";
            this.labelDataBits.Size = new System.Drawing.Size(65, 17);
            this.labelDataBits.TabIndex = 8;
            this.labelDataBits.Text = "Данные:";
            // 
            // comboBoxDataBits
            // 
            this.comboBoxDataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDataBits.FormattingEnabled = true;
            this.comboBoxDataBits.Location = new System.Drawing.Point(94, 91);
            this.comboBoxDataBits.Name = "comboBoxDataBits";
            this.comboBoxDataBits.Size = new System.Drawing.Size(109, 24);
            this.comboBoxDataBits.TabIndex = 7;
            this.comboBoxDataBits.SelectedIndexChanged += new System.EventHandler(this.comboBoxDataBits_SelectedIndexChanged);
            // 
            // comboBoxBautRate
            // 
            this.comboBoxBautRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBautRate.Location = new System.Drawing.Point(94, 61);
            this.comboBoxBautRate.Name = "comboBoxBautRate";
            this.comboBoxBautRate.Size = new System.Drawing.Size(109, 24);
            this.comboBoxBautRate.TabIndex = 6;
            this.comboBoxBautRate.SelectedIndexChanged += new System.EventHandler(this.comboBoxBautRate_SelectedIndexChanged);
            // 
            // labelBautRate
            // 
            this.labelBautRate.AutoSize = true;
            this.labelBautRate.Location = new System.Drawing.Point(6, 64);
            this.labelBautRate.Name = "labelBautRate";
            this.labelBautRate.Size = new System.Drawing.Size(73, 17);
            this.labelBautRate.TabIndex = 5;
            this.labelBautRate.Text = "Скорость:";
            // 
            // labelSerialPort
            // 
            this.labelSerialPort.AutoSize = true;
            this.labelSerialPort.Location = new System.Drawing.Point(6, 34);
            this.labelSerialPort.Name = "labelSerialPort";
            this.labelSerialPort.Size = new System.Drawing.Size(45, 17);
            this.labelSerialPort.TabIndex = 4;
            this.labelSerialPort.Text = "Порт:";
            // 
            // comboBoxPortName
            // 
            this.comboBoxPortName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPortName.FormattingEnabled = true;
            this.comboBoxPortName.Location = new System.Drawing.Point(94, 31);
            this.comboBoxPortName.Name = "comboBoxPortName";
            this.comboBoxPortName.Size = new System.Drawing.Size(109, 24);
            this.comboBoxPortName.TabIndex = 3;
            this.comboBoxPortName.SelectedIndexChanged += new System.EventHandler(this.comboBoxPortName_SelectedIndexChanged);
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(139, 191);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 31);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // SerialPortSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBox1);
            this.Name = "SerialPortSettings";
            this.Size = new System.Drawing.Size(217, 227);
            this.Load += new System.EventHandler(this.SerialPortSettings_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SerialPortSettings_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label labelSerialPort;
        private System.Windows.Forms.ComboBox comboBoxPortName;
        private System.Windows.Forms.ComboBox comboBoxBautRate;
        private System.Windows.Forms.Label labelBautRate;
        private System.Windows.Forms.Label labelDataBits;
        private System.Windows.Forms.ComboBox comboBoxDataBits;
        private System.Windows.Forms.ComboBox comboBoxParity;
        private System.Windows.Forms.ComboBox comboBoxStopBits;
        private System.Windows.Forms.Label labelParity;
        private System.Windows.Forms.Label labelStopBits;
    }
}
