using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using NLog;
using NGK.CAN.ApplicationLayer.Network.Master.Services;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.ApplicationLayer.Network.Devices;
using Common.Controlling;

namespace NGK.CAN.ApplicationLayer.Network.Master.Services
{
    /// <summary>
    /// ������� ������ Boot Up
    /// </summary>
    public sealed class ServiceBootUp: Service
    {
        #region Helper
        /// <summary>
        /// ��������� ��� �������������� ��������� ��� Boot Up �������
        /// �� ��������� ����������
        /// </summary>
        private struct IncomingMessageStuctureBootUp
        {
            #region Fields And Pproperties
            /// <summary>
            /// ����� ������ � ������
            /// </summary>
            internal int DL;
            internal Byte CobeId;
            internal Frame? Answer;
            /// <summary>
            /// ���������� true, ���� ��������� �������������
            /// ��� ������� �������
            /// </summary>
            internal bool IsForService
            {
                get
                {
                    // ����� ��� ��������� (Fct code: 4 ������� ����) �� ������ ���������� (7 ������� ���)
                    // ���������� � CobId ���������
                    const UInt16 MASK_FCT_CODE = 0x780; // 11110000000;
                    // ������������� CodId, �� �������� ������������ ��� �������� ���������
                    // ������������� ��� ������� �������
                    const UInt16 MASK = 0x700;

                    // �������� ������ � ������ ����� ��������� ������ ����������.
                    if ((Answer.Value.FrameType == FrameType.DATAFRAME) &&
                        (Answer.Value.Identifier & MASK_FCT_CODE) == MASK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            /// <summary>
            /// ���������� true, ���� ��������� ����� �������� ������ ���������
            /// </summary>
            internal bool HasIncorrectStructure
            {
                get 
                {
                    if (DL == 1)
                    {
                        if (Answer.Value.Data[0] == 0)
                        {
                            return false;
                        }
                        return true;
                    }
                    return true;
                }
            }
            #endregion

            #region Constructors
            #endregion

            #region Methods
            /// <summary>
            /// ��������� �������� ���������
            /// </summary>
            /// <param name="message">�������� ���������</param>
            /// <returns>��������� ������ ������</returns>
            internal static IncomingMessageStuctureBootUp Parse(Frame message)
            {
                const Byte MASK_COBEID = 0x7F; // �������� 7 ��� ���������� CodeId �� ���� Id 
                IncomingMessageStuctureBootUp frame = 
                    new IncomingMessageStuctureBootUp();
                frame.Answer = message;
                frame.CobeId = (Byte)(((Byte)message.Identifier) & MASK_COBEID);
                frame.DL = message.Data.Length;
                return frame;
            }
            #endregion
        }
        #endregion

        #region Fields And Properties
        //private static Logger _Logger = LogManager.GetLogger("BootUpLogger");

        protected override Logger Logger
        {
            get 
            {
                return null;
                //return _Logger; 
            }
        }

        public override ServiceType ServiceType
        {
            get { return ServiceType.BootUp; }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// �����������
        /// </summary>
        private ServiceBootUp(): base(null)
        {
            throw new NotImplementedException(
                "������� ������� ����������� ����������� ������ ServiceBootUp");
        }
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="controller"></param>
        public ServiceBootUp(INetworkController controller)
            : base(controller)
        { 
        }
        #endregion

        #region Methods
        /// <summary>
        /// ���������� �������� ���������
        /// </summary>
        public override void HandleOutcomingMessages()
        {
            // ������ ������ �� ����� ��������� ���������
            return;
        }
        /// <summary>
        /// ���������� �������� ��������� �� ����
        /// </summary>
        /// <param name="message">�������� ��������� ��� ���������</param>
        //private void HandleIncomingMessages(Frame message)
        public override void HandleIncomingMessages(Frame[] messages)
        {
            String msg;
            IncomingMessageStuctureBootUp msghelper;
            DeviceBase device;
       
            if (Status != Status.Running)
            {
                return;
            }

            foreach (Frame message in messages)
            {
                msghelper = IncomingMessageStuctureBootUp.Parse(message);

                if (!msghelper.IsForService)
                {
                    continue;
                }

                if (msghelper.HasIncorrectStructure)
                {
                    //!!!Warning: ����� ������ � ���� ������ ���������, �� ���������� �������
                    // (message.Data[0] == 0x84) || (message.Data[0] == 0x04) ||
                    // (message.Data[0] == 0x85) || (message.Data[0] == 0x05) ||
                    // (message.Data[0] == 0xFF) || (message.Data[0] == 0x7F))
                    // ��� ���� ������������ � ��������� NodeGuard.
                    continue;
                }

                // ��������� ��� ����� �������.
                //msg = String.Format("Network {0}: ������ {1}: Service BootUp ������ ���������: {2}",
                //    base.NetworkController.NetworkName(), this.ServiceName, message.ToString());
                //_Logger.Trace(msg);

                //���� ���������� ������� �������� ���������
                if (!_NetworkController.Devices.Contains(msghelper.CobeId))
                {
                    // ���������� �� �������
                    msg = String.Format(
                        "Network {0}: ������ ��������� �� ���������� � NodeId {1}, " +
                        "������ ���������� �� ���������������� � ����. Message - {2}",
                        this.NetworkController.NetworkName, msghelper.CobeId, message.ToString());
                    //Logger.Error(msg);
                    continue;
                }

                // ���������� �������. 
                // ������������� ����� ������ ����������
                device = _NetworkController.Devices[msghelper.CobeId];

                lock (_SyncRoot)
                {
                    device.Status = DeviceStatus.Preoperational;
                }
                // ����� � ������... �� �����������
            }
        }
        
        #endregion

        #region Events
        #endregion
    }
}
