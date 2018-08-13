using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities.Orders
{
    public class IncomingOrderLines
    {
        public int ProductID { get; set; }

        public int OrderID { get; set; }

        public int ProductQuantity { get; set; }

        public Decimal ProductUnitPrice { get; set; }
    }
}
