using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataTypes.DateTimeConvertor
{
    /// <summary>
    /// ����� ��������� �������������� ����� ������ ������������ � ��������� CAN ���-���
    /// � ������ CRL 
    /// </summary>
    public static class Unix
    {
        /// <summary>
        /// ����������� ��������� DateTime � ������ ������� UNIX-������
        /// (���������� ������ �� 01/01/1970 00:00:00)
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
        /// ����������� ������ � ������� �������� ������� UNIX-������
        /// � ��������� DateTime
        /// </summary>
        /// <param name="unixTimeFormat">���������� ������ �� 01/01/1970 00:00:00</param>
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
