using System;
using System.Collections.Generic;
using System.Text;
using Mvp.WinApplication;
using Mvp.WinApplication.Infrastructure;
using Infrastructure.API.Managers;

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
        IManagers Managers { get; set; }
        
        #endregion

        #region Methods

        /// <summary>
        /// �������������� ������� � ���������� �������.
        /// ���������� ����� �������� �������
        /// </summary>
        void Initialize(IHostWindow host, object state);

        #endregion
    }
}
