using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.View;
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
        /// <summary>
        /// ���������� ��������� ����
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        void ShowDialog(IPresenter presenter);
    }
}
