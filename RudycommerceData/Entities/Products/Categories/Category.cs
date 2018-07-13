using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities.Products.Categories
{
    public class Category: BaseEntity<int>
    {
        [NotMapped]
        public string LocalizedName { get; set; }

        public override bool IsNew()
        {
            return this.ID <= 0;
        }

        public virtual ICollection<LocalizedCategory> LocalizedCategories { get; set; }

        public virtual ICollection<CategorySpecification> CategorySpecifications { get; set; }

        public override string ToString()
        {
            if (LocalizedName != null)
            {
                return LocalizedName;
            }
            else
            {
                return base.ToString();
            }    
        }
    }
}
