using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CorrosionMonitoringSystem.Models
{
    /// <summary>
    /// Модель устройства
    /// </summary>
    public class Device
    {
        Guid _Id;

        public Guid Id { get { return _Id; } }
    }
}
