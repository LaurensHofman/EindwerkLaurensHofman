using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Models.ASPModels
{
    public class Delivery
    {
        public bool OtherAddress { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validation.Client),
            ErrorMessageResourceName = "StreetReq")]
        [StringLength(maximumLength: 80, MinimumLength = 2, ErrorMessageResourceType = typeof(Validation.Client),
            ErrorMessageResourceName = "StreetLength")]
        [RegularExpression("^(.+)\\s(\\d+(\\s*[^\\d\\s]+)*)$", ErrorMessageResourceName = "StreetRegex",
            ErrorMessageResourceType = typeof(Validation.Client))]
        [Display(Name = nameof(Validation.Client.StreetAndNumber), ResourceType = typeof(Validation.Client))]
        public string StreetAndNumber { get; set; }

        [Display(Name = nameof(Validation.Client.MailBox), ResourceType = typeof(Validation.Client))]
        public string MailBox { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validation.Client),
            ErrorMessageResourceName = "CityReq")]
        [StringLength(maximumLength: 60, MinimumLength = 2, ErrorMessageResourceType = typeof(Validation.Client),
            ErrorMessageResourceName = "CityLength")]
        [Display(Name = nameof(Validation.Client.City), ResourceType = typeof(Validation.Client))]
        public string City { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validation.Client),
            ErrorMessageResourceName = "PostalCodeReq")]
        [StringLength(maximumLength: 10, MinimumLength = 1, ErrorMessageResourceType = typeof(Validation.Client),
            ErrorMessageResourceName = "PostalCodeRange")]
        [Display(Name = nameof(Validation.Client.PostalCode), ResourceType = typeof(Validation.Client))]
        public string PostalCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validation.Client),
            ErrorMessageResourceName = "CountryCodeReq")]
        [Display(Name = nameof(Validation.Client.Country), ResourceType = typeof(Validation.Client))]
        public string CountryCode { get; set; }
        
        [NotMapped]
        public Dictionary<string, string> CountriesByCode { get { return new Countries.CountriesDictionary().OrderBy(x => x.Value).ToDictionary(x => x.Key, y => y.Value); } }

        public Delivery()
        {
            OtherAddress = false;
            CountryCode = "BE";
        }
    }
}
