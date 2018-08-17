﻿using RudycommerceData.Models.ASPModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.EqualityComparer
{
    internal class FilterValueComparer : IEqualityComparer<FilterValue>
    {
        public bool Equals(FilterValue x, FilterValue y)
        {
            return x.BoolValue == y.BoolValue &&
                    x.EnumID == y.EnumID &&
                    x.StringValueIsNumber == y.StringValueIsNumber &&
                    x.Value == y.Value;
        }

        public int GetHashCode(FilterValue obj)
        {
            return base.GetHashCode();
        }
    }
}
