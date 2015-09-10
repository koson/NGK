using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.OSIModel.ApplicationLayer.DB.Network.Services
{
    /// <summary>
    /// —труктура дл€ хранени€ сообщений об ошибках в контекте работы 
    /// сетевых сервисов
    /// </summary>
    public struct ErrorMessageContext
    {
        public DateTime DateTime;
        public String Message;
    }
}
