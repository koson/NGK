using System;
using System.Collections.Generic;
using System.Text;
using Common.Controlling;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public interface ISystemInformationModbusNetworkService : IManageable, IDisposable
    {
        #region Properties
        /// <summary>
        /// ������� ����� ����������
        /// </summary>
        byte Address { get; set; }

        #endregion
        
        #region Methods
        /// <summary>
        /// �������������� ������
        /// </summary>
        void Initialize();

        #endregion
    }
}
