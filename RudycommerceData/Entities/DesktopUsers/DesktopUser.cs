using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities.DesktopUsers
{
    public class DesktopUser : BaseEntity<int>
    {
        public bool IsAdmin { get; set; }

        public bool VerifiedByAdmin { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string Email { get; set; }

        public Language PreferredLanguage { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string EncryptedPassword { get; set; }

        [Required]
        public string Salt { get; set; }

        public override bool IsNew()
        {
            return this.ID <= 0;
        }

    }
}
