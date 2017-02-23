using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Modbus.OSIModel.ApplicationLayer.Master;
using Modbus.OSIModel.DataLinkLayer.Master;
using Modbus.OSIModel.DataLinkLayer.Master.RTU.SerialPort;
using Modbus.OSIModel.Message;
using NGK.MeasuringDeviceTech.Classes.MeasuringDevice;

//====================================================================================
namespace NGK.MeasuringDeviceTech
{
    //================================================================================
    // "buttonSaveDevice"
    // "buttonNewDevice"
    // "buttonDeleteDevice"
    // "buttonConnection"
    // "buttonReadTypeOfDevice"
    // "buttonVerifyInitDevice"
    // "buttonReadDevice"
    // "buttonWriteDevice"
    // "buttonSyncDateTime"
    // "buttonStartMonitor"
    // "buttonStopMonitor"
    public struct TOOLSTRIPBUTTON
    {
         public const String buttonSaveDevice = "buttonSaveDevice";
         public const String buttonNewDevice = "buttonNewDevice";
         public const String buttonDeleteDevice = "buttonDeleteDevice";
         public const String buttonConnection = "buttonConnection";
         public const String buttonReadTypeOfDevice = "buttonReadTypeOfDevice";
         public const String buttonVerifyInitDevice = "buttonVerifyInitDevice";
         public const String buttonReadDevice = "buttonReadDevice";
         public const String buttonWriteDevice = "buttonWriteDevice";
         public const String buttonSyncDateTime = "buttonSyncDateTime";
         public const String buttonStartMonitor = "buttonStartMonitor";
         public const String buttonStopMonitor = "buttonStopMonitor";
    }
    //================================================================================
    //"mnuFile"
    //    "mnuFileCreate"
    //    "mnuFileOpen"
    //    "mnuFileClose"
    //    "mnuFileSave"
    //    "mnuFileSaveAs"
    //    "mnuFilePrint"
    //    "mnuFilePrintPreview"
    //    "mnuFilePrintSettings"
    //    "mnuFilePageSettings"
    //    "mnuFileExit"
    //"mnuDevice"
    //    "mnuDeviceIdentify"
    //    "mnuDeviceVerifyInit"
    //    "mnuDeviceRead"
    //    "mnuDeviceWrite"
    //    "mnuDeviceDateTime"
    //"mnuConnection"
    //    "mnuConnectionConnect"
    //    "mnuConnectionDisconnect"
    //    "mnuConnectionSaveSettings"
    //"mnuSettings"
    //    "mnuSettingsSettings"
    //"mnuHelp"
    //    "mnuHelpHelp"
    //    "mnuHelpAbout"
    public struct MENU
    {
        public const String mnuFile = "mnuFile";
        public const String mnuFieCreate = "mnuFileCreate";
        public const String mnuFileOpen =  "mnuFileOpen";
        public const String mnuFileClose = "mnuFileClose";
        public const String mnuFileSave = "mnuFileSave";
        public const String mnuFileSaveAs = "mnuFileSaveAs";
        public const String mnuFilePrint = "mnuFilePrint";
        public const String mnuFilePrintPreview = "mnuFilePrintPreview";
        public const String mnuFilePrintSettings = "mnuFilePrintSettings";
        public const String mnuFilePageSettings = "mnuFilePageSettings";
        public const String mnuFileExit = "mnuFileExit";
            
        public const String mnuDevice = "mnuDevice";
        public const String mnuDeviceIdentify = "mnuDeviceIdentify";
        public const String mnuDeviceVerifyInit = "mnuDeviceVerifyInit";
        public const String mnuDeviceRead = "mnuDeviceRead";
        public const String mnuDeviceWrite = "mnuDeviceWrite";
        public const String mnuDeviceDateTime = "mnuDeviceDateTime";
        
        public const String mnuConnection = "mnuConnection";
        public const String mnuConnectionConnect = "mnuConnectionConnect";
        public const String mnuConnectionDisconnect = "mnuConnectionDisconnect";
        public const String mnuConnectionSaveSettings = "mnuConnectionSaveSettings";
        
        public const String mnuSettings = "mnuSettings";
        public const String mnuSettingsSettings = "mnuSettingsSettings";
        
        public const String mnuHelp = "mnuHelp";
        public const String mnuHelpHelp = "mnuHelpHelp";
        public const String mnuHelpAbout = "mnuHelpAbout";

    }
    //================================================================================
    public partial class FormMain : Form
    {
        //----------------------------------------------------------------------------
        #region Fields and Properties
        //----------------------------------------------------------------------------
        /// <summary>
        /// Параметры коммандной строки. Передаются при запуске приложения из 
        /// коммандной строки с аргументами
        /// </summary>
        private String[] _CmdLineArgs;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Контектсное меню для элемента treeViewMain: NodeRoot -> NodeConnection
        /// </summary>
        private System.Windows.Forms.ContextMenuStrip _ContexMenuConnection;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Контектсное меню для элемента treeViewMain: NodeRoot -> NodeMeasuringDevice
        /// </summary>
        private System.Windows.Forms.ContextMenuStrip _ContexMenuDevice;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Контектсное меню для элемента propertyGridMain: Вызывается по клику правой
        /// кнопкой мыши по свойству объекта (параметру КИП)
        /// </summary>      
        private System.Windows.Forms.ContextMenuStrip _ContexMenuParameter;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Для выполения фоновых задач в асинхронном режиме
        /// </summary>
        private BackgroundWorker _BackgroundWorker;
        //----------------------------------------------------------------------------
        private NGK.MeasuringDeviceTech.Classes.Print.TextDocumentPrinter _Printer;
        //----------------------------------------------------------------------------
        #endregion
        //----------------------------------------------------------------------------
        #region Constructors
        //----------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        public FormMain()
        {
            //CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();

            // Сохраняем аргументы командной строки
            _CmdLineArgs = new String[0];
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="cmdLineArgs">Аргументы коммандной строки</param>
        /// <remarks>Первый аргумент командной строки - путь к exe-файлу
        /// приложения, Второй аргумент - путь к файлу-образу КИП.
        /// Остальные не имеют смысла</remarks>
        public FormMain(String[] cmdLineArgs)
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();

            // Сохраняем аргументы командной строки
            _CmdLineArgs = cmdLineArgs;
        }
        //----------------------------------------------------------------------------
        #endregion
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события выбора узла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewMain_AfterSelect(object sender, TreeViewEventArgs e)
        {
                switch (e.Node.Name)
                {
                    case "NodeRoot":
                        {
                            this.propertyGridMain.SelectedObject = null;
                            break;
                        }
                    case "NodeConnection":
                        {
                            this.propertyGridMain.SelectedObject = 
                                (Modbus.OSIModel.DataLinkLayer.Master.IDataLinkLayer)_SerialPort;
                            break;
                        }
                    case "NodeMeasuringDevice":
                        {
                            if (_Host != null)
                            {
                                this.propertyGridMain.SelectedObject = _MeasuringDevice;
                            }
                            else
                            {
                                this.propertyGridMain.SelectedObject = null;
                            }
                            break;
                        }
                }
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню "Подключить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemConnect_Click(object sender, EventArgs e)
        {
            //ToolStripMenuItem mnu = sender as ToolStripMenuItem;
            this.Connect();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню "Отключить"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemDisconnect_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mnu = sender as ToolStripMenuItem;
            this.Disconnect();
            return;
        }
        //----------------------------------------------------------------------------
        //private Thread _thread;

        //private void Method()
        //{
        //    throw new Exception("MyExceptionThread"); 
        //}
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события загрузки формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_Load(object sender, EventArgs e)
        {
            //_thread = new Thread(new ThreadStart(Method));
            //_thread.Start();

            //throw new Exception("MyException");


            // Инициализируем меню
            this.InitMenu(ref this.menuStripMain);
            // Инициализируем панель инструментов
            this.InitToolBar(ref this.toolStripMain);
            // Инициализируем контекстные меню
            this.InitContextMenu();
            // Инициализируем строку состояния
            this.InitStatusBar(ref this.statusStripMain);

            // Устанавливаем элементы окна в исходное состояние
            this.DefaultState();

            // Устанавливаем размеры и расположение окна
            try
            {
                this.Location = Properties.Settings.Default.WindowLocation;
                this.Size = Properties.Settings.Default.WindowSize;

                this.splitContainerMain.SplitterDistance = 
                    Properties.Settings.Default.SplitterDistance;

                SetSplitterPropertyGrid(this.propertyGridMain, Properties.Settings.Default.SplitterPropertyGrid);
            }
            catch
            {
                MessageBox.Show(this, "Неудалось найти файл настроек", 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.Size = new Size(600, 800);
                this.splitContainerMain.SplitterDistance = 70;
                this.SetSplitterPropertyGrid(this.propertyGridMain, 50);
                this.StartPosition = FormStartPosition.CenterScreen;
            }

            // Читаем сохранённые настройки подключения
            String portName;
            int bautRate;
            int dataBits;
            System.IO.Ports.Parity parity;
            System.IO.Ports.StopBits stopBits;
            int delay;
            int timeOut;

            try
            {
                portName = Properties.Settings.Default.COMPort;
                bautRate = Properties.Settings.Default.BaudRate;
                dataBits = Properties.Settings.Default.DataBits;
                parity = Properties.Settings.Default.Parity;
                stopBits = Properties.Settings.Default.StopBits;
                delay = Properties.Settings.Default.Delay;
                timeOut = Properties.Settings.Default.TimeOut;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format(
                    "Невозможно прочитать настройки потра, будут применены по умолчанию. Исключение: {0}",
                    ex.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                portName = "COM1";
                bautRate = 19200;
                dataBits = 8;
                parity = System.IO.Ports.Parity.Even;
                stopBits = System.IO.Ports.StopBits.One;
                delay = 500;
                timeOut = 300;
            }

            _SerialPort = 
                this.CreateConnection(portName, bautRate, dataBits, parity, stopBits, timeOut, delay);

            // Настраиваем события от Propertygrid
            // !!! Напрямую события от мыши и клавиатуры не работают. Объясняют это тем,
            // что контрол состоит из других контролов, которые не передают родителю
            // данные события. Нашел как добраться до события OnClick через рефлексию. 
            FieldInfo info = propertyGridMain.GetType().GetField("gridView", 
                BindingFlags.Instance | BindingFlags.NonPublic);
            Control gridView = (Control)info.GetValue(propertyGridMain);
            gridView.Click += new EventHandler(propertyGridMain_Click);


            // Обрабатываем аргументы коммандной строки
            if (_CmdLineArgs.Length != 0)
            {
                if (_CmdLineArgs.Length > 1)
                {
                    // В 0-м индексе предаётся пусть к exe-файлу приложения
                    // Нам нужна строка с 1-ым индексом, где должен передаваться
                    // пусть к файлу с расширением .kip. Если файл существует окрываем его...
                    String path = _CmdLineArgs[1];
                    this.OpenFile(path);
                }
            }

            // Настраиваем печать
            _Printer = new Classes.Print.TextDocumentPrinter();

            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню. Завершаем приложение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            if (_SerialPort != null)
            {
                _SerialPort.CloseConnect();
                _SerialPort = null;
            }

            if (_Host != null)
            {
                _Host = null;
            }

            if (_MeasuringDevice != null)
            {
                _MeasuringDevice = null;
            }

            Application.Exit();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события возникающего после закрытия формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Сохранияем текущие размеры и положение на экране формы
            try
            {
                Properties.Settings.Default.WindowLocation = this.Location;
                Properties.Settings.Default.WindowSize = this.Size;

                Properties.Settings.Default.SplitterDistance = 
                    this.splitContainerMain.SplitterDistance;
                Properties.Settings.Default.SplitterPropertyGrid = 
                    this.GetSplitterPropertyGrid(this.propertyGridMain);
                
                Properties.Settings.Default.Save();
            }
            catch
            {
                // Не удалось сохранить.
            }
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Метод изменяет ширину колонок в контроле PropertyGrid
        /// </summary>
        /// <param name="control">PropertyGrid</param>
        /// <param name="width">Ширина первой колнки</param>
        private void SetSplitterPropertyGrid(PropertyGrid control, int width)
        {
            Type type  = control.GetType();
            
            FieldInfo field = type.GetField("gridView",
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            object valGrid = field.GetValue(control);
            
            Type gridType = valGrid.GetType();

            gridType.InvokeMember("MoveSplitterTo",
                BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance,
                null,
                valGrid,
                new object[] {width});

            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Метод возвращат ширину первой колонки PropertyGrid
        /// </summary>
        /// <param name="control">PropertyGrid</param>
        /// <returns>Ширина первой колонки</returns>
        private int GetSplitterPropertyGrid(PropertyGrid control)
        {
            Type type = control.GetType();
            FieldInfo field = type.GetField("gridView",
                BindingFlags.NonPublic | BindingFlags.Instance);

            object valGrid = field.GetValue(control);
            Type gridType = valGrid.GetType();
            int width = (int)gridType.InvokeMember("GetLabelWidth",
                BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance,
                null, valGrid, new object[] { });

            //MessageBox.Show(width.ToString());
            
            return width;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню "Прочитать данные"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemReadDevice_Click(object sender, EventArgs e)
        {
            this.DeviceRead();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню "Записать данные"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemWriteDevice_Click(object sender, EventArgs e)
        {
            this.DeviceWrite();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик кнопки на панели инструментов "Прочитать данные"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonReadDevice_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(this, "Заглушка", "Внимание",
            //    MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DeviceRead();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик кнопки на панели инструментов "Записать данные"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonWriteDevice_Click(object sender, EventArgs e)
        {
            this.DeviceWrite();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню открыть файл с образом КИП-а
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemOpenFile_Click(object sender, EventArgs e)
        {
            this.OpenFile();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню закрыть устройство.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemCloseFile_Click(object sender, EventArgs e)
        {
            this.DeleteDevice();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню сохранить в файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemSave_Click(object sender, EventArgs e)
        {
            this.SaveFile();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню "Сохранить как"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemSaveAs_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "В данной версии ПО не реализовано", "Внимание",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню "Настройки подключения"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemConnectionSettings_Click(object sender, EventArgs e)
        {
            treeViewMain.SelectedNode = treeViewMain.Nodes["NodeRoot"].Nodes["NodeConnection"];
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню клика мышью по treeViewMain
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewMain_MouseDown(object sender, MouseEventArgs e)
        {
            //TreeView tv = sender as TreeView;
            //Region region;
            //TreeNode node;

            //switch (e.Button)
            //{
            //    case System.Windows.Forms.MouseButtons.Right:
            //        {
            //            node = tv.Nodes["NodeRoot"].Nodes["NodeConnection"];
            //            region = new System.Drawing.Region(node.Bounds);
                        
            //            if (region.IsVisible(e.Location))
            //            {
            //                // Делаем узел активным
            //                tv.SelectedNode = node;
            //                // Отображаем контекстное меню
            //                this.contextMenuStripConnection.Show(tv.PointToScreen(e.Location));
            //            }
                        
            //            node = tv.Nodes["NodeRoot"].Nodes["NodeMeasuringDevice"];
            //            region = new Region(node.Bounds);

            //            if (region.IsVisible(e.Location))
            //            {
            //                // Делаем узел активным
            //                tv.SelectedNode = node;
            //                // Отображаем контекстное меню
            //                this.contextMenuStripDevice.Show(tv.PointToScreen(e.Location));
            //            }
            //            break;
            //        }
            //}
            //return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик контекстного меню. Сохраняем текущие настройки соединения.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemSaveConnection_Click(object sender, EventArgs e)
        {
            // Сохраняем настройки порта в конфигурационный файл
            SavePortSettings();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Сохраняет настройки последовательного порта в 
        /// файл конфигурации приложения 
        /// </summary>
        private void SavePortSettings()
        {
            try
            {
                Properties.Settings.Default.COMPort = _SerialPort.SerialPort.PortName;
                Properties.Settings.Default.BaudRate = _SerialPort.SerialPort.BaudRate;
                Properties.Settings.Default.DataBits = _SerialPort.SerialPort.DataBits;
                Properties.Settings.Default.Parity = _SerialPort.SerialPort.Parity;
                Properties.Settings.Default.StopBits = _SerialPort.SerialPort.StopBits;
                Properties.Settings.Default.Delay = _SerialPort.ValueTurnAroundDelay;
                Properties.Settings.Default.TimeOut = _SerialPort.TimeOut;
                Properties.Settings.Default.Save();
            }
            catch (Exception e)
            {
                MessageBox.Show(this, String.Format("Невозможно сохранить настройки порта. {0}", e.Message), 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню "Справка"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemHelpMy_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "В данной версии ПО, справочная система отсутствует", 
                "Внимание",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню "О программе"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemAbout_Click(object sender, EventArgs e)
        {
            NGK.Forms.AboutBoxMain frm = new NGK.Forms.AboutBoxMain();
            frm.ShowInTaskbar = false;
            frm.ShowDialog();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик кнопки на панели инструментов "Соединение"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonConnection_Click(object sender, EventArgs e)
        {
            ToolStripButton tsb = sender as ToolStripButton;

            if (this._SerialPort != null)
            {
                if (this._SerialPort.IsOpen())
                {
                    this.Disconnect();
                }
                else
                {
                    this.Connect();
                }
            }
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события нажатия кнопки мыши по элементу PropertyGrid
        /// (а точнее, по GridView, в ходящего в его состав)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// Реагируем по правой кнопке мыши:
        /// Если в пропертигрид ничего не отображено, не реагируем
        /// Если отображены свойства подключинеия - не реагируем
        /// Если отображены свойства БИ и есть открытое соединение - вызываем 
        /// контекстное меню, в противном случае не реагируем.
        /// </remarks>
        private void propertyGridMain_Click(object sender, EventArgs e)
        {
            MouseEventArgs args = e as MouseEventArgs;
            Control cntrl = sender as Control;

            switch (args.Button)
            {
                case System.Windows.Forms.MouseButtons.Right:
                    {
                        if (propertyGridMain.SelectedObject != null)
                        {
                            if (_SerialPort != null)
                            {
                                if (_SerialPort.IsOpen())
                                {
                                    //if (propertyGridMain.SelectedObject is MeasuringDeviceMainPower)
                                    if (propertyGridMain.SelectedObject is IMeasuringDevice)
                                    {
                                        GridItem item = propertyGridMain.SelectedGridItem;
                                        // Проверяем доступно ли данное свойство для редактирования
                                        if (item.PropertyDescriptor.IsReadOnly)
                                        {
                                            _ContexMenuParameter.Items["toolStripMenuItemWriteParameter"].Enabled = false;
                                            _ContexMenuParameter.Show(cntrl.PointToScreen(args.Location));
                                        }
                                        else
                                        {
                                            _ContexMenuParameter.Items["toolStripMenuItemWriteParameter"].Enabled = true;
                                            _ContexMenuParameter.Show(cntrl.PointToScreen(args.Location));
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
            }
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик контекстного меню "Прочитать парамер"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Считываем параметр из физического устройства БИ</remarks>
        private void toolStripMenuItemGetParametr_Click(object sender, EventArgs e)
        {
            GridItem property = this.propertyGridMain.SelectedGridItem;
            OperationResult result;

            switch (property.Label)
            {
                case @"Разрешение расширенного диапазона Х10":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_CL_ExtendedModeX10SumPotentialEn(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Измерение наведённого напряжения":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_CL_InducedVoltageEn(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }                        
                        break;
                    }
                case @"Измерение поляризационного потенциала":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_CL_PolarizationPotentialEn(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Измерение тока поляризации":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_CL_PolarizationСurrentEn(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Измерение защитного потенциала":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_CL_ProtectivePotentialEn(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Измерение защитного тока":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_CL_ProtectiveСurrentEn(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Разрешение передачи слова состояния":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_CL_SendStatusWordEn(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Разрешение измерения DC тока натекания":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_CL_DcCurrentRefereceElectrodeEn(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Разрешение измерения AC тока натекания":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_CL_AcCurrentRefereceElectrodeEn(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Элемент питания, норма":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_DI_BattaryVoltageStatus(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Датчик встрытия НГК-КИП СМ(У)":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_DI_CaseOpen(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Датчик коррозии №1":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_DI_CorrosionSensor1(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Датчик коррозии №2":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_DI_CorrosionSensor2(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Датчик коррозии №3":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_DI_CorrosionSensor3(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Состояние напряжения притания устройства НГК-БИ":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_DI_SupplyVoltageStatus(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Скорость обмена СAN, кБ/сек":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_HR_BaudRateCAN(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Токовый шунт, А":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_HR_CurrentShuntValue(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Период измерений, сек.":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_HR_MeasuringPeriod(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Период измерения питающего напряжения, сек.":
                    {
                        this.Cursor = Cursors.WaitCursor; 
                        this._MeasuringDevice.Read_HR_MeasuringVoltagePeriod(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}", 
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Адрес/Номер устройства":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_HR_NetAddress(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Период опроса БПИ, сек":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_HR_PollingPeriodBPI(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Период опроса канала 1, сек":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_HR_PollingPeriodChannel1(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Период опроса канала 2, сек":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_HR_PollingPeriodChannel2(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Период опроса УСИКПСТ, сек":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_HR_PollingPeriodUSIKPST(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Серийный номер":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_SerialNumber(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Напряжение батареи, В":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_BattaryVoltage(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Ток измерительного канала 1 (4-20), mA":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_Current_Channel_1(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Ток измерительного канала 2 (4-20), mA":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_Current_Channel_2(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Дата и время":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        DateTime dt;
                        this._MeasuringDevice.Read_HR_DateTime(ref _Host, out result, out dt);
                       
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Глубина коррозии УСИКПСТ, мкм":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_DepthOfCorrosionUSIKPST(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Наведённое напряжение, В":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_InducedVoltage(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Температура встроенного датчика, гр.С":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_InternalTemperatureSensor(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Ток поляризации, mA":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_Polarization_current(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Поляризационный потенциал, В":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_Polarization_potential(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Ток катодной защиты, А":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_Protective_current(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Защитный потенциал, В":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_Protective_potential(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Скорость коррозии УСИКПСТ, мкм":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_SpeedOfCorrosionUSIKPST(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Код состояния устройства УСИКПСТ":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_StatusUSIKPST(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Напряжение питания НГК-БИ, В":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_SupplyVoltage(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Ток натекания переменный, mA":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_ReferenceElectrodeACCurrent(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Ток натекания постоянный, mA":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_ReferenceElectrodeDCCurrent(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Версия аппаратуры":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_HardWareVersion(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        
                        //    MessageBox.Show(this,
                        //        "В данный момент этот параметр недоступен для чтения.",
                        //        "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                case @"Версия ПО":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_SoftWareVersion(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось прочитать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        //MessageBox.Show(this,
                        //    "Данный параметр в данный момент не доступен для чтения",
                        //    "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                case @"Адрес устройства НГК-БИ сервис":
                    {
                        MessageBox.Show(this,
                            "Данный параметр в данный момент не доступен для чтения и носит информативный характер",
                            "Сообщение",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                case "Код производителя":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_ManufacturerCode(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case "Контрольная сумма":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Read_IR_CRC16(ref _Host, out result);
                        this.Cursor = Cursors.Default;
                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format(
                                "Неудалось прочитать параметр или устройство вернуло некорректное значение параметра. {0}",
                                result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case "Тип устройства":
                    {
                        MessageBox.Show(this,
                            "Данный параметр в данный момент не доступен для чтения",
                            "Сообщение",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                default:
                    {
                        MessageBox.Show(this, "Неизвесный параметр КИП", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
            }

            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик контекстного меню "Записать параметр"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Записываем параметр в физическое устройство БИ</remarks>
        private void toolStripMenuItemSetParametr_Click(object sender, EventArgs e)
        {
            GridItem property = this.propertyGridMain.SelectedGridItem;
            OperationResult result;

            switch(property.Label)
            {
                case @"Разрешение расширенного диапазона Х10":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Write_CL_ExtendedModeX10SumPotentialEn(ref _Host, out result);
                        this.Cursor = Cursors.Default;

                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось записать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Измерение наведённого напряжения":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Write_CL_InducedVoltageEn(ref _Host, out result);
                        this.Cursor = Cursors.Default;

                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось записать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Измерение поляризационного потенциала":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Write_CL_PolarizationPotentialEn(ref _Host, out result);
                        this.Cursor = Cursors.Default;

                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось записать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Измерение тока поляризации":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Write_CL_PolarizationСurrentEn(ref _Host, out result);
                        this.Cursor = Cursors.Default;

                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось записать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Измерение защитного потенциала":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Write_CL_ProtectivePotentialEn(ref _Host, out result);
                        this.Cursor = Cursors.Default;

                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось записать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Измерение защитного тока":
                    {
                        this.Cursor = Cursors.WaitCursor; 
                        this._MeasuringDevice.Write_CL_ProtectiveСurrentEn(ref _Host, out result);
                        this.Cursor = Cursors.Default;

                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось записать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Скорость обмена СAN, кБ/сек":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Write_HR_BaudRateCAN(ref _Host, out result);
                        this.Cursor = Cursors.Default;

                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось записать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Токовый шунт, А":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Write_HR_CurrentShuntValue(ref _Host, out result);
                        this.Cursor = Cursors.Default;

                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось записать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Период измерений, сек.":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Write_HR_MeasuringPeriod(ref _Host, out result);
                        this.Cursor = Cursors.Default;

                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось записать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Адрес/Номер устройства":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Write_HR_NetAddress(ref _Host, out result);
                        this.Cursor = Cursors.Default;

                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось записать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Период опроса БПИ, сек":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Write_HR_PollingPeriodBPI(ref _Host, out result);
                        this.Cursor = Cursors.Default;

                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось записать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Период опроса канала 1, сек":
                    {
                        this.Cursor = Cursors.WaitCursor; 
                        this._MeasuringDevice.Write_HR_PollingPeriodChannel1(ref _Host, out result);
                        this.Cursor = Cursors.Default;

                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось записать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Период опроса канала 2, сек":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Write_HR_PollingPeriodChannel2(ref _Host, out result);
                        this.Cursor = Cursors.Default;

                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось записать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Период опроса УСИКПСТ, сек":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Write_HR_PollingPeriodUSIKPST(ref _Host, out result);
                        this.Cursor = Cursors.Default;

                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось записать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Период измерения питающего напряжения, сек.":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Write_HR_MeasuringVoltagePeriod(ref _Host, out result);
                        this.Cursor = Cursors.Default;

                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось записать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Разрешение передачи слова состояния":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Write_CL_SendStatusWordEn(ref _Host, out result);
                        this.Cursor = Cursors.Default;

                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось записать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Разрешение измерения DC тока натекания":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Write_CL_DcCurrentRefereceElectrodeEn(ref _Host, out result);
                        this.Cursor = Cursors.Default;

                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось записать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case @"Разрешение измерения AC тока натекания":
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this._MeasuringDevice.Write_CL_AcCurrentRefereceElectrodeEn(ref _Host, out result);
                        this.Cursor = Cursors.Default;

                        if (result.Result != OPERATION_RESULT.OK)
                        {
                            MessageBox.Show(this,
                                String.Format("Неудалось записать параметр. {0}", result.Message),
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                default:
                    {
                        MessageBox.Show(this, "Неизвестный параметр КИП", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
            }
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик срабатываения таймера 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerSystemTime_Tick(object sender, EventArgs e)
        {
            // Устанавливаем системную дату
            ((ToolStripLabel)this.statusStripMain.Items["Date"]).Text = 
                DateTime.Now.ToLongDateString();

            ((ToolStripLabel)this.statusStripMain.Items["Time"]).Text =
                DateTime.Now.ToLongTimeString();

            // Выводим дату и вермя в формате UTC:
            ((ToolStripLabel)this.statusStripMain.Items["DateTimeUtc"]).Text =
                "UTC +0: " + DateTime.Now.ToUniversalTime().ToShortDateString() + " " +
                DateTime.Now.ToUniversalTime().ToLongTimeString();
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события возникающего при закрытии формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {            
            // Освобождаем ресуры
            if (_File != null)
            {
                _File.Close();
            }
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Инициализирует меню главной формы
        /// </summary>
        /// <param name="menu">Меню для инициализации</param>
        private void InitMenu(ref System.Windows.Forms.MenuStrip menu)
        {
            ToolStripMenuItem mnu;
            ToolStripSeparator separator;

            // Меню "Файл"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuFile";
            mnu.Text = "Файл";
            menu.Items.Add(mnu);

            // Меню "Файл" -> "Создать"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuFileCreate";
            mnu.Text = "Создать";
            mnu.Image = NGK.MeasuringDeviceTech.Properties.Resources.IconAddDevice;
            mnu.ImageAlign = ContentAlignment.MiddleCenter;
            mnu.ImageScaling = ToolStripItemImageScaling.None;
            mnu.Click += new EventHandler(MenuNewDevice_Click);
            ((ToolStripMenuItem)menu.Items["mnuFile"]).DropDownItems.Add(mnu);

            // Меню "Файл" -> "Открыть"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuFileOpen";
            mnu.Text = "Открыть";
            mnu.Image = NGK.MeasuringDeviceTech.Properties.Resources._075b_UpFolder.ToBitmap();
            mnu.ImageAlign = ContentAlignment.MiddleCenter;
            mnu.ImageScaling = ToolStripItemImageScaling.None;
            mnu.Click += new EventHandler(toolStripMenuItemOpenFile_Click);
            ((ToolStripMenuItem)menu.Items["mnuFile"]).DropDownItems.Add(mnu);

            // Меню "Файл" -> "Закрыть"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuFileClose";
            mnu.Text = "Закрыть";
            mnu.Image = NGK.MeasuringDeviceTech.Properties.Resources.IconDeleteDevice;
            mnu.ImageAlign = ContentAlignment.MiddleCenter;
            mnu.ImageScaling = ToolStripItemImageScaling.None;
            mnu.Click += new EventHandler(toolStripMenuItemCloseFile_Click);
            ((ToolStripMenuItem)menu.Items["mnuFile"]).DropDownItems.Add(mnu);

            // Сепаратор
            separator = new ToolStripSeparator();
            separator.Name = "separatorFile1";
            //this.toolStripSeparator4.Size = new System.Drawing.Size(175, 6);
            ((ToolStripMenuItem)menu.Items["mnuFile"]).DropDownItems.Add(separator);

            // Меню "Файл" -> "Сохранить"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuFileSave";
            mnu.Text = "Сохранить";
            mnu.Image = NGK.MeasuringDeviceTech.Properties.Resources.FloppyDisk;
            mnu.ImageAlign = ContentAlignment.MiddleCenter;
            mnu.ImageScaling = ToolStripItemImageScaling.None;
            mnu.Click += new EventHandler(toolStripMenuItemSave_Click);
            ((ToolStripMenuItem)menu.Items["mnuFile"]).DropDownItems.Add(mnu);

            // Меню "Файл" -> "Сохранить как"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuFileSaveAs";
            mnu.Text = "Сохранить как";
            mnu.Click += new EventHandler(toolStripMenuItemSaveAs_Click);
            mnu.Enabled = false;
            ((ToolStripMenuItem)menu.Items["mnuFile"]).DropDownItems.Add(mnu);
            
            // Сепаратор
            separator = new ToolStripSeparator();
            separator.Name = "separatorFile2";
            ((ToolStripMenuItem)menu.Items["mnuFile"]).DropDownItems.Add(separator);

            // Меню "Файл" -> "Печать"
            mnu = new ToolStripMenuItem();
            mnu.Name = MENU.mnuFilePrint;
            mnu.Text = "Печать";
            mnu.Click += new EventHandler(toolStripMenuItemPrint_Click);
            ((ToolStripMenuItem)menu.Items["mnuFile"]).DropDownItems.Add(mnu);

            // Меню "Файл" -> "Предварительный просмотр"
            mnu = new ToolStripMenuItem();
            mnu.Name = MENU.mnuFilePrintPreview;
            mnu.Text = "Предварительный просмотр";
            mnu.Click += new EventHandler(toolStripMenuItemPrintPreview_Click);
            ((ToolStripMenuItem)menu.Items["mnuFile"]).DropDownItems.Add(mnu);

            // Меню "Файл" -> "Настройки печати"
            mnu = new ToolStripMenuItem();
            mnu.Name = MENU.mnuFilePrintSettings;
            mnu.Text = "Настройки печати";
            mnu.Click += new EventHandler(toolStripMenuItemPintSettings_Click);
            ((ToolStripMenuItem)menu.Items["mnuFile"]).DropDownItems.Add(mnu);

            // Меню "Файл" -> "Настройки страницы"
            mnu = new ToolStripMenuItem();
            mnu.Name = MENU.mnuFilePageSettings;
            mnu.Text = "Настройки старинцы";
            mnu.Click += new EventHandler(toolStripMenuItemPageSettings_Click);
            ((ToolStripMenuItem)menu.Items["mnuFile"]).DropDownItems.Add(mnu);

            // Сепаратор
            separator = new ToolStripSeparator();
            separator.Name = "separatorFile3";
            //this.toolStripSeparator4.Size = new System.Drawing.Size(175, 6);
            ((ToolStripMenuItem)menu.Items["mnuFile"]).DropDownItems.Add(separator);

            // Меню "Файл" -> "Выход"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuFileExit";
            mnu.Text = "Выход";
            mnu.Click += new EventHandler(toolStripMenuItemExit_Click);
            ((ToolStripMenuItem)menu.Items["mnuFile"]).DropDownItems.Add(mnu);

            // Меню "Устройство КИП"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuDevice";
            mnu.Text = "КИП";
            menu.Items.Add(mnu);

            // Меню "Устройство КИП" -> "Идентифицировать"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuDeviceIdentify";
            mnu.Text = "Идентифицировать устройство";
            mnu.Image = NGK.MeasuringDeviceTech.Properties.Resources.IconUnknownDevice;
            mnu.ImageAlign = ContentAlignment.MiddleCenter;
            mnu.ImageScaling = ToolStripItemImageScaling.None;
            mnu.Click += new EventHandler(toolStripMenuItemDeviceIdentify_Click);
            ((ToolStripMenuItem)menu.Items["mnuDevice"]).DropDownItems.Add(mnu);

            // Меню "Устройство КИП" -> "Проверить инициализацию"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuDeviceVerifyInit";
            mnu.Text = "Проверить инициализацию";
            mnu.Image = NGK.MeasuringDeviceTech.Properties.Resources.IconVerifyDevice;
            mnu.ImageAlign = ContentAlignment.MiddleCenter;
            mnu.ImageScaling = ToolStripItemImageScaling.None;
            mnu.Click += new EventHandler(toolStripMenuItemDeviceVerifyInit_Click);
            ((ToolStripMenuItem)menu.Items["mnuDevice"]).DropDownItems.Add(mnu);

            // Сепаратор
            separator = new ToolStripSeparator();
            separator.Name = "separatorDevice1";
            //this.toolStripSeparator4.Size = new System.Drawing.Size(175, 6);
            ((ToolStripMenuItem)menu.Items["mnuDevice"]).DropDownItems.Add(separator);

            // Меню "Устройство КИП" -> "Читать КИП"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuDeviceRead";
            mnu.Text = "Читать КИП";
            mnu.Image = NGK.MeasuringDeviceTech.Properties.Resources.IconLoadFrom;
            mnu.ImageAlign = ContentAlignment.MiddleCenter;
            mnu.ImageScaling = ToolStripItemImageScaling.None;
            mnu.Click += new EventHandler(toolStripMenuItemReadDevice_Click);
            ((ToolStripMenuItem)menu.Items["mnuDevice"]).DropDownItems.Add(mnu);

            // Меню "Устройство КИП" -> "Записать КИП"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuDeviceWrite";
            mnu.Text = "Записать КИП";
            mnu.Image = NGK.MeasuringDeviceTech.Properties.Resources.IconLoadTo;
            mnu.ImageAlign = ContentAlignment.MiddleCenter;
            mnu.ImageScaling = ToolStripItemImageScaling.None;
            mnu.Click += new EventHandler(toolStripMenuItemWriteDevice_Click);
            ((ToolStripMenuItem)menu.Items["mnuDevice"]).DropDownItems.Add(mnu);

            // Сепаратор
            separator = new ToolStripSeparator();
            separator.Name = "separatorDevice2";
            //this.toolStripSeparator4.Size = new System.Drawing.Size(175, 6);
            ((ToolStripMenuItem)menu.Items["mnuDevice"]).DropDownItems.Add(separator);

            // Меню "Устройство КИП" -> "Синхронизировать время"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuDeviceDateTime";
            mnu.Text = "Синхронизировать время";
            mnu.Image = NGK.MeasuringDeviceTech.Properties.Resources.history;
            mnu.ImageAlign = ContentAlignment.MiddleCenter;
            mnu.ImageScaling = ToolStripItemImageScaling.None;
            mnu.Click += new EventHandler(toolStripMenuItemDateTime_Click);
            ((ToolStripMenuItem)menu.Items["mnuDevice"]).DropDownItems.Add(mnu);

            // Меню "Подключение"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuConnection";
            mnu.Text = "Соединение";
            menu.Items.Add(mnu);

            // Меню "Подключение" -> "Подключить"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuConnectionConnect";
            mnu.Text = "Подключить";
            mnu.Image = NGK.MeasuringDeviceTech.Properties.Resources.IconConnect;
            mnu.ImageAlign = ContentAlignment.MiddleCenter;
            mnu.ImageScaling = ToolStripItemImageScaling.None;
            mnu.Click += new EventHandler(toolStripMenuItemConnect_Click);
            ((ToolStripMenuItem)menu.Items["mnuConnection"]).DropDownItems.Add(mnu);

            // Меню "Подключение" -> "Отключить"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuConnectionDisconnect";
            mnu.Text = "Отключить";
            mnu.Image = NGK.MeasuringDeviceTech.Properties.Resources.IconDisconnect;
            mnu.ImageAlign = ContentAlignment.MiddleCenter;
            mnu.ImageScaling = ToolStripItemImageScaling.None;
            mnu.Click += new EventHandler(toolStripMenuItemDisconnect_Click);
            ((ToolStripMenuItem)menu.Items["mnuConnection"]).DropDownItems.Add(mnu);

            // Сепаратор
            separator = new ToolStripSeparator();
            separator.Name = "separatorConnection1";
            //this.toolStripSeparator4.Size = new System.Drawing.Size(175, 6);
            ((ToolStripMenuItem)menu.Items["mnuConnection"]).DropDownItems.Add(separator);

            // Меню "Подключение" -> "Сохранить настройки"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuConnectionSaveSettings";
            mnu.Text = "Сохранить настройки";
            mnu.Click += new EventHandler(toolStripMenuItemConnectionSaveSettings_Click);
            ((ToolStripMenuItem)menu.Items["mnuConnection"]).DropDownItems.Add(mnu);

            // Меню "Настройки"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuSettings";
            mnu.Text = "Настройки";
            menu.Items.Add(mnu);

            // Меню "Помощь" -> "Настройки"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuSettingsSettings";
            mnu.Text = "Настройки";
            mnu.Enabled = false;
            mnu.Click += new EventHandler(toolStripMenuItemSettings_Click);
            ((ToolStripMenuItem)menu.Items["mnuSettings"]).DropDownItems.Add(mnu);

            // Меню "Помощь"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuHelp";
            mnu.Text = "Справка";
            menu.Items.Add(mnu);

            // Меню "Помощь" -> "Справка"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuHelpHelp";
            mnu.Text = "Справка";
            mnu.Image = NGK.MeasuringDeviceTech.Properties.Resources.Annotate_Help.ToBitmap();
            mnu.ImageAlign = ContentAlignment.MiddleCenter;
            mnu.ImageScaling = ToolStripItemImageScaling.None;
            mnu.Click += new EventHandler(toolStripMenuItemHelpMy_Click);
            ((ToolStripMenuItem)menu.Items["mnuHelp"]).DropDownItems.Add(mnu);

            // Сепаратор
            separator = new ToolStripSeparator();
            separator.Name = "separatorHelp1";
            ((ToolStripMenuItem)menu.Items["mnuHelp"]).DropDownItems.Add(separator);

            // Меню "Помощь" -> "О программе"
            mnu = new ToolStripMenuItem();
            mnu.Name = "mnuHelpAbout";
            mnu.Text = "О программе";
            mnu.Image = NGK.MeasuringDeviceTech.Properties.Resources.faviconMy;
            mnu.ImageAlign = ContentAlignment.MiddleCenter;
            mnu.ImageScaling = ToolStripItemImageScaling.None;
            mnu.Click += new EventHandler(toolStripMenuItemAbout_Click);
            ((ToolStripMenuItem)menu.Items["mnuHelp"]).DropDownItems.Add(mnu);
            
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню "Настройки страницы"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemPageSettings_Click(object sender, EventArgs e)
        {
            try
            {
                _Printer.PageSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format("Невозможно выплнить операцию: {0}", ex.Message), 
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню "Настройки печати"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemPintSettings_Click(object sender, EventArgs e)
        {
            _Printer.FileToPrint = this.MakeFileToPrint();
            
            if (_Printer.FileToPrint != null)
            {
                try
                {
                    _Printer.PrintSettings();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, String.Format("Невозможно выплнить операцию: {0}", ex.Message),
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню "Предварительный просмотр"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemPrintPreview_Click(
            object sender, EventArgs e)
        {
            _Printer.FileToPrint = this.MakeFileToPrint();
            if (_Printer.FileToPrint != null)
            {
                try
                {
                    _Printer.PrintPreview();
                    // Поработали с потоком, зкрыли его
                    //_Printer.FileToPrint.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, String.Format("Невозможно выплнить операцию: {0}", ex.Message),
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню "Печать"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemPrint_Click(
            object sender, EventArgs e)
        {
            try
            {
                _Printer.Pint("Устройство НГК-КИП СМ(У)");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, String.Format("Невозможно выплнить операцию: {0}", ex.Message),
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Создаёт файл для печати состояния КИП (БИ)
        /// </summary>
        /// <returns>Поток-файл для печати, если устройства нет возвращается null</returns>
        private StreamReader MakeFileToPrint()
        {
            StreamReader file;
            String description;

            if (this._MeasuringDevice == null)
            {
                return file = null;
            }
            else
            {
                MemoryStream fs = new MemoryStream();
 
                switch (_MeasuringDevice.GetDeviceType())
                {
                    case TYPE_NGK_DEVICE.BI_BATTERY_POWER:
                        {
                            // Формтируем текст отчёта
                            MeasuringDeviceBatteryPower device =
                                (MeasuringDeviceBatteryPower)_MeasuringDevice.GetDevice();
                            description = device.ToString();
                            break; 
                        }
                    case TYPE_NGK_DEVICE.BI_MAIN_POWERED:
                        {
                            // Формтируем текст отчёта
                            MeasuringDeviceMainPower device =
                                (MeasuringDeviceMainPower)_MeasuringDevice.GetDevice();
                            description = device.ToString();
                            break; 
                        }
                    case TYPE_NGK_DEVICE.UNKNOWN_DEVICE:
                        { return file = null; }
                    default:
                        { return file = null; }
                }

                Char[] arrchar = description.ToCharArray();
                Byte[] arrbyte = System.Text.UnicodeEncoding.UTF8.GetBytes(arrchar);
                fs.Write(arrbyte, 0, arrbyte.Length);

                file = new StreamReader(fs);
            }

            return file;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню "Настройки" (настройки программы)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemSettings_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню "Сохранить настройки"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemConnectionSaveSettings_Click(
            object sender, EventArgs e)
        {
            this.SavePortSettings();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню "Проверить инициализацию устройства"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemDeviceVerifyInit_Click(
            object sender, EventArgs e)
        {
            this.DeviceVerityInit();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню "Идентифицировать устройство"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemDeviceIdentify_Click(
            object sender, EventArgs e)
        {
            this.GetTypeDevice();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчики меню "Синхронизировать время"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemDateTime_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(this, "Заглушка", "Внимание",
            //    MessageBoxButtons.OK, MessageBoxIcon.Information);
            OperationResult error;

            this._MeasuringDevice.Write_HR_DateTime(ref _Host, out error);

            if (error.Result != OPERATION_RESULT.OK)
            {
                MessageBox.Show(this, error.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик меню "Создать устройство"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuNewDevice_Click(object sender, EventArgs e)
        {
            this.CreateDevice(null);
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Инициализирует панель инструментов. Выполняется при запуске программы
        /// </summary>
        /// <param name="toolBar">Панель инструментов для инициализации</param>
        private void InitToolBar(ref System.Windows.Forms.ToolStrip toolStrip)
        {
            ToolStripButton button;

            toolStrip.Items.Clear();

            button = new ToolStripButton();
            button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            button.Image = global::NGK.MeasuringDeviceTech.Properties.Resources.FloppyDisk;
            button.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            button.ImageTransparentColor = System.Drawing.Color.Magenta;
            button.Name = "buttonSaveDevice";
            button.Size = new System.Drawing.Size(36, 36);
            button.Text = "Сохранить устройство КИП";
            button.Enabled = false;
            button.Click += new EventHandler(ToolStripButtonSaveDevice_Click);
            toolStrip.Items.Add(button);

            button = new ToolStripButton();
            button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            button.Image = global::NGK.MeasuringDeviceTech.Properties.Resources.IconAddDevice;
            button.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            button.ImageTransparentColor = System.Drawing.Color.Magenta;
            button.Name = "buttonNewDevice";
            button.Size = new System.Drawing.Size(36, 36);
            button.Text = "Создать устройство КИП";
            button.Enabled = false;
            button.Click += new EventHandler(ToolStripButtonNewDevice_Click);
            toolStrip.Items.Add(button);

            button = new ToolStripButton();
            button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            button.Image = global::NGK.MeasuringDeviceTech.Properties.Resources.IconDeleteDevice;
            button.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            button.ImageTransparentColor = System.Drawing.Color.Magenta;
            button.Name = "buttonDeleteDevice";
            button.Size = new System.Drawing.Size(36, 36);
            button.Text = "Закрыть устройство КИП";
            button.Enabled = false;
            button.Click += new EventHandler(ToolStripButtonDeleteDevice_Click);
            toolStrip.Items.Add(button);

            button = new ToolStripButton();
            button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            button.Image = global::NGK.MeasuringDeviceTech.Properties.Resources.IconDisconnect;
            button.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            button.ImageTransparentColor = System.Drawing.Color.Magenta;
            button.Name = "buttonConnection";
            button.Size = new System.Drawing.Size(36, 36);
            button.Text = "Подключиться";
            button.ToolTipText = "Подключиться";
            button.Click += new System.EventHandler(this.toolStripButtonConnection_Click);
            button.Enabled = false;
            toolStrip.Items.Add(button);

            button = new ToolStripButton();
            button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            button.Image = global::NGK.MeasuringDeviceTech.Properties.Resources.IconUnknownDevice;
            button.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            button.ImageTransparentColor = System.Drawing.Color.Magenta;
            button.Name = "buttonReadTypeOfDevice";
            button.Size = new System.Drawing.Size(36, 36);
            button.Text = "Прочитать тип устройства";
            button.Click += new EventHandler(toolStripButtonReadTypeOfDevice_Click);
            button.Enabled = false;
            toolStrip.Items.Add(button);

            button = new ToolStripButton();
            button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            button.Image = global::NGK.MeasuringDeviceTech.Properties.Resources.IconVerifyDevice;
            button.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            button.ImageTransparentColor = System.Drawing.Color.Magenta;
            button.Name = "buttonVerifyInitDevice";
            button.Size = new System.Drawing.Size(36, 36);
            button.Text = "Проверить инициализацию устройства";
            button.Click += new EventHandler(buttonVerifyInitDevice_Click);
            button.Enabled = false;
            toolStrip.Items.Add(button);

            button = new ToolStripButton();
            button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            button.Image = global::NGK.MeasuringDeviceTech.Properties.Resources.IconLoadFrom;
            button.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            button.ImageTransparentColor = System.Drawing.Color.Magenta;
            button.Name = "buttonReadDevice";
            button.Size = new System.Drawing.Size(36, 36);
            button.Text = "Прочитать устройство";
            button.Click += new System.EventHandler(this.toolStripButtonReadDevice_Click);
            button.Enabled = false;
            toolStrip.Items.Add(button);

            button = new ToolStripButton();
            button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            button.Image = global::NGK.MeasuringDeviceTech.Properties.Resources.IconLoadTo;
            button.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            button.ImageTransparentColor = System.Drawing.Color.Magenta;
            button.Name = "buttonWriteDevice";
            button.Size = new System.Drawing.Size(36, 36);
            button.Text = "Записать устройство";
            button.Click += new System.EventHandler(this.toolStripButtonWriteDevice_Click);
            button.Enabled = false;
            toolStrip.Items.Add(button);

            button = new ToolStripButton();
            button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            button.Image = global::NGK.MeasuringDeviceTech.Properties.Resources.history;
            button.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            button.ImageTransparentColor = System.Drawing.Color.Magenta;
            button.Name = "buttonSyncDateTime";
            button.Size = new System.Drawing.Size(36, 36);
            button.Text = "Синхронизировать время";
            button.Click += new System.EventHandler(this.toolStripButtonDateTime_Click);
            button.Enabled = false;
            toolStrip.Items.Add(button);

            button = new ToolStripButton();
            button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            button.Image = global::NGK.MeasuringDeviceTech.Properties.Resources.IconStart.ToBitmap();
            button.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            button.ImageTransparentColor = System.Drawing.Color.Magenta;
            button.Name = "buttonStartMonitor";
            button.Size = new System.Drawing.Size(36, 36);
            button.Text = "Старт монитора параметров";
            button.Click += new EventHandler(toolStripButtonStartMonitor_Click);
            button.Enabled = false;
            toolStrip.Items.Add(button);

            button = new ToolStripButton();
            button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            button.Image = global::NGK.MeasuringDeviceTech.Properties.Resources.IconStop.ToBitmap();
            button.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            button.ImageTransparentColor = System.Drawing.Color.Magenta;
            button.Name = "buttonStopMonitor";
            button.Size = new System.Drawing.Size(36, 36);
            button.Text = "Стоп монитора параметров";
            button.Click += new EventHandler(toolStripButtonStopMonitor_Click); 
            button.Enabled = false;
            toolStrip.Items.Add(button);
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик нажатия кнопки на панели инструментов "Стоп монитора параметров"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonStopMonitor_Click(object sender, EventArgs e)
        {
            this.StopMonitor();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик нажатия кнопки на панели инструментов "Старт монитора параметров"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonStartMonitor_Click(object sender, EventArgs e)
        {
            this.StartMonitor();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Проверяет идентификацию устройства КИП
        /// </summary>
        private void DeviceVerityInit()
        {
            Boolean init;
            DialogResult result;
            OperationResult error;

            this.Cursor = Cursors.WaitCursor;
            this.VerifyInitDevice(out init, out error);
            this.Cursor = Cursors.Default;

            if (error.Result == OPERATION_RESULT.OK)
            {
                if (init)
                {
                    MessageBox.Show(this, "Устройство инициализировано", "Сообщение",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    result = MessageBox.Show(this,
                        "Устройство не инициализировано. Инициализировать?",
                        "Сообщение",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    // Ининициализируем устройство
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        NGK.MeasuringDeviceTech.Forms.FormInitDevice frm =
                            new Forms.FormInitDevice(_MeasuringDevice.GetDeviceType());

                        result = frm.ShowDialog(this);

                        if (result == System.Windows.Forms.DialogResult.OK)
                        {
                            // Записываем в устройство серийный номер

                            this.Cursor = Cursors.WaitCursor;
                            _MeasuringDevice.Write_HR_SerialNumber(ref _Host,
                                frm.SerialNumber, out error);
                            this.Cursor = Cursors.Default;

                            if (error.Result != OPERATION_RESULT.OK)
                            {
                                MessageBox.Show(this,
                                    String.Format(
                                    "При записи серийного номера произошла ошибка: {0}",
                                    error.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                MessageBox.Show(this,
                                    "Cерийный номер успешно записан, устройство инициализировано",
                                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show(this,
                    String.Format("При выполении операции по сети произошла ошибка: {0}",
                    error.Message), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик нажатия кнопки проверить устройство. Устнановлен или нет
        /// серийный номер (инициализация устройства)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonVerifyInitDevice_Click(object sender, EventArgs e)
        {
            this.DeviceVerityInit();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Читает визитную карточку устройства и выводит её на экран
        /// </summary>
        private void GetTypeDevice()
        {
            OperationResult result;
            CallingCard deviceInfo;
            String _typeOfDevice;

            // Получаем данные об устройстве.
            if (_Host == null)
            {
                _Host =
                    new Modbus.OSIModel.ApplicationLayer.Master.Device(
                        "Network", _SerialPort);

                this.ReadCallingCard(out result, out deviceInfo);
                _Host = null;
            }
            else
            {
                this.ReadCallingCard(out result, out deviceInfo);
            }

            if (result.Result == OPERATION_RESULT.OK)
            {
                // Если визитную карточку удалось получить, и виртуальное 
                // устройство не создано, то предлагаем
                // создать не её основе устройство. В противном случае просто выводим информацию
                switch ((UInt16)deviceInfo.TypeOfDevice)
                {
                    case (UInt16)TYPE_NGK_DEVICE.UNKNOWN_DEVICE:
                        {
                            _typeOfDevice = "Неизвестное устройство";
                            break;
                        }
                    case (UInt16)TYPE_NGK_DEVICE.BI_BATTERY_POWER:
                        {
                            _typeOfDevice = "Устройство НГК-БИ(У)-01";
                            break;
                        }
                    case (UInt16)TYPE_NGK_DEVICE.BI_MAIN_POWERED:
                        {
                            _typeOfDevice = "Устройство НГК-БИ(У)-00";
                            break;
                        }
                    default:
                        {
                            throw new Exception("Данное устройство не поддерживается в данной версии ПО");
                        }
                }

                if (_MeasuringDevice == null)
                {
                    // Предлагаем создать устройство
                    DialogResult dlgres = MessageBox.Show(this,
                        String.Format("Тип устройства: {0} \n Версия ПО: {1} \n Версия аппаратуры: {2} \n Серийный номер: {3} \n Создать устройство?",
                        _typeOfDevice, deviceInfo.SofwareVersion, deviceInfo.HardwareVersion, deviceInfo.SerialNumber),
                        "Устройство НГК", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (dlgres == System.Windows.Forms.DialogResult.Yes)
                    {
                        // Создаём новое устройство
                        IMeasuringDevice device;

                        // Создаём экземпляр
                        switch (deviceInfo.TypeOfDevice)
                        {
                            case TYPE_NGK_DEVICE.BI_BATTERY_POWER:
                                {
                                    //MessageBox.Show(this, "Данный тип БИ не поддерживается ПО",
                                    //    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    //return;
                                    MeasuringDeviceBatteryPower dv = new MeasuringDeviceBatteryPower();
                                    device = (IMeasuringDevice)dv;
                                    break;
                                }
                            case TYPE_NGK_DEVICE.BI_MAIN_POWERED:
                                {
                                    MeasuringDeviceMainPower dv = new MeasuringDeviceMainPower();
                                    device = (IMeasuringDevice)dv;
                                    break;
                                }
                            default:
                                {
                                    MessageBox.Show(this, "Данный тип НГК-БИ не поддерживается ПО",
                                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                        }

                        CreateDevice(device);
                    }
                }
                else
                {
                    // Только выводим информацию об устройстве
                    MessageBox.Show(this,
                        String.Format("Тип устройства: {0} \n Версия ПО: {1} \n Версия аппаратуры: {2} \n Серийный номер: {3}",
                        _typeOfDevice, deviceInfo.SofwareVersion, deviceInfo.HardwareVersion, deviceInfo.SerialNumber),
                        "Устройство НГК", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                // При выполении запроса возникла ошибка
                MessageBox.Show(this,
                    String.Format("Неудалось идентифицировать устройство. Ошибка: {0}", result.Message),
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик нажатия кнопки на панели инструментов "Прочитать тип устройства"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void toolStripButtonReadTypeOfDevice_Click(object sender, EventArgs e)
        {
            this.GetTypeDevice();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик нажатия кнопки "Синхронизировать время"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonDateTime_Click(object sender, EventArgs e)
        {
            OperationResult error;
            
            this.Cursor = Cursors.WaitCursor;
            this._MeasuringDevice.Write_HR_DateTime(ref _Host, out error);
            this.Cursor = Cursors.Default;

            if (error.Result != OPERATION_RESULT.OK)
            {
                MessageBox.Show(this, error.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(this, "Дата и время синхронизированы", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработичик события нажатия кнопки "Удалить устройство КИП"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButtonDeleteDevice_Click(object sender, EventArgs e)
        {
            this.DeleteDevice();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события нажатия кнопки "Создать новое устройство КИП"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButtonNewDevice_Click(object sender, EventArgs e)
        {
            this.CreateDevice(null);
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события нажатия кнопки "Сохранить устройство КИП"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripButtonSaveDevice_Click(object sender, EventArgs e)
        {
            this.SaveFile();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Инициализирует контекстные меню. Выполняется при запуске программы.
        /// </summary>
        private void InitContextMenu()
        {
            ToolStripMenuItem mnuItem;

            // Настраиваем контектсное меню для элемента дерева "Подключение"
            mnuItem = new ToolStripMenuItem();
            mnuItem.Name = "toolStripMenuItemSaveConnection";
            mnuItem.Size = new System.Drawing.Size(152, 24);
            mnuItem.Text = "Сохранить";
            mnuItem.Click += new System.EventHandler(this.toolStripMenuItemSaveConnection_Click);

            //this._ContexMenuConnection = new System.Windows.Forms.ContextMenuStrip(this.components); 
            this._ContexMenuConnection = new ContextMenuStrip();
            this._ContexMenuConnection.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            mnuItem});
            this._ContexMenuConnection.Name = "contextMenuStripConnection";
            this._ContexMenuConnection.Size = new System.Drawing.Size(153, 28);

            this.treeViewMain.Nodes["NodeRoot"].Nodes["NodeConnection"].ContextMenuStrip = _ContexMenuConnection;

            // Настраиваем контекстное меню для элемента "КИП"
            this._ContexMenuDevice = new ContextMenuStrip();
            this._ContexMenuDevice.Name = "contextMenuStripDevice";
            this._ContexMenuDevice.Size = new System.Drawing.Size(153, 28);

            mnuItem = new ToolStripMenuItem();
            mnuItem.Name = "toolStripMenuItemReadDevice";
            mnuItem.Size = new System.Drawing.Size(152, 24);
            mnuItem.Text = "Прочитать параметры";
            mnuItem.Click += new System.EventHandler(this.toolStripMenuItemReadDevice_Click);
            this._ContexMenuDevice.Items.Add(mnuItem);

            mnuItem = new ToolStripMenuItem();
            mnuItem.Name = "toolStripMenuItemWriteDevice";
            mnuItem.Size = new System.Drawing.Size(152, 24);
            mnuItem.Text = "Записать параметры";
            mnuItem.Click += new System.EventHandler(this.toolStripMenuItemWriteDevice_Click);
            this._ContexMenuDevice.Items.Add(mnuItem);

            this.treeViewMain.Nodes["NodeRoot"].Nodes["NodeMeasuringDevice"].ContextMenuStrip = _ContexMenuDevice;

            // Настраиваем контекстное меню для параметра КИП
            this._ContexMenuParameter = new ContextMenuStrip();
            this._ContexMenuParameter.Name = "contextMenuStripParameter";
            this._ContexMenuParameter.Size = new System.Drawing.Size(153, 28);

            mnuItem = new ToolStripMenuItem();
            mnuItem.Name = "toolStripMenuItemReadParameter";
            mnuItem.Size = new System.Drawing.Size(152, 24);
            mnuItem.Text = "Прочитать параметр";
            mnuItem.Click += new System.EventHandler(this.toolStripMenuItemGetParametr_Click);
            this._ContexMenuParameter.Items.Add(mnuItem);

            mnuItem = new ToolStripMenuItem();
            mnuItem.Name = "toolStripMenuItemWriteParameter";
            mnuItem.Size = new System.Drawing.Size(152, 24);
            mnuItem.Text = "Записать параметр";
            mnuItem.Click += new System.EventHandler(this.toolStripMenuItemSetParametr_Click);
            this._ContexMenuParameter.Items.Add(mnuItem);

            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Инициализирует строку состояния формы
        /// </summary>
        /// <param name="statusBar"></param>
        private void InitStatusBar(ref StatusStrip statusBar)
        {
            ToolStripStatusLabel label;

            statusBar.Items.Clear();

            label = new ToolStripStatusLabel();
            label.Alignment = ToolStripItemAlignment.Left;
            label.AutoSize = true;
            label.AutoToolTip = true;
            label.BorderSides = ToolStripStatusLabelBorderSides.All;
            label.BorderStyle = Border3DStyle.Flat;
            label.DisplayStyle = ToolStripItemDisplayStyle.Text;
            label.Enabled = true;
            label.Name = "Date";
            label.Text = String.Empty;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.ToolTipText = "Системная дата";
            statusBar.Items.Add(label);

            label = new ToolStripStatusLabel();
            label.Alignment = ToolStripItemAlignment.Left;
            label.AutoSize = true;
            label.AutoToolTip = true;
            label.BorderSides = ToolStripStatusLabelBorderSides.All;
            label.BorderStyle = Border3DStyle.Flat;
            label.DisplayStyle = ToolStripItemDisplayStyle.Text;
            label.Enabled = true;
            label.Name = "Time";
            label.Text = String.Empty;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.ToolTipText = "Системное время";
            statusBar.Items.Add(label);

            label = new ToolStripStatusLabel();
            label.Alignment = ToolStripItemAlignment.Left;
            label.AutoSize = true;
            label.AutoToolTip = true;
            label.BorderSides = ToolStripStatusLabelBorderSides.All;
            label.BorderStyle = Border3DStyle.Flat;
            label.DisplayStyle = ToolStripItemDisplayStyle.Text;
            label.Enabled = true;
            label.Name = "DateTimeUtc";
            label.Text = String.Empty;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.ToolTipText = "Системное время в формает utc: +0";
            statusBar.Items.Add(label);


            label = new ToolStripStatusLabel();
            label.Alignment = ToolStripItemAlignment.Left;
            label.AutoSize = true;
            label.AutoToolTip = true;
            label.BorderSides = ToolStripStatusLabelBorderSides.All;
            label.BorderStyle = Border3DStyle.Flat;
            label.DisplayStyle = ToolStripItemDisplayStyle.Text;
            label.Enabled = true;
            label.Name = "PathToFile";
            label.Text = String.Empty;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.ToolTipText = "Путь к открытому файлу КИП";
            label.Visible = false;
            statusBar.Items.Add(label);

            ToolStripProgressBar prgsBar;
            prgsBar = new ToolStripProgressBar();
            prgsBar.Name = "ProgressScanOperation";
            prgsBar.ToolTipText = "Индикатор процесса выполения операции мониторинга параметров устройства";
            prgsBar.Value = 0;
            prgsBar.Visible = false;
            statusBar.Items.Add(prgsBar);

            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Запускает мониторинг устройства БИ
        /// </summary>
        private void StartMonitor()
        {
            ToolStripMenuItem itemMenu;
            // Настрайваем элементы управления формы для режима мониторинга
            // Настраиваем элементы меню 
            itemMenu = (ToolStripMenuItem)this.menuStripMain.Items[MENU.mnuFile];
            itemMenu.Enabled = true;
            itemMenu.DropDownItems[MENU.mnuFileClose].Enabled = false;
            itemMenu.DropDownItems[MENU.mnuFileExit].Enabled = true;
            itemMenu.DropDownItems[MENU.mnuFileOpen].Enabled = false;
            itemMenu.DropDownItems[MENU.mnuFileSave].Enabled = false;
            itemMenu.DropDownItems[MENU.mnuFileSaveAs].Enabled = false;

            itemMenu = (ToolStripMenuItem)this.menuStripMain.Items[MENU.mnuConnection];
            itemMenu.Enabled = false;

            itemMenu = (ToolStripMenuItem)this.menuStripMain.Items[MENU.mnuDevice];
            itemMenu.Enabled = true;
            itemMenu.DropDownItems[MENU.mnuDeviceDateTime].Enabled = false;
            itemMenu.DropDownItems[MENU.mnuDeviceIdentify].Enabled = false;
            itemMenu.DropDownItems[MENU.mnuDeviceRead].Enabled = false;
            itemMenu.DropDownItems[MENU.mnuDeviceVerifyInit].Enabled = false;
            itemMenu.DropDownItems[MENU.mnuDeviceWrite].Enabled = false;
            
            // Настраиваем контекстные меню
            this._ContexMenuConnection.Enabled = false;
            this._ContexMenuDevice.Enabled = false;
            this._ContexMenuParameter.Enabled = false;

            // Настраиваем панель инструментов
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonConnection].Enabled = false;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonDeleteDevice].Enabled = false;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonNewDevice].Enabled = false;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonReadDevice].Enabled = false;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonReadTypeOfDevice].Enabled = false;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonSaveDevice].Enabled = false;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonStartMonitor].Enabled = false;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonStopMonitor].Enabled = true;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonSyncDateTime].Enabled = false;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonVerifyInitDevice].Enabled = false;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonWriteDevice].Enabled = false;

            // Настраиваем строку состояния
            ToolStripProgressBar bar = 
                (ToolStripProgressBar)this.statusStripMain.Items["ProgressScanOperation"];
            bar.Value = 0;
            bar.Visible = true;

            this.treeViewMain.Enabled = false;
            this.propertyGridMain.Enabled  = false;

            _BackgroundWorker = new BackgroundWorker();
            _BackgroundWorker.WorkerSupportsCancellation = true;
            _BackgroundWorker.WorkerReportsProgress = true;
            _BackgroundWorker.DoWork +=
                new DoWorkEventHandler(_BackgroundWorker_DoWork);
            _BackgroundWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(_BackgroundWorker_RunWorkerCompleted);
            _BackgroundWorker.ProgressChanged +=
                new ProgressChangedEventHandler(_BackgroundWorker_ProgressChanged);
            // Запускаем выполнение
            _BackgroundWorker.RunWorkerAsync();

            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события фонового потока для режима мониторинга
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _BackgroundWorker_ProgressChanged(
            object sender, 
            ProgressChangedEventArgs e)
        {
            ToolStripProgressBar bar = (ToolStripProgressBar)this.statusStripMain.Items["ProgressScanOperation"];
            bar.Value = e.ProgressPercentage;
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события выход из режима мониторинга
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _BackgroundWorker_RunWorkerCompleted(
            object sender, 
            RunWorkerCompletedEventArgs e)
        {
            ToolStripMenuItem itemMenu;
            // Настрайваем элементы управления формы для основного режима работы программы
            // Настраиваем элементы меню 
            itemMenu = (ToolStripMenuItem)this.menuStripMain.Items[MENU.mnuFile];
            itemMenu.Enabled = true;
            itemMenu.DropDownItems[MENU.mnuFileClose].Enabled = true;
            itemMenu.DropDownItems[MENU.mnuFileExit].Enabled = true;
            itemMenu.DropDownItems[MENU.mnuFileOpen].Enabled = true;
            itemMenu.DropDownItems[MENU.mnuFileSave].Enabled = true;
            itemMenu.DropDownItems[MENU.mnuFileSaveAs].Enabled = false;

            itemMenu = (ToolStripMenuItem)this.menuStripMain.Items[MENU.mnuConnection];
            itemMenu.Enabled = true;

            itemMenu = (ToolStripMenuItem)this.menuStripMain.Items[MENU.mnuDevice];
            itemMenu.Enabled = true;
            itemMenu.DropDownItems[MENU.mnuDeviceDateTime].Enabled = true;
            itemMenu.DropDownItems[MENU.mnuDeviceIdentify].Enabled = true;
            itemMenu.DropDownItems[MENU.mnuDeviceRead].Enabled = true;
            itemMenu.DropDownItems[MENU.mnuDeviceVerifyInit].Enabled = true;
            itemMenu.DropDownItems[MENU.mnuDeviceWrite].Enabled = true;

            // Настраиваем контекстные меню
            this._ContexMenuConnection.Enabled = true;
            this._ContexMenuDevice.Enabled = true;
            this._ContexMenuParameter.Enabled = true;

            // Настраиваем панель инструментов
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonConnection].Enabled = true;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonDeleteDevice].Enabled = true;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonNewDevice].Enabled = false;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonReadDevice].Enabled = true;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonReadTypeOfDevice].Enabled = true;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonSaveDevice].Enabled = true;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonStartMonitor].Enabled = true;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonStopMonitor].Enabled = false;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonSyncDateTime].Enabled = true;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonVerifyInitDevice].Enabled = true;
            this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonWriteDevice].Enabled = true;

            ToolStripProgressBar bar = (ToolStripProgressBar)this.statusStripMain.Items["ProgressScanOperation"];
            bar.Value = 0;
            bar.Visible = false;

            this.treeViewMain.Enabled = true;
            this.propertyGridMain.Enabled = true;


            if (e.Cancelled)
            {
                // Операция была отменена пользователем
            }
            else if (e.Error != null)
            {
                // Во время выполения операции произошла ошибка
                MessageBox.Show(this, 
                    String.Format("При обращении к устройству произошла ошибка: {0}", e.Error.Message), 
                    "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Операция завершена успешно
            }
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события выполнения мониторинга устройства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _BackgroundWorker_DoWork(
            object sender, 
            DoWorkEventArgs e)
        {
            OperationResult error;
            // Выполняем операцию в бесконечном цикле
            while (true)
            {
                // Проверяем была ли отменена фоновая операция
                if (_BackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    _MeasuringDevice.Read_DI_BattaryVoltageStatus(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(5);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_DI_CaseOpen(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(10);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_DI_CorrosionSensor1(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(15);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_DI_CorrosionSensor2(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(20);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_DI_CorrosionSensor3(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(25);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_DI_SupplyVoltageStatus(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(30);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_IR_BattaryVoltage(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(35);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_IR_Current_Channel_1(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(40);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_IR_Current_Channel_2(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(45);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    //_MeasuringDevice.Read_IR_DateTime(ref _Host, out error);

                    //if ((error.Result != OPERATION_RESULT.OK) &&
                    //    (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    //{
                    //    throw new Exception(error.Message);
                    //}
                    //else
                    //{
                    //    _BackgroundWorker.ReportProgress(50);
                    //}

                    //if (_BackgroundWorker.CancellationPending)
                    //{
                    //    e.Cancel = true;
                    //    return;
                    //}

                    _MeasuringDevice.Read_IR_ReferenceElectrodeDCCurrent(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(50);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_IR_ReferenceElectrodeACCurrent(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(50);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    DateTime dt;
                    _MeasuringDevice.Read_HR_DateTime(ref _Host, out error, out dt);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(50);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_IR_DepthOfCorrosionUSIKPST(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(55);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_IR_InducedVoltage(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(60);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_IR_InternalTemperatureSensor(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(65);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_IR_Polarization_current(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(70);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_IR_Polarization_potential(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(75);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_IR_Protective_current(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(80);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_IR_Protective_potential(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(85);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_IR_SpeedOfCorrosionUSIKPST(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(90);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_IR_StatusUSIKPST(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(95);
                    }

                    if (_BackgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    _MeasuringDevice.Read_IR_SupplyVoltage(ref _Host, out error);

                    if ((error.Result != OPERATION_RESULT.OK) &&
                        (error.Result != OPERATION_RESULT.INVALID_OPERATION))
                    {
                        throw new Exception(error.Message);
                    }
                    else
                    {
                        _BackgroundWorker.ReportProgress(100);
                    }
                }
            }
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Останавливает мониторинг данных
        /// </summary>
        private void StopMonitor()
        {
            if (_BackgroundWorker != null)
            {
                if (_BackgroundWorker.IsBusy)
                {
                    _BackgroundWorker.CancelAsync();
                }
            }
            return;
        }
        //----------------------------------------------------------------------------
    }
    //================================================================================
}
//====================================================================================
// End Of File