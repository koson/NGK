namespace NGK.MeasuringDeviceTech.Forms
{
    partial class FormInitDevice
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInitDevice));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.textBoxDeviceCode = new System.Windows.Forms.TextBox();
            this.numericUpDownYear = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownSerialNumber = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSerialNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numericUpDownYear);
            this.groupBox1.Controls.Add(this.numericUpDownSerialNumber);
            this.groupBox1.Controls.Add(this.textBoxDeviceCode);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(263, 60);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Серийный номер устройства";
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(199, 79);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 27);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "ОК";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // textBoxDeviceCode
            // 
            this.textBoxDeviceCode.Location = new System.Drawing.Point(15, 30);
            this.textBoxDeviceCode.Name = "textBoxDeviceCode";
            this.textBoxDeviceCode.ReadOnly = true;
            this.textBoxDeviceCode.Size = new System.Drawing.Size(66, 22);
            this.textBoxDeviceCode.TabIndex = 2;
            this.textBoxDeviceCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numericUpDownYear
            // 
            this.numericUpDownYear.Location = new System.Drawing.Point(87, 30);
            this.numericUpDownYear.Name = "numericUpDownYear";
            this.numericUpDownYear.Size = new System.Drawing.Size(62, 22);
            this.numericUpDownYear.TabIndex = 2;
            // 
            // numericUpDownSerialNumber
            // 
            this.numericUpDownSerialNumber.Location = new System.Drawing.Point(155, 30);
            this.numericUpDownSerialNumber.Name = "numericUpDownSerialNumber";
            this.numericUpDownSerialNumber.Size = new System.Drawing.Size(93, 22);
            this.numericUpDownSerialNumber.TabIndex = 3;
            // 
            // FormInitDevice
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(286, 109);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormInitDevice";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Инициализация устройства";
            this.Load += new System.EventHandler(this.FormInitDevice_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSerialNumber)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numericUpDownYear;
        private System.Windows.Forms.NumericUpDown numericUpDownSerialNumber;
        private System.Windows.Forms.TextBox textBoxDeviceCode;
        private System.Windows.Forms.Button buttonOk;
    }
}