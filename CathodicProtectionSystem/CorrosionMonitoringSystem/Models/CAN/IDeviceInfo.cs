using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NGK.CAN.ApplicationLayer.Network.Devices;

namespace NGK.CorrosionMonitoringSystem.Models
{
    public interface IDeviceInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="?"></param>
        [DisplayName("�����")]
        byte NodeId { get; }
        /// <summary>
        /// 
        /// </summary>
        [DisplayName("���� CAN")]
        string NetworkName { get; }
        /// <summary>
        /// 
        /// </summary>
        [DisplayName("������")]
        [Description("������� ��������� ����������")]
        DeviceStatus Status { get; }
        /// <summary>
        /// 
        /// </summary>
        [DisplayName("�����������������")]
        string Location { get; }
    }
}
