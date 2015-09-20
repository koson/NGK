using System;
using Modbus.OSIModel.Message;

namespace Modbus.OSIModel.DataLinkLayer.Slave.RTU.ComPort
{
    /// <summary>
    /// Класс для создания аргументов при событии отправки или приёма сообщения
    /// </summary>
    public class MessageEventArgs
    {
        Message.Message _Message;
        /// <summary>
        /// Отправленное сообщение
        /// </summary>
        public Message.Message Message
        {
            get { return _Message; }
            set { _Message = value; }
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        public MessageEventArgs()
        {
            Message = new Message.Message(0, new PDU());
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="message">Сообщение отправленное 
        /// мастеру или принятое от него</param>
        public MessageEventArgs(Message.Message message)
        {
            Message = message;
        }
    }
}
