using System;
using Modbus.OSIModel.DataLinkLayer.Slave.RTU.ComPort;
using Modbus.OSIModel.Message;

namespace Modbus.OSIModel.DataLinkLayer.Slave
{
    /// <summary>
    /// Интерфейс реализует функционал слоя DataLink layer модели 
    /// протоколов OSI
    /// </summary>
    public interface IDataLinkLayer: IDisposable
    {
        #region Fields And Properties
        /// <summary>
        /// Возвращает наименование порта
        /// </summary>
        string PortName
        {
            get;
        }
        /// <summary>
        /// Метод возвращает тип интерфейса физического уровня
        /// Serial port, CAN и т.п.
        /// </summary>
        Modbus.OSIModel.DataLinkLayer.InterfaceType TypeInterface
        {
            get;
        }
        /// <summary>
        /// Возвращает состояние соединения (true-если соединение
        /// открыто)
        /// </summary>
        Boolean IsOpen
        {
            get;
        }
        /// <summary>
        /// Возвращает режим передачи данных (RTU/ACSII)
        /// </summary>
        TransmissionMode Mode
        {
            get;
        }
        /// <summary>
        /// Возвращает объект физического подключения. Различается
        /// для разных типов подключений.
        /// </summary>
        /// <returns>Настройки</returns>
        //Object Connection
        //{
        //    get;
        //}
        #endregion

        #region Methods
        /// <summary>
        /// Открывает сетевое соединение
        /// </summary>
        void Open();
        /// <summary>
        /// Закрывает сетевое соединение
        /// </summary>
        void Close();
        /// <summary>
        /// Метод отправляет сообщение (ответ подчинённого на запрос) 
        /// </summary>
        /// <param name="answer">Ответное сообщение</param>
        void SendResponse(Message.Message answer);

        #endregion

        #region Events
        /// <summary>
        /// Событие происходит при приёме запроса 
        /// от мастера сети
        /// </summary>
        event EventHandlerRequestWasRecived RequestWasRecived;
        /// <summary>
        /// Событие происходит при отправке ответа на запрос
        /// </summary>
        event EventHandleResponseWasSent ResponseWasSent;
        /// <summary>
        /// Событие происходит при возникновении ошибочной ситуации
        /// </summary>
        event EventHandlerErrorOccurred ErrorOccurred;
        #endregion
    }
}
