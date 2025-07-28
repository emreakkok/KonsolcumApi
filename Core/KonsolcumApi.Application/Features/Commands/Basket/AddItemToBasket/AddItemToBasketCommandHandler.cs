using KonsolcumApi.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Commands.Basket.AddItemToBasket
{
    public class AddItemToBasketCommandHandler : IRequestHandler<AddItemToBasketCommandRequest, AddItemToBasketCommandResponse>
    {
        readonly IBasketService _basketService;

        public AddItemToBasketCommandHandler(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task<AddItemToBasketCommandResponse> Handle(AddItemToBasketCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _basketService.AddItemToBasketAsync(new()
                {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                });

                return new();
            }
            catch (UnauthorizedAccessException)
            {
                // Giriş yapılmamışsa, controller bu exception'ı yakalayıp 401 dönecek
                throw;
            }
        }
    }
}
