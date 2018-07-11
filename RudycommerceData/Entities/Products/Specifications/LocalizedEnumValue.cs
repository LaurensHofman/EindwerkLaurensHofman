using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities.Products.Specifications
{
    public class LocalizedEnumValue
    {
        public int LanguageID { get; set; }

        public int EnumerationID { get; set; }

        [Required]
        public string Value { get; set; }

        public override string ToString()
        {
            return $"Enum: {Value}";
        }
    }
}
