using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NGK.CAN.ApplicationLayer.Network.Devices;

namespace NGK.CAN.ApplicationLayer.Network.Master.Services
{
    /// <summary>
    /// ������� ������ � ������������ ��� ������� �������� 
    /// </summary>
    public class Context
    {
        #region Fields And Properties
        private LinkedList<DeviceContext> _Devices;
        private LinkedListNode<DeviceContext> _CurrentContext;
        public int Count
        {
            get { return _Devices.Count; }
        }
        public DeviceContext CurrentDevice
        {
            get { return _CurrentContext.Value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="devices"></param>
        public Context(DeviceBase[] devices)
        {
            _Devices = new LinkedList<DeviceContext>();
            foreach (DeviceBase device in devices)
            {
                _Devices.AddLast(new LinkedListNode<DeviceContext>(
                    new DeviceContext(device)));
            }
            _CurrentContext = _Devices.First;
        }
        #endregion

        #region Methods
        /// <summary>
        /// ������������� ��������� ���������� � �������� ��������
        /// ��� ���� ������ ��������, �� ��������������� ������ ���������� � ������
        /// </summary>
        public void Next()
        {
            LinkedListNode<DeviceContext> node;
            node = _CurrentContext.Next;
            _CurrentContext = node == null ? _Devices.First : node;
        }
        /// <summary>
        /// ���� � ������ �������� ��� ���������� ����������
        /// </summary>
        /// <param name="nodeId">������� ������������� ����������</param>
        /// <returns></returns>
        public DeviceContext FindDevice(Byte nodeId)
        {
            foreach(DeviceContext device in _Devices)
            {
                if (device.Device.NodeId == nodeId)
                {
                    return device;
                }
            }
            return null;
        }
        #endregion
    }
}
