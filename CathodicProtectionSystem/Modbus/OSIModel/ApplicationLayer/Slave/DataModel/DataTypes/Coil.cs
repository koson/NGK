using System;
using Modbus;

namespace Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes
{
    /// <summary>
    /// Класс для реализации реле модели данных modbus-устройства
    /// </summary>
    [Serializable]
    public class Coil: Parameter<Boolean>
    {
        #region Fields and Properies
        public override ModbusParameterType ParameterType
        {
            get { return ModbusParameterType.Coil; }
        }
        #endregion

        #region Constructors and Destructor
        /// <summary>
        /// Конструктор.
        /// </summary>
        private Coil(): base()
        {
            this._Value = false;            
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="address">Адрес дискретного входа\выхода</param>
        /// <param name="value">Значение дискретного входа\выхода</param>
        /// <param name="description">Описание дискретного входа\выхода</param>
        public Coil(UInt16 address, Boolean value, String description)
            :
            base(address, value, description)
        {
        }
        #endregion

        #region Methods
        #endregion

        #region Events
        #endregion
    }
}
