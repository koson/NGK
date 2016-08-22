using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;

namespace PluginsInfrastructure
{
    public interface IPartialViewPresenter : IRegionPresenter
    {
        string Title { get; }
    }
}
