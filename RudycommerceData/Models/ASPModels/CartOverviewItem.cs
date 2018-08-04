using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Models.ASPModels
{
    public class CartOverviewItem
    {
        public int ProductID { get; set; }

        public Decimal UnitPrice { get; set; }

        public string ProductName { get; set; }

        public string ImageURL { get; set; }

        public int Quantity { get; set; }

        public Decimal Price
        {
            get { return Quantity * UnitPrice; }
        }
    }
}
