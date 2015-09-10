using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
//
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;

//===================================================================================
namespace Modbus.OSIModel.ApplicationLayer.Slave.DataModel
{
    //===============================================================================
    /// <summary>
    /// ����� ��� �������� ������� ���� ������ ������ ���������� modbus. 
    /// </summary>
    public class CoilsCollection: 
        System.Collections.ObjectModel.KeyedCollection<UInt16, Coil>
    {
        //---------------------------------------------------------------------------
        #region Fields And Properties
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� �������� ������ ��������� ���������� ������/�������
        /// </summary>
        private Device _Device;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ����������, �������� ����������� ������ ��������� ����������
        /// ������/�������
        /// </summary>
        public Device Device
        {
            get { return this._Device; }
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Constructors
        //---------------------------------------------------------------------------

        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Methods
        //---------------------------------------------------------------------------
        protected override ushort GetKeyForItem(Coil item)
        {
            return item.Address;
        }
        //---------------------------------------------------------------------------
        protected override void InsertItem(int index, Coil item)
        {
            // ������������� ��������� ������������ ��������.
            item.SetOwner(this._Device); 
            base.InsertItem(index, item);
            // ���������� �������
            this.OnListWasChanged();
            return;
        }
        //---------------------------------------------------------------------------
        protected override void SetItem(int index, Coil item)
        {
            // ������������� ��������� ������������ ��������.
            item.SetOwner(this._Device);
            base.SetItem(index, item);
            // ���������� �������
            this.OnListWasChanged();
            return;
        }
        //---------------------------------------------------------------------------
        protected override void RemoveItem(int index)
        {
            // �������� ��������� ���������� ��������.
            if (this[index] != null)
            {
                this[index].SetOwner(null);
            }
            base.RemoveItem(index);
            // ���������� �������
            this.OnListWasChanged();
            return;
        }
        //---------------------------------------------------------------------------
        protected override void ClearItems()
        {
            // �������� ��������� ���������� ��������.
            for (int i = 0; i < this.Count; i++)
            {
                this[i].SetOwner(null);
            }
            base.ClearItems();
            // ���������� �������
            this.OnListWasChanged();
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ����� ���������� ��� ���������� � ���������, ��� ��������� ��������
        /// _Device. ������ ������ modbus-����������, �������� ���������� ������
        /// ��������� ���������� ������/�������. ���� �������� �� ����� null, �� ������
        /// ��������� ��� ����������� ������ ���������. ��� ��� ����������
        /// ����������
        /// </summary>
        /// <param name="owner">�������� ������ ���������</param>
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
                // ���� ����������, �������� ����������� ������ ��������� ���������� 
                // ������/������� ����������� ���������������, ����� ������ �� ������. 
                // ����� ��� ������. � ��������� ������, ���������� ����������
                if (this._Device.Equals(owner) == false)
                {
                    throw new InvalidOperationException(
                        "������ ��������� ���������� ������/������� ��� ����������� ������� ����������");
                }
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ������� ��������� ������, ��������, �������, ���������� 
        /// ��������� ������
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
        /// ������� ���������� ��� ��������� ������ ���������.
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