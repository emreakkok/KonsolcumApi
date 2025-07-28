using KonsolcumApi.Application.Abstractions.Services;
using KonsolcumApi.Application.DTOs.Order;
using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Persistence.Services
{
    public class OrderService : IOrderService
    {
        readonly IOrderRepository _orderRepository;
        readonly ICompletedOrderRepository _completedOrderRepository;
        readonly IProductRepository _productRepository;
        public OrderService(IOrderRepository orderRepository, ICompletedOrderRepository completedOrderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _completedOrderRepository = completedOrderRepository;
            _productRepository = productRepository;
        }


        public async Task CreateOrderAsync(CreateOrder createOrder)
        {
            var orderCode = $"KC-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            // Önce siparişi oluştur
            var order = new Order
            {
                ShippingAddress = createOrder.ShippingAddress,
                OrderCode = orderCode,
                BasketId = Guid.Parse(createOrder.BasketId)
            };

            await _orderRepository.CreateAsync(order);

            // Sipariş oluşturulduktan sonra stokları düş
            await ReduceStockForOrderAsync(Guid.Parse(createOrder.BasketId));
        }

        private async Task ReduceStockForOrderAsync(Guid basketId)
        {
            try
            {
                // BasketId'ye göre sepetteki ürünleri al
                var basketItems = await _orderRepository.GetBasketItemsByBasketIdAsync(basketId);

                Console.WriteLine($"=== STOK DÜŞÜRME İŞLEMİ ===");
                Console.WriteLine($"BasketId: {basketId}");
                Console.WriteLine($"BasketItems Count: {basketItems?.Count ?? 0}");

                if (basketItems != null && basketItems.Any())
                {
                    foreach (var basketItem in basketItems)
                    {
                        Console.WriteLine($"Stok düşürülüyor - ProductId: {basketItem.ProductId}, Quantity: {basketItem.Quantity}");

                        try
                        {
                            await _productRepository.ReduceStockAsync(basketItem.ProductId, basketItem.Quantity);
                            Console.WriteLine($"✓ Başarılı - ProductId: {basketItem.ProductId}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"✗ Hata - ProductId: {basketItem.ProductId}, Hata: {ex.Message}");
                            // Hata logla ama devam et
                        }
                    }
                }
                Console.WriteLine($"=== STOK DÜŞÜRME BİTTİ ===\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SEPET İTEMLERİ ALMA HATASI: {ex.Message}");
            }
        }

        public async Task<List<ListOrder>> GetAllOrdersAsync(int page, int size)
        {
            return await _orderRepository.GetAllOrdersPagedAsync(page, size);
        }

        public async Task<OrderDetailDto> GetOrderDetailAsync(Guid orderId)
        {
            return await _orderRepository.GetOrderDetailByIdAsync(orderId);
        }

        public async Task<int> GetTotalOrderCountAsync()
        {
            return await _orderRepository.GetTotalOrderCountAsync();
        }


        public async Task<(bool, CompletedOrderDto?)> CompleteOrderAsync(string id)
        {
            return await _orderRepository.CompleteOrderAsync(id);
        }



        public async Task<List<ListOrder>> GetMyOrdersAsync(string userName, int page, int size)
        {
            return await _orderRepository.GetMyOrdersPagedAsync(userName, page, size);
        }

        public async Task<int> GetMyOrderCountAsync(string userName)
        {
            return await _orderRepository.GetMyOrderCountAsync(userName);
        }


    }
}
