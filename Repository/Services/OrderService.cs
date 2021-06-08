using Entities.Interfaces;
using Entities.Models;
using Entities.Models.OrderAggregate;
using Entities.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepository;
        private readonly IPaymentService _paymentService;

        public OrderService(IUnitOfWork unitOfWork, IBasketRepository basketRepository, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _basketRepository = basketRepository;
            _paymentService = paymentService;
        }

        public IGenericRepository<Product> GenericRepository { get; }

        public async Task<Order> CreateOrderAsync(string email, int deliveryId, string basketId, Address shippingaddress)
        {
            //get basket items
            var basket = await _basketRepository.GetBasketAsync(basketId) ?? new CustomerBasket();

            //get products from database to set product price
            var items = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
                var orderItem = new OrderItem(itemOrdered, product.Price, item.Quantity);
                items.Add(orderItem);
            }

            //get delivery method
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryId);

            //check if order exists
            var spec = new OrderWithPaymentIntentIdSpecification(basket.PaymentIntentId);
            var existingOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (existingOrder != null)
            {
                _unitOfWork.Repository<Order>().Delete(existingOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(basket.PaymentIntentId);
            }

            //calculate subtotal
            var subTotal = items.Sum(item => item.Price * item.Quantity);

            //create order
            var order = new Order(items, email, shippingaddress, deliveryMethod, subTotal, basket.PaymentIntentId);

            //save to database
            _unitOfWork.Repository<Order>().Add(order);

            var result = await _unitOfWork.Complete();

            if (result <= 0)
                return null;

            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id, string email)
        {
            var specification = new OrdersWithItemsSpecification(id, email);

            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(specification);
        }

        public async Task<IReadOnlyList<Order>> GetUserOrdersAsync(string email)
        {
            var specification = new OrdersWithItemsSpecification(email);
            var orders = await _unitOfWork.Repository<Order>().ListAsync(specification);
        
            return orders;

        }
    }
}
