using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Api.Plugins;
using Mvp.WinApplication.Infrastructure;
using NGK.Plugins.Services;
using NGK.CAN.ApplicationLayer.Network.Master;
using Mvp.WinApplication.ApplicationService;
using Mvp.Input;
using NGK.Plugins.Presenters;

namespace NGK.Plugins
{
    public class NgkCanPlugin: Plugin
    {
        #region Constructors

        public NgkCanPlugin()
        {
            Name = @"CAN НГК ЭХЗ";

            _ShowDevicesList = new Command(OnShowDevicesList, CanShowDevicesList);

            NavigationMenu = new NavigationMenuItem(Name, null);
            NavigationMenu.SubMenuItems.Add(new NavigationMenuItem("Устройства", _ShowDevicesList));
        }

        #endregion

        #region Fields And Properties

        private static CanNetworkService _CanNetworkService;
        internal static CanNetworkService CanNetworkService
        {
            get { return _CanNetworkService; }
            set 
            { 
                _CanNetworkService = value;
                _ShowDevicesList.CanExecute();
            }
        }

        private static IHostWindow _HostWindow;
        internal static IHostWindow HostWindow 
        { 
            get { return _HostWindow; } 
        }

        #endregion 

        #region Methods

        public override void Initialize(IHostWindow hostWindow, object state)
        {
            _HostWindow = hostWindow;

            // Создаём сервисы приложения
            //try
            //{
            //    // Загружаем конфигурацию из файла
            //    NgkCanNetworksManager.Instance.LoadConfig(Managers.ConfigManager.PathToAppDirectory +
            //        @"\newtorkconfig.bin.nwc");

            //    //Создаём сетевой сервис и регистрируем его
            //    CanNetworkService canNetworkService = new CanNetworkService(
            //        "NgkCanService", NgkCanNetworksManager.Instance, 300, Managers);
            //    base.ApplicationServices.Add(canNetworkService);

            //    //WindowsFormsApplication.Application.RegisterApplicationService(canNetworkService);
            //    //canNetworkService.Initialize(null);
            //}
            //catch (Exception ex)
            //{
            //    throw new InvalidOperationException(
            //        String.Format("Ошибка при инициализации плагина {0}", Name), ex);
            //}
        }

        #endregion 

        #region Commands

        private static Command _ShowDevicesList;
        private static void OnShowDevicesList()
        {
            DevicesListPresenter presenter = new DevicesListPresenter();
            presenter.Show();
        }
        private static bool CanShowDevicesList()
        {
            return CanNetworkService != null;
        }

        #endregion
    }
}
