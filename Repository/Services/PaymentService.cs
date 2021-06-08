using Entities.Interfaces;
using Entities.Models;
using Entities.Models.OrderAggregate;
using Entities.Specifications;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public PaymentService(IBasketRepository basketRepository, IUnitOfWork unitOfWork,
            IConfiguration config)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _config = config;
        }
        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket == null)
                return null;

            var shippingPrice = 0m;

            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>()
                    .GetByIdAsync((int)basket.DeliveryMethodId);
                shippingPrice = deliveryMethod.Price;
            }

            foreach (var item in basket.Items)
            {
                var productsItem = await _unitOfWork.Repository<Entities.Models.Product>().GetByIdAsync(item.Id);
                if (item.Price != productsItem.Price)
                {
                    item.Price = productsItem.Price;
                }
            }

            var service = new PaymentIntentService();

            PaymentIntent intent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)basket.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)shippingPrice * 100,
                    Currency = "EUR",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                intent = await service.CreateAsync(options);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)shippingPrice * 100,
                };
                await service.UpdateAsync(basket.PaymentIntentId, options);
            }

            await _basketRepository.UpdateBasketAsync(basket);

            return basket;
        }

        public async Task<Entities.Models.OrderAggregate.Order> UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var spec = new OrderWithPaymentIntentIdSpecification(paymentIntentId);
            var order = await _unitOfWork.Repository<Entities.Models.OrderAggregate.Order>().GetEntityWithSpec(spec);

            if (order == null)
                return null;

            order.Status = OrderStatus.PaymentFailed;
            await _unitOfWork.Complete();

            return order;
        }

        public async Task<Entities.Models.OrderAggregate.Order> UpdateOrderPaymentSucceded(string paymentIntentId)
        {
            var spec = new OrderWithPaymentIntentIdSpecification(paymentIntentId);
            var order = await _unitOfWork.Repository<Entities.Models.OrderAggregate.Order>().GetEntityWithSpec(spec);

            if (order == null)
                return null;

            order.Status = OrderStatus.PaymentReceived;
            _unitOfWork.Repository<Entities.Models.OrderAggregate.Order>().Update(order);

            await _unitOfWork.Complete();

            return order;
        }
    }
}
