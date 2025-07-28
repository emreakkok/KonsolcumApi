using MediatR;

namespace KonsolcumApi.Application.Features.Commands.Product.UpdateProduct
{
    public class UpdateProductCommandRequest : IRequest<UpdateProductCommandResponse>
    {
        public string Id { get; set; } // Ürün ID'si
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public string CategoryId { get; set; } // Kategori ID'si
    }
}