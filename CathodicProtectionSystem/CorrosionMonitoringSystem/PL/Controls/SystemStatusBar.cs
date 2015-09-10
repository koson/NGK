using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

//========================================================================================
namespace NGK.CorrosionMonitoringSystem.Forms.Controls
{
    //====================================================================================
    /// <summary>
    /// Класс для создания строки состояния системы коррозионной защиты
    /// со строгой типизацией
    /// </summary>
    public class SystemStatusBar: StatusStrip
    {
        #region Fields And Properties
        /// <summary>
        /// Общее количество устройств в системе
        /// </summary>
        private Int32 _TotalDevices;
        /// <summary>
        /// Общее количество устройств в системе
        /// </summary>
        public Int32 TotalDevices
        {
            get { return _TotalDevices; }
            set 
            {
                _TotalDevices = value;
                this._ToolStripLabelTotalDevices.Text =
                String.Format("Всего устройств: {0}", value);
            }
        }
        /// <summary>
        /// Общее количество неисправных устройств
        /// </summary>
        private Int32 _FaultyDevices;
        /// <summary>
        /// Общее количество неисправных устройств
        /// </summary>
        public Int32 FaultyDevices
        {
            get { return _FaultyDevices; }
            set 
            {
                ToolStripButton btn = (ToolStripButton)this._ToolStripButtonFaultyDevices;
                
                _FaultyDevices = value;
                
                btn.Text = String.Format("Неисправных устройств: {0}", value);
                
                if (value == 0)
                {
                    btn.BackColor = this.BackColor;                   
                }
                else if (value > 0)
                {
                    btn.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("FaultyDevices", 
                        "Значения меньше нуля недопустимы");
                }
            }
        }
        public DateTime DateTime
        {
            set 
            {
                this._ToolStripLabelDateTime.Text = 
                    value.ToString(new System.Globalization.CultureInfo("ru-Ru"));
            }
        }
        /// <summary>
        /// Элемент строки состояния для отобржения общего числа устройств в системе
        /// </summary>
        private ToolStripStatusLabel _ToolStripLabelTotalDevices;
        /// <summary>
        /// Кнопка строки состояния для отображения неиспаравных устройств в системе
        /// </summary>
        private ToolStripButton _ToolStripButtonFaultyDevices;
        /// <summary>
        /// Элемент строки состояния для отображения системного времени и даты
        /// </summary>
        private ToolStripStatusLabel _ToolStripLabelDateTime;
        
        #endregion

        #region Constructors
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public SystemStatusBar()
        {
            this._ToolStripLabelTotalDevices = new ToolStripStatusLabel();
            this._ToolStripLabelTotalDevices.Name = "ToolStripLabelTotalDevices";
            this._ToolStripLabelTotalDevices.AutoSize = true;
            this._ToolStripLabelTotalDevices.ToolTipText = "Отображает общее количество устройств в системе";
            this._ToolStripLabelTotalDevices.AutoToolTip = true;
            this._ToolStripLabelTotalDevices.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this._ToolStripLabelTotalDevices.BorderSides = ToolStripStatusLabelBorderSides.All;

            this._ToolStripButtonFaultyDevices = new ToolStripButton();
            this._ToolStripButtonFaultyDevices.Name = "ToolStripButtonFaultyDevices"; 
            this._ToolStripButtonFaultyDevices.AutoSize = true;
            this._ToolStripButtonFaultyDevices.ToolTipText = "Общее количество неисправных устройств в системе";
            this._ToolStripButtonFaultyDevices.AutoToolTip = true;
            this._ToolStripButtonFaultyDevices.DisplayStyle = ToolStripItemDisplayStyle.Text;

            this._ToolStripLabelDateTime = new ToolStripStatusLabel();
            this._ToolStripLabelDateTime.Name = "ToolStripLabelDateTime";
            this._ToolStripLabelDateTime.AutoSize = true;
            this._ToolStripLabelDateTime.ToolTipText = "Системная дата и время";
            this._ToolStripLabelDateTime.AutoToolTip = true;
            this._ToolStripLabelDateTime.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this._ToolStripLabelDateTime.BorderSides = ToolStripStatusLabelBorderSides.All;

            this.Items.AddRange(new ToolStripItem[] { this._ToolStripLabelTotalDevices,
                this._ToolStripButtonFaultyDevices, this._ToolStripLabelDateTime });

            this.TotalDevices = 0;
            this.FaultyDevices = 0;
        }
        //--------------------------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------------------------
    }
    //====================================================================================
}
//========================================================================================
// End Of File