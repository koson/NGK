using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Infrastructure.Dal.DbEntity
{
    /// <summary>
    /// ��������� ���������
    /// </summary>
    public enum Category: byte
    {
        [Description("����������")]
        Information = 0,
        [Description("������")]
        Error = 1,
        [Description("����������� ������")]
        CriticalError = 2
    }
}
