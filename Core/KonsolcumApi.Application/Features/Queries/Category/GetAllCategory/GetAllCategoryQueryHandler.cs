using KonsolcumApi.Application.Features.Queries.GetAllCategory;
using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Application.RequestParameters;
using KonsolcumApi.Application.ViewModels.Categories;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Queries.Category.GetAllCategory
{
    public class GetAllCategoryQueryHandler : IRequestHandler<GetAllCategoryQueryRequest, GetAllCategoryQueryResponse>
    {
        readonly ICategoryRepository _categoryRepository;
        readonly ILogger<GetAllCategoryQueryHandler> _logger;
        readonly IConfiguration _configuration;

        public GetAllCategoryQueryHandler(ICategoryRepository categoryRepository, ILogger<GetAllCategoryQueryHandler> logger, IConfiguration configuration)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<GetAllCategoryQueryResponse> Handle(GetAllCategoryQueryRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Aktif kategoriler getiriliyor");

            var totalCategoryCount = await _categoryRepository.GetTotalCategoryCountAsync();
            var categories = await _categoryRepository.GetAllPagedAsync(request.Page, request.Size);
            var baseUrl = _configuration["BaseUrl"];

            var result = categories.Select(c => new VM_Update_Category
            {
                id = c.Id.ToString(),
                name = c.Name,
                description = c.Description,
                isActive = c.IsActive,
                showcaseImagePath = string.IsNullOrWhiteSpace(c.ShowcaseImagePath)
                ? $"{baseUrl}/assets/default-category.png"
                : $"{baseUrl}/{c.ShowcaseImagePath}"
            }).ToList();

            return new GetAllCategoryQueryResponse
            {
                TotalCategoryCount = totalCategoryCount,
                Categories = result
            };
        }

    }
}
