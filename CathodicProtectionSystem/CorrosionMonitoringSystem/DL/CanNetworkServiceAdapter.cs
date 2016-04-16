using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Master;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary;
using Common.Collections.ObjectModel;
using NGK.CorrosionMonitoringSystem.Models;

namespace NGK.CorrosionMonitoringSystem.BL
{
    /// <summary>
    /// Класс для работы с сервером сетей CAN НГК.
    /// Получает данные от менедржера сетий и вносит их в соответствующие таблицы
    /// </summary>
    public class CanNetworkServiceAdapter
    {
        #region Fields And Properties
        /// <summary>
        /// Список устройств
        /// </summary>
        List<NgkCanDevice> _Devices;
        /// <summary>
        /// Список устройств
        /// </summary>
        public List<NgkCanDevice> Devices
        {
            get { return _Devices; }
        }
        /// <summary>
        /// Словарь объектов устройства
        /// </summary>
        //DataTable _ObjectDictionaryTable;
        /// <summary>
        /// Сводная таблица
        /// </summary>
        //DataTable _PivoteTable;
        private EventHandler _ControllerChangedStatus;
        private EventHandler _DeviceChangedStatus;
        private EventHandler<KeyedCollectionWasChangedEventArgs<DeviceBase>> _DevicesAmountWasChanged;
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public CanNetworkServiceAdapter()
        {
            NgkCanNetworksManager manager = NgkCanNetworksManager.Instance;

            _ControllerChangedStatus = 
                new EventHandler(EventHandlerControllerChangedStatus);
            _DeviceChangedStatus = 
                new EventHandler(EventHandlerDeviceChangedStatus);
            _DevicesAmountWasChanged =
                new EventHandler<KeyedCollectionWasChangedEventArgs<DeviceBase>>(
                EventHandlerDevicesAmountWasChanged);

            _Devices = new List<NgkCanDevice>();
            // Собираем все устройства из всех сетей            
            foreach (NetworkController network in manager.Networks)
            {
                network.ControllerChangedStatus += _ControllerChangedStatus;
                network.Devices.CollectionWasChanged += _DevicesAmountWasChanged;
                
                foreach (DeviceBase device in network.Devices)
                {
                    device.DeviceChangedStatus += _DeviceChangedStatus;
                    // TODO:
                    //device.DataWasChanged += new EventHandler(device_DataWasChanged);
                    _Devices.Add(new NgkCanDevice(device));
                }
            }
        }
        #endregion
        
        #region Methods
        /// <summary>
        /// Обработчик события изменения состояния сети
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void EventHandlerControllerChangedStatus(
            object sender, EventArgs args)
        {
            //NetworkController controller = (NetworkController)sender;
            return;
        }
        /// <summary>
        /// Обработчик события изменения состояния устройства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandlerDeviceChangedStatus(
            object sender, EventArgs e)
        {
            DeviceBase device = (DeviceBase)sender;
            // Устанавливаем статус модели

            foreach (NgkCanDevice item in _Devices)
            {
                if ((item.NetworkId == device.Network.NetworkId) &&
                    (item.NodeId == device.NodeId))
                {
                    item.Status = device.Status;
                }
            }
        }
        /// <summary>
        /// Обработчик события изменения кол-ва устройств
        /// в сети
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandlerDevicesAmountWasChanged(
            object sender, KeyedCollectionWasChangedEventArgs<DeviceBase> e)
        {
            return;
        }
        /// <summary>
        /// Обновляет поля модели из сетевого сервиса
        /// </summary>
        /// <param name="device"></param>
        public void UpdateDevice(NgkCanDevice device)
        {
            DeviceBase dvc;

            dvc = NgkCanNetworksManager.Instance.Networks[device.NetworkId]
                .Devices[device.NodeId];
            NgkCanDevice.Update(device, dvc);
        }
        /// <summary>
        /// Обновляет значения объектного словаря устройства
        /// </summary>
        /// <param name="device"></param>
        public void UpdateObjectDictionary(NgkCanDevice device)
        {
            DeviceBase dvc;

            dvc = NgkCanNetworksManager.Instance.Networks[device.NetworkId]
                .Devices[device.NodeId];
            // Обновляем словарь объектов модели
            NgkCanDevice.Update(device, dvc);
        }

        #endregion
    }
}
