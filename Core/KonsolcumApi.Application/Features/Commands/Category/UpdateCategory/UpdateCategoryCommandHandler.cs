using KonsolcumApi.Application.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Commands.Category.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommandRequest, UpdateCategoryCommandResponse>
    {
        readonly ICategoryRepository _categoryRepository;
        readonly ILogger<UpdateCategoryCommandHandler> _logger;
        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, ILogger<UpdateCategoryCommandHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<UpdateCategoryCommandResponse> Handle(UpdateCategoryCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var categoryId = Guid.Parse(request.id);

                var category = await _categoryRepository.GetByIdAsync(categoryId);

                if (category == null)
                {
                    _logger.LogWarning($"Kategori bulunamadı: ID {request.id}");
                    return new UpdateCategoryCommandResponse { Success = false, Message = "Kategori bulunamadı." };
                }

                // Güncelleme
                category.Name = request.name;
                category.Description = request.description;
                category.IsActive = request.isActive;
                category.UpdatedDate = DateTime.UtcNow;

                await _categoryRepository.UpdateAsync(category);
                _logger.LogInformation($"Kategori güncellendi: ID {request.id}");

                return new UpdateCategoryCommandResponse { Success = true, Message = "Kategori başarıyla güncellendi." };
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, $"Kategori ID formatı geçersiz: {request.id}");
                return new UpdateCategoryCommandResponse { Success = false, Message = "Geçersiz kategori ID formatı." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Kategori güncellenirken bir hata oluştu: ID {request.id}");
                return new UpdateCategoryCommandResponse { Success = false, Message = $"Kategori güncellenirken beklenmeyen bir hata oluştu: {ex.Message}" };
            }
        }
    }
    
}
