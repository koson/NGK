using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //===========================================================================================
    /// <summary>
    /// Входящее (принимаемое) сообщение
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct F_CAN_RX
    {
        ///<summary>
        /// Метка времени сообщения в микросекундах.
        /// Время измеряется с момента открытия адаптера и переполняется 
        /// через интервал равный примерно 71 минуте.
        ///</summary>
        public UInt32 timestamp;
        ///<summary>
        ///"Базовый класс" (само сообщение)
        /// </summary>
        public F_CAN_MSG msg;
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("TimeStamp: {0};", this.timestamp);
            sb.Append(" ");
            sb.AppendFormat("Message: ID={0}; DLC={1};", this.msg.can_id, this.msg.can_dlc);
            sb.Append("DATA: ");
            if (this.msg.data != null)
            {
                foreach (byte vByte in this.msg.data)
                {
                    sb.AppendFormat("{0:X2}h", vByte);
                }
            }
            return sb.ToString();
            //return base.ToString();
        }
    }
    //===========================================================================================
}
