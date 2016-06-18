using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NGK.CorrosionMonitoringSystem.Models
{
    public interface IDeviceSummaryParameters
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
    }
}
