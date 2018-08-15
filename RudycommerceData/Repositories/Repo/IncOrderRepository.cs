using RudycommerceData.Entities.Orders;
using RudycommerceData.Repositories.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Repositories.Repo
{
    public class IncOrderRepository: BaseRepo.BaseRepository<IncomingOrder>, IIncOrderRepository
    {

    }
}
