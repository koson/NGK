using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel;
using NGK.CAN.DataTypes;
using NGK.CAN.ApplicationLayer.Network.Master;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary;
using NGK.CorrosionMonitoringSystem.Forms;
using NGK.CorrosionMonitoringSystem.Forms.Controls;
using NGK.CorrosionMonitoringSystem.Models;
using NGK.CorrosionMonitoringSystem.Core;
using NGK.CorrosionMonitoringSystem.DL;
using Common.Controlling;

namespace NGK.CorrosionMonitoringSystem.BL
{
    /// <summary>
    /// Класс реализует объект бизнес-логики
    /// </summary>
    public class BLController : ISynchronizeInvoke
    {
        /// <summary>
        /// Экранные виды для системы мониторинга 
        /// </summary>
        public enum SystemView
        {
            DevicesListView,
            DetailsDeviceView,
            PivotTableView
        }

        #region Fields And Properties
        private const int NETWORK_CAN_0 = 0;
        /// <summary>
        /// Объект уровня базы данных
        /// </summary>
        private NetworksManager _CanNetworksManager;
        /// <summary>
        /// Объект уровня представления данных
        /// </summary>
        private CorrosionMonitoringSystemForm _Presenter;
        /// <summary>
        /// 
        /// </summary>
        private SystemView _View;
        /// <summary>
        /// Устанавливает вид экрана 
        /// </summary>
        private SystemView View
        {
            get { return this._View; }
            //set
            //{
            //    switch (value)
            //    {
            //        case SystemView.DevicesListView: { break; }
            //        case SystemView.DetailsDeviceView: { break; }
            //        case SystemView.PivotTableView: { break; }
            //        default:
            //            { throw new Exception("Попытка установить неизвестный вид отображения данных системы"); }
            //    }
            //    this._View = value;
            //}
        }
        /// <summary>
        /// Таймер для общего назначения
        /// </summary>
        private Timer _GeneralTimer;
        /// <summary>
        /// Класс для реализации сводной таблицы параметров
        /// </summary>
        private ParametersPivotTable _PivotTable;
        /// <summary>
        /// Делегат для обработки события изменения позиции курсора в datagridview
        /// </summary>
        private EventHandler _EventHandler_DevicesListCurrencyManager_PositionChanged;
        private CanNetworkServiceAdapter _CanNetworkAdapter;
        private ModbusServiceAdapter _ModbusNetworkAdapter;
        #endregion
        
        #region Constructors

        private BLController() 
        {
            throw new NotImplementedException();
        }

        public BLController(NetworksManager networksManager, 
            CorrosionMonitoringSystemForm presenter)
        {
            _CanNetworksManager = networksManager;
            _CanNetworkAdapter = new CanNetworkServiceAdapter();

            _Presenter = presenter;
            _Presenter.Loaded += 
                new EventHandler(EventHandler_Presenter_Loaded);
            _Presenter.SystemButtonClick += 
                new SystemButtonClickEventHandler(
                EventHandler_Presenter_SystemButtonClick);
            
            // Инициализируем сводную таблицу параметров //TODO
            int x = 0;
            _PivotTable = new ParametersPivotTable(
                _CanNetworksManager.Networks[x].Devices.ToArray()); //NETWORK_CAN_0
            _PivotTable.TableWasUpdated += 
                new EventHandler(EvetnHandler_PivotTable_TableWasUpdated);


            // Настраиваем сеть модбас
            //_ModbusNetworkAdapter = new ModbusServiceAdapter();

            // Настраиваем таймер
            _GeneralTimer = new Timer();
            _GeneralTimer.Interval = 1000;
            _GeneralTimer.Tick += new EventHandler(EventHandler_GeneralTimer_Tick);
            _GeneralTimer.Start();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Обработчик события обновления данных в сводной таблице
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EvetnHandler_PivotTable_TableWasUpdated(object sender, EventArgs e)
        {
            this._Presenter.DataGridViewPivotTable.Update();
            this._Presenter.DataGridViewPivotTable.Refresh();
            return;
        }
        /// <summary>
        /// Обработчик события срабатываения таймера общенго назначения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_GeneralTimer_Tick(object sender, EventArgs e)
        {
            // Оновляем данные в сводной таблице
            switch (_View)
            {
                case SystemView.PivotTableView:
                    {
                        _PivotTable.Update(); break;
                    }
                case SystemView.DetailsDeviceView:
                    {
                        // Обновляем список значения объектов словаря устройства
                        UpdateDetailsDevice();
                        _Presenter.DataGridViewDevicesList.Update();
                        _Presenter.DataGridViewDevicesList.Refresh();
                        break;
                    }
                case SystemView.DevicesListView:
                    {
                        break;
                    }
            }
            // Обновляем время в строке состояния
            _Presenter.DateTime = DateTime.Now;
            return;
        }
        /// <summary>
        /// Обновляем список значений объектов для выбранного 
        /// устройства 
        /// </summary>
        private void UpdateDetailsDevice()
        {
            //List<DataObjectInfo> objectDictionary;
            NetworkDevice device;

            if (_View != SystemView.DetailsDeviceView)
            {
                return;
            }
            
            // Получаем устройство
            device = (NetworkDevice)_Presenter.DataGridViewDevicesList.DataSource;
            _CanNetworkAdapter.UpdateObjectDictionary(device);
        }
        /// <summary>
        /// Сбрасывает 
        /// </summary>
        /// <param name="device"></param>
        private void ResetDeviceStatusError(Device device)
        {
            if ((device.Status == DeviceStatus.CommunicationError) ||
            (device.Status == DeviceStatus.ConfigurationError))
            {
                lock (device)
                {
                    device.Status = DeviceStatus.Stopped;
                }
            }
            return;
        }
        /// <summary>
        /// Возвращает состояние системы. Если хоть одна сеть запущена,
        /// значит система работает. Если все сети остановлены - система остановлена
        /// </summary>
        /// <returns></returns>
        private Status GetSystemStatus()
        {
            for (int i = 0; i < this._CanNetworksManager.Networks.Count; i++)
            {
                if (this._CanNetworksManager.Networks[i].Status == Status.Running)
                {
                    return Status.Running;
                }
            }
            return Status.Stopped;
        }
        /// <summary>
        /// Выполняет комманды от кнопок панели
        /// </summary>
        /// <param name="buttons"></param>
        private void ExecuteCommand(string buttonName)
        {
            switch (this.View)
            {
                case SystemView.DevicesListView: // В данный момент отображается список устройств в системе
                    {
                        switch (buttonName)
                        {
                            case ButtonsPanel.ButtonNames.ButtonOne:
                                {
                                    this.SetView(SystemView.PivotTableView);
                                    break; 
                                }
                            case ButtonsPanel.ButtonNames.ButtonTwo:
                                {
                                    this.SetView(SystemView.DetailsDeviceView);
                                    break; 
                                }
                            case ButtonsPanel.ButtonNames.ButtonThree:
                                {
                                    NetworkDevice device = GetCurrentDevice();
                                    this.ResetDeviceStatusError(
                                    _CanNetworksManager.Networks[device.NetworkId]
                                        .Devices[device.NodeId]);
                                    this._Presenter.DataGridViewDevicesList.Focus();
                                    break;
                                }
                            case ButtonsPanel.ButtonNames.ButtonFour:
                                {
                                    Button btn = this._Presenter.GetSystemButton(
                                        ButtonsPanel.ButtonNames.ButtonFour);
                                    if (this.GetSystemStatus() == Status.Running)
                                    {
                                        this.Stop();
                                        btn.Text = "Запуск системы";
                                    }
                                    else
                                    {
                                        this.Start();
                                        btn.Text = "Остановить систему";
                                    }
                                    break;
                                }
                            case ButtonsPanel.ButtonNames.ButtonFive:
                                {
                                    this._Presenter.ButtonsPanelCollapsed = true;
                                    break;
                                }
                            default:
                                {
                                    throw new InvalidOperationException(String.Format(
                                        "Действие на клик от кнопки {0} неопределено", buttonName));
                                }
                        }
                        break; 
                    }
                case SystemView.DetailsDeviceView:
                    {
                        switch (buttonName)
                        {
                            case ButtonsPanel.ButtonNames.ButtonOne:
                                {
                                    throw new InvalidOperationException(String.Format(
                                        "Действие на клик от кнопки {0} неопределено", buttonName));
                                }
                            case ButtonsPanel.ButtonNames.ButtonTwo:
                                {
                                    this.SetView(SystemView.DevicesListView);
                                    break;
                                }
                            case ButtonsPanel.ButtonNames.ButtonThree:
                                {
                                    throw new InvalidOperationException(String.Format(
                                        "Действие на клик от кнопки {0} неопределено", buttonName));
                                }
                            case ButtonsPanel.ButtonNames.ButtonFour:
                                {
                                    Button btn = this._Presenter.GetSystemButton(
                                        ButtonsPanel.ButtonNames.ButtonFour);
                                    if (this.GetSystemStatus() == Status.Running)
                                    {
                                        this.Stop();
                                        btn.Text = "Запуск системы";
                                    }
                                    else
                                    {
                                        this.Start();
                                        btn.Text = "Остановить систему";
                                    }
                                    break;
                                }
                            case ButtonsPanel.ButtonNames.ButtonFive:
                                {
                                    this._Presenter.ButtonsPanelCollapsed = true;
                                    break;
                                }
                            default:
                                {
                                    throw new InvalidOperationException(String.Format(
                                        "Действие на клик от кнопки {0} неопределено", buttonName));
                                }
                        } 
                        break;
                    }
                case SystemView.PivotTableView:
                    {
                        switch (buttonName)
                        {
                            case ButtonsPanel.ButtonNames.ButtonOne:
                                {
                                    throw new InvalidOperationException(String.Format(
                                        "Действие на клик от кнопки {0} неопределено", buttonName));
                                }
                            case ButtonsPanel.ButtonNames.ButtonTwo:
                                {
                                    this.SetView(SystemView.DevicesListView);
                                    break;
                                }
                            case ButtonsPanel.ButtonNames.ButtonThree:
                                {
                                    throw new InvalidOperationException(String.Format(
                                        "Действие на клик от кнопки {0} неопределено", buttonName));
                                }
                            case ButtonsPanel.ButtonNames.ButtonFour:
                                {
                                    Button btn = this._Presenter.GetSystemButton(
                                        ButtonsPanel.ButtonNames.ButtonFour);
                                    if (this.GetSystemStatus() == Status.Running)
                                    {
                                        this.Stop();
                                        btn.Text = "Запуск системы";
                                    }
                                    else
                                    {
                                        this.Start();
                                        btn.Text = "Остановить систему";
                                    }
                                    break;
                                }
                            case ButtonsPanel.ButtonNames.ButtonFive:
                                {
                                    this._Presenter.ButtonsPanelCollapsed = true;
                                    break;
                                }
                            default:
                                {
                                    throw new InvalidOperationException(String.Format(
                                        "Действие на клик от кнопки {0} неопределено", buttonName));
                                }
                        }
                        break; 
                    }
                default:
                    {
                        throw new InvalidOperationException(String.Format(
                            "Cостояние системы {0} не поддерживается в текущей версии ПО", this.View));
                    }
            }
        }
        /// <summary>
        /// Устанавливает режим отображения информации на экране
        /// </summary>
        /// <param name="view"></param>
        private void SetView(SystemView view)
        {
            Button btn;
            DataGridView dgv;

            switch (view)
            {
                case SystemView.DevicesListView:
                    {
                        btn = this._Presenter.GetSystemButton(ButtonsPanel.ButtonNames.ButtonOne);
                        btn.Text = DevicesListViewCaptions.ButtonOneText;
                        btn.Visible = true;
                        btn = this._Presenter.GetSystemButton(ButtonsPanel.ButtonNames.ButtonTwo);
                        btn.Text = DevicesListViewCaptions.ButtonTwoText;
                        btn.Visible = true;
                        btn = this._Presenter.GetSystemButton(ButtonsPanel.ButtonNames.ButtonThree);
                        btn.Text = DevicesListViewCaptions.ButtonThreeText;
                        btn.Visible = true;
                        btn = this._Presenter.GetSystemButton(ButtonsPanel.ButtonNames.ButtonFour);
                        //btn.Text = DevicesListViewCaptions.ButtonFourText;
                        btn.Visible = true;
                        btn = this._Presenter.GetSystemButton(ButtonsPanel.ButtonNames.ButtonFive);
                        btn.Text = DevicesListViewCaptions.ButtonFiveText;
                        btn.Visible = true;

                        InitTableOfDevices(_Presenter.DataGridViewDevicesList);

                        dgv = _Presenter.DataGridViewDevicesList;
                        dgv.DataSource = null;
                        dgv.DataSource = _CanNetworkAdapter.Devices;
                        _Presenter.TabControlViews.SelectedIndex = 
                            _Presenter.TabControlViews.TabPages.IndexOfKey(
                            CorrosionMonitoringSystemForm.TabPageNames.TabPageDevicesList);
                        dgv.Focus();
                        _View = SystemView.DevicesListView;

                        // Настраиваем события
                        CurrencyManager cManager =
                            _Presenter.DataGridViewDevicesList
                            .BindingContext[dgv.DataSource, dgv.DataMember] as CurrencyManager;

                        if ((cManager != null) && (cManager.Count != 0))
                        {
                            if (_EventHandler_DevicesListCurrencyManager_PositionChanged != null)
                            {
                                cManager.PositionChanged -= 
                                    _EventHandler_DevicesListCurrencyManager_PositionChanged;
                            }
                            
                            _EventHandler_DevicesListCurrencyManager_PositionChanged =
                                new EventHandler(EventHandler_DevicesListCurrencyManager_PositionChanged);
                            
                            cManager.PositionChanged += 
                                _EventHandler_DevicesListCurrencyManager_PositionChanged;
                        }
                        
                        // Проверяем, если текущее устройство находится в состоянии аварии изменяем состояние кнопки
                        SetStatusButtonResetStatusDevice();
                        SetColorRowByDeviceStatus();
                        break;
                    }
                case SystemView.DetailsDeviceView:
                    {
                        btn = this._Presenter.GetSystemButton(ButtonsPanel.ButtonNames.ButtonOne);
                        btn.Text = DetailsDeviceViewCaptions.ButtonOneText;
                        btn.Visible = false;
                        btn = this._Presenter.GetSystemButton(ButtonsPanel.ButtonNames.ButtonTwo);
                        btn.Text = DetailsDeviceViewCaptions.ButtonTwoText;
                        btn.Visible = true;
                        btn = this._Presenter.GetSystemButton(ButtonsPanel.ButtonNames.ButtonThree);
                        btn.Text = DetailsDeviceViewCaptions.ButtonThreeText;
                        btn.Visible = false;
                        btn = this._Presenter.GetSystemButton(ButtonsPanel.ButtonNames.ButtonFour);
                        //btn.Text = DetailsDeviceViewCaptions.ButtonFourText;
                        btn.Visible = true;
                        btn = this._Presenter.GetSystemButton(ButtonsPanel.ButtonNames.ButtonFive);
                        btn.Text = DetailsDeviceViewCaptions.ButtonFiveText;
                        btn.Visible = true;
                        // Получаем устройство 
                        dgv = _Presenter.DataGridViewDevicesList;
                        DataGridViewRow row = dgv.SelectedRows[0];
                        Byte id = System.Convert.ToByte(row.Cells["NodeId"].Value);

                        InitTableOfObjectDictionary(dgv);
                        //_Presenter.DataGridViewDevicesList.AutoGenerateColumns = true;

                        dgv.DataSource = null;
                        foreach(NetworkDevice item in _CanNetworkAdapter.Devices)
                        {
                            if (item.NodeId == id)
                            {
                                dgv.DataSource = null;
                                dgv.DataSource = item;
                                dgv.DataMember = "ObjectDictionary";
                            }
                        }

                        // Скрываем ненужные строки индексов объектного словаря
                        //switch(device.DeviceType)
                        //{
                        //    case DeviceType.KIP_MAIN_POWERED_v1:
                        //        {
                        //            HideRows(dgv, Settings.HiddenIndexesKip9810);
                        //            break;
                        //        }
                        //    case DeviceType.KIP_BATTERY_POWER_v1:
                        //        {
                        //            HideRows(dgv, Settings.HiddenIndexesKip9811);
                        //            break;
                        //        }
                        //}

                        dgv.Focus();

                        this._View = SystemView.DetailsDeviceView;
                        break;
                    }
                case SystemView.PivotTableView:
                    {
                        btn = this._Presenter.GetSystemButton(ButtonsPanel.ButtonNames.ButtonOne);
                        btn.Text = PivotTableViewCaptions.ButtonOneText;
                        btn.Visible = false;
                        btn = this._Presenter.GetSystemButton(ButtonsPanel.ButtonNames.ButtonTwo);
                        btn.Text = PivotTableViewCaptions.ButtonTwoText;
                        btn.Visible = true;
                        btn = this._Presenter.GetSystemButton(ButtonsPanel.ButtonNames.ButtonThree);
                        btn.Text = PivotTableViewCaptions.ButtonThreeText;
                        btn.Visible = false;
                        btn = this._Presenter.GetSystemButton(ButtonsPanel.ButtonNames.ButtonFour);
                        //btn.Text = PivotTableViewCaptions.ButtonFourText;
                        btn.Visible = true;
                        btn = this._Presenter.GetSystemButton(ButtonsPanel.ButtonNames.ButtonFive);
                        btn.Text = PivotTableViewCaptions.ButtonFiveText;
                        btn.Visible = true;

                        _Presenter.TabControlViews.SelectedIndex = _Presenter.TabControlViews.TabPages.IndexOfKey(
                            CorrosionMonitoringSystemForm.TabPageNames.TabPagePivotTable);

                        this._Presenter.DataGridViewPivotTable.Focus();

                        this._View = SystemView.PivotTableView;
                        break;
                    }
            }
        }
        /// <summary>
        /// Скрывает строки с указанными номерами индексов объектного
        /// словаря устройства
        /// </summary>
        /// <param name="control"></param>
        /// <param name="indexes"></param>
        private void HideRows(DataGridView control, UInt16[] indexes)
        {
            DataGridViewRow row;
            List<UInt16> list = new List<ushort>(indexes);

            for (int i = 0; i < control.Rows.Count; i++)
            {
                row = control.Rows[i];
                if (list.Contains(System.Convert.ToUInt16(row.Cells["Index"].Value)))
                {
                    // Нельзя скрыть активную строку, если активна устанавливаем на следующую
                    if (control.CurrentRow.Equals(row))
                    {
                        CurrencyManager cManager =
                            control.BindingContext[control.DataSource, control.DataMember]
                            as CurrencyManager;

                        if ((cManager != null)&&(cManager.Count != 0))
                        {
                            if (cManager.Count > 1)
                            {
                                // Устанавливаем курсор на следующую или предыдущую строку
                                if (cManager.Position >= cManager.Count)
                                {
                                    cManager.Position--;
                                }
                                else
                                {
                                    cManager.Position++;
                                }
                                row.Visible = false;
                            }
                        }
                        
                    }
                    else
                    {
                        row.Visible = false;
                    }
                }
            }
        }
        /// <summary>
        /// Обработчик нажатий системных кнопок на панели кнопок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void EventHandler_Presenter_SystemButtonClick(object sender, 
            SystemButtonClickEventArgs args)
        {
            this.ExecuteCommand(args.Button);
            return;
        }
        /// <summary>
        /// Обработчик события загрузки формы представления данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_Presenter_Loaded(object sender, EventArgs e)
        {
            InitPivotTable(this._Presenter.DataGridViewPivotTable);
            ConnectToDevices(this._CanNetworksManager);
            _Presenter.FormClosing += 
                new FormClosingEventHandler(EventHandler_Presenter_FormClosing);
            _Presenter.TabControlViews.SelectedIndexChanged += 
                new EventHandler(EventHandler_Presenter_TabControlViews_SelectedIndexChanged);

            // Настраиваем контролы гридов
            //DataGridView dgv = _Presenter.DataGridViewDevicesList;
            //dgv.StandardTab = true;
            //dgv.MultiSelect = false;
            //dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //dgv.AllowUserToDeleteRows = false;
            //dgv.AllowUserToAddRows = false;

            // Устанавливаем основную страницу;
            SetView(SystemView.PivotTableView);
            // Обновляем строку состояния

            _Presenter.TotalDevices = _CanNetworkAdapter.Devices.Count; ;

            //this._Presenter.DataGridViewDevicesList.ColumnWidthChanged += 
            //    new DataGridViewColumnEventHandler(EventHandler_DataGridViewDevicesList_ColumnWidthChanged);

            // Запускаем систему
            Button btn = _Presenter.GetSystemButton(ButtonsPanel.ButtonNames.ButtonFour);
#if !(DEBUG)
            this.Start();
            btn.Text = "Остановить систему";
#else
            btn.Text = "Запуск системы";
#endif
            return;
        }
        /// <summary>
        /// Сохраняет ширины столбцов гридов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveColumnWidth(DataGridView control)
        {
            // Сохраняем значения
            switch (control.Name)
            {
                case "DataGridViewDevicesList":
                    {
                        Settings.StatusColumnWidth = control.Columns["Status"].Width;
                        Settings.NetworkNameColumnWidth = control.Columns["NetworkName"].Width;
                        Settings.LocationNameColumnWidth = control.Columns["Location"].Width;
                        break;
                    }
                case "DataGridViewObjectDictionary":
                    {
                        Settings.ObjectDictionaryDisplayedNameColumnWidth = 
                            control.Columns[FieldNamesOfObjectDictionary.DISPLAYED_NAME].Width; //"DisplayedName"
                        Settings.ObjectDictionaryIndexValueColumnWidth = 
                            control.Columns[FieldNamesOfObjectDictionary.VALUE].Width; //"IndexValue"
                        Settings.ObjectDictionaryLastUpdateTimeColumnWidth = 
                            control.Columns[FieldNamesOfObjectDictionary.MODIFIED].Width;
                        break;
                    }
                case "DataGridViewParametersPivotTable":
                    {
                        Settings.PivotTableLocationColumnWidth = 
                            control.Columns["Location"].Width;
                        Settings.PivotTablePolarisationPotential_2008ColumnWidth =
                            control.Columns["PolarisationPotential_2008"].Width;
                        Settings.PivotTableProtectionPotential_2009ColumnWidth =
                            control.Columns["ProtectionPotential_2009"].Width;
                        Settings.PivotTableProtectionCurrent_200BColumnWidth = 
                            control.Columns["ProtectionCurrent_200B"].Width;
                        Settings.PivotTablePolarisationCurrent_200СColumnWidth = 
                            control.Columns["PolarisationCurrent_200С"].Width;
                        Settings.PivotTableCorrosion_depth_200FColumnWidth = 
                            control.Columns["Corrosion_depth_200F"].Width;
                        Settings.PivotTableCorrosion_speed_2010ColumnWidth = 
                            control.Columns["Corrosion_speed_2010"].Width;
                        break;
                    }
            }
            return;
        }
        /// <summary>
        /// Обработчик семны закладок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_Presenter_TabControlViews_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl control = (TabControl)sender;

            switch (control.SelectedTab.Name)
            {
                case "TabPageDevicesList":
                    {
                        this.SetView(SystemView.DevicesListView);
                        break; 
                    }
                case "TabPagePivotTable":
                    {
                        this.SetView(SystemView.PivotTableView);
                        break; 
                    }
                default: throw new Exception("Для данной закладки обработка не определена");
            }
            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_Presenter_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (NetworkController cntr in this._CanNetworksManager.Networks)
            {
                if (cntr.Status != Common.Controlling.Status.Stopped)
                {
                    cntr.Stop();
                }
            }
            // Сохраняем шируну столбцов для текущих гидов
            SaveColumnWidth(this._Presenter.DataGridViewDevicesList);
            SaveColumnWidth(this._Presenter.DataGridViewPivotTable);

            return;
        }
        /// <summary>
        /// Перебираем все устройства в доступных сетях и подключаем событие изменения 
        /// статуса устройства
        /// </summary>
        /// <param name="netwokManager"></param>
        private void ConnectToDevices(NetworksManager netwokManager)
        {
            foreach(INetworkController controller in netwokManager.Networks)
            {
                foreach (IDevice device in controller.Devices)
                {
                    device.DeviceChangedStatus += 
                        new EventHandler(EventHandler_NetworkDeviceChangedStatus);
                }
            }
            return;
        }
        /// <summary>
        /// Обработчик изменения состояния устройства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_NetworkDeviceChangedStatus(object sender, EventArgs e)
        {
            IDevice device = (IDevice)sender;

            if (this.View == SystemView.DevicesListView)
            {
                _Presenter.DataGridViewDevicesList.Update();
                _Presenter.DataGridViewDevicesList.Refresh();
                SetColorRowByDeviceStatus();
            }
           
            // Обновляем строку состояния
            int x = 0;
            _Presenter.FaultyDevices = 
                _CanNetworksManager.Networks[x].Devices.FaultyDevices; //NETWORK_CAN_0
            // Устанавливаем состояние кнопки ресет (если это необходимо)
            this.SetStatusButtonResetStatusDevice();
            return;
        }
        /// <summary>
        /// Устанавливает цвет строки в зависимости от текущего состояния устройства
        /// </summary>
        private void SetColorRowByDeviceStatus()
        {
            DataGridViewRow row;
            if (View != SystemView.DevicesListView)
            {
                return;
            }
            // Раскашиваем строки в соотвествии со статусом устройства
            for (int i = 0; i < _Presenter.DataGridViewDevicesList.Rows.Count; i++)
            {
                row = _Presenter.DataGridViewDevicesList.Rows[i];

                switch ((DeviceStatus)row.Cells["Status"].Value)
                {
                    case DeviceStatus.CommunicationError:
                        {
                            row.DefaultCellStyle.BackColor = Settings.CommunicationErrorRowColor;
                            break;
                        }
                    case DeviceStatus.ConfigurationError:
                        {
                            row.DefaultCellStyle.BackColor = Settings.ConfigurationErrorRowColor;
                            break;
                        }
                    case DeviceStatus.Operational:
                        {
                            row.DefaultCellStyle.BackColor = Settings.OperationalModeRowColor;
                            break;
                        }
                    case DeviceStatus.Preoperational:
                        {
                            row.DefaultCellStyle.BackColor = Settings.PreoperationalModeRowColor;
                            break;
                        }
                    case DeviceStatus.Stopped:
                        {
                            row.DefaultCellStyle.BackColor = Settings.StoppedModeRowColor;
                            break;
                        }
                    default:
                        {
                            throw new Exception();
                        }
                } // End of switch()
            } // End of For()
            return;
        }
        /// <summary>
        /// Инициализирует таблиуцу для отображения объектного словаря устройства. 
        /// Выполняется при запуске программы 
        /// </summary>
        /// <param name="control">Grid для отображения таблицы объектов словаря объектов устройства</param>
        public void InitTableOfDevices(DataGridView control)
        {
            ToolStripMenuItem menuItem;
            DataGridViewTextBoxColumn textboxClmn;
            control.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // Настраиваем контрол
            control.Name = "DataGridViewDevicesList";
            control.StandardTab = true;
            control.MultiSelect = false;
            control.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Создаём столбцы
            control.AutoGenerateColumns = false;
            control.Columns.Clear();

            control.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            control.MultiSelect = false;
            control.AllowUserToDeleteRows = false;
            control.AllowUserToAddRows = false;
            control.AutoSizeColumnsMode = 
                DataGridViewAutoSizeColumnsMode.None; // DataGridViewAutoSizeColumnsMode.Fill;
            control.ColumnHeadersHeightSizeMode = 
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            // Создаём столбец "Сетевой адрес"
            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = "NodeId";
            textboxClmn.HeaderText = "Сетевой адрес";
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            textboxClmn.Visible = true;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = "NodeId";
            //textboxClmn.DefaultCellStyle = this._RowStyleDeviceIsStopped;
            control.Columns.Add(textboxClmn);

            // Создаём столбец "Место установки"
            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = "Location";
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            textboxClmn.HeaderText = "Расположение";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            textboxClmn.Visible = true;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            textboxClmn.DataPropertyName = "Location";
            //textboxClmn.DefaultCellStyle = this._RowStyleDeviceIsStopped;
            control.Columns.Add(textboxClmn);

            // Создаём столбец "Статус устройства"
            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = "Status";
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            textboxClmn.HeaderText = "Состояние устройства";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.Automatic;
            textboxClmn.Visible = true;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            textboxClmn.DataPropertyName = "Status";
            control.Columns.Add(textboxClmn);
            
            // Создаём столбец "Сеть CAN"
            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = "NetworkName";
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            textboxClmn.HeaderText = "Сеть CAN";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.Programmatic;
            textboxClmn.Visible = true;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = "NetworkDescription";
            //textboxClmn.DefaultCellStyle = this._RowStyleDeviceIsStopped;
            control.Columns.Add(textboxClmn);

            // Создаём столбец "Период опроса"
            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = "PollingInterval";
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            textboxClmn.HeaderText = "Период опроса, сек";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            textboxClmn.Visible = true;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = "PollingInterval";
            //textboxClmn.DefaultCellStyle = this._RowStyleDeviceIsStopped;
            control.Columns.Add(textboxClmn);

            // Подклчюем источник данных - список сетевых устройств
            control.DataSource = null;
            int x = 0;
            control.DataSource = 
                _CanNetworksManager.Networks[x].Devices; //NETWORK_CAN_0

            // Сортируем устройства по статусу
            //control.Sort(control.Columns["Status"],
            //    ListSortDirection.Descending);

            // Настраиваем стили заголовков столбцов
            DataGridViewCellStyle style;
            style = new DataGridViewCellStyle();
            style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Font font = new System.Drawing.Font(
                    control.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
            style.Font = font;
            style.WrapMode = DataGridViewTriState.True;
            control.ColumnHeadersDefaultCellStyle.ApplyStyle(style);

            //// Настраиваем формат вывода данных в столбцах
            style = new DataGridViewCellStyle();
            style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            style.Font = control.Columns["NodeId"].CellTemplate.Style.Font;
            style.Format = "X";
            control.Columns["NodeId"].CellTemplate.Style = style;

            style = new DataGridViewCellStyle();
            style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            style.Font = control.Columns["Status"].CellTemplate.Style.Font;
            control.Columns["Status"].CellTemplate.Style = style;

            style = new DataGridViewCellStyle();
            style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            style.Font = control.Columns["PollingInterval"].CellTemplate.Style.Font;
            style.Format = "D";
            control.Columns["PollingInterval"].CellTemplate.Style = style;

            style = new DataGridViewCellStyle();
            style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style.Font = control.Columns["Location"].CellTemplate.Style.Font;
            style.WrapMode = DataGridViewTriState.True;
            control.Columns["Location"].CellTemplate.Style = style;

            // Настраиваем события
            CurrencyManager cManager = 
                control.BindingContext[control.DataSource, control.DataMember] as CurrencyManager;

            if ((cManager != null) && (cManager.Count != 0))
            {
                if (_EventHandler_DevicesListCurrencyManager_PositionChanged != null)
                {
                    cManager.PositionChanged -= this._EventHandler_DevicesListCurrencyManager_PositionChanged; 
                }
                this._EventHandler_DevicesListCurrencyManager_PositionChanged =
                    new EventHandler(EventHandler_DevicesListCurrencyManager_PositionChanged);
                cManager.PositionChanged += this._EventHandler_DevicesListCurrencyManager_PositionChanged;
            }

            // Скрываем/отображаем стобцы в зависимости от установок приложения
            if (Settings.IsDebug)
            {
                foreach (DataGridViewColumn clmn in control.Columns)
                {
                    clmn.Visible = true;
                }
            }
            else
            {
                control.Columns["NodeId"].Visible = false;
                control.Columns["Status"].Visible = true;
                control.Columns["NetworkName"].Visible = true;
                control.Columns["Location"].Visible = true;
                control.Columns["PollingInterval"].Visible = false;

                // Устанавливаем ширину столбцов из настроек
                control.Columns["Status"].Width = Settings.StatusColumnWidth;
                control.Columns["NetworkName"].Width = Settings.NetworkNameColumnWidth;
                control.Columns["Location"].Width = Settings.LocationNameColumnWidth;
            }

            AdjustColumnsWidth(control);

            // Инициализируем контекстные меню
            //this._DeviceListContextMenuStripForNetwork = new ContextMenuStrip();
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Name = "ContextMenuStripDevicesList";
            control.ContextMenuStrip = null;
            control.ContextMenuStrip = menu;

            //menuItem = new ToolStripMenuItem();
            //menuItem.Name = "toolStripMenuDeviceStatusSetStopped";
            //menuItem.Text = "Остановить устройство";
            //menuItem.Dock = DockStyle.Fill;
            //menuItem.Click += new EventHandler(EventHandler_ContextMenuStripForNetworkDeviceList_Click);
            ////this._DeviceListContextMenuStripForNetwork.Items.Add(menuItem);
            //menu.Items.Add(menuItem);

            //menuItem = new ToolStripMenuItem();
            //menuItem.Name = "toolStripMenuDeviceStatusSetPreOperational";
            //menuItem.Text = "Приостановить устройство";
            //menuItem.Dock = DockStyle.Fill;
            //menuItem.Click += new EventHandler(EventHandler_ContextMenuStripForNetworkDeviceList_Click);
            ////this._DeviceListContextMenuStripForNetwork.Items.Add(menuItem);
            //menu.Items.Add(menuItem);

            //menuItem = new ToolStripMenuItem();
            //menuItem.Name = "toolStripMenuDeviceStatusSetOperational";
            //menuItem.Text = "Запустить устройство";
            //menuItem.Dock = DockStyle.Fill;
            //menuItem.Click += new EventHandler(EventHandler_ContextMenuStripForNetworkDeviceList_Click);
            ////this._DeviceListContextMenuStripForNetwork.Items.Add(menuItem);
            //menu.Items.Add(menuItem);

            //menuItem = new ToolStripMenuItem();
            //menuItem.Name = "toolStripMenuDeviceStatusSetCommunicationError";
            //menuItem.Text = "Установить ошибку соединения";
            //menuItem.Dock = DockStyle.Fill;
            //menuItem.Click += new EventHandler(EventHandler_ContextMenuStripForNetworkDeviceList_Click);
            ////this._DeviceListContextMenuStripForNetwork.Items.Add(menuItem);
            //menu.Items.Add(menuItem);

            //menuItem = new ToolStripMenuItem();
            //menuItem.Name = "toolStripMenuDeviceStatusSetConfigurationError";
            //menuItem.Text = "Установить ошибку конфигурации";
            //menuItem.Dock = DockStyle.Fill;
            //menuItem.Click += new EventHandler(EventHandler_ContextMenuStripForNetworkDeviceList_Click);
            ////this._DeviceListContextMenuStripForNetwork.Items.Add(menuItem);
            //menu.Items.Add(menuItem);

            //separator = new ToolStripSeparator();
            //separator.Name = "toolStripSeparator1";
            //menu.Items.Add(separator);

            //menuItem = new ToolStripMenuItem();
            //menuItem.Name = "toolStripMenuColumnSettings";
            //menuItem.Text = "Настройки столбцов";
            //menuItem.Dock = DockStyle.Fill;
            //menuItem.Click += new EventHandler(EventHandler_ContextMenuStripForNetworkDeviceList_Click);
            //this._DeviceListContextMenuStripForNetwork.Items.Add(menuItem);

            //menuItem = new ToolStripMenuItem();
            //menuItem.Name = "toolStripMenuSimulator";
            //menuItem.Text = "Симулятор сигналов";
            //menuItem.Dock = DockStyle.Fill;
            //menuItem.Click += new EventHandler(EventHandler_ContextMenuStripForNetworkDeviceList_Click);
            //this._DeviceListContextMenuStripForNetwork.Items.Add(menuItem);

            menuItem = new ToolStripMenuItem();
            menuItem.Name = "toolStripMenuNetworkManager";
            menuItem.Text = "Контроллер сети";
            menuItem.Dock = DockStyle.Fill;
            menuItem.Click += new EventHandler(
                EventHandler_ContextMenuStripForNetworkDeviceList_Click);
            menu.Items.Add(menuItem);

            //menuItem = new ToolStripMenuItem();
            //menuItem.Name = "toolStripMenuSettings";
            //menuItem.Text = "Настройки";
            //menuItem.Dock = DockStyle.Fill;
            //menuItem.Click += new EventHandler(EventHandler_ContextMenuStripForNetworkDeviceList_Click);
            //this._DeviceListContextMenuStripForNetwork.Items.Add(menuItem);

            //separator = new ToolStripSeparator();
            //separator.Name = "toolStripSeparator2";
            //this._DeviceListContextMenuStripForNetwork.Items.Add(separator);

            menuItem = new ToolStripMenuItem();
            menuItem.Name = "toolStripMenuStart";
            menuItem.Text = "Пуск";
            menuItem.Dock = DockStyle.Fill;
            menuItem.Click += new EventHandler(EventHandler_ContextMenuStripForNetworkDeviceList_Click);
            menu.Items.Add(menuItem);

            menuItem = new ToolStripMenuItem();
            menuItem.Name = "toolStripMenuStop";
            menuItem.Text = "Стоп";
            menuItem.Dock = DockStyle.Fill;
            menuItem.Enabled = false;
            menuItem.Click += new EventHandler(EventHandler_ContextMenuStripForNetworkDeviceList_Click);
            menu.Items.Add(menuItem);

            //separator = new ToolStripSeparator();
            //separator.Name = "toolStripSeparator3";
            //this._DeviceListContextMenuStripForNetwork.Items.Add(separator);

            //menuItem = new ToolStripMenuItem();
            //menuItem.Name = "toolStripMenuExit";
            //menuItem.Text = "Выход";
            //menuItem.Dock = DockStyle.Fill;
            //menuItem.Click += new EventHandler(EventHandler_ContextMenuStripForNetworkDeviceList_Click);
            //this._DeviceListContextMenuStripForNetwork.Items.Add(menuItem);

            //control.ContextMenuStrip = this._DeviceListContextMenuStripForNetwork;

            this.SetColorRowByDeviceStatus();

            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        private void AdjustColumnsWidth(DataGridView control)
        {
            if (Settings.IsDebug)
            {
                foreach (DataGridViewColumn column in control.Columns)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            else
            {
                List<int> clmns_visible = new List<int>();

                for (int i = 0; i < control.Columns.Count; i++)
                {
                    control.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    if (control.Columns[i].Visible)
                    {
                        clmns_visible.Add(i);
                    }
                }
                control.Columns[clmns_visible[clmns_visible.Count - 1]].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                return;
            }
        }
        /// <summary>
        /// Возвращает текущее устройство в списке устройств сети. 
        /// </summary>
        /// <returns></returns>
        private NetworkDevice GetCurrentDevice()
        {
            DataGridView control = this._Presenter.DataGridViewDevicesList;
            CurrencyManager manager = 
                control.BindingContext[control.DataSource, control.DataMember] as CurrencyManager;
            if ((manager != null) && (manager.Count > 0))
            {
                return (NetworkDevice)manager.Current;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void SetStatusButtonResetStatusDevice()
        {
            if (this.View == SystemView.DevicesListView)
            {
                Button btn = this._Presenter.GetSystemButton(ButtonsPanel.ButtonNames.ButtonThree);
                NetworkDevice device = GetCurrentDevice();

                if (device != null)
                {
                    if ((device.Status == DeviceStatus.CommunicationError) ||
                        (device.Status == DeviceStatus.ConfigurationError))
                    {
                        btn.Enabled = true;
                    }
                    else
                    {
                        btn.Enabled = false;
                    }
                }
            }
            return;
        }
        /// <summary>
        /// Обработчик события перехода на другую строку таблицы устройств системы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_DevicesListCurrencyManager_PositionChanged(
            object sender, EventArgs e)
        {
            // CurrencyManager cManager = (CurrencyManager)sender;      
            // Проверяем, если текущее устройство находится в состоянии аварии 
            // изменяем состояние кнопки
            SetStatusButtonResetStatusDevice();          
            return;
        }
        /// <summary>
        /// Инициализирует таблиуцу стройств. Выполняется при запуске программы 
        /// </summary>
        /// <param name="control">Grid для отображения таблицы устройств</param>
        private void InitTableOfObjectDictionary(DataGridView control)
        {
            DataGridViewColumn clmn;
            DataGridViewCellStyle style;
            Font font;
            DataGridViewTextBoxColumn textboxClmn;
            DataGridViewCheckBoxColumn checkboxClmn;

            // Создаём столбцы
            control.Name = "DataGridViewObjectDictionary";
            control.AutoGenerateColumns = false;
            control.Columns.Clear();

            control.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            control.MultiSelect = false;
            control.AllowUserToDeleteRows = false;
            control.AllowUserToAddRows = false;
            control.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            control.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            // Создаём столбец "Индекс"
            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = FieldNamesOfObjectDictionary.INDEX; //"Index";
            textboxClmn.HeaderText = "Индекс";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.Automatic;
            textboxClmn.Visible = true;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = FieldNamesOfObjectDictionary.INDEX;
            //textboxClmn.DefaultCellStyle = this._DeviceIsStoppedRowStyle;
            control.Columns.Add(textboxClmn);

            // Создаём столбец "Индекс"
            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = FieldNamesOfObjectDictionary.NAME; //"IndexName";
            textboxClmn.HeaderText = "Наименование объекта";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            textboxClmn.Visible = true;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = FieldNamesOfObjectDictionary.NAME;
            //textboxClmn.DefaultCellStyle = this._DeviceIsStoppedRowStyle;
            control.Columns.Add(textboxClmn);

            // Создаём столбец "Индекс"
            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = FieldNamesOfObjectDictionary.DISPLAYED_NAME; //"DisplayedName";
            textboxClmn.HeaderText = "Параметр";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            textboxClmn.Visible = true;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = FieldNamesOfObjectDictionary.DISPLAYED_NAME;
            //textboxClmn.DefaultCellStyle = this._DeviceIsStoppedRowStyle;
            control.Columns.Add(textboxClmn);

            // Создаём столбец "Индекс"
            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = FieldNamesOfObjectDictionary.VALUE; //"IndexValue";
            textboxClmn.HeaderText = "Значение";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            textboxClmn.Visible = true;
            textboxClmn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = FieldNamesOfObjectDictionary.VALUE;
            //textboxClmn.DefaultCellStyle = this._DeviceIsStoppedRowStyle;
            control.Columns.Add(textboxClmn);

            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = FieldNamesOfObjectDictionary.STATUS; 
            textboxClmn.HeaderText = "Статус";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            textboxClmn.Visible = true;
            textboxClmn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = FieldNamesOfObjectDictionary.STATUS;
            //textboxClmn.DefaultCellStyle = this._DeviceIsStoppedRowStyle;
            control.Columns.Add(textboxClmn);

            // Создаём столбец "Индекс"
            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = FieldNamesOfObjectDictionary.MODIFIED; //"LastUpdateTime";
            textboxClmn.HeaderText = "Дата обновления";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            textboxClmn.Visible = true;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = FieldNamesOfObjectDictionary.MODIFIED;
            //textboxClmn.DefaultCellStyle = this._DeviceIsStoppedRowStyle;
            control.Columns.Add(textboxClmn);

            // Создаём столбец "Индекс"
            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = FieldNamesOfObjectDictionary.DESCRIPTION;// "Description";
            textboxClmn.HeaderText = "Описание параметра";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            textboxClmn.Visible = true;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = FieldNamesOfObjectDictionary.DESCRIPTION;
            //textboxClmn.DefaultCellStyle = this._DeviceIsStoppedRowStyle;
            control.Columns.Add(textboxClmn);

            // Создаём столбец "Индекс"
            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = FieldNamesOfObjectDictionary.CATEGORY; // "Category";
            textboxClmn.HeaderText = "Категория параметра";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.Automatic;
            textboxClmn.Visible = true;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = FieldNamesOfObjectDictionary.CATEGORY;
            //textboxClmn.DefaultCellStyle = this._DeviceIsStoppedRowStyle;
            control.Columns.Add(textboxClmn);

            // Создаём столбец "Индекс"
            checkboxClmn = new DataGridViewCheckBoxColumn(false);
            checkboxClmn.Name = "ReadOnly";
            checkboxClmn.HeaderText = "Доступ к знaчению";
            checkboxClmn.ReadOnly = false;
            checkboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            checkboxClmn.Visible = true;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            checkboxClmn.DataPropertyName = FieldNamesOfObjectDictionary.READ_ONLY;
            //textboxClmn.DefaultCellStyle = this._DeviceIsStoppedRowStyle;
            control.Columns.Add(checkboxClmn);

            // Создаём столбец "Индекс"
            checkboxClmn = new DataGridViewCheckBoxColumn(false);
            checkboxClmn.Name = FieldNamesOfObjectDictionary.ENABLE_CYCLIC_READ;// "SdoReadEnable";
            checkboxClmn.HeaderText = "Чтение параметра по SDO";
            checkboxClmn.ReadOnly = false;
            checkboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            checkboxClmn.Visible = true;
            //checkboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            checkboxClmn.DataPropertyName = FieldNamesOfObjectDictionary.ENABLE_CYCLIC_READ;
            //checkboxClmn.DefaultCellStyle = this._DeviceIsStoppedRowStyle;
            control.Columns.Add(checkboxClmn);

            // Скрываем/отображаем стобцы в зависимости от установок приложения
            if (Settings.IsDebug)
            {
                foreach (DataGridViewColumn item in control.Columns)
                {
                    item.Visible = true;
                }
            }
            else
            {
                control.Columns[FieldNamesOfObjectDictionary.INDEX].Visible = false;
                control.Columns[FieldNamesOfObjectDictionary.NAME].Visible = false;
                control.Columns[FieldNamesOfObjectDictionary.DISPLAYED_NAME].Visible = true;
                control.Columns[FieldNamesOfObjectDictionary.VALUE].Visible = true;
                control.Columns[FieldNamesOfObjectDictionary.MODIFIED].Visible = true;
                control.Columns[FieldNamesOfObjectDictionary.DESCRIPTION].Visible = false;
                control.Columns[FieldNamesOfObjectDictionary.CATEGORY].Visible = false;
                control.Columns[FieldNamesOfObjectDictionary.READ_ONLY].Visible = false;
                control.Columns[FieldNamesOfObjectDictionary.ENABLE_CYCLIC_READ].Visible = false;

                control.Columns[FieldNamesOfObjectDictionary.DISPLAYED_NAME].Width = //"DisplayedName"
                    Settings.ObjectDictionaryDisplayedNameColumnWidth;
                control.Columns[FieldNamesOfObjectDictionary.VALUE].Width = //"IndexValue"
                    Settings.ObjectDictionaryIndexValueColumnWidth;
                control.Columns[FieldNamesOfObjectDictionary.MODIFIED].Width =
                    Settings.ObjectDictionaryLastUpdateTimeColumnWidth;
            }

            // Подстраиваем ширину столбцов
            this.AdjustColumnsWidth(control);

            // Настраиваем стили заголовков столбцов

            style = new DataGridViewCellStyle();
            style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            font = new System.Drawing.Font(
                control.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
            style.Font = font;
            style.WrapMode = DataGridViewTriState.True;
            control.ColumnHeadersDefaultCellStyle.ApplyStyle(style);

            // Настраиваем формат вывода данных в столбцах
            System.Globalization.CultureInfo cultureRussian = 
                new System.Globalization.CultureInfo("ru-RU", false);

            clmn = control.Columns[FieldNamesOfObjectDictionary.INDEX];
            clmn.ReadOnly = true;
            clmn.CellTemplate.Style.Format = "X";
            clmn.CellTemplate.Style.FormatProvider = cultureRussian;

            clmn = control.Columns[FieldNamesOfObjectDictionary.MODIFIED];
            clmn.ReadOnly = true;
            clmn.CellTemplate.Style.Format =
                cultureRussian.DateTimeFormat.ShortDatePattern + " " + cultureRussian.DateTimeFormat.LongTimePattern;
            clmn.CellTemplate.Style.FormatProvider =
               cultureRussian;

            //control.ColumnHeaderMouseClick +=
            //    new DataGridViewCellMouseEventHandler(dataGridViewObjectDictionary_ColumnHeaderMouseClick);
            //control.CellFormatting +=
            //    new DataGridViewCellFormattingEventHandler(EventHandler_dataGridViewObjectDictionary_CellFormatting);
            control.CellParsing += new DataGridViewCellParsingEventHandler(EventHandler_dataGridViewObjectDictionary_CellParsing);
            //control.GotFocus += new EventHandler(EventHandler_DataGridView_GotFocus);
            //control.LostFocus += new EventHandler(EventHandler_DataGridView_LostFocus);
            return;
        }
        /// <summary>
        ///  Инициализируем сводную таблицу параметров
        /// </summary>
        /// <param name="control"></param>
        private void InitPivotTable(DataGridView control)
        {
            DataGridViewTextBoxColumn textboxClmn;

            // Настраиваем контрол
            control.DataSource = null;
            control.StandardTab = true;
            control.MultiSelect = false;
            control.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Создаём столбцы
            control.AutoGenerateColumns = false;
            control.Columns.Clear();

            control.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            control.MultiSelect = false;
            control.AllowUserToDeleteRows = false;
            control.AllowUserToAddRows = false;
            //control.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            control.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            control.ColumnHeadersHeightSizeMode = 
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            //control.RowEnter +=
            //    new DataGridViewCellEventHandler(EventHandler_dataGridViewDeviceList_RowEnter);
            //control.GotFocus += new EventHandler(EventHandler_DataGridView_GotFocus);
            //control.LostFocus += new EventHandler(EventHandler_DataGridView_LostFocus);

            // Создаём столбец "Сетевой адрес"
            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = "NodeId";
            textboxClmn.HeaderText = "Сетевой адрес";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = "NodeId";
            textboxClmn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            textboxClmn.Visible = false;
            control.Columns.Add(textboxClmn);

            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = "Location";
            textboxClmn.HeaderText = "Расположение";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            textboxClmn.Visible = true;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = "Location";
            textboxClmn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            control.Columns.Add(textboxClmn);

            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = "PolarisationPotential_2008";
            textboxClmn.HeaderText = "Поляризационный потенциал, B";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            textboxClmn.Visible = true;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = "PolarisationPotential_2008";
            textboxClmn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            control.Columns.Add(textboxClmn);

            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = "ProtectionPotential_2009";
            textboxClmn.HeaderText = "Защитный потенциал, B";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            textboxClmn.Visible = true;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = "ProtectionPotential_2009";
            textboxClmn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            control.Columns.Add(textboxClmn);

            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = "ProtectionCurrent_200B";
            textboxClmn.HeaderText = "Ток катодной защиты, A";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            textboxClmn.Visible = true;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = "ProtectionCurrent_200B";
            textboxClmn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            control.Columns.Add(textboxClmn);

            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = "PolarisationCurrent_200С";
            textboxClmn.HeaderText = "Ток поляризации, mA";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            textboxClmn.Visible = false;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = "PolarisationCurrent_200С";
            textboxClmn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            control.Columns.Add(textboxClmn);

            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = "Corrosion_depth_200F";
            textboxClmn.HeaderText = "Глубина коррозии, мкм";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            textboxClmn.Visible = true;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = "Corrosion_depth_200F";
            textboxClmn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            control.Columns.Add(textboxClmn);

            textboxClmn = new DataGridViewTextBoxColumn();
            textboxClmn.Name = "Corrosion_speed_2010";
            textboxClmn.HeaderText = "Скорость коррозии, мкм/год";
            textboxClmn.ReadOnly = true;
            textboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            textboxClmn.Visible = true;
            //textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            textboxClmn.DataPropertyName = "Corrosion_speed_2010";
            textboxClmn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            control.Columns.Add(textboxClmn);

            //textboxClmn = new DataGridViewTextBoxColumn();
            //textboxClmn.Name = "Tamper_2015";
            //textboxClmn.HeaderText = "Вскрытие корпуса";
            //textboxClmn.ReadOnly = true;
            //textboxClmn.SortMode = DataGridViewColumnSortMode.NotSortable;
            //textboxClmn.Visible = false;
            ////textboxClmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            //textboxClmn.DataPropertyName = "Tamper_2015";
            //textboxClmn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //control.Columns.Add(textboxClmn);

            // Создаём объект сводной талицы (будет использоваться в качестве источника данных для 
            // диаграмм и сводной таблицы параметров)
            BindingSource bindingSourceNetworkCan = new BindingSource();
            bindingSourceNetworkCan.DataSource = _PivotTable.PivotTable;
            // Подклчюем источник данных - список сетевых устройств
            control.DataSource = null;
            control.DataSource = bindingSourceNetworkCan;
            //control.DataSource = this._BindingSourceNetworkCan1;

            // Сортируем устройства по статусу
            //control.Sort(control.Columns["NodeId"],
            //    ListSortDirection.Ascending);
            //control.Columns["NodeId"].Visible = false;

            // Настраиваем стили заголовков столбцов
            DataGridViewCellStyle style;
            style = new DataGridViewCellStyle();
            style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Font font = new System.Drawing.Font(
                    control.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
            style.Font = font;
            style.WrapMode = DataGridViewTriState.True;
            control.ColumnHeadersDefaultCellStyle.ApplyStyle(style);

            //control.ColumnHeaderMouseClick += 
            //    new DataGridViewCellMouseEventHandler(EventHandler_dataGridViewPivotTable_ColumnHeaderMouseClick);

            // Настраиваем видимость и ширину столбцов грида
            if (Settings.IsDebug)
            {
                foreach (DataGridViewColumn item in control.Columns)
                {
                    item.Visible = true;
                }
            }
            else
            {
                control.Columns["Location"].Width = 
                    Settings.PivotTableLocationColumnWidth;
                control.Columns["PolarisationPotential_2008"].Width = 
                    Settings.PivotTablePolarisationPotential_2008ColumnWidth;
                control.Columns["ProtectionPotential_2009"].Width = 
                    Settings.PivotTableProtectionPotential_2009ColumnWidth;
                control.Columns["ProtectionCurrent_200B"].Width = 
                    Settings.PivotTableProtectionCurrent_200BColumnWidth;
                control.Columns["PolarisationCurrent_200С"].Width = 
                    Settings.PivotTablePolarisationCurrent_200СColumnWidth;
                control.Columns["Corrosion_depth_200F"].Width = 
                    Settings.PivotTableCorrosion_depth_200FColumnWidth;
                control.Columns["Corrosion_speed_2010"].Width = 
                    Settings.PivotTableCorrosion_speed_2010ColumnWidth;

                control.Columns["Location"].Width = Settings.PivotTableLocationColumnWidth;
                control.Columns["PolarisationPotential_2008"].Width = Settings.PivotTablePolarisationPotential_2008ColumnWidth;
                control.Columns["ProtectionPotential_2009"].Width = Settings.PivotTableProtectionPotential_2009ColumnWidth;
                control.Columns["ProtectionCurrent_200B"].Width = Settings.PivotTableProtectionCurrent_200BColumnWidth;
                control.Columns["PolarisationCurrent_200С"].Width = Settings.PivotTablePolarisationCurrent_200СColumnWidth;
                control.Columns["Corrosion_depth_200F"].Width = Settings.PivotTableCorrosion_depth_200FColumnWidth;
                control.Columns["Corrosion_speed_2010"].Width = Settings.PivotTableCorrosion_speed_2010ColumnWidth;
            }
            this.AdjustColumnsWidth(control);
            return;
        }
        /// <summary>
        /// Обработчик события редакирования значения индекса объектного словаря 
        /// устройства НГК
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_dataGridViewObjectDictionary_CellParsing(object sender,
            DataGridViewCellParsingEventArgs e)
        {
            DataGridView control = (DataGridView)sender;
            DataGridViewRow row;

            //if (e.ColumnIndex ==
            //    control.Columns.IndexOf(control.Columns[FieldNamesOfObjectDictionary.VALUE]))
            //{
            //    // Получаем строку с именёнными данными
            //    row = control.Rows[e.RowIndex];

            //    // Определяем тип данных и выполняем преобрзование в соответсвии с типом данных
            //    DataConvertor convertor = (DataConvertor)row
            //        .Cells[FieldNamesOfObjectDictionary.TYPE_OF_VALUE].Value;
            //    e.Value = convertor.ConvertToBasis(System.Convert.ToBoolean(e.Value)); // Устанавливаем новое значение для ячейки.
            //    e.ParsingApplied = true;                
            //}
            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_ContextMenuStripForNetworkDeviceList_Click(
            object sender, EventArgs e)
        {
            ToolStripMenuItem menu = (ToolStripMenuItem)sender;
            //DataGridViewSelectedRowCollection rows;
            //Byte nodeId;
            //IDevice device;

            switch (menu.Name)
            {
                //case "toolStripMenuDeviceStatusSetStopped":
                //    {
                //        // Переводим устройство в состояние "Stopped"
                //        rows = this.dataGridViewDeviceList.SelectedRows;
                //        nodeId = Convert.ToByte(rows[0].Cells["NodeId"].Value);
                //        device = ((NetworkDevicesBindingList)this.dataGridViewDeviceList.DataSource).GetDevice(nodeId);
                //        device.SetDeviceStatus("User", DeviceProxy.DeviceStatus.Stopped, null);
                //        break;
                //    }
                //case "toolStripMenuDeviceStatusSetPreOperational":
                //    {
                //        // Переводим устройство в состояние "PreOperational"
                //        rows = this.dataGridViewDeviceList.SelectedRows;
                //        nodeId = Convert.ToByte(rows[0].Cells["NodeId"].Value);
                //        device = ((NetworkDevicesBindingList)this.dataGridViewDeviceList.DataSource).GetDevice(nodeId);
                //        device.SetDeviceStatus("User", DeviceProxy.DeviceStatus.Preoperational, null);
                //        break;
                //    }
                //case "toolStripMenuDeviceStatusSetOperational":
                //    {
                //        // Переводим устройство в состояние "Operational"
                //        rows = this.dataGridViewDeviceList.SelectedRows;
                //        nodeId = Convert.ToByte(rows[0].Cells["NodeId"].Value);
                //        device = ((NetworkDevicesBindingList)this.dataGridViewDeviceList.DataSource).GetDevice(nodeId);
                //        device.SetDeviceStatus("User", DeviceProxy.DeviceStatus.Operational, null);
                //        break;
                //    }
                //case "toolStripMenuDeviceStatusSetCommunicationError":
                //    {
                //        // Переводим устройство в состояние "CommunicationError"
                //        rows = this.dataGridViewDeviceList.SelectedRows;
                //        nodeId = Convert.ToByte(rows[0].Cells["NodeId"].Value);
                //        device = ((NetworkDevicesBindingList)this.dataGridViewDeviceList.DataSource).GetDevice(nodeId);
                //        device.SetDeviceStatus("User", DeviceProxy.DeviceStatus.CommunicationError, null);
                //        break;
                //    }
                //case "toolStripMenuDeviceStatusSetConfigurationError":
                //    {
                //        // Переводим устройство в состояние "ConfigurationError"
                //        rows = this.dataGridViewDeviceList.SelectedRows;
                //        nodeId = Convert.ToByte(rows[0].Cells["NodeId"].Value);
                //        device = ((NetworkDevicesBindingList)this.dataGridViewDeviceList.DataSource).GetDevice(nodeId);
                //        device.SetDeviceStatus("User", DeviceProxy.DeviceStatus.ConfigurationError, null);
                //        break;
                //    }
                //case "toolStripMenuColumnSettings":
                //    {
                //        // Отображаем форму для настройки видимости столбцов в гриде
                //        Point clientPoint;

                //        clientPoint = this.dataGridViewDeviceList.PointToScreen(this.dataGridViewDeviceList.Location);


                //        ColumnsTuningDialog.ShowDialog(this.dataGridViewDeviceList, clientPoint);
                //        break;
                //    }
                //case "toolStripMenuSimulator":
                //    {
                //        // Отображаем форму симулятора сигналов
                //        TestService.WinForms.FormSimulator form =
                //            new TestService.WinForms.FormSimulator(this._NetworkController);
                //        form.Show();
                //        break;
                //    }
                case "toolStripMenuNetworkManager":
                    {
                        // Отображаем диалог управления сетевым контроллером
                        //NetworkManager frm = new NetworkManager();
                        //frm.NetworkController = this._NetworksManager.Networks[NETWORK_CAN_0];
                        //frm.ShowDialog();
                        break;
                    }
                //case "toolStripMenuSettings":
                //    {
                //        // Отображаем форму настроек
                //        TestNetworkController.WinForms.FormSettings frm =
                //            new TestNetworkController.WinForms.FormSettings();
                //        frm.Form = this;
                //        frm.ShowDialog();
                //        this.UpdateRowsByDeviceStatus();
                //        break;
                //    }
                case "toolStripMenuStart":
                    {
                        // Запускаем контроллер сети
                        menu.Enabled = false;
                        menu.Owner.Items["toolStripMenuStop"].Enabled = true;
                        this._CanNetworksManager.Networks[NETWORK_CAN_0].Start();
                        //this.NetworkController.Start();

                        break;
                    }
                case "toolStripMenuStop":
                    {
                        menu.Enabled = false;
                        menu.Owner.Items["toolStripMenuStart"].Enabled = true;
                        this._CanNetworksManager.Networks[NETWORK_CAN_0].Stop();
                        
                        break;
                    }
                //case "toolStripMenuExit":
                //    {
                //        // Выходим из приложения
                //        Application.Exit();
                //        break;
                //    }
                default:
                    {
                        throw new NotImplementedException(
                            String.Format("Обработчик для меню {0} не реализован", menu.Name));
                    }
            }
            return;
        }
        /// <summary>
        /// Запуск работы системы
        /// </summary>
        private void Start()
        {
            for (int i = 0; i < this._CanNetworksManager.Networks.Count; i++)
            {
                this._CanNetworksManager.Networks[i].Start();
            }
            return;
        }
        /// <summary>
        /// Остановка работы системы
        /// </summary>
        private void Stop()
        {
            for (int i = 0; i < this._CanNetworksManager.Networks.Count; i++)
            {
                this._CanNetworksManager.Networks[i].Stop();
            }
            return; 
        }
        #endregion 
    
        #region ISynchronizeInvoke Members

        IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        object ISynchronizeInvoke.EndInvoke(IAsyncResult result)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        object ISynchronizeInvoke.Invoke(Delegate method, object[] args)
        {
            return _Presenter.Invoke(method, args);
        }

        bool ISynchronizeInvoke.InvokeRequired
        {
            get { return true; }
        }

        #endregion
    }// End Of Class
}// End Of Namespace
