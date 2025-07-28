using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Queries.Product.GetActiveProducts
{
    public class GetActiveProductsQueryRequest : IRequest<GetActiveProductsQueryResponse>
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 5;
    }
}
