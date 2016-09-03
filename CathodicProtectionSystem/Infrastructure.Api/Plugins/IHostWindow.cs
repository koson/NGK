using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Api.Plugins
{
    public interface IHostWindow
    {
        void Show(IPartialViewPresenter presenter);
    }
}
