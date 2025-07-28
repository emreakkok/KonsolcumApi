using KonsolcumApi.Application.Abstractions.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Queries.Order.GetMyOrders
{
    public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQueryRequest, GetMyOrdersQueryResponse>
    {

        readonly IOrderService _orderService;
        readonly IHttpContextAccessor _httpContextAccessor;

        public GetMyOrdersQueryHandler(IOrderService orderService, IHttpContextAccessor httpContextAccessor)
        {
            _orderService = orderService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetMyOrdersQueryResponse> Handle(GetMyOrdersQueryRequest request, CancellationToken cancellationToken)
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
            {
                Console.WriteLine("Kullanıcı oturum bilgisi yok.");

                throw new UnauthorizedAccessException("Kullanıcı bilgisi bulunamadı.");
            }

            var orders = await _orderService.GetMyOrdersAsync(userName, request.Page, request.Size);
            var totalCount = await _orderService.GetMyOrderCountAsync(userName);

            return new GetMyOrdersQueryResponse
            {
                Orders = orders,
                TotalOrderCount = totalCount,
                UserName = userName
            };
        }
    }
}
