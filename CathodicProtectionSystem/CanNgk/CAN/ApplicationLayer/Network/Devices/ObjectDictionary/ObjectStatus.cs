using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary
{
    /// <summary>
    /// ��������� ������� �������
    /// </summary>
    public enum ObjectStatus: int
    {
        /// <summary>
        /// ��� ������
        /// </summary>
        NoError = 0,
        /// <summary>
        /// ������������� ������
        /// </summary>
        UncknownError,
        /// <summary>
        /// ������ ������������, �������� � �� �� �������������
        /// �������� ������� � �������� ����������
        /// </summary>
        ConfigurationError = 1,
        /// <summary>
        /// ������ ��� ������ �� ��������� ����������
        /// </summary>
        ComunicationError = 2,
        /// <summary>
        /// �������� ������� ����� �� ����������� ���������
        /// (���� �������� �� �����������)
        /// </summary>
        OutOfRangeError = 3
    }
}
