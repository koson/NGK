using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary.Collections;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Profiles
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Prototype : IProfile
    {
        #region Constructors

        #endregion

        #region Methods
        /// <summary>
        /// Возвращает профиль устройства на основе известного типа
        /// устройства
        /// </summary>
        /// <param name="type">Тип устройства</param>
        /// <returns>Профиль устройства</returns>
        public static IProfile Create(DeviceType type)
        {
            string msg;

            switch (type)
            {
                case DeviceType.KIP_MAIN_POWERED_v1:
                    {
                        return KIP9810v1.Instance; 
                    }
                case DeviceType.KIP_BATTERY_POWER_v1:
                    {
                        return KIP9811v1.Instance; 
                    }
                default:
                    {
                        msg = String.Format(
                            "Тип устройсва {0} не поддерживается в данной версии ПО", type);
                        throw new NotImplementedException(); 
                    }
            }
        }
        #endregion

        #region IProfile Members

        public abstract DeviceType DeviceType
        {
            get;
        }

        public abstract string Description
        {
            get;
        }

        public abstract Version SoftwareVersion
        {
            get;
        }

        public abstract Version HardwareVersion
        {
            get;
        }

        public abstract ObjectInfoCollection ObjectInfoList
        {
            get; 
        }

        #endregion
    }
}
