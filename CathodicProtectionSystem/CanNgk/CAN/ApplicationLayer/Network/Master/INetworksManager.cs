﻿using System;
using NGK.CAN.ApplicationLayer.Network.Master.Collections;

namespace NGK.CAN.ApplicationLayer.Network.Master
{
    public class NetworkChangedStatusEventAgrs : EventArgs
    {
        public NetworkChangedStatusEventAgrs(INetworkController network)
        {
            _Network = network;
        }

        INetworkController _Network;

        public INetworkController Network
        {
            get { return _Network; }
        }
    }

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
        /// Неисправных устройств в системе
        /// </summary>
        int FaultyDevices { get; }

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

        event EventHandler<NetworkChangedStatusEventAgrs> NetworkChangedStatus;
    }
}