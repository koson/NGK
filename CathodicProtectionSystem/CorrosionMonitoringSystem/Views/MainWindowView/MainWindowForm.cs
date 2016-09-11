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
using Infrastructure.Api.Controls;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public partial class MainWindowForm : Form
    {
        #region Constructors

        public MainWindowForm()
        {
            InitializeComponent();
            _PanelFunctionalButtonsPanel.Hide();
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
            get { return _PanelFunctionalButtonsPanel.Visible; }
            set { _PanelFunctionalButtonsPanel.Visible = value; }
        }

        public ViewType ViewType { get { return ViewType.Window; } }

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

        public Panel WorkingRegionPanel
        {
            get { return _PanelWorkingRegion; }
        }

        public Panel FunctionalButtonsPanel
        {
            get { return _PanelFunctionalButtonsPanel; }
        }

        public string Title
        {
            get { return _LabelTilte.Text; }
            set { _LabelTilte.Text = value; }
        }

        #endregion

        #region Event Handlers

        private void EventHandler_MainWindowView_Load(
            object sender, EventArgs e)
        {
            _Timer = new Timer();
            _Timer.Interval = 1000;
            _Timer.Tick += new EventHandler(EventHandler_Timer_Tick);
            _Timer.Start();

            _PanelFunctionalButtonsPanel.VisibleChanged +=
                new EventHandler(EventHandler_PanelFunctionalButtonsPanel_VisibleChanged);
            _PanelWorkingRegion.GotFocus += 
                new EventHandler(EventHandler_PanelWorkingRegion_GotFocus);
        }

        private void EventHandler_Timer_Tick(
            object sender, EventArgs e)
        {
            _ToolStripStatusLabelDateTime.Text = 
                DateTime.Now.ToString(new System.Globalization.CultureInfo("ru-RU", false));
        }

        //private void EventHandler_Button_Click(
        //    object sender, EventArgs e)
        //{
        //    Button btn = (Button)sender;

        //    if (btn.Equals(_ButtonF6))
        //    {
        //        // Скрывает или отображаем панель конопок
        //        _PanelFunctionalButtonsPanel.Visible = !_PanelFunctionalButtonsPanel.Visible;
        //    }
        //    else
        //        ((ICommand)btn.Tag).Execute();
        //}

        private void EventHandler_PanelFunctionalButtonsPanel_VisibleChanged(
            object sender, EventArgs e)
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
                if (control.Controls.Count > 0)
                    if (!control.Controls[0].Focused)
                        control.Controls[0].Focus();
            }
        }

        private void EventHandler_PanelWorkingRegion_GotFocus(
            object sender, EventArgs e)
        {
            Panel control = (Panel)sender;

            if (control.Controls.Count > 0)
            {
                control.Controls[0].Focus();
            }
        }

        private void EventHandler_PanelFunctionalButtonsPanel_ControlAdded(
            object sender, ControlEventArgs e)
        {
            RedrawFunctionalButtonsPanel();
        }

        private void EventHandler_PanelFunctionalButtonsPanel_Resize(
            object sender, EventArgs e)
        {
            RedrawFunctionalButtonsPanel();
        }

        #endregion

        #region Methods

        public void AddRangeFunctionalButtons(IEnumerable<FunctionalButton> buttons)
        {
            List<FunctionalButton> list = new List<FunctionalButton>(buttons);
            _PanelFunctionalButtonsPanel.Controls.AddRange(list.ToArray());
        }

        public void AddFunctionalButton(FunctionalButton button)
        {
            _PanelFunctionalButtonsPanel.Controls.Add(button);
        }

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

        private void RedrawFunctionalButtonsPanel()
        {
            const int MAX_CONTROLS = 5;
            const int MARGIN = 3;

            Size size = _PanelFunctionalButtonsPanel.Size;

            int height = (size.Height - (MAX_CONTROLS + 1) * MARGIN) / MAX_CONTROLS;
            int width = size.Width - MARGIN * 2;

            foreach (Control control in _PanelFunctionalButtonsPanel.Controls)
            {
                control.Height = height;
                control.Width = width;

                switch (((FunctionalButton)control).Key)
                {
                    case Keys.F2:
                        {
                            control.Location = new Point(MARGIN, MARGIN); 
                            break;
                        }
                    case Keys.F3:
                        {
                            control.Location = new Point(MARGIN, MARGIN * 2 + height);
                            break;
                        }
                    case Keys.F4:
                        {
                            control.Location = new Point(MARGIN, MARGIN * 3 + height * 2);
                            break;
                        }
                    case Keys.F5:
                        {
                            control.Location = new Point(MARGIN, MARGIN * 4 + height * 3);
                            break;
                        }
                    case Keys.F6:
                        {
                            control.Location = new Point(MARGIN, MARGIN * 5 + height * 4);
                            break;
                        }
                }
            }
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