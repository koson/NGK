using System;

//=================================================================================
namespace Modbus.OSIModel.ApplicationLayer.Master
{
    //=============================================================================
    /// <summary>
    /// Интерфейс реализует команды для работы с сетью Modbus  
    /// </summary>
    public interface IApi
    {
        //-------------------------------------------------------------------------
        /// <summary>
        /// Возаращает состояние master-устройства;
        /// </summary>
        /// <returns>true - идёт транзакция "запрос-ответ"</returns>
        Boolean IsBusy();
        //-------------------------------------------------------------------------
        /// <summary>
        /// Функция 0x1. Чтение реле (Выполнение запроса) 
        /// </summary>
        /// <param name="request">Сообщение для отправки запроса</param>
        /// <param name="coils">Прочитанные состояния реле</param>
        /// <returns>Результат выполнения операции</returns>
        Modbus.OSIModel.Message.Result ReadCoils(Message.Message request, 
            out State[] coils);
        //-------------------------------------------------------------------------
       /// <summary>
        /// Функция 0х1. Чтение реле
        /// </summary>
        /// <param name="addressSlave">адрес удалённого slave-устройства, 
        /// из которого нужно получить данные</param>
        /// <param name="startingAddress">Адрес начального реле</param>
        /// <param name="quantity">Количество реле для чтения 1...2000</param>
        /// <param name="coils">Прочитанные состояния реле</param>
        /// <returns>Результат выполнения операции</returns>
        Modbus.OSIModel.Message.Result ReadCoils(byte addressSlave,
            UInt16 startingAddress, UInt16 quantity, out State[] coils);
        //-------------------------------------------------------------------------
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
        Message.Result ReadDiscreteInputs(byte AddressSlave,
            UInt16 StartingAddress, UInt16 Quantity, out State[] inputs);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 0х3. Читает holding-регистры
        /// в удалённом устройстве
        /// </summary>
        /// <param name="AddressSlave">Адрес подчинённого устройства</param>
        /// <param name=param name="StartingAddress">Адрес начального регистра хранения</param>
        /// <param name="Quantity">Длина блока регистров хранения для чтения</param>
        /// <param name="values">Блок прочитанных значений</param>
        /// <returns>Результат выполнения запроса</returns>
        Message.Result ReadHoldingRegisters(byte AddressSlave,
            UInt16 StartingAddress, UInt16 Quantity, out UInt16[] values);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 0х3. Читает holding-регистры
        /// в удалённом устройстве
        /// </summary>
        /// <param name="request">Запрос на чтение регистров</param>
        /// <param name="values">Значения прочитанных регистров</param>
        /// <returns>Результат выполнения запроса</returns>
        Message.Result ReadHoldingRegisters(Message.Message request
            , out UInt16[] values);
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
        Message.Result ReadInputRegisters(byte AddressSlave,
            UInt16 StartingAddress, UInt16 Quantity, out UInt16[] values);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 0х5. Устанавливает реле в состояние вкл./выкл. 
        /// в удалённом устройстве
        /// </summary>
        /// <param name="addressSlave">адрес удалённого slave-устройства, 
        /// в котором нужно установить состояние реле</param>
        /// <param name="addressCoil">Адрес целевого реле</param>
        /// <param name="state">Состояние реле: 0х00FF-ON, 0x0000-OFF</param>
        Message.Result WriteSingleCoil(byte addressSlave, 
            UInt16 addressCoil, ref State state);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 0x6. Записывает значение в одиночный регистр
        /// хранения в удалённом устройстве
        /// </summary>
        /// <param name="request">Запрос на запись регистра</param>
        /// <param name="value">значение регистра до и после выполнения операции</param>
        /// <returns>Результат выполнения запроса</returns>
        Message.Result WriteSingleRegister(Message.Message request, ref UInt16 value);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 0x6. Записывает значение в одиночный регистр
        /// хранения в удалённом устройстве
        /// </summary>
        /// <param name="addressSlave">Адерс подчинённого устройства</param>
        /// <param name="addressRegister">Адрес записываемого регистра хранения</param>
        /// <param name="value">Значение регистра до и после записи</param>
        /// <returns>Результат выполнения запроса</returns>
        Message.Result WriteSingleRegister(Byte addressSlave, UInt16 addressRegister,
            ref UInt16 value);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 0x7. Только для последовательной линии.
        /// Смысл функции не доконца понял.
        /// </summary>
        /// <param name="AddressSlave">адрес удалённого slave-устройства</param>
        void ReadExceptionStatus(byte AddressSlave);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 0х7. Только для последовательной линии.
        /// Диагностическая функция
        /// </summary>
        /// <param name="AddressSlave">Адрес удалённого slave-устройства</param>
        /// <param name="SubFunction">Код подфункции</param>
        /// <param name="Data">данные</param>
        //public void Diagnostics(byte AddressSlave,
        //    SubFunction SubFunction, UInt16[] Data);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 11. Только для последовательной линии.
        /// Не разбирался с этой функцией
        /// </summary>
        /// <param name="AddressSlave">Адрес удалённого slave-устройства</param>
        void GetCommEventCounter(byte AddressSlave);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 12 (0х0С). Только для последовательной линии.
        /// Не разбирался с этой функцией
        /// </summary>
        /// <param name="AddressSlave">Адрес удалённого slave-устройства</param>
        void GetComEventLog(byte AddressSlave);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 15 (0x0F). Устанавливет состояния массива реле 
        /// </summary>
        /// <param name="AddressSlave">Адрес удалённого slave-устройства,
        /// в котором необходимо установить состояния последовательности реле</param>
        /// <param name="StartingAddress">Адрес начального реле</param>
        /// <param name="value">Количество и состояния реле для установки</param>
        /// <returns>Результат выполнения операции</returns>
        Modbus.OSIModel.Message.Result WriteMultipleCoils(byte AddressSlave,
            UInt16 StartingAddress, State[] coils);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 16 (0х10). Записывает значения массива регистров
        /// </summary>
        /// <param name="request">Запрос на запись регистров хранения</param>
        /// <returns>Результат выполнения операции</returns>
        Modbus.OSIModel.Message.Result WriteMultipleRegisters(Message.Message request);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 16 (0х10). Записывает значения массива регистров хранения
        /// </summary>
        /// <param name="AddressSlave">Адрес подчинённого устройства</param>
        /// <param name="StartingAddress">Адрес первого регистра хранения в блоке</param>
        /// <param name="value">Массив значений регистров хранения для 
        /// записи (и длина блока записи)</param>
        /// <returns>Результат выполнения операции</returns>
        Modbus.OSIModel.Message.Result WriteMultipleRegisters(byte AddressSlave,
            UInt16 StartingAddress, UInt16[] value);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 17 (0х11). Только для последовательной линии.
        /// Не разбирался.
        /// </summary>
        /// <param name="AddressSlave">Адрес удалённого slave-устройства</param>
        void ReportSlaveID(byte AddressSlave);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 20 (0х14). С функцией не разбирался
        /// </summary>
        void ReadFileRecord();
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 21 (0х15). С функцией не разбирался
        /// </summary>
        void WriteFileRecord();
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 22 (0х16). С функцией не разбирался
        /// </summary>
        /// <param name="AddressSlave"></param>
        /// <param name="ReferenceAddress"></param>
        /// <param name="And_Mask"></param>
        /// <param name="Or_Mask"></param>
        void MaskWriteRegister(byte AddressSlave,
            UInt16 ReferenceAddress, UInt16 And_Mask,
            UInt16 Or_Mask);
        //---------------------------------------------------------------------------
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
        void ReadWriteMultipleRegisters(byte AddressSlave,
            UInt16 ReadStartingAddress, UInt16 QuantityToRead,
            UInt16 WriteStartingAddress, UInt16 QuantityToWrite,
            byte WriteByteCount, UInt16 WriteRegistersValue);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 24 (0х18). С Функцией не разбирался
        /// </summary>
        /// <param name="AddressSlave"></param>
        /// <param name="FIFOPointerAddress"></param>
        void ReadFIFOQueue(byte AddressSlave,
            UInt16 FIFOPointerAddress);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Метод отсылает запрос с определённым пользователем
        /// кодом и данными.
        /// </summary>
        /// <param name="address">Адрес slave-устройства</param>
        /// <param name="code">Код функции</param>
        /// <param name="data">Данные сообщения</param>
        /// <param name="Answer">Ответное сообщение</param>
        /// <returns>Если запрос прошёл успешно возвращается true</returns>
        Message.Result SendUserDefinedRequest(Byte address,
            Byte code, Byte[] data);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает интерфейс объекта уровня Datalink layer
        /// </summary>
        Modbus.OSIModel.DataLinkLayer.Master.IDataLinkLayer GetDataLinkObject();
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file