namespace NGK.CorrosionMonitoringSystem.Views
{
    partial class NavigationMenuView
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
            this._GroupBoxMenu = new System.Windows.Forms.GroupBox();
            this._ButtonLogViewer = new System.Windows.Forms.Button();
            this._ButtonDeviceDetail = new System.Windows.Forms.Button();
            this._ButtonDeviceList = new System.Windows.Forms.Button();
            this._ButtonPivotTable = new System.Windows.Forms.Button();
            this._ButtonExit = new System.Windows.Forms.Button();
            this._GroupBoxMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // _GroupBoxMenu
            // 
            this._GroupBoxMenu.Controls.Add(this._ButtonLogViewer);
            this._GroupBoxMenu.Controls.Add(this._ButtonDeviceDetail);
            this._GroupBoxMenu.Controls.Add(this._ButtonDeviceList);
            this._GroupBoxMenu.Controls.Add(this._ButtonPivotTable);
            this._GroupBoxMenu.Controls.Add(this._ButtonExit);
            this._GroupBoxMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this._GroupBoxMenu.Location = new System.Drawing.Point(5, 5);
            this._GroupBoxMenu.Margin = new System.Windows.Forms.Padding(5);
            this._GroupBoxMenu.Name = "_GroupBoxMenu";
            this._GroupBoxMenu.Padding = new System.Windows.Forms.Padding(5);
            this._GroupBoxMenu.Size = new System.Drawing.Size(212, 193);
            this._GroupBoxMenu.TabIndex = 0;
            this._GroupBoxMenu.TabStop = false;
            // 
            // _ButtonLogViewer
            // 
            this._ButtonLogViewer.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._ButtonLogViewer.Location = new System.Drawing.Point(8, 155);
            this._ButtonLogViewer.Name = "_ButtonLogViewer";
            this._ButtonLogViewer.Size = new System.Drawing.Size(195, 29);
            this._ButtonLogViewer.TabIndex = 4;
            this._ButtonLogViewer.Text = "Журнал событий";
            this._ButtonLogViewer.UseVisualStyleBackColor = true;
            // 
            // _ButtonDeviceDetail
            // 
            this._ButtonDeviceDetail.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._ButtonDeviceDetail.Location = new System.Drawing.Point(8, 120);
            this._ButtonDeviceDetail.Name = "_ButtonDeviceDetail";
            this._ButtonDeviceDetail.Size = new System.Drawing.Size(195, 29);
            this._ButtonDeviceDetail.TabIndex = 3;
            this._ButtonDeviceDetail.Text = "Информация об устройтве";
            this._ButtonDeviceDetail.UseVisualStyleBackColor = true;
            // 
            // _ButtonDeviceList
            // 
            this._ButtonDeviceList.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._ButtonDeviceList.Location = new System.Drawing.Point(8, 85);
            this._ButtonDeviceList.Name = "_ButtonDeviceList";
            this._ButtonDeviceList.Size = new System.Drawing.Size(195, 29);
            this._ButtonDeviceList.TabIndex = 2;
            this._ButtonDeviceList.Text = "Устройства";
            this._ButtonDeviceList.UseVisualStyleBackColor = true;
            // 
            // _ButtonPivotTable
            // 
            this._ButtonPivotTable.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._ButtonPivotTable.Location = new System.Drawing.Point(8, 50);
            this._ButtonPivotTable.Name = "_ButtonPivotTable";
            this._ButtonPivotTable.Size = new System.Drawing.Size(195, 29);
            this._ButtonPivotTable.TabIndex = 1;
            this._ButtonPivotTable.Text = "Мониторинг";
            this._ButtonPivotTable.UseVisualStyleBackColor = true;
            // 
            // _ButtonExit
            // 
            this._ButtonExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._ButtonExit.Location = new System.Drawing.Point(8, 15);
            this._ButtonExit.Name = "_ButtonExit";
            this._ButtonExit.Size = new System.Drawing.Size(195, 29);
            this._ButtonExit.TabIndex = 0;
            this._ButtonExit.Text = "Закрыть меню";
            this._ButtonExit.UseVisualStyleBackColor = true;
            // 
            // NavigationMenuView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(222, 203);
            this.Controls.Add(this._GroupBoxMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NavigationMenuView";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Меню";
            this.Load += new System.EventHandler(this.EventHandler_NavigationMenuForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EventHandler_NavigationMenuView_FormClosed);
            this._GroupBoxMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox _GroupBoxMenu;
        private System.Windows.Forms.Button _ButtonLogViewer;
        private System.Windows.Forms.Button _ButtonDeviceDetail;
        private System.Windows.Forms.Button _ButtonDeviceList;
        private System.Windows.Forms.Button _ButtonPivotTable;
        private System.Windows.Forms.Button _ButtonExit;
    }
}