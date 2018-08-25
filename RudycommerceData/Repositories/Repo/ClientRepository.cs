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
            // Checks whether this e-mail is already taken by other clients with an account.
            // Those without an account shouldn't be counted, because it could be the same person who is now making an account.
            return _context.Clients.Any(c => c.Email == email && c.AccountUser == true);
        }

        public override Client Add(Client client)
        {
            // If the client wants an account
            if (client.AccountUser)
            {
                // Generate a salt and encrypts the client his password
                client.Salt = Encryption.GetNewSalt(32);
                client.EncryptedPassword = Encryption.EncryptPassword(client.Salt, client.Password);
                client.Password = null;
            }

            return base.Add(client);
        }

        public async Task<Client> FindByEmailAsync(string email)
        {
            // Finds the client with an account with a matching e-mail
            return await _context.Clients.SingleOrDefaultAsync(c => c.Email == email && c.AccountUser == true);
        }
    }
}
