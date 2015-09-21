using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NGK.MeasuringDeviceTech.Classes.MeasuringDevice;

//====================================================================================
namespace NGK.MeasuringDeviceTech.Forms
{
    //================================================================================
    public partial class FormSelectTypeDevice : Form
    {
        //----------------------------------------------------------------------------
        private TYPE_NGK_DEVICE _TypeDevice;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Тип устройства БИ
        /// </summary>
        public TYPE_NGK_DEVICE TypeDevice
        {
            get { return _TypeDevice; }
            set { _TypeDevice = value; }
        }
        //----------------------------------------------------------------------------
        public FormSelectTypeDevice()
        {
            InitializeComponent();
        }
        //----------------------------------------------------------------------------
        private void FormSelectTypeDevice_Load(object sender, EventArgs e)
        {
            List<String> deviceNames;

            this.comboBoxDeviceType.Items.Clear();

            //deviceNames = Enum.GetNames(typeof(TYPE_NGK_DEVICE));
            
            System.Array arr = Enum.GetValues(typeof(TYPE_NGK_DEVICE));

            deviceNames = new List<string>();

            for (int i = 0; i < arr.Length; i++)
			{
                switch ((UInt16)arr.GetValue(i))
                {
                    case (UInt16)TYPE_NGK_DEVICE.UNKNOWN_DEVICE:
                        {
                            //deviceNames.Add("Неизвестное устройство");
                            break;
                        }
                    case (UInt16)TYPE_NGK_DEVICE.BI_BATTERY_POWER:
                        {
                            deviceNames.Add("Устройство БИ(У)-01");
                            break;
                        }
                    case (UInt16)TYPE_NGK_DEVICE.BI_MAIN_POWERED:
                        {
                            deviceNames.Add("Устройство БИ(У)-00");
                            break;
                        }
                    default:
                        {
                            throw new Exception("Получен список устройств не поддерживаемых в данной версии ПО");
                        }
                }			 
			}
            
            this.comboBoxDeviceType.Items.AddRange(deviceNames.ToArray());
            
            this.comboBoxDeviceType.SelectedIndex = 0;
            
            return;
        }
        //----------------------------------------------------------------------------
        private void comboBoxDeviceType_SelectedIndexChanged(
            object sender, 
            EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            String deviceName;

            deviceName = (String)cb.SelectedItem;
            
            switch (deviceName)
            {
                case "Неизвестное устройство":
                    {
                        _TypeDevice = TYPE_NGK_DEVICE.UNKNOWN_DEVICE;
                        break;
                    }
                case "Устройство БИ(У)-01":
                    {
                        _TypeDevice = TYPE_NGK_DEVICE.BI_BATTERY_POWER;
                        break;
                    }
                case "Устройство БИ(У)-00":
                    {
                        _TypeDevice = TYPE_NGK_DEVICE.BI_MAIN_POWERED;
                        break;
                    }
                default:
                    {
                        throw new Exception("Получен список устройств не поддерживаемых в данной версии ПО");
                    }
            }			 
            
            //_TypeDevice = 
            //    (TYPE_NGK_DEVICE)Enum.Parse(typeof(TYPE_NGK_DEVICE), 
            //    (String)cb.SelectedItem);
        }
        //----------------------------------------------------------------------------
    }
    //================================================================================
}
//====================================================================================
// End Of File