using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
//using System.Windows.Forms.Design;
using NGK.CAN.ApplicationLayer.Network.Master;
using Common.Collections.ObjectModel;
using NGK.CAN.Design;

namespace NGK.CAN.ApplicationLayer.Network.Devices.Collections
{
    [Editor(typeof(DevicesCollectionEditor), typeof(UITypeEditor))]
    [Serializable]
    public class DevicesCollection: KeyedCollection<Byte, Device>
    {
        #region Fields And Properties

        private INetworkController _Network;
        
        public INetworkController Owner
        {
            get { return _Network; }
        }
        /// <summary>
        /// Возвращает количество неисправных устройств
        /// </summary>
        public int FaultyDevices
        {
            get 
            {
                int count = 0;

                foreach (Device device in Items)
                {
                    if ((device.Status == DeviceStatus.CommunicationError) ||
                        (device.Status == DeviceStatus.ConfigurationError))
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        #endregion

        #region Constructors

        private DevicesCollection()
        {
            throw new NotImplementedException();
        }

        public DevicesCollection(INetworkController network)
        {
            if (network == null)
            {
                throw new NullReferenceException();
            }

            _Network = network;
        }

        #endregion

        #region Methods

        protected override byte GetKeyForItem(Device item)
        {
            item.Network = _Network;
            return item.NodeId;
        }

        protected override void ClearItems()
        {
            IList<Device> list = Items;

            foreach (Device device in Items)
            {
                device.Network = null;
            }

            base.ClearItems();

            foreach (Device device in list)
            {
                OnCollectionWasChanged(Action.Removing, device);
            }
        }

        protected override void RemoveItem(int index)
        {
            Device device = Items[index];
            if (device == null)
            {
                return;
            }
            device.Network = null;
            base.RemoveItem(index);
            OnCollectionWasChanged(Action.Removing, device); 
        }

        protected override void InsertItem(int index, Device item)
        {
            base.InsertItem(index, item);
            item.Network = _Network;
            OnCollectionWasChanged(Action.Adding, item);
        }

        protected override void SetItem(int index, Device item)
        {
            //item.Network = _Network;
            //base.SetItem(index, item);
            throw new NotImplementedException();
        }

        public Device[] ToArray()
        {
            List<Device> list = new List<Device>(base.Items);
            return list.ToArray();
        }

        private void OnCollectionWasChanged(
            Action action, Device device)
        {
            KeyedCollectionWasChangedEventArgs<Device> args =
                new KeyedCollectionWasChangedEventArgs<Device>(action, device);
            EventHandler<KeyedCollectionWasChangedEventArgs<Device>> handler = 
                CollectionWasChanged;

            if (handler != null)
            {
                foreach (EventHandler<KeyedCollectionWasChangedEventArgs<Device>> SingleCast 
                    in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke syncInvoke =
                        SingleCast.Target as System.ComponentModel.ISynchronizeInvoke;

                    try
                    {
                        if ((syncInvoke != null) && (syncInvoke.InvokeRequired))
                        {
                            syncInvoke.Invoke(SingleCast, new Object[] { this, args });
                        }
                        else
                        {
                            SingleCast(this, args);
                        }
                    }
                    catch
                    { throw; }
                }
            }
        }

        #endregion

        #region Events
        /// <summary>
        /// Событие происходит при изменении коллекции
        /// (добавлении, удалении элементов)
        /// </summary>
        public event EventHandler<KeyedCollectionWasChangedEventArgs<Device>> CollectionWasChanged;
        #endregion
    }
}
