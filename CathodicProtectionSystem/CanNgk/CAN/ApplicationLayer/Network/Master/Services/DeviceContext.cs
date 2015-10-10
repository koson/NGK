using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataLinkLayer.Message;
using NGK.CAN.ApplicationLayer.Transactions;
using NGK.CAN.ApplicationLayer.Network.Master.Services;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary;

namespace NGK.CAN.ApplicationLayer.Network.Master.Services
{
    /// <summary>
    /// Класс для организации списка сетевых устройств, для которых реализуется
    /// циклический опрос параметров объектного словаря сетевыми сервисами. 
    /// Список устройств предоставляет контроллер сети
    /// </summary>
    public class DeviceContext
    {
        #region Fields And Propertie
        /// <summary>
        /// Текущая транзакция
        /// </summary>
        private Transaction _CurrentTransaction;
        /// <summary>
        /// Текущая транзакция
        /// </summary>
        public Transaction CurrentTransaction
        {
            get { return _CurrentTransaction; }
            set 
            { 
                _CurrentTransaction = value;
                // Обнуляем счётчик ошибок
                _ErrorCounter = 0;
                if (value != null)
                {
                    // Подключаем событие окончание транзакции, если она ожидает ответ
                    _CurrentTransaction.TransactionWasEnded +=
                        new EventHandler(EventHandler_CurrentTransaction_TransactionWasEnded);
                }
            }
        }
        /// <summary>
        /// Текущий объект объекта в словаре 
        /// </summary>
        private LinkedListNode<ObjectInfo> _CurrentObject;
        /// <summary>
        /// Текцщий объект словаря объектов
        /// </summary>
        public ObjectInfo CurrentObject
        {
            get { return _CurrentObject == null ? null : _CurrentObject.Value; }
        }
        /// <summary>
        /// 
        /// </summary>
        private DeviceBase _Device;
        /// <summary>
        /// 
        /// </summary>
        public DeviceBase Device 
        { 
            get { return _Device; } 
        }
        /// <summary>
        /// 
        /// </summary>
        private LinkedList<ObjectInfo> _Objects;
        /// <summary>
        /// Счётчик числа неудачных попыток доступа к устройству 
        /// </summary>
        private int _ErrorCounter;
        /// <summary>
        /// Количество неудачных попыток доступа к устройству подряд.
        /// При следующей удачной попытке обнуляется 
        /// </summary>
        public int ErrorCount
        {
            get { return _ErrorCounter; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="device">Модель устройства для которого создаётся данный контекст</param>
        public DeviceContext(DeviceBase device)
        {            
            _Device = device;
            _Objects = new LinkedList<ObjectInfo>(_Device.Profile.ObjectInfoList);
            _CurrentObject = _Objects.First;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Устанавливаем в качестве текущего следующий объект словая устройства.
        /// Если список закончился, устанавливается первый
        /// </summary>
        public void Next()
        {
            _CurrentObject = _CurrentObject.Next;
            
            if (_CurrentObject == null)
            {
                _CurrentObject = _Objects.First;
            }
        }
        /// <summary>
        /// Обработчик события завершения транзакции
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_CurrentTransaction_TransactionWasEnded(
            object sender, EventArgs e)
        {
            string msg;
            Transaction transaction = (Transaction)sender;
            
            switch (transaction.Status)
            {
                case TransactionStatus.Aborted:
                    {
                        // Если транзакция завершилась ошибкой, увеличиваем счётчик
                        ++_ErrorCounter;
                        break;
                    }
                case TransactionStatus.Completed:
                    {
                        // При удачном завершении транзации обнуляем счётчик
                        _ErrorCounter = 0;
                        break;
                    }
                default:
                    {
                        msg = String.Format("Вызов события окончания транзакции при состонии транзакции {0}",
                            transaction.Status);
                        throw new Exception(msg);
                    }
            }
        }
        #endregion
    }
}