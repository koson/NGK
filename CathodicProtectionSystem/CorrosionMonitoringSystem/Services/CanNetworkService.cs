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

namespace NGK.CorrosionMonitoringSystem.Services
{
    public class CanNetworkService: ICanNetworkService, IDisposable
    {
        #region Constructors

        public CanNetworkService(IApplicationController application, 
            INetworksManager networkManager, double pollingInterval)
        {
            _NetworkManager = networkManager;
            _NetworkManager.NetworkChangedStatus += 
                new EventHandler<NetworkChangedStatusEventAgrs>(
                EventHandler_NetworkManager_NetworkChangedStatus);
            _Application = application;

            _Devices = new BindingList<NgkCanDevice>();

            _Timer = new Timer();
            _Timer.AutoReset = true;
            _Timer.Interval = pollingInterval;
            _Timer.Elapsed += new ElapsedEventHandler(EventHandler_Timer_Elapsed);
        }

        #endregion

        #region Fields And Properties

        INetworksManager _NetworkManager;
        IApplicationController _Application;
        Timer _Timer;
        ParametersPivotTable _ParatemersPivotTable;

        BindingList<NgkCanDevice> _Devices;
        public BindingList<NgkCanDevice> Devices 
        { 
            get { return _Devices; } 
        }

        int _FaultyDevices;    
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

        Status _Status;
        public Status Status
        {
            get { return _Status; }
            set
            {
                switch (value)
                {
                    case Common.Controlling.Status.Running:
                        {
                            Start();
                            break;
                        }
                    case Common.Controlling.Status.Stopped:
                        {
                            Start();
                            break;
                        }
                    default:
                        { throw new NotSupportedException(); }
                }
            }
        }

        public DataTable ParametersPivotTable 
        {
            get { return _ParatemersPivotTable.PivotTable; } 
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            _Devices.Clear();

            foreach (NetworkController network in _NetworkManager.Networks)
            {
                foreach (IDevice device in network.Devices)
                {
                    _Devices.Add(new NgkCanDevice(device));
                }
            }

            _ParatemersPivotTable = new ParametersPivotTable(
                (new List<NgkCanDevice>(_Devices)).ToArray());
        }

        public void Start()
        {
            if (_Status == Status.Running)
                return;

            _Status = Status.Running;
            
            StartCanNetwork();

            _Devices.Clear();

            foreach (INetworkController network in _NetworkManager.Networks)
            {
                foreach (IDevice device in network.Devices)
                {
                    _Devices.Add(new NgkCanDevice(device));
                }
            }

            _Timer.Start();
            OnStatusWasChanged();
        }

        public void Stop()
        {
            if (_Status == Status.Stopped)
                return;

            StopCanNetwork();
            _Timer.Stop();
            _Status = Status.Stopped;
            OnStatusWasChanged();
        }

        public void Suspend()
        {
            throw new NotSupportedException(
                "The method or operation is not implemented.");
        }

        void StartCanNetwork()
        {
            foreach (INetworkController network in NetworksManager.Instance.Networks)
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
            foreach (INetworkController network in NetworksManager.Instance.Networks)
            {
                network.Stop();
            }
        }

        public void Dispose()
        {
            if (_Timer != null)
                _Timer.Dispose();
        }

        void OnStatusWasChanged()
        {
            if (StatusWasChanged != null)
                StatusWasChanged(this, new EventArgs());
        }

        void OnFaultyDevicesChanged()
        {
            if (FaultyDevicesChanged != null)
                FaultyDevicesChanged(this, new EventArgs());
        }

        #endregion

        #region Events Handler

        void EventHandler_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _Application.SyncContext.Post(delegate(object state) 
            {
                IDevice canDevice;
                int faultyDevices = 0;

                foreach (NgkCanDevice device in _Devices)
                {
                    canDevice = NetworksManager.Instance.Networks[device.NetworkId]
                        .Devices[device.NodeId];
                    NgkCanDevice.Update(device, canDevice);

                    if (canDevice.Status == DeviceStatus.CommunicationError)
                        faultyDevices++;
                }

                FaultyDevices = faultyDevices;
            }, null);


            //if (_Application.CurrentForm.InvokeRequired)
            //{
            //    _Application.CurrentForm.Invoke((System.Windows.Forms.MethodInvoker)
            //        delegate()
            //        {
            //            IDevice canDevice;
            //            int faultyDevices = 0;

            //            foreach (NgkCanDevice device in _Devices)
            //            {
            //                canDevice = NetworksManager.Instance.Networks[device.NetworkId]
            //                    .Devices[device.NodeId];
            //                NgkCanDevice.Update(device, canDevice);

            //                if (canDevice.Status == DeviceStatus.CommunicationError)
            //                    faultyDevices++;
            //            }

            //            FaultyDevices = faultyDevices;
            //        });
            //}
        }

        void EventHandler_NetworkManager_NetworkChangedStatus(
            object sender, NetworkChangedStatusEventAgrs e)
        {
            //TODO: Сделать пометку в журнал и оповещение об отключение сети.
        }

        #endregion

        #region Events

        public event EventHandler FaultyDevicesChanged;
        public event EventHandler StatusWasChanged;
        
        #endregion
    }
}
