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
            get { return this._Number; }
            set 
            {
                if (value != 0)
                {
                    this._Number = value;
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
            get { return this._RecordsCollection; }
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
                    this._Description = String.Empty;
                }
                else
                {
                    this._Description = value;
                }
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� �������� ������� �����
        /// </summary>
        private Device _Device;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ����������, �������� ����������� ������ ����
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
        /// <summary>
        /// �����������
        /// </summary>
        public File()
        {
            this._Number = 0;
            this._Description = String.Empty;
            this._Device = null;
            this._RecordsCollection = new RecordsCollection();
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="number">����� �����</param>
        /// <param name="description">�������� �����</param>
        public File(UInt16 number, String description)
        {
            this.Number = number;

            if (description == null)
            {
                this._Description = String.Empty;
            }
            else
            {
                this._Description = description;
            }

            this._Device = null;
            this._RecordsCollection = new RecordsCollection();
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
        internal void SetOwner(Device owner)
        {
            if (this._Device == null)
            {
                this._Device = owner;
                this._RecordsCollection.SetOwner(owner);
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
                    // ���� ����������, �������� ����������� ������ ����
                    // ������������ ����������������, ����� ������ �� ������. 
                    // ����� ��� ������. � ��������� ������, ���������� ����������
                    if (this._Device.Equals(owner) == false)
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