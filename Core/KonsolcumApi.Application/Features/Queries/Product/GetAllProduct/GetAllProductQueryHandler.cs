using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Application.ViewModels.Products;
using KonsolcumApi.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Queries.Product.GetAllProduct
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQueryRequest, GetAllProductQueryResponse>
    {
        readonly IProductRepository _productRepository;
        readonly ILogger<GetAllProductQueryHandler> _logger;
        readonly IConfiguration _configuration;

        public GetAllProductQueryHandler(IProductRepository productRepository, ILogger<GetAllProductQueryHandler> logger, IConfiguration configuration)
        {
            _productRepository = productRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<GetAllProductQueryResponse> Handle(GetAllProductQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Get All Products");

                var totalProductCount = await _productRepository.GetTotalProductCountAsync();
                var products = await _productRepository.GetAllPagedAsync(request.Page, request.Size);
                var baseUrl = _configuration["BaseUrl"];

                var result = products.Select(p => new VM_Update_Product
                {
                    id = p.Id.ToString(),
                    name = p.Name,
                    description = p.Description,
                    price = p.Price,
                    stockQuantity = p.StockQuantity,
                    isActive = p.IsActive,
                    isFeatured = p.IsFeatured,
                    categoryId = p.CategoryId.ToString(),
                    categoryName = p.Category?.Name ?? "",
                    createdDate = p.CreatedDate,
                    updatedDate = p.UpdatedDate,
                    showcaseImagePath = string.IsNullOrWhiteSpace(p.ShowcaseImagePath)
                        ? $"{baseUrl}/assets/default-product.png" // Eğer product.ShowcaseImagePath boşsa veya null ise varsayılan resmi kullan
    :                   $"{baseUrl}/{p.ShowcaseImagePath}"
                }).ToList();

                return new()
                {
                    Products = result,
                    TotalProductCount = totalProductCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürünler alınırken hata oluştu.");
                throw; // Hata mesajını client'a ulaştırmak için tekrar fırlat
            }
        }
    }
}
