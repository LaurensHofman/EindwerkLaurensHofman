using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Models.ASPModels
{
    public class SortingDictionary : Dictionary<string, string>
    {
        public SortingDictionary()
        {
            this.Add("Name-ASC", LangResources.Data.SortNameAZ);
            this.Add("Name-DESC", LangResources.Data.SortNameZA);
            this.Add("Price-ASC", LangResources.Data.SortPriceLoHi);
            this.Add("Price-DESC", LangResources.Data.SortPriceHiLo);
        }
    }
}
