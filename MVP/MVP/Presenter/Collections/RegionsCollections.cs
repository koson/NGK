using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Mvp.Presenter.Collections
{
    public class RegionsCollections: KeyedCollection<string, Region>
    {
        protected override string GetKeyForItem(Region item)
        {
            return item.Name;
        }
    }
}
