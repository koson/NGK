using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataTypes;
using NGK.CAN.DataTypes.Helper;

namespace Test.DataTypes
{
    public class DataTypesTest
    {
        public static void TestDataTypeInfo_GetTotalValue()
        {
            NgkUInt16Convertor info = new NgkUInt16Convertor(ScalerTypes.x005);
            decimal value = (Decimal)info.ConvertToOutputValue(4);

            Console.WriteLine("Результат: {0}", value);

            info = new NgkUInt16Convertor((1M / 3M));
            value = (Decimal)info.ConvertToOutputValue(4);

            Console.WriteLine("Результат: {0}", value);

            info = new NgkUInt16Convertor(0.3333333M);
            value = (decimal)info.ConvertToOutputValue(4);

            Console.WriteLine("Результат: {0}", value);
        }

        public static void TestDataTypeInfo_SetValue()
        {
            decimal value;

            NgkUInt16Convertor info = new NgkUInt16Convertor(ScalerTypes.x005);

            value = (Decimal)info.ConvertToOutputValue(2000);
            Console.WriteLine("Результат: {0}", value);

            //value = DataTypeInfo.SetValue((int)100, info);

            Console.WriteLine("Результат: {0}", value);

            // Отрицательные числа
            try
            {
                //value = DataTypeInfo.SetValue((int)-100, info);
                Console.WriteLine("Результат: {0}", value);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: {0}", ex.Message);
            }
        }
    }
}
