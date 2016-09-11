using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;
using System.Windows.Forms;
using Infrastructure.Api.Controls;

namespace Infrastructure.Api.Plugins
{
    public interface IPartialViewPresenter : IRegionPresenter
    {
        string Title { get; }
        IEnumerable<FunctionalButton> FunctionalButtons { get; }
    }
}
