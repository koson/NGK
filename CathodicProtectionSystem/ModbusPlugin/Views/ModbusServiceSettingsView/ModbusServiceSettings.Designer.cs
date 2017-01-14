namespace NGK.Plugins.Views
{
    partial class ModbusServiceSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._PropertyGridModbusServiceSettings = new System.Windows.Forms.PropertyGrid();
            this._TableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _TableLayoutPanel
            // 
            this._TableLayoutPanel.ColumnCount = 1;
            this._TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._TableLayoutPanel.Controls.Add(this._PropertyGridModbusServiceSettings, 0, 0);
            this._TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._TableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._TableLayoutPanel.Name = "_TableLayoutPanel";
            this._TableLayoutPanel.RowCount = 2;
            this._TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 84.83412F));
            this._TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15.16588F));
            this._TableLayoutPanel.Size = new System.Drawing.Size(508, 422);
            this._TableLayoutPanel.TabIndex = 0;
            // 
            // _PropertyGridModbusServiceSettings
            // 
            this._PropertyGridModbusServiceSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this._PropertyGridModbusServiceSettings.Location = new System.Drawing.Point(3, 3);
            this._PropertyGridModbusServiceSettings.Name = "_PropertyGridModbusServiceSettings";
            this._PropertyGridModbusServiceSettings.Size = new System.Drawing.Size(502, 351);
            this._PropertyGridModbusServiceSettings.TabIndex = 0;
            // 
            // ModbusServiceSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._TableLayoutPanel);
            this.Name = "ModbusServiceSettings";
            this.Size = new System.Drawing.Size(508, 422);
            this._TableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _TableLayoutPanel;
        private System.Windows.Forms.PropertyGrid _PropertyGridModbusServiceSettings;
    }
}
