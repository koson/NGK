using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.DataLinkLayer.CanPort;
using NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver;

namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Convert
{
    public static class ConvertNim351
    {
        /// <summary>
        /// ����������� �������� ���� F_CAN_BAUDRATE � �������� 
        /// </summary>
        /// <param name="baudRate">�������� ���� F_CAN_BAUDRATE</param>
        /// <returns>�������� ���� NGK.CAN.OSIModel.DataLinkLayer.CanPort.BaudRate</returns>
        public static BaudRate ConvertToBaudRate(F_CAN_BAUDRATE baudRate)
        {
            BaudRate _baudRate;
            
            switch (baudRate)
            {
                case F_CAN_BAUDRATE.CANBR_10kBaud:
                    {
                        _baudRate = BaudRate.BR10;
                        break;
                    }
                case F_CAN_BAUDRATE.CANBR_20kBaud:
                    {
                        _baudRate = BaudRate.BR20;
                        break;
                    }
                case F_CAN_BAUDRATE.CANBR_50kBaud:
                    {
                        _baudRate = BaudRate.BR50;
                        break;
                    }
                case F_CAN_BAUDRATE.CANBR_100kBaud:
                    {
                        _baudRate = BaudRate.BR100;
                        break;
                    }
                case F_CAN_BAUDRATE.CANBR_125kBaud:
                    {
                        _baudRate = BaudRate.BR125;
                        break;
                    }
                case F_CAN_BAUDRATE.CANBR_250kBaud:
                    {
                        _baudRate = BaudRate.BR250;
                        break;
                    }
                case F_CAN_BAUDRATE.CANBR_500kBaud:
                    {
                        _baudRate = BaudRate.BR500;
                        break;
                    }
                case F_CAN_BAUDRATE.CANBR_800kBaud:
                    {
                        _baudRate = BaudRate.BR800;
                        break;
                    }
                case F_CAN_BAUDRATE.CANBR_1MBaud:
                    {
                        _baudRate = BaudRate.BR1000;
                        break;
                    }
                default:
                    {
                        throw new InvalidCastException(
                            "��������� ������������� �������� ���� F_CAN_BAUDRATE � BaudRate. " +
                            "�� ������� ������������");
                    }
            }
            return _baudRate;
        }
        /// <summary>
        /// ����������� �������� NGK.CAN.OSIModel.DataLinkLayer.CanPort.BaudRate � ��������
        /// F_CAN_BAUDRATE
        /// </summary>
        /// <param name="baudRate">�������� ���� BaudRate</param>
        /// <returns>�������� ���� F_CAN_BAUDRATE</returns>
        public static F_CAN_BAUDRATE ConvertToF_CAN_BAUDRATE(BaudRate baudRate)
        {
            F_CAN_BAUDRATE f_baudRate;

            switch (baudRate)
            {
                case BaudRate.BR10:
                    {
                        f_baudRate = F_CAN_BAUDRATE.CANBR_10kBaud;
                        break;
                    }
                case BaudRate.BR20:
                    {
                        f_baudRate = F_CAN_BAUDRATE.CANBR_20kBaud;
                        break;
                    }
                case BaudRate.BR50:
                    {
                        f_baudRate = F_CAN_BAUDRATE.CANBR_50kBaud;
                        break;
                    }
                case BaudRate.BR100:
                    {
                        f_baudRate = F_CAN_BAUDRATE.CANBR_100kBaud;
                        break;
                    }
                case BaudRate.BR125:
                    {
                        f_baudRate = F_CAN_BAUDRATE.CANBR_125kBaud;
                        break;
                    }
                case BaudRate.BR250:
                    {
                        f_baudRate = F_CAN_BAUDRATE.CANBR_250kBaud;
                        break;
                    }
                case BaudRate.BR500:
                    {
                        f_baudRate = F_CAN_BAUDRATE.CANBR_500kBaud;
                        break;
                    }
                case BaudRate.BR800:
                    {
                        f_baudRate = F_CAN_BAUDRATE.CANBR_800kBaud;
                        break;
                    }
                case BaudRate.BR1000:
                    {
                        f_baudRate = F_CAN_BAUDRATE.CANBR_1MBaud;
                        break;
                    }
                default:
                    {
                        throw new InvalidCastException(
                            "��������� ������������� �������� ���� BaudRate � F_CAN_BAUDRATE. �� ������� ������������");
                    }
            }
            return f_baudRate;
        }
        /// <summary>
        /// ����������� ��������� ������ ������� � ��������� ������� NIM351
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static F_CAN_MSG ConvertToF_CAN_MSG(Frame message)
        {
            F_CAN_MSG result;
            //String msg;

            // ��������� ��������� � �������������� ��� ��� ��������
            result.can_dlc = (Byte)message.Data.Length;
            result.data = new Byte[8];
            Array.Copy(message.Data, result.data, message.Data.Length);

            result.can_id = message.Identifier;
            //buffer.msg.can_id &= (CAN_MSG_MASK.CAN_SFF_MASK | CAN_MSG_MASK.CAN_EFF_MASK);

            if (message.FrameFormat == FrameFormat.ExtendedFrame)
            {
                result.can_id |= CAN_MSG_FLAG.CAN_EFF_FLAG;
            }

            if (message.FrameType == FrameType.REMOTEFRAME)
            {
                //if (message.Data.Length != 0)
                //{
                //    // ��� �������� ��������� �������� ������, ��� 
                //    // ������������� ���� RTR, ����� DLC ������ ������ ���� ����� 0
                //    msg = "�� ������� ��������� ���������. DLC ������ ���� ����� 0 ��� ������������� ���� RTR";
                //    throw new Exception(msg);
                //}
                result.can_id |= CAN_MSG_FLAG.CAN_RTR_FLAG;
            }

            return result; 
        }
        /// <summary>
        /// ����������� ��������� NIM351 � ��������� ������ �������
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Frame ConvertToFrame(F_CAN_MSG message)
        {
            Frame resultMsg;
            //String msg;

            resultMsg = new Frame();

            if (CAN_MSG_FLAG.CAN_EFF_FLAG == (message.can_id & CAN_MSG_FLAG.CAN_EFF_FLAG))
            {
                // ������ ���� ������������ �������
                resultMsg.FrameFormat = FrameFormat.ExtendedFrame;
                resultMsg.Identifier = message.can_id & CAN_MSG_MASK.CAN_EFF_MASK;
            }
            else
            {
                // ������ ����������� ����
                resultMsg.FrameFormat = FrameFormat.StandardFrame;
                resultMsg.Identifier = message.can_id & CAN_MSG_MASK.CAN_SFF_MASK;
            }

            if (CAN_MSG_FLAG.CAN_RTR_FLAG == 
                (message.can_id & CAN_MSG_FLAG.CAN_RTR_FLAG))
            {
                resultMsg.FrameType = FrameType.REMOTEFRAME;
                
                //if (message.can_dlc != 0)
                //{
                //    // �������� ��������� ����� ������������ ������. 
                //    // ��� ������������� ���� RTR, ����� DLC ������ ������ ���� ����� 0
                //    msg = String.Format(
                //        "�� ������������� ���������. DLC ������ ���� ����� 0 ��� ������������� ���� RTR. " +
                //        "�������� ���������: ID={0}; DLC={1}", message.can_id, message.can_dlc);
                //    throw new InvalidCastException(msg);
                //}
            }
            else
            {
                resultMsg.FrameType = FrameType.DATAFRAME;
            }
            
            resultMsg.Data = new Byte[message.can_dlc];
            Array.Copy(message.data, resultMsg.Data, message.can_dlc);
            
            return resultMsg;
        }
        /// <summary>
        /// ����������� ��������� CAN-�������� NIM-351 � ���������� ���������
        /// </summary>
        /// <param name="state">��������� CAN-�������� NIM-351</param>
        /// <returns>����� ��������� ��� CAN-������</returns>
        public static CanPortStatus ConvertToCanPortStatus(F_CAN_STATE state)
        {
            String msg;

            switch (state)
            {
                case F_CAN_STATE.CAN_STATE_BUS_OFF:
                    {
                        return CanPortStatus.IsPassive;
                    }
                case F_CAN_STATE.CAN_STATE_ERROR_ACTIVE:
                    {
                        return CanPortStatus.IsActive;
                    }
                case F_CAN_STATE.CAN_STATE_ERROR_PASSIVE:
                    {
                        return CanPortStatus.IsActive;
                    }
                case F_CAN_STATE.CAN_STATE_ERROR_WARNING:
                    {
                        return CanPortStatus.IsActive;
                    }
                case F_CAN_STATE.CAN_STATE_INIT:
                    {
                        return CanPortStatus.IsPassive;
                    }
                case F_CAN_STATE.CAN_STATE_STOPPED:
                    {
                        return CanPortStatus.IsClosed;
                    }
                case F_CAN_STATE.CAN_STATE_SLEEPING:
                    {
                        throw new NotImplementedException(
                            "�� �������������� �������������� ����������");
                    }
                default:
                    {
                        msg = String.Format(
                            "�������������� {0} � �������� CANPORTSTATUS �� ��������������", state.ToString());
                        throw new InvalidCastException(msg);
                    }
            }
        }
    }
}
