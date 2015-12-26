using System;
using System.Collections.Generic;
using System.Text;

namespace MvpApplication.Manager
{
    public interface IManagersContainer
    {
        IResourceManager ResourceManager { get; }
    }
}
