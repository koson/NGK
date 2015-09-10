using System;
using System.Collections.Generic;
using System.Text;

//========================================================================================
namespace NGK.CAN.ApplicationLayer.Network.Devices
{
    //====================================================================================
    /// <summary>
    /// Класс для сортировки устройств по статусу
    /// </summary>
    public class DeviceStatusComparer : System.Collections.IComparer
    {
        //--------------------------------------------------------------------------------
        int System.Collections.IComparer.Compare(object x, object y)
        {
            IDevice deviceX, deviceY;
            // returned value:
            // Less than zero: x is less than y.
            // Zero: x equals y.
            // Greater than zero: x is greater than y.

            deviceX = (IDevice)x;
            deviceY = (IDevice)x;

            // Чем меньше числовое значение статуса тем он приоритетней
            if ((Byte)deviceX.Status == (Byte)deviceY.Status)
            {
                return 0;
            }
            else if ((Byte)deviceX.Status < (Byte)deviceY.Status)
            {
                return 1;
            }
            else if ((Byte)deviceX.Status > (Byte)deviceY.Status)
            {
                return -1;
            }
            else
            {
                throw new Exception();
            }
        }
        //--------------------------------------------------------------------------------
    }
    //====================================================================================
}
//========================================================================================
// End Of File