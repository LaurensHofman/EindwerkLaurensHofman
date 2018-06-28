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

        public Category Category { get; set; }

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
        public virtual ICollection<Values_ProductSpecifications> Values_ProductSpecifications { get; set; }
    }
}
