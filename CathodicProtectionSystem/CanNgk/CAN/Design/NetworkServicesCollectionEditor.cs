using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using NGK.CAN.ApplicationLayer.Network.Master.Services;

namespace NGK.CAN.OSIModel.ApplicationLayer.NetWork.Master.Design
{
    public sealed class NetworkServicesCollectionEditor: CollectionEditor
    {
        #region Fields And Properties        
        /// <summary>
        /// Массив хранит типы всех сетевых сервисов являющихся наследниками 
        /// абстрактного класса NetworkService
        /// </summary>
        private Type[] _ReturnedTypes;

        #endregion
                
        #region Constructors

        public NetworkServicesCollectionEditor(Type type)
            : base(type)
        { }

        #endregion

        #region Methods
        /// <summary>
        /// Метод возвращает объект сетевого устройства (наследника абстрактного класса
        /// NGK.CAN.OSIModel.ApplicationLayer.DeviceProxy.Device) и добавляет его в коллекцию
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        //protected override object CreateInstance(Type itemType)
        //{
            //// Здесь надо получить сам список сетевых устройств.
            //ArrayList collection = ((NetworkController)Context.Instance).Services;

            //// Создаём экземпляр сетевого устройства
            //Device device;
            //device = (Device)Activator.CreateInstance(itemType, new Object[] { address });
            //// Добавляем в массив
            //collection.Add(device);
            //// Возвращаем сам объект
            //return device;
            ////return base.CreateInstance(itemType);
        //    throw new Exception("Данный список доступен только для редактирования");
        //}
        //--------------------------------------------------------------------------------
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
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Типы сетевых устройств, которые поддерживает данный редактор коллекции
        /// </summary>
        /// <returns></returns>
        protected override Type[] CreateNewItemTypes()
        {
            // Сообщаем редактору возможные типы объектов
            return _ReturnedTypes;
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
            System.ComponentModel.ITypeDescriptorContext context, 
            IServiceProvider provider, object value)
        {
            if (_ReturnedTypes == null)
            {
                // Находим все типы наследующие абстрактный класс
                // NetworkService
                this._ReturnedTypes = this.GetReturnedTypes(provider);
            }

            return base.EditValue(context, provider, value);
        }
        /// <summary>
        /// Возвращает массив типов объектов, которые унаследованны от абстрактного класса
        /// NetworkService
        /// </summary>
        /// <param name="provider"></param>
        /// <returns>Массив типов сетевых устройств</returns>
        private Type[] GetReturnedTypes(IServiceProvider provider)
        {
            List<Type> result = new List<Type>();

            // Получаем интерфейс сервиса. Насколько я понял, это работает только 
            // В design-time. В run-time tds возвращается null !!!
            ITypeDiscoveryService tds = (ITypeDiscoveryService)provider.
                GetService(typeof(ITypeDiscoveryService));

            if (tds != null)
            {
                // Для режима design-time
                // Нам нужны все "родственники" типа Device
                foreach (Type type in tds.GetTypes(typeof(NetworkService), false))
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
                System.Reflection.Assembly asmbl = System.Reflection.Assembly.GetExecutingAssembly();
                
                // Нам нужны все "родственники" типа Device
                foreach (Type type in asmbl.GetTypes())
                {
                    if (type.BaseType == typeof(NetworkService))
                    {
                        if (!result.Contains(type))
                        {
                            result.Add(type);
                        }
                    }
                 }
            }
            return result.ToArray();
        }
        #endregion        
    }
}
