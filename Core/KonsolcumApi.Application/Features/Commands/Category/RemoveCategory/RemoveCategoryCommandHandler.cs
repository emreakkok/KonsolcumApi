using KonsolcumApi.Application.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Commands.Category.RemoveCategory
{
    public class RemoveCategoryCommandHandler : IRequestHandler<RemoveCategoryCommandRequest, RemoveCategoryCommandResponse>
    {
        readonly ICategoryRepository _categoryRepository;
        public RemoveCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<RemoveCategoryCommandResponse> Handle(RemoveCategoryCommandRequest request, CancellationToken cancellationToken)
        {
            var categoryId = Guid.Parse(request.id);

            // Sorgusuz silme
            await _categoryRepository.DeleteAsync(categoryId);

            return new();
        }
    }
}
