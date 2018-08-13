using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities.Base
{
    public abstract class UserWithPassword : User
    {
        public string EncryptedPassword { get; set; }

        public string Salt { get; set; }
        
        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = nameof(Validation.Client.Password), ResourceType = typeof(Validation.Client))]
        public virtual string Password { get; set; }
    }
}
