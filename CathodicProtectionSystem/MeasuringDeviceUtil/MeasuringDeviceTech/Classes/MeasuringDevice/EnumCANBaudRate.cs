﻿using System;
using System.Text;

//====================================================================================
namespace NGK.MeasuringDeviceTech.Classes.MeasuringDevice
{
    //================================================================================
    /// <summary>
    /// Определяет скорости передачи данных по интерфейсу CAN
    /// </summary>
    [Serializable]
    public enum CANBaudRate: ushort
    {
        //----------------------------------------------------------------------------
        /// <summary>
        /// 10 кБит/с
        /// </summary>
        BR10K = 10,
        //----------------------------------------------------------------------------
        /// <summary>
        /// 20 кБит/с
        /// </summary>
        BR20K = 20,
        //----------------------------------------------------------------------------
        /// <summary>
        /// 50 кБит/с
        /// </summary>
        BR50K = 50,
        //----------------------------------------------------------------------------
        /// <summary>
        /// 100 кБит/с
        /// </summary>
        BR100K = 100
        //----------------------------------------------------------------------------
    }
    //================================================================================
}
//====================================================================================