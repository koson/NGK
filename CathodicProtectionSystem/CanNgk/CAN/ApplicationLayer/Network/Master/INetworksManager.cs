using System;
using NGK.CAN.ApplicationLayer.Network.Master.Collections;

namespace NGK.CAN.ApplicationLayer.Network.Master
{
    public interface INetworksManager
    {
        /// <summary>
        /// Список сетей в системе
        /// </summary>
        NetworkControllersCollection Networks { get; }

        /// <summary>
        /// Общее количество устройтсв в системе
        /// </summary>
        int TotalDevices { get; }

        /// <summary>
        /// Сохраняет конфигурацию в файл
        /// </summary>
        /// <param name="pathToFile"></param>
        void SaveConfig(string pathToFile);
        
        /// <summary>
        /// Загружает конфигурацию сети из файла
        /// </summary>
        /// <param name="pathToFile"></param>
        void LoadConfig(string pathToFile);
    }
}
