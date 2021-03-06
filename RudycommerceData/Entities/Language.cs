﻿using RudycommerceData.Entities.Products.Categories;
using RudycommerceData.Entities.Products.Products;
using RudycommerceData.Entities.Products.Specifications;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities
{
    public class Language : BaseEntity<int>
    {
        public ICollection<LocalizedCategory> LocalizedCategories { get; set; }
        public ICollection<LocalizedEnumValue> LocalizedEnumValues { get; set; }
        public ICollection<LocalizedProduct> LocalizedProducts { get; set; }
        public ICollection<LocalizedSpecification> LocalizedSpecifications { get; set; }
        public ICollection<Value_ProductSpecification> Values_ProductSpecifications { get; set; }

        [Required]
        [StringLength(255)]
        public string LocalName { get; set; }
        
        [Required]
        [StringLength(255)]
        public string DutchName { get; set; }
        
        [Required]
        [StringLength(255)]
        public string EnglishName { get; set; }
        
        [Index(IsUnique = true)]
        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string ISO { get; set; }
        
        public bool IsDesktopLanguage { get; set; }
        
        //public bool IsActive { get; set; }
        
        public bool IsDefault { get; set; }

        public override bool IsNew()
        {
            return this.ID <= 0;
        }

        public override string ToString()
        {
            return this.LocalName;
        }

        public Language()
        {
            this.IsDefault = false;
            this.IsDesktopLanguage = false;
        }
    }
}
