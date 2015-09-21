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
    public partial class Channel4_20Control : UserControl
    {
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Константа определяющая выключенное состояние канала измерения.
        /// </summary>
        public const UInt32 MAXVALUE = 604800;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Константа для определения занчения "Выключено"
        /// </summary>
        public const UInt32 OFF = 655350;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Значение параметра
        /// </summary>
        private UInt32 _Value;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Значение параметра
        /// </summary>
        public UInt32 Value
        {
            get 
            { 
                //return _Value;
                if (checkBoxChannelStatus.Checked)
                {
                    return OFF;
                }
                else
                {
                    return (UInt32)this.numericUpDownValue.Value;
                }
            }
            set 
            {
                if (value >= 10)
                {
                    if (value <= MAXVALUE)
                    {
                        // Устройство включено, устанавливаем новое значение
                        this.numericUpDownValue.Value = value;
                        this.numericUpDownValue.Enabled = true;
                        this.checkBoxChannelStatus.Checked = false;
                    }
                    else
                    {
                        if (value == OFF)
                        {
                            // Устройство выключено
                            _Value = value;
                            this.checkBoxChannelStatus.Checked = true;
                            this.numericUpDownValue.Enabled = false;
                        }
                        else
                        {
                            // значения больше MAXVALUE для данного поля не допустимо
                            throw new ArgumentException(
                                String.Format("Введённое больше значения {0} не дапустимы", MAXVALUE), 
                                "Value");
                        }
                    }
                }
                else
                {
                    // значения меньше 10 для данного поля не допустимо
                    throw new ArgumentException("Введённое значение меньше 10", "Value");
                }
            }
        }
        //------------------------------------------------------------------------------------------------------
        public Channel4_20Control()
        {
            InitializeComponent();

            this.numericUpDownValue.Minimum = 10;
            this.numericUpDownValue.Maximum = MAXVALUE;
            this.numericUpDownValue.Increment = 10;
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Событие возникающее при вводе значения пользователем
        /// </summary>
        public event NGK.MeasuringDeviceTech.Classes.MeasuringDevice.UITypeEditors.ValueChangedEventHandler ValueChanged;
        //------------------------------------------------------------------------------------------------------
        private void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                this.ValueChanged(this, new EventArgs());
            }
            return;
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Обработчик кнопки "Ок"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            OnValueChanged();
        }
        //------------------------------------------------------------------------------------------------------
        private void Channel4_20UITypeEditor_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    {
                        OnValueChanged();
                        break;
                    }
            }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события изменения состояния элемента checkBoxChannelStatus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxChannelStatus_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chbx = (CheckBox)sender;

            if (chbx.Checked)
            {
                this.numericUpDownValue.Enabled = false;
            }
            else
            {
                this.numericUpDownValue.Enabled = true;
            }
            return;
        }
        //------------------------------------------------------------------------------------------------------
    }
    //==========================================================================================================
}
//==============================================================================================================
// End of file