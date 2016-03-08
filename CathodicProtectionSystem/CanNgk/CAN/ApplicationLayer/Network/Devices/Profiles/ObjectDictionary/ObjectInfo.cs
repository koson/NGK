using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataTypes;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary
{
    /// <summary>
    /// ����� ��� �������� ������� ������� ����������
    /// </summary>
    public class ObjectInfo
    {
        #region Constructors

        public ObjectInfo(IProfile deviceProfile, UInt16 index, string name, string description,
            bool readOnly, bool sdoCanRead, bool visible, string displayName,
            string measureUnit, ObjectCategory category, ICanDataTypeConvertor convertor,
            UInt32 defaultValue)
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
        }

        public ObjectInfo(IProfile deviceProfile, UInt16 index, string name, string description,
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
        /// ��� ������ �������� �������
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

        public readonly IProfile DeviceProfile;

        #endregion
    }
}
