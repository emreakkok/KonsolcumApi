using KonsolcumApi.Application.Features.Queries.GetAllCategory;
using KonsolcumApi.Application.RequestParameters;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Queries.Category.GetAllCategory
{
    public class GetAllCategoryQueryRequest : IRequest<GetAllCategoryQueryResponse>
    {
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 5;
    }
}
