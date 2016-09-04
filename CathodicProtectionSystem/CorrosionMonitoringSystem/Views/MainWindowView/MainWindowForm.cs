using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mvp.View;
using Mvp.View.Collections.ObjectModel;
using Mvp.Input;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public partial class MainWindowForm : Form
    {
        #region Constructors

        public MainWindowForm()
        {
            InitializeComponent();
            _PanelSystemButtonsRegion.Hide();
            _ButtonF3.Dispose();
            _ButtonF3 = null;
            _ButtonF4.Dispose();
            _ButtonF4 = null;
            _ButtonF5.Dispose();
            _ButtonF5 = null;
        }

        #endregion

        #region Fields And Properties

        protected static object SyncRoot = new object();
        Timer _Timer;

        /// <summary>
        /// Устанавливает время в строке состояния
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

        public ViewType ViewType { get { return ViewType.Window; } }

        public ICommand ShowMenuCommand
        {
            set 
            { 
                _ButtonF2.DataBindings.Clear();
                _ButtonF2.Tag = value;
                _ButtonF2.DataBindings.Add(
                    new Binding("Enabled", _ButtonF2.Tag, "Status")); 
            }
        }

        public bool FormBorderEnable 
        { 
            get { return FormBorderStyle != FormBorderStyle.None; }
            set {  FormBorderStyle = value ? FormBorderStyle.Sizable : FormBorderStyle.None; }
        }

        public bool FullScreen 
        {
            get { return WindowState == FormWindowState.Maximized; }
            set { WindowState = value ? FormWindowState.Maximized : FormWindowState.Normal; }
        }

        /// <summary>
        /// Хранит состояние курсора. Если true - курсор отображён, если 
        /// false - скрыт
        /// </summary>
        private Boolean _CursorState;

        /// <summary>
        /// Возвращает устанавливает состояние курсора (скрыт - false 
        /// или отображён - true)
        /// </summary>
        public Boolean CursorEnabled
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

        public Panel WorkingRegionControl
        {
            get { return _PanelWorkingRegion; }
        }

        public string Title
        {
            get { return _LabelTilte.Text; }
            set { _LabelTilte.Text = value; }
        }

        #endregion

        #region Event Handlers

        void EventHandler_MainWindowView_Load(object sender, EventArgs e)
        {
            _Timer = new Timer();
            _Timer.Interval = 1000;
            _Timer.Tick += new EventHandler(EventHandler_Timer_Tick);
            _Timer.Start();

            _ButtonF2.Click += new EventHandler(EventHandler_Button_Click);
            //_ButtonF3.Click += new EventHandler(EventHandler_Button_Click);
            //_ButtonF4.Click += new EventHandler(EventHandler_Button_Click);
            //_ButtonF5.Click += new EventHandler(EventHandler_Button_Click);
            _ButtonF6.Click += new EventHandler(EventHandler_Button_Click);

            _PanelSystemButtonsRegion.VisibleChanged +=
                new EventHandler(EventHandler_PanelSystemButtonsRegion_VisibleChanged);
            _PanelWorkingRegion.GotFocus += 
                new EventHandler(EventHandler_PanelWorkingRegion_GotFocus);
        }

        void EventHandler_Timer_Tick(object sender, EventArgs e)
        {
            _ToolStripStatusLabelDateTime.Text = 
                DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-RU", false));
        }

        void EventHandler_Button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.Equals(_ButtonF6))
            {
                // Скрывает или отображаем панель конопок
                _PanelSystemButtonsRegion.Visible = !_PanelSystemButtonsRegion.Visible;
            }
            else
                ((ICommand)btn.Tag).Execute();
        }

        void EventHandler_ShowMenuCommand_CanExecuteChanged(object sender, EventArgs e)
        {
            ICommand cmd = (ICommand)sender;
            _ButtonF2.Enabled = cmd.Status;
        }

        void EventHandler_PanelSystemButtonsRegion_VisibleChanged(object sender, EventArgs e)
        {
            Panel control = (Panel)sender;

            // Если панель скрывается, то переводим фокус ввода на рабочий регион
            if (!control.Visible)
            {
                bool result = _PanelWorkingRegion.Focus();
                result = _PanelWorkingRegion.Focused;
            }
            else
            {
                if (!_ButtonF6.Focused)
                    _ButtonF6.Focus();
            }
        }

        void EventHandler_PanelWorkingRegion_GotFocus(object sender, EventArgs e)
        {
            Panel control = (Panel)sender;

            if (control.Controls.Count > 0)
            {
                control.Controls[0].Focus();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Перехватчик сообщений посылаемых системой форме
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns>true если символ " a " Windows Presentation Foundation (WPF) сочетание клавиш; 
        /// в противном случае, false. См. здесь 
        /// http://msdn.microsoft.com/ru-ru/library/system.windows.forms.integration.elementhost.processcmdkey(v=vs.110).aspx
        /// </returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x0100;

            if (msg.Msg == WM_KEYDOWN)
            {
                switch (keyData)
                {
                    //case Keys.F1:
                    case Keys.F2:
                    case Keys.F3:
                    case Keys.F4:
                    case Keys.F5:
                    case Keys.F6:
                        {
                            OnFunctionalButtonClick(keyData);
                            return false;
                        }                        
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void OnFunctionalButtonClick(Keys button)
        {
            if (FunctionalButtonClick != null)
                FunctionalButtonClick(this, new EventArgsFunctionalButtonClick(button));
        }

        #endregion

        #region Events

        public event EventHandler<EventArgsFunctionalButtonClick> FunctionalButtonClick; 

        #endregion
    }
}