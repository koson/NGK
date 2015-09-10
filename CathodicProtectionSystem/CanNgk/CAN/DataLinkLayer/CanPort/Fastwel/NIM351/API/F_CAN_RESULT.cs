using System;
using System.Collections.Generic;
using System.Text;

//===========================================================================================
namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver
{
    //=======================================================================================
    /// <summary>
    /// ��������� ���������� ������� �������� �AN
    /// </summary>
    [Serializable]
    public enum F_CAN_RESULT : int
    {
        //===================================================================================
        /// <summary>
        /// �����
        /// </summary>
        CAN_RES_OK = 0,
        /// <summary>
        /// ���������� ������
        /// </summary>
        CAN_RES_HARDWARE = 1,
        /// <summary>
        /// ���������������� (������������) ��������� ������������� (�����) ��������
        /// </summary>
        CAN_RES_INVALID_HANDLE = 2,
        /// <summary>
        /// ������������ ���������
        /// </summary>
        CAN_RES_INVALID_POINTER = 3,
        /// <summary>
        /// ������������ �������� (���� ��� ���������)
        /// </summary>
        CAN_RES_INVALID_PARAMETER = 4,
        /// <summary>
        /// �� ������� ��������� ��������
        /// </summary>
        CAN_RES_INSUFFICIENT_RESOURCES = 5,
        /// <summary>
        /// �� ������� ������� ����������
        /// </summary>
        CAN_RES_OPEN_DEVICE = 6,
        /// <summary>
        /// ���������� ������ � �������� ����������
        /// </summary>
        CAN_RES_UNEXPECTED = 7,
        /// <summary>
        /// ������ ��������� � �������� ��� � ����������
        /// </summary>
        CAN_RES_FAILURE = 8,
        /// <summary>
        /// ����� ������ ����
        /// </summary>
        CAN_RES_RXQUEUE_EMPTY = 9,
        /// <summary>
        /// �������� �� ��������������
        /// </summary>
        CAN_RES_NOT_SUPPORTED = 10,
        /// <summary>
        /// �������
        /// </summary>
        CAN_RES_TIMEOUT = 11
    }
    //=======================================================================================
}
//===========================================================================================
