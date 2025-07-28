using MediatR;

namespace KonsolcumApi.Application.Features.Queries.Product.GetProductDetail
{
    public class GetProductDetailQueryRequest : IRequest<GetProductDetailQueryResponse>
    {
        public string ProductId { get; set; }
    }
}