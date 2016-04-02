using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Input;

namespace NGK.CorrosionMonitoringSystem.Views
{
    /// <summary>
    /// ��������� �������������� ���������� ���� Region
    /// � �������� ������ ����������
    /// </summary>
    public interface ISystemButtons
    {
        /// <summary>
        /// ���������� ������� ������������� � ������� F3, F4, F5
        /// �� ������ ������ 
        /// </summary>
        Command[] ButtonCommands { get; }
    }
}
