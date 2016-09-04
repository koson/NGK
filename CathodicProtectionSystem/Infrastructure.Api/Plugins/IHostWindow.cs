using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Api.Plugins
{
    public interface IHostWindow
    {
        #region Properties

        IPartialViewPresenter SelectedPartivalViewPresenter { get; }

        #endregion 

        #region Methods

        void Show(IPartialViewPresenter presenter);

        #endregion

        #region Events

        event EventHandler SelectedPartivalViewPresenterChanged;

        #endregion
    }
}
