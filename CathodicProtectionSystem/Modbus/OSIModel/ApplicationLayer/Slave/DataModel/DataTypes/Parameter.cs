using System;
using System.Collections.Generic;
using System.Text;

namespace Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes
{
    /// <summary>
    /// ������ ������ ��� ���������� �������� ������ ������ Modbus (Coil, 
    /// Discrete Input, Holding Register, Input Register)
    /// </summary>
    /// <typeparam name="T">�������� ���� Boolean, UInt16</typeparam>
    [Serializable]
    public abstract class Parameter<T> where T: struct
    {
        #region Fields and Properies
        /// <summary>
        /// ����� �������� ������ ������
        /// </summary>
        protected UInt16 _Address;
        /// <summary>
        /// ����� �������� ������ ������
        /// </summary>
        public UInt16 Address
        {
            get { return this._Address; }
            set { _Address = value; }
        }      
        /// <summary>
        /// �������� �������� ������ ������
        /// </summary>
        protected T _Value;
        /// <summary>
        /// �������� �������� ������ ������
        /// </summary>
        public T Value
        {
            get { return _Value; }
            set 
            {
                _Value = value;
                // ���������� �������
                this.OnValueWasChanged();
            }
        }
        /// <summary>
        /// �������� ��������
        /// </summary>
        protected String _Description;
        /// <summary>
        /// �������� �������� 
        /// </summary>
        public String Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        /// <summary>
        /// ��� ������ (���������) ������ ������ Modbus. 
        /// </summary>
        public abstract ModbusParameterType ParameterType
        {
            get;
        }
        /// <summary>
        /// Modbus-���������� ���������� ������ ������� ������
        /// </summary>
        private Device _Device;
        /// <summary>
        /// Modbus-����������, �������� ������� �������� ������
        /// </summary>
        public Device Device
        {
            get { return _Device; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// �����������
        /// </summary>
        public Parameter()
        {
            this._Device = null;
            this._Address = 0;
            this._Description = String.Empty;
        }
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="address">����� ��������� modbus</param>
        /// <param name="value">�������� ���������</param>
        /// <param name="description">�������� ���������</param>
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
        /// ����� ���������� ��� ��������� �������� ��������� �������� ����. 
        /// ���������� ���� �������� �� ������� ������ � �������� ��� ����������.
        /// ���������� � ������������ � �������� �������� ������ �����
        /// </summary>
        /// <param name="value">����� �������� ���������</param>
        /// <remarks>
        /// ������� ���������� �������, ��� ������ ������� ������:
        /// 1. ValueWasChanged;
        /// 2. MasterSetValue
        /// </remarks>
        internal void SetValue(T value)
        {
            Value = value;
            // ���������� �������
            OnMasterSetValue();
            return;
        }
        /// <summary>
        /// ����� ���������� ��� ���������� ������� ��������� � modbus-����������.
        /// ������ ����������, �������� ���������� �������
        /// �������� ������. ���� �������� �� ����� null, �� ������
        /// �������� ��� ����������� ������� ����������. 
        /// ��� ��� ���������� ����������. ��� �� ����� ���������� ��� ��������
        /// ������� ��������� �� ����������, ��� ���� �������� ��������������� � null
        /// </summary>
        /// <param name="owner">���������� ��������</param>
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
                    // ����������� �������� �� ���������
                    this._Device = owner;
                }
                else
                {
                    // ���� ����������, ������� ����������� ������ ��������, 
                    // ������������ ����������������, �� ������ �� ������. 
                    // ����� ��� ������. � ��������� ������, ���������� ����������
                    if (this.Equals(owner) == false)
                    {
                        throw new InvalidOperationException(
                            "������ �������� ������ ������ modbus ����������� ������� modbus-����������");
                    }
                }
            }
        }
        /// <summary>
        /// ���������� ������� ��� ������ �������� ���� ������ 
        /// �������� ������� ���������
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
        /// ���������� ������� ��������� ��������� ����
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
        /// ������� ��������� ����� ������� �������� ���������.
        /// </summary>
        public event EventHandler ValueWasChanged;
        /// <summary>
        /// ������� ���������, ����� ������ ������ �������� ���������
        /// �������� ����.
        /// </summary>
        public event EventHandler MasterSetValue;
        #endregion
    }
}
