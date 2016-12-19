using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Enumeration
{
    public static class EnumConverter<TEnum, TValue>
        where TEnum : struct
        where TValue : struct
    {
        #region Constructor

        static EnumConverter()
        {
            EnumType = typeof(TEnum);

            if (!EnumType.IsEnum)
                throw new ArgumentException("ѕараметр-тип <TEnum> EnumConverter не €вл€етс€ перечислимым типом");

            var field = EnumType.GetField("value__");

            var valueType = typeof(TValue);

            if (field.FieldType != valueType)
                throw new ArgumentException("ѕараметр-тип <TValue> не соответствует типу значений перечислени€");
        }

        #endregion

        #region Fields And Properties

        private static Type _EnumType;

        public static Type EnumType
        {
            get { return _EnumType; }
            private set { _EnumType = value; }
        }

        #endregion

        public static TEnum ConvertFrom(TValue value)
        {
            if (Enum.IsDefined(EnumType, value))
            {
                return (TEnum)Enum.ToObject(EnumType, value);
            }
            else
            {
                throw new InvalidCastException();
            }
        }

        public static TValue ConvertTo(TEnum enumMember)
        {
            return (TValue)(Object)enumMember;
        }

        public static IEnumerable<TValue> GetValues()
        {
            List<TValue> result = new List<TValue>();

            var array = Enum.GetValues(EnumType);

            foreach (object item in array)
                result.Add((TValue)item);

            return result;
        }
    }
}
