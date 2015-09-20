namespace Modbus.OSIModel.DataLinkLayer.Dialogs
{
    partial class FormSerialPortSettings
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
            _ComboBoxSerialPort = new System.Windows.Forms.ComboBox();
            _GroupBoxComPort = new System.Windows.Forms.GroupBox();
            _LabelDataBits = new System.Windows.Forms.Label();
            _LabelSerialPort = new System.Windows.Forms.Label();
            _ComboBoxDataBits = new System.Windows.Forms.ComboBox();
            _ComboBoxBaudRate = new System.Windows.Forms.ComboBox();
            _ComboBoxParity = new System.Windows.Forms.ComboBox();
            _ComboBoxStopBits = new System.Windows.Forms.ComboBox();
            _LabelBaudRate = new System.Windows.Forms.Label();
            _LabelParity = new System.Windows.Forms.Label();
            _LabelStopBits = new System.Windows.Forms.Label();
            _GroupBoxComPort.SuspendLayout();
            SuspendLayout();
            // 
            // _ComboBoxSerialPort
            // 
            _ComboBoxSerialPort.FormattingEnabled = true;
            _ComboBoxSerialPort.Location = new System.Drawing.Point(93, 21);
            _ComboBoxSerialPort.Name = "_ComboBoxSerialPort";
            _ComboBoxSerialPort.Size = new System.Drawing.Size(88, 24);
            _ComboBoxSerialPort.TabIndex = 0;
            // 
            // _GroupBoxComPort
            // 
            _GroupBoxComPort.Controls.Add(_LabelStopBits);
            _GroupBoxComPort.Controls.Add(_LabelParity);
            _GroupBoxComPort.Controls.Add(_LabelBaudRate);
            _GroupBoxComPort.Controls.Add(_ComboBoxStopBits);
            _GroupBoxComPort.Controls.Add(_ComboBoxParity);
            _GroupBoxComPort.Controls.Add(_ComboBoxBaudRate);
            _GroupBoxComPort.Controls.Add(_ComboBoxDataBits);
            _GroupBoxComPort.Controls.Add(_LabelSerialPort);
            _GroupBoxComPort.Controls.Add(_LabelDataBits);
            _GroupBoxComPort.Controls.Add(_ComboBoxSerialPort);
            _GroupBoxComPort.Location = new System.Drawing.Point(12, 12);
            _GroupBoxComPort.Name = "_GroupBoxComPort";
            _GroupBoxComPort.Size = new System.Drawing.Size(191, 185);
            _GroupBoxComPort.TabIndex = 1;
            _GroupBoxComPort.TabStop = false;
            _GroupBoxComPort.Text = "COM-порт";
            // 
            // _LabelDataBits
            // 
            _LabelDataBits.AutoSize = true;
            _LabelDataBits.Location = new System.Drawing.Point(6, 54);
            _LabelDataBits.Name = "_LabelDataBits";
            _LabelDataBits.Size = new System.Drawing.Size(65, 17);
            _LabelDataBits.TabIndex = 1;
            _LabelDataBits.Text = "Данные:";
            // 
            // _LabelSerialPort
            // 
            _LabelSerialPort.AutoSize = true;
            _LabelSerialPort.Location = new System.Drawing.Point(6, 24);
            _LabelSerialPort.Name = "_LabelSerialPort";
            _LabelSerialPort.Size = new System.Drawing.Size(45, 17);
            _LabelSerialPort.TabIndex = 2;
            _LabelSerialPort.Text = "Порт:";
            // 
            // _ComboBoxDataBits
            // 
            _ComboBoxDataBits.FormattingEnabled = true;
            _ComboBoxDataBits.Location = new System.Drawing.Point(93, 51);
            _ComboBoxDataBits.Name = "_ComboBoxDataBits";
            _ComboBoxDataBits.Size = new System.Drawing.Size(88, 24);
            _ComboBoxDataBits.TabIndex = 3;
            // 
            // _ComboBoxBaudRate
            // 
            _ComboBoxBaudRate.FormattingEnabled = true;
            _ComboBoxBaudRate.Location = new System.Drawing.Point(93, 81);
            _ComboBoxBaudRate.Name = "_ComboBoxBaudRate";
            _ComboBoxBaudRate.Size = new System.Drawing.Size(88, 24);
            _ComboBoxBaudRate.TabIndex = 4;
            // 
            // _ComboBoxParity
            // 
            _ComboBoxParity.FormattingEnabled = true;
            _ComboBoxParity.Location = new System.Drawing.Point(93, 111);
            _ComboBoxParity.Name = "_ComboBoxParity";
            _ComboBoxParity.Size = new System.Drawing.Size(88, 24);
            _ComboBoxParity.TabIndex = 5;
            // 
            // _ComboBoxStopBits
            // 
            _ComboBoxStopBits.FormattingEnabled = true;
            _ComboBoxStopBits.Location = new System.Drawing.Point(93, 141);
            _ComboBoxStopBits.Name = "_ComboBoxStopBits";
            _ComboBoxStopBits.Size = new System.Drawing.Size(88, 24);
            _ComboBoxStopBits.TabIndex = 6;
            // 
            // _LabelBaudRate
            // 
            _LabelBaudRate.AutoSize = true;
            _LabelBaudRate.Location = new System.Drawing.Point(6, 84);
            _LabelBaudRate.Name = "_LabelBaudRate";
            _LabelBaudRate.Size = new System.Drawing.Size(73, 17);
            _LabelBaudRate.TabIndex = 7;
            _LabelBaudRate.Text = "Скорость:";
            // 
            // _LabelParity
            // 
            _LabelParity.AutoSize = true;
            _LabelParity.Location = new System.Drawing.Point(6, 114);
            _LabelParity.Name = "_LabelParity";
            _LabelParity.Size = new System.Drawing.Size(68, 17);
            _LabelParity.TabIndex = 8;
            _LabelParity.Text = "Паритет:";
            // 
            // _LabelStopBits
            // 
            _LabelStopBits.AutoSize = true;
            _LabelStopBits.Location = new System.Drawing.Point(6, 144);
            _LabelStopBits.Name = "_LabelStopBits";
            _LabelStopBits.Size = new System.Drawing.Size(81, 17);
            _LabelStopBits.TabIndex = 9;
            _LabelStopBits.Text = "Стоп биты:";
            // 
            // FormSerialPortSettings
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(216, 209);
            Controls.Add(_GroupBoxComPort);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormSerialPortSettings";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Настройки COM-порта";
            _GroupBoxComPort.ResumeLayout(false);
            _GroupBoxComPort.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox _ComboBoxSerialPort;
        private System.Windows.Forms.GroupBox _GroupBoxComPort;
        private System.Windows.Forms.Label _LabelDataBits;
        private System.Windows.Forms.Label _LabelSerialPort;
        private System.Windows.Forms.Label _LabelStopBits;
        private System.Windows.Forms.Label _LabelParity;
        private System.Windows.Forms.Label _LabelBaudRate;
        private System.Windows.Forms.ComboBox _ComboBoxStopBits;
        private System.Windows.Forms.ComboBox _ComboBoxParity;
        private System.Windows.Forms.ComboBox _ComboBoxBaudRate;
        private System.Windows.Forms.ComboBox _ComboBoxDataBits;
    }
}