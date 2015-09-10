using System;

namespace Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes
{
    /// <summary>
    /// Класс для создания входного регистра
    /// </summary>
    [Serializable]
    public class InputRegister : Parameter<UInt16>
    {
        #region Fields and Properties
        /// <summary>
        /// Тип данных модели данных modbus-устройства
        /// </summary>
        public override ModbusParameterType ParameterType
        {
            get { return ModbusParameterType.InputRegister; }
        }
        #endregion

        #region Constructors and Destructor
        /// <summary>
        /// Конструктор.
        /// </summary>
        public InputRegister()
        {
            this.Value = 0;
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="address">Адрес входного регистра</param>
        /// <param name="value">Значение входного регистра</param>
        /// <param name="description">Описание входного регистра</param>
        public InputRegister(UInt16 address, UInt16 value, String description)
            : base(address, value, description)
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
