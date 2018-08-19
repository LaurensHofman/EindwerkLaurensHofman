using RudycommerceData.Entities;
using RudycommerceLib.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Repositories.Repo
{
    public class ClientRepository: BaseRepo.BaseRepository<Client>, IRepo.IClientRepository
    {
        public bool EmailTaken(string email)
        {
            return _context.Clients.Any(c => c.Email == email && c.AccountUser == true);
        }

        public override Client Add(Client client)
        {
            if (client.AccountUser)
            {
                client.Salt = Encryption.GetNewSalt(32);

                client.EncryptedPassword = Encryption.EncryptPassword(client.Salt, client.Password);
                client.Password = null;
            }

            return base.Add(client);
        }

        public async Task<Client> FindByEmailAsync(string email)
        {
            return await _context.Clients.SingleOrDefaultAsync(c => c.Email == email && c.AccountUser == true);
        }
    }
}
