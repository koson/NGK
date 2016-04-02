using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Input;

namespace NGK.CorrosionMonitoringSystem.Views
{
    /// <summary>
    /// ��������� ����������� ���������� ���� Region � ������
    /// ��� ������ ������� ��������� ���� ���������� �������� ���� ����������
    /// </summary>
    public interface ISystemMenu
    {
        ICommand[] MenuItems { get; }
    }
}
