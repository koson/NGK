using System;
using System.Collections.Generic;
using System.Text;
using Modbus.OSIModel.ApplicationLayer.Slave;

namespace Modbus.OSIModel.ApplicationLayer
{
    /// <summary>
    /// ����� ��� ���������� ������ Modbus
    /// </summary>
    public class NetworksManager
    {
        #region Fields And Properties
        /// <summary>
        /// Singleton
        /// </summary>
        private static NetworksManager _Instance;
        /// <summary>
        /// ���������� ������ ��������� �����
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
        /// ������ ����� 
        /// </summary>
        private NetworksCollection _Networks;
        /// <summary>
        /// ���� Modbus
        /// </summary>
        public NetworksCollection Networks
        {
            get { return _Networks; }
        }
        /// <summary>
        /// ������ ��� �������������
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
