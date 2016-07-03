using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NGK.CAN.DataTypes.TypeConverters;

namespace NGK.CAN.DataTypes
{
    /// <summary>
    /// Предназначен для представления чисел типа UInt16
    /// для отображения на GUI
    /// </summary>
    [TypeConverter(typeof(UInt32WithStatusDisabledConverter))]
    public struct UInt32WithStatusDisabled
    {
        public UInt32WithStatusDisabled(UInt32 value)
        {
            _Value = value;
        }

        private UInt32 _Value;

        public UInt32 Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public override string ToString()
        {
            UInt32WithStatusDisabledConverter converter = new UInt32WithStatusDisabledConverter();
            return (string)converter.ConvertTo(this, typeof(string));
        }
    }
}
