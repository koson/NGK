using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NGK.CorrosionMonitoringSystem.View
{
    public partial class TemplateView : Form
    {
        #region ButtonsPanel
        
        public enum Buttons
        {
            /// <summary>
            /// Меню
            /// </summary>
            F2,
            F3,
            F4,
            F5,
            /// <summary>
            /// Скрыть/отобразить панель кнопок
            /// </summary>
            F6
        }
        
        #endregion

        #region Constructors

        public TemplateView()
        {
            InitializeComponent();
        }

        #endregion

        #region Fields And Properties

        protected static object SyncRoot = new object();

        /// <summary>
        /// Всего устройств в системе
        /// </summary>
        public Int32 TotalDevices 
        { 
            set 
            {
                throw new NotImplementedException();
                //this._StatusStripSystemInfo.TotalDevices = value;
            }
        }

        /// <summary>
        /// Неисправных устройств в системе
        /// </summary>
        public Int32 FaultyDevices
        {
            set
            {
                throw new NotImplementedException();
                //this._StatusStripSystemInfo.FaultyDevices = value;
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
            get { return this._SplitContainerMain.Panel2Collapsed; }
            set { this._SplitContainerMain.Panel2Collapsed = value; }
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

        #endregion

        #region Event Handlers

        void TemplateView_Load(object sender, EventArgs e)
        {
            _ButtonF2.Click += new EventHandler(EventHandler_Button_Click);
            _ButtonF3.Click += new EventHandler(EventHandler_Button_Click);
            _ButtonF4.Click += new EventHandler(EventHandler_Button_Click);
            _ButtonF5.Click += new EventHandler(EventHandler_Button_Click);
            _ButtonF6.Click += new EventHandler(EventHandler_Button_Click);
        }

        void EventHandler_Button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.Equals(_ButtonF2))
            {
                OnButtonClick(new ButtonClickEventArgs(Buttons.F2));
            }
            else if (btn.Equals(_ButtonF3))
            {
                OnButtonClick(new ButtonClickEventArgs(Buttons.F3));
            }
            else if (btn.Equals(_ButtonF4))
            {
                OnButtonClick(new ButtonClickEventArgs(Buttons.F4));
            }
            else if (btn.Equals(_ButtonF5))
            {
                OnButtonClick(new ButtonClickEventArgs(Buttons.F5));
            }
            else if (btn.Equals(_ButtonF6))
            {
                // Скрывает или отображаем панель конопок
                if (_SplitContainerMain.Panel2Collapsed)
                {
                    _SplitContainerMain.Panel2Collapsed = false;
                }
                else
                {
                    _SplitContainerMain.Panel2Collapsed = true;
                }
            }
        }

        void EventHandler_TemplateView_Resize(object sender, EventArgs e)
        {
            Form frm = (Form)sender;
            //_SplitContainerMain.SplitterDistance = frm.Width / 7;
            TuneButtonsPanel();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Метод рассчитывает размеры кнопок в зависимости от размеров панели
        /// </summary>
        /// <returns></returns>
        private void TuneButtonsPanel()
        {
            System.Drawing.Size size;

            size = new Size(_SplitContainerMain.Panel2.Width, 
                _SplitContainerMain.Panel2.Height / 7);

            _ButtonF2.Size = size;
            _ButtonF3.Size = size;
            _ButtonF4.Size = size;
            _ButtonF5.Size = size;
            _ButtonF6.Size = size;

            // Координата кнопки "1"
            _ButtonF2.Location = new Point(0, this._ButtonF3.Height);
            _ButtonF3.Location = new Point(0, this._ButtonF3.Height * 2);
            _ButtonF4.Location = new Point(0, this._ButtonF3.Height * 3);
            _ButtonF5.Location = new Point(0, this._ButtonF3.Height * 4);
            _ButtonF6.Location = new Point(0, this._ButtonF3.Height * 5);

            return;
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
                                if (_SplitContainerMain.Panel2Collapsed)
                                {
                                    _SplitContainerMain.Panel2Collapsed = false;
                                }
                                else
                                {
                                    _SplitContainerMain.Panel2Collapsed = true;
                                }
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
    }

    public class ButtonClickEventArgs : EventArgs
    {
        public ButtonClickEventArgs(TemplateView.Buttons button)
        {
            _Button = button;
        }

        TemplateView.Buttons _Button;

        public TemplateView.Buttons Button
        {
            get { return _Button; }
        }
    }

}