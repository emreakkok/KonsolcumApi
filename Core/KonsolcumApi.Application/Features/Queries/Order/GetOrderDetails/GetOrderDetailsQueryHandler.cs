using KonsolcumApi.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Queries.Order.GetOrderDetails
{
    public class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQueryRequest, GetOrderDetailsQueryResponse>
    {
        readonly IOrderService _orderService;

        public GetOrderDetailsQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<GetOrderDetailsQueryResponse> Handle(GetOrderDetailsQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _orderService.GetOrderDetailAsync(request.OrderId);

                return new GetOrderDetailsQueryResponse
                {
                    Order = order
                };
            }
            catch (Exception ex)
            {
                // Hata loglamasını burada yapın.
                Console.WriteLine($"\n--- HATA: GetOrderDetailsQueryHandler ---");
                Console.WriteLine($"Sipariş ID: {request.OrderId}");
                Console.WriteLine($"Mesaj: {ex.Message}");
                Console.WriteLine($"Inner Exception Mesajı: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: \n{ex.StackTrace}\n");

                throw new ApplicationException($"Sipariş detayı alınamadı: {ex.Message}", ex);
            }
        }
    }
    
}
