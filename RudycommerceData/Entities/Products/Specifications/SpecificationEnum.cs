using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities.Products.Specifications
{
    public class SpecificationEnum
    {
        public int ID { get; set; }

        public Specification Specification { get; set; }

        [NotMapped]
        public string TemporaryNonMLValue { get; set; }

        public virtual ICollection<LocalizedEnumValue> LocalizedEnumValues { get; set; }
    }
}
