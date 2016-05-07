using System;
using System.Collections.Generic;
using System.Text;
using Common.Controlling;

namespace Mvp.WinApplication
{
    /// <summary>
    /// ��������� ��� �������� ������� ����������
    /// </summary>
    public interface IApplicationService: IManageable, IDisposable
    {
        /// <summary>
        /// ������������ �������
        /// </summary>
        string ServiceName { get; }
        /// <summary>
        /// ���������� �������� ����������� ������ ������
        /// </summary>
        IApplicationController Application { get; }
        /// <summary>
        /// ���������� ������ �������: ��������������� ��� ���
        /// </summary>
        bool IsInitialized { get; }
        /// <summary>
        /// �������������� ������
        /// </summary>
        /// <param name="context"></param>
        void Initialize(Object context);
    }
}
