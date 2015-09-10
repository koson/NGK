using System;
using System.Xml.Serialization;
using Modbus;

namespace Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes
{
    /// <summary>
    /// Класс для реализации реле модели данных modbus-устройства
    /// </summary>
    [Serializable]
    public class DiscreteInput : Parameter<Boolean>
    {
        #region Fields and Properies
        public override ModbusParameterType ParameterType
        {
            get { return ModbusParameterType.DiscreteInput; }
        }
        #endregion

        #region Constructors and Destructor
        /// <summary>
        /// Конструктор.
        /// </summary>
        public DiscreteInput()
        {
            this.Value = false;
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="address">Адрес дискретного входа</param>
        /// <param name="value">Значение дискретного входа</param>
        /// <param name="description">Описание дискретного входа</param>
        public DiscreteInput(UInt16 address, Boolean value, String description)
            :
            base(address, value, description)
        {
        }
        #endregion

        #region Methods
        //---------------------------------------------------------------------------
        //---------------------------------------------------------------------------
        #endregion

        #region Events
        //---------------------------------------------------------------------------
        //---------------------------------------------------------------------------
        #endregion
    }
}
