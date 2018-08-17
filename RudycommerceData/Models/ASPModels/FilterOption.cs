using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Models.ASPModels
{
    public class Filters
    {
        public int CategoryID { get; set; }

        public List<FilterOption> FilterOptions { get; set; }

        public Filters()
        {
            FilterOptions = new List<FilterOption>();
        }
    }

    public class FilterOption
    {
        public int SpecID { get; set; }

        public string SpecName { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsEnum { get; set; }

        public bool IsBool { get; set; }

        public List<FilterValue> FilterValues { get; set; }

        public override string ToString()
        {
            return SpecName;
        }
    }

    public class FilterValue
    {
        public string Value { get; set; }

        public bool IsSelected { get; set; }

        public int? EnumID { get; set; }

        public bool? BoolValue { get; set; }

        public bool StringValueIsNumber
        {
            get
            {
                if (float.TryParse(Value, out float val))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override string ToString()
        {
            if (Value == null)
            {
                return BoolValue.ToString() + "; Selected = " + IsSelected.ToString();
            }
            return Value + "; Selected = " + IsSelected.ToString();
        }
    }

    public class FilterOptionSQL
    {
        public int SpecID { get; set; }

        public string SpecName { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsEnum { get; set; }

        public bool IsBool { get; set; }

        public string Value { get; set; }

        public bool IsSelected { get; set; }

        public int? EnumID { get; set; }

        public bool? BoolValue { get; set; }
    }
}
