using KonsolcumApi.Application.Abstractions.Services;
using KonsolcumApi.Application.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Queries.Order.GetAllOrders
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQueryRequest, GetAllOrdersQueryResponse>
    {
        readonly IOrderService _orderService;

        public GetAllOrdersQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<GetAllOrdersQueryResponse> Handle(GetAllOrdersQueryRequest request, CancellationToken cancellationToken)
        {
            var orders = await _orderService.GetAllOrdersAsync(request.Page, request.Size);
            var totalCount = await _orderService.GetTotalOrderCountAsync();

            return new GetAllOrdersQueryResponse
            {
                Orders = orders,
                TotalOrderCount = totalCount
            };
        }
    }
}
