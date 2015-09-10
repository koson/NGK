using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NGK.CAN.DataLinkLayer.Message
{
    /// <summary>
    /// Структура для передачи сообщений в сети CAN
    /// </summary>
    [Description("Структура для передачи сообщений в сети CAN")]
    [Serializable]
    public struct Frame: ICloneable
    {
        public UInt32 _Identifier;
        /// <summary>
        /// Идентификатор сообщения. Для стандартного фрейма 11 бит, для расширенного 29 бит
        /// </summary>
        [Description("Идентификатор сообщения. Для стандартного фрейма 11 бит, для расширенного 29 бит")]
        public UInt32 Identifier
        {
            set
            {
                String msg;

                switch (this.FrameFormat)
                {
                    case FrameFormat.StandardFrame:
                        {
                            if (value > 0x7FF)
                            {
                                msg = String.Format("Попытка установить значение недопустимое значение Id = {0}." +
                                    "Значение индентификатора сообщения не может быть больше 0x7FF (2047) у стандартного фрейма", 
                                    value.ToString());
                                throw new ArgumentOutOfRangeException("Frame.Identifier", msg);
                            }
                            else
                            {
                                this._Identifier = value;
                            }
                            break; 
                        }
                    case FrameFormat.ExtendedFrame:
                        {
                            if (value > 0x1FFFFFFF)
                            {
                                msg = String.Format("Попытка установить значение недопустимое значение Id = {0}." +
                                    "Значение индентификатора сообщения не может быть больше 0x1FFFFFFF (536870911) у расширенного фрейма", 
                                    value.ToString());
                                throw new ArgumentOutOfRangeException("Frame.Identifier", msg);
                            }
                            else
                            {
                                this._Identifier = value;
                            }
                            break;
                        }
                    default:
                        {
                            throw new Exception();
                        }
                }
            }
            get
            {
                return this._Identifier;
            }
        }
        private Byte[] _Data;
        /// <summary>
        /// Данные сообещния
        /// </summary>
        [Description("Поле данных сообщения, не более 8 байт")]
        public Byte[] Data
        {
            get { return _Data; }
            set
            {
                String msg;

                if (this.FrameType == FrameType.REMOTEFRAME)
                {
                    if (value.Length > 0)
                    {
                        msg = "Попытка установить поле данных фрейму типа удалённый запрос";
                        throw new ArgumentException(msg, "Message.Data");
                    }
                    else
                    {
                        _Data = value;
                    }
                }
                else
                {
                    if (value.Length > 8)
                    {
                        msg = String.Format("Длина поля данных от 0...8, а заданно: ", value.Length);
                        //Debug.WriteLine(message);
                        throw new ArgumentOutOfRangeException("Message.Data", msg);
                    }
                    else
                    {
                        _Data = value;
                    }
                }
            }
        }
        public UInt32 TimeStamp;
        private FrameType _FrameType;
        /// <summary>
        /// Тип сообщения
        /// </summary>
        [Description("Тип CAN-сообщения")]
        public FrameType FrameType
        {
            set 
            {
                switch (value)
                {
                    case FrameType.REMOTEFRAME:
                        {
                            this._FrameType = value;
                            // Удалённый запрос не может иметь поля данных, поэтому очищаем
                            // данное поле
                            this._Data = new Byte[0];
                            break; 
                        }
                    case FrameType.DATAFRAME:
                        {
                            this._FrameType = value;
                            break; 
                        }
                    case FrameType.ERRORFRAME:
                        {
                            this._FrameType = value;
                            break;
                        }
                    case FrameType.OVERLOADFRAME:
                        {
                            this._FrameType = value;
                            break;
                        }
                }
            }
            get { return this._FrameType; }
        }
        private FrameFormat _FrameFormat;
        /// <summary>
        /// Формат фрейма передаваемого сообщения. Может быть двух видов
        /// стандартный (ID = 11 бит) и расширенный (ID = 29 бит)
        /// </summary>
        [Description("Формат фрейма передаваемого сообщения. Может быть двух видов" + 
            "стандартный (ID = 11 бит) и расширенный (ID = 29 бит)")]
        public FrameFormat FrameFormat
        {
            get { return this._FrameFormat; }
            set 
            {
                String msg;

                switch (value)
                {
                    case FrameFormat.StandardFrame:
                        {
                            if (this._Identifier > 0x7FFF)
                            {
                                msg = String.Format(
                                    "Невозможно установить стандартный тип фрейма сообщения," + 
                                    "если идентификатор больше 11 бит (расширенный фрейм): {0}",
                                    this._Identifier.ToString());
                                throw new ArgumentException(msg, "Message.FrameFormat");
                            }
                            else
                            {
                                this._FrameFormat = value;
                            }
                            break; 
                        }
                    case FrameFormat.ExtendedFrame:
                        {
                            this._FrameFormat = value;
                            break; 
                        }
                    default:
                        {
                            msg = String.Format(
                                "Попытка установить недопустимый формат фрейма сообщенияе: {0}", 
                                value.ToString());
                            throw new ArgumentException(msg, "Message.FrameFormat"); 
                        }
                }
            }
        }
        /// <summary>
        /// Максимальное значение идентификатора для данного типа сообщения 
        /// </summary>
        public UInt32 IdMaxValue
        {
            get 
            {
                switch (this._FrameFormat)
                {
                    case FrameFormat.StandardFrame:
                        { return 0x7FFF; }
                    case FrameFormat.ExtendedFrame:
                        { return 0x1FFFFFFF; }
                    default:
                        {
                            throw new InvalidOperationException(
                                "Невозможно вернуть значение свойства. Установлен недупустимый формат фрейма сообщения");
                        }
                }
            }
        }
        /// <summary>
        /// Возвращает максимальное значение идентификатора сообщения
        /// для указанного формата кадра сообщения
        /// </summary>
        /// <param name="format">Формат кадра CAN-сообщения</param>
        /// <returns>Максимальное значение идентификатора</returns>
        public static UInt32 GetIdMaxValue(FrameFormat format)
        { 
            //UInt32 id;
            switch (format)
            {
                case FrameFormat.StandardFrame:
                    { return 0x7FFF; }
                case FrameFormat.ExtendedFrame:
                    { return 0x1FFFFFFF; }
                default:
                    {
                        throw new InvalidOperationException(
                            "Невозможно вернуть значение свойства. Установлен недупустимый формат фрейма сообщения");
                    }
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (this.Data != null)
            {
                foreach (Byte item in this.Data)
                {
                    sb.Append(String.Format("0x{0} ", item.ToString("X2")));
                }

                return String.Format("Type: {0}, Format: {1}, TimeStamp: {2}, Id: 0x{3}, Data: {4}",
                    this.FrameType, this.FrameFormat, this.TimeStamp, this.Identifier.ToString("X"), sb.ToString());
            }
            else
            {
                return String.Format("Type: {0}, Format: {1}, TimeStamp: {2}, Id: 0x{3}, Data: null",
                    this.FrameType, this.FrameFormat, this.TimeStamp, this.Identifier.ToString("X"));
            }
            //return base.ToString();
        }

        #region ICloneable Members

        public object Clone()
        {
            Frame frm = new Frame();
            frm._FrameFormat = this._FrameFormat;
            frm._FrameType = this._FrameType;
            frm._Identifier = this._Identifier;
            frm._Data = new byte[this._Data.Length];
            for (int i = 0; i < this._Data.Length; i++)
            {
                frm._Data[i] = this._Data[i];
            }
            return frm;
        }

        #endregion
    }
}
