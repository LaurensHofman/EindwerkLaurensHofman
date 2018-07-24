using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Models.ASPModels
{
    public class ProductListItem
    {
        public int ProductID { get; set; }

        public Decimal UnitPrice { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageURL { get; set; }

        public string CategoryName { get; set; }

        public string TopSpecName { get; set; }

        public string TopSpecValue { get; set; }

        public bool TopSpecIsBool { get; set; }

        public bool? BoolValue { get; set; }

        public string TopSpec
        {
            get
            {
                if (TopSpecIsBool)
                {
                    bool.TryParse(TopSpecValue, out bool isValBool);
                    if (isValBool)
                    {
                        return null;
                    }
                    else return null;
                }
                else
                {
                    return TopSpecName + ": " + TopSpecValue;
                }
            }
        }
    }
}
