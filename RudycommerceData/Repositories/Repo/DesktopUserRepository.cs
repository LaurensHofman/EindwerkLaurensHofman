using RudycommerceData.Entities.DesktopUsers;
using RudycommerceData.Repositories.BaseRepo;
using RudycommerceData.Repositories.IRepo;
using RudycommerceLib.CustomExceptions;
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
        public override DesktopUser Add(DesktopUser entity)
        {
            bool usernameTaken = GetAllQueryable().Any(x => x.Username.ToLower() == entity.Username.ToLower());

            if (usernameTaken)
            {
                throw new UsernameTaken();
            }

            return base.Add(entity);
        }

        /// <summary>
        /// Makes the selected user the new admin, and makes the old admin lose its rights
        /// </summary>
        /// <param name="newAdmin"></param>
        public void AssignNewAdmin(DesktopUser newAdmin)
        {
            DesktopUser oldAdmin = GetAllQueryable().SingleOrDefault(x => x.IsAdmin == true);

            if (oldAdmin != null)
            {
                oldAdmin.IsAdmin = false;
                newAdmin.IsAdmin = true;

                Update(oldAdmin, newAdmin);
            }
        }

        public async Task<DesktopUser> FindByUsernameAsync(string username)
        {
            try
            {
                return await _context.DesktopUsers.SingleOrDefaultAsync(du => du.Username == username);
            }
            catch (Exception)
            {
                throw new UsernameNotUnique();
            }
        }
    }
}
