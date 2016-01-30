using System;
using System.Collections.Generic;
using System.Text;
using NGK.CorrosionMonitoringSystem.Managers.LogManager;
using NGK.CorrosionMonitoringSystem.Managers.SysLogManager;
using NGK.CorrosionMonitoringSystem.Managers.AppConfigManager;
using NGK.CorrosionMonitoringSystem.Managers.Factory;
using NGK.CorrosionMonitoringSystem.Services;
using NGK.CAN.ApplicationLayer.Network.Master;

namespace NGK.CorrosionMonitoringSystem.Managers
{
    public interface IManagers
    {
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
        /// ��������� �� ����������
        /// </summary>
        INavigationService NavigationService { get; }
        /// <summary>
        /// ������� �� �������� ���� ����������
        /// </summary>
        IWindowsFactory WindowsFactory { get; }
        INetworksManager NetworksService { get; }
    }
}
