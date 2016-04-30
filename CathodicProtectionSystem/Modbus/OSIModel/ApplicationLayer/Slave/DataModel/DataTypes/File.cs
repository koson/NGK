using System;
using System.Collections.Generic;
using System.Text;

//===================================================================================
namespace Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes
{
    //===============================================================================
    /// <summary>
    /// ����� ��� ���������� ����� ������ ������ modbus
    /// </summary>
    public class File
    {
        //---------------------------------------------------------------------------
        #region Fields and Properties
        //---------------------------------------------------------------------------
        /// <summary>
        /// ����� �����
        /// </summary>
        private UInt16 _Number;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ����� �����
        /// </summary>
        public UInt16 Number
        {
            get { return _Number; }
            set 
            {
                if (value != 0)
                {
                    _Number = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Number", "����� ����� �� ����� ���� ����� 0");
                }
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ��������� ������� ������������ � ������ �����
        /// </summary>
        private RecordsCollection _RecordsCollection;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������ � �����
        /// </summary>
        public RecordsCollection Records
        {
            get { return _RecordsCollection; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// �������� ������� �����
        /// </summary>
        private String _Description;
        //---------------------------------------------------------------------------
        /// <summary>
        /// �������� ������� �����
        /// </summary>
        public String Description
        {
            get { return _Description; }
            set 
            {
                if (value == null)
                {
                    _Description = String.Empty;
                }
                else
                {
                    _Description = value;
                }
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� �������� ������� �����
        /// </summary>
        private ModbusSlaveDevice _Device;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ����������, �������� ����������� ������ ����
        /// </summary>
        public ModbusSlaveDevice Device
        {
            get { return _Device; }
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Constructors
        //---------------------------------------------------------------------------
        /// <summary>
        /// �����������
        /// </summary>
        public File()
        {
            _Number = 0;
            _Description = String.Empty;
            _Device = null;
            _RecordsCollection = new RecordsCollection();
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="number">����� �����</param>
        /// <param name="description">�������� �����</param>
        public File(UInt16 number, String description)
        {
            Number = number;

            if (description == null)
            {
                _Description = String.Empty;
            }
            else
            {
                _Description = description;
            }

            _Device = null;
            _RecordsCollection = new RecordsCollection();
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Methods
        //---------------------------------------------------------------------------
        /// <summary>
        /// ����� ���������� ��� ���������� � ���������, ��� ��������� ��������
        /// _Device. ������ ������ modbus-����������, �������� ���������� ������
        /// ��������� ���������-��������. ���� �������� �� ����� null, �� ������
        /// ��������� ��� ����������� ������ ���������. ��� ��� ����������
        /// ����������
        /// </summary>
        /// <param name="owner">�������� ������� �����</param>
        internal void SetOwner(ModbusSlaveDevice owner)
        {
            if (_Device == null)
            {
                _Device = owner;
                _RecordsCollection.SetOwner(owner);
            }
            else
            {
                if (owner == null)
                {
                    // ����������� �������� �� ���������
                    _Device = owner;
                }
                else
                {
                    // ���� ����������, �������� ����������� ������ ����
                    // ������������ ����������������, ����� ������ �� ������. 
                    // ����� ��� ������. � ��������� ������, ���������� ����������
                    if (_Device.Equals(owner) == false)
                    {
                        throw new InvalidOperationException(
                            "������ ���� ��� ����������� ������� ����������");
                    }
                }
            }
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file