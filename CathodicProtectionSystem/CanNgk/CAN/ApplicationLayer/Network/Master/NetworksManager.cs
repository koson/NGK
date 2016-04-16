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
    public sealed class NgkCanNetworksManager : INetworksManager
    {
        #region Fields And Properties

        private static NgkCanNetworksManager _Instance;
        
        /// <summary>
        /// Возвращает менеджер сетей.
        /// </summary>
        public static NgkCanNetworksManager Instance
        {
            get 
            {
                if (_Instance == null)
                {
                    lock (_SyncLock)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new NgkCanNetworksManager();
                        }
                    }
                }
                return NgkCanNetworksManager._Instance; 
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

        [Browsable(true)]
        [ReadOnly(true)]
        [DisplayName("Неисправных устройств")]
        [Description("Общее количество неисправных устройств в системе ")]
        [Category("Система")]
        public int FaultyDevices
        {
            get 
            {
                int mount = 0;
 
                foreach (NetworkController network in _NetworksList)
                {
                    foreach (IDevice device in network.Devices)
                    {
                        if ((device.Status == DeviceStatus.CommunicationError) ||
                            (device.Status == DeviceStatus.ConfigurationError))
                        {
                            mount++;
                        }
                    }
                }
                return mount;
            }
        }

        #endregion

        #region Constructors

        private NgkCanNetworksManager()
        {
            _NetworksList = new NetworkControllersCollection();
        }

        #endregion
        
        #region Methods

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

                foreach (NetworkController network in _NetworksList)
                {
                    network.ControllerChangedStatus += new EventHandler(EventHandler_ControllerChangedStatus);
                }
            }
            return;
        }

        void EventHandler_ControllerChangedStatus(object sender, EventArgs e)
        {
            INetworkController network = (INetworkController)sender;
            OnNetworkChangedStatus(new NetworkChangedStatusEventAgrs(network));
        }

        void OnNetworkChangedStatus(NetworkChangedStatusEventAgrs args)
        {
            if (NetworkChangedStatus != null)
            {
                NetworkChangedStatus(this, args);
            }
        }

        #endregion

        #region Events

        public event EventHandler<NetworkChangedStatusEventAgrs> NetworkChangedStatus;

        #endregion
    }
}
