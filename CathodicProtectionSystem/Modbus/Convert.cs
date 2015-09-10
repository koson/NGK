using System;
using System.Collections.Generic;
using System.Text;

//==========================================================================================
namespace Modbus
{
    //======================================================================================
    public static class Convert
    {
        //----------------------------------------------------------------------------------
        /// <summary>
        /// Метод преобразует 2-х байтное число в массив 
        /// байт
        /// </summary>
        /// <param name="Data">2-х байтное число</param>
        /// <returns>[0]-старший байт числа, [1]-младший байт числа</returns>
        public static byte[] ConvertToBytes(UInt16 Data)
        {
            byte[] result = new byte[2];
            result[0] = System.Convert.ToByte(Data >> 8);    // старший байт
            unchecked
            {
                result[1] = (byte)(Data);       // младший байт
            }
            return result;
        }
        //----------------------------------------------------------------------------------
        /// <summary>
        /// Метод преобразует массив из двух байт в число UInt16
        /// </summary>
        /// <param name="array">Массив из двух байт (0-ой байт 
        /// массива содержит старший байт числа, 1-ый байт массива
        /// содержит младший байт числа)</param>
        /// <returns>Число UInt16</returns>
        public static UInt16 ConvertToUInt16(Byte[] array)
        {
            if (array.Length != 2)
            {
                throw new ArgumentException("Количество элементов массива не равно 2", "array");
            }
            else
            {
                int value = 0;
                value = array[0];
                value = value << 8;
                value = value | array[1];
                return System.Convert.ToUInt16(value);
            }
        }
        //----------------------------------------------------------------------------------
        /// <summary>
        /// Возвращает массив байт (2 байта) числового представления
        /// состояния реле (On = 0x00FF; Off = 0x0000)
        /// </summary>
        /// <returns></returns>
        public static byte[] StateToArray(State state)
        {     
            switch (state)
            {
                case State.Off: 
                    {
                        return new Byte[2] { 0x00, 0x00 };
                    }
                case State.On:
                    {
                        return new Byte[2] { 0xFF, 0x00 };
                    }
                default:
                    {
                        throw new ArgumentException(
                            String.Format("Неопознаное значение State: {0}", state), "state");
                    }
            }
        }
        //----------------------------------------------------------------------------------
        /// <summary>
        /// Возвращает числовое представление состояние
        /// реле (On = 0x00FF; Off = 0x0000)
        /// </summary>
        /// <returns></returns>
        public static UInt16 StateToValue(State state)
        {
            switch (state)
            {
                case State.Off: 
                    {
                        return 0x0000;
                    }
                case State.On:
                    {
                        return 0x00FF;
                    }
                default:
                    {
                        throw new ArgumentException(
                            String.Format("Неопознаное значение State: {0}", state), "state");
                    }
            }
        }
        //----------------------------------------------------------------------------------
        /// <summary>
        /// Возвращает состояние из числового представления переданного
        /// через массив байт (2 байта)
        /// </summary>
        /// <param name="value">Массив байт хранящих 
        /// представление состояния дискретного сигнала</param>
        /// <returns>Состояние дискретного сигнала</returns>
        public static State ValueToState(Byte[] value)
        {
            if (value.Length != 2)
            {
                throw new ArgumentException(
                    "Массив переданный в качестве значения, содержит элемнты в количестве не равное двум",
                    "value");
            }
            else
            {
                if (value[1] != 0x00)
                {
                    throw new ArgumentException("Младший байт значения не равен 0",
                        "value");
                }
                else
                {
                    switch (value[0])
                    {
                        case 0xFF:
                            { return State.On; }
                        case 0x00:
                            { return State.Off; }
                        default:
                            {
                                throw new ArgumentException(
                                    "Старший байт значения не равен 0x00 или 0xFF",
                                    "value");
                            }
                    }
                }
            }
        }
        //----------------------------------------------------------------------------------
        /// <summary>
        /// Возвращает представление состояния дискретного сигнала
        /// в виде "On" = 1 и "Off" = 0 
        /// </summary>
        /// <param name="status">Состояние дискретного сигнала</param>
        /// <returns>Числовое представление</returns>
        public static int StatusToBit(State status)
        {
            switch (status)
            {
                case State.Off:
                    {
                        return 0;
                    }
                case State.On:
                    {
                        return 1;
                    }
                default:
                    {
                        throw new ArgumentException(String.Format(
                            "Неопределено числовое значение для аргумента {0}", status), "status");
                    }
            }
        }
        //----------------------------------------------------------------------------------
        /// <summary>
        /// Преобразует тип Boolean в число типа int
        /// (true = 1, false = 0);
        /// </summary>
        /// <param name="state">Исходное значение</param>
        /// <returns>Результат преобразования</returns>
        public static int BooleanToBit(Boolean state)
        {
            if (state == true)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        //----------------------------------------------------------------------------------
        /// <summary>
        /// Преобразует тип State к типу Boolean
        /// </summary>
        /// <param name="value">Значение</param>
        /// <returns>Результат преобразования</returns>
        public static Boolean ToBoolean(State value)
        {
            switch (value)
            {
                case State.Off:
                    { return false; }
                case State.On:
                    { return true; }
                default:
                    { throw new NotImplementedException(); }
            }
        }
        //----------------------------------------------------------------------------------
        /// <summary>
        /// Преобразует тип Bolean в тип State
        /// </summary>
        /// <param name="value">Значение</param>
        /// <returns>Результат</returns>
        public static State ToState(Boolean value)
        {
            switch (value)
            {
                case false:
                    { return State.Off; }
                case true:
                    { return State.On; }
                default:
                    { throw new NotImplementedException(); }
            }
        }
        //----------------------------------------------------------------------------------
    }
    //======================================================================================
}
//==========================================================================================