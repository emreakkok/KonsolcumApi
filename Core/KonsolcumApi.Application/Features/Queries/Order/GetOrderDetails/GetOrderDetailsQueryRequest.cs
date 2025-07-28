using MediatR;

namespace KonsolcumApi.Application.Features.Queries.Order.GetOrderDetails
{
    public class GetOrderDetailsQueryRequest : IRequest<GetOrderDetailsQueryResponse>
    {
        public Guid OrderId { get; set; }
    }
}