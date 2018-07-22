using RudycommerceData.Entities.DesktopUsers;
using RudycommerceData.Repositories.BaseRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Repositories.IRepo
{
    public interface IDesktopUserRepository: IBaseRepository<DesktopUser>
    {
        Task<DesktopUser> FindByUsernameAsync(string username);

        void AssignNewAdmin(DesktopUser newAdmin);
    }
}
