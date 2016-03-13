using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public interface IMainWindowView: IView
    {
        /// <summary>
        /// ���������� ��������� ����
        /// </summary>
        String Title { get; set; }
        /// <summary>
        /// ���������� ��� ������������� ������� � �������� ������ ����
        /// </summary>
        UserControl CurrentControl { get; set; }
    }
}
