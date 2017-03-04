using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

//==============================================================================================================
namespace NGK.MeasuringDeviceTech.Classes.MeasuringDevice
{
    //==========================================================================================================
    /// <summary>
    /// Класс реализует визитную карточку устройства NGK
    /// </summary>
    [Serializable]
    public class CallingCard
    {
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Тип устройства NGK
        /// </summary>
        private TYPE_NGK_DEVICE _TypeOfDevice;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Тип устройства NGK
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Системные данные")]
        [Description("Вариант исполнения устройства НГК-БИ в составе КИП")]
        [DisplayName(@"Тип НГК-БИ")]
        public TYPE_NGK_DEVICE TypeOfDevice
        {
            get { return this._TypeOfDevice; }
            set { this._TypeOfDevice = value; }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Версия ПО
        /// </summary>
        /// <remarks>
        /// Input Register 0x0001
        /// </remarks>
        private float _SofwareVersion = 0;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Версия ПО
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Системные данные")]
        [Description("Версия ПО")]
        [DisplayName("Версия ПО")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float SofwareVersion
        {
            get { return _SofwareVersion; }
            set { _SofwareVersion = value; }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Версия аппаратуры
        /// </summary>
        /// <remarks>
        /// Input Register 0x0002
        /// </remarks>
        private float _HardwareVersion = 0;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Версия аппаратуры
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Системные данные")]
        [Description("Версия аппаратуры")]
        [DisplayName("Версия аппаратуры")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public float HardwareVersion
        {
            get { return _HardwareVersion; }
            set { _HardwareVersion = value; }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Серийный номер устройства
        /// </summary>
        /// <remarks>
        /// Input register 0x000C и 0x000D
        /// </remarks>
        private UInt64 _SerialNumber = UInt64.MaxValue;
        /// <summary>
        /// Серийный номер устройства
        /// </summary>
        //------------------------------------------------------------------------------------------------------
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Системные данные")]
        [Description("Серийный номер устройства")]
        [DisplayName("Серийный номер")]
        [DefaultValue(typeof(UInt64), "0xFFFFFFFFFF")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public UInt64 SerialNumber
        {
            get { return _SerialNumber; }
            set { _SerialNumber = value; }
        }
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Контрольная сумма визитной карточки
        /// </summary>
        private UInt16 _CRC16 = 0;
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Контрольная сумма визитной карточки
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Системные данные")]
        [Description("Контрольная сумма визитной карточки устройства")]
        [DisplayName("Контрольная сумма")]
        //[DefaultValue(typeof(UInt64), "0xFFFFFFFFFFFF")]
        [RefreshProperties(System.ComponentModel.RefreshProperties.All)]
        public UInt16 CRC16
        {
            get { return _CRC16; }
            set { _CRC16 = value; }
        }        
        //------------------------------------------------------------------------------------------------------
    }
    //==========================================================================================================
    /// <summary>
    /// Класс для работы с устройством NGK по MODBUS протоколу
    /// </summary>
    public static class CallingCardOfDevice
    {
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Записывает серийный номер в устройство БИ при первоначальной инициализации
        /// </summary>
        /// <param name="host">Modbus master устройство</param>
        /// <param name="error">Результат выполнеия операции</param>
        /// <param name="addressSlave">Адрес подчинённого устройство</param>
        /// <param name="serialNumber">Серийный номер для записи в устройство</param>
        public static void Write_HR_SerialNumber(ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error, Byte addressSlave ,UInt64 serialNumber)
        {
            // Записываем новое значение в устройство
            Modbus.OSIModel.Message.Result result;
            UInt16[] registers = new ushort[3] { 0, 0, 0 };
            String msg;

            //this.Cursor = Cursors.WaitCursor;

            unchecked
            {
                registers[0] = (UInt16)serialNumber;
                registers[1] = (UInt16)(serialNumber >> 16);
                registers[2] = (UInt16)(serialNumber >> 32);
            }

            result = host.WriteMultipleRegisters(
                addressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.SerialNumber,
                registers);

            if (result.Error != Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            else
            {
                // Проверяем правильно ли записался параметр
                // Читаем записанный параметр
                result = host.ReadHoldingRegisters(addressSlave,
                    BI_ADDRESSES_OF_HOLDINGREGISTERS.SerialNumber,
                    3, out registers);

                if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
                {
                    if (registers.Length != 3)
                    {
                        msg = String.Format(
                            "Ответ НГК-БИ содержит количество прочитанных регистров {0}, должно быть 3",
                            registers.Length);
                        error = new OperationResult(OPERATION_RESULT.FAILURE, msg);
                    }
                    else
                    {
                        UInt64 number_wr;
                        number_wr = 0;
                        number_wr = registers[2];
                        number_wr |= (number_wr << 16);
                        number_wr |= registers[1];
                        number_wr |= (number_wr << 16);
                        number_wr |= registers[0];

                        if (number_wr != serialNumber)
                        {
                            msg = String.Format(
                                "Значение записанного прараметра {0} не совподает с прочитанным {1}",
                                serialNumber, number_wr);
                            error = new OperationResult(OPERATION_RESULT.FAILURE, msg);
                        }
                        else
                        {
                            error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        }
                    }
                }
                else
                {
                    error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
                }
            }
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Читает серийный номер в устройство БИ при первоначальной инициализации
        /// </summary>
        /// <param name="host">Modbus master устройство</param>
        /// <param name="error">Результат выполнеия операции</param>
        /// <param name="addressSlave">Адрес подчинённого устройство</param>
        /// <param name="serialNumber">Серийный номер для записи в устройство</param>
        public static void Read_HR_SerialNumber(ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error, Byte addressSlave, out UInt64 serialNumber)
        {
            ushort[] registers;
            Modbus.OSIModel.Message.Result result;
            String msg;

            serialNumber = 0;

            result = host.ReadHoldingRegisters(addressSlave,
                BI_ADDRESSES_OF_HOLDINGREGISTERS.SerialNumber,
                3, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                if (registers.Length != 3)
                {
                    msg = String.Format(
                        "Ответ НГК-БИ содержит количесво прочитанных регистров {0}, должно быть 3",
                        registers.Length);
                    error = new OperationResult(OPERATION_RESULT.FAILURE, msg);
                }
                else
                {
                    serialNumber = 0;
                    serialNumber = registers[2];
                    serialNumber |= (serialNumber << 16);
                    serialNumber |= registers[1];
                    serialNumber |= (serialNumber << 16);
                    serialNumber |= registers[0];
                    error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                }
            }
            else
            {
                error = new OperationResult(OPERATION_RESULT.FAILURE, String.Empty);
            }
            return;
        }        
        //------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Читает визитную карточку NGK-устройства
        /// </summary>
        /// <param name="host"></param>
        /// <param name="error"></param>
        /// <param name="addressSlave"></param>
        /// <param name="card"></param>
        public static void Read_IRs_CallingCard(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error, 
            Byte addressSlave, out CallingCard card)
        {
            ushort[] registers;
            Modbus.OSIModel.Message.Result result;
            String msg;
            UInt16 startAddress = 0x0000; // Адрес начального входного регистра визитной карточки
            UInt16 length = 7;
            
            result = host.ReadInputRegisters(addressSlave,
                startAddress, length, out registers);

            if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
            {
                if (registers.Length != 7)
                {
                    msg = String.Format(
                        "Ответ НГК-БИ содержит количесво прочитанных регистров {0}, должно быть 7",
                        registers.Length);
                    card = null;
                    error = new OperationResult(OPERATION_RESULT.FAILURE, msg);
                }
                else
                {
                    // Рассчитываем контрольную сумму.
                    List<Byte> arr = new List<Byte>();
                    
                    // Получаем массив байт (без регистра с CRC16)
                    for (int i = 0; i < (registers.Length - 1); i++)
                    {
                        arr.AddRange(Modbus.Convert.ConvertToBytes(registers[i]));
                    }

                    // Рассчитываем CRC16 и сравниваем с прочитанным
                    Byte[] crc16calc_b = Modbus.OSIModel.DataLinkLayer.CRC16.CalcCRC16(arr.ToArray());
                    UInt16 crc16calc = 0;
                    crc16calc = crc16calc_b[1]; // Hi_byte crc16
                    crc16calc = (UInt16)(crc16calc << 8);
                    crc16calc |= crc16calc_b[0]; // Lo_byte crc16


                    // Проверяем контрольную сумму (прочитанную и рассчитаную)
                    if (registers[registers.Length - 1] == crc16calc)
                    {
                        // Контрольная сумма сошлась, получаем данные визтной карты

                        card = new CallingCard();
                        
                        // Получаем тип устройства
                        try
                        {
                            card.TypeOfDevice = (TYPE_NGK_DEVICE)Enum.Parse(typeof(TYPE_NGK_DEVICE), registers[0].ToString());
                        }
                        catch
                        {
                            msg = String.Format(
                                "Неизвестный тип устройства, код типа устройства: {0}",
                                registers[0]);
                            error = new OperationResult(OPERATION_RESULT.FAILURE, msg);
                            return;
                        }

                        // Получаем версию ПО
                        card.SofwareVersion = ((float)registers[1]) / 100;
                        
                        // Получаем версию Аппаратуры
                        card.HardwareVersion = ((float)registers[2]) / 100;

                        // Получаем серийный номер
                        card.SerialNumber = 0;
                        card.SerialNumber = registers[3];
                        card.SerialNumber |= (card.SerialNumber << 16);
                        card.SerialNumber |= registers[4];
                        card.SerialNumber |= (card.SerialNumber << 16);
                        card.SerialNumber |= registers[5];

                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                    }
                    else
                    {
                        msg =
                            "Конрольная сумма визитной краточки не совпала с рассчётной";
                        card = null;
                        error = new OperationResult(OPERATION_RESULT.FAILURE, msg);
                    }
                }
            }
            else
            {
                // При выполнени чтения устройства возникли проблемы...
                card = null;
                error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
            }
            return;
        }
        //------------------------------------------------------------------------------------------------------
    }
    //==========================================================================================================
}
//==============================================================================================================
// End of file