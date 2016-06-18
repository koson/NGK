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
        [DisplayName("Адрес")]
        byte NodeId { get; }
        /// <summary>
        /// 
        /// </summary>
        [DisplayName("Сеть CAN")]
        string NetworkName { get; }
        /// <summary>
        /// 
        /// </summary>
        [DisplayName("Статус")]
        [Description("Текущее состояние устройства")]
        DeviceStatus Status { get; }
        /// <summary>
        /// 
        /// </summary>
        [DisplayName("Месторасположение")]
        string Location { get; }
    }
}
