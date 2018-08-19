using RudycommerceData.Entities;
using RudycommerceData.Repositories.BaseRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Repositories.IRepo
{
    public interface IClientRepository : IBaseRepository<Client>
    {
        bool EmailTaken(string email);

        Task<Client> FindByEmailAsync(string email);
    }
}
