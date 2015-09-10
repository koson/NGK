using System;
using System.Collections.Generic;
using System.Text;

namespace Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes
{
    /// <summary>
    /// Шаблон класса для реализации элемента модели данных Modbus (Coil, 
    /// Discrete Input, Holding Register, Input Register)
    /// </summary>
    /// <typeparam name="T">значения типа Boolean, UInt16</typeparam>
    [Serializable]
    public abstract class Parameter<T> where T: struct
    {
        #region Fields and Properies
        /// <summary>
        /// Адрес элемента модели данных
        /// </summary>
        protected UInt16 _Address;
        /// <summary>
        /// Адрес элемента модели данных
        /// </summary>
        public UInt16 Address
        {
            get { return this._Address; }
            set { _Address = value; }
        }      
        /// <summary>
        /// Значение элемента модели данных
        /// </summary>
        protected T _Value;
        /// <summary>
        /// Значение элемента модели данных
        /// </summary>
        public T Value
        {
            get { return _Value; }
            set 
            {
                _Value = value;
                // Генерируем событие
                this.OnValueWasChanged();
            }
        }
        /// <summary>
        /// Описание элемента
        /// </summary>
        protected String _Description;
        /// <summary>
        /// Описание элемента 
        /// </summary>
        public String Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        /// <summary>
        /// Тип данных (параметра) модели данных Modbus. 
        /// </summary>
        public abstract ModbusParameterType ParameterType
        {
            get;
        }
        /// <summary>
        /// Modbus-устройство содержащее данный элемент данных
        /// </summary>
        private Device _Device;
        /// <summary>
        /// Modbus-устройство, владелец данного элемента данных
        /// </summary>
        public Device Device
        {
            get { return _Device; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        public Parameter()
        {
            this._Device = null;
            this._Address = 0;
            this._Description = String.Empty;
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="address">Адрес параметра modbus</param>
        /// <param name="value">Значение параметра</param>
        /// <param name="description">Описание параметра</param>
        public Parameter(UInt16 address, T value, String description)
        {
            _Address = address;
            _Value = value;
            _Device = null;

            if (description != null)
            {
                _Description = description;
            }
            else
            {
                _Description = String.Empty;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Метод вызывается при установке значения параметра мастером сети. 
        /// Контроллер сети получает от мастера запрос и адресует его устройству.
        /// Устройство в соответствии с запросом вызывает данный метод
        /// </summary>
        /// <param name="value">Новое значение параметра</param>
        /// <remarks>
        /// Порядок следования событий, при вызове данного метода:
        /// 1. ValueWasChanged;
        /// 2. MasterSetValue
        /// </remarks>
        internal void SetValue(T value)
        {
            Value = value;
            // Генерируем событие
            OnMasterSetValue();
            return;
        }
        /// <summary>
        /// Метод вызывается при добавлении данного параметра в modbus-устройство.
        /// Данное устройство, является владельцем данного
        /// элемента данных. Если владелец не равен null, то данный
        /// параметр уже принадлежит другому устройству. 
        /// При это вызывается исключение. Так же метод вызывается при удалении
        /// данного параметра из устройства, при этом владелец устанавливается в null
        /// </summary>
        /// <param name="owner">Устройство владелец</param>
        internal void SetOwner(Device owner)
        {
            if (this._Device == null)
            {
                this._Device = owner;
            }
            else
            {
                if (owner == null)
                {
                    // Освобождаем параметр от владельца
                    this._Device = owner;
                }
                else
                {
                    // Если устройство, которой принадлежит данный параметр, 
                    // эквивалентно устанавливаемому, то ничего не делаем. 
                    // Здесь нет ошибки. В противном случае, генерируем исключение
                    if (this.Equals(owner) == false)
                    {
                        throw new InvalidOperationException(
                            "Данный параметр модели данных modbus принадлежит другому modbus-устройству");
                    }
                }
            }
        }
        /// <summary>
        /// Генерирует событие при записи мастером сети нового 
        /// значения данного параметра
        /// </summary>
        protected virtual void OnMasterSetValue()
        {
            EventHandler handler = this.MasterSetValue;
            EventArgs args = new EventArgs();

            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke syncInvoke =
                        singleCast.Target as System.ComponentModel.ISynchronizeInvoke;

                    if (syncInvoke != null)
                    {
                        if (syncInvoke.InvokeRequired == true)
                        {
                            syncInvoke.Invoke(singleCast, new object[] { this, args });
                        }
                        else
                        {
                            singleCast(this, args);
                        }
                    }
                }
            }
            return;
        }
        /// <summary>
        /// Генерирует событие изменения состояния реле
        /// </summary>
        protected virtual void OnValueWasChanged()
        {
            EventHandler handler = this.ValueWasChanged;
            EventArgs args = new EventArgs();

            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke syncInvoke =
                        singleCast.Target as System.ComponentModel.ISynchronizeInvoke;

                    if (syncInvoke != null)
                    {
                        if (syncInvoke.InvokeRequired == true)
                        {
                            syncInvoke.Invoke(singleCast, new object[] { this, args });
                        }
                        else
                        {
                            singleCast(this, args);
                        }
                    }
                }
            }
            return;
        }
        #endregion

        #region Events
        /// <summary>
        /// Событие возникает после измения значения параметра.
        /// </summary>
        public event EventHandler ValueWasChanged;
        /// <summary>
        /// Событие возникает, после записи нового значения параметра
        /// мастером сети.
        /// </summary>
        public event EventHandler MasterSetValue;
        #endregion
    }
}
