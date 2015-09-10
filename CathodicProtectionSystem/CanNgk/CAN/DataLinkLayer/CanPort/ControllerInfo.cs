using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

//========================================================================================
namespace NGK.CAN.DataLinkLayer.CanPort
{
    //====================================================================================
    /// <summary>
    /// Структура хранит данные о контроллере CAN-порта
    /// </summary>
    [Browsable(true)]
    //public class ControllerInfo
    public struct ControllerInfo
    {
        //--------------------------------------------------------------------------------
        //public Ixxat.Vci3.VciBusType BusType;
        public String BusType;
        //private String _BusType;
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Тип интерфейса (физический уровень OSI-модели стека протоколов)
        /// </summary>
        //[Description("Тип интерфейса (физический уровень OSI-модели стека протоколов)")]
        //[Category("Оборудование")]
        //[DisplayName("Интерфейс")]
        //[Browsable(true)]
        //[ReadOnly(true)]
        //public String BusType
        //{
        //    get { return _BusType; }
        //    set { _BusType = value; }
        //}
        //--------------------------------------------------------------------------------
        //public Ixxat.Vci3.Bal.Can.CanCtrlType ControllerType;
        public String ControllerType;
        //private String _ControllerType;
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Тип микросхемы (наименование) реализующая физический уровень (тип интерфейса)
        /// </summary>
        //[Description("Тип микросхемы (наименование) реализующая физический уровень (тип интерфейса)")]
        //[Category("Оборудование")]
        //[DisplayName("Микросхема")]
        //[Browsable(true)]
        //[ReadOnly(true)]
        //public String ControllerType
        //{
        //    get { return _ControllerType; }
        //    set { _ControllerType = value; }
        //}
        //--------------------------------------------------------------------------------
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("Микросхема: {0}", this.ControllerType));
            sb.Append("; ");
            sb.Append(String.Format("Интерфейс: {0}", this.BusType));
            return sb.ToString();
            //return base.ToString();
        }
        //--------------------------------------------------------------------------------
    }
    //====================================================================================
}
//========================================================================================
// End Of File