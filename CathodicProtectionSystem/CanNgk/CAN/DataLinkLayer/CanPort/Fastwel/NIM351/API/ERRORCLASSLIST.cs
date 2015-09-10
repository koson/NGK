using System;
using System.Collections.Generic;
using System.Text;

//=================================================================================================
namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //=============================================================================================
    // ����� ���� ������ ��������� �� ������
    //#define CAN_ERR_DLC 8 /* dlc for error frames */
    //=============================================================================================
    /// <summary>
    ///  ���� ������ ������������ � ID-����. 
    /// </summary>
    [Flags]
    [Serializable]
    public enum ERRORCLASSLIST : uint
    {
        /// <summary>
        /// ����������� ������ :)
        /// </summary>
        CAN_ERR_UNSPEC = 0x00000000U, /* unspecified */
        /// ������� ��������
        CAN_ERR_TX_TIMEOUT = 0x00000001U, /* TX timeout / data[0..3] */
        // �������� ��������� (data[0])
        CAN_ERR_LOSTARB = 0x00000002U, /* lost arbitration    / data[0]    */
        // ���������� ���� �������� (data[1])
        CAN_ERR_CRTL = 0x00000004U, /* controller problems / data[1]    */
        // ��������� ��������� (data[2..3])
        // ��������! ����� ��������� ����� ����� � �����!
        CAN_ERR_PROT = 0x00000008U, /* protocol violations (may flood!) / data[2..3] */
        // ������ ���������� (data[4]) ???
        CAN_ERR_TRX = 0x00000010U, /* transceiver status  / data[4]    */
        // �� ������� ACK �� ���������� ���������
        CAN_ERR_ACK = 0x00000020U, /* received no ACK on transmission */
        // Bus Off
        CAN_ERR_BUSOFF = 0x00000040U, /* bus off */
        // ������ �� ���� 
        // ��������! ����� ��������� ����� ����� � �����!
        CAN_ERR_BUSERROR = 0x00000080U, /* bus error (may flood!) */
        // ������� �����������
        CAN_ERR_RESTARTED = 0x00000100U /* controller restarted */
    }
    //=============================================================================================
}
//=================================================================================================
// End of file