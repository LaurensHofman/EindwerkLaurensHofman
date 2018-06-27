using RudycommerceData.Entities.Products.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities.Products.Specifications
{
    public class Specification : BaseEntity<int>
    {
        public bool IsMultilingual { get; set; }

        public bool IsBool { get; set; }

        public bool IsEnumeration { get; set; }

        public virtual ICollection<SpecificationEnum> Enumerations { get; set; }

        public virtual ICollection<CategorySpecification> CategorySpecifications { get; set; }

        public override bool IsNew()
        {
            return this.ID <= 0;
        }

        public Specification()
        {
            IsMultilingual = true;
            Enumerations = new List<SpecificationEnum>();
        }
    }
}
