using System;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.ComponentModel;

//===================================================================================
namespace Modbus.OSIModel.DataLinkLayer.Master.ConnectionUITypeEditor
{
    //===============================================================================
    /// <summary>
    /// Класс представляет редактор свойства реализующего 
    /// интерфейс IDataLinkLayer
    /// </summary>
    public class ConnectionUITypeEditor : UITypeEditor
    {
        //---------------------------------------------------------------------------
        private IWindowsFormsEditorService edSvc = null;
        //---------------------------------------------------------------------------
        public override object EditValue(ITypeDescriptorContext context, 
            IServiceProvider provider, object value)
        {
            if (context != null && context.Instance != null && provider != null)
            { 
                // Получаем интерфейс сервиса
                edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                
                if (edSvc != null)
                {
                    IDataLinkLayer current;
                    FormConnectionEditor frm;
                    // Получаем текущий редактируемый компонент или создаём его
                    if (context.Instance != null)
                    {
                        Modbus.WCF.NetworksServer.Network.Network network =
                            (Modbus.WCF.NetworksServer.Network.Network)context.Instance;
                        current = (IDataLinkLayer)network.Connection;
                        if (current != null)
                        {
                            if (current.IsOpen())
                            {
                                current.CloseConnect();
                            }
                        }
                        // Создаём форму для редактирования
                        frm = new FormConnectionEditor();
                        frm.Connection = current;
                    }
                    else
                    {
                        frm = new FormConnectionEditor();
                    }
                    
                    //DialogResult result = frm.ShowDialog();
                    DialogResult result = edSvc.ShowDialog(frm);

                    if (result == DialogResult.OK)
                    {
                        value = frm.Connection;
                    }
                    
                    frm.Dispose();
                }
            }
            return value;
        }
        //---------------------------------------------------------------------------
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        //---------------------------------------------------------------------------
        public override void PaintValue(PaintValueEventArgs e)
        {
            base.PaintValue(e);
        }
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file