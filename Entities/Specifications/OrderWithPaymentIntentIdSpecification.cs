using Entities.Models.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Specifications
{
    public class OrderWithPaymentIntentIdSpecification: BaseSpecification<Order>
    {
        public OrderWithPaymentIntentIdSpecification(string paymentIntentId) 
            : base(o => o.PaymentIntentId == paymentIntentId)
        {

        }
    }
}
