using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataTypes;
using System.ComponentModel;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary
{
    /// <summary>
    /// ����� ��� �������� ������� ������� ����������
    /// </summary>
    public class ObjectInfo
    {
        #region Constructors
        
        /// <summary>
        /// ����������� �������� ������� �������
        /// </summary>
        /// <param name="deviceProfile"></param>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="readOnly"></param>
        /// <param name="sdoCanRead"></param>
        /// <param name="visible"></param>
        /// <param name="displayName"></param>
        /// <param name="measureUnit"></param>
        /// <param name="category"></param>
        /// <param name="convertor"></param>
        /// <param name="defaultValue"></param>
        /// <param name="typeConverter"></param>
        public ObjectInfo(ICanDeviceProfile deviceProfile, UInt16 index, string name, string description,
            bool readOnly, bool sdoCanRead, bool visible, string displayName,
            string measureUnit, ObjectCategory category, ICanDataTypeConvertor convertor,
            UInt32 defaultValue, TypeConverter typeConverter)
        {
            DeviceProfile = deviceProfile;
            Index = index;
            Name = name;
            Description = description;
            ReadOnly = readOnly;
            SdoCanRead = sdoCanRead;
            Visible = visible;
            DisplayName = displayName;
            MeasureUnit = measureUnit;
            Category = category;
            DataTypeConvertor = convertor;
            DefaultValue = defaultValue;
            ComplexParameterName = null;
            TypeConverter = typeConverter;
        }

        /// <summary>
        /// ����������� ��� �������� ������������ (���������) ������� (�������� ������� �������
        /// �� ���������� �������� �������)
        /// </summary>
        /// <param name="deviceProfile"></param>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="readOnly"></param>
        /// <param name="sdoCanRead"></param>
        /// <param name="visible"></param>
        /// <param name="displayName"></param>
        /// <param name="measureUnit"></param>
        /// <param name="category"></param>
        /// <param name="convertor"></param>
        /// <param name="defaultValue"></param>
        /// <param name="complexParameterName"></param>
        public ObjectInfo(ICanDeviceProfile deviceProfile, UInt16 index, string name, string description,
            bool readOnly, bool sdoCanRead, bool visible, string displayName,
            string measureUnit, ObjectCategory category, ICanDataTypeConvertor convertor,
            UInt32 defaultValue, String complexParameterName)
        {
            DeviceProfile = deviceProfile;
            Index = index;
            Name = name;
            Description = description;
            ReadOnly = readOnly;
            SdoCanRead = sdoCanRead;
            Visible = visible;
            DisplayName = displayName;
            MeasureUnit = measureUnit;
            Category = category;
            DataTypeConvertor = convertor;
            DefaultValue = defaultValue;
            ComplexParameterName = complexParameterName;
        }

        #endregion

        #region Fields And Properties
        /// <summary>
        /// ����� �������
        /// </summary>
        public UInt16 Index;
        /// <summary>
        /// �������� �������
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// �������� �������
        /// </summary>
        public readonly string Description;
        /// <summary>
        /// ������ ������ (������ �������� �������� � ������� ����������)
        /// </summary>
        public readonly bool ReadOnly;
        /// <summary>
        /// 
        /// </summary>
        public readonly bool SdoCanRead;
        /// <summary>
        /// ���������/��������� ����������� 
        /// ������� ������� � GUI
        /// </summary>
        public readonly bool Visible;
        /// <summary>
        /// ������������ ������� � GUI
        /// </summary>
        public readonly string DisplayName;
        /// <summary>
        /// ������� ���������
        /// </summary>
        public readonly string MeasureUnit;
        /// <summary>
        /// ��������� ������� �������
        /// </summary>
        public readonly ObjectCategory Category;
        /// <summary>
        /// ��������� ��� �������������� ������ ���������� �� ���� 
        /// � �������� �������� � ������� 
        /// </summary>
        public readonly ICanDataTypeConvertor DataTypeConvertor;
        /// <summary>
        /// �������� �� �������� (�������� � ������� ������������� ������)
        /// </summary>
        public UInt32 DefaultValue;
        /// <summary>
        /// 
        /// </summary>
        public readonly string ComplexParameterName;

        /// <summary>
        /// ��������� �������� �� ������ ������ ���������� ���������� ���� ������
        /// </summary>
        public bool IsComplexParameter
        {
            get { return ComplexParameterName != null; }
        }

        /// <summary>
        /// ��������� ��� �������������� �������� ������� ������ � �������
        /// (�������� ������� TotalValue )
        /// </summary>
        public readonly TypeConverter TypeConverter;

        public readonly ICanDeviceProfile DeviceProfile;

        #endregion
    }
}
