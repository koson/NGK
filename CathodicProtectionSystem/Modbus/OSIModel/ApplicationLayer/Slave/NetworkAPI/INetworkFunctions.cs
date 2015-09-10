using System;
//
using Modbus.OSIModel.Message;

//=================================================================================
namespace Modbus.OSIModel.ApplicationLayer.Slave.NetworkAPI
{
    //=============================================================================
    /// <summary>
    /// Интерфейс реализует команды для работы с сетью Modbus
    /// в режиме Slave-устройства
    /// </summary>
    internal interface INetworkFunctions
    {
        //-------------------------------------------------------------------------
        /// <summary>
        /// Функция 0x1. Чтение реле (Выполнение запроса) 
        /// </summary>
        /// <param name="request">Принятое сообщение с запросом</param>
        /// <returns>Результат выполнения операции</returns>
        Result ReadCoils(Message.Message request);
        //-------------------------------------------------------------------------
        /// <summary>
        /// Функция 0x2. Читает дискретные входы
        /// в удалённом устройстве
        /// </summary>
        /// <param name="request">Принятое сообщение с запросом</param>
        /// <returns>Результат выполения операции</returns>
        Result ReadDiscreteInputs(Message.Message request);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 0х3. Читает holding-регистры
        /// в удалённом устройстве
        /// </summary>
        /// <param name="request">Запрос на чтение регистров</param>
        /// <returns>Результат выполнения запроса</returns>
        Result ReadHoldingRegisters(Message.Message request);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 0х4. Читает входные регистры
        /// </summary>
        /// <param name="request">Принятое сообщение с запросом</param>
        /// <returns>Результат выполнения операции</returns>
        Result ReadInputRegisters(Message.Message request);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 0х5. Устанавливает реле в состояние вкл./выкл. 
        /// в удалённом устройстве
        /// </summary>
        /// <param name="request">Принятое сообщение с запросом</param>
        /// <returns>Результат выполнения операции</returns>
        Result WriteSingleCoil(Message.Message request);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 0x6. Записывает значение в одиночный регистр
        /// хранения в удалённом устройстве
        /// </summary>
        /// <param name="request">Запрос на запись регистра</param>
        /// <returns>Результат выполнения запроса</returns>
        Result WriteSingleRegister(Message.Message request);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 0x7. Только для последовательной линии.
        /// Смысл функции не доконца понял.
        /// </summary>
        /// <param name="AddressSlave">адрес удалённого slave-устройства</param>
        //void ReadExceptionStatus(byte AddressSlave);
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
        //void GetCommEventCounter(byte AddressSlave);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 12 (0х0С). Только для последовательной линии.
        /// Не разбирался с этой функцией
        /// </summary>
        /// <param name="AddressSlave">Адрес удалённого slave-устройства</param>
        //void GetComEventLog(byte AddressSlave);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 15 (0x0F). Устанавливет состояния массива реле 
        /// </summary>
        /// <param name="request">Запрос на запись регистров хранения</param>
        /// <returns>Результат выполнения операции</returns>
        Result WriteMultipleCoils(Message.Message request);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 16 (0х10). Записывает значения массива регистров
        /// </summary>
        /// <param name="request">Запрос на запись регистров хранения</param>
        /// <returns>Результат выполнения операции</returns>
        Result WriteMultipleRegisters(Message.Message request);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 17 (0х11). Только для последовательной линии.
        /// Не разбирался.
        /// </summary>
        /// <param name="AddressSlave">Адрес удалённого slave-устройства</param>
        //void ReportSlaveID(byte AddressSlave);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 20 (0х14). С функцией не разбирался
        /// </summary>
        Result ReadFileRecord(Message.Message request);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 21 (0х15). С функцией не разбирался
        /// </summary>
        //void WriteFileRecord();
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 22 (0х16). С функцией не разбирался
        /// </summary>
        /// <param name="AddressSlave"></param>
        /// <param name="ReferenceAddress"></param>
        /// <param name="And_Mask"></param>
        /// <param name="Or_Mask"></param>
        //void MaskWriteRegister(byte AddressSlave,
        //    UInt16 ReferenceAddress, UInt16 And_Mask,
        //    UInt16 Or_Mask);
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
        //void ReadWriteMultipleRegisters(byte AddressSlave,
        //    UInt16 ReadStartingAddress, UInt16 QuantityToRead,
        //    UInt16 WriteStartingAddress, UInt16 QuantityToWrite,
        //    byte WriteByteCount, UInt16 WriteRegistersValue);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Функция 24 (0х18). С Функцией не разбирался
        /// </summary>
        /// <param name="AddressSlave"></param>
        /// <param name="FIFOPointerAddress"></param>
        //void ReadFIFOQueue(byte AddressSlave,
        //    UInt16 FIFOPointerAddress);
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
        //Message.Result SendUserDefinedRequest(Byte address,
        //    Byte code, Byte[] data);
        //---------------------------------------------------------------------------
        /// <summary>
        /// Реализует отсылку исключения 0x01 
        /// (Функция не реализована или неподдерживается)
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <returns>Результат выполнения</returns>
        Message.Result FunctionNotSupported(Message.Message request);
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file
