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
    public interface ISysemMenu
    {
        ICommand[] MenuItems { get; }
    }
}
