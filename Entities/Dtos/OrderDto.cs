using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class OrderDto
    {
        public string BasketId { get; set; }
        public int DeliveryId { get; set; }
        public AddressDto ShipAddress { get; set; }
    }
}
