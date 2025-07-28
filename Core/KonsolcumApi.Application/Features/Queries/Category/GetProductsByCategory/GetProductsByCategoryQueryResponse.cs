using KonsolcumApi.Application.ViewModels.Products;

namespace KonsolcumApi.Application.Features.Queries.Category.GetProductsByCategory
{
    public class GetProductsByCategoryQueryResponse
    {
        public List<VM_CategoryProduct> Products { get; set; } = new List<VM_CategoryProduct>();
        public int TotalProductCount { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}