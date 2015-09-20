using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Modbus.OSIModel.DataLinkLayer.Master.RTU.SerialPort.UITypeEditor
{
    /// <summary>
    /// Делегат для создания события окончания редактирования свойства классах редакторов типов
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void EditingIsCompleteEventHandler(object sender, EventArgs args);

    public partial class SerialPortSettings : UserControl
    {
        #region Fields and Properties
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Последовательный порт для редактирования его свойств
        /// </summary>
        private System.IO.Ports.SerialPort _SerialPort;
        //-------------------------------------------------------------------------------
        private String _PortName;
        //-------------------------------------------------------------------------------
        private int _BautRate;
        //-------------------------------------------------------------------------------
        private int _DataBits;
        //-------------------------------------------------------------------------------
        private System.IO.Ports.StopBits _StopBits;
        //-------------------------------------------------------------------------------
        private System.IO.Ports.Parity _Parity;
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Последовательный порт для редактирования
        /// </summary>
        public System.IO.Ports.SerialPort SerialPort
        {
            get { return _SerialPort; }
            set { _SerialPort = value; }
        }
        //-------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        public SerialPortSettings()
        {
            InitializeComponent();
        }
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="serialPort">Последовательный порт</param>
        public SerialPortSettings(System.IO.Ports.SerialPort serialPort)
        {
            InitializeComponent();

            _SerialPort = serialPort;
        }
        //-------------------------------------------------------------------------------
        #endregion

        #region Methods
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события загрузки формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPortSettings_Load(
            object sender, 
            EventArgs e)
        {
            // Инициализируем элементы управления формы
            // Загружаем скорости передачи данных
            int[] br = new int[] {110, 300, 600, 1200, 2400, 
                    4800, 9600, 14400, 19200, 38400, 56000, 
                    57600, 115200, 128000, 256000 };
            comboBoxBautRate.DataSource = br;

            // Загружаем список размерности бит данных символа
            int[] dt = new int[] { 7, 8 };
            comboBoxDataBits.DataSource = dt;

            // Загружаем список паритетов
            comboBoxParity.DataSource = Enum.GetNames(typeof(System.IO.Ports.Parity));
            // Загружаем список стоп бит
            comboBoxStopBits.DataSource = Enum.GetNames(typeof(System.IO.Ports.StopBits));

            // Загружаем список портов в системе;
            String[] portNames = System.IO.Ports.SerialPort.GetPortNames();

            // Выводим их пользователю
            comboBoxPortName.DataSource = portNames;

            if (_SerialPort != null)
            {
                // Если порт открыт запрещаем редактирование его свойств


                // Находим в списке порт и устанавливаем его в выподающем списке
                for (int i = 0; i < comboBoxPortName.Items.Count; i++)
                {
                    if (_SerialPort.PortName == (String)comboBoxPortName.Items[i])
                    {
                        comboBoxPortName.SelectedIndex = i;
                    }
                }
            }
            else
            {
                // Создаём новый порт
                _SerialPort = new System.IO.Ports.SerialPort(
                    (String)comboBoxPortName.SelectedItem, 19200, System.IO.Ports.Parity.Even, 
                    8, System.IO.Ports.StopBits.One);

            }
            // Заполняем настройками порта элементы управления формы
            for (int i = 0; i < comboBoxBautRate.Items.Count; i++)
            {
                if (_SerialPort.BaudRate == (int)comboBoxBautRate.Items[i])
                {
                    comboBoxBautRate.SelectedIndex = i;
                }
            }

            for (int i = 0; i < comboBoxDataBits.Items.Count; i++)
            {
                if (_SerialPort.DataBits == (int)comboBoxDataBits.Items[i])
                {
                    comboBoxDataBits.SelectedIndex = i;
                }
            }

            for (int i = 0; i < comboBoxParity.Items.Count; i++)
            {
                if (_SerialPort.Parity == 
                    (System.IO.Ports.Parity)Enum.Parse(typeof(System.IO.Ports.Parity), 
                    (String)comboBoxParity.Items[i]))
                {
                    comboBoxParity.SelectedIndex = i;
                }
            }

            for (int i = 0; i < comboBoxStopBits.Items.Count; i++)
            {
                if (_SerialPort.StopBits == 
                    (System.IO.Ports.StopBits)Enum.Parse(typeof(System.IO.Ports.StopBits),
                    (String)comboBoxStopBits.Items[i]))
                {
                    comboBoxStopBits.SelectedIndex = i;
                }
            }
            return;
        }
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Обработчик выбор нового имени порта в выподающем списке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxPortName_SelectedIndexChanged(
            object sender, 
            EventArgs e)
        {
            // Получаем имя порта и обновляем его в нужном свойстве
            _PortName = (String)((ComboBox)sender).SelectedItem;
            return;
        }
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события выбора скорости передачи данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxBautRate_SelectedIndexChanged(
            object sender, 
            EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            _BautRate = (int)cb.SelectedItem;
            return;
        }
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события выбора количества бит данных 
        /// в передаваемом симоле в выподающем списке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxDataBits_SelectedIndexChanged(
            object sender, 
            EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            _DataBits = (int)cb.SelectedItem;
            return;
        }
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события выбора паритета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxParity_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            _Parity = (System.IO.Ports.Parity)Enum.Parse(typeof(System.IO.Ports.Parity), 
                (String)cb.SelectedItem);
            return;
        }
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события выбора количества стоп-бит
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxStopBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            _StopBits = (System.IO.Ports.StopBits)Enum.Parse(typeof(System.IO.Ports.StopBits),
                (String)cb.SelectedItem);
            return;
        }
        //-------------------------------------------------------------------------------
        /// <summary>
        /// ОБработчик события нажатия клавиш
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPortSettings_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    {
                        // По нажатию кнопки закрываем форму и устанавливаем результат
                        OnEditingIsComplete(new EventArgs());
                        break;
                    }
                case Keys.Enter:
                    {
                        // По нажатию кнопки закрываем форму и устанавливаем результат
                        OnEditingIsComplete(new EventArgs());
                        break;
                    }
            }
        }
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события нажатия на кнопку "ОК"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            // По нажатию кнопки закрываем форму и устанавливаем результат
            _SerialPort.PortName = _PortName;
            _SerialPort.BaudRate = _BautRate;
            _SerialPort.DataBits = _DataBits;
            _SerialPort.StopBits = _StopBits;
            _SerialPort.Parity = _Parity;

            OnEditingIsComplete(new EventArgs());
            return;
        }
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Метод генерирует событие завершение редактирования настроект порта
        /// </summary>
        /// <param name="args"></param>
        private void OnEditingIsComplete(EventArgs args)
        {
            if (EditingIsComplete != null)
            {
                EditingIsComplete(this, args);
            }
            return;
        }
        //-------------------------------------------------------------------------------
        #endregion

        #region Events
        //-------------------------------------------------------------------------------
        /// <summary>
        /// Событие возникает при окончании редактирования настроек порта
        /// </summary>
        public event EditingIsCompleteEventHandler EditingIsComplete;
        //-------------------------------------------------------------------------------
        #endregion
    }
}