using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CorrosionMonitoringSystem.BL
{
    /// <summary>
    /// Структура хранит наименование полей (столбцов) строки индекса объекта словаря 
    /// </summary>
    public struct FieldNamesOfObjectDictionary
    {
        /// <summary>
        /// Индекс объекта словаря
        /// </summary>
        public const String INDEX = "Index";
        /// <summary>
        /// Наименование объекта словаря
        /// </summary>
        public const String NAME = "Name";
        /// <summary>
        /// Наименование объекта словаря для отображения
        /// </summary>
        public const String DISPLAYED_NAME = "DisplayedName";
        /// <summary>
        /// Значение объекта словаря
        /// </summary>
        public const String VALUE = "Value";
        /// <summary>
        /// Время последнего обновления значения объекта словаря
        /// </summary>
        public const String MODIFIED = "Modified";
        /// <summary>
        /// Описание объекта словаря
        /// </summary>
        public const String DESCRIPTION = "Description";
        /// <summary>
        /// Категория объекта словаря
        /// </summary>
        public const String CATEGORY = "Category";
        /// <summary>
        /// Модификатор доступа к значению обеъкта словаря
        /// </summary>
        public const String READ_ONLY = "ReadOnly";
        /// <summary>
        /// Модификатор разрешающий цикличиский опрос данного объекта
        /// (для сетевого сервиса SDO)
        /// </summary>
        public const String ENABLE_CYCLIC_READ = "SdoCanRead"; //"EnableCyclicRead";
        /// <summary>
        /// Состояние при доступе к значению 
        /// </summary>
        public const String STATUS = "Status";
        ///// <summary>
        ///// Отпредлеляет отображается объект словаря в GUI или нет.
        ///// </summary>
        //public const String IS_VISIBLE = "IsVisible";
        /// <summary>
        /// Возвращает массив имен полей индекса объектного словаря
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
