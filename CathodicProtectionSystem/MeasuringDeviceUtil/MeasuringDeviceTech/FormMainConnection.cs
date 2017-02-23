using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Modbus.OSIModel.ApplicationLayer.Master;
using Modbus.OSIModel.DataLinkLayer.Master;
using Modbus.OSIModel.DataLinkLayer.Master.RTU.SerialPort;
using Modbus.OSIModel.Message;
using NGK.Devices;
using System.Reflection;
using System.IO;
using NGK.MeasuringDeviceTech.Classes.MeasuringDevice;

//====================================================================================
namespace NGK.MeasuringDeviceTech
{
    //================================================================================
    /// <summary>
    /// Часть класса формы содержащая функционал работы в сети Modbus
    /// </summary>
    public partial class FormMain : Form
    {
        //----------------------------------------------------------------------------
        #region Fields and Properties
        //----------------------------------------------------------------------------
        private FileStream _File;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Последовательный порт для подключения к сети Modbus
        /// </summary>
        private ComPort _SerialPort;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Функционал для работы с устройством в сети
        /// </summary>
        private Modbus.OSIModel.ApplicationLayer.Master.Device _Host;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Виртуальное устройство БИ
        /// </summary>
        //private MeasuringDeviceMainPower _MeasuringDevice;
        private IMeasuringDevice _MeasuringDevice;
        //----------------------------------------------------------------------------
        #endregion
        //----------------------------------------------------------------------------
        #region Methods
        //----------------------------------------------------------------------------
        /// <summary>
        /// Событие возникает при изменении одного из свойсв устройства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _MeasuringDevice_PropertyChanged(object sender, 
            PropertyChangedEventArgs e)
        {
            if (this.propertyGridMain.InvokeRequired)
            {
                this.propertyGridMain.Invoke(new RefreshPropertyGrid(PropertyGridRefresh));
            }
            else
            {
                this.propertyGridMain.Refresh();
            }
            return;
        } 
        //----------------------------------------------------------------------------
        private delegate void RefreshPropertyGrid();
        /// <summary>
        /// Метод обновляет содержимое PropertyGrid (для асинхронных вызовов)
        /// </summary>
        private void PropertyGridRefresh()
        {
            this.propertyGridMain.Refresh();
            return;
        }
        //----------------------------------------------------------------------------
        public void Read_IR_TypeOfDevice(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error, Byte addressSlave, out TYPE_NGK_DEVICE typeOfDevice)
        {
            Modbus.OSIModel.Message.Result result;
            UInt16[] registers;

            if (host == null)
            {
                typeOfDevice = TYPE_NGK_DEVICE.UNKNOWN_DEVICE;
                error = new OperationResult(OPERATION_RESULT.CONNECTION_ERROR, 
                    "Подключение к сети не создано");
            }
            else
            {
                if (host.DataLinkObject.IsOpen())
                {
                    result = host.ReadInputRegisters(addressSlave,
                        BI_ADDRESSES_OF_INPUTREGISTERS.TypeOfDevice,
                        1, out registers);

                    if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
                    {
                        try
                        {
                            typeOfDevice =
                                (TYPE_NGK_DEVICE)Enum.Parse(typeof(TYPE_NGK_DEVICE),
                                registers[0].ToString());
                            error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                        }
                        catch
                        {
                            typeOfDevice = TYPE_NGK_DEVICE.UNKNOWN_DEVICE;
                            error = new OperationResult(OPERATION_RESULT.UNKNOWN_DEVICE, 
                                String.Format(
                                "Устройство вернуло код неизвестного типа устройства: {0}", registers[0].ToString()));
                        }
                    }
                    else
                    {
                        typeOfDevice = TYPE_NGK_DEVICE.UNKNOWN_DEVICE;
                        error = new OperationResult(OPERATION_RESULT.FAILURE, 
                            result.Description);
                    }
                }
                else
                {
                    typeOfDevice = TYPE_NGK_DEVICE.UNKNOWN_DEVICE;
                    error = new OperationResult(OPERATION_RESULT.CONNECTION_ERROR, 
                        "Соeдинение создано, но не открыто");
                }
            }
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Читает версию ПО подключенного устройства, без созданного виртуального
        /// </summary>
        /// <param name="host"></param>
        /// <param name="error"></param>
        /// <param name="addressSlave"></param>
        /// <param name="version"></param>
        public void Read_IR_SoftWareVersion(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error, Byte addressSlave, out float version)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            if (host == null)
            {
                version = 0;
                error = new OperationResult(OPERATION_RESULT.CONNECTION_ERROR, 
                    "Подключение к сети не создано");
            }
            else
            {
                if (host.DataLinkObject.IsOpen())
                {
                    result = host.ReadInputRegisters(addressSlave,
                        BI_ADDRESSES_OF_INPUTREGISTERS.VersionSoftware,
                        1, out registers);

                    if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
                    {
                        version = ((float)registers[0]) / 100;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                    }
                    else
                    {
                        version = 0;
                        error = new OperationResult(OPERATION_RESULT.FAILURE, result.Description);
                    }
                }
                else
                {
                    version = 0;
                    error = new OperationResult(OPERATION_RESULT.CONNECTION_ERROR, 
                        "Содениение создано, но порт закрыт");
                }
            }
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Читает версию аппаратного обеспечения подключенного устройства, 
        /// без созданного виртуального.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="error"></param>
        /// <param name="addressSlave"></param>
        /// <param name="version"></param>
        public void Read_IR_HardWareVersion(
            ref Modbus.OSIModel.ApplicationLayer.Master.Device host,
            out OperationResult error, Byte addressSlave, out float version)
        {
            Modbus.OSIModel.Message.Result result;
            //String msg;
            UInt16[] registers;

            this.LockGUI();

            if (host == null)
            {
                version = 0;
                error = new OperationResult(OPERATION_RESULT.CONNECTION_ERROR, 
                    "Подключение к сети не создано");
            }
            else
            {
                if (host.DataLinkObject.IsOpen())
                {
                    result = host.ReadInputRegisters(addressSlave,
                        BI_ADDRESSES_OF_INPUTREGISTERS.VersionHardware,
                        1, out registers);

                    if (result.Error == Modbus.OSIModel.ApplicationLayer.Error.NoError)
                    {
                        version = ((float)registers[0]) / 100;
                        error = new OperationResult(OPERATION_RESULT.OK, String.Empty);
                    }
                    else
                    {
                        version = 0;
                        error = new OperationResult(OPERATION_RESULT.FAILURE, 
                            result.Description);
                    }
                }
                else
                {
                    version = 0;
                    error = new OperationResult(OPERATION_RESULT.CONNECTION_ERROR, 
                        "Содениение создано, но порт закрыт");
                }
            }
            this.UnLockGUI();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Читает визитную карточку устройства
        /// </summary>
        /// <param name="result">Результат выполения операции чтения визитной карточки устройства</param>
        /// <param name="deviceInfo">Визитная карточка устройства</param>
        public void ReadCallingCard(
            out OperationResult result, 
            out CallingCard deviceInfo)
        {
            this.LockGUI();
            this.Cursor = Cursors.WaitCursor;
            NGK.MeasuringDeviceTech.Classes.MeasuringDevice.CallingCardOfDevice.Read_IRs_CallingCard(
                ref _Host, out  result, 1, out deviceInfo);
            this.Cursor = Cursors.Default;
            this.UnLockGUI();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Проверяет иницилизировано устройство или нет. Т.е. установлен
        /// серийный номер и следовательно не доступен регистр хранения 
        /// для записи серийного номера
        /// </summary>
        /// <param name="deviceInit">
        /// Если - true, то устройство инициализировано</param>
        /// <param name="error">Статус выполения операции по сети</param>
        public void VerifyInitDevice(
            out Boolean deviceInit, 
            out OperationResult error)
        {
            Boolean init;
            OperationResult result;
            this.LockGUI();
            // Читаем регистр хранения, если он не доступен, то устройство
            // инициализировано
            _MeasuringDevice.Read_HR_VerifyInitDevice(
                ref _Host, out init, out result);
            
            error = result;            
            deviceInit = init;
            this.UnLockGUI();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Блокирует доступ к элементам меню, панели 
        /// инструментов и контекстных меню
        /// Необходимо при выполении сетевых операций
        /// </summary>
        private void LockGUI()
        {
            // Блокируем меню и панель инструментов на время выполения операции
            this.toolStripMain.Enabled = false;
            this.menuStripMain.Enabled = false;
            this._ContexMenuConnection.Enabled = false;
            this._ContexMenuDevice.Enabled = false;
            this._ContexMenuParameter.Enabled = false;

            this.treeViewMain.Enabled = false;
            this.propertyGridMain.Enabled = false;

            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Разблокирует доступ к элементам меню, панели 
        /// инструментов и контекстных меню
        /// </summary>
        private void UnLockGUI()
        {
            this.toolStripMain.Enabled = true;
            this.menuStripMain.Enabled = true;
            this._ContexMenuConnection.Enabled = true;
            this._ContexMenuDevice.Enabled = true;
            this._ContexMenuParameter.Enabled = true;

            this.treeViewMain.Enabled = true;
            this.propertyGridMain.Enabled = true;

            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Метод читает все данные из устройства БИ
        /// </summary>
        private void DeviceRead()
        {
            OperationResult result;
            DialogResult rslt;

            this.LockGUI();
            // Читаем тип устройства и сверяем его с уже созданным виртуальным. Если  
            // несовпадают, то предлагаем созадать новое виртуальное устройство. Если
            // виртуальное устройство не создано, то по прочитанному значению создаём...
            // Если код типа устройства не опредлён, сообщаем об этом пользователю и выходим.
            TYPE_NGK_DEVICE type;

            this._MeasuringDevice.Read_IR_TypeOfDevice(ref _Host, 
                out result, out type);

            if (result.Result != OPERATION_RESULT.OK)
            {
                MessageBox.Show(this, String.Format(
                    "Неудалось определить тип устройства. {0}",
                    result.Message),
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.UnLockGUI();
                return;
            }
            else
            {
                if (_MeasuringDevice != null)
                {
                    if (_MeasuringDevice.GetDeviceType() != type)
                    {
                        // Тип виртуального и физического устройства не совпадают
                        DialogResult dlg = MessageBox.Show(this,
                            "Тип физического устройства не совпадает с текущим. Удалить текущее устройство и создать новое?",
                            "Внимание!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (dlg == System.Windows.Forms.DialogResult.OK)
                        {
                            switch (type)
                            {
                                case TYPE_NGK_DEVICE.BI_BATTERY_POWER:
                                    {
                                        this.DeleteDevice();
                                        this.CreateDevice((IMeasuringDevice)new MeasuringDeviceBatteryPower());
                                        break;
                                    }
                                case TYPE_NGK_DEVICE.BI_MAIN_POWERED:
                                    {
                                        this.DeleteDevice();
                                        this.CreateDevice((IMeasuringDevice)new MeasuringDeviceMainPower());
                                        break;
                                    }
                                default:
                                    {
                                        MessageBox.Show(this,
                                            "Невозможно выполнить операцию. Тип физического устройства не поддерживается в данной версии ПО.",
                                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        this.UnLockGUI();
                                        return;
                                    }
                            }
                        }
                        else
                        {
                            // Ползователь отменил операцию. Выходим
                            this.UnLockGUI();
                            return;
                        }

                    }
                }
                else
                {
                    // Создаём новое устройство устройство
                    switch (type)
                    {
                        case TYPE_NGK_DEVICE.BI_BATTERY_POWER:
                            {
                                this.DeleteDevice();
                                this.CreateDevice((IMeasuringDevice)new MeasuringDeviceBatteryPower());
                                break;
                            }
                        case TYPE_NGK_DEVICE.BI_MAIN_POWERED:
                            {
                                this.DeleteDevice();
                                this.CreateDevice((IMeasuringDevice)new MeasuringDeviceMainPower());
                                break;
                            }
                        default:
                            {
                                MessageBox.Show(this,
                                    "Невозможно выполнить операцию. Тип физического устройства не поддерживается в данной версии ПО.",
                                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.UnLockGUI();
                                return;
                            }
                    }
                }
            }
            
            // Читаем данные из устройства.
            NGK.Forms.FormProgressBar frm = new NGK.Forms.FormProgressBar();
            frm.progressBar.Value = 0;
            frm.labelMessage.Text = "Выполняется чтение данных из устройства НГК-БИ";
            frm.Show(this);
            //frm.StartPosition = FormStartPosition.CenterScreen;
            this.Cursor = Cursors.WaitCursor;

            // Читаем регистры хранения
            this._MeasuringDevice.Read_HR_BaudRateCAN(ref _Host, out result);
            frm.progressBar.Value = 3;
            if (result.Result != OPERATION_RESULT.OK)  
            {
                if (result.Result != OPERATION_RESULT.INVALID_OPERATION)
                {
                    rslt = MessageBox.Show(this, String.Concat(result.Message,
                        " Продолжить выполение?"),
                        "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    switch (rslt)
                    {
                        case System.Windows.Forms.DialogResult.Yes:
                            {
                                break;
                            }
                        case System.Windows.Forms.DialogResult.No:
                            {
                                frm.Close();
                                this.Cursor = Cursors.Default;
                                this.UnLockGUI();
                                return;
                            }
                    }
                }
            }
            this._MeasuringDevice.Read_HR_CurrentShuntValue(ref _Host, out result);
            frm.progressBar.Value = 6;
            if (result.Result != OPERATION_RESULT.OK)
            {
                if (result.Result != OPERATION_RESULT.INVALID_OPERATION)
                {
                    rslt = MessageBox.Show(this, String.Concat(result.Message,
                        " Продолжить выполение?"),
                        "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    switch (rslt)
                    {
                        case System.Windows.Forms.DialogResult.Yes:
                            {
                                break;
                            }
                        case System.Windows.Forms.DialogResult.No:
                            {
                                frm.Close();
                                this.Cursor = Cursors.Default;
                                this.UnLockGUI();
                                return;
                            }
                    }
                }
            }

            DateTime dt;
            this._MeasuringDevice.Read_HR_DateTime(ref _Host, out result, out dt);
            frm.progressBar.Value = 9;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }

            }

            this._MeasuringDevice.Read_HR_MeasuringPeriod(ref _Host, out result);
            frm.progressBar.Value = 12;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_HR_MeasuringVoltagePeriod(ref _Host, out result);
            frm.progressBar.Value = 15;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_HR_NetAddress(ref _Host, out result);
            frm.progressBar.Value = 18;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_HR_PollingPeriodBPI(ref _Host, out result);
            frm.progressBar.Value = 21;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_HR_PollingPeriodChannel1(ref _Host, out result);
            frm.progressBar.Value = 24;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_HR_PollingPeriodChannel2(ref _Host, out result);
            frm.progressBar.Value = 27;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_HR_PollingPeriodUSIKPST(ref _Host, out result);
            frm.progressBar.Value = 30;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }

            // Читаем реле
            this._MeasuringDevice.Read_CL_ExtendedModeX10SumPotentialEn(ref _Host, out result);
            frm.progressBar.Value = 36;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_CL_InducedVoltageEn(ref _Host, out result);
            frm.progressBar.Value = 39;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_CL_PolarizationPotentialEn(ref _Host, out result);
            frm.progressBar.Value = 42;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_CL_PolarizationСurrentEn(ref _Host, out result);
            frm.progressBar.Value = 44;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_CL_ProtectivePotentialEn(ref _Host, out result);
            frm.progressBar.Value = 46;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_CL_ProtectiveСurrentEn(ref _Host, out result);
            frm.progressBar.Value = 48;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_CL_SendStatusWordEn(ref _Host, out result);
            frm.progressBar.Value = 48;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_CL_DcCurrentRefereceElectrodeEn(ref _Host, out result);
            frm.progressBar.Value = 48;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_CL_AcCurrentRefereceElectrodeEn(ref _Host, out result);
            frm.progressBar.Value = 48;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            // Читаем состояния дискретных входов
            this._MeasuringDevice.Read_DI_BattaryVoltageStatus(ref _Host, out result);
            frm.progressBar.Value = 50;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_DI_CaseOpen(ref _Host, out result);
            frm.progressBar.Value = 52;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_DI_CorrosionSensor1(ref _Host, out result);
            frm.progressBar.Value = 54;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_DI_CorrosionSensor2(ref _Host, out result);
            frm.progressBar.Value = 56;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_DI_CorrosionSensor3(ref _Host, out result);
            frm.progressBar.Value = 58;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_DI_SupplyVoltageStatus(ref _Host, out result);
            frm.progressBar.Value = 60;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }

            // Читаем состояния входных регистров.
            this._MeasuringDevice.Read_IR_SoftWareVersion(ref _Host, out result);
            frm.progressBar.Value = 62;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }

            this._MeasuringDevice.Read_IR_HardWareVersion(ref _Host, out result);
            frm.progressBar.Value = 64;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }

            this._MeasuringDevice.Read_IR_SerialNumber(ref _Host, out result);
            frm.progressBar.Value = 66;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }

            this._MeasuringDevice.Read_IR_CRC16(ref _Host, out result);
            frm.progressBar.Value = 68;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }

            this._MeasuringDevice.Read_IR_ManufacturerCode(ref _Host, out result);
            frm.progressBar.Value = 70;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }


            this._MeasuringDevice.Read_IR_BattaryVoltage(ref _Host, out result);
            frm.progressBar.Value = 72;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_IR_Current_Channel_1(ref _Host, out result);
            frm.progressBar.Value = 74;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_IR_Current_Channel_2(ref _Host, out result);
            frm.progressBar.Value = 76;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            //this._MeasuringDevice.Read_IR_DateTime(ref _Host, out result);
            //frm.progressBar.Value = 78;
            //if ((result.Result != OPERATION_RESULT.OK) &&
            //    (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            //{
            //    rslt = MessageBox.Show(this, String.Concat(result.Message,
            //        " Продолжить выполение?"),
            //        "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            //    switch (rslt)
            //    {
            //        case System.Windows.Forms.DialogResult.Yes:
            //            {
            //                break;
            //            }
            //        case System.Windows.Forms.DialogResult.No:
            //            {
            //                frm.Close();
            //                this.Cursor = Cursors.Default;
            //                this.UnLockGUI();
            //                return;
            //            }
            //    }
            //}
            this._MeasuringDevice.Read_IR_DepthOfCorrosionUSIKPST(ref _Host, out result);
            frm.progressBar.Value = 80;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_IR_InducedVoltage(ref _Host, out result);
            frm.progressBar.Value = 82;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_IR_InternalTemperatureSensor(ref _Host, out result);
            frm.progressBar.Value = 84;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_IR_Polarization_current(ref _Host, out result);
            frm.progressBar.Value = 86;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_IR_Polarization_potential(ref _Host, out result);
            frm.progressBar.Value = 88;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_IR_Protective_current(ref _Host, out result);
            frm.progressBar.Value = 90;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_IR_Protective_potential(ref _Host, out result);
            frm.progressBar.Value = 92;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_IR_SpeedOfCorrosionUSIKPST(ref _Host, out result);
            frm.progressBar.Value = 94;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Read_IR_StatusUSIKPST(ref _Host, out result);
            frm.progressBar.Value = 96;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }

            this._MeasuringDevice.Read_IR_SupplyVoltage(ref _Host, out result);
            frm.progressBar.Value = 98;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }

            this._MeasuringDevice.Read_IR_ReferenceElectrodeDCCurrent(ref _Host, out result);
            frm.progressBar.Value = 98;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }

            this._MeasuringDevice.Read_IR_ReferenceElectrodeACCurrent(ref _Host, out result);
            frm.progressBar.Value = 98;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }

            frm.progressBar.Value = 100;
            frm.Close();
            this.Cursor = Cursors.Default;

            // Проверяем инициализировано устройство или нет. Если не инициализировано
            // предлягаем инициализировать
            //Boolean init;
            //this.Cursor = Cursors.WaitCursor;
            
            //this._MeasuringDevice.Read_HR_VerifyInitDevice(
            //    ref _Host, out init, out result);
            
            //this.Cursor = Cursors.Default;
            
            //if (!init)
            //{
            //    MessageBox.Show(this, 
            //        "Устройство не инициализировано", "Внимание", 
            //        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            this.UnLockGUI();
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Метод записывает все данные в устройство
        /// </summary>
        public void DeviceWrite()
        {
            OperationResult result;
            DialogResult rslt;
            NGK.Forms.FormProgressBar frm = new NGK.Forms.FormProgressBar();
            frm.progressBar.Value = 0;
            frm.labelMessage.Text = "Выполняется запись данных в устройство НГК-БИ";
            frm.Show(this);
            frm.StartPosition = FormStartPosition.CenterParent;

            // Записываем регистры хранения
            this._MeasuringDevice.Write_HR_BaudRateCAN(ref _Host, out result);
            frm.progressBar.Value = 5;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Write_HR_CurrentShuntValue(ref _Host, out result);
            frm.progressBar.Value = 10;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Write_HR_DateTime(ref _Host, out result);
            frm.progressBar.Value = 15;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Write_HR_MeasuringPeriod(ref _Host, out result);
            frm.progressBar.Value = 20;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Write_HR_NetAddress(ref _Host, out result);
            frm.progressBar.Value = 25;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Write_HR_PollingPeriodBPI(ref _Host, out result);
            frm.progressBar.Value = 30;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Write_HR_PollingPeriodChannel1(ref _Host, out result);
            frm.progressBar.Value = 35;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Write_HR_PollingPeriodChannel2(ref _Host, out result);
            frm.progressBar.Value = 40;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Write_HR_PollingPeriodUSIKPST(ref _Host, out result);
            frm.progressBar.Value = 50;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }

            // Записываем реле
            this._MeasuringDevice.Write_CL_ExtendedModeX10SumPotentialEn(ref _Host, out result);
            frm.progressBar.Value = 60;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Write_CL_InducedVoltageEn(ref _Host, out result);
            frm.progressBar.Value = 70;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Write_CL_PolarizationPotentialEn(ref _Host, out result);
            frm.progressBar.Value = 80;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Write_CL_PolarizationСurrentEn(ref _Host, out result);
            frm.progressBar.Value = 85;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Write_CL_ProtectivePotentialEn(ref _Host, out result);
            frm.progressBar.Value = 95;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Write_CL_ProtectiveСurrentEn(ref _Host, out result);
            frm.progressBar.Value = 100;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Write_CL_SendStatusWordEn(ref _Host, out result);
            frm.progressBar.Value = 100;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Write_CL_DcCurrentRefereceElectrodeEn(ref _Host, out result);
            frm.progressBar.Value = 100;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            this._MeasuringDevice.Write_CL_AcCurrentRefereceElectrodeEn(ref _Host, out result);
            frm.progressBar.Value = 100;
            if ((result.Result != OPERATION_RESULT.OK) &&
                (result.Result != OPERATION_RESULT.INVALID_OPERATION))
            {
                rslt = MessageBox.Show(this, String.Concat(result.Message,
                    " Продолжить выполение?"),
                    "Ошибка", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (rslt)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            break;
                        }
                    case System.Windows.Forms.DialogResult.No:
                        {
                            frm.Close();
                            this.Cursor = Cursors.Default;
                            this.UnLockGUI();
                            return;
                        }
                }
            }
            frm.Close();
            this.Cursor = Cursors.Default;
            this.UnLockGUI();
            return;
        }
        //----------------------------------------------------------------------------        
        /// <summary>
        /// Сохраняем измениния в файл, если он был открыт или создаём новый файл
        /// </summary>
        /// <remarks>
        /// Сохраняет утройство КИП в файл, если КИП был открыт из файла сохраняем
        /// его новое значение, если КИП был создан в прогрмме предлаеме сохранить
        /// его в новый файл.
        /// </remarks>
        private void SaveFile()
        {
            // Проверяем отктыт ли файл для сохранения, если нет, то создаём 
            // Новый
            Boolean res;
            String msg;

            if (_MeasuringDevice != null)
            {
                if (_File != null)
                {
                    // Файл для записи открыт, сохраняем в него новые данные
                    //res = MeasuringDeviceMainPower.Serialize(_File, _MeasuringDevice);
                    res = _MeasuringDevice.Serialize(_File);

                    if (!res)
                    {
                        msg = "Не удалось сохранить файл";
                        MessageBox.Show(this, msg, "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);                       
                    }
                }
                else
                {
                    // Файла для записи не существует, создаём новый.
                    SaveFileDialog dlg = new SaveFileDialog();

                    dlg.Title = "Сохранить КИП в файл";
                    dlg.AddExtension = true;
                    dlg.DefaultExt = "kip";
                    dlg.SupportMultiDottedExtensions = true;
                    // Формируем имя файла: "КИП"_"Серийный номер КИП-а"_"Дата" 
                    dlg.FileName = "КИП_" + (_MeasuringDevice.GetSerialNumber()).ToString() + "_" +
                        DateTime.Now.ToShortDateString();
                    // Директория по умолчанию
                    //dlg.InitialDirectory = Properties.Settings.Default.FilesDirectory;
                    // Фильтр для файла
                    dlg.Filter = "NGK файлы образов КИП-ов (*.kip)|*.kip";

                    //dlg.CheckFileExists = true;
                    dlg.CheckPathExists = true;
                    dlg.OverwritePrompt = true;

                    DialogResult result = dlg.ShowDialog(this);

                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        // Открываем файл
                        _File = (FileStream)dlg.OpenFile();
                        // Сохраняем в него данные
                        //MeasuringDeviceMainPower.Serialize(_File, _MeasuringDevice);
                        _MeasuringDevice.Serialize(_File);
                        // Сохраняем путь к файлу
                        ((ToolStripLabel)this.statusStripMain.Items["PathToFile"]).Text = dlg.FileName; 
                    }
                }
            }
            else
            {
                msg = "Нет устройства для сохранения";
                MessageBox.Show(this, msg, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Открывает файл образа устройства КИП
        /// </summary>
        /// <param name="path">Путь к файлу - образу КИП</param>
        private void OpenFile(String path)
        {
            DialogResult result;

            // Проверяем если открыт другой файл, закрываем его
            if (_File != null)
            {
                result = MessageBox.Show(this, "Сохранить и закрыть текущее устройство КИП?",
                    "Внимание", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // Сохраняем и закрываем
                    SaveFile();
                    _File.Flush();
                    _File.Close();
                    GC.Collect();
                    _File = null;

                    // Выполняем открытие файла образа 
                    // Проверяем существование файла
                    if (File.Exists(path))
                    {
                        // Файл существует окрываем его
                        _File = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

                        // Десериализуем объект
                        Object obj = MeasuringDeviceMainPower.Deserialize(_File);

                        if (obj == null)
                        {
                            obj = MeasuringDeviceBatteryPower.Deserialize(_File);

                            if (obj == null)
                            {
                                MessageBox.Show(this,
                                    "Неудалось открыть файл, формат не известен", "Ошибки",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }

                        IMeasuringDevice dv = (IMeasuringDevice)obj;

                        if (_MeasuringDevice == null)
                        {
                            MessageBox.Show(this,
                                "Неудалость открыть устройство КИП",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            _File.Flush();
                            _File.Close();
                            GC.Collect();
                            _File = null;
                        }
                        else
                        {
                            // Сохраняем путь к файлу
                            ((ToolStripLabel)this.statusStripMain.Items["PathToFile"]).Text = path;
                            // Добавляем устройство
                            CreateDevice(dv);
                        }
                    }
                    else
                    {
                        // Файл не существует...
                        MessageBox.Show(this,
                            String.Format("Файл не найден по указанному пути: {0}", path),
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                // Выполняем открытие файла образа 
                // Проверяем существование файла
                if (File.Exists(path))
                {
                    // Файл существует окрываем его
                    _File = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

                    // Десериализуем объект
                    Object obj = MeasuringDeviceMainPower.Deserialize(_File);

                    if (obj == null)
                    {
                        obj = MeasuringDeviceBatteryPower.Deserialize(_File);

                        if (obj == null)
                        {
                            MessageBox.Show(this,
                                "Неудалось открыть файл, формат не известен", "Ошибки",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    IMeasuringDevice dv = (IMeasuringDevice)obj;

                    if (dv == null)
                    {
                        MessageBox.Show(this,
                            "Неудалость открыть устройство КИП",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _File.Flush();
                        _File.Close();
                        GC.Collect();
                        _File = null;
                    }
                    else
                    {
                        // Сохраняем путь к файлу
                        ((ToolStripLabel)this.statusStripMain.Items["PathToFile"]).Visible = true;
                        ((ToolStripLabel)this.statusStripMain.Items["PathToFile"]).Text = path;
                        // Добавляем устройство
                        CreateDevice(dv);
                    }
                }
                else
                {
                    // Файл не существует...
                    MessageBox.Show(this,
                        String.Format("Файл не найден по указанному пути: {0}", path),
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Открывает файл образа устройства КИП
        /// </summary>
        private void OpenFile()
        {
            DialogResult result;
            // Проверяем. Если устройство создано, но файл с ним не связан
            // Предлагаем сохранить его
            if (_MeasuringDevice != null)
            {
                // Если удаление устройства отменено пользователем, то просто выходим
                if (!this.DeleteDevice())
                {
                    return;
                }
            }

            // Проверяем если открыт другой файл, закрываем его
            if (_File != null)
            {
                result = MessageBox.Show(this, "Сохранить и закрыть текущее устройство КИП?",
                    "Внимание", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // Сохраняем и закрываем
                    SaveFile();
                    _File.Flush();
                    _File.Close();
                    GC.Collect();
                    _File = null;

                    // Выполняем открытие файла образа
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.Title = "Открыть файл образа КИП";
                    dlg.Multiselect = false;
                    //dlg.InitialDirectory = Properties.Settings.Default.FilesDirectory;
                    dlg.DefaultExt = "kip";
                    dlg.Filter = "NGK файлы образов КИП-ов (*.kip)|*.kip";

                    result = dlg.ShowDialog(this);

                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        // Открываем файл
                        _File = (FileStream)dlg.OpenFile();
                        // Десериализуем объект
                        Object obj = MeasuringDeviceMainPower.Deserialize(_File);
                        
                        if (obj == null)
                        {
                            obj = MeasuringDeviceBatteryPower.Deserialize(_File);

                            if (obj == null)
                            {
                                _File.Flush();
                                _File.Close();
                                GC.Collect();
                                _File = null;

                                MessageBox.Show(this,
                                    "Неудалось открыть файл, формат не известен", "Ошибки",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                                return;
                            }
                        }
                            
                        _MeasuringDevice = (IMeasuringDevice)obj;

                        if (_MeasuringDevice == null)
                        {
                            MessageBox.Show(this,
                                "Неудалость открыть устройство КИП",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            _File.Flush();
                            _File.Close();
                            GC.Collect();
                            _File = null;
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                // Если файл не открыт, открываем...
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Title = "Открыть файл образа КИП";
                dlg.Multiselect = false;
                //dlg.InitialDirectory = Properties.Settings.Default.FilesDirectory;
                dlg.DefaultExt = "kip";
                dlg.Filter = "NGK файлы образов КИП-ов (*.kip)|*.kip";

                result = dlg.ShowDialog(this);

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // Открываем файл
                    //_File = File.Open(dlg.FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    _File = new FileStream(dlg.FileName, FileMode.Open,
                        FileAccess.ReadWrite, FileShare.None);
                    // Десериализуем объект
                    IMeasuringDevice dv;
                    Object obj = MeasuringDeviceMainPower.Deserialize(_File);
                    
                    if (obj == null)
                    {
                        obj = MeasuringDeviceBatteryPower.Deserialize(_File);

                        if (obj == null)
                        {
                            _File.Flush();
                            _File.Close();
                            GC.Collect();
                            _File = null;

                            MessageBox.Show(this,
                                "Неудалось открыть файл, формат не известен", "Ошибки",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    
                    dv = (IMeasuringDevice)obj;

                    if (dv == null)
                    {
                        MessageBox.Show(this,
                            "Неудалость открыть устройство КИП",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _File.Flush();
                        _File.Close();
                        GC.Collect();
                        _File = null;
                    }
                    else
                    {
                        // Сохраняем путь к файлу
                        ((ToolStripLabel)this.statusStripMain.Items["PathToFile"]).Visible = true;
                        ((ToolStripLabel)this.statusStripMain.Items["PathToFile"]).Text = dlg.FileName; 
                        // Добавляем устройство
                        CreateDevice(dv);
                    }
                }
            }
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Устанавливает элементы меню и элементы управления в начальное состояние
        /// </summary>
        private void DefaultState()
        { 
            ToolStripMenuItem itemMenu;
            ToolStripButton button;

            // Главное меню:
            // Меню "Файл"
            itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuFile"]);
            itemMenu.Enabled = true;
            itemMenu.DropDownItems["mnuFileCreate"].Enabled = true;
            itemMenu.DropDownItems["mnuFileOpen"].Enabled = true;
            itemMenu.DropDownItems["mnuFileClose"].Enabled = false;
            itemMenu.DropDownItems["mnuFileSave"].Enabled = false;
            itemMenu.DropDownItems["mnuFileSaveAs"].Enabled = false;
            itemMenu.DropDownItems[MENU.mnuFilePrint].Enabled = false;
            itemMenu.DropDownItems[MENU.mnuFilePrintPreview].Enabled = false;
            itemMenu.DropDownItems[MENU.mnuFilePrintSettings].Enabled = false;
            itemMenu.DropDownItems[MENU.mnuFilePageSettings].Enabled = false;
            itemMenu.DropDownItems["mnuFileExit"].Enabled = true;
            // Меню "Устройство КИП"
            itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuDevice"]);
            itemMenu.Enabled = false;
            itemMenu.DropDownItems["mnuDeviceRead"].Enabled = true;
            itemMenu.DropDownItems["mnuDeviceWrite"].Enabled = true;
            // Меню "Подключение"
            itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuConnection"]);
            itemMenu.Enabled = true;
            itemMenu.DropDownItems["mnuConnectionConnect"].Enabled = true;
            itemMenu.DropDownItems["mnuConnectionDisconnect"].Enabled = false;
            // Меню "Помощь"
            itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuHelp"]);
            itemMenu.Enabled = true;
            itemMenu.DropDownItems["mnuHelpHelp"].Enabled = true;
            itemMenu.DropDownItems["mnuHelpAbout"].Enabled = true;

            // Инициализируем панель инструментов
            button = (ToolStripButton)this.toolStripMain.Items["buttonSaveDevice"];
            button.Enabled = false;
            button = (ToolStripButton)this.toolStripMain.Items["buttonNewDevice"];
            button.Enabled = true;
            button = (ToolStripButton)this.toolStripMain.Items["buttonDeleteDevice"];
            button.Enabled = false;
            button = (ToolStripButton)this.toolStripMain.Items["buttonConnection"];
            button.Enabled = true;
            button = (ToolStripButton)this.toolStripMain.Items["buttonReadDevice"];
            button.Enabled = false;
            button = (ToolStripButton)this.toolStripMain.Items["buttonWriteDevice"];
            button.Enabled = false;

            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Создаём виртуальное устройство и функционал для работы с ним
        /// </summary>
        /// <param name="device">Устройство КИП, 
        /// если передаётся null - создаётся новое устройство</param>
        private void CreateDevice(IMeasuringDevice device)
        {
            if (device == null)
            {
                // Открываем форму для создания нового устройства
                Forms.FormSelectTypeDevice frm = new Forms.FormSelectTypeDevice();
                DialogResult result = frm.ShowDialog(this);

                if (result != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                else
                {
                    // Создаём экземпляр
                    switch (frm.TypeDevice)
                    {
                        case TYPE_NGK_DEVICE.BI_BATTERY_POWER:
                            {
                                //MessageBox.Show(this, "Данный тип БИ не поддерживается ПО",
                                //    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                //return;
                                MeasuringDeviceBatteryPower dv = new MeasuringDeviceBatteryPower();
                                device = (IMeasuringDevice)dv;
                                break; 
                            }
                        case TYPE_NGK_DEVICE.BI_MAIN_POWERED:
                            {
                                MeasuringDeviceMainPower dv = new MeasuringDeviceMainPower();
                                device = (IMeasuringDevice)dv;
                                break; 
                            }
                        default:
                            {
                                MessageBox.Show(this, "Данный тип НГК-БИ не поддерживается ПО", 
                                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                    }
                }
            }

            switch (device.GetDeviceType())
            {
                case TYPE_NGK_DEVICE.BI_MAIN_POWERED:
                    {
                        _Host = 
                            new Modbus.OSIModel.ApplicationLayer.Master.Device(
                                "NetworkModbus", (IDataLinkLayer)_SerialPort);
                        _MeasuringDevice = device;
                        _MeasuringDevice.PropertyChanged += 
                            new PropertyChangedEventHandler(_MeasuringDevice_PropertyChanged);
                        break;
                    }
                case TYPE_NGK_DEVICE.BI_BATTERY_POWER:
                    {
                        _Host = new 
                            Modbus.OSIModel.ApplicationLayer.Master.Device("NetworkModbus", 
                            (IDataLinkLayer)_SerialPort);
                        _MeasuringDevice = device;
                        _MeasuringDevice.PropertyChanged +=
                            new PropertyChangedEventHandler(_MeasuringDevice_PropertyChanged);
                        break;
                    }
                default:
                    {
                        throw new Exception("Попытка создания устройства НГК-БИ с неопределённым типом");
                    }
            }

            // Настраиваем элементы навигации, управления и меню
            ToolStripMenuItem itemMenu;
            ToolStripButton button;

            // Главное меню:
            // Меню "Файл"
            itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuFile"]);
            itemMenu.Enabled = true;
            itemMenu.DropDownItems["mnuFileCreate"].Enabled = false;
            itemMenu.DropDownItems["mnuFileOpen"].Enabled = true;
            itemMenu.DropDownItems["mnuFileClose"].Enabled = true;
            itemMenu.DropDownItems["mnuFileSave"].Enabled = true;
            itemMenu.DropDownItems["mnuFileSaveAs"].Enabled = true;
            itemMenu.DropDownItems[MENU.mnuFilePrint].Enabled = true;
            itemMenu.DropDownItems[MENU.mnuFilePrintPreview].Enabled = true;
            itemMenu.DropDownItems[MENU.mnuFilePrintSettings].Enabled = true;
            itemMenu.DropDownItems[MENU.mnuFilePageSettings].Enabled = true;
            itemMenu.DropDownItems["mnuFileExit"].Enabled = true;

            // Меню "Устройство КИП"
            if (_SerialPort.IsOpen())
            {
                itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuDevice"]);
                itemMenu.Enabled = true;
                itemMenu.DropDownItems["mnuDeviceRead"].Enabled = true;
                itemMenu.DropDownItems["mnuDeviceWrite"].Enabled = true;
            }
            else
            {
                itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuDevice"]);
                itemMenu.Enabled = false;
                itemMenu.DropDownItems["mnuDeviceRead"].Enabled = true;
                itemMenu.DropDownItems["mnuDeviceWrite"].Enabled = true;
            }
            // Меню "Подключение"
            //itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuConnection"]);
            //itemMenu.Enabled = true;
            //itemMenu.DropDownItems["mnuConnectionConnect"].Enabled = false;
            //itemMenu.DropDownItems["mnuConnectionDisconnect"].Enabled = true;
            // Меню "Помощь"
            itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuHelp"]);
            itemMenu.Enabled = true;
            itemMenu.DropDownItems["mnuHelpHelp"].Enabled = true;
            itemMenu.DropDownItems["mnuHelpAbout"].Enabled = true;

            // Инициализируем панель инструментов
            button = (ToolStripButton)this.toolStripMain.Items["buttonSaveDevice"];
            button.Enabled = true;
            button = (ToolStripButton)this.toolStripMain.Items["buttonNewDevice"];
            button.Enabled = false;
            button = (ToolStripButton)this.toolStripMain.Items["buttonDeleteDevice"];
            button.Enabled = true;
            button = (ToolStripButton)this.toolStripMain.Items["buttonConnection"];

            //button.Enabled = true;
            //button.ToolTipText = "Поключить";
            //button.Image = global::NGK.MeasuringDeviceTech.Properties.Resources.IconDisconnect;

            //((ToolStripMenuItem)menuStripMain.Items["mnuConnection"]).DropDownItems["mnuConnectionConnect"].Enabled = true;
            //((ToolStripMenuItem)menuStripMain.Items["mnuConnection"]).DropDownItems["mnuConnectionDisconnect"].Enabled = false;

            if (_SerialPort.IsOpen())
            {
                button = (ToolStripButton)this.toolStripMain.Items["buttonReadDevice"];
                button.Enabled = true;
                button = (ToolStripButton)this.toolStripMain.Items["buttonWriteDevice"];
                button.Enabled = true;
                button = (ToolStripButton)this.toolStripMain.Items["buttonVerifyInitDevice"];
                button.Enabled = true;
                button = (ToolStripButton)this.toolStripMain.Items["buttonSyncDateTime"];
                button.Enabled = true;
                button = (ToolStripButton)this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonStartMonitor];
                button.Enabled = true;
                button = (ToolStripButton)this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonStopMonitor];
                button.Enabled = false;
            }
            else
            {
                button = (ToolStripButton)this.toolStripMain.Items["buttonReadDevice"];
                button.Enabled = false;
                button = (ToolStripButton)this.toolStripMain.Items["buttonWriteDevice"];
                button.Enabled = false;
                button = (ToolStripButton)this.toolStripMain.Items["buttonVerifyInitDevice"];
                button.Enabled = false;
                button = (ToolStripButton)this.toolStripMain.Items["buttonSyncDateTime"];
                button.Enabled = false;
                button = (ToolStripButton)this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonStartMonitor];
                button.Enabled = false;
                button = (ToolStripButton)this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonStopMonitor];
                button.Enabled = false;
            }

            // Переходим в дереве на устройство и обновляем данные
            if (this.treeViewMain.SelectedNode ==
                this.treeViewMain.Nodes["NodeRoot"].Nodes["NodeMeasuringDevice"])
            {
                // Это необходимо делать, т.к. если в данный момен выбран нод "NodeMeasuringDevice"
                // и создаётся устройство, то отображение данных созданного устройства в propertygrid не происходит!!! 
                this.treeViewMain.SelectedNode = this.treeViewMain.Nodes["NodeRoot"];
                this.treeViewMain.SelectedNode = this.treeViewMain.Nodes["NodeRoot"].Nodes["NodeMeasuringDevice"];
            }
            else
            {
                this.treeViewMain.SelectedNode = this.treeViewMain.Nodes["NodeRoot"].Nodes["NodeMeasuringDevice"];
            }
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Удаляет открытое устройство КИП.
        /// </summary>
        /// <returns>возвращает false - елсли операция была отменена пользователем</returns>
        private Boolean DeleteDevice()
        {
            DialogResult result;
            Boolean res = true;

            if (_MeasuringDevice != null)
            {
                result = MessageBox.Show(this,
                    "Устройство КИП будет удалено! Сохранить изменения?", "Внимание",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                switch (result)
                {
                    case System.Windows.Forms.DialogResult.No:
                        {
                            if (this.propertyGridMain.SelectedObject is IMeasuringDevice)
                            {
                                this.propertyGridMain.SelectedObject = null;
                            }

                            _MeasuringDevice = null;

                            // Закрываем файл устройсва КИП
                            if (_File != null)
                            {
                                _File.Flush();
                                _File.Close();
                                GC.Collect();
                                _File = null;

                                // Удаляем путь к файлу
                                ((ToolStripLabel)this.statusStripMain.Items["PathToFile"]).Visible = false;
                                ((ToolStripLabel)this.statusStripMain.Items["PathToFile"]).Text = String.Empty;
                            }

                            // Настраиваем элементы навигации и управления на форме
                            ToolStripMenuItem itemMenu;
                            ToolStripButton button;

                            // Главное меню:
                            // Меню "Файл"
                            itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuFile"]);
                            itemMenu.Enabled = true;
                            itemMenu.DropDownItems["mnuFileCreate"].Enabled = true;
                            itemMenu.DropDownItems["mnuFileOpen"].Enabled = true;
                            itemMenu.DropDownItems["mnuFileClose"].Enabled = false;
                            itemMenu.DropDownItems["mnuFileSave"].Enabled = false;
                            itemMenu.DropDownItems["mnuFileSaveAs"].Enabled = false;
                            itemMenu.DropDownItems[MENU.mnuFilePrint].Enabled = false;
                            itemMenu.DropDownItems[MENU.mnuFilePrintPreview].Enabled = false;
                            itemMenu.DropDownItems[MENU.mnuFilePrintSettings].Enabled = false;
                            itemMenu.DropDownItems[MENU.mnuFilePageSettings].Enabled = false;
                            itemMenu.DropDownItems["mnuFileExit"].Enabled = true;
                            // Меню "Устройство КИП"
                            itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuDevice"]);
                            itemMenu.Enabled = false;
                            itemMenu.DropDownItems["mnuDeviceRead"].Enabled = true;
                            itemMenu.DropDownItems["mnuDeviceWrite"].Enabled = true;
                            // Меню "Подключение"
                            //itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuConnection"]);
                            //itemMenu.Enabled = true;
                            //itemMenu.DropDownItems["mnuConnectionConnect"].Enabled = true;
                            //itemMenu.DropDownItems["mnuConnectionDisconnect"].Enabled = false;
                            // Меню "Помощь"
                            itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuHelp"]);
                            itemMenu.Enabled = true;
                            itemMenu.DropDownItems["mnuHelpHelp"].Enabled = true;
                            itemMenu.DropDownItems["mnuHelpAbout"].Enabled = true;

                            // Инициализируем панель инструментов
                            button = (ToolStripButton)this.toolStripMain.Items["buttonSaveDevice"];
                            button.Enabled = false;
                            button = (ToolStripButton)this.toolStripMain.Items["buttonNewDevice"];
                            button.Enabled = true;
                            button = (ToolStripButton)this.toolStripMain.Items["buttonDeleteDevice"];
                            button.Enabled = false;
                            //button = (ToolStripButton)this.toolStripMain.Items["buttonConnection"];
                            //button.Enabled = true;
                            button = (ToolStripButton)this.toolStripMain.Items["buttonVerifyInitDevice"];
                            button.Enabled = false;
                            button = (ToolStripButton)this.toolStripMain.Items["buttonReadDevice"];
                            button.Enabled = false;
                            button = (ToolStripButton)this.toolStripMain.Items["buttonWriteDevice"];
                            button.Enabled = false;
                            button = (ToolStripButton)this.toolStripMain.Items["buttonSyncDateTime"];
                            button.Enabled = false;
                            button = (ToolStripButton)this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonStartMonitor];
                            button.Enabled = false;
                            button = (ToolStripButton)this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonStopMonitor];
                            button.Enabled = false;

                            res = true;
                            break;
                        }
                    case System.Windows.Forms.DialogResult.Yes:
                        {
                            // Сохраняем устройство в файл. Если он не был открыт, 
                            // то предлагаем сохранить вновь
                            this.SaveFile();

                            // Удаляем устройство КИП
                            if (this.propertyGridMain.SelectedObject is IMeasuringDevice)
                            {
                                this.propertyGridMain.SelectedObject = null;
                            }

                            _MeasuringDevice = null;

                            // Закрываем файл устройсва КИП
                            if (_File != null)
                            {
                                _File.Flush();
                                _File.Close();
                                GC.Collect();
                                _File = null;

                                // Удаляем путь к файлу
                                ((ToolStripLabel)this.statusStripMain.Items["PathToFile"]).Visible = false;
                                ((ToolStripLabel)this.statusStripMain.Items["PathToFile"]).Text = String.Empty;
                            }

                            // Настраиваем элементы навигации и управления на форме
                            ToolStripMenuItem itemMenu;
                            ToolStripButton button;

                            // Главное меню:
                            // Меню "Файл"
                            itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuFile"]);
                            itemMenu.Enabled = true;
                            itemMenu.DropDownItems["mnuFileCreate"].Enabled = true;
                            itemMenu.DropDownItems["mnuFileOpen"].Enabled = true;
                            itemMenu.DropDownItems["mnuFileClose"].Enabled = false;
                            itemMenu.DropDownItems["mnuFileSave"].Enabled = false;
                            itemMenu.DropDownItems["mnuFileSaveAs"].Enabled = false;
                            itemMenu.DropDownItems[MENU.mnuFilePrint].Enabled = false;
                            itemMenu.DropDownItems[MENU.mnuFilePrintPreview].Enabled = false;
                            itemMenu.DropDownItems[MENU.mnuFilePrintSettings].Enabled = false;
                            itemMenu.DropDownItems[MENU.mnuFilePageSettings].Enabled = false;
                            itemMenu.DropDownItems["mnuFileExit"].Enabled = true;
                            // Меню "Устройство КИП"
                            itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuDevice"]);
                            itemMenu.Enabled = false;
                            itemMenu.DropDownItems["mnuDeviceRead"].Enabled = true;
                            itemMenu.DropDownItems["mnuDeviceWrite"].Enabled = true;
                            // Меню "Подключение"
                            //itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuConnection"]);
                            //itemMenu.Enabled = true;
                            //itemMenu.DropDownItems["mnuConnectionConnect"].Enabled = true;
                            //itemMenu.DropDownItems["mnuConnectionDisconnect"].Enabled = false;
                            // Меню "Помощь"
                            itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuHelp"]);
                            itemMenu.Enabled = true;
                            itemMenu.DropDownItems["mnuHelpHelp"].Enabled = true;
                            itemMenu.DropDownItems["mnuHelpAbout"].Enabled = true;

                            // Инициализируем панель инструментов
                            button = (ToolStripButton)this.toolStripMain.Items["buttonSaveDevice"];
                            button.Enabled = false;
                            button = (ToolStripButton)this.toolStripMain.Items["buttonNewDevice"];
                            button.Enabled = true;
                            button = (ToolStripButton)this.toolStripMain.Items["buttonDeleteDevice"];
                            button.Enabled = false;
                            //button = (ToolStripButton)this.toolStripMain.Items["buttonConnection"];
                            //button.Enabled = true;
                            button = (ToolStripButton)this.toolStripMain.Items["buttonVerifyInitDevice"];
                            button.Enabled = false;
                            button = (ToolStripButton)this.toolStripMain.Items["buttonReadDevice"];
                            button.Enabled = false;
                            button = (ToolStripButton)this.toolStripMain.Items["buttonWriteDevice"];
                            button.Enabled = false;
                            button = (ToolStripButton)this.toolStripMain.Items["buttonSyncDateTime"];
                            button.Enabled = false;
                            button = (ToolStripButton)this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonStartMonitor];
                            button.Enabled = false;
                            button = (ToolStripButton)this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonStopMonitor];
                            button.Enabled = false;

                            res = true;
                            break;
                        }
                    case System.Windows.Forms.DialogResult.Cancel:
                        {
                            // Действие отменено.
                            res = false;
                            break;
                        }
                }
            }
            else
            {
                // Устройство не существует, закрыть не можем
                res = true;
            }
            return res;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Метод создаёт подключение к сети Modbus. Создаётся один раз при запуске программы
        /// </summary>
        /// <param name="name">Последовательный порт</param>
        /// <param name="timeout">Таймаут ответа на запрос, мсек</param>
        /// <param name="arounddelay">Задержка при широковещательных командах</param>
        private Modbus.OSIModel.DataLinkLayer.Master.RTU.SerialPort.ComPort CreateConnection(
            String name, 
            int bautRate, 
            int dataBits, 
            System.IO.Ports.Parity parity, 
            System.IO.Ports.StopBits stopBits, 
            int timeout, 
            int arounddelay)
        {
            Modbus.OSIModel.DataLinkLayer.Master.RTU.SerialPort.ComPort comport;
            try
            {

                comport = new ComPort(name,
                    bautRate, parity, dataBits, stopBits,
                    timeout, arounddelay, false, Modbus.OSIModel.DataLinkLayer.Diagnostics.TypeOfMessageLog.Error |
                Modbus.OSIModel.DataLinkLayer.Diagnostics.TypeOfMessageLog.Information | 
                Modbus.OSIModel.DataLinkLayer.Diagnostics.TypeOfMessageLog.Warning, String.Empty);

                //_SerialPort.OpenConnect();
            }
            catch
            {
                //_SerialPort = null;
                //_Host = null;
                //MessageBox.Show(this, 
                //    "Неудалось создать подклчючение", "Ошибка", 
                //    MessageBoxButtons.OK, MessageBoxIcon.Error);
                comport = null;
            }

            return comport;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Метод устанавливаем соединение. Открывает порт для работы с устройством
        /// </summary>
        private void Connect()
        {
            if (this._SerialPort != null)
            {
                try
                {
                    _SerialPort.OpenConnect();
                    // При каждом удачном открытии соединения сохраняем его настройки
                    SavePortSettings();
                }
                catch
                {
                    MessageBox.Show(this,
                        "Неудалось открыть соединение",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Настраиваем элементы навигации, управления и меню
                ToolStripMenuItem itemMenu;
                ToolStripButton button;

                // Главное меню:
                // Меню "Файл"
                itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuFile"]);
                itemMenu.Enabled = true;
                if (_MeasuringDevice != null)
                {
                    itemMenu.DropDownItems["mnuFileCreate"].Enabled = false;
                    itemMenu.DropDownItems["mnuFileOpen"].Enabled = true;
                    itemMenu.DropDownItems["mnuFileClose"].Enabled = true;
                    itemMenu.DropDownItems["mnuFileSave"].Enabled = true;
                    itemMenu.DropDownItems["mnuFileSaveAs"].Enabled = true;
                }
                else
                {
                    itemMenu.DropDownItems["mnuFileCreate"].Enabled = true;
                    itemMenu.DropDownItems["mnuFileOpen"].Enabled = true;
                    itemMenu.DropDownItems["mnuFileClose"].Enabled = false;
                    itemMenu.DropDownItems["mnuFileSave"].Enabled = false;
                    itemMenu.DropDownItems["mnuFileSaveAs"].Enabled = false;
                }
                itemMenu.DropDownItems["mnuFileExit"].Enabled = true;
                
                // Меню "Устройство КИП"
                if (_MeasuringDevice != null)
                {
                    itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuDevice"]);
                    itemMenu.Enabled = true;
                    itemMenu.DropDownItems["mnuDeviceRead"].Enabled = true;
                    itemMenu.DropDownItems["mnuDeviceWrite"].Enabled = true;
                }
                else
                {
                    itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuDevice"]);
                    itemMenu.Enabled = false;
                    itemMenu.DropDownItems["mnuDeviceRead"].Enabled = true;
                    itemMenu.DropDownItems["mnuDeviceWrite"].Enabled = true;
                }
                // Меню "Подключение"
                itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuConnection"]);
                itemMenu.Enabled = true;
                itemMenu.DropDownItems["mnuConnectionConnect"].Enabled = false;
                itemMenu.DropDownItems["mnuConnectionDisconnect"].Enabled = true;
                // Меню "Помощь"
                itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuHelp"]);
                itemMenu.Enabled = true;
                itemMenu.DropDownItems["mnuHelpHelp"].Enabled = true;
                itemMenu.DropDownItems["mnuHelpAbout"].Enabled = true;

                // Инициализируем панель инструментов
                button = (ToolStripButton)this.toolStripMain.Items["buttonReadTypeOfDevice"];
                button.Enabled = true;

                if (_MeasuringDevice != null)
                {
                    button = (ToolStripButton)this.toolStripMain.Items["buttonSaveDevice"];
                    button.Enabled = true;
                    button = (ToolStripButton)this.toolStripMain.Items["buttonNewDevice"];
                    button.Enabled = false;
                    button = (ToolStripButton)this.toolStripMain.Items["buttonDeleteDevice"];
                    button.Enabled = true;
                }
                else
                {
                    button = (ToolStripButton)this.toolStripMain.Items["buttonSaveDevice"];
                    button.Enabled = false;
                    button = (ToolStripButton)this.toolStripMain.Items["buttonNewDevice"];
                    button.Enabled = true;
                    button = (ToolStripButton)this.toolStripMain.Items["buttonDeleteDevice"];
                    button.Enabled = false;
                }

                button = (ToolStripButton)this.toolStripMain.Items["buttonConnection"];
                button.Enabled = true;
                button.ToolTipText = "Отключить";
                button.Image = global::NGK.MeasuringDeviceTech.Properties.Resources.IconConnect;

                ((ToolStripMenuItem)menuStripMain.Items["mnuConnection"]).DropDownItems["mnuConnectionConnect"].Enabled = false;
                ((ToolStripMenuItem)menuStripMain.Items["mnuConnection"]).DropDownItems["mnuConnectionDisconnect"].Enabled = true;

                if (_MeasuringDevice != null)
                {
                    button = (ToolStripButton)this.toolStripMain.Items["buttonReadDevice"];
                    button.Enabled = true;
                    button = (ToolStripButton)this.toolStripMain.Items["buttonWriteDevice"];
                    button.Enabled = true;
                    button = (ToolStripButton)this.toolStripMain.Items["buttonVerifyInitDevice"];
                    button.Enabled = true;
                    button = (ToolStripButton)this.toolStripMain.Items["buttonSyncDateTime"];
                    button.Enabled = true;
                    button = (ToolStripButton)this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonStartMonitor];
                    button.Enabled = true;
                    button = (ToolStripButton)this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonStopMonitor];
                    button.Enabled = false;
                }
                else
                {
                    button = (ToolStripButton)this.toolStripMain.Items["buttonReadDevice"];
                    button.Enabled = false;
                    button = (ToolStripButton)this.toolStripMain.Items["buttonWriteDevice"];
                    button.Enabled = false;
                    button = (ToolStripButton)this.toolStripMain.Items["buttonVerifyInitDevice"];
                    button.Enabled = false;
                    button = (ToolStripButton)this.toolStripMain.Items["buttonSyncDateTime"];
                    button.Enabled = false;
                    button = (ToolStripButton)this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonStartMonitor];
                    button.Enabled = false;
                    button = (ToolStripButton)this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonStopMonitor];
                    button.Enabled = false;
                }
            }
            return;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Метод удаляет соединение с сетью Modbus
        /// </summary>
        private void Disconnect()
        {
            if (this._SerialPort != null)
            {
                try
                {
                    _SerialPort.CloseConnect();
                }
                catch
                {
                    MessageBox.Show(this,
                        "Неудалось закрыть соединение",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                // Настраиваем элементы навигации, управления и меню
                ToolStripMenuItem itemMenu;
                ToolStripButton button;

                // Главное меню:
                // Меню "Файл"
                itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuFile"]);
                itemMenu.Enabled = true;
                if (_MeasuringDevice != null)
                {
                    itemMenu.DropDownItems["mnuFileCreate"].Enabled = false;
                    itemMenu.DropDownItems["mnuFileOpen"].Enabled = true;
                    itemMenu.DropDownItems["mnuFileClose"].Enabled = true;
                    itemMenu.DropDownItems["mnuFileSave"].Enabled = true;
                    itemMenu.DropDownItems["mnuFileSaveAs"].Enabled = true;
                }
                else
                {
                    itemMenu.DropDownItems["mnuFileCreate"].Enabled = true;
                    itemMenu.DropDownItems["mnuFileOpen"].Enabled = true;
                    itemMenu.DropDownItems["mnuFileClose"].Enabled = false;
                    itemMenu.DropDownItems["mnuFileSave"].Enabled = false;
                    itemMenu.DropDownItems["mnuFileSaveAs"].Enabled = false;
                }
                itemMenu.DropDownItems["mnuFileExit"].Enabled = true;
                // Меню "Устройство КИП"
                itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuDevice"]);
                itemMenu.Enabled = false;
                itemMenu.DropDownItems["mnuDeviceRead"].Enabled = true;
                itemMenu.DropDownItems["mnuDeviceWrite"].Enabled = true;
                // Меню "Подключение"
                itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuConnection"]);
                itemMenu.Enabled = true;
                itemMenu.DropDownItems["mnuConnectionConnect"].Enabled = false;
                itemMenu.DropDownItems["mnuConnectionDisconnect"].Enabled = true;
                // Меню "Помощь"
                itemMenu = ((ToolStripMenuItem)this.menuStripMain.Items["mnuHelp"]);
                itemMenu.Enabled = true;
                itemMenu.DropDownItems["mnuHelpHelp"].Enabled = true;
                itemMenu.DropDownItems["mnuHelpAbout"].Enabled = true;

                // Инициализируем панель инструментов
                button = (ToolStripButton)this.toolStripMain.Items["buttonReadTypeOfDevice"];
                button.Enabled = false;

                if (_MeasuringDevice != null)
                {
                    button = (ToolStripButton)this.toolStripMain.Items["buttonSaveDevice"];
                    button.Enabled = true;
                    button = (ToolStripButton)this.toolStripMain.Items["buttonNewDevice"];
                    button.Enabled = false;
                    button = (ToolStripButton)this.toolStripMain.Items["buttonDeleteDevice"];
                    button.Enabled = true;
                }
                else
                {
                    button = (ToolStripButton)this.toolStripMain.Items["buttonSaveDevice"];
                    button.Enabled = false;
                    button = (ToolStripButton)this.toolStripMain.Items["buttonNewDevice"];
                    button.Enabled = true;
                    button = (ToolStripButton)this.toolStripMain.Items["buttonDeleteDevice"];
                    button.Enabled = false;                
                }

                button = (ToolStripButton)this.toolStripMain.Items["buttonConnection"];
                button.Enabled = true;
                button.ToolTipText = "Поключить";
                button.Image = global::NGK.MeasuringDeviceTech.Properties.Resources.IconDisconnect;

                ((ToolStripMenuItem)menuStripMain.Items["mnuConnection"]).DropDownItems["mnuConnectionConnect"].Enabled = true;
                ((ToolStripMenuItem)menuStripMain.Items["mnuConnection"]).DropDownItems["mnuConnectionDisconnect"].Enabled = false;
                
                button = (ToolStripButton)this.toolStripMain.Items["buttonReadDevice"];
                button.Enabled = false;
                button = (ToolStripButton)this.toolStripMain.Items["buttonWriteDevice"];
                button.Enabled = false;
                button = (ToolStripButton)this.toolStripMain.Items["buttonVerifyInitDevice"];
                button.Enabled = false;
                button = (ToolStripButton)this.toolStripMain.Items["buttonSyncDateTime"];
                button.Enabled = false;
                button = (ToolStripButton)this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonStartMonitor];
                button.Enabled = false;
                button = (ToolStripButton)this.toolStripMain.Items[TOOLSTRIPBUTTON.buttonStopMonitor];
                button.Enabled = false;
            }
            return;
        }
        //----------------------------------------------------------------------------
        #endregion
        //----------------------------------------------------------------------------
    }
    //================================================================================
}
//====================================================================================
// End Of File
