using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NGK.CAN.DataTypes.TypeConverters;

namespace NGK.CAN.DataTypes
{
    /// <summary>
    /// Предназначен для представления чисел типа UInt16 имеющим одно значение в качестве "Оключено"
    /// (для отображения на GUI)
    /// </summary>
    [TypeConverter(typeof(UInt32WithStatusDisabledConverter))]
    public struct UInt32WithStatusDisabled
    {
        #region Constructor

        public UInt32WithStatusDisabled(UInt32 value)
        {
            _Value = value;
        }

        #endregion

        #region Fields And Properties

        public const UInt16 DisabledCode = 0xFFFF;  

        private UInt32 _Value;

        public UInt32 Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public bool IsEnabled
        {
            get { return Value == DisabledCode * 10; }
        }

        #endregion 

        #region Methods

        public override string ToString()
        {
            return IsEnabled ? "Откл." : Value.ToString();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is UInt32WithStatusDisabled)
            {
                return this == (UInt32WithStatusDisabled)obj;
            }
            else
                return base.Equals(obj);
        }

        #endregion 

        #region Operators

        public static bool operator ==(UInt32WithStatusDisabled operand1, UInt32WithStatusDisabled operand2)
        {
            return operand1._Value == operand2._Value;
        }

        public static bool operator !=(UInt32WithStatusDisabled operand1, UInt32WithStatusDisabled operand2)
        {
            return operand1._Value != operand2._Value;
        }

        #endregion
    }
}
