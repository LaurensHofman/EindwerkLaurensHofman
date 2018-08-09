using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities
{
    public class Client : Base.UserWithPassword
    {
        public bool AccountUser { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(Validation.Client), 
            ErrorMessageResourceName = "StreetReq")]
        [StringLength(maximumLength:80, MinimumLength = 2, ErrorMessageResourceType = typeof(Validation.Client),
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
            ErrorMessageResourceName = "CityReq")]
        [Display(Name = nameof(Validation.Client.Country), ResourceType = typeof(Validation.Client))]
        public string CountryCode { get; set; }

        [Display(Name = nameof(Validation.Client.WantsNewsLetterLabel), ResourceType = typeof(Validation.Client))]
        public bool WantsNewsletter { get; set; }

        [NotMapped]
        // https://stackoverflow.com/questions/4730183/mvc-model-require-true
        public bool AgreesToTermsAndConditions { get; set; }

        [NotMapped]
        public Dictionary<string, string> CountriesByCode { get { return new Countries.CountriesDictionary().OrderBy(x => x.Value).ToDictionary(x => x.Key, y => y.Value); } }
        
        public Client()
        {
            CountryCode = "BE";
            AgreesToTermsAndConditions = false;
            WantsNewsletter = false;
            AccountUser = false;
        }
    }
}
