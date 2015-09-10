using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.ApplicationLayer.Network.Master.Services
{
    /// <summary>
    /// Типы сетевых сервисов
    /// </summary>
    public enum ServiceType
    {
        BootUp,
        PdoTransmit,
        PdoReceive,
        Sync,
        NodeGuard,
        SdoUpload,
        Nmt,
        Emcy
    }
}
