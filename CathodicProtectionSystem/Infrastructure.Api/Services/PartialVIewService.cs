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
    public class PartialVIewService : Infrastructure.Api.Services.IPartialVIewService
    {
        #region Constructors

        public PartialVIewService(IApplicationController application)
        {
            _Application = application;
        }

        #endregion

        #region Fields And Properties

        private readonly IApplicationController _Application;

        #endregion

        #region Methods

        public void Show(IPartialViewPresenter presenter)
        {
            if (_Application.MainFormPresenter != null && 
                _Application.MainFormPresenter is IHostWindow)
            {
                ((IHostWindow)_Application.MainFormPresenter).Show(presenter);
            }
        }

        #endregion
    }
}
