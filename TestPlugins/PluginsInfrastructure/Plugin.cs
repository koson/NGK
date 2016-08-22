using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Plugin;
using Mvp.WinApplication;
using System.Collections.ObjectModel;
using Mvp.WinApplication.Infrastructure;

namespace PluginsInfrastructure
{
    public abstract class Plugin: PluginBase
    {
        #region Constructors

        public Plugin()
        {
            _ApplicationServices = new List<IApplicationService>();
            _Menu = new List<NavigationMenuItem>();
            _PartialPresenters = new List<IPartialViewPresenter>();
        }

        #endregion

        #region Fields And Properties

        protected readonly List<IApplicationService> _ApplicationServices;
        protected readonly List<NavigationMenuItem> _Menu;
        protected readonly List<IPartialViewPresenter> _PartialPresenters;
        
        /// <summary>
        /// ������� ���������� ��������������� ��������
        /// </summary>
        public ReadOnlyCollection<IApplicationService> ApplicationServices 
        { 
            get { return _ApplicationServices.AsReadOnly(); } 
        }
        /// <summary>
        /// ������������� ���� ��������������� ��������
        /// </summary>
        public ReadOnlyCollection<NavigationMenuItem> Menu
        {
            get { return _Menu.AsReadOnly(); }
        }
        /// <summary>
        /// ��������� �������� ����� ��� ������� ����� ����������
        /// </summary>
        public ReadOnlyCollection<IPartialViewPresenter> PartialPresenters
        {
            get { return _PartialPresenters.AsReadOnly(); }
        }

        #endregion
    }
}
