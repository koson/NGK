using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Design;
using NGK.CAN.ApplicationLayer.Network.Master.Services;

namespace NGK.CAN.ApplicationLayer.Network.Master.Services.Collections
{
    /// <summary>
    /// Класс-контейнер для хранения списка сетей CAN НГК-ЭХЗ
    /// </summary>
    //[Editor(typeof(NetworkServicesCollectionEditor), typeof(UITypeEditor))]
    [Serializable]
    public sealed class NetworkServicesCollection: KeyedCollection<ServiceType, Service>
    {
        #region Fields And Properties
        /// <summary>
        /// Сеть, которой принадлежит данный список устройств
        /// </summary>
        private INetworkController _Network;
        /// <summary>
        /// Сеть, которой принадлежит данный список устройств
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Настройки")]
        [DisplayName("Сеть CAN НГК-ЭХЗ")]
        [Description("Объект для представления сети CAN для мастера сети. Содержит список сетевых устройств и CAN-порт")]
        public INetworkController Network
        {
            get { return _Network; }
            set { _Network = value; }
        }
        #endregion

        #region Constructors
        public NetworkServicesCollection()
            : base()
        {
            this._Network = null;
        }
        public NetworkServicesCollection(INetworkController network)
            : base()
        {
            this._Network = network;
        }
        #endregion

        #region Methods

        protected override ServiceType GetKeyForItem(Service item)
        {
            return item.ServiceType;
        }
        protected override void InsertItem(int index, Service item)
        {
            item.NetworkController = _Network;
            base.InsertItem(index, item);
        }
        protected override void RemoveItem(int index)
        {
            base[index].NetworkController = null;
            base.RemoveItem(index);
        }
        protected override void ClearItems()
        {
            for (int i = 0; i < base.Count; i++)
            {
                base[i].NetworkController = null;
            }
            base.ClearItems();
        }
        protected override void SetItem(int index, Service item)
        {
            item.NetworkController = _Network;
            base.SetItem(index, item);
        }
        public Service[] ToArray()
        {
            List<Service> list = new List<Service>();

            foreach (Service service in base.Items)
            {
                list.Add((Service)service);
            }
            return list.ToArray();
        }
        #endregion
    }// End Of Class
}// End Of Namespace
