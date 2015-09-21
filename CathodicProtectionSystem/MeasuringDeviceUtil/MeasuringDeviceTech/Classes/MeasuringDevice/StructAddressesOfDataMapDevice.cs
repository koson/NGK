using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

//===========================================================================================
namespace NGK.MeasuringDeviceTech.Classes.MeasuringDevice
{  
    //=======================================================================================
    /// <summary>
    /// Адреса реле в адресном пространстве modbus устройства БИ
    /// </summary>
    public struct BI_ADDRESSES_OF_COILS
    {
        public const UInt16 PolarizationPotentialEn = 0x0000;
        public const UInt16 ProtectivePotentialEn = 0x0001;
        public const UInt16 ProtectiveСurrentEn = 0x0002;
        public const UInt16 PolarizationСurrentEn = 0x0003;
        public const UInt16 InducedVoltageEn = 0x0004;
        public const UInt16 ExtendedModeX10SumPotentialEn = 0x0005;
        public const UInt16 SendStatusWordEn = 0x0006;
        public const UInt16 DcCurrentRefereceElectrodeEn = 0x0007;
        public const UInt16 AcCurrentRefereceElectrodeEn = 0x0008;
    }
    //=======================================================================================
    /// <summary>
    /// Адреса регистров хранения в адресном пространстве modbus устройства БИ
    /// </summary>
    public struct BI_ADDRESSES_OF_HOLDINGREGISTERS
    {
        public const UInt16 SerialNumber = 0x0000; // Длина параметра 3 регистар
        public const UInt16 NetAddress = 0x0003; // Длина параметра 1 регистр
        public const UInt16 MeasuringPeriod = 0x0004; // Длина параметра 2 регистра
        public const UInt16 MeasuringSupplyVoltagePeriod = 0x0006; // Длина параметра 1 регистр
        public const UInt16 PollingPeriodUSIKPST = 0x0007; // Длина параметра 1 регистр
        public const UInt16 PollingPeriodBPI = 0x0008; // Длина параметра 1 регистр
        public const UInt16 PollingPeriodChannel1_4_20 = 0x0009; // Длина параметра 1 регистр
        public const UInt16 PollingPeriodChannel2_4_20 = 0x000A; // Длина параметра 1 регистр
        public const UInt16 BaudRateCAN = 0x000B; // Длина параметра 1 регистр
        public const UInt16 CurrentShuntValue = 0x000C; // Длина параметра 1 регистр
        public const UInt16 DateTime = 0x000D; // Длина параметра 2 регистра
    }
    //=======================================================================================
    /// <summary>
    /// Адреса дискретных входов в адресном пространстве modbus устройства БИ
    /// </summary>
    public struct BI_ADDRESSES_OF_DISCRETESINPUTS
    {
        public const UInt16 CaseOpen = 0x0000;
        public const UInt16 SupplyVoltageStatus = 0x0001;
        public const UInt16 BattaryVoltageStatus = 0x0002;
        public const UInt16 CorrosionSensor1 = 0x0003;
        public const UInt16 CorrosionSensor2 = 0x0004;
        public const UInt16 CorrosionSensor3 = 0x0005;
    }
    //=======================================================================================
    /// <summary>
    /// Адреса входных регистров в адресном пространстве modbus устройства БИ
    /// </summary>
    public struct BI_ADDRESSES_OF_INPUTREGISTERS
    {
        // Визитная карточка устройства
        public const UInt16 TypeOfDevice = 0x0000;  // Длина параметра 1 регистр
        public const UInt16 VersionSoftware = 0x0001; // Длина параметра 1 регистр
        public const UInt16 VersionHardware = 0x0002; // Длина параметра 1 регистр
        public const UInt16 SerialNumber = 0x0003; // Длина параметра 3 регистра
        public const UInt16 CRC16 = 0x0006; // Длина параметра 1 регистр
        // Конец визитной карточки
        public const UInt16 ManufacturerCode = 0x0007; // Длина параметра 1 регистр
        public const UInt16 Polarization_potential = 0x0008; // Длина параметра 1 регистр
        public const UInt16 Protective_potential = 0x0009; // Длина параметра 1 регистр
        public const UInt16 InducedVoltage = 0x000A; // Длина параметра 1 регистр
        public const UInt16 Protective_current = 0x000B;  // Длина параметра 1 регистр
        public const UInt16 Polarization_current = 0x000C;  // Длина параметра 1 регистр
        public const UInt16 Current_Channel_1 = 0x000D;  // Длина параметра 1 регистр
        public const UInt16 Current_Channel_2 = 0x000E;  // Длина параметра 1 регистр
        public const UInt16 DepthOfCorrosionUSIKPST = 0x000F;  // Длина параметра 1 регистр
        public const UInt16 SpeedOfCorrosionUSIKPST = 0x0010;  // Длина параметра 1 регистр
        public const UInt16 StatusUSIKPST = 0x0011;  // Длина параметра 1 регистр
        public const UInt16 SupplyVoltage = 0x00012;  // Длина параметра 1 регистр
        public const UInt16 BattaryVoltage = 0x0013;  // Длина параметра 1 регистр
        public const UInt16 InternalTemperatureSensor = 0x0014;  // Длина параметра 1 регистр
        //public const UInt16 DateTime = 0x0015;  // Длина параметра 2 регистра
        public const UInt16 ReferenceElectrodeDcCurrent = 0x0015; // Длина параметра 1 регистр
        public const UInt16 ReferenceElectrodeAcCurrent = 0x0016; // Длина параметра 1 регистр
    }
    //=======================================================================================
}
//===========================================================================================
// End of file