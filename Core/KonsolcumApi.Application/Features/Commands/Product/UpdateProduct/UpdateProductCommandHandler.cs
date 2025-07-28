using KonsolcumApi.Application.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Commands.Product.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommandRequest, UpdateProductCommandResponse>
    {
        readonly IProductRepository _productRepository;
        readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(IProductRepository productRepository, ILogger<UpdateProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }
        public async Task<UpdateProductCommandResponse> Handle(UpdateProductCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // ID'yi Guid'e dönüştür
                if (!Guid.TryParse(request.Id, out Guid productId))
                {
                    _logger.LogWarning($"Geçersiz ürün ID formatı: {request.Id}");
                    return new UpdateProductCommandResponse { Success = false, Message = "Geçersiz ürün ID formatı." };
                }

                // Mevcut ürünü getir
                var product = await _productRepository.GetByIdAsync(productId);

                if (product == null)
                {
                    _logger.LogWarning($"Ürün bulunamadı: ID {request.Id}");
                    return new UpdateProductCommandResponse { Success = false, Message = "Ürün bulunamadı." };
                }

                // Kategori ID'sini Guid'e dönüştür
                if (!Guid.TryParse(request.CategoryId, out Guid categoryId))
                {
                    _logger.LogWarning($"Geçersiz kategori ID formatı: {request.CategoryId}");
                    return new UpdateProductCommandResponse { Success = false, Message = "Geçersiz kategori ID formatı." };
                }

                // Ürün özelliklerini güncelle
                product.Name = request.Name;
                product.Description = request.Description;
                product.Price = request.Price;
                product.StockQuantity = request.StockQuantity;
                product.IsActive = request.IsActive;
                product.IsFeatured = request.IsFeatured;
                product.CategoryId = categoryId; // Kategori ID'sini ata
                product.UpdatedDate = DateTime.UtcNow; // Güncelleme tarihini ayarla

                await _productRepository.UpdateAsync(product);
                _logger.LogInformation($"Ürün güncellendi: ID {request.Id}");

                return new UpdateProductCommandResponse { Success = true, Message = "Ürün başarıyla güncellendi." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ürün güncellenirken bir hata oluştu: ID {request.Id}");
                return new UpdateProductCommandResponse { Success = false, Message = $"Ürün güncellenirken beklenmeyen bir hata oluştu: {ex.Message}" };
            }
        }
    }
}
