using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace SystemParametersRecorder.Models
{
    /// <summary>
    /// ���������� ��������� ������� ������ ���������� � ��
    /// </summary>
    public interface IDevice
    {
        #region Fields And Properties
        /// <summary>
        /// ������������� CAN ����
        /// </summary>
        [DisplayName("Id CAN-����")]
        int CanNetworkId { get; }
        /// <summary>
        /// ������������ CAN ����
        /// </summary>
        string CanNetworkName { get; }
        /// <summary>
        /// ������� ����� ����������
        /// </summary>
        /// <param name="?"></param>
        [DisplayName("�����")]
        byte NodeId { get; }
        /// <summary>
        /// ������������ ����������
        /// </summary>
        [DisplayName("�����������������")]
        string Location { get; }
        /// <summary>
        /// ��������������� ���������, � (0x2008)
        /// null - ���� ��������� ������� ��������� ���������
        /// </summary>
        [DisplayName("��������������� ���������, B")]
        float? PolarisationPotential { get; }
        /// <summary>
        /// ��� �����������, mA (0x200�)
        /// null - ���� ��������� ������� ��������� ���������
        /// </summary>
        [DisplayName("��� �����������, mA")]
        float? PolarisationCurrent { get; }
        /// <summary>
        /// �������� ���������, � (0x2009)
        /// null - ���� ��������� ������� ��������� ���������
        /// </summary>
        [DisplayName("�������� ���������, B")]
        float? ProtectionPotential { get; }
        /// <summary>
        /// ��� �������� ������, � (0x200B)
        /// null - ���� ��������� ������� ��������� ���������
        /// </summary>
        [DisplayName("��� �������� ������, A")]
        float? ProtectionCurrent { get; }
        /// <summary>
        /// ������� �������� (0x200F)
        /// </summary>
        [DisplayName("������� ��������, ���")]
        UInt32? CorrosionDepth { get;}
        /// <summary>
        /// �������� �������� (0x2010)
        /// </summary>
        [DisplayName("�������� ��������, ���/���")]
        UInt32? CorrosionSpeed { get; }

        #endregion
    }
}
