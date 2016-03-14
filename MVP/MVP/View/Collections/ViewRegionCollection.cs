using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Mvp.View.Collections.ObjectModel
{
    public class ViewRegionCollection: KeyedCollection<string, IViewRegion>
    {
        protected override string GetKeyForItem(IViewRegion item)
        {
            return item.RegionName;
        }
    }
}
