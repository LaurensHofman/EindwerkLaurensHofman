using RudycommerceData.Models.ASPModels.ProductDetailsPageSubItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Models.ASPModels
{
    public class ProductDetailsPageItem
    {
        public ProdDetProductInfo ProductInfo { get; set; }

        public List<ProdDetImage> Images { get; set; }

        public List<ProdDetSpecInfoAndValue> SpecificationInfoAndValues { get; set; }
    }
}
