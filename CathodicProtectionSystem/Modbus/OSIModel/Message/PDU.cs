using System;
using System.Collections.Generic;

namespace Modbus.OSIModel.Message
{
    /// <summary>
    /// Класс описывает protocol data unit
    /// </summary>
    [Serializable]
    public class PDU
    {
        #region Fields and Properties
        /// <summary>
        /// Код функции ModBus (поле кода функции)
        /// </summary>
        private byte _functioncode;
        /// <summary>
        /// Возвращает/устанавливает код функции.
        /// </summary>
        public byte Function
        {
            get 
            { 
                return _functioncode; 
            }
            set 
            {
                _functioncode = value;
            }
        }
        /// <summary>
        /// Хранит данные сообщения
        /// </summary>
        private List<byte> _data;
        /// <summary>
        /// Данные сообщение 
        /// </summary>
        public Byte[] Data
        {
            get { return _data.ToArray(); }
            set 
            {
                _data.Clear();
                _data.AddRange(value); 
            }
        }
        /// <summary>
        /// Длина pdu, байт (address + data)
        /// </summary>
        public Int32 Length
        {
            get 
            {
                Int32 length = 1 + _data.Count; 
                return length;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public PDU()
        {
            _data = new List<byte>();
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="functionCode">Код modbus-функции</param>
        /// <param name="data">Данные</param>
        public PDU(Byte functionCode, Byte[] data)
        {
            _functioncode = functionCode;
            _data = new List<byte>();
            _data.AddRange(data);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Добавить байт данных в сообщение
        /// </summary>
        /// <param name="data"></param>
        public void AddDataByte(byte data)
        {
            if (_data.Count < 252)
            {
                _data.Add(data);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
            return;
        }
        /// <summary>
        /// Добавляет в поле данных сообщения байты
        /// </summary>
        /// <param name="data">Массив байт данных</param>
        public void AddDataBytesRange(Byte[] data)
        {
            _data.AddRange(data);
            
            if (_data.Count > 253)
            {
                throw new ArgumentOutOfRangeException();
            }

            return;
        }
        /// <summary>
        /// Удаляет все данные из поля данных и устанавливает
        /// код функции в ноль
        /// </summary>
        public void Clear()
        {
            _functioncode = 0;
            _data.Clear();
            return;
        }
        /// <summary>
        /// Метод возвращает массив, где 1 элемент - код функции, 
        /// а последующие - данные сообщения
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            List<byte> arr = new List<byte>();
            arr.Add(_functioncode);
            arr.AddRange(_data);
            return arr.ToArray();
        }
        /// <summary>
        /// Устанавливает старший бит байта кода функции в 1 
        /// </summary>
        public void SetErrorBit()
        {
            _functioncode = (Byte)(_functioncode | 0x80);
        }
        /// <summary>
        /// Сбрасывает старший бит байта кода функции в 0
        /// </summary>
        public void ResetErrorBit()
        {
            _functioncode = (Byte)(_functioncode & 0x7F);
        }
        #endregion
    }
}