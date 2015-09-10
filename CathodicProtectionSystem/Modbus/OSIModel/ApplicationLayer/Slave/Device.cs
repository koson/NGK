using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
//
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel;
using Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes;
using Modbus.OSIModel.ApplicationLayer.Slave.NetworkAPI;
using Modbus.OSIModel.Message;
//
using Common.Controlling;

//===================================================================================
namespace Modbus.OSIModel.ApplicationLayer.Slave
{
    //===============================================================================
    /// <summary>
    /// Класс для реализации Slave - устройства сети modbus
    /// </summary>
    [Serializable]
    public class Device: INetworkFunctions, IManageable
    {
        //---------------------------------------------------------------------------
        /// <summary>
        /// Структура подзапроса или группы читаемых регистров из файла в запросе
        /// с кодом 0x14
        /// </summary>
        public struct Code0x14SubRequest
        {
            public Byte ReferenceType;
            public UInt16 FileNumber;
            public UInt16 RecordNumber;
            public UInt16 RecordLength;
        }
        //---------------------------------------------------------------------------
        #region Fields and Properties
        //---------------------------------------------------------------------------
        /// <summary>
        /// Сетевой адрес устройства
        /// </summary>
        private Byte _Address;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Сетевой адрес устройства
        /// </summary>
        public Byte Address
        {
            get { return _Address; }
            set 
            {
                if ((value == 0) || (value > 247))
                {
                    throw new ArgumentOutOfRangeException("Address",
                        "Попытка установить запрещённое значение сетевого адреса устройства. " +
                        "Адрес должен быть в диапазоне 1...247");
                }
                else
                {
                    this._Address = value;
                }
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Контроллер сети, которой содержит данное устройство
        /// </summary>
        private NetworkController _NetworkController;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает контроллер сети, которой содержит данное устройство
        /// </summary>
        public NetworkController NetworkController
        {
            get { return _NetworkController; }
            set { _NetworkController = value; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Таблица дискретных входов\выходов
        /// </summary>
        private CoilsCollection _Coils;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Таблица дискретных входов\выходов
        /// </summary>
        public CoilsCollection Coils
        {
            get { return this._Coils; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Таблица дискретных входов
        /// </summary>
        private DiscretesInputsCollection _DiscretesInputs;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Таблица дискретных входов
        /// </summary>
        public DiscretesInputsCollection DiscretesInputs
        {
            get { return _DiscretesInputs; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Таблица регистров хранения
        /// </summary>
        private HoldingRegistersCollection _HoldingRegisters;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Таблица регистров хранения
        /// </summary>
        public HoldingRegistersCollection HoldingRegisters
        {
            get { return _HoldingRegisters; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Таблица входных регистров
        /// </summary>
        private InputRegistersCollection _InputRegisters;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Таблица входных регистров
        /// </summary>
        public InputRegistersCollection InputRegisters
        {
            get { return _InputRegisters; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Список файлов данного устройства
        /// </summary>
        private FilesCollection _Files;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Список файлов устройства
        /// </summary>
        public FilesCollection Files
        {
            get { return this._Files; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Описание устройства
        /// </summary>
        private String _Description;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Описание устройства
        /// </summary>
        public String Description
        {
            get { return this._Description; }
            set 
            {
                if (value == null)
                {
                    this._Description = String.Empty;
                }
                else
                {
                    this._Description = value;
                }
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Статус устройства. 
        /// </summary>
        private Status _Status;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Состояние устройства Modbus
        /// </summary>
        public Status Status
        {
            get { return this._Status; }
            set 
            {
                switch (value)
                {
                    case Status.Running:
                        { this.Start(); break; }
                    case Status.Stopped:
                        { this.Stop(); break; }
                    case Status.Paused:
                        {
                            throw new NotSupportedException(
                                "Данное состояние устройством не поддерживается");
                        }
                    default:
                        {
                            throw new NotImplementedException(
                                "Обработка данного состояния устройством не реализована");
                        }
                }
            }
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Constructors
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        public Device()
        {
            this._Status = Status.Stopped;
            this.Address = 1;
            this._Description = String.Empty;

            this._Coils = new CoilsCollection();
            this._Coils.SetOwner(this);
            this._Coils.ListWasChanged += 
                new EventHandler(EventHandler_Coils_ListWasChanged);
            this._DiscretesInputs = new DiscretesInputsCollection();
            this._DiscretesInputs.SetOwner(this);
            this._DiscretesInputs.ListWasChanged += 
                new EventHandler(EventHandler_DiscretesInputs_ListWasChanged);
            this._InputRegisters = new InputRegistersCollection();
            this._InputRegisters.SetOwner(this);
            this._InputRegisters.ListWasChanged += 
                new EventHandler(EventHandler_InputRegisters_ListWasChanged);
            this._HoldingRegisters = new HoldingRegistersCollection();
            this._HoldingRegisters.SetOwner(this);
            this._HoldingRegisters.ListWasChanged += 
                new EventHandler(EventHandler_HoldingRegisters_ListWasChanged);
            this._Files = new FilesCollection();
            this._Files.SetOwner(this);
            this._Files.ListWasChanged += 
                new EventHandler(EventHandler_Files_ListWasChanged);
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="address">Сетевой адрес устройсва</param>
        public Device(Byte address)
        {
            this._Status = Status.Stopped;
            this.Address = address;
            this._Description = String.Empty;

            this._Coils = new CoilsCollection();
            this._Coils.SetOwner(this);
            this._Coils.ListWasChanged +=
                new EventHandler(EventHandler_Coils_ListWasChanged);
            this._DiscretesInputs = new DiscretesInputsCollection();
            this._DiscretesInputs.SetOwner(this);
            this._DiscretesInputs.ListWasChanged +=
                new EventHandler(EventHandler_DiscretesInputs_ListWasChanged);
            this._InputRegisters = new InputRegistersCollection();
            this._InputRegisters.SetOwner(this);
            this._InputRegisters.ListWasChanged +=
                new EventHandler(EventHandler_InputRegisters_ListWasChanged);
            this._HoldingRegisters = new HoldingRegistersCollection();
            this._HoldingRegisters.SetOwner(this);
            this._HoldingRegisters.ListWasChanged +=
                new EventHandler(EventHandler_HoldingRegisters_ListWasChanged);
            this._Files = new FilesCollection();
            this._Files.SetOwner(this);
            this._Files.ListWasChanged +=
                new EventHandler(EventHandler_Files_ListWasChanged);
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Methods
        //---------------------------------------------------------------------------
        /// <summary>
        /// Запускает устройство. Устанавливает активный статус устройству. 
        /// Устройство отвечает на запросы из сети (от мастера сети)
        /// </summary>
        public void Start()
        {
            this._Status = Status.Running;
            // Генерируем событие
            this.OnStatusWasChanged();
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Останавливает устройство. Устройство не реагирует 
        /// на запросы из сети (от мастера сети)
        /// </summary>
        public void Stop()
        {
            this._Status = Status.Stopped;
            // Генерируем событие
            this.OnStatusWasChanged();
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события изменения списка дискретных входов/выходов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_Coils_ListWasChanged(object sender, EventArgs e)
        {
            // Генерируем событие
            this.OnDeviceChangedConfiguration();
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события изменения списка дискретных входов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_DiscretesInputs_ListWasChanged(object sender, EventArgs e)
        {
            // Генерируем событие
            this.OnDeviceChangedConfiguration();
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события изменения списка файлов устройства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_Files_ListWasChanged(object sender, EventArgs e)
        {
            // Генерируем событие
            this.OnDeviceChangedConfiguration();
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события изменения списка регистров ввода/вывода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_HoldingRegisters_ListWasChanged(
            object sender, EventArgs e)
        {
            // Генерируем событие
            this.OnDeviceChangedConfiguration();
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события изменения списка входных регистров
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_InputRegisters_ListWasChanged(
            object sender, EventArgs e)
        {
            // Генерируем событие
            this.OnDeviceChangedConfiguration();
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Метод получает входящие из сети сообщение (от мастера) обрабатываем его
        /// и формирует ответ, если это необходимо (при адресованном запросе)
        /// </summary>
        /// <param name="message">Входящие сообщение</param>
        /// <returns>Исходящие ответное сообщение</returns>
        internal void GetIncommingMessage(Message.Message message)
        {
            // Проверяем статус устройства, если устройство пассивно
            // не обрабатываем запрос и не отвечаем.
            if (this.Status == Status.Running)
            {
                // Проверяем. Это сообщение адресовано данному устройству или нет
                if ((message.Address == this.Address) || (message.Address == 0))
                {
                    // Сообщение предназначено для данного устройства
                    // !!! данную функцию выполнить в отдельном потоке
                    this.RequestParse(message);
                }
                else
                {
                    // Нет, это сообщение не предназначено для данного устройства
                }
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Метод отправляет ответ slave-устройства мастеру сети на его запрос.
        /// </summary>
        /// <param name="answer">Ответное сообщение</param>
        private void SendResponse(Message.Message answer)
        {
            if (this.NetworkController != null)
            {
                this.NetworkController.GetOutcommingMessage(answer);
            }
            else
            {
                throw new NullReferenceException(
                    "Невозможно отправить ответ мастеру на запрос. Недоступен контроллер сети");
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Забирает запрос и вызывает необходимые 
        /// для его выполнения функции
        /// </summary>
        /// <param name="request">Запрос от мастера</param>
        protected virtual void RequestParse(Message.Message request)
        {
            Message.Result result;
            
            // Получаем код команды и анализируем сообщение
            switch (request.PDUFrame.Function)
            {
                case 0x01: // Функция 0x1. Чтение реле (не может быть широковещательной)
                    {
                        result = ((INetworkFunctions)this).ReadCoils(request);
                        break;
                    }
                case 0x02: // Функция 0x2. Читает дискретные входы (не может быть широковещательной)
                    {
                        result = ((INetworkFunctions)this).ReadDiscreteInputs(request);
                        break;
                    }
                case 0x03: // Функция 0х3. Читает holding-регистры (не может быть широковещательной)
                    {
                        result = ((INetworkFunctions)this).ReadHoldingRegisters(request);
                        break;
                    }
                case 0x04: // Функция 0х4. Читает входные регистры (не может быть широковещательной)
                    {
                        result = ((INetworkFunctions)this).ReadInputRegisters(request);
                        break;
                    }
                case 0x05: // Функция 0х5. Устанавливает реле в состояние вкл./выкл.
                    {
                        result = ((INetworkFunctions)this).WriteSingleCoil(request);
                        break;
                    }
                case 0x06: // Функция 0x6. Записывает значение в одиночный регистр
                    {
                        result = ((INetworkFunctions)this).WriteSingleRegister(request);
                        break;
                    }
                case 0x0F:
                    {
                        result = ((INetworkFunctions)this).WriteMultipleCoils(request);
                        break;
                    }
                case 0x10:
                    {
                        result = ((INetworkFunctions)this).WriteMultipleRegisters(request);
                        break;
                    }
                case 0x14:
                    {
                        result = ((INetworkFunctions)this).ReadFileRecord(request);
                        break;
                    }
                default:
                    {
                        result = ((INetworkFunctions)this).FunctionNotSupported(request);
                        break;
                    }
            }

        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Метод устанавливает владелца данного объекта. Метод вызывается владельцем
        /// при добавлении данного устройства в свой список. А так же, вызывается при 
        /// удалении данного устройства из своего списка. При этом поле обнуляется (null)
        /// </summary>
        /// <param name="owner">Будующий владелец данного устройства</param>
        internal void SetOwner(NetworkController owner)
        {
            if (this._NetworkController == null)
            {
                this._NetworkController = owner;
            }
            else
            {
                if (owner == null)
                {
                    // Осовбождаем владельца, теперь устройство свободно
                    this._NetworkController = null;
                }
                else
                {
                    // Если контроллер сети, которой принадлежит данное устройство 
                    // устанавливаемой, тогда ничего не делаем. 
                    // Здесь нет ошибки. В противном случае, генерируем исключение
                    if (this.Equals(owner) == false)
                    {
                        throw new InvalidOperationException(
                            "Данное modbus-устройство уже принадлежит другому контроллеру сети");
                    }
                }
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Генерирует событие MasterChangedCoils
        /// </summary>
        private void OnMasterChangedCoils()
        {
            EventHandler handler = this.MasterChangedCoils;
            EventArgs args = new EventArgs();

            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke syncInvoke =
                        singleCast.Target as System.ComponentModel.ISynchronizeInvoke;

                    if (syncInvoke != null)
                    {
                        if (syncInvoke.InvokeRequired)
                        {
                            syncInvoke.Invoke(singleCast, new Object[] { this, args });
                        }
                        else
                        {
                            singleCast(this, args);
                        }
                    }
                    else
                    {
                        singleCast(this, args);
                    }
                }
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Генерирует событие MasterChangedHoldingRegisters
        /// </summary>
        private void OnMasterChangedHoldingRegisters()
        {
            EventHandler handler = this.MasterChangedHoldingRegisters;
            EventArgs args = new EventArgs();

            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke syncInvoke =
                        singleCast.Target as System.ComponentModel.ISynchronizeInvoke;

                    if (syncInvoke != null)
                    {
                        if (syncInvoke.InvokeRequired)
                        {
                            syncInvoke.Invoke(singleCast, new Object[] { this, args });
                        }
                        else
                        {
                            singleCast(this, args);
                        }
                    }
                    else
                    {
                        singleCast(this, args);
                    }
                }
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Генерирует событие изменения конфигурации модели данных устройства
        /// </summary>
        private void OnDeviceChangedConfiguration()
        {
            EventHandler handler = this.DeviceChangedConfiguration;
            EventArgs args = new EventArgs();

            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke syncInvoke =
                        singleCast.Target as System.ComponentModel.ISynchronizeInvoke;
                    if (syncInvoke != null)
                    {
                        if (syncInvoke.InvokeRequired)
                        {
                            syncInvoke.Invoke(singleCast, new Object[] { this, args });
                        }
                        else
                        {
                            singleCast(this, args);
                        }
                    }
                    else
                    {
                        singleCast(this, args);
                    }
                }
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Генерирует событие изменения статуса устройства
        /// </summary>
        private void OnStatusWasChanged()
        {
            EventHandler handler = this.StatusWasChanged;
            EventArgs args = new EventArgs();

            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke synckInvoke =
                        singleCast.Target as System.ComponentModel.ISynchronizeInvoke;
                    if (synckInvoke != null)
                    {
                        if (synckInvoke.InvokeRequired)
                        {
                            synckInvoke.Invoke(singleCast, new Object[] { this, args });
                        }
                        else
                        {
                            singleCast(this, args);
                        }
                    }
                    else
                    {
                        singleCast(this, args);
                    }
                }
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает объект в виде строки
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Device; Address: 0x");
            sb.Append(this.Address.ToString("X2"));
            sb.Append("; ");

            if (this._NetworkController != null)
            {
                sb.Append("Network: ");
                sb.Append(this._NetworkController.NetworkName);
                sb.Append("; ");
            }
            else
            {
                sb.Append("Network: null; ");
            }

            sb.Append("Description: ");
            sb.Append(this.Description);
            
            return sb.ToString();
            //return base.ToString();
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Events
        //---------------------------------------------------------------------------
        /// <summary>
        /// Событие возникает после того, как мастер-устройство произвело
        /// запись одного или более дискретных входов/выходов
        /// </summary>
        public event EventHandler MasterChangedCoils;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Событие возникает после того, как мастер-устройство произвело
        /// запись одного или более регистров хранения
        /// </summary>
        public event EventHandler MasterChangedHoldingRegisters;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Событие возникает после того, как мастер-устройство произсело
        /// запись в файл
        /// </summary>
        //public event EventHandler MasterRecordedFile;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Событие происходит после изменения конфигурации модели данных устройства
        /// </summary>
        public event EventHandler DeviceChangedConfiguration;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Событие происходит после изменения статуса устройства
        /// </summary>
        public event EventHandler StatusWasChanged;
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        // !!! Необоходимо сделать статическим методами, для того что бы уменьшить
        // размер устройтва в ОЗУ.
        #region INetworkFunctions Members
        //---------------------------------------------------------------------------
        Result INetworkFunctions.ReadCoils(Modbus.OSIModel.Message.Message request)
        {
            Message.Result result;
            Message.Message answer;
            PDU pdu;
            String message;

            if (request.Address == 0)
            {
                // Ошибка. Данная команда не может быть широковещательная
                message = "Широковещательный запрос на выполнение функции 0x01 невозможен";
                result = new Message.Result(Error.RequestError,
                    message, request, null);
            }
            else
            {
                // Проверяем длину PDU (должна равнятся 5 байтам)
                if (request.PDUFrame.ToArray().Length != 5)
                {
                    // Длина сообщения не верная
                    message = String.Format(
                        "Длина PDU-фрайма в запросе {0}. Должна быть 5 байт", request.PDUFrame.ToArray());
                    // Ответное сообщение не отсылается
                    result = new Message.Result(Error.DataFormatError, message,
                        request, null);
                }
                else
                {
                    // Разбираем сообщение
                    Byte[] array = new Byte[2];
                    // Получаем адрес первого реле
                    Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                    UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                    // Получаем количесво реле для чтения
                    Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                    UInt16 quantity = Modbus.Convert.ConvertToUInt16(array);

                    if ((quantity == 0) || (quantity > 2000))
                    {
                        message = String.Format(
                            "Количество реле для чтения в запросе {0}, а должно быть 1...2000", quantity);
                        pdu = new Message.PDU(0x81, new Byte[] { 0x03 });
                        answer = new Modbus.OSIModel.Message.Message(this._Address, pdu);
                        // Отправляем сообщение
                        this.SendResponse(answer);

                        result = new Message.Result(Error.IllegalDataValue,
                            message, request, answer);

                    }
                    else
                    {
                        // Выполняем запрос. Проверяем доступность в 
                        // системе реле в указанном диапазоне адресов
                        for (int i = 0; i < quantity; i++)
                        {
                            // Проверяем существует ли реле с указанным адресом.
                            // Если реле не существует в системе формируем ответ
                            // c исключение

                            if (this.Coils.Contains(System.Convert.ToUInt16(address + i)) == false)
                            {
                                // Формируем ответ с исключение 2
                                answer = new Modbus.OSIModel.Message.Message(this.Address,
                                    new PDU(0x81, new Byte[] { 0x2 }));

                                // Отправляем ответ мастеру
                                this.SendResponse(answer);

                                result = new Message.Result(Error.IllegalDataAddress,
                                    String.Format("Реле с адресом {0} не существует", 
                                    System.Convert.ToUInt16(address + i)),
                                    request, answer);

                                return result;
                            }
                        }

                        // Все реле найдены формируем ответное сообщение
                        int totalBytes = quantity % 8;

                        if (totalBytes == 0)
                        {
                            totalBytes = quantity / 8;
                        }
                        else
                        {
                            totalBytes = 1 + (quantity / 8);
                        }

                        Byte[] data = new Byte[totalBytes];
                        int number = 0;
                        int index = 0;

                        for (int i = 0; i < quantity; i++)
                        {
                            data[index] = (Byte)(data[index] | 
                                (Byte)(Modbus.Convert.BooleanToBit(
                                this.Coils[System.Convert.ToUInt16(address + i)].Value) << number));

                            if (++number > 7)
                            {
                                number = 0;
                                ++index;
                            }
                        }
                        pdu = new Message.PDU();
                        pdu.Function = 0x01;
                        pdu.AddDataByte((byte)totalBytes);  // Добавляем количество байт с состояниями реле
                        pdu.AddDataBytesRange(data);        // Добавляем байты с состояниями реле

                        answer = new Modbus.OSIModel.Message.Message(this.Address, pdu);
                        // Отправляем ответ мастеру
                        this.SendResponse(answer);
                        result = new Message.Result(Error.NoError, String.Empty, request, answer);
                    }
                }
            }
            return result;

        }
        //---------------------------------------------------------------------------
        Result INetworkFunctions.ReadDiscreteInputs(Message.Message request)
        {
            Message.Result result;
            Message.Message answer;
            Message.PDU pdu;
            String message;

            if (request.Address == 0)
            {
                // Ошибка. Данная команда не может быть широковещательная
                message = "Широковещательный запрос на выполнение функции 0x02 невозможен";
                result = new Message.Result(Error.RequestError,
                    message, request, null);
            }
            else
            {
                // Проверяем длину PDU (должна равнятся 5 байтам)
                if (request.PDUFrame.ToArray().Length != 5)
                {
                    // Длина сообщения не верная
                    String mes = String.Format(
                        "Длина PDU-фрайма равна в запросе 0x2 равна {0} байт. Должна быть 5 байт",
                        request.PDUFrame.ToArray().Length);

                    result = new Message.Result(Error.DataFormatError, mes, request, null);
                    Debug.WriteLine(mes);
                }
                else
                {
                    // Разбираем сообщение
                    Byte[] array = new Byte[2];
                    // Получаем адрес первого дискретного входа
                    Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                    UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                    // Получаем количесво дискретных входов для чтения
                    Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                    UInt16 quantity = Modbus.Convert.ConvertToUInt16(array);

                    if ((quantity == 0) || (quantity > 2000))
                    {
                        message =
                            String.Format("Количество дискретных входов для чтения в запросе {0}, а должно быть 1...2000",
                            quantity);
                        pdu = new Message.PDU(0x82, new Byte[] { 0x03 });
                        answer = new Modbus.OSIModel.Message.Message(this._Address, pdu);

                        // Отправляем сообщение
                        this.SendResponse(answer);

                        result = new Message.Result(Error.IllegalDataValue,
                            message, request, answer);
                    }
                    else
                    {
                        // Выполняем запрос. Проверяем доступность в 
                        // системе дискретных входов в указанном диапазоне адресов
                        //DiscreteInput[] inputs = new DiscreteInput[quantity];

                        for (int i = 0; i < quantity; i++)
                        {
                            //inputs[i] = _discretesInputs.Find((UInt16)(address + i));
                            if (this.DiscretesInputs.Contains(System.Convert.ToUInt16(address + i)) == false)
                            {
                            // Если дискретный вход не существует в системе формируем ответ
                            // c исключением 2
                                pdu = new Message.PDU(0x82, new Byte[] { 0x02 });
                                answer = new Message.Message(this._Address, pdu);

                                message =
                                    String.Format("Дискретный вход с адресом {0} не существует",
                                    (address + i));
                                result = new Message.Result(Error.IllegalDataAddress,
                                    message, request, answer);

                                // Отправляем ответ мастеру
                                this.SendResponse(answer);

                                return result;
                            }
                        }

                        // Все дискретные входы найдены формируем ответное сообщение
                        int totalBytes = quantity % 8;

                        if (totalBytes == 0)
                        {
                            totalBytes = quantity / 8;

                            if (totalBytes == 0)
                            {
                                totalBytes++;
                            }
                        }
                        else
                        {
                            totalBytes = 1 + (quantity / 8);
                        }

                        Byte[] data = new Byte[totalBytes];
                        int number = 0;
                        int index = 0;

                        for (int i = 0; i < quantity; i++)
                        {
                            data[index] = (Byte)(data[index] |
                                (Byte)(Modbus.Convert.BooleanToBit(this.DiscretesInputs[address + i].Value) << number));

                            if (++number > 7)
                            {
                                number = 0;
                                ++index;
                            }
                        }
                        pdu = new Message.PDU();
                        pdu.Function = request.PDUFrame.Function;
                        pdu.AddDataByte((Byte)totalBytes);
                        pdu.AddDataBytesRange(data);
                        answer = new Message.Message(this._Address, pdu);

                        result = new Message.Result(Error.NoError, String.Empty, request, answer);
                        // Отправляем ответ мастеру
                        this.SendResponse(answer);
                    }
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Result INetworkFunctions.ReadHoldingRegisters(Message.Message request)
        {
            Message.Result result;
            Message.Message answer;
            Message.PDU pdu;
            String message;

            if (request.Address == 0)
            {
                // Ошибка. Данная команда не может быть широковещательная
                message = "Широковещательный запрос на выполнение функции 0x03 невозможен";
                result = new Message.Result(Error.RequestError,
                    message, request, null);
            }
            else
            {
                // Проверяем длину PDU (должна равнятся 5 байтам)
                if (request.PDUFrame.ToArray().Length != 5)
                {
                    // Длина сообщения не верная
                    String mes = String.Format(
                        "Длина PDU-фрайма равна в запросе 0x3 равна {0} байт. Должна быть 5 байт",
                        request.PDUFrame.ToArray().Length);

                    result = new Message.Result(Error.DataFormatError, mes, request, null);
                }
                else
                {
                    // Разбираем сообщение
                    Byte[] array = new Byte[2];
                    // Получаем адрес первого регистра входа
                    Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                    UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                    // Получаем количесво регистров для чтения
                    Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                    UInt16 quantity = Modbus.Convert.ConvertToUInt16(array);

                    if ((quantity == 0) || (quantity > 125))
                    {
                        message =
                            String.Format("Количество регистров для чтения в запросе {0}, а должно быть 1...125",
                            quantity);
                        pdu = new Message.PDU(0x83, new Byte[] { 0x03 });
                        answer = new Message.Message(this._Address, pdu);
                        // Отправляем сообщение
                        this.SendResponse(answer);

                        result = new Message.Result(Error.IllegalDataValue,
                            message, request, answer);
                    }
                    else
                    {
                        // Выполняем запрос. Проверяем доступность в 
                        // системе регистров в указанном диапазоне адресов
                        for (int i = 0; i < quantity; i++)
                        {
                            // Если регистр не существует в системе формируем ответ
                            // c исключение
                            if (this.HoldingRegisters.Contains(System.Convert.ToUInt16(address + i)) == false)
                            {
                                // Формируем ответ с исключение 2
                                pdu = new Message.PDU(0x83, new Byte[] { 0x02 });
                                answer = new Message.Message(this._Address, pdu);

                                message =
                                    String.Format("Регистр с адресом {0} не существует",
                                    (address + i));
                                result = new Message.Result(Error.IllegalDataAddress,
                                    message, request, answer);

                                // Отправляем ответ мастеру
                                this.SendResponse(answer);

                                return result;
                            }
                        }

                        // Все регистры найдены формируем ответное сообщение
                        Byte[] temp;
                        List<Byte> data = new List<byte>();

                        data.Add(System.Convert.ToByte((quantity * 2)));

                        for (int i = 0; i < quantity; i++)
                        {
                            temp = Modbus.Convert.ConvertToBytes(this.HoldingRegisters[i].Value);
                            data.AddRange(temp);
                        }
                        pdu = new Message.PDU();
                        pdu.Function = 0x3;
                        pdu.AddDataBytesRange(data.ToArray());
                        answer = new Message.Message(this._Address, pdu);

                        result = new Message.Result(Error.NoError, String.Empty, request, answer);
                        // Отправляем ответ мастеру
                        this.SendResponse(answer);
                    }
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Result INetworkFunctions.ReadInputRegisters(Message.Message request)
        {
            Message.Result result;
            Message.Message answer;
            Message.PDU pdu;
            String message;

            if (request.Address == 0)
            {
                // Ошибка. Данная команда не может быть широковещательная
                message = "Широковещательный запрос на выполнение функции 0x04 невозможен";
                result = new Message.Result(Error.RequestError,
                    message, request, null);
            }
            else
            {
                // Проверяем длину PDU (должна равнятся  байтам)
                if (request.PDUFrame.ToArray().Length != 5)
                {
                    // Длина сообщения не верная
                    String mes = String.Format(
                        "Длина PDU-фрайма равна в запросе 0x4 равна {0} байт. Должна быть 5 байт",
                        request.PDUFrame.ToArray().Length);

                    result = new Message.Result(Error.DataFormatError, mes, request, null);
                }
                else
                {
                    // Разбираем сообщение
                    Byte[] array = new Byte[2];
                    // Получаем адрес первого регистра входа
                    Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                    UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                    // Получаем количесво регистров для чтения
                    Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                    UInt16 quantity = Modbus.Convert.ConvertToUInt16(array);

                    if ((quantity == 0) || (quantity > 125))
                    {
                        message =
                            String.Format("Количество регистров для чтения в запросе {0}, а должно быть 1...125",
                            quantity);
                        pdu = new Message.PDU(0x84, new Byte[] { 0x03 });
                        answer = new Message.Message(this._Address, pdu);

                        // Отправляем сообщение
                        this.SendResponse(answer);

                        result = new Message.Result(Error.IllegalDataValue,
                            message, request, answer);
                    }
                    else
                    {
                        // Выполняем запрос. Проверяем доступность в 
                        // системе регистров в указанном диапазоне адресов
                        for (int i = 0; i < quantity; i++)
                        {
                            // Если регистр не существует в системе формируем ответ
                            // c исключением 2
                            if (this.InputRegisters.Contains(System.Convert.ToUInt16(address + i)) == false)
                            {
                                pdu = new Message.PDU(0x84, new Byte[] { 0x02 });
                                answer = new Message.Message(this.Address, pdu);

                                message =
                                    String.Format("Регистр с адресом {0} не существует",
                                    (address + i));
                                result = new Message.Result(Error.IllegalDataAddress,
                                    message, request, answer);

                                // Отправляем ответ мастеру
                                this.SendResponse(answer);
                                return result;
                            }
                        }

                        // Все регистры найдены формируем ответное сообщение
                        Byte[] temp;
                        List<Byte> data = new List<byte>();

                        for (int i = 0; i < quantity; i++)
                        {
                            temp = Modbus.Convert.ConvertToBytes(
                                this.InputRegisters[System.Convert.ToUInt16(address + i)].Value);
                            data.AddRange(temp);
                        }
                        pdu = new Message.PDU();
                        pdu.Function = 0x04;
                        pdu.AddDataByte((Byte)(data.Count));
                        pdu.AddDataBytesRange(data.ToArray());
                        answer = new Message.Message(this._Address, pdu);

                        result = new Result(Error.NoError, String.Empty, request, answer);
                        // Отправляем ответ мастеру
                        this.SendResponse(answer);
                    }
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Result INetworkFunctions.WriteSingleCoil(Message.Message request)
        {
            Message.Result result;
            Message.Message answer;
            Message.PDU pdu;
            String message;

            // Проверяем длину PDU (должна быть 5 байт)
            if (request.PDUFrame.ToArray().Length != 5)
            {
                // Длина сообщения не верная
                String mes = String.Format(
                    "Длина PDU-фрайма равна в запросе 0x05 равна {0} байт. Должна быть 5 байт",
                    request.PDUFrame.ToArray().Length);

                result = new Message.Result(Error.DataFormatError, mes, request, null);
            }
            else
            {
                // Разбираем сообщение
                Byte[] array = new Byte[2];
                // Получаем адрес реле
                Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                // Получаем значение реле для записи
                Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                UInt16 status = Modbus.Convert.ConvertToUInt16(array);
                State coilValue;
                // Получаем количество байт в пасылке
                switch (status)
                {
                    case 0x0000:
                        {
                            coilValue = State.Off;
                            break;
                        }
                    case 0xFF00:
                        {
                            coilValue = State.On;
                            break;
                        }
                    default:
                        {
                            // Ошибка
                            message =
                                String.Format(
                                "Неверный формат данных для кодировки состояния реле {0}, а должно быть 0x0000 или 0xFF00",
                                status.ToString("X4"));
                            pdu = new Message.PDU((Byte)(0x05 | 0x80), new Byte[] { 0x03 });
                            answer = new Modbus.OSIModel.Message.Message(this._Address, pdu);
                            // Отправляем сообщение
                            this.SendResponse(answer);

                            result = new Message.Result(Error.IllegalDataValue,
                                message, request, answer);
                            return result;
                        }
                }

                // Выполняем запрос. Проверяем доступность в 
                // системе регистров в указанном диапазоне адресов

                // Если регистр не существует в системе формируем ответ
                // c исключение
                if (this.Coils.Contains(address) == false)
                {
                    // Формируем ответ с исключение 2
                    pdu = new Message.PDU((Byte)(0x80 | 0x05), new Byte[] { 0x02 });
                    answer = new Modbus.OSIModel.Message.Message(this._Address, pdu);
                    message =
                        String.Format("Реле с адресом {0} не существует",
                            address);
                    result = new Message.Result(Error.IllegalDataAddress,
                        message, request, answer);

                    // Отправляем ответ мастеру
                    this.SendResponse(answer);
                    return result;
                }
                else
                {
                    // Реле найдено, устанавливаем новое значение и
                    // формируем ответное сообщение
                    this.Coils[address].Value = Modbus.Convert.ToBoolean(coilValue);

                    // Формируем ответ.
                    List<Byte> data_ = new List<byte>();
                    data_.AddRange(Modbus.Convert.ConvertToBytes(address));
                    data_.AddRange(Modbus.Convert.StateToArray(coilValue));

                    pdu = new Message.PDU();
                    pdu.Function = 0x05;
                    pdu.AddDataBytesRange(data_.ToArray());
                    answer = new Message.Message(this._Address, pdu);

                    result = new Message.Result(Error.NoError, String.Empty, request, answer);
                    // Отправляем ответ мастеру
                    this.SendResponse(answer);
                    // Формируем событие
                    this.OnMasterChangedCoils();
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Result INetworkFunctions.WriteSingleRegister(Message.Message request)
        {
            Message.Result result;
            Message.Message answer;
            Message.PDU pdu;
            String message;

            // Проверяем длину PDU (должна быть 5 байт)
            if (request.PDUFrame.ToArray().Length != 5)
            {
                // Длина сообщения не верная
                String mes = String.Format(
                    "Длина PDU-фрайма равна в запросе 0x06 равна {0} байт. Должна быть 5 байт",
                    request.PDUFrame.ToArray().Length);

                result = new Message.Result(Error.DataFormatError, mes, request, null);
            }
            else
            {
                // Разбираем сообщение
                Byte[] array = new Byte[2];
                // Получаем адрес регистра
                Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                // Получаем значение регистра
                Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                UInt16 value = Modbus.Convert.ConvertToUInt16(array);

                // Выполняем запрос. Проверяем доступность в 
                // регистра 
                if (this._HoldingRegisters.Contains(address) == false)
                {
                    // Если регистр не существует в системе формируем ответ
                    // c исключение
                    // Формируем ответ с исключение 2
                    pdu = new Message.PDU((Byte)(0x06 | 0x80), new Byte[] { 0x02 });
                    answer = new Message.Message(this._Address, pdu);

                    message =
                        String.Format("Регистр с адресом {0} не существует", address);
                    
                    result = new Message.Result(Error.IllegalDataAddress,
                        message, request, answer);

                    // Отправляем ответ мастеру
                    this.SendResponse(answer);

                    return result;
                }
                else
                {
                    // Все регистры найдены, устанавливаем новые значения и
                    // формируем ответное сообщение
                    this._HoldingRegisters[address].Value = value;

                    // Формируем ответ.
                    pdu = new Message.PDU();
                    pdu.Function = 0x06;
                    pdu.AddDataBytesRange(Modbus.Convert.ConvertToBytes(address));
                    pdu.AddDataBytesRange(Modbus.Convert.ConvertToBytes(value));
                    answer = new Modbus.OSIModel.Message.Message(this._Address, pdu);

                    result = new Message.Result(Error.NoError, String.Empty, request, answer);
                    // Отправляем ответ мастеру
                    this.SendResponse(answer);
                    // Формируем событие
                    this.OnMasterChangedHoldingRegisters();
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Result INetworkFunctions.WriteMultipleCoils(Message.Message request)
        {
            Message.Result result;
            Message.Message answer;
            Message.PDU pdu; 
            String message;


            // Проверяем длину PDU (должна быть не менее 7 байтам)
            if (request.PDUFrame.ToArray().Length < 7)
            {
                // Длина сообщения не верная
                String mes = String.Format(
                    "Длина PDU-фрайма равна в запросе 0x0F равна {0} байт. Должна быть не менее 7 байт",
                    request.PDUFrame.ToArray().Length);

                result = new Message.Result(Error.DataFormatError, mes, request, null);
            }
            else
            {
                // Разбираем сообщение
                Byte[] array = new Byte[2];
                // Получаем адрес первого реле
                Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                // Получаем количесво реле для записи
                Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                UInt16 quantity = Modbus.Convert.ConvertToUInt16(array);
                // Получаем количество байт в пасылке
                Byte count = request.PDUFrame.Data[4];

                int totalBytes = quantity % 8;

                if (totalBytes == 0)
                {
                    totalBytes = quantity / 8;
                }
                else
                {
                    totalBytes = 1 + (quantity / 8);
                }

                if ((quantity == 0) || (quantity > 0x7B0) || (totalBytes != count))
                {
                    message =
                        String.Format("Количество реле для записи в запросе равно {0}, а должно быть 1...0x7B0",
                        quantity);
                    pdu = new Message.PDU((Byte)(0xF | 0x80), new Byte[] { 0x03 });
                    answer = new Message.Message(this._Address, pdu);

                    // Отправляем сообщение
                    this.SendResponse(answer);

                    result = new Message.Result(Error.IllegalDataValue,
                        message, request, answer);
                }
                else
                {
                    // Выполняем запрос. Проверяем доступность в 
                    // системе регистров в указанном диапазоне адресов
                    for (int i = 0; i < quantity; i++)
                    {
                        // Если регистр не существует в системе формируем ответ
                        // c исключение
                        if (this.Coils.Contains(System.Convert.ToUInt16(address + i)) == false)
                        {
                            // Формируем ответ с исключение 2
                            pdu = new Message.PDU((Byte)(0x80 | 0x0F), new Byte[] { 0x02 });
                            answer = new Message.Message(this.Address, pdu);

                            message =
                                String.Format("Реле с адресом {0} не существует",
                                (address + i));
                            result = new Message.Result(Error.IllegalDataAddress,
                                message, request, answer);

                            // Отправляем ответ мастеру
                            this.SendResponse(answer);
                            return result;
                        }
                    }

                    // Все реле найдены, устанавливаем новые значения и
                    // формируем ответное сообщение
                    Byte status;
                    // Устанавливаем новые значения в реле

                    for (int i = 0; i < count; i++)
                    {
                        status = request.PDUFrame.Data[5 + i];

                        for (int y = 0; y < 8; y++)
                        {
                            totalBytes = i * 8 + y;

                            if (totalBytes < quantity)
                            {
                                if (((status >> y) & 0x01) == 0)
                                {
                                    this.Coils[System.Convert.ToUInt16(address + totalBytes)].Value = 
                                        Modbus.Convert.ToBoolean(State.Off); 
                                    //rows[totalBytes]["Value"] = Modbus.Convert.ToBoolean(State.Off);
                                }
                                else
                                {
                                    this.Coils[System.Convert.ToUInt16(address + totalBytes)].Value =
                                        Modbus.Convert.ToBoolean(State.On);
                                    //rows[totalBytes]["Value"] = Modbus.Convert.ToBoolean(State.On);
                                }
                            }
                        }
                    }
                    // Формируем ответ.
                    List<Byte> data_ = new List<byte>();
                    data_.AddRange(Modbus.Convert.ConvertToBytes(address));
                    data_.AddRange(Modbus.Convert.ConvertToBytes(quantity));

                    pdu = new Message.PDU();
                    pdu.Function = 0x0F;
                    pdu.AddDataBytesRange(data_.ToArray());
                    answer = new Message.Message(this._Address, pdu);

                    result = new Message.Result(Error.NoError, String.Empty, request, answer);
                    // Отправляем ответ мастеру
                    this.SendResponse(answer);
                    // Формируем событие
                    this.OnMasterChangedCoils();
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Result INetworkFunctions.WriteMultipleRegisters(Message.Message request)
        {
            Message.Result result;
            Message.Message answer;
            Message.PDU pdu;
            String message;

            // Проверяем длину PDU (должна быть не менее 8 байтам)
            if (request.PDUFrame.ToArray().Length < 8)
            {
                // Длина сообщения не верная
                String mes = String.Format(
                    "Длина PDU-фрайма равна в запросе 0x10 равна {0} байт. Должна быть не менее 8 байт",
                    request.PDUFrame.ToArray().Length);

                result = new Message.Result(Error.DataFormatError, mes, request, null);
            }
            else
            {
                // Разбираем сообщение
                Byte[] array = new Byte[2];
                // Получаем адрес первого регистра
                Array.Copy(request.PDUFrame.Data, 0, array, 0, 2);
                UInt16 address = Modbus.Convert.ConvertToUInt16(array);
                // Получаем количесво регистров для записи
                Array.Copy(request.PDUFrame.Data, 2, array, 0, 2);
                UInt16 quantity = Modbus.Convert.ConvertToUInt16(array);
                // Получаем количество байт в пасылке
                Byte count = request.PDUFrame.Data[4];

                if ((quantity == 0) || (quantity > 123) || ((quantity * 2) != count))
                {
                    message =
                        String.Format("Количество регистров для записи в запросе равно {0}, а должно быть 1...123",
                        quantity);
                    pdu = new Message.PDU((Byte)(0x10 | 0x80), new Byte[] { 0x03 });
                    answer = new Modbus.OSIModel.Message.Message(this._Address, pdu);

                    // Отправляем сообщение
                    this.SendResponse(answer);

                    result = new Message.Result(Error.IllegalDataValue,
                        message, request, answer);
                }
                else
                {
                    // Выполняем запрос. Проверяем доступность в 
                    // системе регистров в указанном диапазоне адресов
                    for (int i = 0; i < quantity; i++)
                    {
                        // Если регистр не существует в системе формируем ответ
                        // c исключение
                        if (this.HoldingRegisters.Contains(System.Convert.ToUInt16(address + i)) == false)
                        {
                            // Формируем ответ с исключение 2
                            pdu = new Message.PDU((Byte)(0x10 | 0x80), new Byte[] { 0x02 });
                            answer = new Message.Message(this._Address, pdu);
                            message =
                                String.Format("Регистр с адресом {0} не существует",
                                (address + i));
                            result = new Message.Result(Error.IllegalDataAddress,
                                message, request, answer);

                            // Отправляем ответ мастеру
                            this.SendResponse(answer);
                            return result;
                        }
                    }

                    // Все регистры найдены, устанавливаем новые значения и
                    // формируем ответное сообщение
                    Byte[] temp = new Byte[2];
                    List<Byte> data = new List<byte>();

                    // Устанавливаем новые значения в регистры
                    for (int i = 0; i < quantity; i++)
                    {
                        Array.Copy(request.PDUFrame.Data, (5 + (i * 2)), temp, 0, 2);
                        this._HoldingRegisters[System.Convert.ToUInt16(address + i)].Value = 
                            Modbus.Convert.ConvertToUInt16(temp);
                    }

                    // Формируем ответ.
                    temp = Modbus.Convert.ConvertToBytes(address);
                    data.AddRange(temp);
                    temp = Modbus.Convert.ConvertToBytes(quantity);
                    data.AddRange(temp);

                    pdu = new Message.PDU();
                    pdu.Function = 0x10;
                    pdu.AddDataBytesRange(data.ToArray());
                    answer = new Message.Message(this._Address, pdu);
                    result = new Message.Result(Error.NoError, String.Empty, request, answer);
                    // Отправляем ответ мастеру
                    this.SendResponse(answer);
                    // Формируем событие
                    this.OnMasterChangedHoldingRegisters();
                }
            }
            return result;
        }
        //---------------------------------------------------------------------------
        Result INetworkFunctions.ReadFileRecord(Message.Message request)
        {
            Message.Result result;
            Message.Message answer;
            //Message.PDU pdu;
            String message;
            UInt16 var;


            Code0x14SubRequest[] subRequestList;

            const int ByteCountIndex = 0; // Индекс параметра Byte Count в массиве данных запроса  

            if (request.Address == 0)
            {
                // Ошибка. Данная команда не может быть широковещательная
                message = "Широковещательный запрос на выполнение функции 0x14 невозможен";
                result = new Message.Result(Error.RequestError,
                    message, request, null);
                return result;
            }
            else
            {
                if (request.PDUFrame.Length < 7)
                {
                    // Ошибка. Минимальная длина pdu запроса 0х14 это 7 байт
                    message = "Ошибка. Длина pdu запроса 0x14 менее 7 байт";
                    answer = new Message.Message(this._Address,
                        new PDU((Byte)(0x14 | 0x80), new byte[] { 0x03 }));

                    result = new Message.Result(Error.RequestError,
                        message, request, answer);
                    // Отправляем ответ
                    this.SendResponse(answer);
                    return result;
                }
                else
                {
                    if ((request.PDUFrame.Data[ByteCountIndex] < 0x07) ||
                        (request.PDUFrame.Data[ByteCountIndex] > 0xF5))
                    {
                        // Ошибка. Указанная длина Byte Count вне допустимого диапазона значений
                        message = String.Format(
                            "Ошибка. Код функции 0x14. Параметр Byte Count = {0} вне допустимого диапазона 0x07...0xF5",
                            request.PDUFrame.Data[ByteCountIndex], (request.PDUFrame.Length - 2));

                        answer = new Message.Message(this._Address,
                        new PDU((Byte)(0x14 | 0x80), new byte[] { 0x03 }));

                        result = new Message.Result(Error.RequestError,
                            message, request, answer);
                        // Отправляем ответ
                        this.SendResponse(answer);
                        return result;
                    }
                    else
                    {
                        // Проверяем фактическую длину запроса и величину переданную 
                        // в запросе "Byte Count"
                        if ((Int32)request.PDUFrame.Data[ByteCountIndex] != (request.PDUFrame.Length - 2))
                        {
                            // Ошибка. Указанная длина не совпадает с фактической
                            message = String.Format(
                                "Ошибка. Параметр Byte Count = {0} не совпадает с фактической длиной {1}",
                                request.PDUFrame.Data[ByteCountIndex], (request.PDUFrame.Length - 2));

                            answer = new Message.Message(this._Address,
                            new PDU((Byte)(0x14 | 0x80), new byte[] { 0x03 }));

                            result = new Message.Result(Error.RequestError,
                                message, request, answer);
                            // Отправляем ответ
                            this.SendResponse(answer);
                            return result;
                        }
                        else
                        {
                            // Определяем количество групп (подзапросов) в данном запросе
                            if ((request.PDUFrame.Data.Length - 1) % 7 != 0)
                            {
                                // Длина подзапроса всегда равна 7, поэтому должно быть
                                // кратно. Если при делении на 7, остаток не равен 0, то
                                // данные не корректны
                                // Ошибка. Указанная длина не совпадает с фактической
                                message = String.Format("Ошибка. Неверная длина данных в запросе");

                                answer = new Message.Message(this._Address,
                                new PDU((Byte)(0x14 | 0x80), new byte[] { 0x03 }));

                                result = new Message.Result(Error.RequestError,
                                    message, request, answer);
                                // Отправляем ответ
                                this.SendResponse(answer);
                                return result;

                            }
                            else
                            {
                                // Подзапросы имеют корректный формат. Получаем их
                                // Получаем количество подзапросов
                                subRequestList = new Code0x14SubRequest[((request.PDUFrame.Data.Length - 1) / 7)];
                                // Получаем сами подзапросы
                                for (int i = 0; i < subRequestList.Length; i++)
                                {
                                    subRequestList[i].ReferenceType = request.PDUFrame.Data[(7 * i + 1)];
                                    var = (UInt16)(((UInt16)request.PDUFrame.Data[(7 * i + 2)]) << 8); // Hi
                                    var = (UInt16)(var | request.PDUFrame.Data[(7 * i + 3)]); // Lo
                                    subRequestList[i].FileNumber = var;
                                    var = (UInt16)(((UInt16)request.PDUFrame.Data[(7 * i + 4)]) << 8); // Hi
                                    var = (UInt16)(var | request.PDUFrame.Data[(7 * i + 5)]); // Lo
                                    subRequestList[i].RecordNumber = var;
                                    var = (UInt16)(((UInt16)request.PDUFrame.Data[(7 * i + 6)]) << 8); // Hi
                                    var = (UInt16)(var | request.PDUFrame.Data[(7 * i + 7)]); // Lo
                                    subRequestList[i].RecordLength = var;
                                }
                                // Проверяем корректность данных в подзапросах 
                                for (int i = 0; i < subRequestList.Length; i++)
                                {
                                    // Проверяем значение параметра "The reference type". Оно всегда
                                    // должно равняться 0x6
                                    if (subRequestList[i].ReferenceType != 0x6)
                                    {
                                        // Ошибка
                                        answer = new Message.Message(this._Address,
                                            new PDU((Byte)(0x80 | 0x14), new byte[] { 0x2 }));
                                        message = "Ошибка: Подзапрос имеет недопустимое значение праметра The reference type";
                                        result = new Result(Error.RequestError, message, request, answer);
                                        
                                        this.SendResponse(answer);
                                        
                                        return result;
                                    }
                                    else
                                    {
                                        // Проверяем адрес старовтовой записы. Должен быть 
                                        // не более 10000 (0x270F)
                                        if (subRequestList[i].RecordNumber > 0x270F)
                                        {
                                            // Ошибка
                                            answer = new Message.Message(this._Address,
                                                new PDU((Byte)(0x80 | 0x14), new byte[] { 0x2 }));
                                            message = "Ошибка: Подзапрос имеет недопустимое значение номера записи файла";
                                            result = new Result(Error.RequestError, message, request, answer);

                                            this.SendResponse(answer);

                                            return result;
                                        }
                                        else
                                        {
                                            // Проверяем конечный адрес (Начальный адрес + длина блока)
                                            if (subRequestList[i].RecordNumber + subRequestList[i].RecordLength > 0x270F)
                                            {
                                                // Ошибка
                                                answer = new Message.Message(this._Address,
                                                    new PDU((Byte)(0x80 | 0x14), new byte[] { 0x2 }));
                                                message = "Ошибка: Подзапрос имеет недопустимое значение длины блока читаемых записей из файла";
                                                result = new Result(Error.RequestError, message, request, answer);

                                                this.SendResponse(answer);

                                                return result;
                                            }
                                            else
                                            {
                                                // Все проверки пройдены. Теперь можно получать конечные 
                                                // значения
                                            }
                                        }
                                    }
                                }
                                
                                // Определяем общую длину ответа и проверяем превышает ли она допустимый размер
                                var = 2; // Function code + Resp. Data length
                                for (int i = 0; i < subRequestList.Length; i++)
                                {
                                    var = System.Convert.ToUInt16(var + 2); // File resp. length + Ref. Type 
                                    var = System.Convert.ToUInt16(var + (subRequestList[i].RecordLength * 2));
                                }

                                if (var > 253)
                                {
                                    // Ошибка. Длина запрощенных данных превышает максимальную
                                    // длину PDU ответного пакета
                                    answer = new Message.Message(this._Address,
                                        new PDU((Byte)(0x80 | 0x14), new byte[] { 0x2 }));
                                    message = "Ошибка: Длина запрошенных данных превышает максимальную длину PDU";
                                    result = new Result(Error.RequestError, message, request, answer);

                                    this.SendResponse(answer);

                                    return result;
                                }
                                else
                                {
                                    // Если оказались в этой точке, значит запрос корректен. Получаем значения
                                    Code0x14SubRequest subRequest;
                                    List<byte> list = new List<byte>(var);
                                    list.Add(System.Convert.ToByte(var - 2)); // Поле Resp. data length

                                    for (int i = 0; i < subRequestList.Length; i++)
                                    {
                                        subRequest = subRequestList[i];

                                        if (this._Files.Contains(subRequest.FileNumber) == true)
                                        {
                                            // Файл найден. Получаем его записи
                                            list.Add(System.Convert.ToByte(subRequest.RecordLength * 2 + 1)); // File resp. length
                                            list.Add(0x06); // Ref. type

                                            File file = this._Files[subRequest.FileNumber];
                                            
                                            for (int y = 0; y < subRequest.RecordLength; y++)
                                            {
                                                // Проверяем существует ли запись с данным номером
                                                var = System.Convert.ToUInt16(
                                                    subRequest.RecordNumber + System.Convert.ToUInt16(y)); 
                                                if (file.Records.Contains(var) == true)
                                                {
                                                    // Указанная запись существует. Получаем её значение
                                                    // Сохраняем в ответе в виде последоватлеьности байт
                                                    list.AddRange(Modbus.Convert.ConvertToBytes(
                                                        file.Records[var].Value));
                                                }
                                                else
                                                {
                                                    // Указанной записи не существует в данном файле
                                                    // Ошибка. Длина запрощенных данных превышает максимальную
                                                    // длину PDU ответного пакета
                                                    answer = new Message.Message(this._Address,
                                                        new PDU((Byte)(0x80 | 0x14), new byte[] { 0x4 }));
                                                    message = String.Format(
                                                        "Ошибка: Не найдена запрашиваемая запись с номером {0} в запрашиваемом файле c номером {0}",
                                                        var, file.Number);
                                                    result = new Result(Error.RequestError, message, request, answer);

                                                    this.SendResponse(answer);

                                                    return result;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // Файла с таким номером не существует. Возвращает исключение
                                            answer = new Message.Message(this._Address,
                                                new PDU((Byte)(0x80 | 0x14), new byte[] { 0x4 }));
                                            message = String.Format(
                                                "Ошибка: Не найден запрашиваемый файл c номером {0}",
                                                subRequest.FileNumber);
                                            result = new Result(Error.RequestError, message, request, answer);

                                            this.SendResponse(answer);

                                            return result;
                                        }
                                    }

                                    // Если оказались в данной точке, значит предыдущий цикл выполнился устпешно
                                    // Можно отправлять данные
                                    answer = new Message.Message(this._Address,
                                        new PDU(0x14, list.ToArray()));
                                    message = String.Empty;
                                    result = new Result(Error.NoError, message, request, answer);

                                    this.SendResponse(answer);

                                    return result;
                                }
                            }
                        }
                    }
                }
            }
            //return result;
        }
        //---------------------------------------------------------------------------
        Result INetworkFunctions.FunctionNotSupported(Message.Message request)
        {
            Message.Message answer;
            Message.PDU pdu = new Message.PDU();
            pdu.Function = (Byte)(request.PDUFrame.Function | 0x80);
            pdu.AddDataByte(0x01);   //Error.IllegalFunction
            answer = new Modbus.OSIModel.Message.Message(this.Address, pdu);

            // Отправляем ответ
            this.SendResponse(answer);

            Message.Result result =
                new Message.Result(Error.IllegalFunction,
                    "Функция не поддерживается данным устройством",
                    request, answer);
            return result;
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region IManageable Members
        //---------------------------------------------------------------------------
        void IManageable.Start()
        {
            this.Start();
        }
        //---------------------------------------------------------------------------
        void IManageable.Stop()
        {
            this.Stop();
        }
        //---------------------------------------------------------------------------
        void IManageable.Suspend()
        {
            throw new NotSupportedException("Данный метод не поддерживается");
        }
        //---------------------------------------------------------------------------
        Status IManageable.Status
        {
            get
            {
                return this.Status;
            }
            set
            {
                this.Status = value;
            }
        }
        //---------------------------------------------------------------------------
        event EventHandler IManageable.StatusWasChanged
        {
            add 
            {
                this.StatusWasChanged += value;
            }
            remove 
            {
                this.StatusWasChanged -= value;
            }
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file