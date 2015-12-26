using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Presenter
{
    public interface ICommand
    {
        /// <summary>
        /// ��������� ������� �� ����������
        /// </summary>
        void Execute();
        /// <summary>
        /// ���������� ��������, ������� ���������� ����� ��
        /// ���� ��������� ������ ������� 
        /// </summary>
        bool CanExecute { get; }
        /// <summary>
        /// ������� ����������� ��� ��������� �������� CanExecute
        /// </summary>
        event EventHandler CanExecuteChanged;
    }
}
