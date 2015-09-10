using System;

namespace NGK.CAN.ApplicationLayer.Transactions
{
    /// <summary>
    /// Определяет тип сетевой транзакции
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Транзакция запроса по уникальному адресу
        /// </summary>
        UnicastMode,
        /// <summary>
        /// Транзакция широковещаетльного запроса 
        /// </summary>
        BroadcastMode,
        /// <summary>
        /// Тип транзации не определён
        /// </summary>
        Undefined
    }
}
