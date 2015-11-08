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
    /// Класс для создания главного окна системы мониторинга
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
        /// Всего устройств в системе
        /// </summary>
        public Int32 TotalDevices
        {
            set 
            {
                this._StatusStripSystemInfo.TotalDevices = value;
            }
        }
        /// <summary>
        /// Неисправных устройств в системе
        /// </summary>
        public Int32 FaultyDevices
        {
            set
            {
                this._StatusStripSystemInfo.FaultyDevices = value;
            }
        }
        /// <summary>
        /// Устанавливает время в строке состояния
        /// </summary>
        public DateTime DateTime
        {
            set { this._StatusStripSystemInfo.DateTime = value; }
        }
        /// <summary>
        /// Панель кнопок
        /// </summary>
        private ButtonsPanel _ButtonsPanel;
        /// <summary>
        /// Статусная строка главного окна
        /// </summary>
        private SystemStatusBar _StatusStripSystemInfo;
        /// <summary>
        /// Основной контейнер. Правая панель - для кнопок,
        /// левая содержит - _TabControlMainFrame
        /// </summary>
        private SplitContainer _SplitContainerMainFrame;       
        /// <summary>
        /// 
        /// </summary>
        //private SplitContainer _SplitContainerWorkFrame;
        /// <summary>
        /// Закладки для представления различной информации о системе
        /// </summary>
        private TabControl _TabControlMainFrame;
        /// <summary>
        /// Возвращает контрол табулированных закладок для различных видов отображения информации.
        /// </summary>
        public TabControl TabControlViews
        {
            get { return _TabControlMainFrame; }
        }
        /// <summary>
        /// Закладка для _TabControlMainFrame. Здесь отображается состояние
        /// сети и устройств в сети.
        /// </summary>
        private TabPage _TabPageDevicesList;
        
        /// <summary>
        /// Закладка для _TabControlMainFrame. Здесь отображается сводная
        /// таблица параметров всех устройств КИП системы.
        /// </summary>
        private TabPage _TabPagePivotTable;  
        /// <summary>
        /// Грид для отображения списка сетевых устройств
        /// </summary>
        private DataGridView _DataGridViewDevicesList;
        /// <summary>
        /// Грид для отображения списка сетевых устройств
        /// </summary>
        public DataGridView DataGridViewDevicesList
        {
            get { return this._DataGridViewDevicesList; }
        }
        /// <summary>
        /// Грид для отображения сводной таблицы параметров со всех устройств сети.
        /// </summary>
        private DataGridView _DataGridViewParametersPivotTable;
        /// <summary>
        /// Грид для отображения сводной таблицы параметров со всех устройств сети.
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
        /// Хранит состояние курсора. Если true - курсор отображён, если 
        /// false - скрыт
        /// </summary>
        private Boolean _CursorState;
        
        /// <summary>
        /// Возвращает устанавливает состояние курсора (скрыт - false 
        /// или отображён - true)
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
        /// Конструктор по умолчанию
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
        /// Обработчик события загрузки формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_FormMain_Load(object sender, EventArgs e)
        {
            // Инициализируем форму
            this.CursorState = Settings.CursorEnable;
            this.ShowInTaskbar = Settings.ShowInTaskbar;
            if (Settings.FormBorderEnable == false)
            { 
                this.FormBorderStyle = FormBorderStyle.None;
            }
            this.Icon = Properties.Resources.faviconMy;
            this.Text = "Система коррозионного мониторинга";

            // Задаём маштабирование элементов формы в зависимости от разарешения
            // экрана
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowOnly;
            
            // Основной контейнер: правое - основное рабочее окно, левое - панель кнопок
            this._SplitContainerMainFrame = new SplitContainer();
            this._SplitContainerMainFrame.Name = "SplitContainerMainFrame";
            this._SplitContainerMainFrame.Dock = DockStyle.Fill;
            this._SplitContainerMainFrame.Orientation = Orientation.Vertical;

            this._SplitContainerMainFrame.Panel2MinSize = this._SplitContainerMainFrame.Width / 10;
            this._SplitContainerMainFrame.SplitterDistance = this._SplitContainerMainFrame.Width / 10 * 9;
            this._SplitContainerMainFrame.SplitterMoved += 
                new SplitterEventHandler(EventHandler_SplitContainerMainFrame_SplitterMoved);
            this._SplitContainerMainFrame.TabStop = true;
            
            // Панель кнопок
            this._ButtonsPanel = 
                new ButtonsPanel(this._SplitContainerMainFrame.Panel2);
            this._ButtonsPanel.ButtonOne.Click += new EventHandler(EventHandler_SystemButton_Click);
            this._ButtonsPanel.ButtonTwo.Click += new EventHandler(EventHandler_SystemButton_Click);
            this._ButtonsPanel.ButtonThree.Click += new EventHandler(EventHandler_SystemButton_Click);
            this._ButtonsPanel.ButtonFour.Click += new EventHandler(EventHandler_SystemButton_Click);
            this._ButtonsPanel.ButtonFive.Click += new EventHandler(EventHandler_SystemButton_Click);

            // Закладки для помещаются в правую панель основного контейнера _SplitContainerMainFrame
            this._TabControlMainFrame = new TabControl();
            this._TabControlMainFrame.Name = "TabControlMainFrame";
            this._TabControlMainFrame.Dock = DockStyle.Fill;
            this._TabControlMainFrame.Selecting += 
                new TabControlCancelEventHandler(EventHandler_TabControlMainFrame_Selecting);

            this._SplitContainerMainFrame.Panel1.Controls.Add(this._TabControlMainFrame);

            // Настройка закладок для _TabControlMainFrame
            this._TabPageDevicesList = new TabPage();
            this._TabPageDevicesList.Name = TabPageNames.TabPageDevicesList;
            this._TabPageDevicesList.Text = "Система";
            this._TabControlMainFrame.TabPages.Add(this._TabPageDevicesList);

            this._DataGridViewDevicesList = new DataGridView();
            this._DataGridViewDevicesList.Name = "ListOfDevices";
            this._DataGridViewDevicesList.Dock = DockStyle.Fill;
            this._TabPageDevicesList.Controls.Add(this._DataGridViewDevicesList);

            this._TabPagePivotTable = new TabPage();
            this._TabPagePivotTable.Name = TabPageNames.TabPagePivotTable;
            this._TabPagePivotTable.Text = "Сводная таблица";
            this._TabControlMainFrame.TabPages.Add(this._TabPagePivotTable);

            // Грид для отображения сводной таблицы параметров
            this._DataGridViewParametersPivotTable = new DataGridView();
            this._DataGridViewParametersPivotTable.Name = "DataGridViewParametersPivotTable";
            this._DataGridViewParametersPivotTable.Dock = DockStyle.Fill;
            this._TabPagePivotTable.Controls.Add(this._DataGridViewParametersPivotTable);

            // Настраиваем строку состояния окна
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
        /// Обработчик кликов по системным кнопкам на панели кнопок
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
        /// Обработчик события перемещения сплитера
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
        /// Обработчик события отображения формы
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
        /// Возвращат системную кнопку на панели конопок по её наименованию
        /// </summary>
        /// <param name="button"></param>
        /// <param name="text"></param>
        public Button GetSystemButton(string buttonName)
        {
            return _ButtonsPanel.GetButton(buttonName);
        }
        /// <summary>
        /// Генерирует событие Loaded
        /// </summary>
        private void OnLoaded()
        {
            if (this.Loaded != null)
            {
                this.Loaded(this, new EventArgs());
            }
        }
        /// <summary>
        /// Генерирует событие нажатия системной кнопки на панели кнопок
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
        /// Перехватчик сообщений посылаемых системой форме
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns>true если символ " a " Windows Presentation Foundation (WPF) сочетание клавиш; 
        /// в противном случае, false. См. здесь 
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
                            // Показываем окно управления сетями
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
                            // По нажатию F2 исполняем программный клик по кнопке F2
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
                            // Скрывает или отображаем панель конопок
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
        /// Событие происходит после события Load формы
        /// </summary>
        public event EventHandler Loaded;
        /// <summary>
        /// Событие клика по системной кнопке на панели кнопок
        /// </summary>
        public event SystemButtonClickEventHandler SystemButtonClick;
        #endregion

    }// End Of Class
}//End Of NameSpace