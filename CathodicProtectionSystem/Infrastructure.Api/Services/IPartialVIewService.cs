using System;
using Infrastructure.Api.Plugins;

namespace Infrastructure.Api.Services
{
    public interface IPartialVIewService
    {
        IHostWindow Host { get; }
    }
}
