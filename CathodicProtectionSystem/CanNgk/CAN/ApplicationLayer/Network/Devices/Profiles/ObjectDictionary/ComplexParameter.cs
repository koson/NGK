using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary
{
    public class ComplexParameter
    {
        #region Constructors

        public ComplexParameter(string parameterName, UInt16[] indexes, IObjectsCombiner combiner)
        {
            Name = parameterName;
            LinkedIndexes = indexes;
            Combiner = combiner;
        }

        #endregion

        #region Fields And Properties
        /// <summary>
        /// ������������ ���������
        /// </summary>
        public readonly String Name;
        /// <summary>
        /// ������� �������� ������� ����������, ������� ���������
        /// � ����������� ����������� ���� ���������
        /// </summary>
        public readonly UInt16[] LinkedIndexes;
        /// <summary>
        /// ������ ��� ������ ��������� �������� ��������� �� 
        /// �������� ������� 
        /// </summary>
        public readonly IObjectsCombiner Combiner;

        #endregion
    }
}
