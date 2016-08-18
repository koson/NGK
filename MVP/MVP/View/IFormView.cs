using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View.Collections.ObjectModel;

namespace Mvp.View
{
    public interface IFormView: IView
    {
        RegionContainersCollection Regions { get; }
    }
}
