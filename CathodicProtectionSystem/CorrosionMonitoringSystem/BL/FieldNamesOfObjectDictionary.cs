using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CorrosionMonitoringSystem.BL
{
    /// <summary>
    /// ��������� ������ ������������ ����� (��������) ������ ������� ������� ������� 
    /// </summary>
    public struct FieldNamesOfObjectDictionary
    {
        /// <summary>
        /// ������ ������� �������
        /// </summary>
        public const String INDEX = "Index";
        /// <summary>
        /// ������������ ������� �������
        /// </summary>
        public const String NAME = "Name";
        /// <summary>
        /// ������������ ������� ������� ��� �����������
        /// </summary>
        public const String DISPLAYED_NAME = "DisplayedName";
        /// <summary>
        /// �������� ������� �������
        /// </summary>
        public const String VALUE = "Value";
        /// <summary>
        /// ����� ���������� ���������� �������� ������� �������
        /// </summary>
        public const String MODIFIED = "Modified";
        /// <summary>
        /// �������� ������� �������
        /// </summary>
        public const String DESCRIPTION = "Description";
        /// <summary>
        /// ��������� ������� �������
        /// </summary>
        public const String CATEGORY = "Category";
        /// <summary>
        /// ����������� ������� � �������� ������� �������
        /// </summary>
        public const String READ_ONLY = "ReadOnly";
        /// <summary>
        /// ����������� ����������� ����������� ����� ������� �������
        /// (��� �������� ������� SDO)
        /// </summary>
        public const String ENABLE_CYCLIC_READ = "SdoCanRead"; //"EnableCyclicRead";
        /// <summary>
        /// ��������� ��� ������� � �������� 
        /// </summary>
        public const String STATUS = "Status";
        ///// <summary>
        ///// ������������ ������������ ������ ������� � GUI ��� ���.
        ///// </summary>
        //public const String IS_VISIBLE = "IsVisible";
        /// <summary>
        /// ���������� ������ ���� ����� ������� ���������� �������
        /// </summary>
        /// <returns></returns>
        public static String[] GetNames()
        {
            String[] names = new string[10];
            names[0] = FieldNamesOfObjectDictionary.INDEX;
            names[1] = FieldNamesOfObjectDictionary.NAME;
            names[2] = FieldNamesOfObjectDictionary.DISPLAYED_NAME;
            names[3] = FieldNamesOfObjectDictionary.VALUE;
            names[4] = FieldNamesOfObjectDictionary.MODIFIED;
            names[5] = FieldNamesOfObjectDictionary.DESCRIPTION;
            names[6] = FieldNamesOfObjectDictionary.CATEGORY;
            names[7] = FieldNamesOfObjectDictionary.READ_ONLY;
            names[8] = FieldNamesOfObjectDictionary.ENABLE_CYCLIC_READ;
            names[9] = FieldNamesOfObjectDictionary.STATUS;
            //names[10] = FieldNamesOfObjectDictionary.IS_VISIBLE;
            return names;
        }
        //--------------------------------------------------------------------------------
    }
    //====================================================================================
}
//========================================================================================
// End Of File
