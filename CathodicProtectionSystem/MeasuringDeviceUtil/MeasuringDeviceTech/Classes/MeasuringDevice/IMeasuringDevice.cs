using System;
using System.Text;
using System.IO;

//====================================================================================
namespace NGK.MeasuringDeviceTech.Classes.MeasuringDevice
{
    //================================================================================
    /// <summary>
    /// Итерфейс, который должно реализовать каждое устройство БИ 
    /// </summary>
    public interface IMeasuringDevice : System.ComponentModel.INotifyPropertyChanged
    {
        //----------------------------------------------------------------------------
        /// <summary>
        /// Возвращает серийный номер устройства
        /// </summary>
        /// <returns></returns>
        UInt64 GetSerialNumber();
        //----------------------------------------------------------------------------
        /// <summary>
        /// Возвращает тип/вариант исполнения устройства БИ
        /// </summary>
        /// <returns></returns>
        TYPE_NGK_DEVICE GetDeviceType();
        //----------------------------------------------------------------------------
        /// <summary>
        /// Возвращает объект устройства БИ
        /// </summary>
        /// <returns></returns>
        Object GetDevice();
        //----------------------------------------------------------------------------
        Object Deserialize(String path);
        //----------------------------------------------------------------------------
        Object Deserialize(FileStream stream);
        //----------------------------------------------------------------------------
        Object DeserializeXml(String path);
        //----------------------------------------------------------------------------
        Object DeserializeXml(FileStream stream);
        //----------------------------------------------------------------------------
        Boolean Serialize(String path);
        //----------------------------------------------------------------------------
        Boolean Serialize(FileStream stream);
        //----------------------------------------------------------------------------
        Boolean SerializeXml(String path);
        //----------------------------------------------------------------------------
        Boolean SerializeXml(FileStream stream);
        //----------------------------------------------------------------------------
        void Read_HR_VerifyInitDevice(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out Boolean init,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_HR_SerialNumber(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out UInt64 serialNumber,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_HR_SerialNumber(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host, 
            UInt64 serialNumber,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_HR_NetAddress(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_HR_NetAddress(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_HR_MeasuringPeriod(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_HR_MeasuringPeriod(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_HR_MeasuringVoltagePeriod(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------        
        void Write_HR_MeasuringVoltagePeriod(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_HR_PollingPeriodUSIKPST(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_HR_PollingPeriodUSIKPST(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_HR_PollingPeriodBPI(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_HR_PollingPeriodBPI(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_HR_PollingPeriodChannel1(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_HR_PollingPeriodChannel1(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_HR_PollingPeriodChannel2(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_HR_PollingPeriodChannel2(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_HR_BaudRateCAN(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_HR_BaudRateCAN(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_HR_CurrentShuntValue(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_HR_CurrentShuntValue(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_HR_DateTime(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_HR_DateTime(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error, out DateTime dataTime);
        //----------------------------------------------------------------------------
        void Read_CL_PolarizationPotentialEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_CL_PolarizationPotentialEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_CL_ProtectivePotentialEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_CL_ProtectivePotentialEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_CL_ProtectiveСurrentEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_CL_ProtectiveСurrentEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_CL_PolarizationСurrentEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_CL_PolarizationСurrentEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_CL_InducedVoltageEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_CL_InducedVoltageEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_CL_ExtendedModeX10SumPotentialEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_CL_ExtendedModeX10SumPotentialEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_CL_SendStatusWordEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_CL_SendStatusWordEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_CL_DcCurrentRefereceElectrodeEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_CL_DcCurrentRefereceElectrodeEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_CL_AcCurrentRefereceElectrodeEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Write_CL_AcCurrentRefereceElectrodeEn(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_DI_CaseOpen(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_DI_SupplyVoltageStatus(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_DI_BattaryVoltageStatus(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_DI_CorrosionSensor1(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_DI_CorrosionSensor2(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_DI_CorrosionSensor3(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_TypeOfDevice(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error, out TYPE_NGK_DEVICE type);
        //----------------------------------------------------------------------------
        void Read_IR_SoftWareVersion(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_HardWareVersion(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_SerialNumber(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_CRC16(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_ManufacturerCode(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_Polarization_potential(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_Protective_potential(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_InducedVoltage(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_Protective_current(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_Polarization_current(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_Current_Channel_1(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_Current_Channel_2(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_DepthOfCorrosionUSIKPST(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_SpeedOfCorrosionUSIKPST(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_StatusUSIKPST(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_SupplyVoltage(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_BattaryVoltage(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_InternalTemperatureSensor(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        //void Read_IR_DateTime(
        //    ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
        //    out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_ReferenceElectrodeDCCurrent(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
        void Read_IR_ReferenceElectrodeACCurrent(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error);
        //----------------------------------------------------------------------------
    }
    //================================================================================
}
//====================================================================================
// End of file