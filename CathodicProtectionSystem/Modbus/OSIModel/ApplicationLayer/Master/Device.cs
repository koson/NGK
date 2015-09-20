using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Modbus.OSIModel.ApplicationLayer.Master
{
    /// <summary>
    /// Класс для реализации функционала протокола modbus
    /// при работе сервера в режиме Master
    /// </summary>
    public class Device : IApi
    {
        #region Constructors and Destructor
        /// <summary>
        /// Запрещённый конструктор по умолчанию
        /// </summary>
        private Device() { }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="nameNetwork">Имя Modbus-сети</param>
        /// <param name="dataLinkObject">Объект уровня DataLink Layer</param>
        public Device(String nameNetwork,
            Modbus.OSIModel.DataLinkLayer.Master.IDataLinkLayer dataLinkObject)
        {            
            // Задаём имя сервера
            _name = nameNetwork;

            // Задаём объект уровня DataLinkLayer
            _dataLinkObject = dataLinkObject;
            
            // Сервер находится в режиме простоя
            StopTransaction();

            return;
        }
        #endregion

        #region Fields and Properties
        //---------------------------------------------------------------------------
        /// <summary>
        /// Хранит объект уровня DataLink Layer (реализующий его интерфейс)
        /// </summary>
        private Modbus.OSIModel.DataLinkLayer.Master.IDataLinkLayer _dataLinkObject;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Название Modbus-сети
        /// </summary>
        private String _name;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Название Modbus-сети
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Флаг, если установлен указывает, что сервер 
        /// находится в состоянии транзакции "запрос-ответ"
        /// </summary>
        private bool _flgBusy;
        //---------------------------------------------------------------------------
        #endregion

        #region Methods
        //---------------------------------------------------------------------------
        /// <summary>
        /// Метод устанавливает состояние сервера 
        /// в транзацию "Запрос/ответ"
        /// </summary>
        private void StartTransaction()
        {
            _flgBusy = true;
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Метод устанавливает состояние сервера
        /// в режим простоя (транзация завершена)
        /// </summary>
        private void StopTransaction()
        {
            _flgBusy = false;
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Метод разбирает полученое сообщение и если 
        /// код функции содержит признак ошибки (единица в старшем разряде)
        /// разбирает сообщение и формирует ответ (результат запроса к устройству)
        /// Если ошибок не было возвращается false и result == null, если ошибка
        /// присутствует возвращается true и result != null/
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <param name="answer">Ответное сообщение</param>
        /// <param name="result">Результат разбора сообщения</param>
        /// <returns>Результат выполнения запроса: ошибка - true, 
        /// нет ошибок - false</returns>
        private Boolean IsError(
            Modbus.OSIModel.Message.Message request,
            Modbus.OSIModel.Message.Message answer, 
            out Modbus.OSIModel.Message.Result result)
        {
            // Разбираем сообщение
            if ((answer.PDUFrame.Function & 0x80) == 0x80)
            {
                // Устройство не смогло выполнить запрос, определяем причину
                switch (answer.PDUFrame.Data[0])
                {
                    case (Byte)Error.Acknowledge:
                        {
                            result = new Modbus.OSIModel.Message.Result(Error.Acknowledge,
                                "Подчиненный принял запрос и обрабатывает его, но это требует много времени. " +
                                "Этот ответ предохраняет главного от генерации ошибки таймаута. " + 
                                "Главный может выдать команду Poll Program Complete для обнаружения" + 
                                "завершения обработки команды.",
                                request, answer);
                            break;
                        }
                    case (Byte)Error.IllegalDataAddress:
                        {
                            result = new Modbus.OSIModel.Message.Result(Error.IllegalDataAddress,
                                "Адрес данных указанный в запросе не доступен данному подчиненному.",
                                request, answer);
                            break;
                        }
                    case (Byte)Error.IllegalDataValue:
                        {
                            result = new Modbus.OSIModel.Message.Result(Error.IllegalDataValue,
                                "Величина содержащаяся в поле данных запроса является не допустимой величиной для подчиненного.",
                                request, answer);
                            break;
                        }
                    case (Byte)Error.IllegalFunction:
                        {
                            result = new Modbus.OSIModel.Message.Result(Error.IllegalFunction,
                                "Принятый код функции не может быть обработан на подчиненном.",
                                request, answer);
                            break;
                        }
                    case (Byte)Error.MemoryParityError:
                        {
                            result = new Modbus.OSIModel.Message.Result(Error.MemoryParityError,
                                "Подчиненный пытается читать расширенную память, но обнаружил ошибку паритета. Главный может повторить запрос, но обычно в таких случаях требуется ремонт.",
                                request, answer);
                            break;
                        }
                    case (Byte)Error.NegativeAcknowledge:
                        {
                            result = new Modbus.OSIModel.Message.Result(Error.NegativeAcknowledge,
                                "Подчиненный не может выполнить программную функцию, принятую в запросе. Этот код возвращается для неудачного программного запроса, использующего функции с номерами 13 или 14. Главный должен запросить диагностическую информацию или информацию обошибках с подчиненного.",
                                request, answer);
                            break;
                        }
                    case (Byte)Error.SlaveDeviceBusy:
                        {
                            result = new Modbus.OSIModel.Message.Result(Error.SlaveDeviceBusy,
                                "Подчиненный занят обработкой команды. Главный должен повторить сообщение позже, когда  подчиненный освободится.",
                                request, answer);
                            break;
                        }
                    case (Byte)Error.SlaveDeviceFailure:
                        {
                            result = new Modbus.OSIModel.Message.Result(Error.SlaveDeviceFailure,
                                "Невосстанавливаемая ошибка имела место пока подчиненный пытался выполнить затребованное действие.",
                                request, answer);
                            break;
                        }
                    default:
                        {
                            result = new Modbus.OSIModel.Message.Result(Error.UnknownError,
                                "Неизвестная ошибка", request, answer);
                            break;
                        }
                }
                return true;
            }
            else
            {
                // Ошибок нет
                result = null;
                return false;
            }

        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 0х1. Чтение реле
        /// </summary>
        /// <param name="addressSlave">адрес удалённого slave-устройства, 
        /// из которого нужно получить данные</param>
        /// <param name="startingAddress">Адрес начального реле</param>
        /// <param name="quantity">Количество реле для чтения 1...2000</param>
        /// <param name="answer">Ответное сообщение</param>
        /// <param name="coils">Прочитанные состояния реле</param>
        /// <returns>Результат выполнения операции</returns>
        public Modbus.OSIModel.Message.Result ReadCoils(
            Modbus.OSIModel.Message.Message request,
            out State[] coils)
        {
            Modbus.OSIModel.Message.Message answer;
            Modbus.OSIModel.Message.Result result;
            Byte[] arrTemp = new Byte[2];
            
            Array.Copy(request.PDUFrame.Data, 2, arrTemp, 0, 2);

            UInt16 quantity = Modbus.Convert.ConvertToUInt16(arrTemp);
            coils = new State[quantity];

            while (_flgBusy)
            {
                // Если идёт транзакция ждём окончания
            }

            // Устанавливаем начало транзакции
            StartTransaction();
    
            // Отправляем запрос
            Modbus.OSIModel.DataLinkLayer.RequestError error =
                _dataLinkObject.SendMessage(request, out answer);

            switch (error)
            {
                case Modbus.OSIModel.DataLinkLayer.RequestError.NoError:
                    {
                        // Разбираем сообщение на предмет ошибок
                        if (IsError(request, answer, out result))
                        {
                            // Ошибка была
                            break;
                        }
                        else
                        {
                            // Ошибки нет возвращаем результат выполнения запроса
                            // Проверяем содержимое посылки
                            // Рассчитываем необходимое количество байт для передачи состояний реле
                            int length = quantity / 8;

                            if (quantity % 8 != 0)
                            {
                                ++length;
                            }

                            if ((int)answer.PDUFrame.Data[0] != length)
                            {
                                result = new Message.Result(Error.DataFormatError,
                                    String.Format(
                                    "Количество байт {0} в сообщении не равно требуемому {1} для передачи состояний реле в количестве {2}",
                                    answer.PDUFrame.Data[0], length, quantity),
                                    request, answer);
                            }
                            else
                            {
                                Byte data, temp;
                                
                                for (int i = 0; i < answer.PDUFrame.Data[0]; i++)
                                {
                                    data = answer.PDUFrame.Data[i + 1];

                                    for (int y = 0; y < 8; y++)
                                    {
                                        if (8 * i + y < quantity)
                                        {
                                            temp = (Byte)(data >> y);

                                            if ((temp & 0x01) == 0x01)
                                            {
                                                coils[(8 * i + y)] = State.On;
                                            }
                                            else
                                            {
                                                coils[(8 * i + y)] = State.Off;
                                            }
                                        }
                                    }
                                }
                            }
                            result = new Message.Result(Error.NoError, String.Empty,
                                request, answer);
                            
                            break;
                        }
                    }
                case Modbus.OSIModel.DataLinkLayer.RequestError.TimeOut:
                    {
                        // Таймаут ответа
                        result = new Modbus.OSIModel.Message.Result(Error.TimeOut,
                            "Ответ не был получен в заданное время", request, null);
                        break;
                    }
                default:
                    {
                        // Ошибка уровня Datalink layer.
                        result = new Modbus.OSIModel.Message.Result(Error.ReciveMessageError,
                            error.ToString(), request, null);
                        break;
                    }
            }
            StopTransaction();
            return result;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 0х1. Чтение реле
        /// </summary>
        /// <param name="addressSlave">адрес удалённого slave-устройства, 
        /// из которого нужно получить данные</param>
        /// <param name="startingAddress">Адрес начального реле</param>
        /// <param name="quantity">Количество реле для чтения 1...2000</param>
        /// <param name="answer">Ответное сообщение</param>
        /// <param name="coils">Прочитанные состояния реле</param>
        /// <returns>Запрос на чтение реле</returns>
        public Modbus.OSIModel.Message.Result ReadCoils(
            byte addressSlave,
            UInt16 startingAddress, 
            UInt16 quantity, 
            out State[] coils)
        {
            Modbus.OSIModel.Message.Message request;
            
            // Подготавливаем данные
            List<byte> array = new List<byte>();
            array.AddRange(Modbus.Convert.ConvertToBytes(startingAddress));
            array.AddRange(Modbus.Convert.ConvertToBytes(quantity));
            
            request = 
                new Modbus.OSIModel.Message.Message(addressSlave, 0x1, array.ToArray());
            
            // Выполняем запрос
            Message.Result result = ReadCoils(request, out coils);
            
            return result;
        }
        /// <summary>
        /// Функция 0x2. Читает дискретные входы
        /// в удалённом устройстве
        /// </summary>
        /// <param name="AddressSlave">адрес удалённого slave-устройства, 
        /// из которого нужно получить данные</param>
        /// <param name="StartingAddress">Адрес начального дискретного входа</param>
        /// <param name="Quantity">Количество дискретных входов для чтения 1...2000</param>
        /// <param name="inputs">Состояния прочитанных дискретных входов</param>
        /// <returns>Результат выполения операции</returns>
        public Message.Result ReadDiscreteInputs(byte AddressSlave,
            UInt16 StartingAddress, UInt16 Quantity, out State[] inputs)
        {
            Modbus.OSIModel.Message.Message request, answer;
            Modbus.OSIModel.Message.Result result;
            inputs = null;
            String message;

            // Данный запрос не может быть широковещательным
            if (AddressSlave == 0)
            {
                message = "Функция с кодом 0х2, не может быть широковещательным запросом";
                throw new ArgumentException(message, "addressSlave");
            }

            // Подготавливаем данные
            List<byte> array = new List<byte>();
            array.AddRange(Modbus.Convert.ConvertToBytes(StartingAddress));
            array.AddRange(Modbus.Convert.ConvertToBytes(Quantity));

            request =
                new Modbus.OSIModel.Message.Message(AddressSlave, 0x2, array.ToArray());

            // Выполняем запрос
            while (_flgBusy)
            {
                // Если идёт транзакция ждём окончания
            }

            // Устанавливаем начало транзакции
            StartTransaction();

            // Отправляем запрос
            Modbus.OSIModel.DataLinkLayer.RequestError error =
                _dataLinkObject.SendMessage(request, out answer);

            switch (error)
            {
                case Modbus.OSIModel.DataLinkLayer.RequestError.NoError:
                    {
                        // Разбираем сообщение на предмет ошибок
                        if (IsError(request, answer, out result))
                        {
                            // Ошибка была
                            break;
                        }
                        else
                        {
                            // Ошибки нет возвращаем результат выполнения запроса
                            // Проверяем содержимое посылки
                            // Рассчитываем необходимое количество байт для передачи состояний дискретных входов
                            int length = Quantity / 8;

                            if (Quantity % 8 != 0)
                            {
                                ++length;
                            }

                            if ((int)answer.PDUFrame.Data[0] != length)
                            {
                                result = new Message.Result(
                                    Error.DataFormatError,
                                    String.Format("Количество байт {0} в сообщении не равно требуемому {1} для передачи состояний реле в количестве {2}",
                                    answer.PDUFrame.Data[0], 
                                    length, 
                                    Quantity),
                                    request, 
                                    answer);
                            }
                            else
                            {
                                Byte data, temp;
                                inputs = new State[Quantity];

                                for (int i = 0; i < answer.PDUFrame.Data[0]; i++)
                                {
                                    data = answer.PDUFrame.Data[i + 1];

                                    for (int y = 0; y < 8; y++)
                                    {
                                        if (8 * i + y < Quantity)
                                        {
                                            temp = (Byte)(data >> y);

                                            if ((temp & 0x01) == 0x01)
                                            {
                                                inputs[(8 * i + y)] = State.On;
                                            }
                                            else
                                            {
                                                inputs[(8 * i + y)] = State.Off;
                                            }
                                        }
                                    }
                                }
                            }

                            result = new Message.Result(Error.NoError, String.Empty,
                                request, answer);

                            break;
                        }
                    }
                case Modbus.OSIModel.DataLinkLayer.RequestError.TimeOut:
                    {
                        // Таймаут ответа
                        result = new Modbus.OSIModel.Message.Result(Error.TimeOut,
                            "Ответ не был получен в заданное время", request, null);
                        break;
                    }
                default:
                    {
                        // Ошибка уровня Datalink layer.
                        result = new Modbus.OSIModel.Message.Result(Error.ReciveMessageError,
                            error.ToString(), request, null);
                        break;
                    }
            }
            StopTransaction();
            return result;
        }
        /// <summary>
        /// Функция 0х3. Читает holding-регистры
        /// в удалённом устройстве
        /// </summary>
        /// <param name="request">Запрос на чтение регистров</param>
        /// <param name="values">Прочитанные занчения регистров</param>
        /// <returns>Результат выполения операции</returns>
        public Message.Result ReadHoldingRegisters(
            Message.Message request, 
            out UInt16[] values)
        {
            Modbus.OSIModel.Message.Message answer;
            Modbus.OSIModel.Message.Result result;

            Byte[] arr = new byte[2];
            Array.Copy(request.PDUFrame.Data, 2, arr, 0, 2);
            UInt16 quantity = Modbus.Convert.ConvertToUInt16(arr);


            // Проверяем состояние сервера
            if (_flgBusy == true)
            {
                throw new Exception(String.Format(
                    "Попытка выполнить запрос (код функции 0x3) к серверу {0}, во время выполнения предыдущего",
                    Name));
            }
            else
            {
                // Устанавливаем начало транзакции
                StartTransaction();

                // Отправляем запрос
                Modbus.OSIModel.DataLinkLayer.RequestError error =
                    _dataLinkObject.SendMessage(request, out answer);

                switch (error)
                {
                    case Modbus.OSIModel.DataLinkLayer.RequestError.NoError:
                        {
                            // Разбираем сообщение на предмет ошибок
                            if (IsError(request, answer, out result))
                            {
                                // Ошибка была
                                values = null;
                                break;
                            }
                            else
                            {
                                // Ошибки нет возвращаем результат выполнения запроса
                                values = new ushort[quantity];

                                // Проверяем количество байт принятых в ответном сообщении
                                // Их должно быть в два раза больше чем количество регистров
                                // для чтения
                                if (quantity == (answer.PDUFrame.Data[0] / 2))
                                {
                                    // Количество байт в порядке. Получаем прочитанные значения
                                    // holding-регистров
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        values[i] = answer.PDUFrame.Data[i * 2 + 1];
                                        // старший байт регистра
                                        values[i] = (UInt16)(values[i] << 8);
                                        // младший байт регистра
                                        values[i] = (UInt16)(values[i] | answer.PDUFrame.Data[i * 2 + 2]);
                                    }
                                    result = new Modbus.OSIModel.Message.Result(Error.NoError,
                                        String.Empty, request, answer);
                                }
                                else
                                {
                                    values = null;
                                    result = new Modbus.OSIModel.Message.Result(Error.DataFormatError,
                                        "Поле данных ответа содержит некорректные данные",
                                        request, answer);
                                }

                                break;
                            }
                        }
                    case Modbus.OSIModel.DataLinkLayer.RequestError.TimeOut:
                        {
                            // Таймаут ответа
                            values = null;
                            result = new Modbus.OSIModel.Message.Result(Error.TimeOut,
                                "Ответ не был получен в заданное время", request, null);
                            break;
                        }
                    default:
                        {
                            // Ошибка уровня Datalink layer.
                            values = null;
                            result = new Modbus.OSIModel.Message.Result(Error.ReciveMessageError,
                                error.ToString(), request, null);
                            break;
                        }
                }
                StopTransaction();
                return result;
            }
        }
        /// <summary>
        /// Функция 0х3. Читает holding-регистры
        /// в удалённом устройстве
        /// </summary>
        /// <param name="AddressSlave">Адрес подчинённого устройства</param>
        /// <param name="StartingAddress">Адрес первого регистра в блоке</param>
        /// <param name="Quantity">Длина блока регистров хранения для чтения</param>
        /// <param name="values">Значения прочитанных регистров</param>
        /// <returns>Результат выполения операции</returns>
        public Message.Result ReadHoldingRegisters(
            byte AddressSlave, 
            ushort StartingAddress, 
            ushort Quantity, 
            out ushort[] values)
        {            
            Message.PDU frame = new Message.PDU();
            frame.Function = 0x3;
            frame.AddDataBytesRange(Convert.ConvertToBytes(StartingAddress));
            frame.AddDataBytesRange(Convert.ConvertToBytes(Quantity));

            Message.Message request = new Message.Message(AddressSlave, frame);
            return ReadHoldingRegisters(request, out values);
        }
        /// <summary>
        /// Функция 0х3. Формирует сообщение для организации запроса чтения
        /// holding-регистров в физическом устройстве
        /// </summary>
        /// <param name="addressSlave">адрес удалённого slave-устройства, 
        /// из которого нужно получить данные</param>
        /// <param name="startingAddress">Адрес начального holding-регистра</param>
        /// <param name="quantity">Количество holding-регистров для чтения 1...125</param>
        /// <returns>Результат выполения операции</returns>
        public static Message.Message ReadHoldingRegisters(byte addressSlave,
            UInt16 startingAddress, UInt16 quantity)
        {
            string message;
            Modbus.OSIModel.Message.Message request;
            
            // Данный запрос не может быть широковещательным
            if (addressSlave == 0)
            {
                message = "Функция с кодом 0х3, не может быть широковещательным запросом";
                throw new ArgumentException(message, "addressSlave");
            }
            else
            {
                if ((quantity > 0) && (quantity <= 125))
                {
                    // Подготавливаем данные
                    List<byte> array = new List<byte>();
                    array.AddRange(Modbus.Convert.ConvertToBytes(startingAddress));
                    array.AddRange(Modbus.Convert.ConvertToBytes(quantity));

                    // Формируем запрос
                    request = new Modbus.OSIModel.Message.Message(addressSlave, 0x3, array.ToArray());
                }
                else
                {
                    message = String.Format("Количество регистров для чтения {0}, должно быть от 1...125", 
                        quantity);
                    throw new ArgumentOutOfRangeException("quantity", message);
                }
            }
            return request;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 0х4. Читает входные регистры
        /// </summary>
        /// <param name="AddressSlave">адрес удалённого slave-устройства, 
        /// из которого нужно получить данные</param>
        /// <param name="StartingAddress">Адрес начального входного регистра</param>
        /// <param name="Quantity">Количество регистров для чтения 1...125</param>
        /// <param name="values">Прочатанные значения регистров</param>
        /// <returns>Результат выполнения операции</returns>
        public Message.Result ReadInputRegisters(byte AddressSlave,
            UInt16 StartingAddress, UInt16 Quantity, out UInt16[] values)
        {
            string message;
            Modbus.OSIModel.Message.Message request;
            Modbus.OSIModel.Message.Message answer;
            Modbus.OSIModel.Message.Result result;

            // Данный запрос не может быть широковещательным
            if (AddressSlave == 0)
            {
                message = "Функция с кодом 0х4, не может быть широковещательным запросом";
                throw new ArgumentException(message, "AddressSlave");
            }
            else
            {
                if ((Quantity > 0) && (Quantity <= 125))
                {
                    // Подготавливаем данные
                    List<byte> array = new List<byte>();
                    array.AddRange(Modbus.Convert.ConvertToBytes(StartingAddress));
                    array.AddRange(Modbus.Convert.ConvertToBytes(Quantity));

                    // Формируем запрос
                    request = new Modbus.OSIModel.Message.Message(AddressSlave, 0x4, array.ToArray());
                }
                else
                {
                    message = String.Format("Количество регистров для чтения {0}, должно быть от 1...125",
                        Quantity);
                    throw new ArgumentOutOfRangeException("quantity", message);
                }
            }

            // Проверяем состояние сервера
            if (_flgBusy == true)
            {
                throw new Exception(String.Format(
                    "Попытка выполнить запрос (код функции 0x4) к серверу {0}, во время выполнения предыдущего",
                    Name));
            }
            else
            {
                // Устанавливаем начало транзакции
                StartTransaction();

                // Отправляем запрос
                Modbus.OSIModel.DataLinkLayer.RequestError error =
                    _dataLinkObject.SendMessage(request, out answer);

                switch (error)
                {
                    case Modbus.OSIModel.DataLinkLayer.RequestError.NoError:
                        {
                            // Разбираем сообщение на предмет ошибок
                            if (IsError(request, answer, out result))
                            {
                                // Ошибка была
                                values = null;
                                break;
                            }
                            else
                            {
                                // Ошибки нет возвращаем результат выполнения запроса
                                values = new ushort[Quantity];

                                // Проверяем количество байт принятых в ответном сообщении
                                // Их должно быть в два раза больше чем количество регистров
                                // для чтения
                                if (Quantity == (answer.PDUFrame.Data[0] / 2))
                                {
                                    // Количество байт в порядке. Получаем прочитанные значения
                                    // входных регистров
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        values[i] = answer.PDUFrame.Data[i * 2 + 1];
                                        // старший байт регистра
                                        values[i] = (UInt16)(values[i] << 8);
                                        // младший байт регистра
                                        values[i] = (UInt16)(values[i] | answer.PDUFrame.Data[i * 2 + 2]);
                                    }
                                    result = new Modbus.OSIModel.Message.Result(Error.NoError,
                                        String.Empty, request, answer);
                                }
                                else
                                {
                                    values = null;
                                    result = new Modbus.OSIModel.Message.Result(Error.DataFormatError,
                                        "Поле данных ответа содержит некорректные данные",
                                        request, answer);
                                }

                                break;
                            }
                        }
                    case Modbus.OSIModel.DataLinkLayer.RequestError.TimeOut:
                        {
                            // Таймаут ответа
                            values = null;
                            result = new Modbus.OSIModel.Message.Result(Error.TimeOut,
                                "Ответ не был получен в заданное время", request, null);
                            break;
                        }
                    default:
                        {
                            // Ошибка уровня Datalink layer.
                            values = null;
                            result = new Modbus.OSIModel.Message.Result(Error.ReciveMessageError,
                                error.ToString(), request, null);
                            break;
                        }
                }
                StopTransaction();
                return result;
            }
        }
        /// <summary>
        /// Функция 0х5. Устанавливает реле в состояние вкл./выкл. 
        /// в удалённом устройстве
        /// </summary>
        /// <param name="addressSlave">адрес удалённого slave-устройства, 
        /// в котором нужно установить состояние реле</param>
        /// <param name="addressCoil">Адрес целевого реле</param>
        /// <param name="state">Состояние реле: 0х00FF-ON, 0x0000-OFF</param>
        public Message.Result WriteSingleCoil(byte addressSlave, 
            UInt16 addressCoil, ref State state)
        {
            string message;
            Modbus.OSIModel.Message.Message request, answer;
            Modbus.OSIModel.Message.Result result;

            // Проверяем состояние сервера
            if (_flgBusy == true)
            {
                return result = new Message.Result(Error.IllegalReguest,
                    String.Format("Попытка выполнить запрос (код функции 0x5) к серверу {0}, во время выполнения предыдущего",
                    Name), null, null);
                //throw new Exception(String.Format(
                //    "Попытка выполнить запрос (код функции 0x5) к серверу {0}, во время выполнения предыдущего",
                //    Name));
            }
            else
            {
                // Данный запрос не может быть широковещательным
                if (addressSlave == 0)
                {
                    message = "Функция с кодом 0х5, не может быть отправлена как широковещательный запрос";
                    throw new Exception(message);
                }
                else
                {

                    // Устанавливаем начало транзакции
                    StartTransaction();

                    // Подготавливаем данные
                    List<byte> array = new List<byte>();
                    array.AddRange(Modbus.Convert.ConvertToBytes(addressCoil));
                    //Byte[] state;
                    
                    //if (ValueCoil == Modbus.Device.CoilState.ON)
                    //{
                    //    state = new byte[2] { 0xFF, 0x00 };
                    //    array.AddRange(state);
                    //}
                    //else
                    //{
                    //    state = new byte[2] { 0x00, 0x00 };
                    //    array.AddRange(state);
                    //}
                    array.AddRange(Modbus.Convert.StateToArray(state));

                    // Отправляем запрос
                    request = new Modbus.OSIModel.Message.Message(addressSlave, 0x5, array.ToArray());

                    // Отправляем запрос
                    Modbus.OSIModel.DataLinkLayer.RequestError error =
                        _dataLinkObject.SendMessage(request, out answer);

                    switch (error)
                    {
                        case Modbus.OSIModel.DataLinkLayer.RequestError.NoError:
                            {
                                // Разбираем сообщение на предмет ошибок
                                if (IsError(request, answer, out result))
                                {
                                    // Ошибка была
                                    break;
                                }
                                else
                                {
                                    // Ошибки нет возвращаем результат выполнения запроса
                                    // Проверяем длину данных
                                    if (answer.PDUFrame.Data.Length == 4)
                                    {
                                        // Длина корректана проверяем
                                        // адрес дискретного входа
                                        List<Byte> arr = new List<byte>();
                                        arr.Add(answer.PDUFrame.Data[0]);
                                        arr.Add(answer.PDUFrame.Data[1]);
                                        UInt16 value = Convert.ConvertToUInt16(arr.ToArray());
                                        if (value == addressCoil)
                                        {
                                            // Адрес верный
                                            // получаем новое состояние реле
                                            arr.Clear();
                                            arr.Add(answer.PDUFrame.Data[2]);
                                            arr.Add(answer.PDUFrame.Data[3]);
                                            state = Convert.ValueToState(arr.ToArray());
                                            result = new Modbus.OSIModel.Message.Result(Error.NoError,
                                                String.Empty, request, answer);
                                        }
                                        else
                                        {
                                            // Адрес дискретного входа отличается от адреса в запросе
                                            result = new Modbus.OSIModel.Message.Result(Error.DataFormatError,
                                                String.Format(
                                                "Адрес реле в запросе {0} не соответствует адерсу реле в ответе {1}", 
                                                addressCoil, value), 
                                                request, answer);
                                        }
                                    }
                                    else
                                    {
                                        // Некорректная длина данных в ответе
                                        result = new Modbus.OSIModel.Message.Result(Error.DataFormatError,
                                            String.Format(
                                            "Неверная длина данных {0} в ответе, а ожидается 4",
                                            answer.PDUFrame.Data.Length),
                                            request, answer);
                                    }
                                    break;
                                }
                            }
                        case Modbus.OSIModel.DataLinkLayer.RequestError.TimeOut:
                            {
                                // Таймаут ответа
                                result = new Modbus.OSIModel.Message.Result(Error.TimeOut,
                                    "Ответ не был получен в заданное время", request, null);
                                break;
                            }
                        default:
                            {
                                // Ошибка уровня Datalink layer.
                                result = new Modbus.OSIModel.Message.Result(Error.ReciveMessageError,
                                    error.ToString(), request, null);
                                break;
                            }
                    }
                    StopTransaction();
                    return result;
                }
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 0x6. Записывает значение в одиночный регистр
        /// хранения в удалённом устройстве
        /// </summary>
        /// <param name="request">Запрос на выполнение операции</param>
        /// <param name="value">Записанное в устройство значение регистра</param>
        /// <returns>Результат выполнения запроса</returns>
        public Modbus.OSIModel.Message.Result WriteSingleRegister(Message.Message request,
            ref UInt16 value)
        {
            Modbus.OSIModel.Message.Message answer;
            Modbus.OSIModel.Message.Result result;

            // Проверяем состояние сервера
            if (_flgBusy == true)
            {
                value = 0;
                return result = new Message.Result(Error.IllegalReguest,
                    String.Format("Попытка выполнить запрос (код функции 0x6) к серверу {0}, во время выполнения предыдущего",
                    Name), null, null);
                //throw new Exception(String.Format(
                //    "Попытка выполнить запрос (код функции 0x6) к серверу {0}, во время выполнения предыдущего",
                //    Name));
            }
            else
            {
                // Устанавливаем начало транзакции
                StartTransaction();

                // Отправляем запрос
                Modbus.OSIModel.DataLinkLayer.RequestError error =
                    _dataLinkObject.SendMessage(request, out answer);

                switch (error)
                {
                    case Modbus.OSIModel.DataLinkLayer.RequestError.NoError:
                        {
                            // Разбираем сообщение на предмет ошибок
                            if (IsError(request, answer, out result))
                            {
                                // Ошибка была
                                value = 0;
                                break;
                            }
                            else
                            {
                                // Ошибки нет возвращаем результат выполнения запроса
                                // Проверяем длину сообщения
                                if (answer.PDUFrame.Data.Length != 4)
                                {
                                    value = 0;
                                    result = new Message.Result(Error.DataFormatError,
                                        String.Format(
                                        "Длина ответного сообщения {0} не соответствет ожидаемой 4",
                                        answer.PDUFrame.Data.Length), request, answer);
                                }
                                else
                                {
                                    // Проверяем адрес регистра в ответном сообщениии
                                    Byte[] arr = new Byte[2];
                                    Array.Copy(answer.PDUFrame.Data, 0, arr, 0, 2);
                                    value = Modbus.Convert.ConvertToUInt16(arr);
                                    
                                    Array.Copy(request.PDUFrame.Data, 0, arr, 0, 2);
                                    UInt16 var = Modbus.Convert.ConvertToUInt16(arr);
                                    
                                    if (value != var)
                                    {
                                        result = new Message.Result(Error.DataFormatError,
                                            String.Format(
                                            "Адрес регистра {0} в запросе не совпадает с аресом в ответном сообщении {1}",
                                            var, value), request, answer);
                                    }
                                    else
                                    {
                                        Array.Copy(answer.PDUFrame.Data, 2, arr, 0, 2);
                                        value = Modbus.Convert.ConvertToUInt16(arr);
                                        result = new Modbus.OSIModel.Message.Result(Error.NoError,
                                            String.Empty, request, answer);
                                    }
                                }
                                break;
                            }
                        }
                    case Modbus.OSIModel.DataLinkLayer.RequestError.TimeOut:
                        {
                            // Таймаут ответа
                            value = 0;
                            result = new Modbus.OSIModel.Message.Result(Error.TimeOut,
                                "Ответ не был получен в заданное время", request, null);
                            break;
                        }
                    default:
                        {
                            // Ошибка уровня Datalink layer.
                            value = 0;
                            result = new Modbus.OSIModel.Message.Result(Error.ReciveMessageError,
                                error.ToString(), request, null);
                            break;
                        }
                }
                StopTransaction();
                return result;
            }
        }
        /// <summary>
        /// Функция 0x6. Формирует запрос на запись регистра хранения
        /// в физическом устройстве
        /// </summary>
        /// <param name="addressSlave">адрес удалённого slave-устройства, 
        /// в котором нужно записать регистр хранения</param>
        /// <param name="addressRegister">адерс регистра хранения</param>
        /// <param name="value">значение регистра</param>
        /// <returns>Modus-запрос</returns>        
        public Message.Result WriteSingleRegister(
            byte addressSlave, 
            ushort addressRegister, 
            ref ushort value)
        {
            Message.Message msg = Device.WriteSingleRegister(addressSlave, addressRegister, value);
            return WriteSingleRegister(msg, ref value); 
        }
        /// <summary>
        /// Функция 0x6. Формирует запрос на запись регистра хранения
        /// в физическом устройстве
        /// </summary>
        /// <param name="addressSlave">адрес удалённого slave-устройства, 
        /// в котором нужно записать регистр хранения</param>
        /// <param name="addressRegister">адерс регистра хранения</param>
        /// <param name="value">значение регистра</param>
        /// <returns>Modus-запрос</returns>
        public static Message.Message WriteSingleRegister(byte addressSlave,
            UInt16 addressRegister, UInt16 value)
        {
            Modbus.OSIModel.Message.Message request;

            // Подготавливаем данные
            List<byte> array = new List<byte>();
            array.AddRange(Modbus.Convert.ConvertToBytes(addressRegister));
            array.AddRange(Modbus.Convert.ConvertToBytes(value));

            // Возвращаем запрос
            request = new Modbus.OSIModel.Message.Message(addressSlave, 0x6, array.ToArray());
            return request;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 0x7. Только для последовательной линии.
        /// Смысл функции не доконца понял.
        /// </summary>
        /// <param name="AddressSlave">адрес удалённого slave-устройства</param>
        public void ReadExceptionStatus(byte AddressSlave)
        {
            throw new NotImplementedException();
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 0х7. Только для последовательной линии.
        /// Диагностическая функция
        /// </summary>
        /// <param name="AddressSlave">Адрес удалённого slave-устройства</param>
        /// <param name="SubFunction">Код подфункции</param>
        /// <param name="Data">данные</param>
        //public void Diagnostics(byte AddressSlave,
        //    SubFunction SubFunction, UInt16[] Data)
        //{
        //    throw new NotImplementedException();
        //}
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 11. Только для последовательной линии.
        /// Не разбирался с этой функцией
        /// </summary>
        /// <param name="AddressSlave">Адрес удалённого slave-устройства</param>
        public void GetCommEventCounter(byte AddressSlave)
        {
            throw new NotImplementedException();
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 12 (0х0С). Только для последовательной линии.
        /// Не разбирался с этой функцией
        /// </summary>
        /// <param name="AddressSlave">Адрес удалённого slave-устройства</param>
        public void GetComEventLog(byte AddressSlave)
        {
            throw new NotImplementedException();
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 15 (0x0F). Устанавливет состояния массива реле 
        /// </summary>
        /// <param name="AddressSlave">Адрес удалённого slave-устройства,
        /// в котором необходимо установить состояния последовательности реле</param>
        /// <param name="StartingAddress">Адрес начального реле</param>
        /// <param name="coils">Количество и состояния реле для записи</param>
        public Modbus.OSIModel.Message.Result WriteMultipleCoils(byte AddressSlave,
            UInt16 StartingAddress, State[] coils)
        {
            String message;
            Message.Message request;
            Modbus.OSIModel.Message.Message answer;
            Modbus.OSIModel.Message.Result result;

            if ((coils.Length == 0) && (coils.Length > 0x7B1))
            {
                message = String.Format(
                    "Длина блока регистров {0} выходит за пределы диапазона 1...0x7B0",
                    coils.Length);
                throw new ArgumentOutOfRangeException("Quantity", message);
            }

            // Проверяем выход блока данных за граници диапазона допустимых адресов
            if ((StartingAddress + coils.Length) > 0xFFFF)
            {
                message = String.Format(
                    "Длина блока регистров {0} больше допустимой величины 0xFFFF",
                    (StartingAddress + coils.Length));
                throw new ArgumentException(message, "Quantity");
            }

            // Формируем запрос
            List<Byte> data = new List<byte>();
            data.AddRange(Convert.ConvertToBytes(StartingAddress)); // Адрес первого регистра в блоке
            data.AddRange(Convert.ConvertToBytes((UInt16)coils.Length)); // Длина блока регистров

            int quantity = coils.Length / 8;

            if (quantity == 0)
            {
                ++quantity;
            }
            else
            {
                if (coils.Length % 8 != 0)
                {
                    ++quantity;
                }
            }

            data.Add((Byte)quantity); // Количество байт 
            // Значения регистров
            Byte dataByte;
          
            for (int i = 0; i < quantity; i++)
            {
                dataByte = 0;

                for (int y = 0; y < 8; y++)
                {
                    if (coils.Length > (i * 8 + y))
                    {
                        if (coils[i * 8 + y] == State.On)
                        {
                            dataByte |= (Byte)(1 << y);
                        }
                    }
                }
                // Полученный байт сохраняем в поле данных
                data.Add(dataByte);
            }

            Message.PDU pdu = new Message.PDU(0x0F, data.ToArray());
            request = new Message.Message(AddressSlave, pdu);

            // Проверяем состояние сервера
            if (_flgBusy == true)
            {
                return result = new Message.Result(Error.IllegalReguest,
                    String.Format("Попытка выполнить запрос (код функции 0x0F) к серверу {0}, во время выполнения предыдущего",
                    Name), null, null);
            }
            else
            {
                // Устанавливаем начало транзакции
                StartTransaction();

                // Отправляем запрос
                Modbus.OSIModel.DataLinkLayer.RequestError error =
                    _dataLinkObject.SendMessage(request, out answer);

                switch (error)
                {
                    case Modbus.OSIModel.DataLinkLayer.RequestError.NoError:
                        {
                            // Разбираем сообщение на предмет ошибок
                            if (IsError(request, answer, out result))
                            {
                                // Ошибка была
                                break;
                            }
                            else
                            {
                                // Ошибки нет возвращаем результат выполнения запроса
                                // Проверяем длину сообщения
                                if (answer.PDUFrame.Data.Length != 4)
                                {
                                    result = new Message.Result(Error.DataFormatError,
                                        String.Format(
                                        "Длина ответного сообщения {0} не соответствет ожидаемой 4",
                                        answer.PDUFrame.Data.Length), request, answer);
                                }
                                else
                                {
                                    // Проверяем адрес начального реле в ответном сообщениии
                                    Byte[] arr = new Byte[2];
                                    Array.Copy(answer.PDUFrame.Data, 0, arr, 0, 2);
                                    UInt16 var = Modbus.Convert.ConvertToUInt16(arr);

                                    if (var != StartingAddress)
                                    {
                                        result = new Message.Result(Error.DataFormatError,
                                            String.Format(
                                            "Адрес начального реле {0} в запросе не совпадает с адресом в ответном сообщении {1}",
                                            StartingAddress, var), request, answer);
                                    }
                                    else
                                    {
                                        // Проверяем длину блока реле
                                        Array.Copy(answer.PDUFrame.Data, 2, arr, 0, 2);
                                        var = Modbus.Convert.ConvertToUInt16(arr);

                                        if (var != coils.Length)
                                        {
                                            result = new Message.Result(Error.DataFormatError,
                                               String.Format(
                                               "Длина блока реле {0} в запросе не совпадает с длиной блока в ответном сообщении {1}",
                                               coils.Length, var), request, answer);
                                        }
                                        else
                                        {
                                            // Ответ корректен
                                            result = new Modbus.OSIModel.Message.Result(Error.NoError,
                                                String.Empty, request, answer);
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    case Modbus.OSIModel.DataLinkLayer.RequestError.TimeOut:
                        {
                            // Таймаут ответа
                            result = new Modbus.OSIModel.Message.Result(Error.TimeOut,
                                "Ответ не был получен в заданное время", request, null);
                            break;
                        }
                    default:
                        {
                            // Ошибка уровня Datalink layer.
                            result = new Modbus.OSIModel.Message.Result(Error.ReciveMessageError,
                                error.ToString(), request, null);
                            break;
                        }
                }
                StopTransaction();
                return result;
            }   
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 16 (0х10). Записывает значения массива регистров
        /// </summary>
        /// <param name="AddressSlave">Адрес удалённого slave-устройства,
        /// в котором необходимо установить значения последовательности 
        /// регистров</param>
        /// <param name="StartingAddress">Адрес первого регистра</param>
        /// <param name="value">Количество, значения регистров 1...123</param>
        /// <returns>Результат выполнения операции</returns>
        public Modbus.OSIModel.Message.Result WriteMultipleRegisters(byte AddressSlave,
            UInt16 StartingAddress, UInt16[] value)
        {
            String message;
            Message.Message request;
            Modbus.OSIModel.Message.Message answer;
            Modbus.OSIModel.Message.Result result;

            if ((value.Length == 0) && (value.Length > 124))
            {
                message = String.Format(
                    "Длина блока регистров {0} выходит за пределы диапазона 1...124", 
                    value.Length);
                throw new ArgumentOutOfRangeException("Quantity", message);
            }
            
            // Проверяем выход блока данных за граници диапазона допустимых адресов
            if ((StartingAddress + value.Length) > 0xFFFF)
            {
                message = String.Format(
                    "Длина блока регистров {0} больше допустимой величины 0xFFFF",
                    (StartingAddress + value.Length));
                throw new ArgumentException(message, "Quantity");
            }

            // Формируем запрос
            List<Byte> data = new List<byte>();
            data.AddRange(Convert.ConvertToBytes(StartingAddress)); // Адрес первого регистра в блоке
            data.AddRange(Convert.ConvertToBytes((UInt16)value.Length)); // Длина блока регистров
            data.Add((Byte)(value.Length * 2)); // Количество байт 
            // Значения регистров
            for (int i = 0; i < value.Length; i++)
            {
                data.AddRange(Convert.ConvertToBytes(value[i]));
            }

            Message.PDU pdu = new Message.PDU(0x10, data.ToArray());
            request = new Message.Message(AddressSlave, pdu);

            // Проверяем состояние сервера
            if (_flgBusy == true)
            {
                return result = new Message.Result(Error.IllegalReguest,
                    String.Format("Попытка выполнить запрос (код функции 0x10) к серверу {0}, во время выполнения предыдущего",
                    Name), null, null);
            }
            else
            {
                // Устанавливаем начало транзакции
                StartTransaction();

                // Отправляем запрос
                Modbus.OSIModel.DataLinkLayer.RequestError error =
                    _dataLinkObject.SendMessage(request, out answer);

                switch (error)
                {
                    case Modbus.OSIModel.DataLinkLayer.RequestError.NoError:
                        {
                            // Разбираем сообщение на предмет ошибок
                            if (IsError(request, answer, out result))
                            {
                                // Ошибка была
                                break;
                            }
                            else
                            {
                                // Ошибки нет возвращаем результат выполнения запроса
                                // Проверяем длину сообщения
                                if (answer.PDUFrame.Data.Length != 4)
                                {
                                    result = new Message.Result(Error.DataFormatError,
                                        String.Format(
                                        "Длина ответного сообщения {0} не соответствет ожидаемой 4",
                                        answer.PDUFrame.Data.Length), request, answer);
                                }
                                else
                                {
                                    // Проверяем адрес начального регистра в ответном сообщениии
                                    Byte[] arr = new Byte[2];
                                    Array.Copy(answer.PDUFrame.Data, 0, arr, 0, 2);
                                    UInt16 var = Modbus.Convert.ConvertToUInt16(arr);
                                    
                                    if (var != StartingAddress)
                                    {
                                        result = new Message.Result(Error.DataFormatError,
                                            String.Format(
                                            "Адрес начального регистра {0} в запросе не совпадает с адресом в ответном сообщении {1}",
                                            StartingAddress, var), request, answer);
                                    }
                                    else
                                    {
                                        // Проверяем длину блока регистров
                                        Array.Copy(answer.PDUFrame.Data, 2, arr, 0, 2);
                                        var = Modbus.Convert.ConvertToUInt16(arr);

                                        if (var != value.Length)
                                        {
                                            result = new Message.Result(Error.DataFormatError,
                                               String.Format(
                                               "Длина блока регистров {0} в запросе не совпадает с длиной блока в ответном сообщении {1}",
                                               value.Length, var), request, answer);
                                        }
                                        else
                                        {
                                            // Ответ корректен
                                            result = new Modbus.OSIModel.Message.Result(Error.NoError,
                                                String.Empty, request, answer);
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    case Modbus.OSIModel.DataLinkLayer.RequestError.TimeOut:
                        {
                            // Таймаут ответа
                            result = new Modbus.OSIModel.Message.Result(Error.TimeOut,
                                "Ответ не был получен в заданное время", request, null);
                            break;
                        }
                    default:
                        {
                            // Ошибка уровня Datalink layer.
                            result = new Modbus.OSIModel.Message.Result(Error.ReciveMessageError,
                                error.ToString(), request, null);
                            break;
                        }
                }
                StopTransaction();
                return result;
            }           
        }
        /// <summary>
        /// Функция 16 (0х10). Записывает значения массива регистров
        /// </summary>
        /// <param name="request">Сообщение с запросом 
        /// на запись массива регистров</param>
        /// <returns>Результат выполнения операции</returns>
        public Modbus.OSIModel.Message.Result WriteMultipleRegisters(
            Message.Message request)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Функция 17 (0х11). Только для последовательной линии.
        /// Не разбирался.
        /// </summary>
        /// <param name="AddressSlave">Адрес удалённого slave-устройства</param>
        public void ReportSlaveID(byte AddressSlave)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Функция 20 (0х14). С функцией не разбирался
        /// </summary>
        public void ReadFileRecord()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Функция 21 (0х15). С функцией не разбирался
        /// </summary>
        public void WriteFileRecord()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Функция 22 (0х16). С функцией не разбирался
        /// </summary>
        /// <param name="AddressSlave"></param>
        /// <param name="ReferenceAddress"></param>
        /// <param name="And_Mask"></param>
        /// <param name="Or_Mask"></param>
        public void MaskWriteRegister(byte AddressSlave,
            UInt16 ReferenceAddress, UInt16 And_Mask,
            UInt16 Or_Mask)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Функция 23 (0х17). С функцией не разбирался
        /// </summary>
        /// <param name="AddressSlave"></param>
        /// <param name="ReadStartingAddress"></param>
        /// <param name="QuantityToRead"></param>
        /// <param name="WriteStartingAddress"></param>
        /// <param name="QuantityToWrite"></param>
        /// <param name="WriteByteCount"></param>
        /// <param name="WriteRegistersValue"></param>
        public void ReadWriteMultipleRegisters(byte AddressSlave,
            UInt16 ReadStartingAddress, UInt16 QuantityToRead,
            UInt16 WriteStartingAddress, UInt16 QuantityToWrite,
            byte WriteByteCount, UInt16 WriteRegistersValue)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Функция 24 (0х18). С Функцией не разбирался
        /// </summary>
        /// <param name="AddressSlave"></param>
        /// <param name="FIFOPointerAddress"></param>
        public void ReadFIFOQueue(byte AddressSlave,
            UInt16 FIFOPointerAddress)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Метод отсылает запрос с определённым пользователем
        /// кодом и данными.
        /// </summary>
        /// <param name="address">Адрес slave-устройства</param>
        /// <param name="code">Код функции</param>
        /// <param name="data">Данные сообщения</param>
        /// <param name="Answer">Ответное сообщение</param>
        /// <returns>Если запрос прошёл успешно возвращается true</returns>
        public Message.Result SendUserDefinedRequest(Byte address,
            Byte code, Byte[] data)
        {
            Modbus.OSIModel.Message.Message request, answer;
            Modbus.OSIModel.Message.Result result;

            // Проверяем состояние сервера
            if (_flgBusy == true)
            {
                throw new Exception(String.Format(
                    "Попытка выполнить запрос User defined к серверу {0}, во время выполнения предыдущего",
                    Name));
            }
            else
            {
                StartTransaction();

                request = new Modbus.OSIModel.Message.Message(address, code, data);
 
                // Отправляем запрос
                Modbus.OSIModel.DataLinkLayer.RequestError error =
                    _dataLinkObject.SendMessage(request, out answer);

                switch (error)
                {
                    case Modbus.OSIModel.DataLinkLayer.RequestError.NoError:
                        {
                            // Разбираем сообщение на предмет ошибок
                            if (IsError(request, answer, out result))
                            {
                                // Ошибка была
                                break;
                            }
                            else
                            {
                                // Ошибки нет возвращаем результат выполнения запроса
                                result = new Modbus.OSIModel.Message.Result(Error.NoError,
                                    String.Empty, request, answer);
                                break;
                            }
                        }
                    case Modbus.OSIModel.DataLinkLayer.RequestError.TimeOut:
                        {
                            // Таймаут ответа
                            result = new Modbus.OSIModel.Message.Result(Error.TimeOut,
                                "Ответ не был получен в заданное время", request, null);
                            break;
                        }
                    default:
                        {
                            // Ошибка уровня Datalink layer.
                            result = new Modbus.OSIModel.Message.Result(Error.ReciveMessageError,
                                error.ToString(), request, null);
                            break;
                        }
                }
                StopTransaction();
                return result;
            }
        }
        /// <summary>
        /// Возвращает интерфейс объекта уровня Datalink layer
        /// </summary>
        public Modbus.OSIModel.DataLinkLayer.Master.IDataLinkLayer DataLinkObject
        {
            get { return _dataLinkObject; }
        }
        /// <summary>
        /// Возвращает интерфейс объекта уровня Datalink layer
        /// </summary>
        /// <returns></returns>
        public Modbus.OSIModel.DataLinkLayer.Master.IDataLinkLayer GetDataLinkObject()
        {
            return _dataLinkObject;
        }
        public Boolean IsBusy()
        {
            return _flgBusy;
        }
        #endregion
    }
}
