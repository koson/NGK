using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using NGK.CAN.ApplicationLayer.Network.Master;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CorrosionMonitoringSystem.Models;
using Mvp.WinApplication;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public class CanNetworkService: ICanNetworkService, IDisposable
    {
        public CanNetworkService(IApplicationController application, 
            INetworksManager networkManager, double pollingInterval)
        {
            _NetworkManager = networkManager;
            _NetworkManager.NetworkChangedStatus += 
                new EventHandler<NetworkChangedStatusEventAgrs>(
                EventHandler_NetworkManager_NetworkChangedStatus);
            _Application = application;
            List<CanDevice> allDevices = new List<CanDevice>();
            foreach (INetworkController network in _NetworkManager.Networks)
            {
                foreach (IDevice device in network.Devices)
                {
                    allDevices.Add(new CanDevice(device));
                }
            }
            _Devices = allDevices.ToArray();
            _Timer = new Timer();
            _Timer.AutoReset = true;
            _Timer.Interval = pollingInterval;
            _Timer.Elapsed += new ElapsedEventHandler(EventHandler_Timer_Elapsed);
        }

        INetworksManager _NetworkManager;
        IApplicationController _Application;
        CanDevice[] _Devices;
        Timer _Timer;

        #region ICanNetworkService Members

        public CanDevice[] Devices
        {
            get { return _Devices; }
        }

        #endregion

        #region IManageable Members

        public void Start()
        {
            if (_Status == Common.Controlling.Status.Running)
                return;

            _Status = Common.Controlling.Status.Running;
            _Timer.Start();
            
        }

        public void Stop()
        {
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

        Common.Controlling.Status _Status;
        public Common.Controlling.Status Status
        {
            get { return _Status; }
            set
            {
                switch (value)
                {
                    case Common.Controlling.Status.Running: { Start(); break; }
                    case Common.Controlling.Status.Stopped: { Start(); break; }
                    default: { throw new NotSupportedException(); }
                }
            }
        }

        public event EventHandler StatusWasChanged;

        #endregion

        void EventHandler_NetworkManager_NetworkChangedStatus(
            object sender, NetworkChangedStatusEventAgrs e)
        {
            //TODO: Сделать пометку в журнал и оповещение об отключение сети.
        }

        void EventHandler_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            if (_Application.CurrentForm.InvokeRequired)
            {
                _Application.CurrentForm.Invoke((System.Windows.Forms.MethodInvoker)
                    delegate() {
                        foreach (CanDevice device in _Devices)
                        {
                            //device.
                        }
                    });
            }
            throw new Exception("The method or operation is not implemented.");
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
