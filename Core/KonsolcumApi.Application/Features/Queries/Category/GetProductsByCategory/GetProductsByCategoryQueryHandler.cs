using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Application.ViewModels.Products;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Queries.Category.GetProductsByCategory
{
    public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQueryRequest, GetProductsByCategoryQueryResponse>
    {

        private readonly ICategoryRepository _categoryRepository;

        public GetProductsByCategoryQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<GetProductsByCategoryQueryResponse> Handle(GetProductsByCategoryQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Kategori ID'sinin geçerli olup olmadığını kontrol et
                if (request.CategoryId == Guid.Empty)
                {
                    return new GetProductsByCategoryQueryResponse
                    {
                        Success = false,
                        Message = "Geçersiz kategori ID'si."
                    };
                }

                // Kategori adını al
                var categoryName = await _categoryRepository.GetCategoryNameByIdAsync(request.CategoryId);
                if (string.IsNullOrEmpty(categoryName) || categoryName == "Bilinmeyen Kategori")
                {
                    return new GetProductsByCategoryQueryResponse
                    {
                        Success = false,
                        Message = "Kategori bulunamadı."
                    };
                }

                // Kategoriye ait ürünleri sayfalama ile al
                var (products, totalCount) = await _categoryRepository.GetProductsByCategoryPagedAsync(
                    request.CategoryId,
                    request.Page,
                    request.Size
                );

                // Domain nesnelerini view model'e dönüştür
                var productViewModels = products.Select(p => new VM_CategoryProduct
                {
                    Id = p.Id.ToString(),
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    IsActive = p.IsActive,
                    IsFeatured = p.IsFeatured,
                    CategoryId = p.CategoryId.ToString(),
                    CategoryName = p.Category?.Name ?? categoryName,
                    CreatedDate = p.CreatedDate,
                    // Resim yolunu tam URL olarak döndür
                    ShowcaseImagePath = !string.IsNullOrEmpty(p.ShowcaseImagePath)
                        ? $"https://localhost:7240/{p.ShowcaseImagePath}"
                        : null
                }).ToList();

                // Başarılı yanıt döndür
                return new GetProductsByCategoryQueryResponse
                {
                    Products = productViewModels,
                    TotalProductCount = totalCount,
                    CategoryName = categoryName,
                    Success = true,
                    Message = $"{categoryName} kategorisinde {totalCount} ürün bulundu."
                };
            }
            catch (Exception ex)
            {
                // Hata durumunda log kaydet (ILogger kullanabilirsiniz)
                return new GetProductsByCategoryQueryResponse
                {
                    Success = false,
                    Message = $"Ürünler getirilirken bir hata oluştu: {ex.Message}"
                };
            }
        }
    }
}
