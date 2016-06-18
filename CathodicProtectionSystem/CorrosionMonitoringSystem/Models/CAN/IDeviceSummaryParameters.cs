using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NGK.CorrosionMonitoringSystem.Models
{
    public interface IDeviceSummaryParameters
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="?"></param>
        [DisplayName("Адрес")]
        byte NodeId { get; }
        /// <summary>
        /// 
        /// </summary>
        [DisplayName("Месторасположение")]
        string Location { get; }
        /// <summary>
        /// Поляризационный потенциал, В (0x2008)
        /// null - если измерение данного параметра отключено
        /// </summary>
        [DisplayName("Поляризационный потенциал, B")]
        float? PolarisationPotential { get; }
        /// <summary>
        /// Ток поляризации, mA (0x200С)
        /// null - если измерение данного параметра отключено
        /// </summary>
        [DisplayName("Ток поляризации, mA")]
        float? PolarisationCurrent { get; }
        /// <summary>
        /// Защитный потенциал, В (0x2009)
        /// null - если измерение данного параметра отключено
        /// </summary>
        [DisplayName("Защитный потенциал, B")]
        float? ProtectionPotential { get; }
        /// <summary>
        /// Ток катодной защиты, А (0x200B)
        /// null - если измерение данного параметра отключено
        /// </summary>
        [DisplayName("Ток катодной защиты, A")]
        float? ProtectionCurrent { get; }
    }
}
