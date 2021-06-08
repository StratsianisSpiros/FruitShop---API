using Entities.Models.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string email, int deliveryMethod, 
                        string basketId, Address shippingaddress);
        Task<IReadOnlyList<Order>> GetUserOrdersAsync(string email);

        Task<Order> GetOrderByIdAsync(int id, string email);

        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync();
    }
}
