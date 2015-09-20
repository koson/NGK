using System;
using System.Collections.Generic;
using System.Text;
//
using Modbus.OSIModel.ApplicationLayer;

namespace Modbus.OSIModel.Message
{
    /// <summary>
    /// Класс предназначен для возвращения результата
    /// modbus-запроса
    /// </summary>
    [Serializable]
    public class Result
    {
        /// <summary>
        /// Хранит результат выполнения операции
        /// </summary>
        private Error _ErrorCode;
        /// <summary>
        /// Код ошибки 
        /// результата выполнения запроса
        /// </summary>
        public Error Error
        {
            get { return _ErrorCode; }
            set { _ErrorCode = value; }
        }
        /// <summary>
        /// Содержит описание ошибки
        /// </summary>
        private String _Description;
        /// <summary>
        /// Описание ошибки
        /// </summary>
        public String Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
        /// <summary>
        /// Хранит полученое сообщение
        /// </summary>
        private Message _Answer;
        /// <summary>
        /// Ответное сообщение
        /// при выполнении запроса. Если запрос потерпел
        /// неудачу, содержит null
        /// </summary>
        public Message Answer
        {
            get { return _Answer; }
            set { _Answer = value; }
        }
        /// <summary>
        /// Хранит запрос
        /// </summary>
        private Message _Request;
        /// <summary>
        /// Запрос
        /// </summary>
        public Message Request
        {
            get { return _Request; }
            set { _Request = value; }
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        public Result()
        {}
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="error">Код ошибки выполнения запроса</param>
        /// <param name="description">Описание ошибки</param>
        /// <param name="request">Запрос</param>
        /// <param name="answer">Ответное сообщение на запрос</param>
        public Result(Error error, String description,
            Message request, Message answer)
        {
            _Request = request;
            _ErrorCode = error;
            _Description = description;
            _Answer = answer;
            return;
        }
    }
}