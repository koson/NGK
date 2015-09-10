using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataLinkLayer.Message;

namespace NGK.CAN.DataLinkLayer.CanPort
{
    /// <summary>
    /// Для хранения и передачи настроечных парметров порта
    /// </summary>
    public struct PortSettings
    {
        /// <summary>
        /// Скорость обмена данными
        /// </summary>
        public BaudRate BitRate;
        /// <summary>
        /// Формат кадров (поле ID: 11 или 29 бит) с которыми работает порт
        /// могут складыватся по "ИЛИ"
        /// </summary>
        public FrameFormat FrameFormat;
        /// <summary>
        /// Разрешить режим монитора сети: Tx passive
        /// </summary>
        public Boolean ListenOnlyMode;
        /// <summary>
        /// Разрешить приём сообщения Error Frame
        /// </summary>
        //public Boolean ErrorFrameEnable = true;
        /// <summary>
        /// Разрешить низкоскоростной режим работы CAN-интерфейса
        /// ????????
        /// </summary>
        //public Boolean LowSpeedModeEnable = false;
        public struct DefaultSettings
        { 
            public const BaudRate BitRateDefault = BaudRate.BR10;
            public const FrameFormat FrameFormatDefault = FrameFormat.MixedFrame;
            public const Boolean ListenOlnyModeDefault = false;
        }
    }
}
