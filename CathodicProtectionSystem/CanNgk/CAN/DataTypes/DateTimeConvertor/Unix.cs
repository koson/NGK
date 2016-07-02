using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataTypes.DateTimeConvertor
{
    /// <summary>
    /// Класс реализует преобразования между типами используемых в протоколе CAN НГК-ЭХЗ
    /// и типами CRL 
    /// </summary>
    public static class Unix
    {
        /// <summary>
        /// Преобразует структура DateTime в формат времени UNIX-систем
        /// (Количество секунд от 01/01/1970 00:00:00)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static UInt32 ToUnixTime(DateTime dateTime)
        {
            DateTime unixStartTime;
            TimeSpan ts;

            unixStartTime = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

            //ts = (dateTime.ToUniversalTime()).Subtract(unixStartTime);
            ts = dateTime.Subtract(unixStartTime);

            if (ts.TotalSeconds >= 0)
                return System.Convert.ToUInt32(ts.TotalSeconds);
            else
                return System.Convert.ToUInt32(unixStartTime.TimeOfDay.TotalSeconds);
        }
        /// <summary>
        /// Преобразует данные в формате хранения времени UNIX-систем
        /// в структуру DateTime
        /// </summary>
        /// <param name="unixTimeFormat">Количество секунд от 01/01/1970 00:00:00</param>
        /// <returns>DateTime</returns>
        public static DateTime ToDateTime(UInt32 unixTimeFormat)
        {
            DateTime unixStartTime;

            unixStartTime =
                new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

            return unixStartTime.AddSeconds(unixTimeFormat);
        }
    }
}
