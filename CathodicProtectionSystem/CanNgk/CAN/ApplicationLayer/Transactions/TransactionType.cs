using System;

namespace NGK.CAN.ApplicationLayer.Transactions
{
    /// <summary>
    /// ���������� ��� ������� ����������
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// ���������� ������� �� ����������� ������
        /// </summary>
        UnicastMode,
        /// <summary>
        /// ���������� ������������������ ������� 
        /// </summary>
        BroadcastMode,
        /// <summary>
        /// ��� ��������� �� ��������
        /// </summary>
        Undefined
    }
}
