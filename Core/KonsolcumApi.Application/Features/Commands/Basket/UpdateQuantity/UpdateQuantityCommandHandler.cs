using KonsolcumApi.Application.Abstractions.Services;
using KonsolcumApi.Application.ViewModels.Baskets;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Commands.Basket.UpdateQuantity
{
    public class UpdateQuantityCommandHandler : IRequestHandler<UpdateQuantityCommandRequest, UpdateQuantityCommandResponse>
    {
        readonly IBasketService _basketService;

        public UpdateQuantityCommandHandler(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task<UpdateQuantityCommandResponse> Handle(UpdateQuantityCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Request validation
                if (string.IsNullOrEmpty(request.BasketItemId))
                    throw new ArgumentException("BasketItemId gereklidir");

                if (request.Quantity <= 0)
                    throw new ArgumentException("Miktar 0'dan büyük olmalıdır");

                // ViewModel oluştur
                var basketItem = new VM_Update_BasketItem
                {
                    BasketItemId = request.BasketItemId,
                    Quantity = request.Quantity
                };

                // Service'i çağır
                await _basketService.UpdateQuantityAsync(basketItem);

                return new UpdateQuantityCommandResponse
                {
                    Success = true,
                    Message = "Miktar başarıyla güncellendi"
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Miktar güncellenirken hata oluştu: {ex.Message}");
            }
        }
    }
}
