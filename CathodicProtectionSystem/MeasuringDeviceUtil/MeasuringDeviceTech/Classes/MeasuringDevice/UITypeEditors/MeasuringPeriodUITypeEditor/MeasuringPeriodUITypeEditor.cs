using System;
using System.Collections.Generic;
using System.Text;

//==============================================================================================================
namespace NGK.MeasuringDeviceTech.Classes.MeasuringDevice.UITypeEditors
{
    //==========================================================================================================
    public class MeasuringPeriodUITypeEditor: System.Drawing.Design.UITypeEditor
    {
        //------------------------------------------------------------------------------------------------------
        private System.Windows.Forms.Design.IWindowsFormsEditorService service;
        //------------------------------------------------------------------------------------------------------
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
                    if (context.Instance is MeasuringDeviceMainPower)
                    {
                        MeasuringPeriodMainPowerControl cntr =
                            new MeasuringPeriodMainPowerControl(TYPE_NGK_DEVICE.BI_MAIN_POWERED);
                        cntr.ValueChanged += new ValueChangedEventHandler(cntr_ValueChanged);
                        cntr.Value = (UInt32)value;
                        service.DropDownControl(cntr);
                        value = cntr.Value;
                    }
                    else if (context.Instance is MeasuringDeviceBatteryPower)
                    {
                        MeasuringPeriodMainPowerControl cntr =
                            new MeasuringPeriodMainPowerControl(TYPE_NGK_DEVICE.BI_BATTERY_POWER);
                        cntr.ValueChanged += new ValueChangedEventHandler(cntr_ValueChanged);
                        cntr.Value = (UInt32)value;
                        service.DropDownControl(cntr);
                        value = cntr.Value;
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            "Объект для которого вызывается редактор не яввляется устройством БИ");
                    }
                }
            }
            else
            {
                return base.EditValue(context, provider, value);
            }
            return value;
        }
        //------------------------------------------------------------------------------------------------------
        void cntr_ValueChanged(object sender, EventArgs args)
        {
            service.CloseDropDown();
        }
        //------------------------------------------------------------------------------------------------------
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
        //------------------------------------------------------------------------------------------------------
    }
    //==========================================================================================================
}
//==============================================================================================================
// End of file