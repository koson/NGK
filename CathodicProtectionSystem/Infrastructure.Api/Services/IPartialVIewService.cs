using System;
using Infrastructure.Api.Plugins;

namespace Infrastructure.Api.Services
{
    public interface IPartialVIewService
    {
        void Show(IPartialViewPresenter presenter);
    }
}
