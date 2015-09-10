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
            this._ComboBoxSerialPort = new System.Windows.Forms.ComboBox();
            this._GroupBoxComPort = new System.Windows.Forms.GroupBox();
            this._LabelDataBits = new System.Windows.Forms.Label();
            this._LabelSerialPort = new System.Windows.Forms.Label();
            this._ComboBoxDataBits = new System.Windows.Forms.ComboBox();
            this._ComboBoxBaudRate = new System.Windows.Forms.ComboBox();
            this._ComboBoxParity = new System.Windows.Forms.ComboBox();
            this._ComboBoxStopBits = new System.Windows.Forms.ComboBox();
            this._LabelBaudRate = new System.Windows.Forms.Label();
            this._LabelParity = new System.Windows.Forms.Label();
            this._LabelStopBits = new System.Windows.Forms.Label();
            this._GroupBoxComPort.SuspendLayout();
            this.SuspendLayout();
            // 
            // _ComboBoxSerialPort
            // 
            this._ComboBoxSerialPort.FormattingEnabled = true;
            this._ComboBoxSerialPort.Location = new System.Drawing.Point(93, 21);
            this._ComboBoxSerialPort.Name = "_ComboBoxSerialPort";
            this._ComboBoxSerialPort.Size = new System.Drawing.Size(88, 24);
            this._ComboBoxSerialPort.TabIndex = 0;
            // 
            // _GroupBoxComPort
            // 
            this._GroupBoxComPort.Controls.Add(this._LabelStopBits);
            this._GroupBoxComPort.Controls.Add(this._LabelParity);
            this._GroupBoxComPort.Controls.Add(this._LabelBaudRate);
            this._GroupBoxComPort.Controls.Add(this._ComboBoxStopBits);
            this._GroupBoxComPort.Controls.Add(this._ComboBoxParity);
            this._GroupBoxComPort.Controls.Add(this._ComboBoxBaudRate);
            this._GroupBoxComPort.Controls.Add(this._ComboBoxDataBits);
            this._GroupBoxComPort.Controls.Add(this._LabelSerialPort);
            this._GroupBoxComPort.Controls.Add(this._LabelDataBits);
            this._GroupBoxComPort.Controls.Add(this._ComboBoxSerialPort);
            this._GroupBoxComPort.Location = new System.Drawing.Point(12, 12);
            this._GroupBoxComPort.Name = "_GroupBoxComPort";
            this._GroupBoxComPort.Size = new System.Drawing.Size(191, 185);
            this._GroupBoxComPort.TabIndex = 1;
            this._GroupBoxComPort.TabStop = false;
            this._GroupBoxComPort.Text = "COM-порт";
            // 
            // _LabelDataBits
            // 
            this._LabelDataBits.AutoSize = true;
            this._LabelDataBits.Location = new System.Drawing.Point(6, 54);
            this._LabelDataBits.Name = "_LabelDataBits";
            this._LabelDataBits.Size = new System.Drawing.Size(65, 17);
            this._LabelDataBits.TabIndex = 1;
            this._LabelDataBits.Text = "Данные:";
            // 
            // _LabelSerialPort
            // 
            this._LabelSerialPort.AutoSize = true;
            this._LabelSerialPort.Location = new System.Drawing.Point(6, 24);
            this._LabelSerialPort.Name = "_LabelSerialPort";
            this._LabelSerialPort.Size = new System.Drawing.Size(45, 17);
            this._LabelSerialPort.TabIndex = 2;
            this._LabelSerialPort.Text = "Порт:";
            // 
            // _ComboBoxDataBits
            // 
            this._ComboBoxDataBits.FormattingEnabled = true;
            this._ComboBoxDataBits.Location = new System.Drawing.Point(93, 51);
            this._ComboBoxDataBits.Name = "_ComboBoxDataBits";
            this._ComboBoxDataBits.Size = new System.Drawing.Size(88, 24);
            this._ComboBoxDataBits.TabIndex = 3;
            // 
            // _ComboBoxBaudRate
            // 
            this._ComboBoxBaudRate.FormattingEnabled = true;
            this._ComboBoxBaudRate.Location = new System.Drawing.Point(93, 81);
            this._ComboBoxBaudRate.Name = "_ComboBoxBaudRate";
            this._ComboBoxBaudRate.Size = new System.Drawing.Size(88, 24);
            this._ComboBoxBaudRate.TabIndex = 4;
            // 
            // _ComboBoxParity
            // 
            this._ComboBoxParity.FormattingEnabled = true;
            this._ComboBoxParity.Location = new System.Drawing.Point(93, 111);
            this._ComboBoxParity.Name = "_ComboBoxParity";
            this._ComboBoxParity.Size = new System.Drawing.Size(88, 24);
            this._ComboBoxParity.TabIndex = 5;
            // 
            // _ComboBoxStopBits
            // 
            this._ComboBoxStopBits.FormattingEnabled = true;
            this._ComboBoxStopBits.Location = new System.Drawing.Point(93, 141);
            this._ComboBoxStopBits.Name = "_ComboBoxStopBits";
            this._ComboBoxStopBits.Size = new System.Drawing.Size(88, 24);
            this._ComboBoxStopBits.TabIndex = 6;
            // 
            // _LabelBaudRate
            // 
            this._LabelBaudRate.AutoSize = true;
            this._LabelBaudRate.Location = new System.Drawing.Point(6, 84);
            this._LabelBaudRate.Name = "_LabelBaudRate";
            this._LabelBaudRate.Size = new System.Drawing.Size(73, 17);
            this._LabelBaudRate.TabIndex = 7;
            this._LabelBaudRate.Text = "Скорость:";
            // 
            // _LabelParity
            // 
            this._LabelParity.AutoSize = true;
            this._LabelParity.Location = new System.Drawing.Point(6, 114);
            this._LabelParity.Name = "_LabelParity";
            this._LabelParity.Size = new System.Drawing.Size(68, 17);
            this._LabelParity.TabIndex = 8;
            this._LabelParity.Text = "Паритет:";
            // 
            // _LabelStopBits
            // 
            this._LabelStopBits.AutoSize = true;
            this._LabelStopBits.Location = new System.Drawing.Point(6, 144);
            this._LabelStopBits.Name = "_LabelStopBits";
            this._LabelStopBits.Size = new System.Drawing.Size(81, 17);
            this._LabelStopBits.TabIndex = 9;
            this._LabelStopBits.Text = "Стоп биты:";
            // 
            // FormSerialPortSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(216, 209);
            this.Controls.Add(this._GroupBoxComPort);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSerialPortSettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройки COM-порта";
            this._GroupBoxComPort.ResumeLayout(false);
            this._GroupBoxComPort.PerformLayout();
            this.ResumeLayout(false);

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