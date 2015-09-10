using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using NGK.CAN.DataLinkLayer.CanPort.Fastwel;
using NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351;
using NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver;

//========================================================================================
namespace NGK.CAN.DataLinkLayer.CanPort.Design.Controls
{
    //====================================================================================
    /// <summary>
    /// Контрол для визуального редактирования свойств CAN-порта Fastwel NIM-351 
    /// </summary>
    public partial class FastwelNIM351PortTuner : UserControl, ICanPortEditor
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Fields And Properties
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private CanPort.Fastwel.NIM351.CanPort _CanPort;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// CAN-порт
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [DisplayName("CAN-порт")]
        [Category("Аппаратура")]
        [Description("CAN порт NIM-351 производства Fastwel")]
        public CanPort.Fastwel.NIM351.CanPort CanPort
        {
            get { return _CanPort; }
            set 
            {
                _CanPort = value;

                PropertyGrid control = (PropertyGrid)this.Controls["PropertyGridPortSettings"];
                
                if (control != null)
                {
                    control.SelectedObject = null;
                    control.SelectedObject = _CanPort;
                }
            }
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #endregion
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Constructors And Destructors
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public FastwelNIM351PortTuner()
        {
            InitializeComponent();
            this.Load += 
                new EventHandler(EventHandler_FastwelNIM351PortTuner_Load);
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #endregion
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Methods
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обработчик загрузки формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_FastwelNIM351PortTuner_Load(object sender, EventArgs e)
        {
            this.Controls.Clear();

            PropertyGrid control = new PropertyGrid();
            control.Name = "PropertyGridPortSettings";
            control.Dock = DockStyle.Fill;
            this.Controls.Add(control);
            
            // Устанавливаем CAN-порт для его редактирования 
            // Если порт не задан, создаём новый
            if (this._CanPort == null)
            {
                this._CanPort = new Fastwel.NIM351.CanPort();
            }
            control.SelectedObject = null;
            control.SelectedObject = this._CanPort;
            
            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #endregion
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region ICanPortEditor Members
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        ICanPort ICanPortEditor.CanPort
        {
            get
            {
                return (ICanPort)this.CanPort;
            }
            set
            {
                if (value is Fastwel.NIM351.CanPort)
                {
                    this.CanPort = (Fastwel.NIM351.CanPort)value;
                }
                else
                {
                    // Это CAN-порт другого производителя
                    this.CanPort = null;
                }
            }
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        bool ICanPortEditor.CanEditCanPort(ICanPort canPort)
        {
            if (canPort is Fastwel.NIM351.CanPort)
            {
                return true;
            }
            else
            {
                // Это CAN-порт другого производителя
                return false;
            }           
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #endregion
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    }
    //====================================================================================
}
//========================================================================================
// End Of File