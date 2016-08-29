using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;

namespace Infrastructure.Api.Plugins
{
    public interface IPartialViewPresenter : IRegionPresenter
    {
        string Title { get; }
    }
}
