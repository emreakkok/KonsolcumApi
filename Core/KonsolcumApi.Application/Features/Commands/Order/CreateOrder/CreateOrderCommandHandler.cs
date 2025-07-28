using KonsolcumApi.Application.Abstractions.Hubs;
using KonsolcumApi.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Commands.Order.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommandRequest, CreateOrderCommandResponse>
    {
        readonly IOrderService _orderService;
        readonly IBasketService _basketService;
        readonly IOrderHubService _orderHubService;

        public CreateOrderCommandHandler(IOrderService orderService, IBasketService basketService, IOrderHubService orderHubService)
        {
            _orderService = orderService;
            _basketService = basketService;
            _orderHubService = orderHubService;
        }

        public async Task<CreateOrderCommandResponse> Handle(CreateOrderCommandRequest request, CancellationToken cancellationToken)
        {
            var hasItems = await _basketService.HasItemsInBasketAsync();
            if (!hasItems)
            {
                throw new InvalidOperationException("Sepetinizde ürün bulunmamaktadır. Sipariş oluşturulamaz.");
            }

            var basket = _basketService.GetUserActiveBasketAsync;
            if (basket == null)
            {
                throw new InvalidOperationException("Aktif sepet bulunamadı.");
            }

            await _orderService.CreateOrderAsync(new()
            {
                ShippingAddress = request.ShippingAddress,
                BasketId = _basketService.GetUserActiveBasketAsync?.Id.ToString()

            });

            await _orderHubService.OrderAddedMessageAsync("Yeni bir sipariş var");

            return new();
        }
    }
}
