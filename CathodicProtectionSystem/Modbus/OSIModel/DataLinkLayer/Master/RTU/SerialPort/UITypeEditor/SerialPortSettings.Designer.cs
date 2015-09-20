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
            groupBox1 = new System.Windows.Forms.GroupBox();
            comboBoxParity = new System.Windows.Forms.ComboBox();
            comboBoxStopBits = new System.Windows.Forms.ComboBox();
            labelParity = new System.Windows.Forms.Label();
            labelStopBits = new System.Windows.Forms.Label();
            labelDataBits = new System.Windows.Forms.Label();
            comboBoxDataBits = new System.Windows.Forms.ComboBox();
            comboBoxBautRate = new System.Windows.Forms.ComboBox();
            labelBautRate = new System.Windows.Forms.Label();
            labelSerialPort = new System.Windows.Forms.Label();
            comboBoxPortName = new System.Windows.Forms.ComboBox();
            buttonOk = new System.Windows.Forms.Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            groupBox1.Controls.Add(comboBoxParity);
            groupBox1.Controls.Add(comboBoxStopBits);
            groupBox1.Controls.Add(labelParity);
            groupBox1.Controls.Add(labelStopBits);
            groupBox1.Controls.Add(labelDataBits);
            groupBox1.Controls.Add(comboBoxDataBits);
            groupBox1.Controls.Add(comboBoxBautRate);
            groupBox1.Controls.Add(labelBautRate);
            groupBox1.Controls.Add(labelSerialPort);
            groupBox1.Controls.Add(comboBoxPortName);
            groupBox1.Location = new System.Drawing.Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(209, 185);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Настройки порта";
            // 
            // comboBoxParity
            // 
            comboBoxParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxParity.FormattingEnabled = true;
            comboBoxParity.Location = new System.Drawing.Point(94, 121);
            comboBoxParity.Name = "comboBoxParity";
            comboBoxParity.Size = new System.Drawing.Size(109, 24);
            comboBoxParity.TabIndex = 12;
            comboBoxParity.SelectedIndexChanged += new System.EventHandler(comboBoxParity_SelectedIndexChanged);
            // 
            // comboBoxStopBits
            // 
            comboBoxStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxStopBits.FormattingEnabled = true;
            comboBoxStopBits.Location = new System.Drawing.Point(94, 151);
            comboBoxStopBits.Name = "comboBoxStopBits";
            comboBoxStopBits.Size = new System.Drawing.Size(109, 24);
            comboBoxStopBits.TabIndex = 11;
            comboBoxStopBits.SelectedIndexChanged += new System.EventHandler(comboBoxStopBits_SelectedIndexChanged);
            // 
            // labelParity
            // 
            labelParity.AutoSize = true;
            labelParity.Location = new System.Drawing.Point(6, 124);
            labelParity.Name = "labelParity";
            labelParity.Size = new System.Drawing.Size(64, 17);
            labelParity.TabIndex = 10;
            labelParity.Text = "Паритет";
            // 
            // labelStopBits
            // 
            labelStopBits.AutoSize = true;
            labelStopBits.Location = new System.Drawing.Point(6, 154);
            labelStopBits.Name = "labelStopBits";
            labelStopBits.RightToLeft = System.Windows.Forms.RightToLeft.No;
            labelStopBits.Size = new System.Drawing.Size(81, 17);
            labelStopBits.TabIndex = 9;
            labelStopBits.Text = "Стоп биты:";
            // 
            // labelDataBits
            // 
            labelDataBits.AutoSize = true;
            labelDataBits.Location = new System.Drawing.Point(6, 94);
            labelDataBits.Name = "labelDataBits";
            labelDataBits.Size = new System.Drawing.Size(65, 17);
            labelDataBits.TabIndex = 8;
            labelDataBits.Text = "Данные:";
            // 
            // comboBoxDataBits
            // 
            comboBoxDataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxDataBits.FormattingEnabled = true;
            comboBoxDataBits.Location = new System.Drawing.Point(94, 91);
            comboBoxDataBits.Name = "comboBoxDataBits";
            comboBoxDataBits.Size = new System.Drawing.Size(109, 24);
            comboBoxDataBits.TabIndex = 7;
            comboBoxDataBits.SelectedIndexChanged += new System.EventHandler(comboBoxDataBits_SelectedIndexChanged);
            // 
            // comboBoxBautRate
            // 
            comboBoxBautRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxBautRate.Location = new System.Drawing.Point(94, 61);
            comboBoxBautRate.Name = "comboBoxBautRate";
            comboBoxBautRate.Size = new System.Drawing.Size(109, 24);
            comboBoxBautRate.TabIndex = 6;
            comboBoxBautRate.SelectedIndexChanged += new System.EventHandler(comboBoxBautRate_SelectedIndexChanged);
            // 
            // labelBautRate
            // 
            labelBautRate.AutoSize = true;
            labelBautRate.Location = new System.Drawing.Point(6, 64);
            labelBautRate.Name = "labelBautRate";
            labelBautRate.Size = new System.Drawing.Size(73, 17);
            labelBautRate.TabIndex = 5;
            labelBautRate.Text = "Скорость:";
            // 
            // labelSerialPort
            // 
            labelSerialPort.AutoSize = true;
            labelSerialPort.Location = new System.Drawing.Point(6, 34);
            labelSerialPort.Name = "labelSerialPort";
            labelSerialPort.Size = new System.Drawing.Size(45, 17);
            labelSerialPort.TabIndex = 4;
            labelSerialPort.Text = "Порт:";
            // 
            // comboBoxPortName
            // 
            comboBoxPortName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxPortName.FormattingEnabled = true;
            comboBoxPortName.Location = new System.Drawing.Point(94, 31);
            comboBoxPortName.Name = "comboBoxPortName";
            comboBoxPortName.Size = new System.Drawing.Size(109, 24);
            comboBoxPortName.TabIndex = 3;
            comboBoxPortName.SelectedIndexChanged += new System.EventHandler(comboBoxPortName_SelectedIndexChanged);
            // 
            // buttonOk
            // 
            buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            buttonOk.Location = new System.Drawing.Point(139, 191);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new System.Drawing.Size(75, 31);
            buttonOk.TabIndex = 1;
            buttonOk.Text = "OK";
            buttonOk.UseVisualStyleBackColor = true;
            buttonOk.Click += new System.EventHandler(buttonOk_Click);
            // 
            // SerialPortSettings
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            Controls.Add(buttonOk);
            Controls.Add(groupBox1);
            Name = "SerialPortSettings";
            Size = new System.Drawing.Size(217, 227);
            Load += new System.EventHandler(SerialPortSettings_Load);
            KeyDown += new System.Windows.Forms.KeyEventHandler(SerialPortSettings_KeyDown);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);

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
