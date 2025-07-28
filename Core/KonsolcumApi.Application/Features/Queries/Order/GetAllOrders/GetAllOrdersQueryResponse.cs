using KonsolcumApi.Application.DTOs.Order;

namespace KonsolcumApi.Application.Features.Queries.Order.GetAllOrders
{
    public class GetAllOrdersQueryResponse
    {
        public int TotalOrderCount { get; set; }
        public List<ListOrder> Orders { get; set; }
    }

}