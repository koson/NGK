using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//==============================================================================================================
namespace NGK.MeasuringDeviceTech.Classes.MeasuringDevice.UITypeEditors
{
    //==========================================================================================================
    /// <summary>
    /// Контрол для редактора типа свойства MeasuringPeriod устройствао БИ на постоянном питании
    /// </summary>
    public partial class MeasuringPeriodMainPowerControl : UserControl
    {
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Значение параметра
        /// </summary>
        public UInt32 Value
        {
            get 
            {
                switch ((String)comboBoxMode.SelectedItem)
                {
                    case "Указанный период":
                        {
                            return (UInt32)numericUpDownValue.Value;
                        }
                    case "Измерять постоянно":
                        {
                            return 0;
                        }
                    case "Ответ по запросу":
                        {
                            return 0xFFFFFFFF;
                        }
                }
                throw new Exception("Ошибка при получении значения");
            }
            set 
            {
                switch (value)
                {
                    case 0:
                        {
                            numericUpDownValue.Enabled = false;
                            comboBoxMode.SelectedItem = "Измерять постоянно";
                            break;
                        }
                    case 0xFFFFFFFF:
                        {
                            numericUpDownValue.Enabled = false;
                            comboBoxMode.SelectedItem = "Ответ по запросу";
                            break;
                        }
                    default:
                        {
                            numericUpDownValue.Enabled = true;
                            comboBoxMode.SelectedItem = "Указанный период";
                            break;
                        }
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Конструктор по умолчанию запрещён
        /// </summary>
        private MeasuringPeriodMainPowerControl()
        { }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="typeOfDevice">Тип устройства</param>
        public MeasuringPeriodMainPowerControl(TYPE_NGK_DEVICE typeOfDevice)
        {
            InitializeComponent();

            switch (typeOfDevice)
            {
                case TYPE_NGK_DEVICE.BI_MAIN_POWERED:
                    {
                        comboBoxMode.Items.Clear();
                        comboBoxMode.Items.Add("Указанный период");
                        comboBoxMode.Items.Add("Измерять постоянно");
                        comboBoxMode.Items.Add("Ответ по запросу");
                        comboBoxMode.SelectedItem = "Указанный период";

                        numericUpDownValue.Minimum = 1;
                        numericUpDownValue.Maximum = 0x93A80;
                        numericUpDownValue.Increment = 1;

                        break; 
                    }
                case TYPE_NGK_DEVICE.BI_BATTERY_POWER:
                    {
                        comboBoxMode.Items.Clear();
                        comboBoxMode.Items.Add("Указанный период");
                        //comboBoxMode.Items.Add("Измерять постоянно");
                        comboBoxMode.Items.Add("Ответ по запросу");
                        comboBoxMode.SelectedItem = "Указанный период";

                        numericUpDownValue.Minimum = 0x0A;
                        numericUpDownValue.Maximum = 0x93A80;
                        numericUpDownValue.Increment = 1;

                        break; 
                    }
                case TYPE_NGK_DEVICE.UNKNOWN_DEVICE:
                    {
                        throw new Exception("Невозможно создать контрол, неизвестное устройство"); 
                    }
                default:
                    { 
                        throw new Exception("Невозможно создать контрол, неизвестное устройство"); 
                    }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Обработчик нажатия кнопки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            OnValueChanged();
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Генерирут событие изменения значения параметра
        /// </summary>
        private void OnValueChanged()
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, new EventArgs());
            }
            return;
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Наступает при зменении значения параметра
        /// </summary>
        public event NGK.MeasuringDeviceTech.Classes.MeasuringDevice.UITypeEditors.ValueChangedEventHandler ValueChanged;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события нажатия клавишь
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MeasuringPeriodMainPowerControl_KeyDown(object sender, KeyEventArgs e)
        {
            OnValueChanged();
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события изменения режима ввода значения параметра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;

            switch ((String)cb.SelectedItem)
            {
                case "Указанный период":
                    {
                        numericUpDownValue.Enabled = true;
                        break;
                    }
                case "Измерять постоянно":
                    {
                        numericUpDownValue.Enabled = false;
                        break;
                    }
                case "Ответ по запросу":
                    {
                        numericUpDownValue.Enabled = false;
                        break;
                    }
            }
            return;
        }
        //------------------------------------------------------------------------------------------------------
    }
    //==========================================================================================================
}
//==============================================================================================================
// End of file