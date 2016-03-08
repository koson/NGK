using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary
{
    public class ComplexParameter
    {
        #region Constructors

        public ComplexParameter(string parameterName, string displayName,
            string description, bool readOnly, bool visible, string measureUnit,
            ObjectCategory category, IComplexParameterConverter converter, 
            params UInt16[] indexes)
        {
            Name = parameterName;
            DisplayName = displayName;
            Description = description;
            ReadOnly = readOnly;
            Visible = visible;
            MeasureUnit = measureUnit;
            Category = category;
            LinkedIndexes = indexes;
            _Converter = converter;
        }

        #endregion

        #region Fields And Properties
        /// <summary>
        /// ������������ ���������
        /// </summary>
        public readonly String Name;
        public readonly String DisplayName;
        public readonly string Description;
        public readonly bool ReadOnly;
        public readonly bool Visible;
        public readonly string MeasureUnit;
        public readonly ObjectCategory Category;

        /// <summary>
        /// ������� �������� ������� ����������, ������� ���������
        /// � ����������� ����������� ���� ���������
        /// </summary>
        public readonly UInt16[] LinkedIndexes;

        IComplexParameterConverter _Converter;
        /// <summary>
        /// ������ ��� ������ ��������� �������� ��������� �� 
        /// �������� ������� 
        /// </summary>
        public IComplexParameterConverter Converter
        {
            get { return (IComplexParameterConverter)_Converter.Clone(); }
        }

        #endregion
    }
}
