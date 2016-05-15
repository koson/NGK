using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.ComponentModel;
using NGK.CAN.ApplicationLayer.Network.Master;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CorrosionMonitoringSystem.Models;
using Mvp.WinApplication;
using Common.Controlling;
using System.Data;
using System.Diagnostics;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public class CanNetworkService: ApplicationServiceBase,
        ICanNetworkService, IDisposable
    {
        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="application"></param>
        /// <param name="networkManager"></param>
        /// <param name="pollingInterval">Интервал обновления CAN-устройства, мсек</param>
        public CanNetworkService(string serviceName, 
            INgkCanNetworksManager networkManager, double pollingInterval):
            base(serviceName)
        {
            _NetworkManager = networkManager;
            _NetworkManager.NetworkChangedStatus += 
                new EventHandler<NetworkChangedStatusEventAgrs>(
                EventHandler_NetworkManager_NetworkChangedStatus);

            _Devices = new BindingList<NgkCanDevice>();

            _Timer = new Timer();
            _Timer.AutoReset = true;
            _Timer.Interval = pollingInterval;
            _Timer.Elapsed += new ElapsedEventHandler(EventHandler_Timer_Elapsed);
        }

        #endregion

        #region Fields And Properties

        INgkCanNetworksManager _NetworkManager;
        Timer _Timer;
        ParametersPivotTable _ParatemersPivotTable;

        BindingList<NgkCanDevice> _Devices;
        /// <summary>
        /// Список устройств в системе
        /// </summary>
        public BindingList<NgkCanDevice> Devices 
        { 
            get { return _Devices; } 
        }

        int _FaultyDevices;
        /// <summary>
        /// Количество неисправных устройств
        /// </summary>
        public int FaultyDevices 
        { 
            get { return _FaultyDevices; }
            private set
            {
                if (_FaultyDevices != value)
                {
                    _FaultyDevices = value;
                    OnFaultyDevicesChanged();
                }

            } 
        }
        /// <summary>
        /// Сводная таблица параметров системы коррозионного 
        /// мониторинга
        /// </summary>
        public DataTable ParametersPivotTable 
        {
            get { return _ParatemersPivotTable.PivotTable; } 
        }

        #endregion

        #region Methods

        public override void  Initialize(object context)
        {
            _Devices.Clear();

            foreach (CanNetworkController network in _NetworkManager.Networks)
            {
                foreach (IDevice device in network.Devices)
                {
                    _Devices.Add(new NgkCanDevice(device));
                }
            }

            _ParatemersPivotTable = new ParametersPivotTable(_Devices); 

            base.Initialize(context);
        }

        public override void OnStarting()
        {
            StartCanNetwork();

            _Devices.Clear();

            foreach (ICanNetworkController network in _NetworkManager.Networks)
            {
                foreach (IDevice device in network.Devices)
                {
                    _Devices.Add(new NgkCanDevice(device));
                }
            }

            _Timer.Start();
        }

        public override void OnStopping()
        {
            _Timer.Stop();
            StopCanNetwork();
        }

        public override void Suspend()
        {
            throw new NotSupportedException();
        }

        void StartCanNetwork()
        {
            foreach (ICanNetworkController network in NgkCanNetworksManager.Instance.Networks)
            {
                if (network.CanPort != null)
                {
                    network.Start();
                }
                else
                {
                    //TODO: Сообщение пользователю и запись в журнал.
                }
            }
        }

        void StopCanNetwork()
        {
            foreach (ICanNetworkController network in NgkCanNetworksManager.Instance.Networks)
            {
                network.Stop();
            }
        }

        public override void Dispose()
        {
            if (_Timer != null)
                _Timer.Dispose();
        }

        void OnFaultyDevicesChanged()
        {
            if (FaultyDevicesChanged != null)
                FaultyDevicesChanged(this, new EventArgs());
        }
        
        #endregion

        #region Event Handlers

        void EventHandler_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.SyncContext.Post(delegate(object state) 
            {
                IDevice canDevice;
                int faultyDevices = 0;

                foreach (NgkCanDevice device in _Devices)
                {
                    canDevice = NgkCanNetworksManager.Instance.Networks[device.NetworkId]
                        .Devices[device.NodeId];
                    NgkCanDevice.Update(device, canDevice);

                    if (canDevice.Status == DeviceStatus.CommunicationError)
                        faultyDevices++;
                }

                FaultyDevices = faultyDevices;

                // Обновляем сводную таблицу
                _ParatemersPivotTable.Update();
            }, null);
        }

        void EventHandler_NetworkManager_NetworkChangedStatus(
            object sender, NetworkChangedStatusEventAgrs e)
        {
            //TODO: Сделать пометку в журнал и оповещение об отключение сети.
        }

        #endregion

        #region Events

        public event EventHandler FaultyDevicesChanged;

        #endregion
    }
}
