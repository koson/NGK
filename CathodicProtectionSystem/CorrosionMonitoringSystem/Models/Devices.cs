using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Master;

namespace NGK.CorrosionMonitoringSystem.Models
{
    /// <summary>
    /// ћодель - плоский список всех устройтсв в сети
    /// </summary>
    public class Devices
    {
        #region Constructors

        public Devices(INetworksManager netwroks)
        {
            _NetworkManager = netwroks;
            List<IDevice> list = new List<IDevice>();

            foreach (INetworkController network in _NetworkManager.Networks)
            {
                list.AddRange(network.Devices);
            }

            _Devices = list.ToArray();
        }

        #endregion

        #region Fields And Properties
        
        INetworksManager _NetworkManager;
        IDevice[] _Devices;

        public IDevice[] Devices
        {
            get { return _Devices; }
        }

        #endregion

        #region Methods
        #endregion
    }
}
