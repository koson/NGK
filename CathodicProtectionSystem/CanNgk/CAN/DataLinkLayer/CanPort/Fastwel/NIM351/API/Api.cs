using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Text.RegularExpressions;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.DataLinkLayer.CanPort;

//=================================================================================================
namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //=============================================================================================
    public static class Api
    {
        //-----------------------------------------------------------------------------------------
        // Function		 :  F_CAN_RESULT fw_can_init(void)
        // Parameters	 : 
        // Return value  :  ��������� ������������� ���������� ���������
        //                  CAN_RES_OK    - �����
        //                  CAN_RES_XX    - ��������, �������� �� CAN_RES_OK, ��������������� �� ������
        // Description	 :  �������������� ���������� ��������� ��� ����������� ��������.
        // ������ ������� ������ ���� ������� � ������ ���������, � ������� ��������������
        // ����������������� � ���������.           
        //
        /// <summary>
        /// �������������� ���������� ��������� ��� ����������� ��������.
        /// ������ ������� ������ ���� ������� � ������ ���������, � ������� ��������������
        /// ����������������� � ���������.
        /// </summary>
        /// <returns>��������� ���������� ��������</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_init")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_init();
        //-----------------------------------------------------------------------------------------
        // Function		 :  F_CAN_RESULT fw_can_open(F_CAN_DEVID id, PF_CAN_HANDLE phDev)
        // Parameters	 :  id - ������������� (������) ��������, ������� � 1
        //               :  phDev - ��������� �� ���������� ���� F_CAN_HANDLE, � �������, ��� ��������
        //                  �������� ��������, �������� ����� ����������. ������������ ������ ����� ���������
        //                  ��� ������ ������� fw_can_is_handle_valid:
        //                  PF_CAN_HANDLE handle;
        // if(CAN_RES_OK == fw_can_open(1, &handle, true) && fw_can_is_handle_valid(handle))
        // {
        //      //���������� � �������� 1 ������� �������
        //      fw_can_close(handle);
        // }
        // Return value : 
        // CAN_RES_OK - �����;
        // ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
        // Description	 :  ������� ��������� ������� � �������� ��������������� (��������).
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">������������� (������) ��������, ������� � 1</param>
        /// <param name="phDev">��������� �� ���������� ���� F_CAN_HANDLE, � �������, ��� ��������
        ///                  �������� ��������, �������� ����� ����������</param>
        /// <returns>��������� ���������� ��������</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_open")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_open(
            Int32 id,
            out SafeFileHandle phDev);
        //-----------------------------------------------------------------------------------------
        // Function	    :  F_CAN_RESULT fw_can_close(F_CAN_HANDLE hDev)
        // Parameters	:  hDev - ����� ������������ ��������
        // Return value : CAN_RES_OK - �����;
        //                ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
        // Description	 :  ��������� ����� ��������, ����� �������� ��� ������ fw_can_open().
        // ����� ������� ������ hDev �� ����� �������������� ��� ������� � ��������.
        /// <summary>
        /// ��������� ����� ��������, ����� �������� ��� ������ fw_can_open().
        /// ����� ������� ������ hDev �� ����� �������������� ��� ������� � ��������.
        /// </summary>
        /// <param name="hDev">����� ������������ ��������</param>
        /// <returns>��������� ���������� ��������</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_close")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_close(SafeFileHandle hDev);
        //-----------------------------------------------------------------------------------------
        // Function     : F_CAN_RESULT fw_can_get_controller_config(F_CAN_HANDLE hDev, F_CAN_SETTINGS pDcb)
        // Parameters   : hDev - ����� ��������
        //              : pDcb - ��������� �� ��������� ���������� CAN-��������
        // Return value : CAN_RES_OK - �����;
        //                ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
        // Description  : ���������� ������� ��������� CAN-��������.
        /// <summary>
        /// ���������� ������� ��������� CAN-��������
        /// </summary>
        /// <param name="hDev">���������� ����������</param>
        /// <param name="pDcb">��������� � ����������� CAN-��������</param>
        /// <returns>��������� ���������� ��������</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
           CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_get_controller_config")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_get_controller_config(
            SafeFileHandle hDev,
            out F_CAN_SETTINGS pDcb);
        //-----------------------------------------------------------------------------------------
        // Function     : F_CAN_RESULT fw_can_set_controller_config(F_CAN_HANDLE hDev, PF_CAN_SETTINGS pDcb)
        // Parameters   : hDev - ����� ��������
        //              : pDcb - ��������� �� ��������� ���������� CAN-��������
        // Return value : CAN_RES_OK - �����;
        // ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
        // Description  :  ������ ����� �������� ���������� CAN-��������. ��������� ����������
        // �������� ������, ����� ������� ��������� � ��������� CAN_STATE_INIT.
        /// <summary>
        /// ������ ����� �������� ���������� CAN-��������. ��������� ����������
        /// �������� ������, ����� ������� ��������� � ��������� CAN_STATE_INIT.
        /// </summary>
        /// <param name="hDev">���������� ����������</param>
        /// <param name="pDcb">��������� � ����������� CAN-��������</param>
        /// <returns>��������� ���������� ��������</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
          CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_set_controller_config")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_set_controller_config(
            SafeFileHandle hDev,
            ref F_CAN_SETTINGS pDcb);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_get_timeouts(F_CAN_HANDLE hDev, PF_CAN_TIMEOUTS pTimeouts)
        // Parameters   :  hDev - ����� ��������
        //              :  pTimeouts - ��������� �� ��������� ��������� CAN-��������
        // Return value : CAN_RES_OK - �����;
        //                ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
        // Description  :  ���������� ������� �������� ��������� �������� ������, ������ � ������ ��������� CAN-��������.
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_get_timeouts")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_get_timeouts(
            SafeFileHandle hDev,
            out F_CAN_TIMEOUTS pTimeouts);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_set_timeouts(F_CAN_HANDLE hDev, PF_CAN_TIMEOUTS pTimeouts)
        // Parameters   :  hDev - ����� ��������
        //              :  pTimeouts - ��������� �� ��������� ��������� CAN-��������
        // Return value :  CAN_RES_OK - �����;
        //                 ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
        // Description  :  ������������� ����� �������� ��������� �������� ������, ������ � ������ ��������� CAN-��������.
        /// <summary>
        /// ������������� ����� �������� ��������� �������� ������, ������ � ������ ��������� CAN-��������.
        /// </summary>
        /// <param name="hDev">���������� ����������</param>
        /// <param name="pTimeouts">��������� � ����������</param>
        /// <returns>��������� ���������� ��������</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_set_timeouts")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_set_timeouts(
            SafeFileHandle hDev,
            ref F_CAN_TIMEOUTS pTimeouts);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_get_controller_state(F_CAN_HANDLE hDev, PF_CAN_STATE pState)
        // Parameters   :  hDev - ����� ��������
        //              :  pState - ��������� �� ����������, � ������� ����� ���������� ������� ��������� ��������,
        // �������:
        //              CAN_STATE_INIT - ��������� ���������
        //              CAN_STATE_ERROR_ACTIVE - ���-�� ������ ������/�������� �� ����� 96
        //              CAN_STATE_ERROR_WARNING	- ���-�� ������ ������/�������� �� 96 �� 127
        //              CAN_STATE_ERROR_PASSIVE -	���-�� ������ ������/�������� �� 128 �� 255
        //              CAN_STATE_BUS_OFF -	���-�� ������ ������/�������� >= 256 (BUSOFF)
        //              CAN_STATE_STOPPED - ������� ����������
        //              CAN_STATE_SLEEPING - �� ������������ � ������� ������
        // Return value : CAN_RES_OK - �����;
        //              ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
        // Description  :  ���������� ������� ��������� CAN-��������
        /// <summary>
        /// ���������� ������� ��������� CAN-��������
        /// </summary>
        /// <param name="hDev">���������� ����������</param>
        /// <param name="pState"> ��������� � ����������� CAN-��������:
        ///             CAN_STATE_INIT - ��������� ���������
        ///             CAN_STATE_ERROR_ACTIVE - ���-�� ������ ������/�������� �� ����� 96
        ///             CAN_STATE_ERROR_WARNING	- ���-�� ������ ������/�������� �� 96 �� 127
        ///             CAN_STATE_ERROR_PASSIVE -	���-�� ������ ������/�������� �� 128 �� 255
        ///             CAN_STATE_BUS_OFF -	���-�� ������ ������/�������� >= 256 (BUSOFF)
        ///             CAN_STATE_STOPPED - ������� ����������
        ///             CAN_STATE_SLEEPING - �� ������������ � ������� ������
        ///</param>
        /// <returns>��������� ���������� ��������</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
           CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_get_controller_state")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_get_controller_state(
            SafeFileHandle hDev,
            out F_CAN_STATE pState);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_get_stats(F_CAN_HANDLE hDev, PF_CAN_STATS pStats)
        // Parameters   :  hDev - ����� ��������
        //              :  pDcb - ��������� �� ��������� �������������� ���������� CAN-��������
        // Return value : 
        //              CAN_RES_OK - �����;
        //              ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
        // Description  :  ���������� �������������� ���������� ��������� ��������.
        /// <summary>
        /// ���������� �������������� ���������� ��������� ��������.
        /// </summary>
        /// <param name="hDev">���������� ����������</param>
        /// <param name="pStats">��������� �� ����������� ����������� CAN-��������:
        /// ���-�� �������� ������; ���-�� �������� ����; ���-�� ���������� ������;
        /// ���-�� ���������� ����; ���-�� ������ ������; ���-�� ������������ ������ ������;
        /// ���-�� ������ ��������; ���-�� ������������ ������ ��������;
        /// ���-�� ������ �� ���� (������������, CRC, etc.);
        /// ���-�� ��������� � CAN_STATE_ERROR_WARNING;
        /// ���-�� ��������� � CAN_STATE_ERROR_PASSIVE;
        /// ���-�� ��������� � CAN_STATE_BUS_OFF;
        /// ���-�� "����������" ���������;
        /// ���-�� ������������ �����������
        /// </param>
        /// <returns>��������� ���������� ��������</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_get_stats")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_get_stats(
            SafeFileHandle hDev,
            out F_CAN_STATS pStats);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_clear_stats(F_CAN_HANDLE hDev)
        // Parameters   :  hDev - ����� ��������
        // Return value :  CAN_RES_OK - �����;
        //                 ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
        // Description  :  ���������� � 0 �������������� ���������� ��������� ��������.
        /// <summary>
        /// ���������� � 0 �������������� ���������� ��������� ��������.
        /// </summary>
        /// <param name="hDev">���������� ����������</param>
        /// <returns>��������� ���������� ��������</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_clear_stats")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_clear_stats(SafeFileHandle hDev);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_start(F_CAN_HANDLE hDev)
        // Parameters   :  hDev - ����� ������������ ��������
        // Return value :  CAN_RES_OK - �����;
        //                 ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
        // Description  :  ��������� �������� �������. ��� �������� ������� ������� �����������
        //                 ����������� � ����� CAN � ������� �� ���������� ��������� CAN_STATE_INIT.
        /// <summary>
        /// ��������� �������� �������. ��� �������� ������� ������� �����������
        /// ����������� � ����� CAN � ������� �� ���������� ��������� CAN_STATE_INIT.
        /// </summary>
        /// <param name="hDev">���������� ����������</param>
        /// <returns>��������� ���������� �������</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_start")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_start(SafeFileHandle hDev);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_stop(F_CAN_HANDLE hDev)
        // Parameters   :  hDev - ����� ���������������� ��������
        // Return value :  CAN_RES_OK - �����;
        //                 ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
        // Description  :  ������������� �������� �������. ��� �������� �������� ������� �������������
        //                 �� ���� CAN. ����� ���������� ������� ������� ��������� � ��������� CAN_STATE_INIT.
        /// <summary>
        /// ������������� �������� �������. ��� �������� �������� ������� �������������
        /// �� ���� CAN. ����� ���������� ������� ������� ��������� � ��������� CAN_STATE_INIT.
        /// </summary>
        /// <param name="hDev">���������� ����������</param>
        /// <returns>��������� ���������� ��������</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_stop")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_stop(SafeFileHandle hDev);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_send(F_CAN_HANDLE hDev, PF_CAN_TX pTx, size_t nTx, size_t* nSend)
        // Parameters   :  hDev - ����� ��������
        //                 pTx - ��������� �� ����� ������������ ���������
        //                 nTx - ���������� ��������� � ������ pTx
        //                 nSend - ��������� �� ����������, � ������� ����� �������� ���������� ������� ����������
        //                         ��������� ��� �������� �� ������ �������.
        // Return value : CAN_RES_OK - �����;
        //                ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
        // Description  :  �������� ��������� � ���� CAN.
        //                 ������� ����������� �� ��� ���, ���� ��������� �� ����� ��������, ���� ����
        //                 �� ���������� ������� ��������. ������� �������� ������ ���� �������������� �����
        //                 ��� ������ fw_can_set_timeouts().
        /// <summary>
        /// �������� ��������� � ���� CAN. ������� ����������� �� ��� ���, ���� ��������� �� ����� ��������, 
        /// ���� ���� �� ���������� ������� ��������. ������� �������� ������ ���� �������������� �����
        /// ��� ������ fw_can_set_timeouts().
        /// </summary>
        /// <param name="hDev">���������� ����������</param>
        /// <param name="pTx">��������� �� ����� ������������ ���������</param>
        /// <param name="nTx">���������� ��������� � ������ pTx</param>
        /// <param name="nSend">��������� �� ����������, � ������� ����� �������� 
        /// ���������� ������� ���������� ��������� ��� �������� �� ������ �������.</param>
        /// <returns>��������� ���������� ��������</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "fw_can_send")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_send(
            SafeFileHandle hDev,
            ref F_CAN_TX pTx,
            UInt32 nTx,
            ref UInt32 nSend);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_recv(F_CAN_HANDLE hDev, PF_CAN_RX pRx, size_t szRx, size_t* nRecv)
        // Parameters   :  hDev - ����� ��������
        //                 pRx - ��������� �� ����� ������ � ����������
        //                 nRx - ������� ������ ������
        //                 nRecv - ��������� �� ����������, � ������� ����� �������� ���������� �������� ���������,
        //                         ����������� � ������ pRx ��� �������� �� ������ �������.
        // Return value :  CAN_RES_OK - �����;
        //                 ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
        // Description  :  ��� ������ ������ ������� ���������� ���������� ��������� ��������� �� ���������� ������
        // �������� ��� ������� ��������. ���� ���������� ����� ����, ������� ����������� � ������� 
        // ������ ���� �� ������ ��������� ��� �������� ������. ������� ������ ������ ���� �������������� 
        // ����� ��� ������ fw_can_set_timeouts().
        /// <summary>
        /// ��� ������ ������ ������� ���������� ���������� ��������� ��������� �� ���������� ������
        /// �������� ��� ������� ��������. ���� ���������� ����� ����, ������� ����������� � ������� 
        /// ������ ���� �� ������ ��������� ��� �������� ������. ������� ������ ������ ���� �������������� 
        /// ����� ��� ������ fw_can_set_timeouts().
        /// </summary>
        /// <param name="hDev">���������� ����������</param>
        /// <param name="pRx">��������� �� ����� ������ � ����������</param>
        /// <param name="szRx">������� ������ ������</param>
        /// <param name="nRecv">��������� �� ����������, � ������� ����� �������� 
        /// ���������� �������� ���������, ����������� � ������ pRx ��� �������� 
        /// �� ������ �������.</param>
        /// <returns>��������� ���������� ��������</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_recv")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_recv(
            SafeFileHandle hDev,
            out F_CAN_RX pRx, // ����� ������� ��������� ������ F_CAN_RX[]. ��, ��� ���� ����� ���� szRx = 1 
            UInt32 szRx,
            out UInt32 nRecv);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_recv(F_CAN_HANDLE hDev, PF_CAN_RX pRx, size_t szRx, size_t* nRecv)
        // Parameters   :  hDev - ����� ��������
        //                 pRx - ��������� �� ����� ������ � ����������
        //                 nRx - ������� ������ ������
        //                 nRecv - ��������� �� ����������, � ������� ����� �������� ���������� �������� ���������,
        //                         ����������� � ������ pRx ��� �������� �� ������ �������.
        // Return value :  CAN_RES_OK - �����;
        //                 ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
        // Description  :  ��� ������ ������ ������� ���������� ���������� ��������� ��������� �� ���������� ������
        // �������� ��� ������� ��������. ���� ���������� ����� ����, ������� ����������� � ������� 
        // ������ ���� �� ������ ��������� ��� �������� ������. ������� ������ ������ ���� �������������� 
        // ����� ��� ������ fw_can_set_timeouts().
        /// <summary>
        /// ��� ������ ������ ������� ���������� ���������� ��������� ��������� �� ���������� ������
        /// �������� ��� ������� ��������. ���� ���������� ����� ����, ������� ����������� � ������� 
        /// ������ ���� �� ������ ��������� ��� �������� ������. ������� ������ ������ ���� �������������� 
        /// ����� ��� ������ fw_can_set_timeouts().
        /// </summary>
        /// <param name="hDev">���������� ����������</param>
        /// <param name="pRx">��������� �� ����� ������ � ����������</param>
        /// <param name="szRx">������� ������ ������</param>
        /// <param name="nRecv">��������� �� ����������, � ������� ����� �������� 
        /// ���������� �������� ���������, ����������� � ������ pRx ��� �������� 
        /// �� ������ �������.</param>
        /// <returns>��������� ���������� ��������</returns>
        /// <remarks>
        //������ ����������:
        //static void Read1(SafeFileHandle hDevice)
        //{
        //    CAN_RESULT result;
        //    F_CAN_RX[] rx_buf;
        //    UInt32 nRx;

        //    int size = Marshal.SizeOf(typeof(F_CAN_RX));

        //    IntPtr ptr = Marshal.AllocHGlobal(size);

        //    result = Api.fw_can_recv(hDevice, ptr, 5, out nRx);

        //    if (result == CAN_RESULT.CAN_RES_OK)
        //    {
        //        rx_buf = new F_CAN_RX[5];
        //        Console.WriteLine("Frames to read: {0}", nRx);

        //        for (int i = 0; i < nRx; i++)
        //        {
        //            IntPtr ptr1 = new IntPtr(ptr.ToInt32() + i * size);
        //            rx_buf[i] = (F_CAN_RX)Marshal.PtrToStructure(ptr1, typeof(F_CAN_RX));

        //            // ������ ����� ��������
        //            Console.WriteLine("Frame {0}:", i);
        //            Console.WriteLine("Time Stamp: {0}", rx_buf[i].timestamp);
        //            Console.WriteLine("Id: {0}", rx_buf[i].msg.can_id);
        //            Console.WriteLine("Total Bytes: {0}", rx_buf[i].msg.can_dlc);
        //            Console.WriteLine("Total Massages: {0} ", nRx);
        //            for (int x = 0; x < rx_buf[i].msg.can_dlc; x++)
        //            {
        //                Console.WriteLine("Data: {0}", rx_buf[i].msg.data[x]);
        //            }
        //            Console.WriteLine(" ");

        //        }

        //        Marshal.FreeHGlobal(ptr);
        //    }
        //    else
        //    {
        //        Console.WriteLine("Read. Error: {0}", Enum.GetName(typeof(CAN_RESULT), result));
        //        Console.WriteLine(" ");
        //    }
        //    return;
        //}
        /// </remarks>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_recv")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_recv(
            SafeFileHandle hDev,
            IntPtr buffer, //��������� �� ������ F_CAN_RX[] pRx, 
            UInt32 szRx,
            out UInt32 nRecv);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_peek_message(F_CAN_HANDLE hDev, PF_CAN_RX pRx)
        // Parameters   :  hDev - ����� ��������
        //                 pRx - ��������� �� ���������� F_CAN_RX, � ������� ����� �������� ���������
        // Return value :  CAN_RES_OK - �����;
        //                 ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
        //                 CAN_RES_RXQUEUE_EMPTY - ��� ��������� �� ���������� ������ ������ �������� ���
        //                 ������� ��������.
        // Description  :  ��� ������ ������ ������� ���������� ���������� ������ ��������� �� �����������
        //                 ������ ������. ������� �� ����������� � ���������� ����������� ��� ����� ��������� ������.
        /// <summary>
        /// ��� ������ ������ ������� ���������� ���������� ������ ��������� �� �����������
        /// ������ ������. ������� �� ����������� � ���������� ����������� ��� ����� ��������� ������.
        /// </summary>
        /// <param name="hDev">���������� ����������</param>
        /// <param name="pRx">pRx - ��������� �� ���������� F_CAN_RX, 
        /// � ������� ����� �������� ���������</param>
        /// <returns>��������� ���������� ��������</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_peek_message")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_peek_message(
            SafeFileHandle hDev,
            out F_CAN_RX pRx);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_post_message(F_CAN_HANDLE hDev, PF_CAN_TX pTx)
        // Parameters   :  hDev - ����� ��������
        //                 pTx - ��������� �� ����������-���������, ���������� ��������
        // Return value :  CAN_RES_OK - �����;
        //                 ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
        //                 CAN_RES_TXQUEUE_FULL - ������������ ������ ��������.
        // Description  :  ������� �������� ��������� �� ���������� ����� �������� ��������.
        // ������� �� ����������� � �� ������� ���������� �������� ���������.
        /// <summary>
        /// ������� �������� ��������� �� ���������� ����� �������� ��������.
        /// ������� �� ����������� � �� ������� ���������� �������� ���������.
        /// </summary>
        /// <param name="hDev">���������� ����������</param>
        /// <param name="pTx">��������� �� ����������-���������, ���������� ��������</param>
        /// <returns>���������  ���������� ��������</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_post_message")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_post_message(
            SafeFileHandle hDev,
            ref F_CAN_TX pTx);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_purge(F_CAN_HANDLE hDev, F_CAN_PURGE_MASK flags)
        // Parameters   :  hDev - ����� ��������
        //                 flags - ����� ������/���������� ������� �������� ��� ���������
        //                 CAN_PURGE_TXABORT - ��������� ���������� ��������� � �������� �� ������.
        //                 CAN_PURGE_TXCLEAR - ���������� � �������� ������� ������ �� ��������.
        //                 CAN_PURGE_RXCLEAR - ������� ���������� ����� ������.
        //                 CAN_PURGE_RXABORT - ��������� ���������� ��������� � �������� �� ������.
        //                 CAN_PURGE_HWRESET - ��������� ���������� ����� ��������. ��� ���� ������������
        //                 �������� ������ ��������. ��������� �������� �����������.
        // Return value :  CAN_RES_OK - �����;
        //                 ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
        // Description  :  �������, � ����������� �� �������� flags, ������� ������ ������ �/��� �������� �/���
        // ��������� ���������� ����� ��������. 
        // ����� ����, ��������� �������� �������/���������� ��������� � �������� �� ������/������.
        /// <summary>
        /// �������, � ����������� �� �������� flags, ������� ������ ������ �/��� �������� �/���
        /// ��������� ���������� ����� ��������. ����� ����, ��������� �������� �������/���������� 
        /// ��������� � �������� �� ������/������.
        /// </summary>
        /// <param name="hDev">���������� ����������</param>
        /// <param name="flags">����� ������/���������� ������� �������� ��� ���������:
        ///                 CAN_PURGE_TXABORT - ��������� ���������� ��������� � �������� �� ������.
        ///                 CAN_PURGE_TXCLEAR - ���������� � �������� ������� ������ �� ��������.
        ///                 CAN_PURGE_RXCLEAR - ������� ���������� ����� ������.
        ///                 CAN_PURGE_RXABORT - ��������� ���������� ��������� � �������� �� ������.
        ///                 CAN_PURGE_HWRESET - ��������� ���������� ����� ��������. ��� ���� ������������
        ///                 �������� ������ ��������. ��������� �������� �����������.</param>
        /// <returns>��������� ���������� ��������</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_purge")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_purge(
            SafeFileHandle hDev,
            [MarshalAs(UnmanagedType.U4)] F_CAN_PURGE_MASK flags);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_wait(F_CAN_HANDLE hDev, PF_CAN_WAIT pWait, size_t msTimeout)
        // Parameters   :  hDev - ����� ��������
        //                 pWait - ��������� �� ������, ���������� ����� ���������� ������� CAN-�������� pWait->waitMask
        //                 � �������� �������� �������� �������.
        //                 msTimeout - ������� �������� � �������������.
        // Return value :  CAN_RES_OK - �����;
        //                 CAN_RES_TIMEOUT - �������, ������� ������ �� ������������� ����������, 
        //                 � ������� CAN_RES_OK � CAN_RES_TIMEOUT � ���� pWait->status ����� �������� �������� �������� �������.
        //                 ��������, �������� �� CAN_RES_OK � CAN_RES_TIMEOUT, ��������������� �� ������ ������ �������.
        //                 Description  :  ������� ��������� ���������� ����������� ������ �� ��� ���, ���� ������ CAN-�������� �� 
        //                 ����� ��������������� ������ �� ��������, ������������ ������ pWait->waitMask, ��� 
        //                 �� ������� ������� �������� ���������� msTimeout. ��� ���������� ������ ������� � ����� 
        //                 CAN_RES_OK ��� CAN_RES_TIMEOUT ���� pWait->status ����� ��������� ������� �������� �������.
        /// <summary>
        /// ������� ��������� ���������� ����������� ������ �� ��� ���, ���� ������ CAN-�������� �� 
        /// ����� ��������������� ������ �� ��������, ������������ ������ pWait->waitMask, ��� 
        /// �� ������� ������� �������� ���������� msTimeout. ��� ���������� ������ ������� � ����� 
        /// CAN_RES_OK ��� CAN_RES_TIMEOUT ���� pWait->status ����� ��������� ������� �������� �������.
        /// </summary>
        /// <param name="hDev">���������� CAN-�����</param>
        /// <param name="pWait">pWait - ��������� �� ������, ���������� ����� ���������� 
        /// ������� CAN-�������� pWait->waitMask � �������� �������� �������� �������.</param>
        /// <param name="msTimeout">������� �������� � �������������.</param>
        /// <returns>��������� ���������� ��������:
        ///                CAN_RES_OK - �����;
        ///                 CAN_RES_TIMEOUT - �������, ������� ������ �� ������������� ����������, 
        ///                 � ������� CAN_RES_OK � CAN_RES_TIMEOUT � ���� pWait->status ����� �������� �������� �������� �������.
        ///                 ��������, �������� �� CAN_RES_OK � CAN_RES_TIMEOUT, ��������������� �� ������ ������ �������.
        ///                 Description  :  ������� ��������� ���������� ����������� ������ �� ��� ���, ���� ������ CAN-�������� �� 
        ///                 ����� ��������������� ������ �� ��������, ������������ ������ pWait->waitMask, ��� 
        ///                 �� ������� ������� �������� ���������� msTimeout. ��� ���������� ������ ������� � ����� 
        ///                 CAN_RES_OK ��� CAN_RES_TIMEOUT ���� pWait->status ����� ��������� ������� �������� �������.</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_wait")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_wait(
            SafeFileHandle hDev,
            ref F_CAN_WAIT pWait,
            UInt32 msTimeout);
        //-----------------------------------------------------------------------------------------
        // Function     :  F_CAN_RESULT fw_can_get_clear_errors(F_CAN_HANDLE hDev, PF_CAN_ERRORS pErrors)
        // Parameters   :  hDev - ����� ��������
        //                 pErrors - ��������� �� ������, � ������� ����� ���������� �������� ������.
        //                 ���� � �������� pErrors �������� NULL, ���������� ����� ��������� ������ ���
        //                 ������� ��������.
        // Return value :  CAN_RES_OK - �����;
        // ��������, �������� �� CAN_RES_OK, ��������������� �� ������.
        // Description  :  ������� �������� �������� ��������� ������ ������� �������� �
        // ������ pErrors � ���������� ��������. ���� � �������� pErrors �������� NULL, 
        // ����� ���������� ����� ��������� ������ ��� ������� ��������.
        /// <summary>
        /// ������� �������� �������� ��������� ������ ������� �������� �
        /// ������ pErrors � ���������� ��������. ���� � �������� pErrors �������� NULL, 
        /// ����� ���������� ����� ��������� ������ ��� ������� ��������.
        /// </summary>
        /// <param name="hDev">���������� ����������</param>
        /// <param name="pErrors">��������� �� ������, � ������� ����� ���������� �������� ������.
        /// ���� � �������� pErrors �������� NULL, ���������� ����� ��������� ������ ���
        /// ������� ��������.</param>
        /// <returns>��������� ���������� ��������</returns>
        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
            CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_get_clear_errors")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_get_clear_errors(
            SafeFileHandle hDev,
            out F_CAN_ERRORS pErrors);

        [DllImport("fwcan.dll", CallingConvention = CallingConvention.Cdecl,
    CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = "fw_can_get_clear_errors")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern F_CAN_RESULT fw_can_get_clear_errors(
            SafeFileHandle hDev,
            IntPtr pErrors);
        //-----------------------------------------------------------------------------------------
        public static Boolean f_can_success(F_CAN_RESULT result)
        {
            if (result == F_CAN_RESULT.CAN_RES_OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// ��������� ����������� ���������� �� ������������
        /// (���������� �� ������������ Fastwel)
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static Boolean fw_can_is_handle_valid(SafeFileHandle handle)
        {
            if ((handle == null) || (handle.IsInvalid))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //-----------------------------------------------------------------------------------------
        /// <summary>
        /// ������ � ���������� ��������� ������ ������ ����� NIM351
        /// </summary>
        /// <param name="mode">����� ������ �����</param>
        /// <param name="frameFormat">������ �����</param>
        /// <param name="errorFrameEnable">���������/��������� �������� �������������� ���������</param>
        /// <returns></returns>
        public static CAN_OPMODE OpModeBuilder(PortMode mode,
            FrameFormat frameFormat, Boolean errorFrameEnable)
        {
            CAN_OPMODE result;
            String msg;

            result = CAN_OPMODE.CAN_OPMODE_INIT;

            switch (mode)
            {
                case PortMode.NORMAL:
                    { break; }
                case PortMode.LISTEN_ONLY:
                    { result |= CAN_OPMODE.CAN_OPMODE_LSTNONLY; break; }
                case PortMode.SELFTEST:
                    { result |= CAN_OPMODE.CAN_OPMODE_SELFTEST; break; }
                case PortMode.SELFRECV:
                    { result |= CAN_OPMODE.CAN_OPMODE_SELFRECV; break; }
                default:
                    {
                        msg = String.Format(Properties.ErrorMessages.NotSupportedValue, mode);
                        throw new InvalidCastException(msg);
                    }
            }

            switch (frameFormat)
            {
                case FrameFormat.StandardFrame:
                    { result |= CAN_OPMODE.CAN_OPMODE_STANDARD; break; }
                case FrameFormat.ExtendedFrame:
                    { result |= CAN_OPMODE.CAN_OPMODE_EXTENDED; break; }
                case FrameFormat.MixedFrame:
                    { result |= (CAN_OPMODE.CAN_OPMODE_STANDARD | CAN_OPMODE.CAN_OPMODE_EXTENDED); break; }
                default:
                    {
                        msg = String.Format(Properties.ErrorMessages.NotSupportedValue, frameFormat);
                        throw new InvalidCastException(msg);
                    }
            }

            if (errorFrameEnable)
            { result |= CAN_OPMODE.CAN_OPMODE_ERRFRAME; }

            return result;
        }
        /// <summary>
        /// ������ ���������
        /// </summary>
        /// <param name="identifier">������������� ���������</param>
        /// <param name="frameType">��� ���������</param>
        /// <param name="frameFormat">������ ���������</param>
        /// <param name="data">������ ���������</param>
        /// <returns></returns>
        public static F_CAN_MSG MessageBuilder(uint identifier, FrameType frameType,
            FrameFormat frameFormat, byte[] data)
        {
            F_CAN_MSG result;
            String msg;

            // ��������� ��������� � �������������� ��� ��� ��������
            result.can_dlc = (Byte)data.Length;
            result.data = new Byte[8];
            Array.Copy(data, result.data, data.Length);

            result.can_id = identifier;
            //buffer.msg.can_id &= (CAN_MSG_MASK.CAN_SFF_MASK | CAN_MSG_MASK.CAN_EFF_MASK);

            if (frameFormat == FrameFormat.ExtendedFrame)
            {
                result.can_id |= CAN_MSG_FLAG.CAN_EFF_FLAG;
            }

            if (frameType == FrameType.REMOTEFRAME)
            {
                if (data.Length != 0)
                {
                    // ��� �������� ��������� �������� ������, ��� 
                    // ������������� ���� RTR, ����� DLC ������ ������ ���� ����� 0
                    msg = "�� ������� ��������� ���������. DLC ������ ���� ����� 0 ��� ������������� ���� RTR";
                    throw new Exception(msg);
                }
                result.can_id |= CAN_MSG_FLAG.CAN_RTR_FLAG;
            }

            return result; 
        }
        /// <summary>
        /// ���������� ����� ����� �� ���������� ������������ �����
        /// </summary>
        /// <param name="portName">������������ ����� � ������� CANx ��� x ����� ����� 1...9</param>
        /// <returns></returns>
        public static int GetPortNumber(string portName)
        {
            if (CheckPortName(portName))
            {
                return int.Parse(portName.Substring(3, 1));
            }
            else
            {
                throw new ArgumentException(
                    "���������� �������� ����� �����. " +
                    "������������ ����� �� ������������ ������� CANx, ��� x ����� ����� 1...9");
            }
        }
        /// <summary>
        /// ��������� ������ � ������������ ����� �� �����������
        /// �������.
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public static Boolean CheckPortName(String portName)
        {
            String PortNameRegexPattern = @"^CAN[1-9]$";

            if (Regex.IsMatch(portName, PortNameRegexPattern))
            { return true; }
            else
            { return false; }
        }
    }
    //=============================================================================================
}
//=================================================================================================
// End Of File