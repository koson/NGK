using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.ComponentModel;
//using System.Collections.UsersGeneric;
using NGK.CAN.ApplicationLayer.Network.Master.Collections;
using NGK.CAN.ApplicationLayer.Network.Master;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles;
using NGK.CAN.DataLinkLayer.CanPort;
using NGK.CAN.DataLinkLayer.CanPort.IXXAT;
using NGK.CAN.DataLinkLayer.Message;

namespace NGK.CAN.ApplicationLayer.Network.Master
{
    /// <summary>
    /// Класс управляет сетями CAN НГК-ЭХЗ
    /// </summary>
    /// <remarks>Реализует паттерн Singlton</remarks>
    [Serializable]
    public sealed class NetworksManager : INetworksManager
    {
        #region Fields And Properties

        private static NetworksManager _Instance;
        
        /// <summary>
        /// Возвращает менеджер сетей.
        /// </summary>
        public static NetworksManager Instance
        {
            get 
            {
                if (_Instance == null)
                {
                    lock (_SyncLock)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new NetworksManager();
                        }
                    }
                }
                return NetworksManager._Instance; 
            }
        }
        
        /// <summary>
        /// Объект для синхронизации доступа к ресурсам
        /// </summary>
        private static Object _SyncLock = new object();
        
        /// <summary>
        /// Список сетей CAN НГК-ЭХЗ
        /// </summary>
        private NetworkControllersCollection _NetworksList;

        /// <summary>
        /// Сети CAN НГК-ЭХЗ
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [DisplayName("Сети CAN НГК-ЭХЗ")]
        [Description("Список сетей CAN НГК-ЭХЗ")]
        [Category("Сетевые настройки")]
        public NetworkControllersCollection Networks
        {
            get { return _NetworksList; }
        }
        [Browsable(true)]
        [ReadOnly(true)]
        [DisplayName("Общее количество устройств")]
        [Description("Общее количество устройств в системе ")]
        [Category("Система")]
        public int TotalDevices
        {
            get 
            {
                int total = 0;

                foreach (NetworkController network in _NetworksList)
                {
                    total += network.Devices.Count;
                }
                return total;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        private NetworksManager()
        {
            //this._NetworksList = new ListWithEvents<NetworkController>();
            //// Подключаем события для ферификации добавляемых сетей в список.
            //this._NetworksList.ItemIsAdding += 
            //    new GenericCancelEventHandler<NetworkController>(EventHandler_NetworksList_ItemIsAdding);
            //this._NetworksList.ItemIsReplacing += 
            //    new GenericCancelEventHandler<NetworkController>(_NetworksList_ItemIsReplacing);

            this._NetworksList = new NetworkControllersCollection();
        }
        #endregion
        
        #region Methods
        /// <summary>
        /// Обработчик события добавления сети в список сетей.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        //private void EventHandler_NetworksList_ItemIsAdding(object sender, 
        //    GenericCancelEventArgs<NetworkController> args)
        //{
        //    // Проверяем существует ли добавляемая сеть в списке (проверка по ссылке).
        //    if (-1 != this._NetworksList.IndexOf(args.Item))
        //    {
        //        // Данный объект уже содержится в списке. Отменяем добавление/замену
        //        args.Cancel = true;
        //        return;
        //    }
            
        //    foreach (NetworkController controller in this._NetworksList)
        //    {
        //        // Проверяем существует ли добавляемая
        //        // сеть в списке (проверка по имени сети)
        //        if (controller.NetworkName == args.Item.NetworkName)
        //        {
        //            // Сеть с данным именем уже существует. Запрещаем добавление
        //            // сети в список
        //            args.Cancel = true;
        //            return;
        //        }
        //        // Проверяем существует ли добавляемая сеть  
        //        // в списке (проверка по типу и имени CAN порта)
        //        if (args.Item.CanPort != null)
        //        {
        //            if (controller.CanPort.Equals(args.Item.CanPort))
        //            {
        //                // Сеть с данным CAN-портом уже существует. Запрещаем добавление
        //                args.Cancel = true;
        //                return;
        //            }
        //        }
        //        else
        //        {
        //            args.Cancel = false;
        //            return;
        //        }
        //    }
        //    // Если всё нормально, разрешаем добавление сети в список
        //    args.Cancel = false;
        //    return;
        //}
        /// <summary>
        /// Обработчик события замены сети в списке сетей. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        //private void _NetworksList_ItemIsReplacing(object sender, 
        //    GenericCancelEventArgs<NetworkController> args)
        //{
        //    // Проверяем существует ли заменяемая сеть в списке (проверка по ссылке).
        //    if (-1 != this._NetworksList.IndexOf(args.Item))
        //    {
        //        // Данный объект уже содержится в списке. Отменяем замену сети в списке
        //        args.Cancel = true;
        //        return;
        //    }

        //    foreach (NetworkController controller in this._NetworksList)
        //    {
        //        // Проверяем существует ли заменяемая
        //        // сеть в списке (проверка по имени сети)
        //        if (controller.NetworkName == args.Item.NetworkName)
        //        {
        //            // Сеть с данным именем уже существует. Запрещаем замену сети в списке
        //            args.Cancel = true;
        //            return;
        //        }
        //    }
        //    // Если всё нормально, разрешаем замену сети в списке
        //    args.Cancel = false;
        //    return;
        //}
        /// <summary>
        /// Метод сохраняет конфигурацию сетей в указанный файл
        /// </summary>
        /// <param name="pathToFile">
        /// Путь к файлу + имя файл. Расширение файла должно быть .bin</param>
        public void SaveConfig(String pathToFile)
        {
            using (FileStream fs =
                new FileStream(pathToFile, FileMode.Create,
                FileAccess.ReadWrite, FileShare.None))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, _NetworksList);
            }
            return;
        }
        /// <summary>
        /// Загружает конфигурацию сетей в менеджер сетей из файла
        /// расположенного по указанному пути
        /// </summary>
        /// <param name="pathToFile">Путь к файлу конфигурации, включая имя файла</param>
        public void LoadConfig(String pathToFile)
        {
            using (FileStream fs = new FileStream(pathToFile, 
                FileMode.Open, FileAccess.Read, FileShare.None))
            {       
                BinaryFormatter bf = new BinaryFormatter();
                _NetworksList = (NetworkControllersCollection)bf.Deserialize(fs);
            }

            // Код для отладки
            //CanPort port = new CanPort("HW318371");
            //port.BitRate = BaudRate.BR10;
            //port.FrameFormat = FrameFormat.StandardFrame;
            //port.Mode = PortMode.NORMAL;

            //NetworkController controller = new NetworkController(port, "NetworkTest");
            //Device device;
            //device = Device.Create(DeviceType.KIP_MAIN_POWERED_v1);
            //controller.Devices.Add(device);

            //Networks.Add(controller);

            return;
        }
        #endregion
    }
}
