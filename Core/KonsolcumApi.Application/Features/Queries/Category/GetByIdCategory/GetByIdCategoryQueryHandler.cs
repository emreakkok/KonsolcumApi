using KonsolcumApi.Application.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Queries.Category.GetByIdCategory
{
    public class GetByIdCategoryQueryHandler : IRequestHandler<GetByIdCategoryQueryRequest, GetByIdCategoryQueryResponse>
    {
        readonly ICategoryRepository _categoryRepository;

        public GetByIdCategoryQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<GetByIdCategoryQueryResponse> Handle(GetByIdCategoryQueryRequest request, CancellationToken cancellationToken)
        {
            Domain.Entities.Category category = await _categoryRepository.GetByIdAsync(request.id);
            return new()
            {
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive
            };
        }
    }
}
