using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.DAL.DbModels
{
    /// <summary>
    /// ��������� ���������
    /// </summary>
    public enum Category: byte
    {
        Information = 0,
        Error = 1,
        CriticalError = 2
    }
}
