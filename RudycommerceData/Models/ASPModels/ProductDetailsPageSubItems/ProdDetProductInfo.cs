using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Models.ASPModels.ProductDetailsPageSubItems
{
    public class ProdDetProductInfo
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Decimal UnitPrice { get; set; }

        public int CurrentStock { get; set; }

        public string BrandIconURL { get; set; }

        public string BrandName { get; set; }
    }
}
