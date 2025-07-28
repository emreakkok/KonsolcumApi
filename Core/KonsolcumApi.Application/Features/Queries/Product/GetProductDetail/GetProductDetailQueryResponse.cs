using KonsolcumApi.Application.DTOs.File;
using KonsolcumApi.Application.ViewModels.File;
using static KonsolcumApi.Application.Features.Queries.File.GetFile.GetFileQueryResponse;

namespace KonsolcumApi.Application.Features.Queries.Product.GetProductDetail
{
    public class GetProductDetailQueryResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ShowcaseImagePath { get; set; }
        public List<FileDTO> Images { get; set; } = new List<FileDTO>();
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}