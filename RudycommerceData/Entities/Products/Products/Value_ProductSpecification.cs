﻿using RudycommerceData.Entities.Products.Specifications;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities.Products.Products
{
    public class Value_ProductSpecification
    {
        public int ProductID { get; set; }

        public int SpecificationID { get; set; }

        public int? LanguageID { get; set; }

        public string Value { get; set; }

        public bool? BoolValue { get; set; }

        public int? SpecificationEnumID { get; set; }
        public virtual SpecificationEnum SpecificationEnum { get; set; }

        [NotMapped]
        public string DisplayValue { get; set; }

        [NotMapped]
        public bool HoldsValueForOtherNonML { get; set; }

        public override string ToString()
        {
            if (Value != null)
            {
                return Value;
            }
            else
            {
                if (DisplayValue != null)
                {
                    return DisplayValue;
                }
                else
                {
                    return base.ToString();
                }
            }
        }

        public Value_ProductSpecification()
        {
            BoolValue = null;
        }
    }
}
