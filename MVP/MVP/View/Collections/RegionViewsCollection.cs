using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Mvp.View.Collections.ObjectModel
{
    public class RegionContainersCollection: KeyedCollection<string, IRegionContainer>
    {
        protected override string GetKeyForItem(IRegionContainer item)
        {
            return item.Name;
        }
    }
}
