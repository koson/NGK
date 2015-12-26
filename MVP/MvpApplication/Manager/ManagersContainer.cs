using System;
using System.Collections.Generic;
using System.Text;

namespace MvpApplication.Manager
{
    public class ManagersContainer: IManagersContainer
    {
        public IResourceManager ResourceManager
        {
            get { return new ResourceManager(); }
        }
    }
}
