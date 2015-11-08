using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NGK.CAN.ApplicationLayer.Network.Master;
using NGK.CorrosionMonitoringSystem.Forms.Controls;
using NGK.CorrosionMonitoringSystem.Core;

namespace NGK.CorrosionMonitoringSystem.Forms
{
    /// <summary>
    /// ����� ��� �������� �������� ���� ������� �����������
    /// </summary>
    public partial class CorrosionMonitoringSystemForm : Form
    {
        public struct TabPageNames
        {
            public static string TabPageDevicesList = "TabPageDevicesList";
            public static string TabPagePivotTable = "TabPagePivotTable"; 
        }

        #region Fields And Properties
        /// <summary>
        /// ����� ��������� � �������
        /// </summary>
        public Int32 TotalDevices
        {
            set 
            {
                this._StatusStripSystemInfo.TotalDevices = value;
            }
        }
        /// <summary>
        /// ����������� ��������� � �������
        /// </summary>
        public Int32 FaultyDevices
        {
            set
            {
                this._StatusStripSystemInfo.FaultyDevices = value;
            }
        }
        /// <summary>
        /// ������������� ����� � ������ ���������
        /// </summary>
        public DateTime DateTime
        {
            set { this._StatusStripSystemInfo.DateTime = value; }
        }
        /// <summary>
        /// ������ ������
        /// </summary>
        private ButtonsPanel _ButtonsPanel;
        /// <summary>
        /// ��������� ������ �������� ����
        /// </summary>
        private SystemStatusBar _StatusStripSystemInfo;
        /// <summary>
        /// �������� ���������. ������ ������ - ��� ������,
        /// ����� �������� - _TabControlMainFrame
        /// </summary>
        private SplitContainer _SplitContainerMainFrame;       
        /// <summary>
        /// 
        /// </summary>
        //private SplitContainer _SplitContainerWorkFrame;
        /// <summary>
        /// �������� ��� ������������� ��������� ���������� � �������
        /// </summary>
        private TabControl _TabControlMainFrame;
        /// <summary>
        /// ���������� ������� �������������� �������� ��� ��������� ����� ����������� ����������.
        /// </summary>
        public TabControl TabControlViews
        {
            get { return _TabControlMainFrame; }
        }
        /// <summary>
        /// �������� ��� _TabControlMainFrame. ����� ������������ ���������
        /// ���� � ��������� � ����.
        /// </summary>
        private TabPage _TabPageDevicesList;
        
        /// <summary>
        /// �������� ��� _TabControlMainFrame. ����� ������������ �������
        /// ������� ���������� ���� ��������� ��� �������.
        /// </summary>
        private TabPage _TabPagePivotTable;  
        /// <summary>
        /// ���� ��� ����������� ������ ������� ���������
        /// </summary>
        private DataGridView _DataGridViewDevicesList;
        /// <summary>
        /// ���� ��� ����������� ������ ������� ���������
        /// </summary>
        public DataGridView DataGridViewDevicesList
        {
            get { return this._DataGridViewDevicesList; }
        }
        /// <summary>
        /// ���� ��� ����������� ������� ������� ���������� �� ���� ��������� ����.
        /// </summary>
        private DataGridView _DataGridViewParametersPivotTable;
        /// <summary>
        /// ���� ��� ����������� ������� ������� ���������� �� ���� ��������� ����.
        /// </summary>
        public DataGridView DataGridViewPivotTable
        {
            get { return this._DataGridViewParametersPivotTable; }
        }
        public Boolean ButtonsPanelCollapsed
        {
            get { return this._SplitContainerMainFrame.Panel2Collapsed; }
            set { this._SplitContainerMainFrame.Panel2Collapsed = value; }
        }
        /// <summary>
        /// ������ ��������� �������. ���� true - ������ ��������, ���� 
        /// false - �����
        /// </summary>
        private Boolean _CursorState;
        
        /// <summary>
        /// ���������� ������������� ��������� ������� (����� - false 
        /// ��� �������� - true)
        /// </summary>
        public Boolean CursorState
        {
            get { return _CursorState; }
            set 
            {
                if (value)
                {
                    Cursor.Show();
                    this._CursorState = value;
                }
                else
                {
                    Cursor.Hide();
                    this._CursorState = value;
                }
            }
        }
        #endregion

        #region Constructors
        
        /// <summary>
        /// ����������� �� ���������
        /// </summary>
        public CorrosionMonitoringSystemForm()
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Normal;
            FormBorderStyle = FormBorderStyle.Sizable;

            Load += new EventHandler(EventHandler_FormMain_Load);
            Shown += new EventHandler(EventHandler_FormMain_Shown);
        }

        #endregion

        #region EventHadlers For FormMain
        /// <summary>
        /// ���������� ������� �������� �����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_FormMain_Load(object sender, EventArgs e)
        {
            // �������������� �����
            this.CursorState = Settings.CursorEnable;
            this.ShowInTaskbar = Settings.ShowInTaskbar;
            if (Settings.FormBorderEnable == false)
            { 
                this.FormBorderStyle = FormBorderStyle.None;
            }
            this.Icon = Properties.Resources.faviconMy;
            this.Text = "������� ������������� �����������";

            // ����� �������������� ��������� ����� � ����������� �� �����������
            // ������
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowOnly;
            
            // �������� ���������: ������ - �������� ������� ����, ����� - ������ ������
            this._SplitContainerMainFrame = new SplitContainer();
            this._SplitContainerMainFrame.Name = "SplitContainerMainFrame";
            this._SplitContainerMainFrame.Dock = DockStyle.Fill;
            this._SplitContainerMainFrame.Orientation = Orientation.Vertical;

            this._SplitContainerMainFrame.Panel2MinSize = this._SplitContainerMainFrame.Width / 10;
            this._SplitContainerMainFrame.SplitterDistance = this._SplitContainerMainFrame.Width / 10 * 9;
            this._SplitContainerMainFrame.SplitterMoved += 
                new SplitterEventHandler(EventHandler_SplitContainerMainFrame_SplitterMoved);
            this._SplitContainerMainFrame.TabStop = true;
            
            // ������ ������
            this._ButtonsPanel = 
                new ButtonsPanel(this._SplitContainerMainFrame.Panel2);
            this._ButtonsPanel.ButtonOne.Click += new EventHandler(EventHandler_SystemButton_Click);
            this._ButtonsPanel.ButtonTwo.Click += new EventHandler(EventHandler_SystemButton_Click);
            this._ButtonsPanel.ButtonThree.Click += new EventHandler(EventHandler_SystemButton_Click);
            this._ButtonsPanel.ButtonFour.Click += new EventHandler(EventHandler_SystemButton_Click);
            this._ButtonsPanel.ButtonFive.Click += new EventHandler(EventHandler_SystemButton_Click);

            // �������� ��� ���������� � ������ ������ ��������� ���������� _SplitContainerMainFrame
            this._TabControlMainFrame = new TabControl();
            this._TabControlMainFrame.Name = "TabControlMainFrame";
            this._TabControlMainFrame.Dock = DockStyle.Fill;
            this._TabControlMainFrame.Selecting += 
                new TabControlCancelEventHandler(EventHandler_TabControlMainFrame_Selecting);

            this._SplitContainerMainFrame.Panel1.Controls.Add(this._TabControlMainFrame);

            // ��������� �������� ��� _TabControlMainFrame
            this._TabPageDevicesList = new TabPage();
            this._TabPageDevicesList.Name = TabPageNames.TabPageDevicesList;
            this._TabPageDevicesList.Text = "�������";
            this._TabControlMainFrame.TabPages.Add(this._TabPageDevicesList);

            this._DataGridViewDevicesList = new DataGridView();
            this._DataGridViewDevicesList.Name = "ListOfDevices";
            this._DataGridViewDevicesList.Dock = DockStyle.Fill;
            this._TabPageDevicesList.Controls.Add(this._DataGridViewDevicesList);

            this._TabPagePivotTable = new TabPage();
            this._TabPagePivotTable.Name = TabPageNames.TabPagePivotTable;
            this._TabPagePivotTable.Text = "������� �������";
            this._TabControlMainFrame.TabPages.Add(this._TabPagePivotTable);

            // ���� ��� ����������� ������� ������� ����������
            this._DataGridViewParametersPivotTable = new DataGridView();
            this._DataGridViewParametersPivotTable.Name = "DataGridViewParametersPivotTable";
            this._DataGridViewParametersPivotTable.Dock = DockStyle.Fill;
            this._TabPagePivotTable.Controls.Add(this._DataGridViewParametersPivotTable);

            // ����������� ������ ��������� ����
            this._StatusStripSystemInfo = new SystemStatusBar();
            this._StatusStripSystemInfo.Name = "StatusStripSystemInfo";
            this._SplitContainerMainFrame.Panel1.Controls.Add(this._StatusStripSystemInfo);

            this.SuspendLayout();
            this.Controls.AddRange(new Control[] { this._SplitContainerMainFrame });
            this.ResumeLayout(false);
            this.PerformLayout();
            return;
        }
        /// <summary>
        /// ���������� ������ �� ��������� ������� �� ������ ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_SystemButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            foreach (string name in ButtonsPanel.ButtonNames.ToArray())
            {
                if (name.Equals(btn.Name))
                {
                    this.OnSystemButtonClick(btn.Name);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_TabControlMainFrame_Selecting(
            object sender, TabControlCancelEventArgs e)
        {
            
            return;
        }
        /// <summary>
        /// ���������� ������� ����������� ��������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_SplitContainerMainFrame_SplitterMoved(object sender, SplitterEventArgs e)
        {
            SplitContainer container = (SplitContainer)sender;
            //container.Panel2;
            return;
        }

        /// <summary>
        /// ���������� ������� ����������� �����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_FormMain_Shown(object sender, EventArgs e)
        {
            this.OnLoaded();
            return;
        }
        #endregion

        #region Methods
        /// <summary>
        /// ��������� ��������� ������ �� ������ ������� �� � ������������
        /// </summary>
        /// <param name="button"></param>
        /// <param name="text"></param>
        public Button GetSystemButton(string buttonName)
        {
            return _ButtonsPanel.GetButton(buttonName);
        }
        /// <summary>
        /// ���������� ������� Loaded
        /// </summary>
        private void OnLoaded()
        {
            if (this.Loaded != null)
            {
                this.Loaded(this, new EventArgs());
            }
        }
        /// <summary>
        /// ���������� ������� ������� ��������� ������ �� ������ ������
        /// </summary>
        /// <param name="button"></param>
        private void OnSystemButtonClick(String buttonName)
        {
            if (this.SystemButtonClick != null)
            {
                this.SystemButtonClick(this, new SystemButtonClickEventArgs(buttonName));
            }
        }
        /// <summary>
        /// ����������� ��������� ���������� �������� �����
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns>true ���� ������ " a " Windows Presentation Foundation (WPF) ��������� ������; 
        /// � ��������� ������, false. ��. ����� 
        /// http://msdn.microsoft.com/ru-ru/library/system.windows.forms.integration.elementhost.processcmdkey(v=vs.110).aspx
        /// </returns>
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x0100;
            
            if (msg.Msg == WM_KEYDOWN)
            {
                switch (keyData)
                {
                    case Keys.F1:
                        {
#if DEBUG
                            // ���������� ���� ���������� ������
                            FormNetworkControl frm = FormNetworkControl.Instance;
                            frm.TopMost = true;
                            frm.Show();
                            return false;
#else
                            return true;
#endif
                          
                        }
                    case Keys.F2:
                        {
                            // �� ������� F2 ��������� ����������� ���� �� ������ F2
                            this._ButtonsPanel.ButtonOne.PerformClick();
                            return false;
                        }
                    case Keys.F3:
                        {
                            this._ButtonsPanel.ButtonTwo.PerformClick();
                            return false;
                        }
                    case Keys.F4:
                        {
                            this._ButtonsPanel.ButtonThree.PerformClick();
                            return false;
                        }
                    case Keys.F5:
                        {
                            this._ButtonsPanel.ButtonFour.PerformClick();
                            return false;
                        }
                    case Keys.F6:
                        {
                            //this._ButtonsPanel.ButtonFive.PerformClick();
                            // �������� ��� ���������� ������ �������
                            if (this._SplitContainerMainFrame.Panel2Collapsed)
                            {
                                this._SplitContainerMainFrame.Panel2Collapsed = false;
                            }
                            else
                            {
                                this._SplitContainerMainFrame.Panel2Collapsed = true;
                            }
                            return false;
                        }
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region Events
        /// <summary>
        /// ������� ���������� ����� ������� Load �����
        /// </summary>
        public event EventHandler Loaded;
        /// <summary>
        /// ������� ����� �� ��������� ������ �� ������ ������
        /// </summary>
        public event SystemButtonClickEventHandler SystemButtonClick;
        #endregion

    }// End Of Class
}//End Of NameSpace