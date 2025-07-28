using KonsolcumApi.Application.Abstractions.Hubs;
using KonsolcumApi.Application.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Commands.Product.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommandRequest, CreateProductCommandResponse>
    {
        readonly IProductRepository _productRepository;
        readonly IProductHubService _productHubService;

        public CreateProductCommandHandler(IProductRepository productRepository, IProductHubService productHubService)
        {
            _productRepository = productRepository;
            _productHubService = productHubService;
        }

        public async Task<CreateProductCommandResponse> Handle(CreateProductCommandRequest request, CancellationToken cancellationToken)
        {

            Guid categoryGuid = Guid.Parse(request.categoryId);


            await _productRepository.CreateAsync(new()
            {
                Id = Guid.NewGuid(),
                Name = request.name,
                Description = request.description,
                Price = request.price,
                StockQuantity = request.stockQuantity,
                IsActive = request.isActive,
                IsFeatured = request.isFeatured,
                CategoryId = categoryGuid
            });

            await _productHubService.ProductAddedMessageAsync($"{request.name} isminde ürün eklenmiştir.");

            return new();

        }
    }
}
