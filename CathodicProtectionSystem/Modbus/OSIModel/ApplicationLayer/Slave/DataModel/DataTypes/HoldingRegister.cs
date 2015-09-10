using System;

namespace Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes
{
    /// <summary>
    /// Класс для создания регистра хранения
    /// </summary>
    [Serializable]
    public class HoldingRegister : Parameter<UInt16>
    {
        #region Fields and Properties
        public override ModbusParameterType ParameterType
        {
            get { return ModbusParameterType.HoldingRegister; }
        }
        #endregion

        #region Constructors and Destructor
        /// <summary>
        /// Конструктор.
        /// </summary>
        public HoldingRegister()
        {
            Value = 0;
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="address">Адрес регистра ввода\вывода</param>
        /// <param name="value">Значение регистра ввода\вывода</param>
        /// <param name="description">Описание регистра ввода\вывода</param>
        public HoldingRegister(UInt16 address, UInt16 value, String description)
            : base(address, value, description)
        {
        }
        #endregion        

        #region Methods
        #endregion

        #region Events
        #endregion
    }
}
