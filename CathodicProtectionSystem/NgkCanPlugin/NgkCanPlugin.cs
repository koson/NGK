using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Api.Plugins;
using Mvp.WinApplication.Infrastructure;
using NGK.Plugins.Services;
using NGK.CAN.ApplicationLayer.Network.Master;
using Mvp.WinApplication.ApplicationService;

namespace NGK.Plugins
{
    public class NgkCanPlugin: Plugin
    {
        #region Constructors

        public NgkCanPlugin()
        {
            Name = @"CAN НГК ЭХЗ";

            NavigationMenu = new NavigationMenuItem(Name, null);
            NavigationMenu.SubMenuItems.Add(null);
        }

        #endregion

        #region Fields And Properties

        internal static CanNetworkService CanNetworkService = null;

        #endregion 

        #region Methods

        public override void Initialize(object state)
        {
            // Создаём сервисы приложения
            try
            {
                // Загружаем конфигурацию из файла
                NgkCanNetworksManager.Instance.LoadConfig(Managers.ConfigManager.PathToAppDirectory +
                    @"\newtorkconfig.bin.nwc");

                //Создаём сетевой сервис и регистрируем его
                CanNetworkService = new CanNetworkService(
                    "NgkCanService", NgkCanNetworksManager.Instance, 300, Managers);
                base.ApplicationServices.Add(canNetworkService);

                //WindowsFormsApplication.Application.RegisterApplicationService(canNetworkService);
                //canNetworkService.Initialize(null);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    String.Format("Ошибка при инициализации плагина {0}", Name), ex);
            }
        }

        #endregion 

        #region Commands
        #endregion
    }
}
