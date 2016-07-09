using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.ApplicationLayer.Network.Master.Services;
using NGK.CAN.DataLinkLayer.Message;

namespace Test
{
    public static class SDOServiceTest
    {
        public static void Test()
        {
            Frame msg = new Frame();
            msg.Identifier = 0x586;
            msg.FrameType = FrameType.DATAFRAME;
            msg.FrameFormat = FrameFormat.StandardFrame;
            msg.Data = new byte[] { 0x43, 0x31, 0x20, 0x00, 0xC5, 0x44, 0x81, 0x57 };

            NGK.CAN.ApplicationLayer.Network.Master.Services.ServiceSdoUpload.IncomingMessageStuctureSdo result
                = ServiceSdoUpload.IncomingMessageStuctureSdo.Parse(msg);
        }
    }
}
