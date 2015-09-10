using System;
using System.Collections.Generic;
using System.Text;

//=======================================================================================
namespace Modbus.OSIModel.DataLinkLayer.Master.RTU.SerialPort.UITypeEditor
{
    //===================================================================================
    /// <summary>
    /// Редактор типа для Master SerialPort
    /// </summary>
    public class SerialPortUITypeEditor: System.Drawing.Design.UITypeEditor
    {
        //-------------------------------------------------------------------------------
        #region Fields and Properties
        //-------------------------------------------------------------------------------
        private System.Windows.Forms.Design.IWindowsFormsEditorService service;
        //-------------------------------------------------------------------------------
        #endregion
        //-------------------------------------------------------------------------------
        #region Methods
        //-------------------------------------------------------------------------------
        public override object EditValue(
            System.ComponentModel.ITypeDescriptorContext context, 
            IServiceProvider provider, 
            object value)
        {
            if ((context != null) && (context.Instance != null) && (provider != null))
            {
                service =
                    (System.Windows.Forms.Design.IWindowsFormsEditorService)provider.GetService(
                    typeof(System.Windows.Forms.Design.IWindowsFormsEditorService));

                if (service != null)
                {
                    if (context.Instance is IDataLinkLayer)
                    {
                        System.IO.Ports.SerialPort port = (System.IO.Ports.SerialPort)value; 
                        SerialPortSettings cntr =
                            new SerialPortSettings(port);
                        cntr.EditingIsComplete += new EditingIsCompleteEventHandler(cntr_EditingIsComplete);

                        if (port.IsOpen)
                        {
                            // Запрещаем редактирование
                            cntr.Enabled = false;
                        }
                        
                        service.DropDownControl(cntr);
                        value = cntr.SerialPort;
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            "Объект для которого вызывается редактор не яввляется IDataLinkLayer");
                    }
                }
            }
            else
            {
                return base.EditValue(context, provider, value);
            }
            return value;
        }
        //-------------------------------------------------------------------------------
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(
            System.ComponentModel.ITypeDescriptorContext context)
        {
            if ((context != null) && (context.Instance != null))
            {
                return System.Drawing.Design.UITypeEditorEditStyle.DropDown;
            }
            else
            {
                return base.GetEditStyle(context);
            }
        }
        //-------------------------------------------------------------------------------
        private void cntr_EditingIsComplete(
            object sender, 
            EventArgs args)
        {
            service.CloseDropDown();
        }
        //-------------------------------------------------------------------------------
        #endregion
        //-------------------------------------------------------------------------------
    }
    //===================================================================================
}
//=======================================================================================
// End Of File