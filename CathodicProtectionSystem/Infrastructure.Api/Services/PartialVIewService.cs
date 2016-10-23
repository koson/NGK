using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Api.Plugins;
using Mvp.WinApplication;

namespace Infrastructure.Api.Services
{
    /// <summary>
    /// Сервис для отображения частичного предстваления на главной
    /// форме приложения
    /// </summary>
    public class PartialVIewService : IPartialVIewService
    {
        #region Constructors

        public PartialVIewService(IApplicationController application)
        {
            if (application.MainFormPresenter is IHostWindow)
                _HostWindow = (IHostWindow)application.MainFormPresenter;
        }

        #endregion

        #region Fields And Properties

        private readonly IHostWindow _HostWindow;
        public IHostWindow Host { get { return _HostWindow; } }

        #endregion

        #region Methods
        #endregion
    }
}
