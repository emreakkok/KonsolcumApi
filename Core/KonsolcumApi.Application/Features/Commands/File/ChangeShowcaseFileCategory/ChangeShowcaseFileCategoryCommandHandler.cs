using KonsolcumApi.Application.Features.Commands.File.ChangeShowcaseFile;
using KonsolcumApi.Application.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Commands.File.ChangeShowcaseFileCategory
{
    public class ChangeShowcaseFileCategoryCommandHandler : IRequestHandler<ChangeShowcaseFileCategoryCommandRequest, ChangeShowcaseFileCategoryCommandResponse>
    {
        private readonly IFileRepository _fileRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<ChangeShowcaseFileCategoryCommandHandler> _logger;

        public ChangeShowcaseFileCategoryCommandHandler(IFileRepository fileRepository, ICategoryRepository categoryRepository, ILogger<ChangeShowcaseFileCategoryCommandHandler> logger)
        {
            _fileRepository = fileRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<ChangeShowcaseFileCategoryCommandResponse> Handle(ChangeShowcaseFileCategoryCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Önce belirtilen imageId'nin geçerli olup olmadığını kontrol et
                var targetImage = await _fileRepository.GetByIdAsync(Guid.Parse(request.ImageId));
                if (targetImage == null)
                {
                    return new ChangeShowcaseFileCategoryCommandResponse
                    {
                        Success = false,
                        Message = "Belirtilen resim bulunamadı."
                    };
                }

                // Product ID kontrolü
                var category = await _categoryRepository.GetByIdAsync(Guid.Parse(request.CategoryId));
                if (category == null)
                {
                    return new ChangeShowcaseFileCategoryCommandResponse
                    {
                        Success = false,
                        Message = "Belirtilen kategori bulunamadı."
                    };
                }

                // Bu ürüne ait tüm resimlerin showcase durumunu false yap
                var categoryImages = await _fileRepository.GetFilesByCategoryIdAsync(Guid.Parse(request.CategoryId));
                foreach (var image in categoryImages)
                {
                    image.Showcase = false;
                    await _fileRepository.UpdateAsync(image);
                }

                // Seçilen resmi showcase olarak işaretle
                targetImage.Showcase = true;
                await _fileRepository.UpdateAsync(targetImage);

                // Product tablosundaki ShowcaseImagePath'i güncelle
                category.ShowcaseImagePath = targetImage.Path;
                await _categoryRepository.UpdateAsync(category);

                _logger.LogInformation($"Kategori {request.CategoryId} için showcase resim {request.ImageId} olarak değiştirildi.");

                return new ChangeShowcaseFileCategoryCommandResponse
                {
                    Success = true,
                    Message = "Vitrin resmi başarıyla güncellendi."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Showcase resim değiştirme işlemi sırasında hata oluştu.");
                return new ChangeShowcaseFileCategoryCommandResponse
                {
                    Success = false,
                    Message = "Vitrin resmi güncellenirken bir hata oluştu."
                };
            }
        }
    }
}
