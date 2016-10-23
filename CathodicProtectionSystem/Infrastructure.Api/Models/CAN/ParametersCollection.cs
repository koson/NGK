using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Text;

namespace Infrastructure.Api.Models.CAN
{
    public class ParametersCollection: KeyedCollection<string, Parameter>
    {
        protected override string GetKeyForItem(Parameter item)
        {
            return item.Name;
        }
    }
}
