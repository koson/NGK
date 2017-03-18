using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary.Collections;
using NGK.CAN.ApplicationLayer.Network.Master;

namespace NGK.CAN.ApplicationLayer.Network.Devices
{
    /// <summary>
    /// Интерфейс реализует функционал для работы с устройством со стороны
    /// пользователя (отображение параметров на форме и т.п.).
    /// </summary>
    public interface IDevice
    {
        #region Propeties

        /// <summary>
        /// Сетевой идентификатор устройства 1...127
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Сетевые настройки")]
        [DisplayName("Адрес")]
        [Description("Сетевой идентификатор устройства")]
        //[DefaultValue(typeof(UInt16),"0")]
        //[RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        Byte NodeId
        {
            get;
            set;
        }

        /// <summary>
        /// Словарь объектов устройства
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Устройство")]
        [DisplayName("Словарь объектов")]
        [Description("Словарь объектов устройства")]
        ObjectCollection ObjectDictionary 
        {
            get;
        }

        /// <summary>
        /// Тип устройства
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Устройство")]
        [DisplayName("Тип устройства")]
        [Description("Тип устройства для сети CAN НГК-ЭХЗ")]
        DeviceType DeviceType
        {
            get;
        }

        /// <summary>
        /// Серийный номер устройства
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Устройство")]
        [DisplayName("Серийный номер")]
        [Description("Серийный номер устройства")]
        UInt64 SerialNumber
        {
            set;
            get;
        }

        /// <summary>
        /// Возвращает визитную карточку устройства НГК
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Устройство")]
        [DisplayName("Визитная карточка")]
        [Description("Основные идентификационные данные устройства")]
        //[TypeConverter(typeof(VisitingCardTypeConverter))]
        VisitingCard VisitingCard
        {
            get;
        }

        /// <summary>
        /// Статус устройства
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Устройство")]
        [DisplayName("Статус")]
        [Description("Текущее состояние устройства")]
        DeviceStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// Возвращает или устанавливает сеть (контроллер сети), которой пренадлежит данное устройство
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Сетевые настройки")]
        [DisplayName("Сеть CAN НГК-ЭХЗ")]
        [Description("Сеть, которой принадлежит данное устройство")]
        ICanNetworkController Network
        {
            get;
            set; 
        }

        /// <summary>
        /// Наименование географической точки установки оборудования  
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Сетевые настройки")]
        [DisplayName("Месторасположение")]
        [Description("Наименование географического места расположения КИП")]
        //[DefaultValue(typeof(UInt16),"0")]
        //[RefreshProperties(System.ComponentModel.RefreshProperties.All)] 
        String LocationName
        {
            get;
            set; 
        }

        /// <summary>
        /// Возвращает время (сек) опроса устройства (равно времени 
        /// измерения и передачи данных у БИ(У))
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Интервал опроса, с")]
        [Description("Период опроса устройства")]
        UInt32 PollingInterval
        {
            get;
            set;
        }

        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Настройки")]
        [DisplayName("Площадь электрода, кв.мм")]
        [Description("Площадь вспомогательного электрода, кв.мм")]
        UInt16 ElectrodeArea
        {
            get;
            set;
        }

        /// <summary>
        /// Профиль устройства
        /// </summary>
        ICanDeviceProfile Profile{ get; }

        /// <summary>
        /// Уникальный идентификатор объекта
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Устройство")]
        [DisplayName("GUID")]
        [Description("Уникальный идентификатор объекта")]
        Guid Id { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Возвращает значение объекта словаря с указанным индексом
        /// </summary>
        /// <param name="address">Индекс объекта</param>
        /// <returns>Значение объекта</returns>
        ValueType GetObject(UInt16 index);

        #endregion

        #region Events

        /// <summary>
        /// Устройство изменило своё состояние
        /// </summary>
        event EventHandler DeviceChangedStatus;
        
        /// <summary>
        /// Событие происходит при изменении параметров объектного словаря
        /// или других параметров устройтсва
        /// </summary>
        event EventHandler DataWasChanged;
        
        #endregion
    }
}
