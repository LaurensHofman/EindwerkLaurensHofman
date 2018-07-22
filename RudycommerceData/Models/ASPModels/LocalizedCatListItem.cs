using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RudycommerceData.Models.ASPModels
{
    public class LocalizedCatListItem
    {
        public int CategoryID { get; set; }

        public string LocalizedName { get; set; }

        public string LocalizedPluralName { get; set; }
    }
}