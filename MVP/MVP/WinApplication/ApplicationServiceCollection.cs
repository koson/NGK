using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Mvp.WinApplication.Collections.ObjectModel
{
    public sealed class ApplicationServiceCollection: 
        KeyedCollection<string, ApplicationServiceBase>
    {
        #region Methods

        protected override string GetKeyForItem(ApplicationServiceBase item)
        {
            return item.ServiceName;
        }

        public ReadOnlyCollection<ApplicationServiceBase> AsReadOnly()
        {
            return new ReadOnlyCollection<ApplicationServiceBase>(this);
        }

        #endregion
    }
}
