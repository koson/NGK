using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;

namespace Mvp.WinApplication
{
    public interface IApplicationController
    {
        /// <summary>
        /// ���������� ������� ���� �������
        /// </summary>
        IPresenter CurrentWindow { get; }
        /// <summary>
        /// ���������� ����� ���� ������� 
        /// </summary>
        /// <param name="presenter"></param>
        void ShowWindow(IPresenter presenter);
    }
}
