using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NGK.CAN.DataLinkLayer.CanPort;
using NGK.CAN.DataLinkLayer.CanPort.IXXAT;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.ApplicationLayer.Network.Master;
using NGK.CAN.ApplicationLayer.Network.Master.Collections;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles;

namespace Test.ControllerTest
{
    public static class NetworkControllerTest
    {
        public static void NetworkTest()
        {
            CanPort port = new CanPort("HW318371");
            port.BitRate = BaudRate.BR10;
            port.FrameFormat = FrameFormat.StandardFrame;
            port.Mode = PortMode.NORMAL;

            NetworkController controller = new NetworkController(port, 1);
            controller.Devices.Add(Device.Create(DeviceType.KIP_MAIN_POWERED_v1));
            controller.Start();
            return;
        }

        public static void SerializeNetworkControllerCollectionTest()
        {
            CanPort port = new CanPort("HW318371");
            port.BitRate = BaudRate.BR10;
            port.FrameFormat = FrameFormat.StandardFrame;
            port.Mode = PortMode.NORMAL;

            NetworkController controller = new NetworkController(port, 1);
            controller.Devices.Add(Device.Create(DeviceType.KIP_MAIN_POWERED_v1));

            NetworkControllersCollection collection = new NetworkControllersCollection();
            collection.Add(controller);

            // Сериализуем в файл
            using (FileStream fs = new FileStream("serializedNetworks.bin", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, collection);
            }
        }
    }
}
