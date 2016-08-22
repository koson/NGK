using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Mvp.View
{
    public interface IPartialView: IView
    {
        #region Properties
        #endregion

        #region Methods

        void Hide();

        #endregion
    }
}
