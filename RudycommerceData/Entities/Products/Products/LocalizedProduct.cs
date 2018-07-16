using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities.Products.Products
{
    public class LocalizedProduct
    {
        public int ProductID { get; set; }

        public int LanguageID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public override string ToString()
        {
            if (this.Name == null)
            {
                return base.ToString();
            }
            else
            {
                return this.Name;
            }
        }
    }
}
