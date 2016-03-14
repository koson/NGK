using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Mvp.View
{
    public interface IViewRegion
    {
        #region Properties
        
        string RegionName { get; }
        
        #endregion

        #region Methods

        void Show(IView partialView);
        
        #endregion
    }
}
