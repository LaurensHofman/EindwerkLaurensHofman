using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Models
{
    public class SpecificationOverviewItem
    {
        public int ID { get; set; }

        public string SpecName { get; set; }

        public bool IsBool { get; set; }

        public bool IsML { get; set; }

        public bool IsEnum { get; set; }
    }
}
