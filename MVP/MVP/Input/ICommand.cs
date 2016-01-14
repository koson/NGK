using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.Input
{
    [Command]
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
        bool CanExecute();
        /// <summary>
        /// ������� ����������� ��� ��������� �������� CanExecute
        /// </summary>
        event EventHandler CanExecuteChanged;
    }
}
