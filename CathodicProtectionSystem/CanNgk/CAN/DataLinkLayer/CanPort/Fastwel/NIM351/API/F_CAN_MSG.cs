using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    /// <summary>
    /// CAN-сообщение
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct F_CAN_MSG
    {
        /// <summary>
        /// CAN-ID, включая флаги EFF/RTR/ERR
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 can_id;
        /// <summary>
        /// Кол-во данных в кадре (от 0 до 8)
        /// </summary> 
        [MarshalAs(UnmanagedType.U1)]
        public Byte can_dlc;
        ///<summary>
        ////Поле данных кадра (длина должна быть 8),
        /// !!!Для иницилизаци этого поля нужно вызвать Init();
        ///</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Byte[] data;
        ///// <summary>
        ///// Иницализирует поле data;
        ///// </summary>
        //public void Init()
        //{
        //    can_id = 0;
        //    can_dlc = 0;
        //    this.data = new Byte[8];
        //}
    }
}
