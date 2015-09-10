using System;
using System.Collections.Generic;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;

//===================================================================================
namespace Modbus.OSIModel.ApplicationLayer.Slave.DataModel
{
    //===============================================================================
    /// <summary>
    /// Класс для реализации таблицы входных регистров устройсва modbus
    /// </summary>
    [Serializable]
    public class InputRegistersCollection : 
        System.Collections.ObjectModel.KeyedCollection<UInt16, InputRegister>
    {
        //---------------------------------------------------------------------------
        #region Fields and Properties
        //---------------------------------------------------------------------------
        /// <summary>
        /// Устройство владелец данной коллекции входных регистров
        /// </summary>
        private Device _Device;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает устройство, которому принадлежит данная коллекция 
        /// входных регистров
        /// </summary>
        public Device Device
        {
            get { return this._Device; }
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Constructors and Destructor
        //---------------------------------------------------------------------------
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Methods
        //---------------------------------------------------------------------------
        protected override ushort GetKeyForItem(InputRegister item)
        {
            return item.Address;
        }
        //---------------------------------------------------------------------------
        protected override void InsertItem(int index, InputRegister item)
        {
            // Устанавливаем владельца добавляемого элемента.
            item.SetOwner(this._Device);
            base.InsertItem(index, item);
            // Генерируем событие
            this.OnListWasChanged();
            return;
        }
        //---------------------------------------------------------------------------
        protected override void SetItem(int index, InputRegister item)
        {
            // Устанавливаем владельца добавляемого элемента.
            item.SetOwner(this._Device);
            base.SetItem(index, item);
            // Генерируем событие
            this.OnListWasChanged();
            return;
        }
        //---------------------------------------------------------------------------
        protected override void RemoveItem(int index)
        {
            // Обнуляем владельца удаляемого элемента.
            if (this[index] != null)
            {
                this[index].SetOwner(null);
            }
            base.RemoveItem(index);
            // Генерируем событие
            this.OnListWasChanged();
            return;
        }
        //---------------------------------------------------------------------------
        protected override void ClearItems()
        {
            // Обнуляем владельца удаляемого элемента.
            for (int i = 0; i < this.Count; i++)
            {
                this[i].SetOwner(null);
            }
            base.ClearItems();
            // Генерируем событие
            this.OnListWasChanged();
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Метод вызывается при добавлении в коллекцию, для установки свойства
        /// _Device. Данный объект modbus-устройства, является владельцем данной
        /// коллекции регистров-хранения. Если владелец не равен null, то данная
        /// коллекция уже принадлежит другой коллекции. При это вызывается
        /// исключение
        /// </summary>
        /// <param name="owner">Владелец данной коллекции</param>
        internal void SetOwner(Device owner)
        {
            if (this._Device == null)
            {
                this._Device = owner;

                for (int i = 0; i < this.Count; i++)
                {
                    this[i].SetOwner(this._Device);
                }
            }
            else
            {
                // Если устройство, которому принадлежит данная коллекция входных 
                // регистров эквивалента устанавливаемой, тогда ничего не делаем. 
                // Здесь нет ошибки. В противном случае, генерируем исключение
                if (this._Device.Equals(owner) == false)
                {
                    throw new InvalidOperationException(
                        "Данная коллекция входных регистров уже принадлежит другому устройству");
                }
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Генерирует событие изменение списка, удаление, вставка, добавление 
        /// элементов списка
        /// </summary>
        private void OnListWasChanged()
        {
            EventHandler handler = this.ListWasChanged;
            EventArgs args = new EventArgs();
            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke syncInvoke =
                        singleCast.Target as System.ComponentModel.ISynchronizeInvoke;
                    if (syncInvoke != null)
                    {
                        if (syncInvoke.InvokeRequired)
                        {
                            syncInvoke.Invoke(singleCast, new Object[] { this, args });
                        }
                        else
                        {
                            singleCast(this, args);
                        }
                    }
                    else
                    {
                        singleCast(this, args);
                    }
                }
            }
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Events
        //---------------------------------------------------------------------------
        /// <summary>
        /// Событие происходит при изменении списка коллекции.
        /// </summary>
        public event EventHandler ListWasChanged;
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file