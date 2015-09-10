using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using NGK.CAN.ApplicationLayer.Network.Master.Services;
using NGK.CAN.DataLinkLayer.CanPort;
using NGK.CAN.DataLinkLayer.Message;
//using NGK.CAN.ApplicationLayer.Network.Master.Design;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.Collections;
using NGK.CAN.ApplicationLayer.Network.Master.Services.Collections;
using Common.Controlling;
using NGK.CAN.Design;

namespace NGK.CAN.ApplicationLayer.Network.Master
{
    /// <summary>
    /// Интерфейс для контроллера сети
    /// </summary>
    [TypeConverter(typeof(INetworkControllerTypeConverter))]
    public interface INetworkController : IManageable, IDisposable
    {
        [DisplayName("Сеть CAN НГК-ЭХЗ")]
        [Description("Идентификатор сети")]
        [ReadOnly(false)]
        [Browsable(true)]
        [Category("Сетевые настройки")]
        UInt32 NetworkId
        {
            get;
        }
        /// <summary>
        /// Описание сети
        /// </summary>
        /// <returns>Наименование сети</returns>
        [DisplayName("Сеть CAN НГК-ЭХЗ")]
        [Description("Наименование сети")]
        [ReadOnly(false)]
        [Browsable(true)]
        [Category("Сетевые настройки")]
        String Description
        {
            get;
        }
        //[DisplayName("Статус")]
        //[Description("Состояние сетевого контроллера")]
        //[ReadOnly(false)]
        //[Browsable(true)]
        //[Category("Сетевые настройки")]
        ////[TypeConverter(typeof(EnumConverter))]
        //Status Status
        //{
        //    get;
        //    set;
        //}
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Количество попыток доступа к устройству")]
        [Description("Количество неудачных попыток доступа к устройству, после чего устройство переходит в состояние неисправности.")]
        Int32 TotalAttempts
        {
            get;
            set;
        }
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Период сигнала SYNC, мсек")]
        [Description("Период генерации сообщения SYNC в сеть")]
        Double PeriodSync
        {
            get;
            set;
        }
        /// <summary>
        /// Возвращает массив сетевых устройств определённых в конфигурации сети. 
        /// </summary>
        /// <returns></returns>
        [DisplayName("Устройства")]
        [Description("Список сетевых устройств")]
        [Category("Настройки")]
        [ReadOnly(false)]
        [Browsable(true)]
        DevicesCollection Devices
        {
            get;
        }
        /// <summary>
        /// Возвращает массив сетевых сервисов котроллера сети
        /// </summary>
        /// <returns></returns>
        [DisplayName("Сетевые сервисы")]
        [Description("Сетевые сервисы доступные в сетевом контроллере")]
        [ReadOnly(true)]
        [Browsable(true)]
        [Category("Сетевые настройки")]
        NetworkServicesCollection Services
        {
            get;
        }
        /// <summary>
        /// Возвращает CAN-порт через который работает данная сеть
        /// </summary>
        /// <returns></returns>
        [DisplayName("CAN порт")]
        [Description("CAN порт для работы с сетью CAN")]
        [ReadOnly(false)]
        [Browsable(true)]
        [Category("Сетевые настройки")]
        ICanPort CanPort
        {
            get;
        }
        /// <summary>
        /// Возвращает время последней синхронизации сетевого времени
        /// (сервис ServicePdoReceive)
        /// </summary>
        [DisplayName("Последняя синхр. времени")]
        [Description("Возвращает время последней синхронизации сетевого времени")]
        [ReadOnly(true)]
        [Browsable(true)]
        [Category("Сетевые настройки")]
        DateTime SynchronisationLastTime
        {
            get;
        }
        /// <summary>
        /// Возвращает/устанавливает прериод времени (сек.) через
        /// корорый сервис ServicePdoReceive синхронизирует сетевое время)
        /// </summary>
        [DisplayName("Интервал синхр. времени")]
        [Description("Интервал времени (сек.) через корорый сервис ServicePdoReceive " +
            "синхронизирует сетевое время")]
        [ReadOnly(true)]
        [Browsable(true)]
        [Category("Сетевые настройки")]
        Int32 SynchronisationInterval
        {
            get;
            set;
        }
        /// <summary>
        /// Возвращает количество устройств в сети с указанным статусом 
        /// </summary>
        /// <param name="status">Статус устройства</param>
        /// <returns>Количество устройств с указанным статусом</returns>
        Int32 GetCountOfDevicesByStatus(DeviceStatus status);
        /// <summary>
        /// Отправляет сообещние в сеть
        /// </summary>
        /// <param name="outcommingMessage"></param>
        void SendMessageToCanPort(Frame message);
    }
}
