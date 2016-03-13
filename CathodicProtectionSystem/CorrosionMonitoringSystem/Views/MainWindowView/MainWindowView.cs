using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public partial class MainWindowView : Form, IMainWindowView, IButtonsPanel, IStatusPanel
    {
        #region ButtonsPanel
        
        public enum Buttons
        {
            /// <summary>
            /// ����
            /// </summary>
            F2,
            F3,
            F4,
            F5,
            /// <summary>
            /// ������/���������� ������ ������
            /// </summary>
            F6
        }
        
        #endregion

        #region Constructors

        public MainWindowView()
        {
            InitializeComponent();
        }

        #endregion

        #region Fields And Properties

        protected static object SyncRoot = new object();
        Timer _Timer;

        /// <summary>
        /// ����� ��������� � �������
        /// </summary>
        public Int32 TotalDevices 
        { 
            set 
            {
                _ToolStripButtonTotalDevices.Text = 
                    String.Format("����� ���������: {0}", value);
            }
        }

        /// <summary>
        /// ����������� ��������� � �������
        /// </summary>
        public Int32 FaultyDevices
        {
            set
            {
                _ToolStripButtonFaultyDevices.Text =
                    String.Format("����������� ���������: {0}", value);
                if (value > 0)
                {
                    _ToolStripButtonFaultyDevices.BackColor = Color.Red;
                }
                else
                {
                    _ToolStripButtonFaultyDevices.BackColor = Control.DefaultBackColor;
                }
            }
        }

        /// <summary>
        /// ������������� ����� � ������ ���������
        /// </summary>
        public DateTime DateTime
        {
            set
            {
                throw new NotImplementedException();
                //this._StatusStripSystemInfo.DateTime = value;
            }
        }

        public Boolean ButtonsPanelCollapsed
        {
            get { return _PanelSystemButtonsRegion.Visible; }
            set { _PanelSystemButtonsRegion.Visible = value; }
        }

        public Boolean ButtonF3IsAccessible
        {
            get { return _ButtonF3.Visible && _ButtonF3.Enabled; }
            set 
            { 
                _ButtonF3.Visible = value; 
                _ButtonF3.Enabled = value; 
            }
        }

        public Boolean ButtonF4IsAccessible
        {
            get { return _ButtonF4.Visible && _ButtonF4.Enabled; }
            set
            {
                _ButtonF4.Visible = value;
                _ButtonF4.Enabled = value;
            }
        }

        public Boolean ButtonF5IsAccessible
        {
            get { return _ButtonF5.Visible && _ButtonF5.Enabled; }
            set
            {
                _ButtonF5.Visible = value;
                _ButtonF5.Enabled = value;
            }
        }

        public String ButtonF3Text
        {
            set { _ButtonF3.Text = value; } 
        }

        public String ButtonF4Text
        {
            set { _ButtonF4.Text = value; }
        }

        public String ButtonF5Text
        {
            set { _ButtonF5.Text = value; }
        }

        public ViewType ViewType { get { return ViewType.Window; } }

        #endregion

        #region Event Handlers

        void TemplateView_Load(object sender, EventArgs e)
        {
            _Timer = new Timer();
            _Timer.Interval = 1000;
            _Timer.Tick += new EventHandler(EventHandler_Timer_Tick);
            _Timer.Start();

            _ButtonF2.Click += new EventHandler(EventHandler_Button_Click);
            _ButtonF3.Click += new EventHandler(EventHandler_Button_Click);
            _ButtonF4.Click += new EventHandler(EventHandler_Button_Click);
            _ButtonF5.Click += new EventHandler(EventHandler_Button_Click);
            _ButtonF6.Click += new EventHandler(EventHandler_Button_Click);
        }

        void EventHandler_Timer_Tick(object sender, EventArgs e)
        {
            _ToolStripStatusLabelDateTime.Text = 
                DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-RU", false));
        }

        void EventHandler_Button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.Equals(_ButtonF2))
            {
                //OnButtonClick(new ButtonClickEventArgs(Buttons.F2));
            }
            else if (btn.Equals(_ButtonF3))
            {
                //OnButtonClick(new ButtonClickEventArgs(Buttons.F3));
            }
            else if (btn.Equals(_ButtonF4))
            {
                //OnButtonClick(new ButtonClickEventArgs(Buttons.F4));
            }
            else if (btn.Equals(_ButtonF5))
            {
                //OnButtonClick(new ButtonClickEventArgs(Buttons.F5));
            }
            else if (btn.Equals(_ButtonF6))
            {
                // �������� ��� ���������� ������ �������
                _PanelSystemButtonsRegion.Visible = !_PanelSystemButtonsRegion.Visible;
            }
        }

        void EventHandler_TemplateView_Resize(object sender, EventArgs e)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// ����������� ��������� ���������� �������� �����
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns>true ���� ������ " a " Windows Presentation Foundation (WPF) ��������� ������; 
        /// � ��������� ������, false. ��. ����� 
        /// http://msdn.microsoft.com/ru-ru/library/system.windows.forms.integration.elementhost.processcmdkey(v=vs.110).aspx
        /// </returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x0100;

            if (msg.Msg == WM_KEYDOWN)
            {
                switch (keyData)
                {
//                    case Keys.F1:
//                        {
//#if DEBUG
//                            // ���������� ���� ���������� ������
//                            FormNetworkControl frm = FormNetworkControl.Instance;
//                            frm.TopMost = true;
//                            frm.Show();
//                            return false;
//#else
//                            return true;
//#endif

//                        }
                    case Keys.F2:
                        {
                            // �� ������� F2 ��������� ����������� ���� �� ������ F2
                            _ButtonF2.PerformClick();
                            return false;
                        }
                    case Keys.F3:
                        {
                            _ButtonF3.PerformClick();
                            return false;
                        }
                    case Keys.F4:
                        {
                            _ButtonF4.PerformClick();
                            return false;
                        }
                    case Keys.F5:
                        {
                            _ButtonF5.PerformClick();
                            return false;
                        }
                    case Keys.F6:
                        {
                            if (_ButtonF6.Visible)
                            {
                                _ButtonF6.PerformClick();
                            }
                            else
                            {
                                // �������� ��� ���������� ������ �������
                                _PanelSystemButtonsRegion.Visible = !_PanelSystemButtonsRegion.Visible;
                            }
                            return false;
                        }
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Event Generators

        void OnButtonClick(ButtonClickEventArgs args)
        {
            if (ButtonClick != null)
            {
                ButtonClick(this, args);
            }
        }

        #endregion

        #region Events

        public event EventHandler<ButtonClickEventArgs> ButtonClick;
        
        #endregion

        #region IMainWindowView Members

        public string Title
        {
            get
            {
                return Text;
            }
            set
            {
                Text = value;
                _LabelTilte.Text = Text;
            }
        }

        public UserControl CurrentControl
        {
            get
            {
                return _PanelWorkingRegion.Controls.Count > 0 ?
                    (UserControl)_PanelWorkingRegion.Controls[0] : null;
            }
            set
            {
                foreach (Control control in _PanelWorkingRegion.Controls)
                {
                    control.Dispose();
                }
                _PanelWorkingRegion.Controls.Clear();

                value.Dock = DockStyle.Fill;
                _PanelWorkingRegion.Controls.Add(value);
            }
        }

        #endregion
    }
}