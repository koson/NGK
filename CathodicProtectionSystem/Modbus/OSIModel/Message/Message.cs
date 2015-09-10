using System;
using System.Collections.Generic;
using System.Text;

//===================================================================================
namespace Modbus.OSIModel.Message
{
    //===============================================================================
    /// <summary>
    /// Класс для хранения modbus-сообщеия
    /// </summary>
    public class Message
    {
        //---------------------------------------------------------------------------
        #region Fields and Properties
        //---------------------------------------------------------------------------
        /// <summary>
        /// Поле хранит адрес устройства от которого
        /// пришло сообщение
        /// </summary>
        private Byte _Address;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Адресс slave-устройства
        /// от которого пришло ответное сообщение
        /// </summary>
        public Byte Address
        {
            get { return this._Address; }
            set { this._Address = value; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Хранит PDU-фрайм
        /// </summary>
        private PDU _PDU;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает PDU-фрайм
        /// (код функции + данные)
        /// </summary>
        public PDU PDUFrame
        {
            get { return this._PDU; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Метод возвращает 
        /// сумму CRC16 для данного сообщения
        /// </summary>
        /// <returns>CRC16</returns>
        public CRC16 CRC16
        {
            get
            {
                byte[] _array = this.ToArrayWithoutCRC16();
                UInt16 crct = 0xFFFF;

                for (int i = 0; i < _array.Length; i++)
                {
                    crct = (UInt16)(crct ^ System.Convert.ToUInt16(_array[i]));

                    for (int n = 0; n <= 7; n++)
                    {
                        if ((crct & 0x0001) == 0x0001)
                        {
                            crct = (UInt16)(crct >> 1);
                            crct = (UInt16)(crct ^ 0xA001);
                        }
                        else
                        {
                            crct = (UInt16)(crct >> 1);
                        }
                    }
                }
                return new CRC16(crct);
            }
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Constructors
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор по умолчанию запрещён
        /// </summary>
        private Message()
        {
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="address">Адрес ответившего устройства</param>
        /// <param name="frame">PDU-фрайм (код функции + данные)</param>
        public Message(byte address, Modbus.OSIModel.Message.PDU frame)
        {
            _Address = address;
            _PDU = frame;
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="address"></param>
        /// <param name="function"></param>
        /// <param name="data"></param>
        public Message(byte address, Byte function, Byte[] data)
        {
            _Address = address;
            _PDU = new PDU();
            _PDU.Function = function;
            _PDU.AddDataBytesRange(data);
            return;
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Methods
        //---------------------------------------------------------------------------
        /// <summary>
        /// Метод возвращает код функции в выполненной
        /// при запросе
        /// </summary>
        /// <returns>код Modbus-функции</returns>
        public byte GetFuction()
        {
            return _PDU.Function;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает адресс ответившего устройства
        /// </summary>
        /// <returns>адрес slave-устройства</returns>
        public byte GetAddress()
        {
            return _Address;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Метод возвращает данные содержащиеся 
        /// в сообщении
        /// </summary>
        /// <returns>Данные сообщения</returns>
        public Byte[] GetData()
        {
            return _PDU.Data;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Метод возвращает сообщение ввиде массива байт
        /// (Адрес + Код функции + данные) без CRC16
        /// </summary>
        /// <returns></returns>
        public Byte[] ToArrayWithoutCRC16()
        {
            List<byte> array = new List<byte>();
            array.Add(_Address);
            array.AddRange(_PDU.ToArray());
            return array.ToArray();
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает сообщение целиком в виде массива байт
        /// </summary>
        /// <returns></returns>
        public Byte[] ToArray()
        {
            List<byte> array = new List<byte>();
            array.Add(_Address);
            array.AddRange(_PDU.ToArray());
            array.AddRange(this.CRC16.ToArray());
            return array.ToArray();
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file