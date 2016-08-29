using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.LogManager;
using NGK.CorrosionMonitoringSystem.Managers.SysLogManager;
using NGK.CorrosionMonitoringSystem.Managers.AppConfigManager;
using NGK.CorrosionMonitoringSystem.Managers.Factory;
using NGK.CorrosionMonitoringSystem.Services;

namespace NGK.CorrosionMonitoringSystem.Managers
{
    public interface IManagers
    {
        /// <summary>
        /// ������ ��
        /// </summary>
        Version SoftwareVersion { get; }
        /// <summary>
        /// ������ ����������
        /// </summary>
        Version HardwareVersion { get; }
        /// <summary>
        /// �������� ��� ����������� ����������
        /// </summary>
        ILogManager Logger { get; }
        /// <summary>
        /// �������� ��� ������ ��������� ����������
        /// � ������ ������� ����������
        /// </summary>
        ISysLogManager SystemLogger { get; }
        /// <summary>
        /// �������� ��� ������ � ������ ������������
        /// ����������
        /// </summary>
        IConfigManager ConfigManager { get; }
        /// <summary>
        /// ������ ��� ������ � CAN-������
        /// </summary>
        ICanNetworkService CanNetworkService { get; }
        /// <summary>
        /// ������ ��� ������ � Modbus-����� 
        /// (����� Slave: ��� ���������� ������ �������� ������ )
        /// </summary>
        ISystemInformationModbusNetworkService ModbusSystemInfoNetworkService { get; }
    }
}
