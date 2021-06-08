using AutoMapper;
using Entities.Dtos;
using Entities.Interfaces;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerBasket(string id)
        {
            return Ok(await _basketRepository.GetBasketAsync(id) ?? new CustomerBasket(id));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCustomerBasket(CustomerBasketDto basketDto)
        {
            var basket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basketDto);
      
            return Ok(await _basketRepository.UpdateBasketAsync(basket));
        }

        [HttpDelete]
        public async Task DeleteCustomerBasket(string id)
        {
            await _basketRepository.DeleteBasketAsync(id);
        }
    }
}
