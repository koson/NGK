using System;
using System.Collections.Generic;
using System.Text;

namespace Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes
{
    /// <summary>
    /// ����� ��� ���������� ������ � ������� ����� ������ ������ modbus
    /// </summary>
    public class Record: Parameter<UInt16>
    {
        #region Fields And Properties
        public override ModbusParameterType ParameterType
        {
            get { return ModbusParameterType.Record; }
        }
        #endregion

        #region Constructors and Destructor
        /// <summary>
        /// �����������.
        /// </summary>
        private Record(): base()
        {
            _Value = 0;            
        }
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="address">����� ����������� �����\������</param>
        /// <param name="value">�������� ����������� �����\������</param>
        /// <param name="description">�������� ����������� �����\������</param>
        public Record(UInt16 address, UInt16 value, String description)
            :
            base(address, value, description)
        {
        }
        #endregion

        #region Methods
        //---------------------------------------------------------------------------
        //---------------------------------------------------------------------------
        #endregion
    }
}
