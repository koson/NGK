using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Infrastructure.Dal.DbEntity
{
    /// <summary>
    /// ������������ ������� ������� SystemEventCodes
    /// </summary>
    public enum SystemEventCodes: int
    {
        [Description("�� ����������")]
        Undefined = 0,
        [Description("������ �������")]
        SystemWasStarted = 1,
        [Description("��������� �������")]
        SystemWasStopped = 2
    }
}
