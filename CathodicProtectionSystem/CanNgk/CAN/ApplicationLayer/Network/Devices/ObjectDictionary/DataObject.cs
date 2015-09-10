using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataTypes;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary;

namespace NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary
{
    public class DataObject
    {
        #region Fields And Properties
        private Device _Device;
        
        public Device Device
        {
            get { return _Device; }
            set { _Device = value; }
        }

        private UInt16 _Index;

        public UInt16 Index
        {
            get { return _Index; }
            set { _Index = value; }
        }
        private UInt32 _Value;

        public UInt32 Value
        {
            get { return _Value; }
            set 
            {
                _Value = value;
                _Modified = DateTime.Now;
            }
        }
        private DateTime _Modified;
        public DateTime Modified
        {
            get { return _Modified; }
            //set { _Modified = value; }
        }
        private ObjectStatus _Status = ObjectStatus.NoError;
        /// <summary>
        /// Состояние объекта словаря
        /// </summary>
        public ObjectStatus Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        private ObjectInfo _Info;
        /// <summary>
        /// Информация об объекте словаря
        /// </summary>
        public ObjectInfo Info
        {
            get { return _Info; }
        }
        /// <summary>
        /// Конечное значение параметра с учётом преобразований
        /// </summary>
        public ValueType TotalValue
        {
            get { return _Info.DataType.ConvertToTotalValue(_Value); }
            set { _Value = _Info.DataType.ConvertToBasis(value); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public DataObject(ObjectInfo info)
        {
            _Info = info;
            _Index = info.Index;
            _Value = info.DefaultValue;
            _Status = ObjectStatus.NoError;
        }
        #endregion
    }
}
