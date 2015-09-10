namespace Modbus.OSIModel.DataLinkLayer.Master.ConnectionUITypeEditor
{
    partial class FormConnectionEditor
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
            this.propertyGridProperties = new System.Windows.Forms.PropertyGrid();
            this.labelConnectionType = new System.Windows.Forms.Label();
            this.comboBoxTypeConnection = new System.Windows.Forms.ComboBox();
            this.groupBoxConnectionType = new System.Windows.Forms.GroupBox();
            this.comboBoxMode = new System.Windows.Forms.ComboBox();
            this.labelMode = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.groupBoxConnectionType.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGridProperties
            // 
            this.propertyGridProperties.Location = new System.Drawing.Point(6, 108);
            this.propertyGridProperties.Name = "propertyGridProperties";
            this.propertyGridProperties.Size = new System.Drawing.Size(270, 296);
            this.propertyGridProperties.TabIndex = 0;
            // 
            // labelConnectionType
            // 
            this.labelConnectionType.AutoSize = true;
            this.labelConnectionType.Location = new System.Drawing.Point(18, 24);
            this.labelConnectionType.Name = "labelConnectionType";
            this.labelConnectionType.Size = new System.Drawing.Size(120, 17);
            this.labelConnectionType.TabIndex = 3;
            this.labelConnectionType.Text = "Тип соединения:";
            // 
            // comboBoxTypeConnection
            // 
            this.comboBoxTypeConnection.FormattingEnabled = true;
            this.comboBoxTypeConnection.Location = new System.Drawing.Point(143, 21);
            this.comboBoxTypeConnection.Name = "comboBoxTypeConnection";
            this.comboBoxTypeConnection.Size = new System.Drawing.Size(121, 24);
            this.comboBoxTypeConnection.TabIndex = 2;
            // 
            // groupBoxConnectionType
            // 
            this.groupBoxConnectionType.Controls.Add(this.comboBoxMode);
            this.groupBoxConnectionType.Controls.Add(this.labelMode);
            this.groupBoxConnectionType.Controls.Add(this.comboBoxTypeConnection);
            this.groupBoxConnectionType.Controls.Add(this.labelConnectionType);
            this.groupBoxConnectionType.Location = new System.Drawing.Point(6, 12);
            this.groupBoxConnectionType.Name = "groupBoxConnectionType";
            this.groupBoxConnectionType.Size = new System.Drawing.Size(270, 90);
            this.groupBoxConnectionType.TabIndex = 4;
            this.groupBoxConnectionType.TabStop = false;
            this.groupBoxConnectionType.Text = "Тип подключения";
            // 
            // comboBoxMode
            // 
            this.comboBoxMode.FormattingEnabled = true;
            this.comboBoxMode.Location = new System.Drawing.Point(143, 51);
            this.comboBoxMode.Name = "comboBoxMode";
            this.comboBoxMode.Size = new System.Drawing.Size(121, 24);
            this.comboBoxMode.TabIndex = 4;
            // 
            // labelMode
            // 
            this.labelMode.AutoSize = true;
            this.labelMode.Location = new System.Drawing.Point(18, 54);
            this.labelMode.Name = "labelMode";
            this.labelMode.Size = new System.Drawing.Size(123, 17);
            this.labelMode.TabIndex = 5;
            this.labelMode.Text = "Режим передачи:";
            // 
            // buttonOk
            // 
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(201, 410);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // FormConnectionEditor
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 438);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBoxConnectionType);
            this.Controls.Add(this.propertyGridProperties);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConnectionEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Редактор";
            this.groupBoxConnectionType.ResumeLayout(false);
            this.groupBoxConnectionType.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGridProperties;
        private System.Windows.Forms.Label labelConnectionType;
        private System.Windows.Forms.ComboBox comboBoxTypeConnection;
        private System.Windows.Forms.GroupBox groupBoxConnectionType;
        private System.Windows.Forms.ComboBox comboBoxMode;
        private System.Windows.Forms.Label labelMode;
        private System.Windows.Forms.Button buttonOk;
    }
}