using MediatR;

namespace KonsolcumApi.Application.Features.Queries.Category.GetProductsByCategory
{
    public class GetProductsByCategoryQueryRequest : IRequest<GetProductsByCategoryQueryResponse>
    {
        public Guid CategoryId { get; set; }

        public int Page { get; set; } = 0;

        public int Size { get; set; } = 12;
    }
}