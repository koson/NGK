using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
//==============================================================================================================
namespace NGK.MeasuringDeviceTech.Classes.MeasuringDevice.UITypeEditors
{
    //==========================================================================================================
    /// <summary>
    /// Редактор типа для значений измерительных каналов 4-20 мА
    /// </summary>
    public class Channel4_20TypeEditor : System.Drawing.Design.UITypeEditor
    {
        private System.Windows.Forms.Design.IWindowsFormsEditorService service;
        //------------------------------------------------------------------------------------------------------
        public override UITypeEditorEditStyle GetEditStyle(
            ITypeDescriptorContext context)
        {
            // Задаём модальный режим для формы
            if ((context != null) && (context.Instance != null))
            {
                return UITypeEditorEditStyle.DropDown;
            }
            else
            {
                return base.GetEditStyle(context);
            }
        }
        //------------------------------------------------------------------------------------------------------
        public override object EditValue(
            ITypeDescriptorContext context, 
            IServiceProvider provider, 
            object value)
        {
            if ((context != null) && (context.Instance != null) && (provider != null))
            {
                // Получаем интерфейс сервиса
                service =
                    (System.Windows.Forms.Design.IWindowsFormsEditorService)provider.GetService(
                    typeof(System.Windows.Forms.Design.IWindowsFormsEditorService));

                if (service != null)
                { 
                    Channel4_20Control cntrl =
                        new Channel4_20Control();
                    cntrl.ValueChanged +=
                        new  MeasuringDeviceTech.Classes.MeasuringDevice.UITypeEditors.ValueChangedEventHandler(cntrl_ValueChanged);
                    cntrl.Value = (UInt32)value;
                    service.DropDownControl(cntrl);
                    value = cntrl.Value;
                }
            }
            else
            {
                return base.EditValue(context, provider, value);
            }

            return value;
        }
        //------------------------------------------------------------------------------------------------------
        void cntrl_ValueChanged(object sender, EventArgs args)
        {
            if (service != null)
            {
                service.CloseDropDown();
            }
        }
        //------------------------------------------------------------------------------------------------------
    }
    //==========================================================================================================
}
//==============================================================================================================
// End of file