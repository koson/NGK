using System;
using System.Collections.Generic;
using System.Text;

namespace Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes
{
    /// <summary>
    /// Класс для реализации записи в составе файла модели данных modbus
    /// </summary>
    public class Record: Parameter<UInt16>
    {
        #region Fields And Properties
        public override ModbusParameterType ParameterType
        {
            get { return ModbusParameterType.Record; }
        }
        #endregion

        #region Constructors and Destructor
        /// <summary>
        /// Конструктор.
        /// </summary>
        private Record(): base()
        {
            _Value = 0;            
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="address">Адрес дискретного входа\выхода</param>
        /// <param name="value">Значение дискретного входа\выхода</param>
        /// <param name="description">Описание дискретного входа\выхода</param>
        public Record(UInt16 address, UInt16 value, String description)
            :
            base(address, value, description)
        {
        }
        #endregion

        #region Methods
        //---------------------------------------------------------------------------
        //---------------------------------------------------------------------------
        #endregion
    }
}
