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
    public partial class MainWindowView : Form, IMainWindowView
    {
        #region Constructors

        public MainWindowView()
        {
            InitializeComponent();

            _ViewRegionCollection = new ViewRegionCollection();
            _ViewRegionCollection.Add(new ViewRegion(_PanelWorkingRegion));
            _ViewRegionCollection.Add(new ViewRegion(_PanelTitleRegion));
        }

        #endregion

        #region Fields And Properties

        protected static object SyncRoot = new object();
        Timer _Timer;
        ViewRegionCollection _ViewRegionCollection;

        /// <summary>
        /// Всего устройств в системе
        /// </summary>
        public Int32 TotalDevices 
        { 
            set 
            {
                _ToolStripButtonTotalDevices.Text = 
                    String.Format("Всего устройств: {0}", value);
            }
        }

        /// <summary>
        /// Неисправных устройств в системе
        /// </summary>
        public Int32 FaultyDevices
        {
            set
            {
                _ToolStripButtonFaultyDevices.Text =
                    String.Format("Неисправных устройств: {0}", value);
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

        public IViewRegion[] ViewRegions
        {
            get
            {
                IViewRegion[] regions;
                lock (SyncRoot)
                {
                    regions = new IViewRegion[_ViewRegionCollection.Count];
                    _ViewRegionCollection.CopyTo(regions, 0);
                }
                return regions;
            }
        }

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

        public ICommand[] ButtonCommands
        {
            set
            {
                Button[] btns = new Button[] { _ButtonF3, _ButtonF4, _ButtonF5 };

                foreach (Button btn in btns)
                {
                    btn.Text = String.Empty;
                    btn.Enabled = false;
                    btn.Visible = false;
                    btn.Tag = null;
                    btn.DataBindings.Clear();
                }

                if (value == null)
                    return;

                for (int i = 0; i < value.Length; i++)
                {
                    btns[i].Text = value[i].Name;
                    btns[i].Visible = true;
                    btns[i].Tag = value[i];
                    btns[i].DataBindings.Add(new Binding("Enabled", btns[i].Tag, "Status")); 
                }
            }
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
//                    case Keys.F1:
//                        {
//#if DEBUG
//                            // Показываем окно управления сетями
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
                            // По нажатию F2 исполняем программный клик по кнопке F2
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
                                // Скрывает или отображаем панель конопок
                                _PanelSystemButtonsRegion.Visible = !_PanelSystemButtonsRegion.Visible;
                            }
                            return false;
                        }
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Events        
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

        public IViewRegion WorkingRegion
        {
            get { return _ViewRegionCollection[_PanelWorkingRegion.Name]; }
        }

        public IViewRegion TitleRegion
        {
            get { return _ViewRegionCollection[_PanelTitleRegion.Name]; }
        }

        #endregion
    }
}