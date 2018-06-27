using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities.Products.Categories
{
    public class CategorySpecification
    {
        public int CategoryID { get; set; }

        public int SpecificationID { get; set; }

        public int DisplayOrder { get; set; }

        [NotMapped]
        public string PropertyName { get; set; }

    }
}
