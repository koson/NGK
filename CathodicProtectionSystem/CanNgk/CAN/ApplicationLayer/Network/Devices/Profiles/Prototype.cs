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
        #region Fields And Properties

        protected ObjectInfoCollection _ObjectInfoList;
        protected ComplexParameterCollection _ComplexParameters;

        public abstract DeviceType DeviceType { get; }

        public abstract string Description { get; }

        public abstract Version SoftwareVersion { get; }

        public abstract Version HardwareVersion { get; }

        public ObjectInfoCollection ObjectInfoList { get { return _ObjectInfoList; } }

        public ComplexParameterCollection ComplexParameters { get { return _ComplexParameters; } }

        protected static object SyncRoot = new object(); 

        #endregion

        #region Constructors

        public Prototype()
        {
            CreateObjectDictionary();
        }

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

        protected virtual void CreateObjectDictionary()
        {
            _ComplexParameters = new ComplexParameterCollection();
            _ObjectInfoList = new ObjectInfoCollection();
        }

        #endregion
    }
}
