using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

//========================================================================================
namespace NGK.CAN.DataLinkLayer.CanPort
{
    //====================================================================================
    /// <summary>
    /// Класс предоставляет информацию об устройстве CAN-порта
    /// </summary>
    public class DeviceInfo
    {
        //--------------------------------------------------------------------------------
        public string _Description;
        //-------------------------------------------------------------------------------- 
        /// <summary>
        /// Gets the device description string. 
        /// Связано со свойством: Ixxat.Vci3.IVciDevice.Description
        /// </summary>
        [Description("Описание устройства")]
        [Category("Устройство")]
        [DisplayName("Описание")]
        [Browsable(true)]
        [ReadOnly(true)]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        //--------------------------------------------------------------------------------
        public Guid _DeviceClass;
        //-------------------------------------------------------------------------------- 
        /// <summary>
        /// Gets the ID of the device class. Each device driver identifies its 
        /// device class in the form of a globally unique ID (GUID). Different 
        /// adapters belong to different device classes. Applications can use 
        /// the device class to distinguish between an IPC-I165/PCI and a PC-I04/PCI card, for example.
        /// Связано со свойством: Guid Ixxat.Vci3.IVciDevice.DeviceClass
        /// </summary>
        [Description("Gets the ID of the device class. Each device driver identifies its device class in the form of a globally unique ID (GUID). Different adapters belong to different device classes. Applications can use the device class to distinguish between an IPC-I165/PCI and a PC-I04/PCI card, for example.")]
        [Category("Устройство")]
        [DisplayName("GUID класса устройства")]
        [Browsable(true)]
        [ReadOnly(true)]
        public Guid DeviceClass
        {
            get { return _DeviceClass; }
            set { _DeviceClass = value; }
        }
        //--------------------------------------------------------------------------------
        public Version _DriverVersion;
        //-------------------------------------------------------------------------------- 
        /// <summary>
        /// Gets the version of the VCI device driver. 
        /// Связано со свойством: Version Ixxat.Vci3.IVciDevice.DriverVersion
        /// </summary>
        [Description("Gets the version of the VCI device driver.")]
        [Category("Устройство")]
        [DisplayName("Версия драйвера устройства")]
        [Browsable(true)]
        [ReadOnly(true)]
        public Version DriverVersion
        {
            get { return _DriverVersion; }
            set { _DriverVersion = value; }
        }
        //--------------------------------------------------------------------------------
        public Version _HardwareVersion;
        //-------------------------------------------------------------------------------- 
        /// <summary>
        /// Gets the version of the VCI device hardware. 
        /// Связано со свойством: Version Ixxat.Vci3.IVciDevice.HardwareVersion
        /// </summary>
        [Description("Gets the version of the VCI device hardware")]
        [Category("Устройство")]
        [DisplayName("Версия аппаратного обеспечения")]
        [Browsable(true)]
        [ReadOnly(true)]
        public Version HardwareVersion
        {
            get { return _HardwareVersion; }
            set { _HardwareVersion = value; }
        }
        //--------------------------------------------------------------------------------
        public string _Manufacturer;
        //-------------------------------------------------------------------------------- 
        /// <summary>
        /// Gets the device manufacturer string.
        /// Связано со свойством: string Ixxat.Vci3.IVciDevice.Manufacturer
        /// </summary>
        [Description("Gets the device manufacturer string")]
        [Category("Устройство")]
        [DisplayName("Производитель")]
        [Browsable(true)]
        [ReadOnly(true)]
        public string Manufacturer
        {
            get { return _Manufacturer; }
            set { _Manufacturer = value; }
        }
        //--------------------------------------------------------------------------------
        public object _UniqueHardwareId;
        //-------------------------------------------------------------------------------- 
        /// <summary>
        /// Gets the unique ID of the adapter. Each adapter has a unique ID 
        /// that can be used to distinguish between two PC-I04/PCI cards, for example. 
        /// Because this value can be either a GUID or a string with the serial 
        /// number the retrieved value is either a string reference or a boxed Guid instance.
        /// Связано со свойством: object Ixxat.Vci3.IVciDevice.UniqueHardwareId
        /// </summary>
        [Description("Gets the unique ID of the adapter. Each adapter has a unique ID that can be used to distinguish between two PC-I04/PCI cards, for example. Because this value can be either a GUID or a string with the serial number the retrieved value is either a string reference or a boxed Guid instance.")]
        [Category("Устройство")]
        [DisplayName("GUID адаптера")]
        [Browsable(true)]
        [ReadOnly(true)]
        public object UniqueHardwareId
        {
            get { return _UniqueHardwareId; }
            set { _UniqueHardwareId = value; }
        }
        //--------------------------------------------------------------------------------
        ///// <summary>
        ///// Gets the unique VCI object id of the device.
        ///// Связано со свойством: long Ixxat.Vci3.IVciDevice.VciObjectId
        ///// </summary>
        //[Description("Gets the unique VCI object id of the device")]
        //[Category("Устройство")]
        //[DisplayName("")]
        //[ReadOnly(true)]
        //public long VciObjectId;
        //--------------------------------------------------------------------------------
        public ControllerInfo[] _Equipment;
        //-------------------------------------------------------------------------------- 
        /// <summary>
        /// Gets a description of the hardware equipment of the device.
        /// Связано со свойством: Ixxat.Vci3.VciCtrlInfo[] Ixxat.Vci3.IVciDevice.Equipment
        /// </summary>
        [Description("Gets a description of the hardware equipment of the device.")]
        [Category("Аппаратура")]
        [DisplayName("Аппаратура")]
        [Browsable(true)]
        [ReadOnly(true)]
        [TypeConverter(typeof(ArrayConverter))]
        public ControllerInfo[] Equipment
        {
            get { return _Equipment; }
            set { _Equipment = value; }
        }
        //-------------------------------------------------------------------------------- 
    }
    //====================================================================================
}
//========================================================================================
// End Of File