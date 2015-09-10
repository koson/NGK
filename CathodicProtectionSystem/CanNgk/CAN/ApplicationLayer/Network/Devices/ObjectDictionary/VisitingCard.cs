using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using NGK.CAN.DataTypes;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary.ComponentModel;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles;
using Common.ComponentModel;

namespace NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary
{
    /// <summary>
    /// Визитная карточка устройства НГК-ЭХЗ
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(VisitingCardTypeConverter))]
    public struct VisitingCard
    {
        #region Helper
        /// <summary>
        /// Индексы объектов входящих в визитную карточку устройства
        /// </summary>
        public struct Indexes
        {

            public const UInt16 SoftwareVersion = 0x2001;
            public const UInt16 HardwareVersion = 0x2002;
            public const UInt16 SerialNumberHigh = 0x2005;
            public const UInt16 SerialNumberMiddle = 0x2004;
            public const UInt16 SerialNumberLow = 0x2003;
            public const UInt16 CRC16 = 0x2006;

        }
        #endregion

        #region Fields And Properties
        /// <summary>
        /// Тип устройства NGK
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Системные данные")]
        [Description("Вариант исполнения устройства БИ в составе КИП")]
        [DisplayName(@"Тип устройства")]
        public DeviceType DeviceType
        {
            get { return _Device.DeviceType; }
        }
        /// <summary>
        /// Версия ПО
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Системные данные")]
        [Description("Версия ПО")]
        [DisplayName("Версия ПО")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Version SoftwareVersion
        {
            get
            {
                //String msg;

                return ((ProductVersion)_Device.GetObject(
                    Indexes.SoftwareVersion)).Version;
            }
        }
        /// <summary>
        /// Версия аппаратуры
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Системные данные")]
        [Description("Версия аппаратуры")]
        [DisplayName("Версия аппаратуры")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public Version HardwareVersion
        {
            get
            {
                //String msg;
                return ((ProductVersion)_Device.GetObject(
                    Indexes.HardwareVersion)).Version;                
            }
        }
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Системные данные")]
        [Description("Серийный номер устройства")]
        [DisplayName("Серийный номер")]
        [DefaultValue(typeof(UInt64), "0xFFFFFFFFFF")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public UInt64 SerialNumber
        {
            get
            {
                return _Device.SerialNumber;
            }
            set
            {
                _Device.SerialNumber = value;
            }
        }
        /// <summary>
        /// Контрольная сумма визитной карточки
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Системные данные")]
        [Description("Контрольная сумма визитной карточки устройства")]
        [DisplayName("Контрольная сумма")]
        //[DefaultValue(typeof(UInt64), "0xFFFFFFFFFFFF")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public UInt16 CRC16
        {
            get
            {
                //String msg;
                UInt16 index = 0x2006;
                return System.Convert.ToUInt16(_Device.GetObject(index));
                //return GetCRC16();
            }
            set
            {
                UInt16 index = 0x2006;
                _Device.SetObject(index, value);
            }
        }
        /// <summary>
        /// Устройство НГК, которому принадлежит данная визитная карточка
        /// </summary>
        public Device _Device;
        /// <summary>
        /// Устройство НГК, которому принадлежит данная визитная карточка
        /// </summary>
        [Browsable(false)]
        public Device Device
        {
            get { return _Device; }
            set { this._Device = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="device"></param>
        public VisitingCard(Device device)
        {
            this._Device = device;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Возваращает строковое представление визитной карточки в формате
        /// [Тип устройства];[Серийный номер];[Версия аппаратуры];[версия ПО]
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            EnumTypeConverter converter =
                new EnumTypeConverter(typeof(DeviceType));

            return String.Format("{0}; {1}; {2}; {3}",
                converter.ConvertToString(DeviceType), SerialNumber, 
                HardwareVersion, SoftwareVersion);
            //return base.ToString();
        }

        /// <summary>
        /// Метод рассчитывает контрольную 
        /// сумму CRC16
        /// </summary>
        /// <param name="DataArray">массив байт для рассчёта контрольной суммы</param>
        /// <returns>CRC16</returns>
        public static UInt16 CalcCRC16(byte[] data)
        {
            UInt16 crct = 0xFFFF;

            for (int i = 0; i < data.Length; i++)
            {
                crct = (UInt16)(crct ^ System.Convert.ToUInt16(data[i]));

                for (int n = 0; n <= 7; n++)
                {
                    if ((crct & 0x0001) == 0x0001)
                    {
                        crct = (UInt16)(crct >> 1);
                        crct = (UInt16)(crct ^ 0xA001);
                    }
                    else
                    {
                        crct = (UInt16)(crct >> 1);
                    }
                }
            }
                
            return crct;
        }
        /// <summary>
        /// Рассчитывает контрольная сумму для указанной карты
        /// </summary>
        /// <returns></returns>
        public UInt16 GetCRC16()
        {
            // Предствавляем карту как массив байт
            List<Byte> array = new List<byte>();

            UInt16 x = (UInt16)_Device.DeviceType;
            array.Add((byte)x);
            array.Add((byte)(x >> 8));
            
            x = (new ProductVersion(SoftwareVersion)).TotalVersion;
            array.Add((byte)x);
            array.Add((byte)(x >> 8));

            x = (new ProductVersion(HardwareVersion)).TotalVersion;
            array.Add((byte)x);
            array.Add((byte)(x >> 8));

            array.Add((byte)(SerialNumber >> 40));
            array.Add((byte)(SerialNumber >> 32));
            array.Add((byte)(SerialNumber >> 24));
            array.Add((byte)(SerialNumber >> 16));
            array.Add((byte)(SerialNumber >> 8));
            array.Add((byte)SerialNumber);

            return VisitingCard.CalcCRC16(array.ToArray());
        }
        #endregion
    }

}
