using MediatR;

namespace KonsolcumApi.Application.Features.Queries.Order.GetMyOrders
{
    public class GetMyOrdersQueryRequest : IRequest<GetMyOrdersQueryResponse>
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 5;
    }
}