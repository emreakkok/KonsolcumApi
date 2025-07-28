using KonsolcumApi.Application.DTOs.File;
using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Application.ViewModels.File;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KonsolcumApi.Application.Features.Queries.File.GetFile.GetFileQueryResponse;

namespace KonsolcumApi.Application.Features.Queries.Product.GetProductDetail
{
    public class GetProductDetailQueryHandler : IRequestHandler<GetProductDetailQueryRequest, GetProductDetailQueryResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileRepository _fileRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<GetProductDetailQueryHandler> _logger;

        public GetProductDetailQueryHandler(
            IProductRepository productRepository,
            IFileRepository fileRepository,
            ICategoryRepository categoryRepository,
            ILogger<GetProductDetailQueryHandler> logger)
        {
            _productRepository = productRepository;
            _fileRepository = fileRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<GetProductDetailQueryResponse> Handle(GetProductDetailQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (!Guid.TryParse(request.ProductId, out var productId))
                {
                    return new GetProductDetailQueryResponse
                    {
                        Success = false,
                        Message = "Geçersiz ürün ID'si."
                    };
                }

                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return new GetProductDetailQueryResponse
                    {
                        Success = false,
                        Message = "Ürün bulunamadı."
                    };
                }

                // Kategori bilgisini al
                var category = await _categoryRepository.GetByIdAsync(product.CategoryId);

                // Ürün resimlerini al
                var productImages = await _fileRepository.GetFilesByProductIdAsync(productId);

                // Response'u hazırla
                var response = new GetProductDetailQueryResponse
                {
                    Id = product.Id.ToString(),
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    IsActive = product.IsActive,
                    IsFeatured = product.IsFeatured,
                    CategoryId = product.CategoryId.ToString(),
                    CategoryName = category?.Name ?? "Bilinmeyen Kategori",
                    CreatedDate = product.CreatedDate,
                    UpdatedDate = product.UpdatedDate,
                    ShowcaseImagePath = product.ShowcaseImagePath,
                    Images = productImages.Select(img => new FileDTO
                    {
                        Id = img.Id.ToString(),
                        FileName = img.FileName,
                        Path = img.Path,
                        Showcase = img.Showcase,
                        CreatedDate = img.CreatedDate,
                        UpdatedDate = img.UpdatedDate
                    }).OrderByDescending(img => img.Showcase).ThenBy(img => img.CreatedDate).ToList(),
                    Success = true,
                    Message = "Ürün detayları başarıyla getirildi."
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ürün detayları getirilirken hata oluştu. ProductId: {ProductId}", request.ProductId);
                return new GetProductDetailQueryResponse
                {
                    Success = false,
                    Message = "Ürün detayları getirilirken bir hata oluştu."
                };
            }
        }
    }
}
