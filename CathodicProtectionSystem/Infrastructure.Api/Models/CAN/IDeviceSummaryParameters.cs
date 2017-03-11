using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Infrastructure.Api.Models.CAN
{
    public interface IDeviceSummaryParameters: INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="?"></param>
        [DisplayName("�����")]
        [Browsable(false)]
        byte NodeId { get; }
        /// <summary>
        /// 
        /// </summary>
        [DisplayName("������� �� �������")]
        [Description("����� ������������")]
        [Browsable(true)]
        string Location { get; }
        /// <summary>
        /// ��������������� ���������, � (0x2008)
        /// null - ���� ��������� ������� ��������� ���������
        /// </summary>
        [DisplayName("\"���\", B")]
        [Description("��������������� ���������, B")]
        [Browsable(true)]
        float? PolarisationPotential { get; }
        /// <summary>
        /// ��� �����������, mA (0x200�)
        /// null - ���� ��������� ������� ��������� ���������
        /// </summary>
        [DisplayName("��� �����������, mA")]
        [Browsable(false)]
        float? PolarisationCurrent { get; }
        /// <summary>
        /// �������� ���������, � (0x2009)
        /// null - ���� ��������� ������� ��������� ���������
        /// </summary>
        [DisplayName("\"���\", B")]
        [Description("�������� ���������, B")]
        [Browsable(true)]
        float? ProtectionPotential { get; }
        /// <summary>
        /// ��� �������� ������, � (0x200B)
        /// null - ���� ��������� ������� ��������� ���������
        /// </summary>
        [DisplayName("��� �������� ������, A")]
        [Browsable(true)]
        float? ProtectionCurrent { get; }
        /// <summary>
        /// ������� �������� (0x200F)
        /// </summary>
        [DisplayName("������� ��������, ���")]
        [Browsable(true)]
        UInt32? CorrosionDepth { get;}
        /// <summary>
        /// �������� �������� (0x2010)
        /// </summary>
        [DisplayName("�������� ��������, ���/���")]
        [Browsable(true)]
        UInt32? CorrosionSpeed { get; }
        /// <summary>
        /// �������� ������� �������
        /// </summary>
        [DisplayName("��������")]
        [Browsable(true)]
        Boolean Tamper { get; }
    }
}
