using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [Description("�����")]
        NoError = 0,
        /// <summary>
        /// ������������� ������
        /// </summary>
        [Description("����. ������")]
        UncknownError,
        /// <summary>
        /// ������ ������������, �������� � �� �� �������������
        /// �������� ������� � �������� ����������
        /// </summary>
        [Description("����. ������")]
        ConfigurationError = 1,
        /// <summary>
        /// ������ ��� ������ �� ��������� ����������
        /// </summary>
        [Description("������ ����.")]
        ComunicationError = 2,
        /// <summary>
        /// �������� ������� ����� �� ����������� ���������
        /// (���� �������� �� �����������)
        /// </summary>
        [Description("�����. ��������")]
        OutOfRangeError = 3
    }
}
