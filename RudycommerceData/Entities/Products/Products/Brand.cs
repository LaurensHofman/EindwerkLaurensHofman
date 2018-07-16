using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities.Products.Products
{
    public class Brand : BaseEntity<int>
    {
        public string Name { get; set; }

        public string LogoURL { get; set; }

        [NotMapped]
        public string LocalLogoPath { get; set; }

        public override bool IsNew()
        {
            return this.ID <= 0;
        }

        public override string ToString()
        {
            if (Name == null)
            {
                return base.ToString();
            }
            else
            {
                return Name;
            }
        }
    }
}
