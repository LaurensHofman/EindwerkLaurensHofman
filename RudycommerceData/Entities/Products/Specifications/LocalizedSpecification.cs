using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities.Products.Specifications
{
    public class LocalizedSpecification
    {
        public int PropertyID { get; set; }

        public int LanguageID { get; set; }

        [Required]
        public string LookupName { get; set; }

        [DataType(DataType.MultilineText)]
        public string AdviceDescription { get; set; }
    }
}
