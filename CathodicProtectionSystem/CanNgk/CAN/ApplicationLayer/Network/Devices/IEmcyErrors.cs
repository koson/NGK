using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.ApplicationLayer.Network.Devices
{
    /// <summary>
    /// ����� ������ ��� ������� EMCY
    /// ��� ���������� ������ ����������� ������ ���������
    /// </summary>
    public interface IEmcyErrors
    {
        /// <summary>
        /// ���� �������� 
        /// </summary>
        bool Tamper { get; set; }
        /// <summary>
        /// O����� �������� (���������) �������
        /// </summary>
        bool MainSupplyPowerError { get; set; }
        /// <summary>
        /// ������������� ���������� ������� �������
        /// </summary>
        bool BatteryError { get; set; }
        /// <summary>
        /// ������ ����������� ��(�)-01
        /// </summary>
        bool RegistrationError { get; set; }
        /// <summary>
        /// ������ ������������ ������ ��(�)-01.
        /// </summary>
        bool DuplicateAddressError { get; set; }
        /// <summary>
        /// ����������� ���������� �������.
        /// </summary>
        bool ConnectedServiceConnector { get; set; }
    }
}
