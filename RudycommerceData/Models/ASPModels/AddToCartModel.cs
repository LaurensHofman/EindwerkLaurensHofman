using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Models.ASPModels
{
    public class AddToCartModel
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public string UnitPrice { get; set; }
    }
}
