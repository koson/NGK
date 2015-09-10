using System;
using System.Collections.Generic;
using System.Text;

//===========================================================================================
namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //=======================================================================================
    /// <summary>
    /// Результат выполнения функций драйвера СAN
    /// </summary>
    [Serializable]
    public enum F_CAN_RESULT : int
    {
        //===================================================================================
        /// <summary>
        /// успех
        /// </summary>
        CAN_RES_OK = 0,
        /// <summary>
        /// Аппаратная ошибка
        /// </summary>
        CAN_RES_HARDWARE = 1,
        /// <summary>
        /// Недействительный (неправильный) системный идентификатор (хэндл) адаптера
        /// </summary>
        CAN_RES_INVALID_HANDLE = 2,
        /// <summary>
        /// Некорректный указатель
        /// </summary>
        CAN_RES_INVALID_POINTER = 3,
        /// <summary>
        /// Неправильный параметр (один или несколько)
        /// </summary>
        CAN_RES_INVALID_PARAMETER = 4,
        /// <summary>
        /// Не хватило системных ресурсов
        /// </summary>
        CAN_RES_INSUFFICIENT_RESOURCES = 5,
        /// <summary>
        /// Не удалось открыть устройство
        /// </summary>
        CAN_RES_OPEN_DEVICE = 6,
        /// <summary>
        /// Внутренняя ошибка в драйвере устройства
        /// </summary>
        CAN_RES_UNEXPECTED = 7,
        /// <summary>
        /// Ошибка обращения к драйверу или к устройству
        /// </summary>
        CAN_RES_FAILURE = 8,
        /// <summary>
        /// Буфер приема пуст
        /// </summary>
        CAN_RES_RXQUEUE_EMPTY = 9,
        /// <summary>
        /// Операция не поддерживается
        /// </summary>
        CAN_RES_NOT_SUPPORTED = 10,
        /// <summary>
        /// Таймаут
        /// </summary>
        CAN_RES_TIMEOUT = 11
    }
    //=======================================================================================
}
//===========================================================================================
