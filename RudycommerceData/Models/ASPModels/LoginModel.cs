using RudycommerceData.Entities;
using RudycommerceData.Repositories.IRepo;
using RudycommerceLib.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Models.ASPModels
{
    public class LoginModel
    {
        [Required]
        [Display(Name = nameof(Validation.Client.Email), ResourceType = typeof(Validation.Client))]
        public string Email { get; set; }

        [Required]
        [Display(Name = nameof(Validation.Client.Password), ResourceType = typeof(Validation.Client))]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        public async Task<int?> Authenticate(IClientRepository _repo)
        {
            Client client = await _repo.FindByEmailAsync(this.Email);

            if (client == null)
            {
                return null;
            }

            string encryptedPassword = Encryption.EncryptPassword(client.Salt, this.Password);
            
            if (client.EncryptedPassword == encryptedPassword)
            {
                return client.ID;
            }
            else
            {
                return null;
            }
        }
    }
}
