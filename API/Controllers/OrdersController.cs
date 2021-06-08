using API.Errors;
using AutoMapper;
using Entities.Dtos;
using Entities.Interfaces;
using Entities.Models.OrderAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService  orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderDto orderDto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var address = _mapper.Map<AddressDto, Address>(orderDto.ShipAddress);

            var order = await _orderService.CreateOrderAsync(email, orderDto.DeliveryId, orderDto.BasketId, address);

            if (order == null)
                return StatusCode(400, "Problem creating order");

            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserOrders()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var orders = await _orderService.GetUserOrdersAsync(email);
         
            return Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserOrderById(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var order = await _orderService.GetOrderByIdAsync(id, email);

            if (order == null)
                return NotFound(new ApiResponse(404));

            return Ok(_mapper.Map<Order, OrderToReturnDto>(order));
        }

        [HttpGet("deliveryMethods")]
        public async Task<IActionResult> GetDeliveryMethods()
        {
            return Ok(await _orderService.GetDeliveryMethodAsync());
        }
    }
}
