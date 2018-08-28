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
        /// <summary>
        /// Gets the Order, changes the status code to 1 and updates the order
        /// </summary>
        /// <param name="id">The ID of the Incoming Order</param>
        /// <returns></returns>
        public async Task<IncomingOrder> SetOrderAsReadyForPickup(int id)
        {
            var order = await GetAsync(id);

            if (order.StatusCode == 0)
            {
                order.StatusCode = 1;

                await UpdateAsync(order);
            }

            return order;
        }

        public async Task<IncomingOrder> SetOrderAsPickedUp(int id)
        {
            var order = await GetAsync(id);

            if (order.StatusCode == 1)
            {
                order.StatusCode = 2;

                await UpdateAsync(order);
            }

            return order;
        }

        public async Task<IncomingOrder> SetOrderAsDelivered(int id)
        {
            var order = await GetAsync(id);

            if (order.StatusCode == 2)
            {
                order.StatusCode = 3;

                await UpdateAsync(order);
            }

            return order;
        }
    }
}
