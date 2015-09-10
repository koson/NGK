using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Modbus.OSIModel.DataLinkLayer
{
    //=================================================
    /// <summary>
    /// Класс для расчёта и хранения CRC16
    /// </summary>
    public class CRC16
    { 
        //---------------------------------------------
        /// <summary>
        /// Метод рассчитывает контрольную 
        /// сумму CRC16
        /// </summary>
        /// <param name="DataArray">массив байт для рассчёта контрольной суммы</param>
        /// <returns>CRC16: [0]-младший байт, [1]-старший байт</returns>
        public static byte[] CalcCRC16(byte[] DataArray)
        {
            byte[] crc = new byte[2];

            byte[] _array = DataArray;

            byte _high = 0;
            byte _low = 0;

            UInt16 crct = 0xFFFF;

            for (int i = 0; i < _array.Length; i++)
            {
                crct = (UInt16)(crct ^ System.Convert.ToUInt16(_array[i]));

                for (int n = 0; n <= 7; n++)
                {
                    if ((crct & 0x0001) == 0x0001)
                    {
                        crct = (UInt16)(crct >> 1);
                        crct = (UInt16)(crct ^ 0xA001);
                    }
                    else
                    {
                        crct = (UInt16)(crct >> 1);
                    }
                }
            }

            //crc.HEIGHTBYTE = Convert.ToByte(crct >> 8);
            _high = System.Convert.ToByte(crct >> 8);

            unchecked
            {
                _low = (Byte)(crct);
            }

            Debug.Print(_low.ToString());
            Debug.Print(_high.ToString());

            crc[0] = _low;
            crc[1] = _high;
                
            return crc;
        }
        //---------------------------------------------
        /// <summary>
        /// Метод проверяет контрольную сумму CRC16 в 
        /// modbus-сообщении
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <returns>возращает true если контрольня сумма верная</returns>
        public static bool VerefyCRC16(byte[] message)
        {
            byte[] array = new byte[message.Length - 2];
            
            Array.Copy(message, array, (message.Length - 2));
            
            byte[] crc = CalcCRC16(array);
            
            if ((crc[0] == message[(message.Length - 2)])
                && (crc[1] == message[(message.Length - 1)]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //---------------------------------------------
    }
    //=================================================
}
//=====================================================
// End of file 