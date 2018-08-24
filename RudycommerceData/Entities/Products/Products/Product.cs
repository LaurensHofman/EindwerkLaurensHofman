using RudycommerceData.Entities.Orders;
using RudycommerceData.Entities.Products.Categories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities.Products.Products
{
    public class Product : BaseEntity<int>
    {
        [Required]
        public decimal? UnitPrice { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public int? InitialStock { get; set; }

        [Required]
        public int CurrentStock { get; set; }

        [Required]
        public int? MinimumStock { get; set; }

        public int CategoryID { get; set; }
        public virtual Category Category { get; set; }

        public int BrandID { get; set; }
        public virtual Brand Brand { get; set; }

        public Product()
        {
            this.IsActive = true;
        }

        public override bool IsNew()
        {
            return this.ID <= 0;
        }

        public virtual ICollection<ProductImage> Images { get; set; }
        public virtual ICollection<LocalizedProduct> LocalizedProducts { get; set; }
        public virtual ICollection<Value_ProductSpecification> Values_ProductSpecifications { get; set; }

        public ICollection<IncomingOrderLines> IncomingOrderLines { get; set; }

        public override string ToString()
        {
            if (LocalizedProducts == null)
            {
                return base.ToString();
            }
            else
            {
                LocalizedProduct lp = LocalizedProducts.FirstOrDefault();

                if (lp.Name == null)
                {
                    return base.ToString();
                }
                else
                {
                    return lp.Name;
                }
            }
        }
    }
}
