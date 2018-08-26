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
            // Looks for the client
            Client client = await _repo.FindByEmailAsync(this.Email);

            // If the client is null, no client was found
            if (client == null)
            {
                return null;
            }

            // Encrypts the inserted password in the same way it was encrypted in the database
            string encryptedPassword = Encryption.EncryptPassword(client.Salt, this.Password);
            
            // If the result is the same, that means the same password was inserted
            if (client.EncryptedPassword == encryptedPassword)
            {
                return client.ID;
            }
            else
            {
                // If the result is not the same, that means another password was inserted
                return null;
            }
        }
    }
}
