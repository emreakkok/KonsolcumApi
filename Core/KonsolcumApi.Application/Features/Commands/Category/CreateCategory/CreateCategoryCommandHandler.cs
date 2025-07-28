using KonsolcumApi.Application.Abstractions.Hubs;
using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Commands.Category.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommandRequest, CreateCategoryCommandResponse>
    {
        readonly ICategoryRepository _categoryRepository;
        readonly ICategoryHubService _categoryHubService;

        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, ICategoryHubService categoryHubService)
        {
            _categoryRepository = categoryRepository;
            _categoryHubService = categoryHubService;
        }

        public async Task<CreateCategoryCommandResponse> Handle(CreateCategoryCommandRequest request, CancellationToken cancellationToken)
        {

            await _categoryRepository.CreateAsync(new()
            {
                Id = Guid.NewGuid(),
                Name = request.name,
                Description = request.description,
                IsActive = request.isActive,
                CreatedDate = DateTime.UtcNow
            });

            await _categoryHubService.CategoryAddedMessageAsync($"{request.name} isminde kategori eklenmiştir.");

            return new();
            
        }
    }
}
