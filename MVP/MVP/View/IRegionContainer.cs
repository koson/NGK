using System;
using System.Collections.Generic;
using System.Text;

namespace Mvp.View
{
    public interface IRegionContainer
    {
        string Name { get; }
        IRegionView RegionView { get; set; }
    }
}
