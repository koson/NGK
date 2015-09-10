using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
//
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel;

//===================================================================================
namespace Modbus.OSIModel.ApplicationLayer.Slave.Dialogs
{
    //===============================================================================
    public partial class FormEditNetworkController : Form
    {
        //---------------------------------------------------------------------------
        #region Fields And Properties
        //---------------------------------------------------------------------------
        /// <summary>
        /// Сеть Modbus
        /// </summary>
        private NetworkController _Network;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Устанавливает/возвращает редактируемую сеть Modbus
        /// </summary>
        public NetworkController Network
        {
            get { return _Network; }
            set 
            { 

                if (value != null)
                {
                    if (value.Status == Common.Controlling.Status.Stopped)
                    {
                        this._Network = value;
                        this._Network.DevicesListWasChanged +=
                            new EventHandler(EventHandler_Network_DevicesListWasChanged);
                        this._Network.NetworkChangedStatus +=
                            new EventHandler(EventHandler_Network_NetworkChangedStatus);
                    }
                    else
                    {
                        throw new InvalidOperationException("Попытка редактировать активную сеть");
                    }
                }
            }
        }
        //---------------------------------------------------------------------------
        private BindingSource _BindingSourceDevicesList;
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
        public FormEditNetworkController()
        {
            InitializeComponent();
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Methods
        //---------------------------------------------------------------------------
        private void EventHandler_Network_NetworkChangedStatus(
            object sender, EventArgs e)
        {
            NetworkController network = (NetworkController)sender;
            if ((network.Status == Common.Controlling.Status.Running) ||
                (network.Status == Common.Controlling.Status.Paused))
            {
                MessageBox.Show(this, "Внимание. Редактируемый сетевой контроллер изменил " + 
                    "своё состояние на активное. Продолжение редактирования сети опасно", 
                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return;
        }
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
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
            this._ContextMenuStripCoils.Enabled = true;
            this._ContextMenuStripDevicesList.Enabled = true;
            this._ContextMenuStripHoldingRegisters.Enabled = true;
            this._ContextMenuStripDiscretesInputs.Enabled = true;
            this._ContextMenuStripFiles.Enabled = true;
            this._ContextMenuStripInputRegisters.Enabled = true;
            this._ContextMenuStripRecords.Enabled = true;

            // Настраиваем источники данных
            this._BindingSourceFile = new BindingSource();
            //this._BindingSourceRecords.DataMember = "Records";

            // Для списка устройств
            this._BindingSourceDevicesList = new BindingSource();
            this._BindingSourceDevicesList.PositionChanged +=
                new EventHandler(EventHandler_BindingSourceDevicesList_PositionChanged);
            this._BindingSourceDevicesList.ListChanged +=
                new ListChangedEventHandler(EventHandler_BindingSourceDevicesList_ListChanged);
            this._BindingSourceDevicesList.DataMember = "Devices";
            this._BindingSourceDevicesList.DataSource = this._Network;

            // Настраиваем гриды для отображения данных устройства
            this.InitDataGridViewDevices();
            this.InitDataGridViewHoldingRegisters();
            this.InitDataGridViewCoils();
            this.InitDataGridViewInputRegisters();
            this.InitDataGridViewDiscretesInputs();
            this.InitDataGridViewFiles();
            this.InitDataGridViewRecords();

            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Запускается при старте приложения. Настраивает DataGridView
        /// </summary>
        private void InitDataGridViewDevices()
        {
            DataGridViewColumn column;

            this._DataGridViewDevicesList.AllowUserToAddRows = false;
            this._DataGridViewDevicesList.AllowUserToDeleteRows = false;
            this._DataGridViewDevicesList.AutoGenerateColumns = false;
            this._DataGridViewDevicesList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this._DataGridViewDevicesList.MultiSelect = false;
            this._DataGridViewDevicesList.Dock = DockStyle.Fill;
            this._DataGridViewDevicesList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this._DataGridViewDevicesList.CellParsing +=
                new DataGridViewCellParsingEventHandler(EventHandler_DataGridViewDevicesList_CellParsing);
            this._DataGridViewDevicesList.CellEndEdit +=
                new DataGridViewCellEventHandler(EventHandler_DataGridViewDevicesList_CellEndEdit);
            this._DataGridViewDevicesList.DataError +=
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
            this._DataGridViewDevicesList.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "NetworkController";
            column.DataPropertyName = "NetworkController";
            column.HeaderText = "Владелец";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(NetworkController);
            column.ReadOnly = true;
            this._DataGridViewDevicesList.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Description";
            column.DataPropertyName = "Description";
            column.HeaderText = "Примечания";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(String);
            column.ReadOnly = false;
            this._DataGridViewDevicesList.Columns.Add(column);

            DataGridViewComboBoxColumn cbxcolumn = new DataGridViewComboBoxColumn();
            cbxcolumn.Name = "Status";
            cbxcolumn.DataSource = Enum.GetValues(typeof(Common.Controlling.Status));
            cbxcolumn.DataPropertyName = "Status";
            cbxcolumn.HeaderText = "Состояние";
            cbxcolumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
            cbxcolumn.FlatStyle = FlatStyle.Flat;
            cbxcolumn.ValueType = typeof(Common.Controlling.Status);
            cbxcolumn.ReadOnly = false;
            this._DataGridViewDevicesList.Columns.Add(cbxcolumn);

            this._DataGridViewDevicesList.DataSource = null;
            this._DataGridViewDevicesList.DataSource = this._BindingSourceDevicesList;

            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Запускается при старте приложения. Настраивает DataGridView
        /// </summary>
        private void InitDataGridViewHoldingRegisters()
        {
            DataGridViewColumn column;
            
            // Настраиваем источник данных
            this._DataGridViewHoldingRegisters.AllowUserToAddRows = false;
            this._DataGridViewHoldingRegisters.AllowUserToDeleteRows = false;
            this._DataGridViewHoldingRegisters.AutoGenerateColumns = false;
            this._DataGridViewHoldingRegisters.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this._DataGridViewHoldingRegisters.MultiSelect = false;
            this._DataGridViewHoldingRegisters.Dock = DockStyle.Fill;
            this._DataGridViewHoldingRegisters.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this._DataGridViewHoldingRegisters.CellParsing +=
                new DataGridViewCellParsingEventHandler(EventHandler_DataGridViewHoldingRegisters_CellParsing);
            this._DataGridViewHoldingRegisters.CellEndEdit +=
                new DataGridViewCellEventHandler(_DataGridViewHoldingRegisters_CellEndEdit);
            this._DataGridViewHoldingRegisters.DataError +=
                new DataGridViewDataErrorEventHandler(EventHandler_DataGridViewHoldingRegisters_DataError);
            this._DataGridViewHoldingRegisters.DataSource = null;
            //this._DataGridViewHoldingRegisters.DataSource = this._BindingSourceHoldingRegisters;
            this._DataGridViewHoldingRegisters.DataMember = "HoldingRegisters";
            this._DataGridViewHoldingRegisters.DataSource = this._BindingSourceDevicesList;

            column = new DataGridViewTextBoxColumn();
            column.Name = "Address";
            column.DataPropertyName = "Address";
            column.HeaderText = "Адрес";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(UInt16);
            column.Visible = true;
            column.ReadOnly = false;
            this._DataGridViewHoldingRegisters.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Value";
            column.DataPropertyName = "Value";
            column.HeaderText = "Значение";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(UInt16);
            column.Visible = true;
            column.ReadOnly = false;
            this._DataGridViewHoldingRegisters.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Description";
            column.DataPropertyName = "Description";
            column.HeaderText = "Примечания";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(String);
            column.Visible = true;
            column.ReadOnly = false;
            this._DataGridViewHoldingRegisters.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "ParameterType";
            column.DataPropertyName = "ParameterType";
            column.HeaderText = "Тип данных";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(ModbusParameterType);
            column.Visible = true;
            column.ReadOnly = true;
            this._DataGridViewHoldingRegisters.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Device";
            column.DataPropertyName = "Device";
            column.HeaderText = "Владелец";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(Device);
            column.Visible = true;
            column.ReadOnly = true;
            this._DataGridViewHoldingRegisters.Columns.Add(column);

            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Запускается при старте приложения. Настраивает DataGridView
        /// </summary>
        private void InitDataGridViewInputRegisters()
        {
            DataGridViewColumn column;

            this._DataGridViewInputRegisters.AllowUserToAddRows = false;
            this._DataGridViewInputRegisters.AllowUserToDeleteRows = false;
            this._DataGridViewInputRegisters.AutoGenerateColumns = false;
            this._DataGridViewInputRegisters.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this._DataGridViewInputRegisters.MultiSelect = false;
            this._DataGridViewInputRegisters.Dock = DockStyle.Fill;
            this._DataGridViewInputRegisters.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this._DataGridViewInputRegisters.CellParsing +=
                new DataGridViewCellParsingEventHandler(EventHandler_DataGridViewInputRegisters_CellParsing);
            this._DataGridViewInputRegisters.CellEndEdit +=
                new DataGridViewCellEventHandler(_DataGridViewInputRegisters_CellEndEdit);
            this._DataGridViewInputRegisters.DataError +=
                new DataGridViewDataErrorEventHandler(EventHandler_DataGridViewInputRegisters_DataError);
            this._DataGridViewInputRegisters.DataSource = null;
            //this._DataGridViewInputRegisters.DataSource = this._BindingSourceInputRegisters;
            this._DataGridViewInputRegisters.DataMember = "InputRegisters";
            this._DataGridViewInputRegisters.DataSource = this._BindingSourceDevicesList;

            column = new DataGridViewTextBoxColumn();
            column.Name = "Address";
            column.DataPropertyName = "Address";
            column.HeaderText = "Адрес";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(UInt16);
            column.Visible = true;
            column.ReadOnly = false;
            this._DataGridViewInputRegisters.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Value";
            column.DataPropertyName = "Value";
            column.HeaderText = "Значение";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(UInt16);
            column.Visible = true;
            column.ReadOnly = false;
            this._DataGridViewInputRegisters.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Description";
            column.DataPropertyName = "Description";
            column.HeaderText = "Примечания";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(String);
            column.Visible = true;
            column.ReadOnly = false;
            this._DataGridViewInputRegisters.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "ParameterType";
            column.DataPropertyName = "ParameterType";
            column.HeaderText = "Тип данных";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(ModbusParameterType);
            column.Visible = true;
            column.ReadOnly = true;
            this._DataGridViewInputRegisters.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Device";
            column.DataPropertyName = "Device";
            column.HeaderText = "Владелец";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(Device);
            column.Visible = true;
            column.ReadOnly = true;
            this._DataGridViewInputRegisters.Columns.Add(column);
            
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Запускается при старте приложения. Настраивает DataGridView
        /// </summary>
        private void InitDataGridViewCoils()
        {
            DataGridViewColumn column;

            this._DataGridViewCoils.AllowUserToAddRows = false;
            this._DataGridViewCoils.AllowUserToDeleteRows = false;
            this._DataGridViewCoils.AutoGenerateColumns = false;
            this._DataGridViewCoils.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this._DataGridViewCoils.MultiSelect = false;
            this._DataGridViewCoils.Dock = DockStyle.Fill;
            this._DataGridViewCoils.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this._DataGridViewCoils.CellParsing +=
                new DataGridViewCellParsingEventHandler(this.EventHandler_DataGridViewCoils_CellParsing);
            this._DataGridViewCoils.CellEndEdit +=
                new DataGridViewCellEventHandler(EventHadler_DataGridViewCoils_CellEndEdit);
            this._DataGridViewCoils.DataError +=
                new DataGridViewDataErrorEventHandler(EventHandler_DataGridViewCoils_DataError);
            this._DataGridViewCoils.DataSource = null;
            //this._DataGridViewCoils.DataSource = this._BindingSourceCoils;
            //this._DataGridViewCoils.DataSource = this._BingingSourceCurrentDevice;
            this._DataGridViewCoils.DataMember = "Coils";
            this._DataGridViewCoils.DataSource = this._BindingSourceDevicesList;

            column = new DataGridViewTextBoxColumn();
            column.Name = "Address";
            column.HeaderText = "Адрес";
            column.DataPropertyName = "Address";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.Visible = true;
            column.ReadOnly = false;
            this._DataGridViewCoils.Columns.Add(column);

            column = new DataGridViewCheckBoxColumn();
            column.Name = "Value";
            column.HeaderText = "Значение";
            column.DataPropertyName = "Value";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(Boolean);
            column.Visible = true;
            column.ReadOnly = false;
            this._DataGridViewCoils.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Description";
            column.HeaderText = "Примечания";
            column.DataPropertyName = "Description";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(String);
            column.Visible = true;
            column.ReadOnly = false;
            this._DataGridViewCoils.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "ParameterType";
            column.HeaderText = "Тип данных";
            column.DataPropertyName = "ParameterType";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(ModbusParameterType);
            column.Visible = true;
            column.ReadOnly = true;
            this._DataGridViewCoils.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Device";
            column.HeaderText = "Владелец";
            column.DataPropertyName = "Device";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(Device);
            column.Visible = true;
            column.ReadOnly = true;
            this._DataGridViewCoils.Columns.Add(column);
            
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Запускается при старте приложения. Настраивает DataGridView
        /// </summary>
        private void InitDataGridViewDiscretesInputs()
        {
            DataGridViewColumn column;

            this._DataGridViewDiscretesInputs.AllowUserToAddRows = false;
            this._DataGridViewDiscretesInputs.AllowUserToDeleteRows = false;
            this._DataGridViewDiscretesInputs.AutoGenerateColumns = false;
            this._DataGridViewDiscretesInputs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this._DataGridViewDiscretesInputs.MultiSelect = false;
            this._DataGridViewDiscretesInputs.Dock = DockStyle.Fill;
            this._DataGridViewDiscretesInputs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this._DataGridViewDiscretesInputs.CellParsing +=
                new DataGridViewCellParsingEventHandler(EventHandler_DataGridViewDiscretesInputs_CellParsing);
            this._DataGridViewDiscretesInputs.CellEndEdit +=
                new DataGridViewCellEventHandler(_DataGridViewDiscretesInputs_CellEndEdit);
            this._DataGridViewDiscretesInputs.DataError +=
                new DataGridViewDataErrorEventHandler(EventHandler_DataGridViewDiscretesInputs_DataError);
            this._DataGridViewDiscretesInputs.DataSource = null;
            //this._DataGridViewDiscretesInputs.DataSource = this._BindingSourceDiscrestesInputs;
            this._DataGridViewDiscretesInputs.DataMember = "DiscretesInputs";
            this._DataGridViewDiscretesInputs.DataSource = this._BindingSourceDevicesList;

            column = new DataGridViewTextBoxColumn();
            column.Name = "Address";
            column.DataPropertyName = "Address";
            column.HeaderText = "Адрес";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(UInt16);
            column.Visible = true;
            column.ReadOnly = false;
            this._DataGridViewDiscretesInputs.Columns.Add(column);

            column = new DataGridViewCheckBoxColumn();
            column.Name = "Value";
            column.DataPropertyName = "Value";
            column.HeaderText = "Значение";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(Boolean);
            column.Visible = true;
            column.ReadOnly = false;
            this._DataGridViewDiscretesInputs.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Description";
            column.DataPropertyName = "Description";
            column.HeaderText = "Примечания";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(String);
            column.Visible = true;
            column.ReadOnly = false;
            this._DataGridViewDiscretesInputs.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "ParameterType";
            column.DataPropertyName = "ParameterType";
            column.HeaderText = "Тип данных";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(ModbusParameterType);
            column.Visible = true;
            column.ReadOnly = true;
            this._DataGridViewDiscretesInputs.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Device";
            column.DataPropertyName = "Device";
            column.HeaderText = "Владелец";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(Device);
            column.Visible = true;
            column.ReadOnly = true;
            this._DataGridViewDiscretesInputs.Columns.Add(column);

            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        private void InitDataGridViewFiles()
        {
            DataGridViewColumn column;
            //CurrencyManager cManager;

            this._DataGridViewFiles.AllowUserToAddRows = false;
            this._DataGridViewFiles.AllowUserToDeleteRows = false;
            this._DataGridViewFiles.AutoGenerateColumns = false;
            this._DataGridViewFiles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this._DataGridViewFiles.MultiSelect = false;
            this._DataGridViewFiles.Dock = DockStyle.Fill;
            this._DataGridViewFiles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this._DataGridViewFiles.CellParsing +=
                new DataGridViewCellParsingEventHandler(EventHandler_DataGridViewFiles_CellParsing);
            this._DataGridViewFiles.CellEndEdit +=
                new DataGridViewCellEventHandler(EventHandler_DataGridViewFiles_CellEndEdit);
            this._DataGridViewFiles.DataError +=
                new DataGridViewDataErrorEventHandler(EventHandler_DataGridViewFiles_DataError);
            this._DataGridViewFiles.SelectionChanged += 
                new EventHandler(EventHandler_DataGridViewFiles_SelectionChanged);
            this._DataGridViewFiles.DataSource = null;
            this._DataGridViewFiles.DataMember = "Files";
            this._DataGridViewFiles.DataSource = this._BindingSourceDevicesList;

            //cManager = this._DataGridViewFiles.BindingContext[
            //    this._DataGridViewFiles.DataSource, this._DataGridViewFiles.DataMember] as CurrencyManager;
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
            this._DataGridViewFiles.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Description";
            column.DataPropertyName = "Description";
            column.HeaderText = "Примечания";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(String);
            column.Visible = true;
            column.ReadOnly = false;
            this._DataGridViewFiles.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Device";
            column.DataPropertyName = "Device";
            column.HeaderText = "Владелец";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(Device);
            column.Visible = true;
            column.ReadOnly = true;
            this._DataGridViewFiles.Columns.Add(column);
            
            return;
        }

        //---------------------------------------------------------------------------
        /// <summary>
        /// Запускается при старте приложения. Настраивает DataGridView
        /// </summary>
        private void InitDataGridViewRecords()
        {
            DataGridViewColumn column;

            this._DataGridViewRecords.AllowUserToAddRows = false;
            this._DataGridViewRecords.AllowUserToDeleteRows = false;
            this._DataGridViewRecords.AutoGenerateColumns = false;
            this._DataGridViewRecords.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this._DataGridViewRecords.MultiSelect = false;
            this._DataGridViewRecords.Dock = DockStyle.Fill;
            this._DataGridViewRecords.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this._DataGridViewRecords.CellParsing +=
                new DataGridViewCellParsingEventHandler(EventHandler_DataGridViewRecords_CellParsing);
            this._DataGridViewRecords.CellEndEdit +=
                new DataGridViewCellEventHandler(EventHandler_DataGridViewRecords_CellEndEdit);
            this._DataGridViewRecords.DataError +=
                new DataGridViewDataErrorEventHandler(EventHandler_DataGridViewRecords_DataError);
            //this._DataGridViewRecords.DataMember = String.Empty;
            //this._DataGridViewRecords.DataSource = this._BindingSourceRecords;

            column = new DataGridViewTextBoxColumn();
            column.Name = "Address";
            column.HeaderText = "Номер записи";
            column.DataPropertyName = "Address";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(UInt16);
            column.Visible = true;
            column.ReadOnly = false;
            this._DataGridViewRecords.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Value";
            column.HeaderText = "Значение";
            column.DataPropertyName = "Value";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(UInt16); 
            column.Visible = true;
            column.ReadOnly = false;
            this._DataGridViewRecords.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Description";
            column.HeaderText = "Примечания";
            column.DataPropertyName = "Description";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(String);
            column.Visible = true;
            column.ReadOnly = false;
            this._DataGridViewRecords.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "ParameterType";
            column.HeaderText = "Тип данных";
            column.DataPropertyName = "ParameterType";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(ModbusParameterType);
            column.Visible = true;
            column.ReadOnly = true;
            this._DataGridViewRecords.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.Name = "Device";
            column.HeaderText = "Владелец";
            column.DataPropertyName = "Device";
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.ValueType = typeof(Device);
            column.Visible = true;
            column.ReadOnly = true;
            this._DataGridViewRecords.Columns.Add(column);

            return;
        }
        //---------------------------------------------------------------------------
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

                this._BindingSourceFile = new BindingSource(file, String.Empty);

                this._DataGridViewRecords.DataSource = null;
                this._DataGridViewRecords.DataSource = this._BindingSourceFile;
                this._DataGridViewRecords.DataMember = "Records";

                this._DataGridViewRecords.Enabled = true;
                //this._ContextMenuStripRecords.Enabled = true;
            }
            else
            {
                this._BindingSourceFile = new BindingSource(null, String.Empty);

                this._DataGridViewRecords.DataSource = null;
                this._DataGridViewRecords.DataSource = this._BindingSourceFile;
                this._DataGridViewRecords.DataMember = String.Empty;

                this._DataGridViewRecords.Enabled = false;
                //this._ContextMenuStripRecords.Enabled = false;
            }
            return;
        }
        //---------------------------------------------------------------------------
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
                            this._TabControlDevice.Enabled = false;
                        }
                        break; 
                    }
                case ListChangedType.ItemAdded:
                    {
                        // Список устройств всегда больше 0, разрешаем
                        // работу элементов окна.
                        this._TabControlDevice.Enabled = true;
                        break;
                    }
                case ListChangedType.Reset:
                    {
                        if (bs.Count > 0)
                        {
                            this._TabControlDevice.Enabled = true;
                        }
                        else
                        {
                            this._TabControlDevice.Enabled = false;
                        }
                        break;
                    }
                case ListChangedType.ItemMoved:
                    { break; }
            }
            return;
        }
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
        private void EventHandler_DataGridViewDevicesList_CellEndEdit(
            object sender, DataGridViewCellEventArgs e)
        {
            DataGridView control = (DataGridView)sender;
            control.Rows[e.RowIndex].ErrorText = String.Empty;
            return;
        }
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
        private void EventHadler_DataGridViewCoils_CellEndEdit(
            object sender, DataGridViewCellEventArgs e)
        {
            DataGridView control = (DataGridView)sender;
            control.Rows[e.RowIndex].ErrorText = String.Empty;
            return;
        }
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
        private void _DataGridViewDiscretesInputs_CellEndEdit(
            object sender, DataGridViewCellEventArgs e)
        {
            DataGridView control = (DataGridView)sender;
            control.Rows[e.RowIndex].ErrorText = String.Empty;
            return;
        }
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
        private void _DataGridViewInputRegisters_CellEndEdit(
            object sender, DataGridViewCellEventArgs e)
        {
            DataGridView control = (DataGridView)sender;
            control.Rows[e.RowIndex].ErrorText = String.Empty;
            return;
        }
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
        private void _DataGridViewHoldingRegisters_CellEndEdit(
            object sender, DataGridViewCellEventArgs e)
        {
            DataGridView control = (DataGridView)sender;
            control.Rows[e.RowIndex].ErrorText = String.Empty;
            return;
        }
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
        private void EventHandler_DataGridViewRecords_CellEndEdit(
            object sender, DataGridViewCellEventArgs e)
        {
            DataGridView control = (DataGridView)sender;
            control.Rows[e.RowIndex].ErrorText = String.Empty;
            return;
        }
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
        private void EventHandler_DataGridViewFiles_CellEndEdit(
            object sender, DataGridViewCellEventArgs e)
        {
            DataGridView control = (DataGridView)sender;
            control.Rows[e.RowIndex].ErrorText = String.Empty;
            return;
        }
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
        /// <summary>
        /// Добавляет устройство в сеть
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        private void AddDevice()
        {
            Byte address;
            Device device;
            
            // Находим свобоный адрес и добавляем устройство с этим адресом 
            for (address = 1; address < 248; address++)
			{
                if (!this._Network.Devices.Contains(address))
                {
                    device = new Device(address);
                    this._BindingSourceDevicesList.Add(device);

                    if (this._BindingSourceDevicesList.Count > 0)
                    {
                        this._ContextMenuStripCoils.Enabled = true;
                        this._ContextMenuStripDiscretesInputs.Enabled = true;
                        this._ContextMenuStripFiles.Enabled = true;
                        this._ContextMenuStripHoldingRegisters.Enabled = true;
                        this._ContextMenuStripInputRegisters.Enabled = true;
                        this._ContextMenuStripRecords.Enabled = false;

                        this._DataGridViewCoils.Enabled = true;
                        this._DataGridViewDiscretesInputs.Enabled = true;
                        this._DataGridViewFiles.Enabled = true;
                        this._DataGridViewHoldingRegisters.Enabled = true;
                        this._DataGridViewInputRegisters.Enabled = true;
                        this._DataGridViewRecords.Enabled = false;
                    }
                    return;
                }
			}
            throw new InvalidOperationException(
                "Не удалось добавить новое устройство, все адреса заняты");
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Удаляет устройство из сети
        /// </summary>
        private void RemoveDevice()
        {
            this._BindingSourceDevicesList.RemoveCurrent();
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Добавляет регистр входа/вывода в текущее устройство
        /// </summary>
        private void AddHoldingRegister()
        {
            UInt16 address;
            Device device;
            HoldingRegister register;

            device = (Device)this._BindingSourceDevicesList.Current;

            for (address = 0; address <= UInt16.MaxValue; address++)
            {
                if (!device.HoldingRegisters.Contains(address))
                {
                    register = new HoldingRegister(address, 0, String.Empty);

                    device.HoldingRegisters.Add(register);
                    this._BindingSourceDevicesList.ResetCurrentItem();

                    if (device.HoldingRegisters.Count > 0)
                    {
                        this._ToolStripMenuItemRemoveHoldingRegister.Enabled = true;
                    }
                    if (device.HoldingRegisters.Count == UInt16.MaxValue)
                    {
                        this._ToolStripMenuItemAddHoldingRegister.Enabled = false;
                    }

                    return;
                }
            }
            throw new InvalidOperationException(
                "Не удалось добавить новый holding register в устройство, все адреса заняты");
        }
        //---------------------------------------------------------------------------
        private void RemoveHoldingRegister()
        {
            CurrencyManager manager;
            HoldingRegister register;
            Device device;

            manager = this._DataGridViewHoldingRegisters.BindingContext[
                this._DataGridViewHoldingRegisters.DataSource, 
                this._DataGridViewHoldingRegisters.DataMember] as CurrencyManager;
            register = (HoldingRegister)manager.Current;
            device = (Device)this._BindingSourceDevicesList.Current;
            device.HoldingRegisters.Remove(register);
            this._BindingSourceDevicesList.ResetCurrentItem();

            if (device.HoldingRegisters.Count == 0)
            {
                this._ToolStripMenuItemRemoveHoldingRegister.Enabled = false;
            }
            this._ToolStripMenuItemAddHoldingRegister.Enabled = true;
            return;
        }
        //---------------------------------------------------------------------------
        private void AddInputRegister()
        {
            UInt16 address;
            Device device;
            InputRegister register;

            device = (Device)this._BindingSourceDevicesList.Current;

            for (address = 0; address <= UInt16.MaxValue; address++)
            {
                if (!device.InputRegisters.Contains(address))
                {
                    register = new InputRegister(address, 0, String.Empty);
                    device.InputRegisters.Add(register);
                    this._BindingSourceDevicesList.ResetCurrentItem();

                    if (device.InputRegisters.Count > 0)
                    {
                        this._ToolStripMenuItemRemoveInputRegister.Enabled = true;
                    }
                    if (device.InputRegisters.Count == UInt16.MaxValue)
                    {
                        this._ToolStripMenuItemAddInputRegister.Enabled = false;
                    }
                    return;
                }
            }
            throw new InvalidOperationException(
                "Не удалось добавить новый input register в устройство, все адреса заняты");
        }
        //---------------------------------------------------------------------------
        private void RemoveInputRegister()
        {
            CurrencyManager manager;
            InputRegister register;
            Device device;

            manager = this._DataGridViewInputRegisters.BindingContext[
                this._DataGridViewInputRegisters.DataSource,
                this._DataGridViewInputRegisters.DataMember] as CurrencyManager;
            register = (InputRegister)manager.Current;
            device = (Device)this._BindingSourceDevicesList.Current;
            device.InputRegisters.Remove(register);
            this._BindingSourceDevicesList.ResetCurrentItem();

            if (device.InputRegisters.Count == 0)
            {
                this._ToolStripMenuItemRemoveInputRegister.Enabled = false;
            }
            this._ToolStripMenuItemAddInputRegister.Enabled = true;

            return;
        }
        //---------------------------------------------------------------------------
        private void AddCoil()
        {
            UInt16 address;
            Device device;
            Coil coil;

            device = (Device)this._BindingSourceDevicesList.Current;

            for (address = 0; address <= UInt16.MaxValue; address++)
            {
                if (!device.Coils.Contains(address))
                {
                    coil = new Coil(address, false, String.Empty);
                    device.Coils.Add(coil);
                    this._BindingSourceDevicesList.ResetCurrentItem();

                    if (device.Coils.Count > 0)
                    {
                        this._ToolStripMenuItemRemoveCoil.Enabled = true;
                    }
                    if (device.Coils.Count == UInt16.MaxValue)
                    {
                        this._ToolStripMenuItemAddCoil.Enabled = false;
                    }
                    return;
                }
            }
            throw new InvalidOperationException(
                "Не удалось добавить новый coil в устройство, все адреса заняты");
        }
        //---------------------------------------------------------------------------
        private void RemoveCoil()
        {
            CurrencyManager manager;
            Coil coil;
            Device device;

            manager = this._DataGridViewCoils.BindingContext[
                this._DataGridViewCoils.DataSource,
                this._DataGridViewCoils.DataMember] as CurrencyManager;
            coil = (Coil)manager.Current;
            device = (Device)this._BindingSourceDevicesList.Current;
            device.Coils.Remove(coil);
            this._BindingSourceDevicesList.ResetCurrentItem();

            if (device.Coils.Count == 0)
            {
                this._ToolStripMenuItemRemoveCoil.Enabled = false;
            }

            this._ToolStripMenuItemAddCoil.Enabled = true;

            return;
        }
        //---------------------------------------------------------------------------
        private void AddDiscreteInput()
        {
            UInt16 address;
            Device device;
            DiscreteInput input;

            device = (Device)this._BindingSourceDevicesList.Current;

            for (address = 0; address <= UInt16.MaxValue; address++)
            {
                if (!device.DiscretesInputs.Contains(address))
                {
                    input = new DiscreteInput(address, false, String.Empty);
                    device.DiscretesInputs.Add(input);
                    this._BindingSourceDevicesList.ResetCurrentItem();

                    if (device.DiscretesInputs.Count > 0)
                    {
                        this._ToolStripMenuItemRemoveDiscreteInput.Enabled = true;
                    }
                    if (device.DiscretesInputs.Count == UInt16.MaxValue)
                    {
                        this._ToolStripMenuItemAddDiscreteInput.Enabled = false;
                    }
                    return;
                }
            }
            throw new InvalidOperationException(
                "Не удалось добавить новый discrete input в устройство, все адреса заняты");
        }
        //---------------------------------------------------------------------------
        private void RemoveDiscreteInput()
        {
            CurrencyManager manager;
            DiscreteInput input;
            Device device;

            manager = this._DataGridViewDiscretesInputs.BindingContext[
                this._DataGridViewDiscretesInputs.DataSource,
                this._DataGridViewDiscretesInputs.DataMember] as CurrencyManager;
            input = (DiscreteInput)manager.Current;
            device = (Device)this._BindingSourceDevicesList.Current;
            device.DiscretesInputs.Remove(input);
            this._BindingSourceDevicesList.ResetCurrentItem();

            if (device.DiscretesInputs.Count == 0)
            {
                this._ToolStripMenuItemRemoveDiscreteInput.Enabled = false;
            }
            this._ToolStripMenuItemAddDiscreteInput.Enabled = true;
            return;
        }
        //---------------------------------------------------------------------------
        private void AddFile()
        {
            UInt16 number;
            Device device;
            File file;

            device = (Device)this._BindingSourceDevicesList.Current;

            for (number = 1; number < 10000; number++)
            {
                if (!device.Files.Contains(number))
                {
                    file = new File(number, String.Empty);
                    
                    //this._BindingSourceFiles.Add(file);
                    //this._BindingSourceFiles.EndEdit();
                    device.Files.Add(file);
                    this._BindingSourceDevicesList.ResetCurrentItem();

                    if (device.Files.Count > 0)
                    {
                        this._ToolStripMenuItemRemoveFile.Enabled = true;
                        this._ContextMenuStripRecords.Enabled = true;
                    }
                    if (device.Files.Count > 9999)
                    {
                        this._ToolStripMenuItemAddFile.Enabled = false;
                    }
                    return;
                }
            }

            throw new InvalidOperationException(
                "Неудалось добавить новый файл в устройство, все номера заняты");
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        private void RemoveFile()
        {
            File file;
            Device device;
            
            file = (File)this._BindingSourceFile.Current;
            device = (Device)this._BindingSourceDevicesList.Current;
            device.Files.Remove(file);
            this._BindingSourceDevicesList.ResetCurrentItem();
            
            //this._BindingSourceFile.EndEdit();
            //this._BindingSourceFile.ResetBindings(false);

            if (device.Files.Count == 0)
            {
                this._ToolStripMenuItemRemoveFile.Enabled = false;
                this._ContextMenuStripRecords.Enabled = false;
                this._ToolStripMenuItemAddFile.Enabled = true;
            }
            else
            {
                this._ToolStripMenuItemRemoveFile.Enabled = true;
                this._ContextMenuStripRecords.Enabled = true;
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Добавляет запись файл
        /// </summary>
        /// <param name="file">файл с записями</param>
        private void AddRecord()
        {
            UInt16 number;
            File file;
            Record record;

            file = (File)this._BindingSourceFile.Current;

            for (number = 0; number <= UInt16.MaxValue; number++)
            {
                if (!file.Records.Contains(number))
                {
                    record = new Record(number, 0, String.Empty);
                    file.Records.Add(record);
                    this._BindingSourceFile.ResetCurrentItem();
                    

                    if (file.Records.Count > 0)
                    {
                        this._ToolStripMenuItemRemoveRecord.Enabled = true;
                    }
                    if (file.Records.Count == UInt16.MaxValue)
                    {
                        this._ToolStripMenuItemAddRecord.Enabled = false;
                    }

                    return;
                }
            }
            
            throw new InvalidOperationException(
                "Неудалось добавить новую запись в файл устройства, все номера заняты");
        }
        //---------------------------------------------------------------------------
        private void RemoveRecord()
        {
            Record record;
            File file;
            CurrencyManager manager;

            file = (File)this._BindingSourceFile.Current;

            manager = this._DataGridViewRecords.BindingContext[
                this._DataGridViewRecords.DataSource, this._DataGridViewRecords.DataMember] 
                as CurrencyManager;
            
            record = (Record)manager.Current;
            file.Records.Remove(record);
            
            this._BindingSourceFile.ResetCurrentItem();

            if (file.Records.Count == 0)
            {
                this._ToolStripMenuItemRemoveRecord.Enabled = false;
            }
            this._ToolStripMenuItemAddRecord.Enabled = true;

            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_ToolStripMenuItemAddDevice_Click(
            object sender, EventArgs e)
        {
            //ToolStripMenuItem menu = (ToolStripMenuItem)sender;
            this.AddDevice();
            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_ToolStripMenuItemRemoveDevice_Click(
            object sender, EventArgs e)
        {
            this.RemoveDevice();
            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_ToolStripMenuItemAddHoldingRegister_Click(
            object sender, EventArgs e)
        {
            this.AddHoldingRegister();
            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_ToolStripMenuItemRemoveHoldingRegister_Click(
            object sender, EventArgs e)
        {
            this.RemoveHoldingRegister();
            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_ToolStripMenuItemAddInputRegister_Click(
            object sender, EventArgs e)
        {
            this.AddInputRegister();
            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_ToolStripMenuItemRemoveInputRegister_Click(
            object sender, EventArgs e)
        {
            this.RemoveInputRegister();
            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_ToolStripMenuItemAddDiscreteInput_Click(
            object sender, EventArgs e)
        {
            this.AddDiscreteInput();
            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_ToolStripMenuItemRemoveDiscreteInput_Click(
            object sender, EventArgs e)
        {
            this.RemoveDiscreteInput();
            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_ToolStripMenuItemAddFile_Click(object sender, EventArgs e)
        {
            this.AddFile();
            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_ToolStripMenuItemRemoveFile_Click(object sender, EventArgs e)
        {
            this.RemoveFile();
            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_ToolStripMenuItemAddRecord_Click(object sender, EventArgs e)
        {
            this.AddRecord();
            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_ToolStripMenuItemRemoveRecord_Click(object sender, EventArgs e)
        {
            this.RemoveRecord();
            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_ToolStripMenuItemAddCoil_Click(object sender, EventArgs e)
        {
            this.AddCoil();
            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_ToolStripMenuItemRemoveCoil_Click(object sender, EventArgs e)
        {
            this.RemoveCoil();
            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_FormEditNetworkController_FormClosing(
            object sender, FormClosingEventArgs e)
        {
            return;
        }
        //---------------------------------------------------------------------------
        private void EventHandler_FormEditNetworkController_FormClosed(
            object sender, FormClosedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            return;
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file