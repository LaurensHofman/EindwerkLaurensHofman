using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public int? PreferredLanguageID { get; set; }
        public virtual Language PreferredLanguage { get; set; }

        [Required]
        [Index("IX_UniqueUsername", IsUnique = true, Order = 1)]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string EncryptedPassword { get; set; }

        [Required]
        public string Salt { get; set; }

        public override bool IsNew()
        {
            return this.ID <= 0;
        }

        [NotMapped]
        public string FullName
        {
            get
            {
                return this.FirstName + " " + this.LastName;
            }
        }

        public override string ToString()
        {
            return FullName;
        }

    }
}
