using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace NGK.CAN.DataTypes
{
    /// <summary>
    /// ������� ����� ��� �������� ����� ������
    /// </summary>
    public interface ICanDataTypeConvertor: IXmlSerializable
    {
        #region Fields And Properties

        /// <summary>
        /// ��������� �����. ����� ������ �����
        /// (�� struct Scaler)
        /// </summary>
        decimal Scaler { get; }
        
        /// <summary>
        /// ����� �������� ��� �����������
        /// </summary>
        Boolean Signed { get; }
        
        /// <summary>
        /// ����� ���
        /// </summary>
        Boolean IsBoolean { get; }

        /// <summary>
        /// ��� ������ TotalValue
        /// </summary>
        Type OutputDataType { get; }

        #endregion

        #region Methods

        /// <summary>
        /// ����������� ������� ����� (�������� ������������ �� ����) 
        /// � �������� ����� 
        /// </summary>
        /// <param name="value">������������� �������� ���������� � ������  
        /// (��������/�����������, �����/������������)</param>
        /// <returns></returns>
        ValueType ConvertToOutputValue(UInt32 basis);
        /// <summary>
        /// ����������� �������� � ����� ��� �������� �� ���� 
        /// ��� ��� �������� � ������� ��������
        /// </summary>
        /// <param name="totalValue"></param>
        /// <returns></returns>
        UInt32 ConvertToBasis(ValueType outputValue);
        /// <summary>
        /// ����������� ����� � ������ ��� �������� � ����
        /// </summary>
        Byte[] ToArray(UInt32 basis);
        /// <summary>
        /// ����������� ������ ���� � ����� 
        /// </summary>
        /// <param name="array"></param>
        /// <returns>Basis of value</returns>
        UInt32 ConvertFromArray(Byte[] array);

        #endregion
    }
}
