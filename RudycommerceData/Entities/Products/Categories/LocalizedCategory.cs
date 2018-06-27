using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities.Products.Categories
{
    public class LocalizedCategory
    {
        public int CategoryID { get; set; }
        
        public int LanguageID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string PluralName { get; set; }

    }
}
