using KonsolcumApi.Application.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Commands.File.ChangeShowcaseFile
{
    public class ChangeShowcaseFileCommandHandler : IRequestHandler<ChangeShowcaseFileCommandRequest, ChangeShowcaseFileCommandResponse>
    {
        private readonly IFileRepository _fileRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ChangeShowcaseFileCommandHandler> _logger;

        public ChangeShowcaseFileCommandHandler(
            IFileRepository fileRepository,
            IProductRepository productRepository,
            ILogger<ChangeShowcaseFileCommandHandler> logger)
        {
            _fileRepository = fileRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ChangeShowcaseFileCommandResponse> Handle(ChangeShowcaseFileCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Önce belirtilen imageId'nin geçerli olup olmadığını kontrol et
                var targetImage = await _fileRepository.GetByIdAsync(Guid.Parse(request.ImageId));
                if (targetImage == null)
                {
                    return new ChangeShowcaseFileCommandResponse
                    {
                        Success = false,
                        Message = "Belirtilen resim bulunamadı."
                    };
                }

                // Product ID kontrolü
                var product = await _productRepository.GetByIdAsync(Guid.Parse(request.ProductId));
                if (product == null)
                {
                    return new ChangeShowcaseFileCommandResponse
                    {
                        Success = false,
                        Message = "Belirtilen ürün bulunamadı."
                    };
                }

                // Bu ürüne ait tüm resimlerin showcase durumunu false yap
                var productImages = await _fileRepository.GetFilesByProductIdAsync(Guid.Parse(request.ProductId));
                foreach (var image in productImages)
                {
                    image.Showcase = false;
                    await _fileRepository.UpdateAsync(image);
                }

                // Seçilen resmi showcase olarak işaretle
                targetImage.Showcase = true;
                await _fileRepository.UpdateAsync(targetImage);

                // Product tablosundaki ShowcaseImagePath'i güncelle
                product.ShowcaseImagePath = targetImage.Path;
                await _productRepository.UpdateAsync(product);

                _logger.LogInformation($"Product {request.ProductId} için showcase resim {request.ImageId} olarak değiştirildi.");

                return new ChangeShowcaseFileCommandResponse
                {
                    Success = true,
                    Message = "Vitrin resmi başarıyla güncellendi."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Showcase resim değiştirme işlemi sırasında hata oluştu.");
                return new ChangeShowcaseFileCommandResponse
                {
                    Success = false,
                    Message = "Vitrin resmi güncellenirken bir hata oluştu."
                };
            }
        }
    }
}