using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.OrderAggregate
{
    public class Order : BaseModel
    {
        public Order()
        {

        }

        public Order(IReadOnlyList<OrderItem> orderItems, string buyerEmail, Address shipAddress, 
                DeliveryMethod deliveryMethod, decimal subTotal, string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            ShipAddress = shipAddress;
            DeliveryMethod = deliveryMethod;
            OrderItems = orderItems;
            SubTotal = subTotal;
            paymentIntentId = paymentIntentId;
        }

        public string BuyerEmail { get; set; }
        public DateTime? OrderDate { get; set; } = DateTime.Now;
        public Address ShipAddress { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public IReadOnlyList<OrderItem> OrderItems { get; set; }
        public decimal SubTotal { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string PaymentIntentId { get; set; }
        public decimal GetTotal()
        {
            return SubTotal + DeliveryMethod.Price;
        }
    }
}
