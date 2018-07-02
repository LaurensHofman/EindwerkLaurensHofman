using RudycommerceData.Entities.DesktopUsers;
using RudycommerceData.Repositories.BaseRepo;
using RudycommerceData.Repositories.IRepo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Repositories.Repo
{
    public class DesktopUserRepository : BaseRepository<DesktopUser>, IDesktopUserRepository
    {
        public async Task<DesktopUser> FindByUsernameAsync(string username)
        {
            return await _context.DesktopUsers.SingleOrDefaultAsync(du => du.Username == username);
        }
    }
}
