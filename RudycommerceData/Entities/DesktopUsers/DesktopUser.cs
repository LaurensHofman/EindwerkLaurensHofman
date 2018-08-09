using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Entities.DesktopUsers
{
    public class DesktopUser : Base.UserWithPassword
    {
        public bool IsAdmin { get; set; }

        public bool VerifiedByAdmin { get; set; }

        public int? PreferredLanguageID { get; set; }
        public virtual Language PreferredLanguage { get; set; }

        [Required]
        [Index("IX_UniqueUsername", IsUnique = true, Order = 1)]
        [MaxLength(50)]
        public string Username { get; set; }
        
        public override string ToString()
        {
            return FullName;
        }
    }
}
