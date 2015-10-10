using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Drawing.Design;
using System.ComponentModel.Design;
//using System.Windows.Forms.Design;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.Collections;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles;
using NGK.CAN.ApplicationLayer.Network.Master;

namespace NGK.CAN.Design
{
    /// <summary>
    /// Класс для редактора устройства сети CAN НГК-ЭХЗ 
    /// </summary>
    public sealed class DevicesCollectionEditor : CollectionEditor
    {
        #region Fields And Properties
        /// <summary>
        /// Массив хранит типы всех сетевых устройств являющихся наследниками 
        /// абстрактного класса NGK.CAN.OSIModel.ApplicationLayer.DeviceProxy.Device
        /// </summary>
        private Type[] _ReturnedTypes;
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public DevicesCollectionEditor(Type type)
            : base(type)
        { }
        #endregion

        #region Methods
        /// <summary>
        /// Метод возвращает объект сетевого устройства (наследника абстрактного класса
        /// NGK.CAN.ApplicationLayer.Network.Devices.Device) и добавляет его в коллекцию
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        protected override object CreateInstance(Type itemType)
        {
            // Здесь надо получить сам список сетевых устройств.
            DevicesCollection collection = 
                ((NetworkController)Context.Instance).Devices;

            // Находим уникальный сетевой адрес для нового устройства
            for (byte address = 1; address < 128; address++)
            {
                if (!collection.Contains(address))
                {
                    // Создаём экземпляр сетевого устройства
                    DeviceBase device = null;
                    
                    switch (itemType.FullName)
                    {
                        case @"NGK.CAN.ApplicationLayer.Network.Devices.Profiles.KIP9810v1":
                            {
                                device = DeviceBase.Create(DeviceType.KIP_MAIN_POWERED_v1);
                                device.NodeId = address;
                                // Добавляем в массив
                                collection.Add(device);
                                break;
                            }
                        case @"NGK.CAN.ApplicationLayer.Network.Devices.Profiles.KIP9811v1":
                            {
                                device = DeviceBase.Create(DeviceType.KIP_BATTERY_POWER_v1);
                                device.NodeId = address;
                                // Добавляем в массив
                                collection.Add(device);
                                break;
                            }
                        default:
                            {
                                throw new NotSupportedException(String.Format(
                                    "Тип профиля устройсва {0} не поддерживается в текущей версии ПО",
                                    itemType.FullName));
                            }
                    }
                    // Возвращаем сам объект
                    return device;
                }
            }
            
            throw new InvalidOperationException(
                "Невозможно добавить устройство, закончилось адресное пространство сети");
            //return base.CreateInstance(itemType);
        }
        /// <summary>
        /// Метод отображает свойства объекта коллекции для редактирования
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override string GetDisplayText(object value)
        {
            if (value != null)
            {
                return value.ToString();
            }
            else
            {
                return base.GetDisplayText(value);
            }
        }
        /// <summary>
        /// Типы сетевых устройств, которые поддерживает данный редактор коллекции
        /// </summary>
        /// <returns></returns>
        protected override Type[] CreateNewItemTypes()
        {
            // Сообщаем редактору возможные типы объектов
            //return new Type[] { typeof(kip9810_v1), typeof(kip9811_v1) };

            return new Type[] { typeof(KIP9810v1), typeof(KIP9811v1) };
            //return _ReturnedTypes;
            //return base.CreateNewItemTypes();
        }  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object EditValue(
            ITypeDescriptorContext context, 
            IServiceProvider provider, object value)
        {
            if (_ReturnedTypes == null)
            {
                // Находим все типы наследующие абстрактный класс
                // NGK.CAN.OSIModel.ApplicationLayer.DeviceProxy.Device
                //this._ReturnedTypes = this.GetReturnedTypes(provider);
            }

            return base.EditValue(context, provider, value);
        }
        /// <summary>
        /// Возвращает массив типов объектов, которые унаследованны от абстрактного класса
        /// NGK.CAN.OSIModel.ApplicationLayer.DeviceProxy.Device
        /// </summary>
        /// <param name="provider"></param>
        /// <returns>Массив типов сетевых устройств</returns>
        private Type[] GetReturnedTypes(IServiceProvider provider)
        {
            List<Type> result = new List<Type>();

            // Получаем интерфейс сервиса. Насколько я понял, это работает только 
            // В design-time. В run-time tds возвращается null !!!
            System.ComponentModel.Design.ITypeDiscoveryService tds =
                (System.ComponentModel.Design.ITypeDiscoveryService)provider.GetService(
                typeof(System.ComponentModel.Design.ITypeDiscoveryService));

            if (tds != null)
            {
                // Для режима design-time
                // Нам нужны все "родственники" типа Device
                foreach (Type type in tds.GetTypes(typeof(Prototype), false))
                {
                    if (!result.Contains(type))
                    {
                        result.Add(type);
                    }
                }
            }
            else
            {
                // Для режима run-time
                //System.Reflection.Assembly asmbl = System.Reflection.Assembly.GetAssembly(typeof(Device));
                Assembly asmbl = Assembly.GetExecutingAssembly();
                
                // Нам нужны все "родственники" типа Device
                foreach (Type type in asmbl.GetTypes())
                {
                    if (type.BaseType == typeof(Prototype))
                    {
                        if (!result.Contains(type))
                        {
                            result.Add(type);
                        }
                    }
                }

                //result.Add(typeof(kip9810_v1));
                //result.Add(typeof(kip9811_v1));
            }

            return result.ToArray();
        }
        #endregion
    }
}
