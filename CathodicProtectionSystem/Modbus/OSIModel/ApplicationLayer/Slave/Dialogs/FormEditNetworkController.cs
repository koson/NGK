using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel;

namespace Modbus.OSIModel.ApplicationLayer.Slave.Dialogs
{
    public partial class FormEditNetworkController : Form
    {
        #region Constructors

        public FormEditNetworkController()
        {
            InitializeComponent();
        }

        #endregion

        #region Fields And Properties
        /// <summary>
        /// Сеть Modbus
        /// </summary>
        private ModbusNetworkControllerSlave _Network;
        /// <summary>
        /// Устанавливает/возвращает редактируемую сеть Modbus
        /// </summary>
        public ModbusNetworkControllerSlave Network
        {
            get { return _Network; }
            set 
            { 

                if (value != null)
                {
                    if (value.Status == Common.Controlling.Status.Stopped)
                    {
                        _Network = value;
                        _Network.DevicesListWasChanged +=
                            new EventHandler(EventHandler_Network_DevicesListWasChanged);
                        _Network.StatusWasChanged +=
                            new EventHandler(EventHandler_Network_NetworkChangedStatus);
                    }
                    else
                    {
                        throw new InvalidOperationException("Попытка редактировать активную сеть");
                    }
                }
            }
        }
        private BindingSource _BindingSourceDevicesList;
        private BindingSource _BindingSourceFile;

        #endregion
        
        #region Methods

        private void EventHandler_Network_NetworkChangedStatus(
            object sender, EventArgs e)
        {
            ModbusNetworkControllerSlave network = (ModbusNetworkControllerSlave)sender;
            if ((network.Status == Common.Controlling.Status.Running) ||
                (network.Status == Common.Controlling.Status.Paused))
            {
                MessageBox.Show(this, "Внимание. Редактируемый сетевой контроллер изменил " + 
                    "своё состояние на активное. Продолжение редактирования сети опасно", 
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return;
        }
        /// <summary>
        /// Обработчик события изменения списка устройств в редактируемой сети
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_Network_DevicesListWasChanged(
            object sender, EventArgs e)
        {
            return;
        }
        /// <summary>
        /// Обработчик события загрузки формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_FormEditNetworkController_Load(
            object sender, EventArgs e)
        {
            FormEditNetworkController form = (FormEditNetworkController)sender;

            // Контекстное меню
            _ContextMenuStripCoils.Enabled = true;
            _ContextMenuStripDevicesList.Enabled = true;
            _ContextMenuStripHoldingRegisters.Enabled = true;
            _ContextMenuStripDiscretesInputs.Enabled = true;
            _ContextMenuStripFiles.Enabled = true;
            _ContextMenuStripInputRegisters.Enabled = true;
            _ContextMenuStripRecords.Enabled = true;

            // Настраиваем источники данных
            _BindingSourceFile = new BindingSource();
            //_BindingSourceRecords.DataMember = "Records";

            // Для списка устройств
            _BindingSourceDevicesList = new BindingSource();
            _BindingSourceDevicesList.PositionChanged +=
                new EventHandler(EventHandler_BindingSourceDevicesList_PositionChanged);
            _BindingSourceDevicesList.ListChanged +=
                new ListChangedEventHandler(EventHandler_BindingSourceDevicesList_ListChanged);
            _BindingSourceDevicesList.DataMember = "Devices";
            _BindingSourceDevicesList.DataSource = _Network;

            // Настраиваем гриды для отображения данных устройства
            InitDataGridViewDevices();
            InitDataGridViewHoldingRegisters();
            InitDataGridViewCoils();
            InitDataGridViewInputRegisters();
            InitDataGridViewDiscretesInputs();
            InitDataGridViewFiles();
            InitDataGridViewRecords();

            return;
        }
        /// <summary>
        /// Запускается при старте приложения. Настраивает DataGridView
        /// </summary>
        private void InitDataGridViewDevices()
        {
            DataGridViewColumn column;

            _DataGridViewDevicesList.AllowUserToAddRows = false;
            _DataGridViewDevicesList.AllowUserToDeleteRows = false;
            _DataGridViewDevicesList.AutoGenerateColumns = false;
            _DataGridViewDevicesList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _DataGridViewDevicesList.MultiSelect = false;
            _DataGridViewDevicesList.Dock = DockStyle.Fill;
            _DataGridViewDevicesList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _DataGridViewDevicesList.CellParsing +=
                new DataGridViewCellParsingEventHandler(EventHandler_DataGridViewDevicesList_CellParsing);
            _DataGridViewDevicesList.CellEndEdit +=
                new DataGridViewCellEventHandler(EventHandler_DataGridViewDevicesList_CellEndEdit);
            _DataGridViewDevicesList.DataError +=
                new DataGridViewDataErrorEventHandler(EventHandler_DataGridViewDevicesList_DataError);

            // Настраиваем столбцы
            column = new DataGridViewTextBoxColumn();
            column.Name = "Address";
            column.DataPropertyName = "Address";
            column.HeaderText = "Адрес";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(Byte);
            column.ReadOnly = false;
            _DataGridViewDevicesList.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "NetworkController";
            column.DataPropertyName = "NetworkController";
            column.HeaderText = "Владелец";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(ModbusNetworkControllerSlave);
            column.ReadOnly = true;
            _DataGridViewDevicesList.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Description";
            column.DataPropertyName = "Description";
            column.HeaderText = "Примечания";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(String);
            column.ReadOnly = false;
            _DataGridViewDevicesList.Columns.Add(column);

            DataGridViewComboBoxColumn cbxcolumn = new DataGridViewComboBoxColumn();
            cbxcolumn.Name = "Status";
            cbxcolumn.DataSource = Enum.GetValues(typeof(Common.Controlling.Status));
            cbxcolumn.DataPropertyName = "Status";
            cbxcolumn.HeaderText = "Состояние";
            cbxcolumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
            cbxcolumn.FlatStyle = FlatStyle.Flat;
            cbxcolumn.ValueType = typeof(Common.Controlling.Status);
            cbxcolumn.ReadOnly = false;
            _DataGridViewDevicesList.Columns.Add(cbxcolumn);

            _DataGridViewDevicesList.DataSource = null;
            _DataGridViewDevicesList.DataSource = _BindingSourceDevicesList;

            return;
        }
        /// <summary>
        /// Запускается при старте приложения. Настраивает DataGridView
        /// </summary>
        private void InitDataGridViewHoldingRegisters()
        {
            DataGridViewColumn column;
            
            // Настраиваем источник данных
            _DataGridViewHoldingRegisters.AllowUserToAddRows = false;
            _DataGridViewHoldingRegisters.AllowUserToDeleteRows = false;
            _DataGridViewHoldingRegisters.AutoGenerateColumns = false;
            _DataGridViewHoldingRegisters.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _DataGridViewHoldingRegisters.MultiSelect = false;
            _DataGridViewHoldingRegisters.Dock = DockStyle.Fill;
            _DataGridViewHoldingRegisters.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _DataGridViewHoldingRegisters.CellParsing +=
                new DataGridViewCellParsingEventHandler(EventHandler_DataGridViewHoldingRegisters_CellParsing);
            _DataGridViewHoldingRegisters.CellEndEdit +=
                new DataGridViewCellEventHandler(_DataGridViewHoldingRegisters_CellEndEdit);
            _DataGridViewHoldingRegisters.DataError +=
                new DataGridViewDataErrorEventHandler(EventHandler_DataGridViewHoldingRegisters_DataError);
            _DataGridViewHoldingRegisters.DataSource = null;
            //_DataGridViewHoldingRegisters.DataSource = _BindingSourceHoldingRegisters;
            _DataGridViewHoldingRegisters.DataMember = "HoldingRegisters";
            _DataGridViewHoldingRegisters.DataSource = _BindingSourceDevicesList;

            column = new DataGridViewTextBoxColumn();
            column.Name = "Address";
            column.DataPropertyName = "Address";
            column.HeaderText = "Адрес";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(UInt16);
            column.Visible = true;
            column.ReadOnly = false;
            _DataGridViewHoldingRegisters.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Value";
            column.DataPropertyName = "Value";
            column.HeaderText = "Значение";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(UInt16);
            column.Visible = true;
            column.ReadOnly = false;
            _DataGridViewHoldingRegisters.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Description";
            column.DataPropertyName = "Description";
            column.HeaderText = "Примечания";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(String);
            column.Visible = true;
            column.ReadOnly = false;
            _DataGridViewHoldingRegisters.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "ParameterType";
            column.DataPropertyName = "ParameterType";
            column.HeaderText = "Тип данных";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(ModbusParameterType);
            column.Visible = true;
            column.ReadOnly = true;
            _DataGridViewHoldingRegisters.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Device";
            column.DataPropertyName = "Device";
            column.HeaderText = "Владелец";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(ModbusSlaveDevice);
            column.Visible = true;
            column.ReadOnly = true;
            _DataGridViewHoldingRegisters.Columns.Add(column);

            return;
        }
        /// <summary>
        /// Запускается при старте приложения. Настраивает DataGridView
        /// </summary>
        private void InitDataGridViewInputRegisters()
        {
            DataGridViewColumn column;

            _DataGridViewInputRegisters.AllowUserToAddRows = false;
            _DataGridViewInputRegisters.AllowUserToDeleteRows = false;
            _DataGridViewInputRegisters.AutoGenerateColumns = false;
            _DataGridViewInputRegisters.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _DataGridViewInputRegisters.MultiSelect = false;
            _DataGridViewInputRegisters.Dock = DockStyle.Fill;
            _DataGridViewInputRegisters.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _DataGridViewInputRegisters.CellParsing +=
                new DataGridViewCellParsingEventHandler(EventHandler_DataGridViewInputRegisters_CellParsing);
            _DataGridViewInputRegisters.CellEndEdit +=
                new DataGridViewCellEventHandler(_DataGridViewInputRegisters_CellEndEdit);
            _DataGridViewInputRegisters.DataError +=
                new DataGridViewDataErrorEventHandler(EventHandler_DataGridViewInputRegisters_DataError);
            _DataGridViewInputRegisters.DataSource = null;
            //_DataGridViewInputRegisters.DataSource = _BindingSourceInputRegisters;
            _DataGridViewInputRegisters.DataMember = "InputRegisters";
            _DataGridViewInputRegisters.DataSource = _BindingSourceDevicesList;

            column = new DataGridViewTextBoxColumn();
            column.Name = "Address";
            column.DataPropertyName = "Address";
            column.HeaderText = "Адрес";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(UInt16);
            column.Visible = true;
            column.ReadOnly = false;
            _DataGridViewInputRegisters.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Value";
            column.DataPropertyName = "Value";
            column.HeaderText = "Значение";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(UInt16);
            column.Visible = true;
            column.ReadOnly = false;
            _DataGridViewInputRegisters.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Description";
            column.DataPropertyName = "Description";
            column.HeaderText = "Примечания";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(String);
            column.Visible = true;
            column.ReadOnly = false;
            _DataGridViewInputRegisters.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "ParameterType";
            column.DataPropertyName = "ParameterType";
            column.HeaderText = "Тип данных";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(ModbusParameterType);
            column.Visible = true;
            column.ReadOnly = true;
            _DataGridViewInputRegisters.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Device";
            column.DataPropertyName = "Device";
            column.HeaderText = "Владелец";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(ModbusSlaveDevice);
            column.Visible = true;
            column.ReadOnly = true;
            _DataGridViewInputRegisters.Columns.Add(column);
            
            return;
        }
        /// <summary>
        /// Запускается при старте приложения. Настраивает DataGridView
        /// </summary>
        private void InitDataGridViewCoils()
        {
            DataGridViewColumn column;

            _DataGridViewCoils.AllowUserToAddRows = false;
            _DataGridViewCoils.AllowUserToDeleteRows = false;
            _DataGridViewCoils.AutoGenerateColumns = false;
            _DataGridViewCoils.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _DataGridViewCoils.MultiSelect = false;
            _DataGridViewCoils.Dock = DockStyle.Fill;
            _DataGridViewCoils.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _DataGridViewCoils.CellParsing +=
                new DataGridViewCellParsingEventHandler(EventHandler_DataGridViewCoils_CellParsing);
            _DataGridViewCoils.CellEndEdit +=
                new DataGridViewCellEventHandler(EventHadler_DataGridViewCoils_CellEndEdit);
            _DataGridViewCoils.DataError +=
                new DataGridViewDataErrorEventHandler(EventHandler_DataGridViewCoils_DataError);
            _DataGridViewCoils.DataSource = null;
            //_DataGridViewCoils.DataSource = _BindingSourceCoils;
            //_DataGridViewCoils.DataSource = _BingingSourceCurrentDevice;
            _DataGridViewCoils.DataMember = "Coils";
            _DataGridViewCoils.DataSource = _BindingSourceDevicesList;

            column = new DataGridViewTextBoxColumn();
            column.Name = "Address";
            column.HeaderText = "Адрес";
            column.DataPropertyName = "Address";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.Visible = true;
            column.ReadOnly = false;
            _DataGridViewCoils.Columns.Add(column);

            column = new DataGridViewCheckBoxColumn();
            column.Name = "Value";
            column.HeaderText = "Значение";
            column.DataPropertyName = "Value";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(Boolean);
            column.Visible = true;
            column.ReadOnly = false;
            _DataGridViewCoils.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Description";
            column.HeaderText = "Примечания";
            column.DataPropertyName = "Description";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(String);
            column.Visible = true;
            column.ReadOnly = false;
            _DataGridViewCoils.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "ParameterType";
            column.HeaderText = "Тип данных";
            column.DataPropertyName = "ParameterType";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(ModbusParameterType);
            column.Visible = true;
            column.ReadOnly = true;
            _DataGridViewCoils.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Device";
            column.HeaderText = "Владелец";
            column.DataPropertyName = "Device";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(ModbusSlaveDevice);
            column.Visible = true;
            column.ReadOnly = true;
            _DataGridViewCoils.Columns.Add(column);
            
            return;
        }
        /// <summary>
        /// Запускается при старте приложения. Настраивает DataGridView
        /// </summary>
        private void InitDataGridViewDiscretesInputs()
        {
            DataGridViewColumn column;

            _DataGridViewDiscretesInputs.AllowUserToAddRows = false;
            _DataGridViewDiscretesInputs.AllowUserToDeleteRows = false;
            _DataGridViewDiscretesInputs.AutoGenerateColumns = false;
            _DataGridViewDiscretesInputs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _DataGridViewDiscretesInputs.MultiSelect = false;
            _DataGridViewDiscretesInputs.Dock = DockStyle.Fill;
            _DataGridViewDiscretesInputs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _DataGridViewDiscretesInputs.CellParsing +=
                new DataGridViewCellParsingEventHandler(EventHandler_DataGridViewDiscretesInputs_CellParsing);
            _DataGridViewDiscretesInputs.CellEndEdit +=
                new DataGridViewCellEventHandler(_DataGridViewDiscretesInputs_CellEndEdit);
            _DataGridViewDiscretesInputs.DataError +=
                new DataGridViewDataErrorEventHandler(EventHandler_DataGridViewDiscretesInputs_DataError);
            _DataGridViewDiscretesInputs.DataSource = null;
            //_DataGridViewDiscretesInputs.DataSource = _BindingSourceDiscrestesInputs;
            _DataGridViewDiscretesInputs.DataMember = "DiscretesInputs";
            _DataGridViewDiscretesInputs.DataSource = _BindingSourceDevicesList;

            column = new DataGridViewTextBoxColumn();
            column.Name = "Address";
            column.DataPropertyName = "Address";
            column.HeaderText = "Адрес";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(UInt16);
            column.Visible = true;
            column.ReadOnly = false;
            _DataGridViewDiscretesInputs.Columns.Add(column);

            column = new DataGridViewCheckBoxColumn();
            column.Name = "Value";
            column.DataPropertyName = "Value";
            column.HeaderText = "Значение";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(Boolean);
            column.Visible = true;
            column.ReadOnly = false;
            _DataGridViewDiscretesInputs.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Description";
            column.DataPropertyName = "Description";
            column.HeaderText = "Примечания";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(String);
            column.Visible = true;
            column.ReadOnly = false;
            _DataGridViewDiscretesInputs.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "ParameterType";
            column.DataPropertyName = "ParameterType";
            column.HeaderText = "Тип данных";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(ModbusParameterType);
            column.Visible = true;
            column.ReadOnly = true;
            _DataGridViewDiscretesInputs.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Device";
            column.DataPropertyName = "Device";
            column.HeaderText = "Владелец";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(ModbusSlaveDevice);
            column.Visible = true;
            column.ReadOnly = true;
            _DataGridViewDiscretesInputs.Columns.Add(column);

            return;
        }
        /// <summary>
        /// 
        /// </summary>
        private void InitDataGridViewFiles()
        {
            DataGridViewColumn column;
            //CurrencyManager cManager;

            _DataGridViewFiles.AllowUserToAddRows = false;
            _DataGridViewFiles.AllowUserToDeleteRows = false;
            _DataGridViewFiles.AutoGenerateColumns = false;
            _DataGridViewFiles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _DataGridViewFiles.MultiSelect = false;
            _DataGridViewFiles.Dock = DockStyle.Fill;
            _DataGridViewFiles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _DataGridViewFiles.CellParsing +=
                new DataGridViewCellParsingEventHandler(EventHandler_DataGridViewFiles_CellParsing);
            _DataGridViewFiles.CellEndEdit +=
                new DataGridViewCellEventHandler(EventHandler_DataGridViewFiles_CellEndEdit);
            _DataGridViewFiles.DataError +=
                new DataGridViewDataErrorEventHandler(EventHandler_DataGridViewFiles_DataError);
            _DataGridViewFiles.SelectionChanged += 
                new EventHandler(EventHandler_DataGridViewFiles_SelectionChanged);
            _DataGridViewFiles.DataSource = null;
            _DataGridViewFiles.DataMember = "Files";
            _DataGridViewFiles.DataSource = _BindingSourceDevicesList;

            //cManager = _DataGridViewFiles.BindingContext[
            //    _DataGridViewFiles.DataSource, _DataGridViewFiles.DataMember] as CurrencyManager;
            //cManager.PositionChanged +=new EventHandler(EventHandler_cManager_PositionChanged);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Number";
            column.DataPropertyName = "Number";
            column.HeaderText = "Номер файла";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(UInt16);
            column.Visible = true;
            column.ReadOnly = false;
            _DataGridViewFiles.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Description";
            column.DataPropertyName = "Description";
            column.HeaderText = "Примечания";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(String);
            column.Visible = true;
            column.ReadOnly = false;
            _DataGridViewFiles.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Device";
            column.DataPropertyName = "Device";
            column.HeaderText = "Владелец";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(ModbusSlaveDevice);
            column.Visible = true;
            column.ReadOnly = true;
            _DataGridViewFiles.Columns.Add(column);
            
            return;
        }
        /// <summary>
        /// Запускается при старте приложения. Настраивает DataGridView
        /// </summary>
        private void InitDataGridViewRecords()
        {
            DataGridViewColumn column;

            _DataGridViewRecords.AllowUserToAddRows = false;
            _DataGridViewRecords.AllowUserToDeleteRows = false;
            _DataGridViewRecords.AutoGenerateColumns = false;
            _DataGridViewRecords.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _DataGridViewRecords.MultiSelect = false;
            _DataGridViewRecords.Dock = DockStyle.Fill;
            _DataGridViewRecords.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _DataGridViewRecords.CellParsing +=
                new DataGridViewCellParsingEventHandler(EventHandler_DataGridViewRecords_CellParsing);
            _DataGridViewRecords.CellEndEdit +=
                new DataGridViewCellEventHandler(EventHandler_DataGridViewRecords_CellEndEdit);
            _DataGridViewRecords.DataError +=
                new DataGridViewDataErrorEventHandler(EventHandler_DataGridViewRecords_DataError);
            //_DataGridViewRecords.DataMember = String.Empty;
            //_DataGridViewRecords.DataSource = _BindingSourceRecords;

            column = new DataGridViewTextBoxColumn();
            column.Name = "Address";
            column.HeaderText = "Номер записи";
            column.DataPropertyName = "Address";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(UInt16);
            column.Visible = true;
            column.ReadOnly = false;
            _DataGridViewRecords.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Value";
            column.HeaderText = "Значение";
            column.DataPropertyName = "Value";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(UInt16); 
            column.Visible = true;
            column.ReadOnly = false;
            _DataGridViewRecords.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Description";
            column.HeaderText = "Примечания";
            column.DataPropertyName = "Description";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(String);
            column.Visible = true;
            column.ReadOnly = false;
            _DataGridViewRecords.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "ParameterType";
            column.HeaderText = "Тип данных";
            column.DataPropertyName = "ParameterType";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(ModbusParameterType);
            column.Visible = true;
            column.ReadOnly = true;
            _DataGridViewRecords.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Device";
            column.HeaderText = "Владелец";
            column.DataPropertyName = "Device";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(ModbusSlaveDevice);
            column.Visible = true;
            column.ReadOnly = true;
            _DataGridViewRecords.Columns.Add(column);

            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_DataGridViewFiles_SelectionChanged(
            object sender, EventArgs e)
        {
            DataGridView control;
            CurrencyManager manager;
            File file;

            control = (DataGridView)sender;
            manager = control.BindingContext[control.DataSource, control.DataMember] as CurrencyManager;
            // Получаем текущий файл
            // Если список пуст, то записи файла не отображаются
            if (manager.Count != 0)
            {
                file = (File)manager.Current;

                _BindingSourceFile = new BindingSource(file, String.Empty);

                _DataGridViewRecords.DataSource = null;
                _DataGridViewRecords.DataSource = _BindingSourceFile;
                _DataGridViewRecords.DataMember = "Records";

                _DataGridViewRecords.Enabled = true;
                //_ContextMenuStripRecords.Enabled = true;
            }
            else
            {
                _BindingSourceFile = new BindingSource(null, String.Empty);

                _DataGridViewRecords.DataSource = null;
                _DataGridViewRecords.DataSource = _BindingSourceFile;
                _DataGridViewRecords.DataMember = String.Empty;

                _DataGridViewRecords.Enabled = false;
                //_ContextMenuStripRecords.Enabled = false;
            }
            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_BindingSourceDevicesList_ListChanged(
            object sender, ListChangedEventArgs e)
        {
            BindingSource bs = (BindingSource)sender;

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemDeleted:
                    {
                        // При удаленни устройства проверяем количество
                        // устройств в списке. Если список пуст блокируем
                        // соответствующие элементы окна
                        if (bs.Count == 0)
                        {
                            _TabControlDevice.Enabled = false;
                        }
                        break; 
                    }
                case ListChangedType.ItemAdded:
                    {
                        // Список устройств всегда больше 0, разрешаем
                        // работу элементов окна.
                        _TabControlDevice.Enabled = true;
                        break;
                    }
                case ListChangedType.Reset:
                    {
                        if (bs.Count > 0)
                        {
                            _TabControlDevice.Enabled = true;
                        }
                        else
                        {
                            _TabControlDevice.Enabled = false;
                        }
                        break;
                    }
                case ListChangedType.ItemMoved:
                    { break; }
            }
            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_BindingSourceDevicesList_PositionChanged(
            object sender, EventArgs e)
        {
            BindingSource bs;
            bs = (BindingSource)sender;

            return;
        }
        /// <summary>
        /// Обработчик события грида окончания ввода пользователем нового значения
        /// в ячейку.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_DataGridViewDevicesList_CellParsing(
            object sender, DataGridViewCellParsingEventArgs e)
        {
            DataGridView dgv;
            dgv = (DataGridView)sender;

            switch (e.ColumnIndex)
            {
                case 0: // Сетевой адрес устройства
                    {
                        Byte result;
                        if (Byte.TryParse((String)e.Value, out result))
                        {
                            e.Value = result;
                            dgv.Rows[e.RowIndex].ErrorText = String.Empty;
                            e.ParsingApplied = true;
                        }
                        else
                        {
                            dgv.Rows[e.RowIndex].ErrorText = "Значение должно быть 1...247";
                            e.ParsingApplied = false;
                        }
                        break;
                    }
                case 2: // Поле "Description"
                    {
                        e.ParsingApplied = true;
                        break;
                    }
                case 3: //
                    {
                        if (Enum.IsDefined(typeof(Common.Controlling.Status), e.Value))
                        {
                            e.Value = (Common.Controlling.Status)Enum.Parse(typeof(Common.Controlling.Status), e.Value as String);
                            e.ParsingApplied = true;
                        }
                        else
                        {
                            e.ParsingApplied = false;
                        }
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException("Попытка редактирования поля только для чтения");
                    }
            }
            return;
        }
        private void EventHandler_DataGridViewDevicesList_DataError(
            object sender, DataGridViewDataErrorEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            if ((e.Context & DataGridViewDataErrorContexts.Parsing) ==
                DataGridViewDataErrorContexts.Parsing)
            {
                e.ThrowException = false;
                e.Cancel = true;
            }
            else if ((e.Context & DataGridViewDataErrorContexts.Commit) == 
                DataGridViewDataErrorContexts.Commit)
            {
                dgv.Rows[e.RowIndex].ErrorText = e.Exception.Message;
                e.ThrowException = false;
                e.Cancel = true;
            }
            else
            {
                e.ThrowException = true;
                e.Cancel = false;
            }
            return;
        }
        private void EventHandler_DataGridViewDevicesList_CellEndEdit(
            object sender, DataGridViewCellEventArgs e)
        {
            DataGridView control = (DataGridView)sender;
            control.Rows[e.RowIndex].ErrorText = String.Empty;
            return;
        }
        private void EventHandler_DataGridViewCoils_CellParsing(
            object sender, DataGridViewCellParsingEventArgs e)
        {
            DataGridView dgv;
            DataGridViewColumn column;

            dgv = (DataGridView)sender;
            column = dgv.Columns[e.ColumnIndex];
            
            switch(column.Name)
            {
                case "Address":
                    {
                        UInt16 address;
                        if (UInt16.TryParse((String)e.Value, out address))
                        {
                            e.Value = address;
                            e.ParsingApplied = true;
                            dgv.Rows[e.RowIndex].ErrorText = String.Empty;
                        }
                        else
                        {
                            dgv.Rows[e.RowIndex].ErrorText = "Значение должно быть числом 0...65535";
                            //dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "Значение должно быть числом 0...65535";
                            e.ParsingApplied = false;
                        }
                        break; 
                    }
                case "Value":
                    {
                        e.ParsingApplied = true;
                        break; 
                    }
                case "Description":
                    {
                        e.ParsingApplied = true;
                        break; 
                    }
                default:
                    {
                        throw new InvalidOperationException(
                            "Попытка редактирования значения столбца только для чтения");
                    }
            }
            return;
        }

        private void EventHandler_DataGridViewCoils_DataError(
            object sender, DataGridViewDataErrorEventArgs e)
        {
            DataGridView dgv;
            
            dgv = (DataGridView)sender;

            if ((e.Context & DataGridViewDataErrorContexts.Parsing)
                == DataGridViewDataErrorContexts.Parsing)
            {
                e.ThrowException = false;                
                e.Cancel = true;
            }
            else
            {
                e.ThrowException = true;
                e.Cancel = false;
            }
            return;
        }

        private void EventHadler_DataGridViewCoils_CellEndEdit(
            object sender, DataGridViewCellEventArgs e)
        {
            DataGridView control = (DataGridView)sender;
            control.Rows[e.RowIndex].ErrorText = String.Empty;
            return;
        }

        private void EventHandler_DataGridViewDiscretesInputs_CellParsing(
            object sender, DataGridViewCellParsingEventArgs e)
        {
            DataGridView dgv;
            DataGridViewColumn column;

            dgv = (DataGridView)sender;
            column = dgv.Columns[e.ColumnIndex];

            switch (column.Name)
            {
                case "Address":
                    {
                        UInt16 address;
                        if (UInt16.TryParse((String)e.Value, out address))
                        {
                            e.Value = address;
                            dgv.Rows[e.RowIndex].ErrorText = String.Empty;
                            e.ParsingApplied = true;
                        }
                        else
                        {
                            dgv.Rows[e.RowIndex].ErrorText = "Значение должно быть числом 0...65535";
                            e.ParsingApplied = false;
                        }
                        break;
                    }
                case "Value":
                    {
                        e.ParsingApplied = true;
                        break;
                    }
                case "Description":
                    {
                        e.ParsingApplied = true;
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException(
                            "Попытка редактирования значения столбца только для чтения");
                    }
            }
            return;
        }

        private void EventHandler_DataGridViewDiscretesInputs_DataError(
            object sender, DataGridViewDataErrorEventArgs e)
        {
            DataGridView dgv;

            dgv = (DataGridView)sender;

            if ((e.Context & DataGridViewDataErrorContexts.Parsing)
                == DataGridViewDataErrorContexts.Parsing)
            {
                e.ThrowException = false;
                e.Cancel = true;
            }
            else
            {
                e.ThrowException = true;
                e.Cancel = false;
            }
            return;
        }

        private void _DataGridViewDiscretesInputs_CellEndEdit(
            object sender, DataGridViewCellEventArgs e)
        {
            DataGridView control = (DataGridView)sender;
            control.Rows[e.RowIndex].ErrorText = String.Empty;
            return;
        }

        private void EventHandler_DataGridViewInputRegisters_CellParsing(
            object sender, DataGridViewCellParsingEventArgs e)
        {
            DataGridView dgv;
            DataGridViewColumn column;

            dgv = (DataGridView)sender;
            column = dgv.Columns[e.ColumnIndex];

            switch (column.Name)
            {
                case "Address":
                    {
                        UInt16 address;
                        if (UInt16.TryParse((String)e.Value, out address))
                        {
                            e.Value = address;
                            dgv.Rows[e.RowIndex].ErrorText = String.Empty;
                            e.ParsingApplied = true;
                        }
                        else
                        {
                            dgv.Rows[e.RowIndex].ErrorText = "Значение должно быть числом 0...65535";
                            e.ParsingApplied = false;
                        }
                        break;
                    }
                case "Value":
                    {
                        UInt16 value;
                        if (UInt16.TryParse((String)e.Value, out value))
                        {
                            e.Value = value;
                            dgv.Rows[e.RowIndex].ErrorText = String.Empty;
                            e.ParsingApplied = true;
                        }
                        else
                        {
                            dgv.Rows[e.RowIndex].ErrorText = "Значение должно быть числом 0...65535";
                            e.ParsingApplied = false;
                        }
                        break;
                    }
                case "Description":
                    {
                        e.ParsingApplied = true;
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException(
                            "Попытка редактирования значения столбца только для чтения");
                    }
            }
            return;
        }

        private void EventHandler_DataGridViewInputRegisters_DataError(
            object sender, DataGridViewDataErrorEventArgs e)
        {
            DataGridView dgv;

            dgv = (DataGridView)sender;

            if ((e.Context & DataGridViewDataErrorContexts.Parsing)
                == DataGridViewDataErrorContexts.Parsing)
            {
                e.ThrowException = false;
                e.Cancel = true;
            }
            else
            {
                e.ThrowException = true;
                e.Cancel = false;
            }
            return;
        }

        private void _DataGridViewInputRegisters_CellEndEdit(
            object sender, DataGridViewCellEventArgs e)
        {
            DataGridView control = (DataGridView)sender;
            control.Rows[e.RowIndex].ErrorText = String.Empty;
            return;
        }

        private void EventHandler_DataGridViewHoldingRegisters_CellParsing(
            object sender, DataGridViewCellParsingEventArgs e)
        {
            DataGridView dgv;
            DataGridViewColumn column;

            dgv = (DataGridView)sender;
            column = dgv.Columns[e.ColumnIndex];

            switch (column.Name)
            {
                case "Address":
                    {
                        UInt16 address;
                        if (UInt16.TryParse((String)e.Value, out address))
                        {
                            e.Value = address;
                            dgv.Rows[e.RowIndex].ErrorText = String.Empty;
                            e.ParsingApplied = true;
                        }
                        else
                        {
                            dgv.Rows[e.RowIndex].ErrorText = "Значение должно быть числом 0...65535";
                            e.ParsingApplied = false;
                        }
                        break;
                    }
                case "Value":
                    {
                        UInt16 value;
                        if (UInt16.TryParse((String)e.Value, out value))
                        {
                            e.Value = value;
                            dgv.Rows[e.RowIndex].ErrorText = String.Empty;
                            e.ParsingApplied = true;
                        }
                        else
                        {
                            dgv.Rows[e.RowIndex].ErrorText = "Значение должно быть числом 0...65535";
                            e.ParsingApplied = false;
                        }
                        break;
                    }
                case "Description":
                    {
                        e.ParsingApplied = true;
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException(
                            "Попытка редактирования значения столбца только для чтения");
                    }
            }
            return;
        }

        private void EventHandler_DataGridViewHoldingRegisters_DataError(
            object sender, DataGridViewDataErrorEventArgs e)
        {
            DataGridView dgv;

            dgv = (DataGridView)sender;

            if ((e.Context & DataGridViewDataErrorContexts.Parsing)
                == DataGridViewDataErrorContexts.Parsing)
            {
                e.ThrowException = false;
                e.Cancel = true;
            }
            else
            {
                e.ThrowException = true;
                e.Cancel = false;
            }
            return;
        }

        private void _DataGridViewHoldingRegisters_CellEndEdit(
            object sender, DataGridViewCellEventArgs e)
        {
            DataGridView control = (DataGridView)sender;
            control.Rows[e.RowIndex].ErrorText = String.Empty;
            return;
        }

        private void EventHandler_DataGridViewRecords_DataError(
            object sender, DataGridViewDataErrorEventArgs e)
        {
            DataGridView dgv;

            dgv = (DataGridView)sender;

            if ((e.Context & DataGridViewDataErrorContexts.Parsing)
                == DataGridViewDataErrorContexts.Parsing)
            {
                e.ThrowException = false;
                e.Cancel = true;
            }
            else
            {
                e.ThrowException = true;
                e.Cancel = false;
            }
            return;
        }

        private void EventHandler_DataGridViewRecords_CellEndEdit(
            object sender, DataGridViewCellEventArgs e)
        {
            DataGridView control = (DataGridView)sender;
            control.Rows[e.RowIndex].ErrorText = String.Empty;
            return;
        }

        private void EventHandler_DataGridViewRecords_CellParsing(
            object sender, DataGridViewCellParsingEventArgs e)
        {
            DataGridView dgv;
            DataGridViewColumn column;

            dgv = (DataGridView)sender;
            column = dgv.Columns[e.ColumnIndex];

            switch (column.Name)
            {
                case "Address":
                    {
                        UInt16 address;
                        if (UInt16.TryParse((String)e.Value, out address))
                        {
                            e.Value = address;
                            dgv.Rows[e.RowIndex].ErrorText = String.Empty;
                            e.ParsingApplied = true;
                        }
                        else
                        {
                            dgv.Rows[e.RowIndex].ErrorText = "Значение должно быть числом 0...65535";
                            e.ParsingApplied = false;
                        }
                        break;
                    }
                case "Value":
                    {
                        UInt16 value;
                        if (UInt16.TryParse((String)e.Value, out value))
                        {
                            e.Value = value;
                            dgv.Rows[e.RowIndex].ErrorText = String.Empty;
                            e.ParsingApplied = true;
                        }
                        else
                        {
                            dgv.Rows[e.RowIndex].ErrorText = "Значение должно быть числом 0...65535";
                            e.ParsingApplied = false;
                        }
                        break;
                    }
                case "Description":
                    {
                        e.ParsingApplied = true;
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException(
                            "Попытка редактирования значения столбца только для чтения");
                    }
            }
            return;
        }

        private void EventHandler_DataGridViewFiles_DataError(
            object sender, DataGridViewDataErrorEventArgs e)
        {
            DataGridView dgv;

            dgv = (DataGridView)sender;

            if ((e.Context & DataGridViewDataErrorContexts.Parsing)
                == DataGridViewDataErrorContexts.Parsing)
            {
                e.ThrowException = false;
                e.Cancel = true;
            }
            else
            {
                e.ThrowException = true;
                e.Cancel = false;
            }
            return;
        }

        private void EventHandler_DataGridViewFiles_CellEndEdit(
            object sender, DataGridViewCellEventArgs e)
        {
            DataGridView control = (DataGridView)sender;
            control.Rows[e.RowIndex].ErrorText = String.Empty;
            return;
        }

        private void EventHandler_DataGridViewFiles_CellParsing(
            object sender, DataGridViewCellParsingEventArgs e)
        {
            DataGridView dgv;
            DataGridViewColumn column;

            dgv = (DataGridView)sender;
            column = dgv.Columns[e.ColumnIndex];

            switch (column.Name)
            {
                case "Number":
                    {
                        UInt16 number;
                        if (UInt16.TryParse((String)e.Value, out number))
                        {
                            e.Value = number;
                            dgv.Rows[e.RowIndex].ErrorText = String.Empty;
                            e.ParsingApplied = true;
                        }
                        else
                        {
                            dgv.Rows[e.RowIndex].ErrorText = "Значение должно быть числом 0...65535";
                            e.ParsingApplied = false;
                        }
                        break;
                    }
                case "Description":
                    {
                        e.ParsingApplied = true;
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException(
                            "Попытка редактирования значения столбца только для чтения");
                    }
            }
            return;
        }

        /// <summary>
        /// Добавляет устройство в сеть
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        private void AddDevice()
        {
            Byte address;
            ModbusSlaveDevice device;
            
            // Находим свобоный адрес и добавляем устройство с этим адресом 
            for (address = 1; address < 248; address++)
			{
                if (!_Network.Devices.Contains(address))
                {
                    device = new ModbusSlaveDevice(address);
                    _BindingSourceDevicesList.Add(device);

                    if (_BindingSourceDevicesList.Count > 0)
                    {
                        _ContextMenuStripCoils.Enabled = true;
                        _ContextMenuStripDiscretesInputs.Enabled = true;
                        _ContextMenuStripFiles.Enabled = true;
                        _ContextMenuStripHoldingRegisters.Enabled = true;
                        _ContextMenuStripInputRegisters.Enabled = true;
                        _ContextMenuStripRecords.Enabled = false;

                        _DataGridViewCoils.Enabled = true;
                        _DataGridViewDiscretesInputs.Enabled = true;
                        _DataGridViewFiles.Enabled = true;
                        _DataGridViewHoldingRegisters.Enabled = true;
                        _DataGridViewInputRegisters.Enabled = true;
                        _DataGridViewRecords.Enabled = false;
                    }
                    return;
                }
			}
            throw new InvalidOperationException(
                "Не удалось добавить новое устройство, все адреса заняты");
        }
        /// <summary>
        /// Удаляет устройство из сети
        /// </summary>
        private void RemoveDevice()
        {
            _BindingSourceDevicesList.RemoveCurrent();
            return;
        }
        /// <summary>
        /// Добавляет регистр входа/вывода в текущее устройство
        /// </summary>
        private void AddHoldingRegister()
        {
            UInt16 address;
            ModbusSlaveDevice device;
            HoldingRegister register;

            device = (ModbusSlaveDevice)_BindingSourceDevicesList.Current;

            for (address = 0; address <= UInt16.MaxValue; address++)
            {
                if (!device.HoldingRegisters.Contains(address))
                {
                    register = new HoldingRegister(address, 0, String.Empty);

                    device.HoldingRegisters.Add(register);
                    _BindingSourceDevicesList.ResetCurrentItem();

                    if (device.HoldingRegisters.Count > 0)
                    {
                        _ToolStripMenuItemRemoveHoldingRegister.Enabled = true;
                    }
                    if (device.HoldingRegisters.Count == UInt16.MaxValue)
                    {
                        _ToolStripMenuItemAddHoldingRegister.Enabled = false;
                    }

                    return;
                }
            }
            throw new InvalidOperationException(
                "Не удалось добавить новый holding register в устройство, все адреса заняты");
        }

        private void RemoveHoldingRegister()
        {
            CurrencyManager manager;
            HoldingRegister register;
            ModbusSlaveDevice device;

            manager = _DataGridViewHoldingRegisters.BindingContext[
                _DataGridViewHoldingRegisters.DataSource, 
                _DataGridViewHoldingRegisters.DataMember] as CurrencyManager;
            register = (HoldingRegister)manager.Current;
            device = (ModbusSlaveDevice)_BindingSourceDevicesList.Current;
            device.HoldingRegisters.Remove(register);
            _BindingSourceDevicesList.ResetCurrentItem();

            if (device.HoldingRegisters.Count == 0)
            {
                _ToolStripMenuItemRemoveHoldingRegister.Enabled = false;
            }
            _ToolStripMenuItemAddHoldingRegister.Enabled = true;
            return;
        }

        private void AddInputRegister()
        {
            UInt16 address;
            ModbusSlaveDevice device;
            InputRegister register;

            device = (ModbusSlaveDevice)_BindingSourceDevicesList.Current;

            for (address = 0; address <= UInt16.MaxValue; address++)
            {
                if (!device.InputRegisters.Contains(address))
                {
                    register = new InputRegister(address, 0, String.Empty);
                    device.InputRegisters.Add(register);
                    _BindingSourceDevicesList.ResetCurrentItem();

                    if (device.InputRegisters.Count > 0)
                    {
                        _ToolStripMenuItemRemoveInputRegister.Enabled = true;
                    }
                    if (device.InputRegisters.Count == UInt16.MaxValue)
                    {
                        _ToolStripMenuItemAddInputRegister.Enabled = false;
                    }
                    return;
                }
            }
            throw new InvalidOperationException(
                "Не удалось добавить новый input register в устройство, все адреса заняты");
        }

        private void RemoveInputRegister()
        {
            CurrencyManager manager;
            InputRegister register;
            ModbusSlaveDevice device;

            manager = _DataGridViewInputRegisters.BindingContext[
                _DataGridViewInputRegisters.DataSource,
                _DataGridViewInputRegisters.DataMember] as CurrencyManager;
            register = (InputRegister)manager.Current;
            device = (ModbusSlaveDevice)_BindingSourceDevicesList.Current;
            device.InputRegisters.Remove(register);
            _BindingSourceDevicesList.ResetCurrentItem();

            if (device.InputRegisters.Count == 0)
            {
                _ToolStripMenuItemRemoveInputRegister.Enabled = false;
            }
            _ToolStripMenuItemAddInputRegister.Enabled = true;

            return;
        }

        private void AddCoil()
        {
            UInt16 address;
            ModbusSlaveDevice device;
            Coil coil;

            device = (ModbusSlaveDevice)_BindingSourceDevicesList.Current;

            for (address = 0; address <= UInt16.MaxValue; address++)
            {
                if (!device.Coils.Contains(address))
                {
                    coil = new Coil(address, false, String.Empty);
                    device.Coils.Add(coil);
                    _BindingSourceDevicesList.ResetCurrentItem();

                    if (device.Coils.Count > 0)
                    {
                        _ToolStripMenuItemRemoveCoil.Enabled = true;
                    }
                    if (device.Coils.Count == UInt16.MaxValue)
                    {
                        _ToolStripMenuItemAddCoil.Enabled = false;
                    }
                    return;
                }
            }
            throw new InvalidOperationException(
                "Не удалось добавить новый coil в устройство, все адреса заняты");
        }

        private void RemoveCoil()
        {
            CurrencyManager manager;
            Coil coil;
            ModbusSlaveDevice device;

            manager = _DataGridViewCoils.BindingContext[
                _DataGridViewCoils.DataSource,
                _DataGridViewCoils.DataMember] as CurrencyManager;
            coil = (Coil)manager.Current;
            device = (ModbusSlaveDevice)_BindingSourceDevicesList.Current;
            device.Coils.Remove(coil);
            _BindingSourceDevicesList.ResetCurrentItem();

            if (device.Coils.Count == 0)
            {
                _ToolStripMenuItemRemoveCoil.Enabled = false;
            }

            _ToolStripMenuItemAddCoil.Enabled = true;

            return;
        }

        private void AddDiscreteInput()
        {
            UInt16 address;
            ModbusSlaveDevice device;
            DiscreteInput input;

            device = (ModbusSlaveDevice)_BindingSourceDevicesList.Current;

            for (address = 0; address <= UInt16.MaxValue; address++)
            {
                if (!device.DiscretesInputs.Contains(address))
                {
                    input = new DiscreteInput(address, false, String.Empty);
                    device.DiscretesInputs.Add(input);
                    _BindingSourceDevicesList.ResetCurrentItem();

                    if (device.DiscretesInputs.Count > 0)
                    {
                        _ToolStripMenuItemRemoveDiscreteInput.Enabled = true;
                    }
                    if (device.DiscretesInputs.Count == UInt16.MaxValue)
                    {
                        _ToolStripMenuItemAddDiscreteInput.Enabled = false;
                    }
                    return;
                }
            }
            throw new InvalidOperationException(
                "Не удалось добавить новый discrete input в устройство, все адреса заняты");
        }

        private void RemoveDiscreteInput()
        {
            CurrencyManager manager;
            DiscreteInput input;
            ModbusSlaveDevice device;

            manager = _DataGridViewDiscretesInputs.BindingContext[
                _DataGridViewDiscretesInputs.DataSource,
                _DataGridViewDiscretesInputs.DataMember] as CurrencyManager;
            input = (DiscreteInput)manager.Current;
            device = (ModbusSlaveDevice)_BindingSourceDevicesList.Current;
            device.DiscretesInputs.Remove(input);
            _BindingSourceDevicesList.ResetCurrentItem();

            if (device.DiscretesInputs.Count == 0)
            {
                _ToolStripMenuItemRemoveDiscreteInput.Enabled = false;
            }
            _ToolStripMenuItemAddDiscreteInput.Enabled = true;
            return;
        }

        private void AddFile()
        {
            UInt16 number;
            ModbusSlaveDevice device;
            File file;

            device = (ModbusSlaveDevice)_BindingSourceDevicesList.Current;

            for (number = 1; number < 10000; number++)
            {
                if (!device.Files.Contains(number))
                {
                    file = new File(number, String.Empty);
                    
                    //_BindingSourceFiles.Add(file);
                    //_BindingSourceFiles.EndEdit();
                    device.Files.Add(file);
                    _BindingSourceDevicesList.ResetCurrentItem();

                    if (device.Files.Count > 0)
                    {
                        _ToolStripMenuItemRemoveFile.Enabled = true;
                        _ContextMenuStripRecords.Enabled = true;
                    }
                    if (device.Files.Count > 9999)
                    {
                        _ToolStripMenuItemAddFile.Enabled = false;
                    }
                    return;
                }
            }

            throw new InvalidOperationException(
                "Неудалось добавить новый файл в устройство, все номера заняты");
        }

        /// <summary>
        /// 
        /// </summary>
        private void RemoveFile()
        {
            File file;
            ModbusSlaveDevice device;
            
            file = (File)_BindingSourceFile.Current;
            device = (ModbusSlaveDevice)_BindingSourceDevicesList.Current;
            device.Files.Remove(file);
            _BindingSourceDevicesList.ResetCurrentItem();
            
            //_BindingSourceFile.EndEdit();
            //_BindingSourceFile.ResetBindings(false);

            if (device.Files.Count == 0)
            {
                _ToolStripMenuItemRemoveFile.Enabled = false;
                _ContextMenuStripRecords.Enabled = false;
                _ToolStripMenuItemAddFile.Enabled = true;
            }
            else
            {
                _ToolStripMenuItemRemoveFile.Enabled = true;
                _ContextMenuStripRecords.Enabled = true;
            }
            return;
        }

        /// <summary>
        /// Добавляет запись файл
        /// </summary>
        /// <param name="file">файл с записями</param>
        private void AddRecord()
        {
            UInt16 number;
            File file;
            Record record;

            file = (File)_BindingSourceFile.Current;

            for (number = 0; number <= UInt16.MaxValue; number++)
            {
                if (!file.Records.Contains(number))
                {
                    record = new Record(number, 0, String.Empty);
                    file.Records.Add(record);
                    _BindingSourceFile.ResetCurrentItem();
                    

                    if (file.Records.Count > 0)
                    {
                        _ToolStripMenuItemRemoveRecord.Enabled = true;
                    }
                    if (file.Records.Count == UInt16.MaxValue)
                    {
                        _ToolStripMenuItemAddRecord.Enabled = false;
                    }

                    return;
                }
            }
            
            throw new InvalidOperationException(
                "Неудалось добавить новую запись в файл устройства, все номера заняты");
        }

        private void RemoveRecord()
        {
            Record record;
            File file;
            CurrencyManager manager;

            file = (File)_BindingSourceFile.Current;

            manager = _DataGridViewRecords.BindingContext[
                _DataGridViewRecords.DataSource, _DataGridViewRecords.DataMember] 
                as CurrencyManager;
            
            record = (Record)manager.Current;
            file.Records.Remove(record);
            
            _BindingSourceFile.ResetCurrentItem();

            if (file.Records.Count == 0)
            {
                _ToolStripMenuItemRemoveRecord.Enabled = false;
            }
            _ToolStripMenuItemAddRecord.Enabled = true;

            return;
        }

        private void EventHandler_ToolStripMenuItemAddDevice_Click(
            object sender, EventArgs e)
        {
            //ToolStripMenuItem menu = (ToolStripMenuItem)sender;
            AddDevice();
            return;
        }

        private void EventHandler_ToolStripMenuItemRemoveDevice_Click(
            object sender, EventArgs e)
        {
            RemoveDevice();
            return;
        }

        private void EventHandler_ToolStripMenuItemAddHoldingRegister_Click(
            object sender, EventArgs e)
        {
            AddHoldingRegister();
            return;
        }

        private void EventHandler_ToolStripMenuItemRemoveHoldingRegister_Click(
            object sender, EventArgs e)
        {
            RemoveHoldingRegister();
            return;
        }

        private void EventHandler_ToolStripMenuItemAddInputRegister_Click(
            object sender, EventArgs e)
        {
            AddInputRegister();
            return;
        }

        private void EventHandler_ToolStripMenuItemRemoveInputRegister_Click(
            object sender, EventArgs e)
        {
            RemoveInputRegister();
            return;
        }

        private void EventHandler_ToolStripMenuItemAddDiscreteInput_Click(
            object sender, EventArgs e)
        {
            AddDiscreteInput();
            return;
        }

        private void EventHandler_ToolStripMenuItemRemoveDiscreteInput_Click(
            object sender, EventArgs e)
        {
            RemoveDiscreteInput();
            return;
        }

        private void EventHandler_ToolStripMenuItemAddFile_Click(object sender, EventArgs e)
        {
            AddFile();
            return;
        }

        private void EventHandler_ToolStripMenuItemRemoveFile_Click(object sender, EventArgs e)
        {
            RemoveFile();
            return;
        }

        private void EventHandler_ToolStripMenuItemAddRecord_Click(object sender, EventArgs e)
        {
            AddRecord();
            return;
        }

        private void EventHandler_ToolStripMenuItemRemoveRecord_Click(object sender, EventArgs e)
        {
            RemoveRecord();
            return;
        }

        private void EventHandler_ToolStripMenuItemAddCoil_Click(object sender, EventArgs e)
        {
            AddCoil();
            return;
        }

        private void EventHandler_ToolStripMenuItemRemoveCoil_Click(object sender, EventArgs e)
        {
            RemoveCoil();
            return;
        }

        private void EventHandler_FormEditNetworkController_FormClosing(
            object sender, FormClosingEventArgs e)
        {
            return;
        }

        private void EventHandler_FormEditNetworkController_FormClosed(
            object sender, FormClosedEventArgs e)
        {
            DialogResult = DialogResult.OK;
            return;
        }

        #endregion
    }
}
