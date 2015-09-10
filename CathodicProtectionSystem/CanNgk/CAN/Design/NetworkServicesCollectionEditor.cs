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
        /// ������ ������ ���� ���� ������� �������� ���������� ������������ 
        /// ������������ ������ NetworkService
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
        /// ����� ���������� ������ �������� ���������� (���������� ������������ ������
        /// NGK.CAN.OSIModel.ApplicationLayer.DeviceProxy.Device) � ��������� ��� � ���������
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        //protected override object CreateInstance(Type itemType)
        //{
            //// ����� ���� �������� ��� ������ ������� ���������.
            //ArrayList collection = ((NetworkController)Context.Instance).Services;

            //// ������ ��������� �������� ����������
            //Device device;
            //device = (Device)Activator.CreateInstance(itemType, new Object[] { address });
            //// ��������� � ������
            //collection.Add(device);
            //// ���������� ��� ������
            //return device;
            ////return base.CreateInstance(itemType);
        //    throw new Exception("������ ������ �������� ������ ��� ��������������");
        //}
        //--------------------------------------------------------------------------------
        /// <summary>
        /// ����� ���������� �������� ������� ��������� ��� ��������������
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
        /// ���� ������� ���������, ������� ������������ ������ �������� ���������
        /// </summary>
        /// <returns></returns>
        protected override Type[] CreateNewItemTypes()
        {
            // �������� ��������� ��������� ���� ��������
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
                // ������� ��� ���� ����������� ����������� �����
                // NetworkService
                this._ReturnedTypes = this.GetReturnedTypes(provider);
            }

            return base.EditValue(context, provider, value);
        }
        /// <summary>
        /// ���������� ������ ����� ��������, ������� ������������� �� ������������ ������
        /// NetworkService
        /// </summary>
        /// <param name="provider"></param>
        /// <returns>������ ����� ������� ���������</returns>
        private Type[] GetReturnedTypes(IServiceProvider provider)
        {
            List<Type> result = new List<Type>();

            // �������� ��������� �������. ��������� � �����, ��� �������� ������ 
            // � design-time. � run-time tds ������������ null !!!
            ITypeDiscoveryService tds = (ITypeDiscoveryService)provider.
                GetService(typeof(ITypeDiscoveryService));

            if (tds != null)
            {
                // ��� ������ design-time
                // ��� ����� ��� "������������" ���� Device
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
                // ��� ������ run-time
                System.Reflection.Assembly asmbl = System.Reflection.Assembly.GetExecutingAssembly();
                
                // ��� ����� ��� "������������" ���� Device
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
