using System;
using System.Collections.Generic;
using System.Text;
using Modbus.OSIModel.ApplicationLayer.Slave;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;

namespace NGK.CorrosionMonitoringSystem.DL
{
    /// <summary>
    /// ��������� ������ ������� CAN-��������� � �������� ������ ������
    /// modbus-����������
    /// </summary>
    public class ModbusAdapterParameterContext
    {
        #region Fields And Properties

        #region Modbus
        /// <summary>
        /// ���������� modbus
        /// </summary>
        public readonly UInt16 FileNumber;
        /// <summary>
        /// ������� � ����� ���������� modbus
        /// </summary>
        public readonly UInt16 RecordNumber;
        #endregion

        #region CAN
        /// <summary>
        /// ������������ ���� CAN ���
        /// </summary>
        public readonly String CanNetwrokName;
        /// <summary>
        /// ������� ������������� ���������� CAN
        /// </summary>
        public readonly Byte NodeId;
        /// <summary>
        /// ������ ������� ������� ����������
        /// </summary>
        private readonly UInt16 Index;
        #endregion

        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        private ModbusAdapterParameterContext()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canNetwrokName"></param>
        /// <param name="nodeId"></param>
        /// <param name="index"></param>
        /// <param name="fileNumber"></param>
        /// <param name="recordNumber"></param>
        public ModbusAdapterParameterContext(String canNetwrokName, 
            Byte nodeId, UInt16 index, UInt16 fileNumber, UInt16 recordNumber)
        {
            CanNetwrokName = canNetwrokName;
            NodeId = nodeId;
            Index = index;
            FileNumber = fileNumber;
            RecordNumber = recordNumber;
        }
        #endregion

        #region Methods

        #endregion
    }
}
