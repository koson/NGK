using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;

namespace Mvp.WinApplication
{
    public interface IApplicationController
    {
        /// <summary>
        /// ���������� ��� ������������� ������� �����
        /// </summary>
        IPresenter CurrentScreen { get; set; }
    }
}
