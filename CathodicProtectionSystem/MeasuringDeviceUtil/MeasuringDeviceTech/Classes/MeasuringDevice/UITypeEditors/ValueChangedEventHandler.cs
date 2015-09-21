using System;
using System.Collections.Generic;
using System.Text;

//==============================================================================================================
namespace NGK.MeasuringDeviceTech.Classes.MeasuringDevice.UITypeEditors
{
    //==========================================================================================================
    /// <summary>
    /// Делегат для создания события изменения свойства в классах редакторов типов
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void ValueChangedEventHandler(object sender, EventArgs args);
    //==========================================================================================================
}
//==============================================================================================================
// End of file