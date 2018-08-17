using RudycommerceData.Entities.Products.Categories;
using RudycommerceData.Entities.Products.Products;
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

        //public bool IsNumber { get; set; }

        public virtual ICollection<SpecificationEnum> Enumerations { get; set; }
        public virtual ICollection<LocalizedSpecification> LocalizedSpecifications { get; set; }
        public ICollection<CategorySpecification> CategorySpecifications { get; set; }
        public ICollection<Value_ProductSpecification> Values_ProductSpecifications { get; set; }

        public override bool IsNew()
        {
            return this.ID <= 0;
        }

        public Specification()
        {
            Enumerations = new List<SpecificationEnum>();
        }

        public override string ToString()
        {
            return $"ML:{IsMultilingual.ToString()}/Bool:{IsBool.ToString()}/Enum:{IsEnumeration.ToString()}";
        }
    }
}
