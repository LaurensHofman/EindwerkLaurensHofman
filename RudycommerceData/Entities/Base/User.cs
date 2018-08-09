using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities.Base
{
    public abstract class User: BaseEntity<int>
    {
        [Required(ErrorMessageResourceName = "LastNameReq",
            ErrorMessageResourceType = typeof(Validation.Client))]
        [StringLength(maximumLength: 30, MinimumLength = 1, 
            ErrorMessageResourceName = "LastNameLength", ErrorMessageResourceType = typeof(Validation.Client))]
        [Display(Name = nameof(Validation.Client.LastName), ResourceType = typeof(Validation.Client))]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceName = "FirstNameReq",
            ErrorMessageResourceType = typeof(Validation.Client))]
        [StringLength(maximumLength: 30, MinimumLength = 1,
            ErrorMessageResourceName = "FirstNameLength", ErrorMessageResourceType = typeof(Validation.Client))]
        [Display(Name = nameof(Validation.Client.FirstName), ResourceType = typeof(Validation.Client))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "EmailReq",
            ErrorMessageResourceType = typeof(Validation.Client))]
        [DataType(DataType.EmailAddress, 
            ErrorMessageResourceType = typeof(Validation.Client), 
            ErrorMessageResourceName = "EmailType")]
        [Display(Name = nameof(Validation.Client.Email), ResourceType = typeof(Validation.Client))]
        public string Email { get; set; }

        [NotMapped]
        public string FullName { get { return this.FirstName + " " + this.LastName; } }

        public override string ToString()
        {
            return FullName;
        }

        public override bool IsNew()
        {
            return this.ID <= 0;
        }
    }
}
