using System;
using System.Collections.Generic;
using System.Text;
using Mvp.WinApplication;
using Mvp.WinApplication.Infrastructure;
using Infrastructure.API.Managers;
using System.Windows.Forms;

namespace Infrastructure.Api.Plugins
{
    public interface IPlugin
    {
        #region Properties
        
        /// <summary>
        /// ������� ���������� ��������������� ��������
        /// </summary>
        IEnumerable<IApplicationService> ApplicationServices { get; }
        /// <summary>
        /// �������� ������� �������������� ���� ���������������� ��������
        /// </summary>
        NavigationMenuItem NavigationMenu { get; }
        /// <summary>
        /// ��������� �������� ����� ��� ������� ����� ����������
        /// </summary>
        IEnumerable<IPartialViewPresenter> PartialPresenters { get; }
        /// <summary>
        /// �������� ��������� ������ ��������������� ��������
        /// </summary>
        IEnumerable<ToolStripItem> StatusBarItems { get; }
        /// <summary>
        /// ��������� ��� ���������� ��������� �������� ����������
        /// </summary>
        IManagers Managers { get; }
        
        #endregion

        #region Methods

        /// <summary>
        /// �������������� ������� � ���������� �������.
        /// ���������� ����� �������� �������
        /// </summary>
        void Initialize(IHostWindow host, IManagers managers, object state);

        #endregion
    }
}
