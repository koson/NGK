using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
//
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;

namespace Modbus.OSIModel.ApplicationLayer.Slave.DataModel
{
    /// <summary>
    /// ����� ��� �������� ������� ���� ������ ������ ���������� modbus. 
    /// </summary>
    public class CoilsCollection: 
        System.Collections.ObjectModel.KeyedCollection<UInt16, Coil>
    {
        #region Fields And Properties
        /// <summary>
        /// ���������� �������� ������ ��������� ���������� ������/�������
        /// </summary>
        private ModbusSlaveDevice _Device;
        /// <summary>
        /// ���������� ����������, �������� ����������� ������ ��������� ����������
        /// ������/�������
        /// </summary>
        public ModbusSlaveDevice Device
        {
            get { return _Device; }
        }
        #endregion

        #region Constructors
        //---------------------------------------------------------------------------

        //---------------------------------------------------------------------------
        #endregion

        #region Methods

        protected override ushort GetKeyForItem(Coil item)
        {
            return item.Address;
        }

        protected override void InsertItem(int index, Coil item)
        {
            // ������������� ��������� ������������ ��������.
            item.SetOwner(_Device); 
            base.InsertItem(index, item);
            // ���������� �������
            OnListWasChanged();
            return;
        }

        protected override void SetItem(int index, Coil item)
        {
            // ������������� ��������� ������������ ��������.
            item.SetOwner(_Device);
            base.SetItem(index, item);
            // ���������� �������
            OnListWasChanged();
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
            // ���������� �������
            OnListWasChanged();
            return;
        }

        protected override void ClearItems()
        {
            // �������� ��������� ���������� ��������.
            for (int i = 0; i < Count; i++)
            {
                this[i].SetOwner(null);
            }
            base.ClearItems();
            // ���������� �������
            OnListWasChanged();
            return;
        }
        /// <summary>
        /// ����� ���������� ��� ���������� � ���������, ��� ��������� ��������
        /// _Device. ������ ������ modbus-����������, �������� ���������� ������
        /// ��������� ���������� ������/�������. ���� �������� �� ����� null, �� ������
        /// ��������� ��� ����������� ������ ���������. ��� ��� ����������
        /// ����������
        /// </summary>
        /// <param name="owner">�������� ������ ���������</param>
        internal void SetOwner(ModbusSlaveDevice owner)
        {
            if (_Device == null)
            {
                _Device = owner;

                for (int i = 0; i < Count; i++)
                {
                    this[i].SetOwner(_Device);
                }
            }
            else
            {
                // ���� ����������, �������� ����������� ������ ��������� ���������� 
                // ������/������� ����������� ���������������, ����� ������ �� ������. 
                // ����� ��� ������. � ��������� ������, ���������� ����������
                if (_Device.Equals(owner) == false)
                {
                    throw new InvalidOperationException(
                        "������ ��������� ���������� ������/������� ��� ����������� ������� ����������");
                }
            }
        }
        /// <summary>
        /// ���������� ������� ��������� ������, ��������, �������, ���������� 
        /// ��������� ������
        /// </summary>
        private void OnListWasChanged()
        {
            EventHandler handler = ListWasChanged;
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
        #endregion

        #region Events
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������� ���������� ��� ��������� ������ ���������.
        /// </summary>
        public event EventHandler ListWasChanged;
        //---------------------------------------------------------------------------
        #endregion
    }
}
