using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Models
{
    public class CategoryOverviewItem
    {
        public int ID { get; set; }

        public string LocalizedName { get; set; }

        public string LocalizedPluralName { get; set; }

        public int SpecCount { get; set; }
    }
}
