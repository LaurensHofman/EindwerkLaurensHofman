using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Models
{
    public class NecessarySpecification
    {
        public string LookupName { get; set; }
        
        public bool IsBool { get; set; }
        
        public bool IsMultilingual { get; set; }
        
        public bool IsEnumeration { get; set; }
        
        public int CategoryID { get; set; }
        
        public int SpecificationID { get; set; }

        public int DisplayOrder { get; set; }
    }
}
