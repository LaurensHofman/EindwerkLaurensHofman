using RudycommerceData.Entities.Products.Specifications;
using RudycommerceData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Mapping
{
    public static class EntitiesMapping
    {
        public static SpecificationOverviewItem MapToSpecificationOverviewItem(Specification spec, int langID)
        {
            string name = spec.LocalizedSpecifications.SingleOrDefault(x => x.LanguageID == langID).LookupName;

            SpecificationOverviewItem specOVI = new SpecificationOverviewItem
            {
                ID = spec.ID,
                SpecName = name,
                IsBool = spec.IsBool,
                IsEnum = spec.IsEnumeration,
                IsML = spec.IsMultilingual
            };

            return specOVI;
        }

        public static List<SpecificationOverviewItem> MapToSpecificationOverviewItem(List<Specification> specList, int langID)
        {
            List<SpecificationOverviewItem> list = new List<SpecificationOverviewItem>();

            foreach (var spec in specList)
            {
                list.Add(MapToSpecificationOverviewItem(spec, langID));
            }

            return list;
        }
    }
}
