using System;
using System.Collections.Generic;
using System.Text;
//
using Modbus.OSIModel.ApplicationLayer;

//=================================================
namespace Modbus.OSIModel.Message
{
    //---------------------------------------------
    /// <summary>
    /// Класс предназначен для возвращения результата
    /// modbus-запроса
    /// </summary>
    [Serializable]
    public class Result
    {
        //-----------------------------------------
        /// <summary>
        /// Хранит результат выполнения операции
        /// </summary>
        private Error _ErrorCode;
        //-----------------------------------------
        /// <summary>
        /// Код ошибки 
        /// результата выполнения запроса
        /// </summary>
        public Error Error
        {
            get { return this._ErrorCode; }
            set { this._ErrorCode = value; }
        }
        //-----------------------------------------
        /// <summary>
        /// Содержит описание ошибки
        /// </summary>
        private String _Description;
        //-----------------------------------------
        /// <summary>
        /// Описание ошибки
        /// </summary>
        public String Description
        {
            get { return this._Description; }
            set { this._Description = value; }
        }
        //-----------------------------------------
        /// <summary>
        /// Хранит полученое сообщение
        /// </summary>
        private Message _Answer;
        //-----------------------------------------
        /// <summary>
        /// Ответное сообщение
        /// при выполнении запроса. Если запрос потерпел
        /// неудачу, содержит null
        /// </summary>
        public Message Answer
        {
            get { return this._Answer; }
            set { this._Answer = value; }
        }
        //-----------------------------------------
        /// <summary>
        /// Хранит запрос
        /// </summary>
        private Message _Request;
        //-----------------------------------------
        /// <summary>
        /// Запрос
        /// </summary>
        public Message Request
        {
            get { return _Request; }
            set { this._Request = value; }
        }
        //-----------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        public Result()
        {}
        //-----------------------------------------
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
            this._Request = request;
            this._ErrorCode = error;
            this._Description = description;
            this._Answer = answer;
            return;
        }
        //-----------------------------------------
    }
    //---------------------------------------------
}
//=================================================