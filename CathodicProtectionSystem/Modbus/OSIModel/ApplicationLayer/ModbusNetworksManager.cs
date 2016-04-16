using System;
using System.Collections.Generic;
using System.Text;
using Modbus.OSIModel.ApplicationLayer.Slave;

namespace Modbus.OSIModel.ApplicationLayer
{
    /// <summary>
    /// ����� ��� ���������� ������ Modbus
    /// </summary>
    public class ModbusNetworksManager
    {
        #region Constructors

        private ModbusNetworksManager()
        {
            _Networks = new ModbusNetworksCollection();
        }

        #endregion

        #region Fields And Properties
        /// <summary>
        /// Singleton
        /// </summary>
        static volatile ModbusNetworksManager _Instance;
        /// <summary>
        /// ���������� ������ ��������� �����
        /// </summary>
        public static ModbusNetworksManager Instance
        {
            get 
            {
                if (_Instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new ModbusNetworksManager();
                        }
                    }
                }
                return _Instance;
            }
        }
        /// <summary>
        /// ������ ����� 
        /// </summary>
        ModbusNetworksCollection _Networks;
        /// <summary>
        /// ���� Modbus
        /// </summary>
        public ModbusNetworksCollection Networks
        {
            get { return _Networks; }
        }
        /// <summary>
        /// ������ ��� �������������
        /// </summary>
        static object SyncRoot = new object();
        
        #endregion

        #region Methods
        /// <summary>
        /// ��������� ������������ ����� �� ����� 
        /// </summary>
        /// <param name="path">���� � ����� ������������</param>
        public void LoadConfig(string path)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// ��������� ������� ������������ � ����
        /// </summary>
        /// <param name="path">���� � ����� ������������</param>
        public void SaveConfig(string path)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// ������ ����� ����
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ModbusNetworkControllerSlave Create(WorkMode type)
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
