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

        #region TopSpecs

        public string TopSpec1Name { get; set; }

        public string TopSpec1Value { get; set; }

        public bool TopSpec1IsBool { get; set; }

        public bool? Bool1Value { get; set; }

        public string TopSpec1
        {
            get
            {
                if (TopSpec1IsBool)
                {
                    bool.TryParse(TopSpec1Value, out bool isValBool);
                    if (isValBool)
                    {
                        return null;
                    }
                    else return null;
                }
                else
                {
                    return TopSpec1Name + ": " + TopSpec1Value;
                }
            }
        }


        public string TopSpec2Name { get; set; }

        public string TopSpec2Value { get; set; }

        public bool TopSpec2IsBool { get; set; }

        public bool? Bool2Value { get; set; }

        public string TopSpec2
        {
            get
            {
                if (TopSpec2IsBool)
                {
                    bool.TryParse(TopSpec2Value, out bool isValBool);
                    if (isValBool)
                    {
                        return null;
                    }
                    else return null;
                }
                else
                {
                    return TopSpec2Name + ": " + TopSpec2Value;
                }
            }
        }

        #endregion
    }
}
