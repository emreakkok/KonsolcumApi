using KonsolcumApi.Application.Consts;
using KonsolcumApi.Application.CustomAttributes;
using KonsolcumApi.Application.Enums;
using KonsolcumApi.Application.Features.Commands.Order.CompleteOrder;
using KonsolcumApi.Application.Features.Commands.Order.CreateOrder;
using KonsolcumApi.Application.Features.Queries.Order.GetAllOrders;
using KonsolcumApi.Application.Features.Queries.Order.GetMyOrders;
using KonsolcumApi.Application.Features.Queries.Order.GetOrderDetails;
using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Domain.Entities;
using KonsolcumApi.Persistence.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KonsolcumApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        readonly IMediator _mediator;

        public OrdersController(IOrderRepository orderRepository, IMediator mediator)
        {
            _orderRepository = orderRepository;
            _mediator = mediator;
        }

        [HttpPost]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Orders, ActionType = ActionType.Writing, Definition = "Create Order")]
        public async Task<IActionResult> CreateOrder(CreateOrderCommandRequest request)
        {
            CreateOrderCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        // GET: api/Orders
        [HttpGet]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Orders, ActionType = ActionType.Reading, Definition = "Get All Orders")]
        public async Task<ActionResult> GetAllOrders([FromQuery] GetAllOrdersQueryRequest request)
        {
            GetAllOrdersQueryResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("my-orders")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Orders, ActionType = ActionType.Reading, Definition = "Get My Orders")]
        public async Task<ActionResult> GetMyOrders([FromQuery] GetMyOrdersQueryRequest request)
        {
            GetMyOrdersQueryResponse response = await _mediator.Send(request);
            return Ok(response);
        }


        [HttpGet("order-details/{orderId}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Orders, ActionType = ActionType.Reading, Definition = "Get Order Details")]
        public async Task<IActionResult> GetOrderDetails([FromRoute] Guid orderId)
        {
            try
            {
                var request = new GetOrderDetailsQueryRequest { OrderId = orderId };
                GetOrderDetailsQueryResponse response = await _mediator.Send(request);

                if (response.Order == null)
                {
                    return NotFound(new { message = "Sipariş bulunamadı." });
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Hata loglamasını burada yapın, örneğin ILogger<OrdersController> kullanarak
                // Eğer bir logger enjekte ettiyseniz: _logger.LogError(ex, "Sipariş detayı getirilemedi: {OrderId}", orderId);
                Console.WriteLine($"HATA: OrdersController.GetOrderDetails - Sipariş ID: {orderId}");
                Console.WriteLine($"Mesaj: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                // Daha detaylı bilgi için InnerException mesajını da ekleyelim.
                return StatusCode(500, new { message = "Sipariş detayı getirilirken bir hata oluştu.", error = ex.Message, innerError = ex.InnerException?.Message });
            }
        }


        [HttpGet("complete-order/{Id}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Orders, ActionType = ActionType.Updating, Definition = "Complete Order")]
        public async Task<IActionResult> CompleteOrder([FromRoute] CompleteOrderCommandRequest request)
        {
            CompleteOrderCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }


        // DELETE: api/Orders/{id}
        [HttpDelete("{id}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Orders, ActionType = ActionType.Deleting, Definition = "Delete Order")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var success = await _orderRepository.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}
