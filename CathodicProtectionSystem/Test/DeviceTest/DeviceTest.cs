using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NGK.CAN.ApplicationLayer.Network.Devices;

namespace Test.DeviceTest
{
    public static class DeviceClassTest
    {
        public static void ToStringTest()
        {
            Device device = 
                Device.Create(DeviceType.KIP_MAIN_POWERED_v1);
            //string str = device.ToString();
            Console.WriteLine(device.ToString());
        }

        public static void CreateFromStringTest()
        {
            //Type={0}; Network={1}; Address={2}; Location={3}; PollingInterval={4}
            
            string str = "Type=KIP_MAIN_POWERED_v1; Network=; Address=1; " +
                "Location=; PollingInterval=1";
            Device device = Device.Create(str);
        }
    }
}
