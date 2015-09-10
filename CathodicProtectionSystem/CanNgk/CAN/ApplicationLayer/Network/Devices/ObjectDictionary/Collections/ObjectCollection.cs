using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary;
using Common.Collections.ObjectModel;

namespace NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary.Collections
{
    public class ObjectCollection: KeyedCollection<UInt16, DataObject>
    {
        #region Fields And Properties

        private Device _Device;
        
        public Device Owner
        {
            get { return _Device; }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        private ObjectCollection()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        public ObjectCollection(Device owner)
        {
            if (owner == null)
            {
                throw new NullReferenceException();
            }
            _Device = owner;
        }
        #endregion

        #region Methods

        protected override ushort GetKeyForItem(DataObject item)
        {
            item.Device = _Device;
            return item.Index;
        }

        protected override void ClearItems()
        {
            IList<DataObject> list = Items;

            foreach (DataObject item in Items)
            {
                item.Device = null;
            }
            base.ClearItems();

            foreach (DataObject item in list)
            {
                OnCollectionWasChanged(Action.Removing, item);
            }
        }

        protected override void InsertItem(int index, DataObject item)
        {
            base.InsertItem(index, item);
            item.Device = _Device;
            OnCollectionWasChanged(Action.Adding, item);
        }

        protected override void SetItem(int index, DataObject item)
        {
            //base.SetItem(index, item);
            throw new NotImplementedException();
        }

        protected override void RemoveItem(int index)
        {
            DataObject item = Items[index];
            
            if (item != null)
            {
                item.Device = null;
                base.RemoveItem(index);
                OnCollectionWasChanged(Action.Removing, item);
            }
        }

        private void OnCollectionWasChanged(
            Action action, DataObject dataObject)
        {
            KeyedCollectionWasChangedEventArgs<DataObject> args =
                new KeyedCollectionWasChangedEventArgs<DataObject>(action, dataObject);
            EventHandler<KeyedCollectionWasChangedEventArgs<DataObject>> handler =
                CollectionWasChanged;

            if (handler != null)
            {
                foreach (EventHandler<KeyedCollectionWasChangedEventArgs<DataObject>> SingleCast
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

        public event EventHandler<KeyedCollectionWasChangedEventArgs<DataObject>> CollectionWasChanged;
        
        #endregion
    }
}
