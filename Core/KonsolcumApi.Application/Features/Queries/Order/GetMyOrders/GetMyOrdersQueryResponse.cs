using KonsolcumApi.Application.DTOs.Order;

namespace KonsolcumApi.Application.Features.Queries.Order.GetMyOrders
{
    public class GetMyOrdersQueryResponse
    {
        public int TotalOrderCount { get; set; }
        public List<ListOrder> Orders { get; set; }
        public string UserName { get; set; }
    }
}