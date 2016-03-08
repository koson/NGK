using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary
{
    public interface IComplexParameterConverter: ICloneable
    {
        /// <summary>
        /// ��� ������ �������� ���������
        /// </summary>
        Type ValueType { get; }
        /// <summary>
        /// ��������� �� ���� ������� ������� � ����������
        /// ����������� �������� (����������� ��������)
        /// </summary>
        /// <param name="objectValues">������ �������� ��������
        /// ������� ����������</param>
        /// <returns>�������� ������������ ���������</returns>
        Object ConvertTo(object[] objectValues);
        /// <summary>
        /// ��������� �������� ������������ ���������
        /// � ����������� ������ �������� �������� ������� ���������� 
        /// </summary>
        /// <param name="complexParamValue">�������� ������������ ���������</param>
        /// <returns>�������� �������� ������� ����������</returns>
        Object[] ConvertFrom(object complexParamValue);
        /// <summary>
        /// ��������� �� ����������� ���������� �������� ConvertTo
        /// </summary>
        /// <returns></returns>
        bool CanConvertTo();
        /// <summary>
        /// ��������� �� ����������� ���������� �������� ConvertFrom
        /// </summary>
        /// <returns></returns>
        bool CanConvertFrom();
    }
}
