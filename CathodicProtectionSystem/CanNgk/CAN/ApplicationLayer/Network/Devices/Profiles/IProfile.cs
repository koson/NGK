using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary.Collections;
using NGK.CAN.DataTypes;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles
{
    /// <summary>
    /// ������� ����������
    /// </summary>
    public interface IProfile
    {
        #region Fields And Properties
        /// <summary>
        /// ��� ����������
        /// </summary>
        DeviceType DeviceType { get; }
        /// <summary>
        /// ���������� �� ����������
        /// </summary>
        string Description { get; }
        /// <summary>
        /// ������ ��
        /// </summary>
        Version SoftwareVersion { get; }
        /// <summary>
        /// ������ ����������
        /// </summary>
        Version HardwareVersion { get; }
        /// <summary>
        /// �������� �������� ������� ����������
        /// </summary>
        ObjectInfoCollection ObjectInfoList { get; }

        #endregion
    }
}
