using KonsolcumApi.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Abstractions.Services
{
    public interface IOrderService
    {
        Task CreateOrderAsync(CreateOrder createOrder);

        Task<List<ListOrder>> GetAllOrdersAsync(int page, int size);
        Task<int> GetTotalOrderCountAsync();

        Task<OrderDetailDto> GetOrderDetailAsync(Guid orderId);

        Task <(bool,CompletedOrderDto)> CompleteOrderAsync(string id);


        Task<List<ListOrder>> GetMyOrdersAsync(string userName, int page, int size);
        Task<int> GetMyOrderCountAsync(string userName);


    }
}
