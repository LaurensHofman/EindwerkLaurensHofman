using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities.Products.Products
{
    public class ProductImage : BaseEntity<int>
    {
        [Required]
        public int Order { get; set; }

        [Required]
        public string ImageURL { get; set; }

        [NotMapped]
        public string FileLocation { get; set; }

        public int? ProductID { get; set; }
        public Product Product { get; set; }

        public override bool IsNew()
        {
            return this.ID <= 0;
        }

        public override string ToString()
        {
            if (ImageURL != null)
            {
                return "ORD: " + Order.ToString() + " URL: " + ImageURL;
            }
            else
            {
                if (FileLocation != null)
                {
                    return "ORD: " + Order.ToString() + " URL: " + FileLocation;
                }
                else
                {
                    return base.ToString();
                }
            }
        }
    }
}
