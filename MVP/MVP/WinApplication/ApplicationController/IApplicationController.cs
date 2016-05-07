using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.View;
using Mvp.Presenter;
using System.Threading;

namespace Mvp.WinApplication
{
    public interface IApplicationController
    {
        #region Properties

        SynchronizationContext SyncContext { get; }
        
        ApplicationContext AppContext { get; }
        /// <summary>
        /// ���������� ������� ���� �������
        /// </summary>
        IPresenter CurrentPresenter { get; }
        /// <summary>
        /// ������� �����
        /// </summary>
        Form CurrentForm { get; }
        /// <summary>
        /// ������ ��
        /// </summary>
        Version Version { get; }
        /// <summary>
        /// ������� ����������
        /// </summary>
        IApplicationService[] AppServices { get; }

        #endregion

        #region Methods
        
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
        /// <summary>
        /// ������������ ������ ����������
        /// </summary>
        /// <param name="service"></param>
        void RegisterApplicationService(IApplicationService service);

        #endregion
    }
}
