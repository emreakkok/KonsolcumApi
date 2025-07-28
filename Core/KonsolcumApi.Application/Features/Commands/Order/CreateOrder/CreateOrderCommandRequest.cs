using MediatR;

namespace KonsolcumApi.Application.Features.Commands.Order.CreateOrder
{
    public class CreateOrderCommandRequest : IRequest<CreateOrderCommandResponse>
    {
        public string ShippingAddress { get; set; }
    }
}