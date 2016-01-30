using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Master;

namespace NGK.CorrosionMonitoringSystem.Models
{
    /// <summary>
    /// Модель - плоский список всех устройтсв в сети
    /// </summary>
    public class Devices: IDisposable
    {
        #region Constructors

        public Devices(INetworksManager netwroks)
        {
            _Devices = new List<Device>();

            _Timer = new Timer();
            _Timer.Interval = 300;
            _Timer.Tick += new EventHandler(EventHandler_Timer_Tick);
            _Timer.Start();
        }

        #endregion

        #region Fields And Properties
        
        Timer _Timer;
        INetworksManager _NetworkManager;
        List<Device> _Devices;

        #endregion

        #region Event Handlers

        void EventHandler_Timer_Tick(object sender, EventArgs e)
        {
            // Обновляем модель
            foreach (INetworkController controller in _NetworkManager.Networks)
            {
                foreach (IDevice device in controller.Devices)
                {
                   //controller.Devices[0].Cl
                }
            }
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            _Timer.Stop();
            _Timer.Dispose();
        }

        #endregion
    }
}
