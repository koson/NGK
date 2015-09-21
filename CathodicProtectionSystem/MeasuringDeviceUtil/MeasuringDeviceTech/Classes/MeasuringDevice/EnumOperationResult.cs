﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

//====================================================================================
namespace NGK.MeasuringDeviceTech.Classes.MeasuringDevice
{
    //================================================================================
    /// <summary>
    /// Определяет коды ошибок при выполнении операций с устройствами НГК
    /// </summary>
    public enum OPERATION_RESULT
    {
        //----------------------------------------------------------------------------
        /// <summary>
        /// Операция выполнена успешно
        /// </summary>
        [Description("Операция выполнена успешно")]
        OK = 0,
        //----------------------------------------------------------------------------
        /// <summary>
        /// При выполении операции произошла какая-то ошибка
        /// (Общий и неопределённый вид сбоя выполнения операции)
        /// </summary>
        [Description("При выполении операции произошла какая-то ошибка (Общий и неопределённый вид сбоя выполнения операции)")]
        FAILURE = 1,
        //----------------------------------------------------------------------------
        /// <summary>
        /// Устройство не поддерживает данную операцию 
        /// (операция недопустима для данного устройства НГК)
        /// </summary>
        [Description("Устройство не поддерживает данную операцию (операция не допустима для данного устройства НГК)")]
        INVALID_OPERATION = 2,
        //----------------------------------------------------------------------------
        /// <summary>
        /// Ошибка функционирования сети
        /// </summary>
        [Description("Ошибка функционирования сети")]
        CONNECTION_ERROR = 3,
        //----------------------------------------------------------------------------
        /// <summary>
        /// Устройство вернуло ошибку выполнения операции
        /// </summary>
        [Description("Устройство вернуло ошибку выполнения операции")]
        DEVICE_ERROR = 4,
        //----------------------------------------------------------------------------
        /// <summary>
        /// Неизвестное устройство (тип устройства не опознан) 
        /// </summary>
        [Description("Неизвестное устройство (тип устройства не опознан)")]
        UNKNOWN_DEVICE = 5,
        //----------------------------------------------------------------------------
        /// <summary>
        /// Некорректный ответ устройства на запрос
        /// </summary>
        [Description("Некорректный ответ устройства на запрос")]
        INCORRECT_ANSWER = 6,
        //----------------------------------------------------------------------------
        /// <summary>
        /// Код 2: Адрес данных указанный в 
        /// запросе не доступен данному подчиненному.
        /// (Возвращается в modbus-ответе)
        /// </summary>
        [Description("Адрес данных указанный в запросе не доступен")]
        IllegalDataAddress = 7,
        //----------------------------------------------------------------------------
    }
    //================================================================================
}
//====================================================================================
// End of file