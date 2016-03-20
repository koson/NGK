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
        IPresenter CurrentPresenter { get; }
        Form CurrentForm { get; }
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
        DialogResult ShowDialog(IPresenter presenter);
    }
}
