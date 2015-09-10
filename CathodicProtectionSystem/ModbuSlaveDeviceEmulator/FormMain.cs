using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
//
using Modbus.OSIModel.DataLinkLayer.Slave;
using Modbus.OSIModel.DataLinkLayer.Slave.RTU.ComPort;
using Modbus.OSIModel.ApplicationLayer.Slave;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;

//===================================================================================
namespace ModbuSlaveDevicesNetwork
{
    //===============================================================================
    public partial class FormMain : Form
    {
        //---------------------------------------------------------------------------
        #region Fields And Properties
        //---------------------------------------------------------------------------
        /// <summary>
        /// Объект для физического подключения к сети
        /// </summary>
        private ComPort _SerialPort;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Контроллер сети modbus
        /// </summary>
        private NetworkController _Network;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Для теста
        /// </summary>
        public NetworkController Network
        {
            get { return _Network; }
            set { _Network = value; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Путь к текущему файлу конфигурации сети.
        /// </summary>
        private String _PathToFileNetworkConfig;
        //---------------------------------------------------------------------------
        private BindingSource _BindingSourceDevices;
        //---------------------------------------------------------------------------
        private BindingSource _BindingSourceFile;
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Constructors
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            this.Load += new EventHandler(EventHandler_FormMain_Load);
            this.FormClosed += new FormClosedEventHandler(
                EventHandler_FormMain_FormClosed);
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Event Handlers For FormMain
        //---------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события загрузки формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_FormMain_Load(object sender, EventArgs e)
        {
            FormMain form = (FormMain)sender;

            // Инициализируем статусную строку
            this.InitStatusBar(ref this._StatusStripMainWindow);

            this._TreeViewNetwork.AfterCheck += 
                new TreeViewEventHandler(EventHandler_TreeViewNetwork_AfterCheck);

            this._BindingSourceDevices = new BindingSource();
            this._BindingSourceDevices.CurrentChanged += new EventHandler(EventHandler_BindingSourceDevices_CurrentChanged);
            this._BindingSourceFile = new BindingSource();

            this.InitGrids();
            
            // Загрузаем настройки приложения
            this.LoadAppConfiguration();

            

            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_BindingSourceDevices_CurrentChanged(object sender, EventArgs e)
        {
            BindingSource bs = (BindingSource)sender;
            Device device = (Device)bs.Current;

            if (device != null)
            {
                device.MasterChangedCoils += 
                    new EventHandler(EventHadler_Device_MasterChangedData);
                device.MasterChangedHoldingRegisters += 
                    new EventHandler(EventHadler_Device_MasterChangedData);
            }
            return;
        }
        //---------------------------------------------------------------------------
        private void EventHadler_Device_MasterChangedData(object sender, EventArgs e)
        {
            // При записи данных мастером в текущее отображаемое устройство
            // обновляем контекст
            this._BindingSourceDevices.ResetCurrentItem();
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_FormMain_FormClosed(object sender, 
            FormClosedEventArgs e)
        {
            if (this._Network != null)
            {
                this._Network.Stop();
            }
            if (this._SerialPort != null)
            {
                this._SerialPort.Dispose();
            }
            return;
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region ToolStripMenuItem
        //---------------------------------------------------------------------------
        /// <summary>
        /// Обработчик выбора меню настройки COM-порта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_ToolStripMenuItemNetworkSerialPort_Click(
            object sender, EventArgs e)
        {
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события выбора меню "Создать"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_ToolStripMenuItemNewFile_Click(
            object sender, EventArgs e)
        {
            // Создаём новую сеть
            // Если текущая сеть != null, то спрашиваем пользователя хочет ли он
            // сохранить текущую конфигурацию.
            NetworkController network = new NetworkController();
            this.SetCurrentNetwork(network);
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_ToolStripMenuItemFileSave_Click(
            object sender, EventArgs e)
        {
            // Получаем путь к файлу конфигурации сети
            String pathToFile;
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);
            pathToFile = configuration.AppSettings.Settings["PathToNetworkConfigFile"].Value;
            this._Network.SaveToXmlFile(pathToFile);
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события выбора меню "Сохранить как"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_ToolStripMenuItemFileSaveAs_Click(
            object sender, EventArgs e)
        {
            // Сохраняем текущую конфигурацию сети в указанный пользователем
            // файл
            //...
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Сохранить конфигурацию сети как";
            dialog.FileName = "Untitled";
            dialog.AddExtension = true;
            dialog.DefaultExt = "xml";
            dialog.OverwritePrompt = true;
            dialog.FilterIndex = 1;
            dialog.Filter = "Xml Documents (*.xml)|*.xml";

            DialogResult result = dialog.ShowDialog(this);
            
            if (result == DialogResult.OK)
            {
                this._Network.SaveToXmlFile(dialog.FileName);
            }

            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_ToolStripMenuItemNetworkEdit_Click(
            object sender, EventArgs e)
        {
            this.EditNetwork(this._Network);
            return;
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region EventHandlers For TreeViewNetwork
        //---------------------------------------------------------------------------
        /// <summary>
        /// Обработчикс события 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_TreeViewNetwork_AfterSelect(
            object sender, TreeViewEventArgs e)
        {
            Device device;
            TreeView control = (TreeView)sender;

            if ((e.Action == TreeViewAction.ByKeyboard) ||
                (e.Action == TreeViewAction.ByMouse) || 
                (e.Action == TreeViewAction.Unknown))
            {
                if (e.Node.Equals(control.TopNode))
                {
                    this._TabControlDeviceProperties.Visible = false;
                }
                else if (e.Node.Tag is Device)
                {
                    device = e.Node.Tag as Device;
                    this._TabControlDeviceProperties.Visible = true;

                    this._TabControlDeviceProperties.Enabled = true;
                    this._BindingSourceDevices.Position = this._BindingSourceDevices.IndexOf(device);
                    Object obj = this._DataGridViewHoldingRegisters.DataSource;
                }
                else
                {
                    this._TabControlDeviceProperties.Enabled = false;
                }
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_TreeViewNetwork_AfterCheck(object sender, TreeViewEventArgs e)
        {
            Device device;
            TreeView control = (TreeView)sender;

            if ((e.Action == TreeViewAction.ByKeyboard) || (e.Action == TreeViewAction.ByMouse))
            {
                if (control.TopNode.Equals(e.Node))
                {
                    // Если установлен или снят корневой элемент, то выполняем
                    // действия со всем списком устройств
                    foreach (TreeNode node in control.TopNode.Nodes)
                    {
                        device = (Device)node.Tag;

                        if (e.Node.Checked)
                        {
                            node.Checked = true;
                            device.Start();
                        }
                        else
                        {
                            node.Checked = false;
                            device.Stop();
                        }
                    }
                }
                else
                {
                    // Если выбран узел конкретного устройства, по выполняем
                    // действия по отношению к нему
                    device = (Device)e.Node.Tag;

                    if (e.Node.Checked)
                    {
                        device.Start();
                    }
                    else
                    {
                        device.Stop();
                    }
                }
            }
            else
            { }
            return;
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Methods
        //---------------------------------------------------------------------------
        /// <summary>
        /// Инициализирует гриды
        /// </summary>
        private void InitGrids()
        {
            DataGridViewCellStyle cellstyle;
            DataGridViewCellStyle headercellstyle;
            Font font;
            DataGridViewTextBoxColumn txtColumn;
            DataGridViewCheckBoxColumn chkColumn;

            cellstyle = new DataGridViewCellStyle();
            cellstyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //cellstyle.Format = "X4";

            headercellstyle = new DataGridViewCellStyle();
            headercellstyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            font = this._DataGridViewCoils.ColumnHeadersDefaultCellStyle.Font;
            headercellstyle.Font = new Font(font, FontStyle.Bold);

            #region DataGridViewCoils
            // Загружаем данные устройства
            this._DataGridViewCoils.AutoGenerateColumns = false;
            this._DataGridViewCoils.DataSource = null;
            this._DataGridViewCoils.DataMember = "Coils";
            this._DataGridViewCoils.AllowUserToAddRows = false;
            this._DataGridViewCoils.AllowUserToDeleteRows = false;

            txtColumn = new DataGridViewTextBoxColumn();
            txtColumn.Name = "Column_Coil_Address";
            txtColumn.ReadOnly = true;
            txtColumn.DefaultCellStyle = cellstyle;
            txtColumn.HeaderText = "Адрес";
            txtColumn.HeaderCell.Style = headercellstyle;
            txtColumn.DataPropertyName = "Address";
            this._DataGridViewCoils.Columns.Add(txtColumn);

            chkColumn = new DataGridViewCheckBoxColumn();
            chkColumn.Name = "Column_Coil_Value";
            chkColumn.ReadOnly = false;
            chkColumn.DefaultCellStyle = cellstyle;
            chkColumn.HeaderText = "Значение";
            chkColumn.HeaderCell.Style = headercellstyle;
            chkColumn.DataPropertyName = "Value";
            this._DataGridViewCoils.Columns.Add(chkColumn);

            txtColumn = new DataGridViewTextBoxColumn();
            txtColumn.Name = "Column_Coil_Description";
            txtColumn.DataPropertyName = "Description";
            txtColumn.ReadOnly = false;
            //column.DefaultCellStyle = cellstyle;
            //column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            txtColumn.HeaderText = "Примечания";
            txtColumn.HeaderCell.Style = headercellstyle;
            this._DataGridViewCoils.Columns.Add(txtColumn);
            #endregion

            #region DataGridViewDiscretesInputs
            this._DataGridViewDiscretesInputs.AutoGenerateColumns = false;
            this._DataGridViewDiscretesInputs.DataSource = null;
            this._DataGridViewDiscretesInputs.DataMember = "DiscretesInputs";
            this._DataGridViewDiscretesInputs.AllowUserToAddRows = false;
            this._DataGridViewDiscretesInputs.AllowUserToDeleteRows = false;


            txtColumn = new DataGridViewTextBoxColumn();
            txtColumn.Name = "Column_DiscretesInputs_Address";
            txtColumn.DataPropertyName = "Address";
            txtColumn.ReadOnly = true;
            txtColumn.DefaultCellStyle = cellstyle;
            txtColumn.HeaderText = "Адрес";
            txtColumn.HeaderCell.Style = headercellstyle;
            this._DataGridViewDiscretesInputs.Columns.Add(txtColumn);


            chkColumn = new DataGridViewCheckBoxColumn();
            chkColumn.Name = "Column_DiscretesInputs_Value";
            chkColumn.ReadOnly = false;
            chkColumn.DefaultCellStyle = cellstyle;
            chkColumn.HeaderText = "Значение";
            chkColumn.HeaderCell.Style = headercellstyle;
            chkColumn.DataPropertyName = "Value";
            this._DataGridViewDiscretesInputs.Columns.Add(chkColumn);

            txtColumn = new DataGridViewTextBoxColumn();
            txtColumn.Name = "Column_DiscretesInputs_Description";
            txtColumn.DataPropertyName = "Description";
            txtColumn.ReadOnly = false;
            //column.DefaultCellStyle = cellstyle;
            //column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            txtColumn.HeaderText = "Примечания";
            txtColumn.HeaderCell.Style = headercellstyle;
            this._DataGridViewDiscretesInputs.Columns.Add(txtColumn);
            #endregion

            #region DataGridViewHoldingRegisters
            this._DataGridViewHoldingRegisters.AutoGenerateColumns = false;
            this._DataGridViewHoldingRegisters.DataSource = null;
            //this._DataGridViewHoldingRegisters.DataSource = device;
            this._DataGridViewHoldingRegisters.DataMember = "HoldingRegisters";
            this._DataGridViewHoldingRegisters.AllowUserToAddRows = false;
            this._DataGridViewHoldingRegisters.AllowUserToDeleteRows = false;

            txtColumn = new DataGridViewTextBoxColumn();
            txtColumn.Name = "Column_HoldingRegisters_Address";
            txtColumn.DataPropertyName = "Address";
            txtColumn.ReadOnly = true;
            txtColumn.DefaultCellStyle = cellstyle;
            txtColumn.HeaderText = "Адрес";
            txtColumn.HeaderCell.Style = headercellstyle;
            this._DataGridViewHoldingRegisters.Columns.Add(txtColumn);

            txtColumn = new DataGridViewTextBoxColumn();
            txtColumn.Name = "Column_HoldingRegisters_Description";
            txtColumn.DataPropertyName = "Value";
            txtColumn.ReadOnly = false;
            txtColumn.DefaultCellStyle = cellstyle;
            txtColumn.HeaderText = "Значение";
            txtColumn.HeaderCell.Style = headercellstyle;
            this._DataGridViewHoldingRegisters.Columns.Add(txtColumn);

            txtColumn = new DataGridViewTextBoxColumn();
            txtColumn.Name = "Column_HoldingRegisters_Description";
            txtColumn.DataPropertyName = "Description";
            txtColumn.ReadOnly = false;
            //column.DefaultCellStyle = cellstyle;
            //column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            txtColumn.HeaderText = "Примечания";
            txtColumn.HeaderCell.Style = headercellstyle;
            this._DataGridViewHoldingRegisters.Columns.Add(txtColumn);
            #endregion

            #region DataGridViewInputRegisters
            this._DataGridViewInputRegisters.AutoGenerateColumns = false;
            this._DataGridViewInputRegisters.DataSource = null;
            //this._DataGridViewInputRegisters.DataSource = device;
            this._DataGridViewInputRegisters.DataMember = "InputRegisters";
            this._DataGridViewInputRegisters.AllowUserToAddRows = false;
            this._DataGridViewInputRegisters.AllowUserToDeleteRows = false;


            txtColumn = new DataGridViewTextBoxColumn();
            txtColumn.Name = "Column_InputRegisters_Address";
            txtColumn.DataPropertyName = "Address";
            txtColumn.ReadOnly = true;
            txtColumn.DefaultCellStyle = cellstyle;
            txtColumn.HeaderText = "Адрес";
            txtColumn.HeaderCell.Style = headercellstyle;
            this._DataGridViewInputRegisters.Columns.Add(txtColumn);

            txtColumn = new DataGridViewTextBoxColumn();
            txtColumn.Name = "Column_InputRegisters_Value";
            txtColumn.DataPropertyName = "Value";
            txtColumn.ReadOnly = false;
            txtColumn.DefaultCellStyle = cellstyle;
            txtColumn.HeaderText = "Значение";
            txtColumn.HeaderCell.Style = headercellstyle;
            this._DataGridViewInputRegisters.Columns.Add(txtColumn);

            txtColumn = new DataGridViewTextBoxColumn();
            txtColumn.Name = "Column_InputRegisters_Description";
            txtColumn.DataPropertyName = "Description";
            txtColumn.ReadOnly = false;
            //column.DefaultCellStyle = cellstyle;
            //column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            txtColumn.HeaderText = "Примечания";
            txtColumn.HeaderCell.Style = headercellstyle;
            this._DataGridViewInputRegisters.Columns.Add(txtColumn);

            #endregion

            #region DataGridViewFiles
            this._DataGridViewFiles.AutoGenerateColumns = false;
            this._DataGridViewFiles.DataSource = null;
            this._DataGridViewFiles.DataMember = "Files";
            this._DataGridViewFiles.AllowUserToAddRows = false;
            this._DataGridViewFiles.AllowUserToDeleteRows = false;
            this._DataGridViewFiles.SelectionChanged += new EventHandler(EventHandler_DataGridViewFiles_SelectionChanged);

            txtColumn = new DataGridViewTextBoxColumn();
            txtColumn.Name = "Column_Files_Description";
            txtColumn.DataPropertyName = "Number";
            txtColumn.ReadOnly = true;
            txtColumn.DefaultCellStyle = cellstyle;
            txtColumn.HeaderText = "Номер файла";
            txtColumn.HeaderCell.Style = headercellstyle;
            this._DataGridViewFiles.Columns.Add(txtColumn);

            txtColumn = new DataGridViewTextBoxColumn();
            txtColumn.Name = "Column_Files_Description";
            txtColumn.DataPropertyName = "Description";
            txtColumn.ReadOnly = false;
            //column.DefaultCellStyle = cellstyle;
            //column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            txtColumn.HeaderText = "Примечания";
            txtColumn.HeaderCell.Style = headercellstyle;
            this._DataGridViewFiles.Columns.Add(txtColumn);

            #endregion

            #region DataGridViewRecords
            this._DataGridViewRecords.AutoGenerateColumns = false;
            this._DataGridViewRecords.DataSource = null;
            this._DataGridViewRecords.DataMember = "Records";
            this._DataGridViewRecords.AllowUserToAddRows = false;
            this._DataGridViewRecords.AllowUserToDeleteRows = false;

            txtColumn = new DataGridViewTextBoxColumn();
            txtColumn.Name = "Column_Records_Address";
            txtColumn.DataPropertyName = "Address";
            txtColumn.ReadOnly = true;
            txtColumn.DefaultCellStyle = cellstyle;
            txtColumn.HeaderText = "Номер записи";
            txtColumn.HeaderCell.Style = headercellstyle;
            this._DataGridViewRecords.Columns.Add(txtColumn);

            txtColumn = new DataGridViewTextBoxColumn();
            txtColumn.Name = "Column_Records_Value";
            txtColumn.DataPropertyName = "Value";
            txtColumn.ReadOnly = false;
            txtColumn.DefaultCellStyle = cellstyle;
            txtColumn.HeaderText = "Значение";
            txtColumn.HeaderCell.Style = headercellstyle;
            this._DataGridViewRecords.Columns.Add(txtColumn);

            txtColumn = new DataGridViewTextBoxColumn();
            txtColumn.Name = "Column_Records_Description";
            txtColumn.DataPropertyName = "Description";
            txtColumn.ReadOnly = false;
            //column.DefaultCellStyle = cellstyle;
            //column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            txtColumn.HeaderText = "Примечания";
            txtColumn.HeaderCell.Style = headercellstyle;
            this._DataGridViewRecords.Columns.Add(txtColumn);

            #endregion

            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_DataGridViewFiles_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView control = (DataGridView)sender;
            CurrencyManager manager = 
                control.BindingContext[control.DataSource, control.DataMember] as CurrencyManager;

            if (manager != null)
            {
                if (manager.Count != 0)
                {
                    File file = (File)manager.Current;
                    this._BindingSourceFile = new BindingSource(file, String.Empty);

                    this._DataGridViewRecords.DataSource = this._BindingSourceFile;
                    this._DataGridViewRecords.DataMember = "Records";
                }
                else
                {
                    this._DataGridViewRecords.DataSource = null;
                }
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события изменения конфигурации сети
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_Network_DevicesListWasChanged(object sender, EventArgs e)
        {
            //NetworkController network = (NetworkController)sender;
            // Обновляем элементы окна
            this.ShowNetwork(ref this._TreeViewNetwork, ref this._Network);
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Метод отображает устройства сети в элементе control;
        /// </summary>
        /// <param name="control">Элемент для отображения списка устройств в сети</param>
        /// <param name="network">Сеть Modbus</param>
        private void ShowNetwork(ref TreeView control, ref NetworkController network)
        {
            TreeNode node;

            if (control == null)
            {
                return;
            }
            else
            {
                control.Nodes.Clear();

                if (network == null)
                {
                    control.Enabled = false;
                }
                else
                {
                    control.Enabled = true;
                    control.ShowNodeToolTips = true;

                    node = new TreeNode();
                    node.Name = "NodeRoot";
                    node.Text = network.NetworkName;
                    node.ToolTipText = "Наименование сети Modbus";
                    control.Nodes.Add(node);
                    control.TopNode = node;

                    if (network.Devices.Count > 0)
                    {
                        control.TopNode.Checked = true;

                        foreach (Device device in network.Devices)
                        {
                            node = new TreeNode();
                            node.Name = "Node 0x" + device.Address.ToString("X2");
                            node.Text = "Устройство 0x" + device.Address.ToString("X2");
                            node.ToolTipText = device.Description;
                            if (device.Status == Common.Controlling.Status.Running)
                            {
                                node.Checked = true;
                            }
                            else
                            {
                                // если есть хотя бы одно остановленное устройво
                                // то снимаем галочку с корневого элемента
                                control.TopNode.Checked = false;
                            }
                            node.Tag = device;
                            control.TopNode.Nodes.Add(node);
                        }
                    }
                    else
                    {
                        control.TopNode.Checked = false;
                    }
                    control.ExpandAll();
                    control.SelectedNode = control.TopNode;
                }
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Метод запускает форму для редактирования сети Modbus
        /// </summary>
        /// <param name="network">Сеть Modbus</param>
        private void EditNetwork(NetworkController network)
        {
            DialogResult result;

            if (this._Network != null)
            {
                if (this._Network.Connection != null)
                {
                    if (this._Network.Connection.IsOpen)
                    {
                        result = MessageBox.Show(this, 
                            "Соединение активно, остановить и продолжить редактирование сети ?",
                            "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            this._Network.Stop(); // Останавливаем сетевой контроллер                           
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                // Вызываем форму для редактирования сети
                Modbus.OSIModel.ApplicationLayer.Slave.Dialogs.EditNetworkControllerDialog dialog =
                    new Modbus.OSIModel.ApplicationLayer.Slave.Dialogs.EditNetworkControllerDialog();
                dialog.Network = this._Network;
                result = dialog.ShowDialog();

                this.ShowNetwork(ref this._TreeViewNetwork, ref this._Network);

            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Метод вызывается при старте ПО, ищет xml-файл конфигурации, проверяет
        /// его по xsd-схеме и если всё нормально создаёт сеть. Если не удалось
        /// произвести данную операцию, пользователю предлагается создать новую
        /// сеть или завершить программу
        /// </summary>
        /// <param name="pathToXmlConfigFile">Путь к файлу конфигурации сети</param>
        private NetworkController LoadNetworkConfiguration(String pathToXmlConfigFile)
        {
            NetworkController network = null;
            string path = pathToXmlConfigFile;

            try
            {
                network = NetworkController.Create(path, Application.StartupPath + @"\config.xsd");
            }
            catch (Exception ex)
            {
                DialogResult result = MessageBox.Show(this, "При загрузке файла конфигурации сети возникла ошибка: " + 
                    ex.Message + " Создать новую сеть или выйти из приложения?",
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                
                if (result == DialogResult.Yes)
                {
                    // Создаём новую пустую сеть
                    network = new NetworkController();
                }
                else
                {
                    // Завершаем приложение
                    Application.Exit();
                }
            }
            return network;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Необходимо подключить сборку System.Configuration</remarks>
        private void LoadAppConfiguration()
        {
            // Получаем настройки из файла конфигурации приложения
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(
                    ConfigurationUserLevel.None);

                try
                {
                    this._PathToFileNetworkConfig = config.AppSettings.Settings["PathToNetworkConfigFile"].Value;
                    
                    if (this._PathToFileNetworkConfig == String.Empty)
                    {
                        // Используется каталог приложения (по умолчанию)
                        this._PathToFileNetworkConfig = Application.StartupPath + @"\config.xml";
                        
                    }
                }
                catch
                {
                    MessageBox.Show(this, "Не удалось получить путь к файлу конфигурации сети из файла конфигурации прложения. " +
                        "Будет использован поиск файла конфигурации по умолчанию",
                        "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Используется каталог приложения (по умолчанию)
                    this._PathToFileNetworkConfig = Application.StartupPath + @"\config.xml";
                }

                // Создаём объект для подключения
                try
                {
                    String portName = config.AppSettings.Settings["PortName"].Value;
                    if (portName == String.Empty)
                    {
                        this._SerialPort = null;
                    }
                    else
                    {
                        int baudRate = Int32.Parse(config.AppSettings.Settings["BaudRate"].Value);
                        System.IO.Ports.Parity parity =
                            (System.IO.Ports.Parity)Enum.Parse(typeof(System.IO.Ports.Parity),
                            config.AppSettings.Settings["Parity"].Value, true);
                        int dataBits = Int32.Parse(config.AppSettings.Settings["DataBits"].Value);
                        System.IO.Ports.StopBits stopBits = (System.IO.Ports.StopBits)Enum.Parse(
                            typeof(System.IO.Ports.StopBits),
                            config.AppSettings.Settings["StopBits"].Value, true);
                        this._SerialPort = new ComPort(portName, baudRate, parity, dataBits, stopBits);

                        this._ToolStripButtonStart.Enabled = true;
                        this._ToolStripButtonStop.Enabled = false;
                    }
                }
                catch
                {
                    MessageBox.Show(this, "Порт подключения к сети не создан, неверные настройки в файле конфигурации приложения" +
                        "Приложение будет закрыто",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this._SerialPort = null;
                    Application.Exit();
                }

            }
            catch
            {
                MessageBox.Show(this, "Не удалось получить настройки из Файла конфигурации приложения",
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this._SerialPort = null;
                // Используется каталог приложения (по умолчанию)
                this._PathToFileNetworkConfig = Application.StartupPath + @"\config.xml";
            }
            // Cоздаём сеть
            NetworkController network = this.LoadNetworkConfiguration(this._PathToFileNetworkConfig);
            this.SetCurrentNetwork(network);
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Сохраняет текущие настройки приложения 
        /// </summary>
        private void SaveAppConfiguration()
        {
            System.Configuration.Configuration config =
                System.Configuration.ConfigurationManager.OpenExeConfiguration(
                System.Configuration.ConfigurationUserLevel.None);

            //config.AppSettings.Settings["PathToNetworkConfigFile"].Value = "Путь к файлу";

            if (this._SerialPort == null)
            {
                config.AppSettings.Settings["PortName"].Value = String.Empty;
            }
            else
            {
                config.AppSettings.Settings["PortName"].Value = this._SerialPort.SerialPort.PortName;
                config.AppSettings.Settings["BaudRate"].Value = this._SerialPort.SerialPort.BaudRate.ToString();
                config.AppSettings.Settings["Parity"].Value = this._SerialPort.SerialPort.Parity.ToString();
                config.AppSettings.Settings["DataBits"].Value = this._SerialPort.SerialPort.DataBits.ToString();
                config.AppSettings.Settings["StopBits"].Value = this._SerialPort.SerialPort.StopBits.ToString();
            }

            config.Save(System.Configuration.ConfigurationSaveMode.Modified);
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Устанавливает сетк как текущую для работы программы
        /// </summary>
        /// <param name="network"></param>
        private void SetCurrentNetwork(NetworkController network)
        {
            if (this._Network != null)
            {
                if (this._Network.Status == Common.Controlling.Status.Running)
                {
                    throw new InvalidOperationException(
                        "Невозможно сменить рабочую сеть, текущая сеть имеет активное состояние");
                }
            }

            if (network == null)
            {
                this._Network = network;
                this._ToolStripMenuItemNetwork.Enabled = false;
                this._ToolStripMenuItemFileSave.Enabled = false;
                this._ToolStripMenuItemFileSaveAs.Enabled = false;
                this._BindingSourceDevices.DataSource = null;
            }
            else
            {
                // Устанавливаем новую сеть в качестве текущей
                this._Network = network;
                this._Network.Connection = this._SerialPort;
                this._Network.DevicesListWasChanged +=
                    new EventHandler(EventHandler_Network_DevicesListWasChanged);
                this._Network.NetworkErrorOccurred += new NetworkErrorOccurredEventHandler(EventHandler_Network_NetworkErrorOccurred);

                this._ToolStripMenuItemFileSave.Enabled = true;
                this._ToolStripMenuItemFileSaveAs.Enabled = true;

                this._ToolStripMenuItemNetwork.Enabled = true;
                // Разрешаем редакитрование сети
                this._ToolStripMenuItemNetworkEdit.Enabled = true;
                this._ToolStripButtonStart.Enabled = true;
                this._ToolStripButtonStop.Enabled = false;

                this._BindingSourceDevices.DataSource = null;
                this._BindingSourceDevices.DataSource = Network.Devices;
                this._DataGridViewCoils.DataSource = this._BindingSourceDevices;
                this._DataGridViewDiscretesInputs.DataSource = this._BindingSourceDevices;
                this._DataGridViewFiles.DataSource = this._BindingSourceDevices;
                this._DataGridViewHoldingRegisters.DataSource = this._BindingSourceDevices;
                this._DataGridViewInputRegisters.DataSource = this._BindingSourceDevices;
            }

            // Отображаем текущую сеть сеть
            this.ShowNetwork(ref this._TreeViewNetwork, ref this._Network);
            // Отображем статусную строку
            this.SetPathToFileNetworkConfigToStatusStrip(this._PathToFileNetworkConfig);
            this.SetSerialPortSettingToStatusStrip(this._SerialPort);
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события возникновения ошибок в работе сети
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void EventHandler_Network_NetworkErrorOccurred(
            object sender, NetworkErrorEventArgs args)
        {
            String msg = String.Format("Категрия: {0}; Описание: {1}", 
                args.Category.ToString(), args.ErrorDescription);
            MessageBox.Show(this, msg, "Ошибка", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Инициализирует строку состояния приложения
        /// </summary>
        private void InitStatusBar(ref StatusStrip control)
        {
            //ToolStripTextBox txtItem;
            ToolStripStatusLabel lblItem;

            if (control == null)
            {
                throw new Exception();
            }
            control.Items.Clear();
            control.ShowItemToolTips = true;
            control.Stretch = true;
            control.Capture = false;

            lblItem = new ToolStripStatusLabel();
            lblItem.Name = "_ToolStripLabelPathToFile";
            lblItem.Text = String.Empty;
            lblItem.Alignment = ToolStripItemAlignment.Left;
            lblItem.TextAlign = ContentAlignment.MiddleLeft;
            lblItem.BorderSides = ToolStripStatusLabelBorderSides.All;
            lblItem.BorderStyle = Border3DStyle.Flat;
            lblItem.AutoToolTip = true;
            control.Items.Add(lblItem);

            lblItem = new ToolStripStatusLabel();
            lblItem.Name = "_ToolStripLabelSettingsPort";
            lblItem.Text = String.Empty;
            lblItem.Alignment = ToolStripItemAlignment.Left;
            lblItem.TextAlign = ContentAlignment.MiddleLeft;
            lblItem.BorderSides = ToolStripStatusLabelBorderSides.All;
            lblItem.BorderStyle = Border3DStyle.Flat;
            lblItem.AutoToolTip = true;
            lblItem.ToolTipText = "COM-порт, корость, кол-во бит данных, кол-во стоп-бит, паритет";
            control.Items.Add(lblItem);
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Отображает значение в строке состояния приложения
        /// </summary>
        /// <param name="path">путь к файлу конфигурации сети</param>
        private void SetPathToFileNetworkConfigToStatusStrip(String path)
        {
            ToolStripStatusLabel lable;
            lable = (ToolStripStatusLabel)this._StatusStripMainWindow.Items["_ToolStripLabelPathToFile"];
            lable.Text = path;
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Отображает значение в строке состояния приложения
        /// </summary>
        /// <param name="serialPort">Порт для отображения данных</param>
        private void SetSerialPortSettingToStatusStrip(ComPort serialPort)
        {
            // Создаём строку 
            
            if (serialPort != null)
            {
                this._StatusStripMainWindow.Items["_ToolStripLabelSettingsPort"].Text =
                    String.Format("{0}, {1}, {2}, {3}, {4}", serialPort.SerialPort.PortName,
                    serialPort.SerialPort.BaudRate.ToString(), serialPort.SerialPort.DataBits.ToString(),
                    serialPort.SerialPort.StopBits.ToString(), serialPort.SerialPort.Parity.ToString());
            }
            else
            {
                this._StatusStripMainWindow.Items["_ToolStripLabelSettingsPort"].Text = String.Empty;
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ToolStripButtonStart_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;
            this._Network.Start();
            btn.Enabled = false;
            this._ToolStripButtonStop.Enabled = true;
            this._ToolStripMenuItemNetworkEdit.Enabled = false;
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ToolStripButtonStop_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;
            this._Network.Stop();
            btn.Enabled = false;
            this._ToolStripButtonStart.Enabled = true;
            this._ToolStripMenuItemNetworkEdit.Enabled = true;
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ToolStripMenuItemFileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file