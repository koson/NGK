using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CorrosionMonitoringSystem.View
{
    /// <summary>
    /// ��������� �������������� � �������� ������ ����������
    /// </summary>
    public interface IButtonsPanel
    {
        Boolean ButtonF3IsAccessible { get; set; }
        Boolean ButtonF4IsAccessible { get; set; }
        Boolean ButtonF5IsAccessible { get; set; }

        event EventHandler<ButtonClickEventArgs> ButtonClick;
    }
}
