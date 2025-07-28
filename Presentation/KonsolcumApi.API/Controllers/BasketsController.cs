using KonsolcumApi.Application.Consts;
using KonsolcumApi.Application.CustomAttributes;
using KonsolcumApi.Application.Enums;
using KonsolcumApi.Application.Features.Commands.Basket.AddItemToBasket;
using KonsolcumApi.Application.Features.Commands.Basket.RemoveBasketItem;
using KonsolcumApi.Application.Features.Commands.Basket.UpdateQuantity;
using KonsolcumApi.Application.Features.Queries.Basket.GetBasketItems;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KonsolcumApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]
    // Program.cs'teki scheme ile eşleşmeli
    public class BasketsController : ControllerBase
    {
        readonly IMediator _mediator;

        public BasketsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Baskets, ActionType = ActionType.Reading,Definition = "Get Basket Items")]
        public async Task<IActionResult> GetBasketItems([FromQuery] GetBasketItemsQueryRequest getBasketItemsQueryRequest)
        {
            try
            {
                List<GetBasketItemsQueryResponse> response = await _mediator.Send(getBasketItemsQueryRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Baskets, ActionType = ActionType.Writing, Definition = "Add Item To Basket")]
        public async Task<IActionResult> AddItemToBasket([FromBody] AddItemToBasketCommandRequest addItemToBasketCommandRequest)
        {
            try
            {
                AddItemToBasketCommandResponse response = await _mediator.Send(addItemToBasketCommandRequest);
                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Sepete ürün eklemek için giriş yapmalısınız." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Baskets, ActionType = ActionType.Updating, Definition = "Update Quantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityCommandRequest updateQuantityCommandRequest)
        {
            try
            {
                // Input validation
                if (string.IsNullOrEmpty(updateQuantityCommandRequest.BasketItemId))
                    return BadRequest(new { message = "Sepet öğesi ID'si gereklidir" });

                if (updateQuantityCommandRequest.Quantity <= 0)
                    return BadRequest(new { message = "Miktar 0'dan büyük olmalıdır" });

                UpdateQuantityCommandResponse response = await _mediator.Send(updateQuantityCommandRequest);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Detaylı log ekle
                Console.WriteLine($"UpdateQuantity Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{BasketItemId}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Baskets, ActionType = ActionType.Deleting, Definition = "Remove Basket Item")]
        public async Task<IActionResult> RemoveBasketItem([FromRoute] RemoveBasketItemCommandRequest removeBasketItemCommandRequest)
        {
            try
            {
                RemoveBasketItemCommandResponse response = await _mediator.Send(removeBasketItemCommandRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}