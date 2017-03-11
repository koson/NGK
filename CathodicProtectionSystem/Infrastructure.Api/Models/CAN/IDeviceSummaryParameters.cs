using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Infrastructure.Api.Models.CAN
{
    public interface IDeviceSummaryParameters: INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="?"></param>
        [DisplayName("Адрес")]
        [Browsable(false)]
        byte NodeId { get; }
        /// <summary>
        /// 
        /// </summary>
        [DisplayName("Позиция на объекте")]
        [Description("Место расположение")]
        [Browsable(true)]
        string Location { get; }
        /// <summary>
        /// Поляризационный потенциал, В (0x2008)
        /// null - если измерение данного параметра отключено
        /// </summary>
        [DisplayName("\"БОС\", B")]
        [Description("Поляризационный потенциал, B")]
        [Browsable(true)]
        float? PolarisationPotential { get; }
        /// <summary>
        /// Ток поляризации, mA (0x200С)
        /// null - если измерение данного параметра отключено
        /// </summary>
        [DisplayName("Ток поляризации, mA")]
        [Browsable(false)]
        float? PolarisationCurrent { get; }
        /// <summary>
        /// Защитный потенциал, В (0x2009)
        /// null - если измерение данного параметра отключено
        /// </summary>
        [DisplayName("\"СОС\", B")]
        [Description("Защитный потенциал, B")]
        [Browsable(true)]
        float? ProtectionPotential { get; }
        /// <summary>
        /// Ток катодной защиты, А (0x200B)
        /// null - если измерение данного параметра отключено
        /// </summary>
        [DisplayName("Ток катодной защиты, A")]
        [Browsable(true)]
        float? ProtectionCurrent { get; }
        /// <summary>
        /// Глубина коррозии (0x200F)
        /// </summary>
        [DisplayName("Глубина коррозии, мкм")]
        [Browsable(true)]
        UInt32? CorrosionDepth { get;}
        /// <summary>
        /// Скорость коррозии (0x2010)
        /// </summary>
        [DisplayName("Скорость коррозии, мкм/год")]
        [Browsable(true)]
        UInt32? CorrosionSpeed { get; }
        /// <summary>
        /// Вскрытие корпуса прибора
        /// </summary>
        [DisplayName("Вскрытие")]
        [Browsable(true)]
        Boolean Tamper { get; }
    }
}
