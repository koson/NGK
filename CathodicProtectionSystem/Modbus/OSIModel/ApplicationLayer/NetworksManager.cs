using System;
using System.Collections.Generic;
using System.Text;
using Modbus.OSIModel.ApplicationLayer.Slave;

namespace Modbus.OSIModel.ApplicationLayer
{
    /// <summary>
    /// Класс для управления сетями Modbus
    /// </summary>
    public class NetworksManager
    {
        #region Fields And Properties
        /// <summary>
        /// Singleton
        /// </summary>
        private static NetworksManager _Instance;
        /// <summary>
        /// Возвращает объект менеджера сетей
        /// </summary>
        public static NetworksManager Instance
        {
            get 
            {
                if (_Instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new NetworksManager();
                        }
                    }
                }
                return _Instance;
            }
        }
        /// <summary>
        /// Список сетей 
        /// </summary>
        private NetworksCollection _Networks;
        /// <summary>
        /// Сети Modbus
        /// </summary>
        public NetworksCollection Networks
        {
            get { return _Networks; }
        }
        /// <summary>
        /// Объект для синхронизации
        /// </summary>
        private static object SyncRoot = new object();
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        private NetworksManager()
        {
            _Networks = new NetworksCollection();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Загружает конфигурацию сетей из файла 
        /// </summary>
        /// <param name="path">Путь к файлу конфигурации</param>
        public void LoadConfig(string path)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Сохраняет текущую конфигурацию в файл
        /// </summary>
        /// <param name="path">Путь к файлу конфигурации</param>
        public void SaveConfig(string path)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Создаёт новую сеть
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static NetworkController Create(WorkMode type)
        {
            //NetworkController controller;

            switch (type)
            {
                case WorkMode.Master:
                    { throw new NotImplementedException(); }
                case WorkMode.Slave:
                    {
                        throw new NotImplementedException();
                        //controller = new NetworkController(
                        //break;
                    }
                default:
                    { throw new NotImplementedException(); }
            }
        }
        #endregion
    }
}
