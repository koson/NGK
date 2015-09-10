using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;

//========================================================================================
namespace NGK.CAN.DataLinkLayer.CanPort.Design
{
    //====================================================================================
    /// <summary>
    /// Редактор типа для ICanPort
    /// </summary>
    public class ICanPortUITypeEditor: UITypeEditor
    {
        //--------------------------------------------------------------------------------
        #region Feilds And Propeties
        //--------------------------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------------------------
        #region Constructors
        //--------------------------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------------------------
        public override object EditValue(ITypeDescriptorContext context, 
            IServiceProvider provider, 
            object value)
        {
            if (context != null && context.Instance != null && provider != null)
            {
                // Получаем интерфейс сервиса
                System.Windows.Forms.Design.IWindowsFormsEditorService edSvc =
                    (System.Windows.Forms.Design.IWindowsFormsEditorService)provider.GetService(
                    typeof(System.Windows.Forms.Design.IWindowsFormsEditorService));
                if (edSvc != null)
                {
                    ICanPortUITypeEditorDialogForm form = new ICanPortUITypeEditorDialogForm();
                    // Устанавливаем текущее значение для редактирования
                    form.ICanPort = (ICanPort)value;
                    // Вызываем окно редактора
                    System.Windows.Forms.DialogResult result = edSvc.ShowDialog(form);
                    // Получаем новое значение
                    value = form.ICanPort;                   
                }
            }
            //return base.EditValue(context, provider, value);
            return value;
        }
        //--------------------------------------------------------------------------------
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
            //return base.GetEditStyle(context);
        }
        //--------------------------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------------------------
    }
    //====================================================================================
}
//========================================================================================
// End Of File