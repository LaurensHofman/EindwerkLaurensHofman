﻿using System;
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

        public override bool IsNew()
        {
            return this.ID <= 0;
        }
    }
}