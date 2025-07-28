using KonsolcumApi.Application.DTOs.Order;
using KonsolcumApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<List<ListOrder>> GetAllOrdersPagedAsync(int page, int size);
        Task<int> GetTotalOrderCountAsync();

        Task<OrderDetailDto> GetOrderDetailByIdAsync(Guid orderId);

        Task<(bool, CompletedOrderDto?)> CompleteOrderAsync(string id);

        Task<List<ListOrder>> GetMyOrdersPagedAsync(string username, int page, int size);
        Task<int> GetMyOrderCountAsync(string username);

        Task<List<BasketItemDto>> GetBasketItemsByBasketIdAsync(Guid basketId);

        public class BasketItemDto
        {
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }

    }
}
