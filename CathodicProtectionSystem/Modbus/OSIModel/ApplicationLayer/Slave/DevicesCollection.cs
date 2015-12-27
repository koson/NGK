using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Common.Controlling;

namespace Modbus.OSIModel.ApplicationLayer.Slave
{
    /// <summary>
    /// ����� ��������� ��������� slave modbus-���������
    /// </summary>
    public class DevicesCollection: KeyedCollection<Byte, Device>
    {
        #region Fields and Properties
        /// <summary>
        /// ���������� ����, ������� ����������� ������ ��������� ���������
        /// </summary>
        private NetworkController _NetworkController;
        /// <summary>
        /// ���������� ���������� ����, ������� ����������� ������ ��������� ���������
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
            // �������� ��������� ���������� ��������.
            foreach (Device device in Items)
            {
                device.SetOwner(null);
            }
            
            base.ClearItems();
            
            // �������� �������
            OnItemsListWasChanged();
            
            return;
        }

        protected override byte GetKeyForItem(Device item)
        {
            return item.Address;
        }

        protected override void InsertItem(int index, Device item)
        {
            // ������������� ��������� ������������ ��������.
            item.SetOwner(_NetworkController); 
            base.InsertItem(index, item);
            // �������� �������
            OnItemsListWasChanged();
            return;
        }

        protected override void RemoveItem(int index)
        {
            // �������� ��������� ���������� ��������.
            if (this[index] != null)
            {
                this[index].SetOwner(null);
            }
            base.RemoveItem(index);
            // �������� �������
            OnItemsListWasChanged();
            return;
        }

        protected override void SetItem(int index, Device item)
        {
            // ������������� ��������� ������������ ��������.
            item.SetOwner(_NetworkController);
            base.SetItem(index, item);
            // �������� �������
            OnItemsListWasChanged();
            return;
        }
        /// <summary>
        /// ���������� ������� ItemsListWasChanged
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
        /// ������� ��������� ��� ����������, ������ ��� �������� �������� 
        /// � ������
        /// </summary>
        public event EventHandler ItemsListWasChanged;
        #endregion
    }
}
