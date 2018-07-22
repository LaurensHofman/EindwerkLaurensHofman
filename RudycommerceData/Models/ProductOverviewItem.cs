using RudycommerceLib.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Models
{
    public class ProductOverviewItem
    {
        public int ID { get; set; }

        public string ProductName { get; set; }

        public string ImageURL { get; set; }

        public bool IsActive { get; set; }

        public string CategoryName { get; set; }

        public decimal UnitPrice { get; set; }

        public int CurrentStock { get; set; }

        public int MinimumStock { get; set; }

        public string BrandName { get; set; }

        public string FirstSpecName { get; set; }

        public string FirstSpecValue { get; set; }

        public bool IsFirstSpecBool { get; set; }

        public string FirstSpec
        {
            get
            {
                if (IsFirstSpecBool)
                {
                    return FirstSpecName + ": " + ((FirstSpecValue.ToLower() == "true") ? LangResource.Yes : LangResource.No);
                }
                else
                {
                    return FirstSpecName + ": " + FirstSpecValue;
                }
            }
        }

        public bool IsBelowMinStock
        {
            get
            {
                return CurrentStock < MinimumStock;
            }
        }

        public override string ToString()
        {
            return ProductName;
        }
    }
}
