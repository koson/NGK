using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View.Collections.ObjectModel;
using System.Windows.Forms;

namespace Mvp.View
{
    public interface IWindowView: IView
    {
        #region Fields And Properties

        RegionContainersCollection Regions { get; }
        Form Form { get; }

        #endregion

        #region Methods

        void Hide();

        #endregion

        #region Events
        #endregion
    }
}
