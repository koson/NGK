using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.DataLinkLayer.CanPort.Fastwel;
using NGK.CAN.DataLinkLayer.CanPort.IXXAT;

namespace NGK.CAN.DataLinkLayer.CanPort
{
    /// <summary>
    /// Базовый класс для Can-портов 
    /// </summary>
    public abstract class CanPortBase: ICanPort
    {
        /// <summary>
        /// Создаёт объект CAN-порта конкретного типа на основе 
        /// строки с параметрами CAN-порта и возвращает инферфейс
        /// </summary>
        /// <param name="configPort">Описатель порта, полученный при 
        /// вызове метода ToString() конкретного класса порта</param>
        /// <returns></returns>
        public static ICanPort Create(string configPort)
        {
            string[] pair;
            Dictionary<string, string> settings = new Dictionary<string, string>();
            // Получаем из строки пары: [параметр:значение]
            string[] parameters = configPort.Split(';');
            // Преобразуем пары в словарь
            foreach (string param in parameters)
            {
                pair = param.Split(':');
                if (pair.Length != 2)
                {
                    throw new InvalidCastException();
                }
                settings.Add(pair[0].Trim(), pair[1].Trim());
            }
            // Проверяем наличие всех необходимых ключей
            if (!(settings.ContainsKey("Manufacturer") && settings.ContainsKey("HardwareType")
                && settings.ContainsKey("PortName") && settings.ContainsKey("BitRate")
                && settings.ContainsKey("Mode") && settings.ContainsKey("FrameFormat")))
            {
                throw new InvalidCastException();
            }

            BaudRate bitRate = (BaudRate)Enum.Parse(typeof(BaudRate), settings["BitRate"]);
            FrameFormat frameFormat = (FrameFormat)Enum.Parse(typeof(FrameFormat), settings["FrameFormat"]);
            PortMode mode = (PortMode)Enum.Parse(typeof(PortMode), settings["Mode"]);

            // На основе полученных данных строим объект порта
            switch (settings["Manufacturer"].ToUpper())
            {
                case "FASTWEL": 
                    {
                        switch(settings["HardwareType"].ToUpper())
                        {
                            case "NIM-351":
                                {
                                    return (ICanPort)new Fastwel.NIM351.CanPort(settings["PortName"],
                                        bitRate, frameFormat, mode); 
                                }
                            default:
                                { throw new NotSupportedException(""); }
                        }
                        //break; 
                    }
                case "IXXAT": 
                    {
                        return (ICanPort)new IXXAT.CanPort(settings["PortName"],
                            bitRate, frameFormat, mode);
                    }
                default:
                    { throw new NotSupportedException(""); }
            }
        }

        public override string ToString()
        {
            return String.Format("Manufacturer:{0}; HardwareType:{1}; PortName:{2}; BitRate:{3}; Mode:{4}; FrameFormat:{5};",
                Manufacturer, HardwareType, PortName, BitRate, Mode, FrameFormat);
        }

        #region ICanPort Members

        public abstract string PortName
        {
            get; set;
        }

        public abstract BaudRate BitRate
        {
            get; set;
        }

        public abstract string HardwareType
        {
            get; 
        }

        public abstract string Manufacturer
        {
            get;
        }

        public abstract Version HardwareVersion
        {
            get;
        }

        public abstract Version SoftwareVersion
        {
            get;
        }

        public abstract bool IsOpen
        {
            get;
        }

        public abstract CanPortStatus PortStatus
        {
            get;
        }

        public abstract PortMode Mode
        {
            get; set;
        }

        public abstract FrameFormat FrameFormat
        {
            get; set;
        }

        public abstract bool ErrorFrameEnable
        {
            get; set;
        }

        public abstract int MessagesToRead
        {
            get;
        }
        public abstract void Open();
        public abstract void Close();
        public abstract void Reset();
        public abstract void Start();
        public abstract void Stop();
        public abstract void WriteMessage(uint identifier, FrameType frameType, 
            FrameFormat frameFormat, byte[] data);
        public abstract void Write(Frame message);
        public abstract bool Read(out Frame message);
        public abstract Frame[] ReadMessages(int count);
        public abstract Frame[] ReadMessages();

        public abstract event EventHandler MessageReceived;
        public abstract event EventHandlerErrorRecived ErrorReceived;
        public abstract event EventHandlerPortChangesStatus PortChangedStatus;

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion

        #region ISerializable Members

        public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, 
            System.Runtime.Serialization.StreamingContext context)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
