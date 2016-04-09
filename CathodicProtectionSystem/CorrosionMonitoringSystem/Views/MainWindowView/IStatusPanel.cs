using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public interface IStatusPanel
    {
        /// <summary>
        /// ����� ��������� � �������
        /// </summary>
        Int32 TotalDevices { set; }

        /// <summary>
        /// ����������� ��������� � �������
        /// </summary>
        Int32 FaultyDevices { set; }

        /// <summary>
        /// ������������� ��������� � ������� �������
        /// </summary>
        Int32 UnreadMessages { set; }
    }
}
