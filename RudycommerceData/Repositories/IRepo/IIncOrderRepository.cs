using RudycommerceData.Entities.Orders;
using RudycommerceData.Repositories.BaseRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Repositories.IRepo
{
    public interface IIncOrderRepository : IBaseRepository<IncomingOrder>
    {
        Task<IncomingOrder> SetOrderAsReadyForPickup(int id);

        Task<IncomingOrder> SetOrderAsDelivered(int id);

        Task<IncomingOrder> SetOrderAsPickedUp(int id);
    }
}
