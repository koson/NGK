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
        /// ���������� ������� ���������� �� ������ ���������� ����
        /// ����������
        /// </summary>
        /// <param name="type">��� ����������</param>
        /// <returns>������� ����������</returns>
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
                            "��� ��������� {0} �� �������������� � ������ ������ ��", type);
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
