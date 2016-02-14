using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Master;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary;
using Common.Collections.ObjectModel;
using NGK.CorrosionMonitoringSystem.Models;

namespace NGK.CorrosionMonitoringSystem.BL
{
    /// <summary>
    /// ����� ��� ������ � �������� ����� CAN ���.
    /// �������� ������ �� ���������� ����� � ������ �� � ��������������� �������
    /// </summary>
    public class CanNetworkServiceAdapter
    {
        #region Fields And Properties
        /// <summary>
        /// ������ ���������
        /// </summary>
        List<NetworkDevice> _Devices;
        /// <summary>
        /// ������ ���������
        /// </summary>
        public List<NetworkDevice> Devices
        {
            get { return _Devices; }
        }
        /// <summary>
        /// ������� �������� ����������
        /// </summary>
        //DataTable _ObjectDictionaryTable;
        /// <summary>
        /// ������� �������
        /// </summary>
        //DataTable _PivoteTable;
        private EventHandler _ControllerChangedStatus;
        private EventHandler _DeviceChangedStatus;
        private EventHandler<KeyedCollectionWasChangedEventArgs<DeviceBase>> _DevicesAmountWasChanged;
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public CanNetworkServiceAdapter()
        {
            NetworksManager manager = NetworksManager.Instance;

            _ControllerChangedStatus = 
                new EventHandler(EventHandlerControllerChangedStatus);
            _DeviceChangedStatus = 
                new EventHandler(EventHandlerDeviceChangedStatus);
            _DevicesAmountWasChanged =
                new EventHandler<KeyedCollectionWasChangedEventArgs<DeviceBase>>(
                EventHandlerDevicesAmountWasChanged);

            _Devices = new List<NetworkDevice>();
            // �������� ��� ���������� �� ���� �����            
            foreach (NetworkController network in manager.Networks)
            {
                network.ControllerChangedStatus += _ControllerChangedStatus;
                network.Devices.CollectionWasChanged += _DevicesAmountWasChanged;
                
                foreach (DeviceBase device in network.Devices)
                {
                    device.DeviceChangedStatus += _DeviceChangedStatus;
                    // TODO:
                    //device.DataWasChanged += new EventHandler(device_DataWasChanged);
                    _Devices.Add(new NetworkDevice(device));
                }
            }
        }
        #endregion
        
        #region Methods
        /// <summary>
        /// ���������� ������� ��������� ��������� ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void EventHandlerControllerChangedStatus(
            object sender, EventArgs args)
        {
            //NetworkController controller = (NetworkController)sender;
            return;
        }
        /// <summary>
        /// ���������� ������� ��������� ��������� ����������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandlerDeviceChangedStatus(
            object sender, EventArgs e)
        {
            DeviceBase device = (DeviceBase)sender;
            // ������������� ������ ������

            foreach (NetworkDevice item in _Devices)
            {
                if ((item.NetworkId == device.Network.NetworkId) &&
                    (item.NodeId == device.NodeId))
                {
                    item.Status = device.Status;
                }
            }
        }
        /// <summary>
        /// ���������� ������� ��������� ���-�� ���������
        /// � ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandlerDevicesAmountWasChanged(
            object sender, KeyedCollectionWasChangedEventArgs<DeviceBase> e)
        {
            return;
        }
        /// <summary>
        /// ��������� ���� ������ �� �������� �������
        /// </summary>
        /// <param name="device"></param>
        public void UpdateDevice(NetworkDevice device)
        {
            DeviceBase dvc;

            dvc = NetworksManager.Instance.Networks[device.NetworkId]
                .Devices[device.NodeId];
            device.Location = dvc.LocationName;
            device.PollingInterval = dvc.PollingInterval;
            device.Status = dvc.Status;
        }
        /// <summary>
        /// ��������� �������� ���������� ������� ����������
        /// </summary>
        /// <param name="device"></param>
        public void UpdateObjectDictionary(NetworkDevice device)
        {
            DeviceBase dvc;

            dvc = NetworksManager.Instance.Networks[device.NetworkId]
                .Devices[device.NodeId];
            // ��������� ������� �������� ������
            foreach (Parameter parameter in device.ObjectDictionary)
            { 
                DataObject param = dvc.ObjectDictionary[parameter.Index];
                parameter.Modified = param.Modified;
                parameter.Status = param.Status;
                //if (param.TotalValue is Boolean)
                //{
                //    parameter.Value = (Boolean)param.TotalValue ? "��" : "���";
                //}
                //else
                //{
                //    parameter.Value = param.TotalValue.ToString();
                //}
                parameter.Value = param.TotalValue;
            }
        }

        #endregion
    }
}
