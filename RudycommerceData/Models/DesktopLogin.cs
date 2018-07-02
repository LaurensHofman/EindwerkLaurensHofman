using RudycommerceData.Entities.DesktopUsers;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Models
{
    public class DesktopLogin
    {
        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        private IDesktopUserRepository _repo;

        public DesktopLogin()
        {
            _repo = new DesktopUserRepository();
        }

        public async Task<int?> Authenticate()
        {
            DesktopUser loginUser = await _repo.FindByUsernameAsync(this.Username);

            if (loginUser == null)
            {
                return null;
            }

            string encryptedPassword = Encryption.EncryptPassword(loginUser.Salt, this.Password);

            if (loginUser.VerifiedByAdmin)
            {
                if (loginUser.EncryptedPassword == encryptedPassword)
                {
                    return loginUser.ID;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return 0;
            }           
        }
    }
}
