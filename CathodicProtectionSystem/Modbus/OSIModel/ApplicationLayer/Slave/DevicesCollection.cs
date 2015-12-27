using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Common.Controlling;

namespace Modbus.OSIModel.ApplicationLayer.Slave
{
    /// <summary>
    /// Класс реализует коллекцию slave modbus-устройств
    /// </summary>
    public class DevicesCollection: KeyedCollection<Byte, Device>
    {
        #region Fields and Properties
        /// <summary>
        /// Контроллер сети, которой принадлежит данная коллекция устройств
        /// </summary>
        private NetworkController _NetworkController;
        /// <summary>
        /// Возвращает контроллер сети, которой принадлежит данная коллекция устройств
        /// </summary>
        public NetworkController NetworkController
        {
            get { return _NetworkController; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        private DevicesCollection()
        { 
            throw new NotImplementedException(); 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="network"></param>
        public DevicesCollection(NetworkController network)
        {
            if (network == null)
            {
                throw new NullReferenceException();
            }
            _NetworkController = network;
        }
        #endregion

        #region Methods

        protected override void ClearItems()
        {
            // Обнуляем владельца удаляемого элемента.
            foreach (Device device in Items)
            {
                device.SetOwner(null);
            }
            
            base.ClearItems();
            
            // Вызываем событие
            OnItemsListWasChanged();
            
            return;
        }

        protected override byte GetKeyForItem(Device item)
        {
            return item.Address;
        }

        protected override void InsertItem(int index, Device item)
        {
            // Устанавливаем владельца добавляемого элемента.
            item.SetOwner(_NetworkController); 
            base.InsertItem(index, item);
            // Вызываем событие
            OnItemsListWasChanged();
            return;
        }

        protected override void RemoveItem(int index)
        {
            // Обнуляем владельца удаляемого элемента.
            if (this[index] != null)
            {
                this[index].SetOwner(null);
            }
            base.RemoveItem(index);
            // Вызываем событие
            OnItemsListWasChanged();
            return;
        }

        protected override void SetItem(int index, Device item)
        {
            // Устанавливаем владельца добавляемого элемента.
            item.SetOwner(_NetworkController);
            base.SetItem(index, item);
            // Вызываем событие
            OnItemsListWasChanged();
            return;
        }
        /// <summary>
        /// Генерирует событие ItemsListWasChanged
        /// </summary>
        public void OnItemsListWasChanged()
        {
            EventArgs args = new EventArgs();
            EventHandler handler = ItemsListWasChanged;

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
                            syncInvoke.Invoke(singleCast, new object[] { this, args });
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
            return;
        }
        #endregion

        #region Events
        /// <summary>
        /// Событие возникает при добавлении, замене или удалении элемента 
        /// в списке
        /// </summary>
        public event EventHandler ItemsListWasChanged;
        #endregion
    }
}
