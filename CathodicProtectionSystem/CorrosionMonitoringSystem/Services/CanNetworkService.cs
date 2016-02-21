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
        BindingList<NgkCanDevice> _Devices;
        Timer _Timer;

        public BindingList<NgkCanDevice> Devices { get { return _Devices; } }

        #endregion

        #region IManageable Members

        public void Start()
        {
            StartCanNetwork();

            _Devices.Clear();

            foreach (INetworkController network in _NetworkManager.Networks)
            {
                foreach (IDevice device in network.Devices)
                {
                    _Devices.Add(new NgkCanDevice(device));
                }
            }

            if (_Status == Common.Controlling.Status.Running)
                return;

            _Status = Common.Controlling.Status.Running;
            _Timer.Start();
            
        }

        public void Stop()
        {
            StopCanNetwork();

            if (_Status == Common.Controlling.Status.Stopped)
                return;

            _Status = Common.Controlling.Status.Stopped;
            _Timer.Stop();
        }

        public void Suspend()
        {
            throw new NotSupportedException(
                "The method or operation is not implemented.");
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

        public event EventHandler StatusWasChanged;

        #endregion

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

        void EventHandler_NetworkManager_NetworkChangedStatus(
            object sender, NetworkChangedStatusEventAgrs e)
        {
            //TODO: Сделать пометку в журнал и оповещение об отключение сети.
        }

        void EventHandler_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IDevice canDevice;

            if (_Application.CurrentForm.InvokeRequired)
            {
                _Application.CurrentForm.Invoke((System.Windows.Forms.MethodInvoker)
                    delegate() {
                        foreach (NgkCanDevice device in _Devices)
                        {
                            canDevice = NetworksManager.Instance.Networks[device.NetworkId]
                                .Devices[device.NodeId];
                            NgkCanDevice.Update(device, canDevice);
                        }
                    });
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_Timer != null)
                _Timer.Dispose();
        }

        #endregion
    }
}
