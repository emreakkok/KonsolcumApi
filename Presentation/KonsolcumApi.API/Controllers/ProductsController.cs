using KonsolcumApi.Application.Consts;
using KonsolcumApi.Application.CustomAttributes;
using KonsolcumApi.Application.Enums;
using KonsolcumApi.Application.Features.Commands.File.ChangeShowcaseFile;
using KonsolcumApi.Application.Features.Commands.File.RemoveFile;
using KonsolcumApi.Application.Features.Commands.File.UploadFile;
using KonsolcumApi.Application.Features.Commands.Product.CreateProduct;
using KonsolcumApi.Application.Features.Commands.Product.UpdateProduct;
using KonsolcumApi.Application.Features.Queries.Category.GetActiveCategories;
using KonsolcumApi.Application.Features.Queries.File.GetFile;
using KonsolcumApi.Application.Features.Queries.Product.GetActiveProducts;
using KonsolcumApi.Application.Features.Queries.Product.GetAllProduct;
using KonsolcumApi.Application.Features.Queries.Product.GetProductDetail;
using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Application.ViewModels.Products;
using KonsolcumApi.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KonsolcumApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ProductsController : ControllerBase
    {
        readonly private IProductRepository _productRepository;
        readonly IMediator _mediator;

        public ProductsController(IProductRepository productRepository, IMediator mediator)
        {
            _productRepository = productRepository;
            _mediator = mediator;
        }

        [HttpGet("GetActiveProducts")]
        public async Task<ActionResult> GetActiveProducts([FromQuery] GetActiveProductsQueryRequest getActiveProductsQueryRequest)
        {
            GetActiveProductsQueryResponse response = await _mediator.Send(getActiveProductsQueryRequest);
            return Ok(response);
        }

        [HttpGet("{id}/detail")]
        public async Task<ActionResult> GetProductDetail([FromRoute] string id)
        {
            var request = new GetProductDetailQueryRequest { ProductId = id };
            var response = await _mediator.Send(request);

            if (!response.Success)
                return NotFound(response.Message);

            return Ok(response);
        }
        // GET: api/Products
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Products, ActionType = ActionType.Reading, Definition = "Get All Products")]
        public async Task<ActionResult> GetAllProducts([FromQuery]GetAllProductQueryRequest getAllProductQueryRequest)
        {
            GetAllProductQueryResponse response = await _mediator.Send(getAllProductQueryRequest);
            return Ok(response);
        }


        // GET: api/Products/{id}
        [HttpGet("{id}")] // Ürünü ID'ye göre getirme action'ı
        public async Task<IActionResult> GetProductById([FromRoute] string id)
        {

            if (!Guid.TryParse(id, out Guid productId))
            {
                return BadRequest("Geçersiz ürün ID formatı.");
            }
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound("Ürün bulunamadı.");
            }
            return Ok(product);
        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Products, ActionType = ActionType.Writing, Definition = "Create Product")]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] CreateProductCommandRequest createProductCommandRequest)
        {
            CreateProductCommandResponse response = await _mediator.Send(createProductCommandRequest);
            return Ok();
        }


        // PUT: api/Products/{id}
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Products, ActionType = ActionType.Updating, Definition = "Update Product")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommandRequest updateProductCommandRequest)
        {

            if (updateProductCommandRequest == null || !Guid.TryParse(updateProductCommandRequest.Id, out Guid requestId) || requestId == Guid.Empty)
            {
                return BadRequest("Geçersiz ürün ID'si.");
            }

            UpdateProductCommandResponse response = await _mediator.Send(updateProductCommandRequest);

            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                if (response.Message == "Ürün bulunamadı.")
                {
                    return NotFound(response.Message); // 404 Not Found
                }
                return BadRequest(response.Message); // Diğer hatalar için 400 Bad Request
            }
        }

        // DELETE: api/Products/{id}
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Products, ActionType = ActionType.Deleting, Definition = "Delete Product")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var success = await _productRepository.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Products, ActionType = ActionType.Writing, Definition = "Upload Images")]
        public async Task<IActionResult> UploadImages([FromQuery] string id, [FromForm] IFormFileCollection files)
        {
            var uploadFileCommandRequest = new UploadFileCommandRequest
            {
                id = id,
                type = "product",
                files = files
            };

            UploadFileCommandResponse response = await _mediator.Send(uploadFileCommandRequest);

            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(new { Message = response.Message, Files = response.Files });
        }

        [HttpGet("{id}/images")]
        public async Task<IActionResult> GetProductImages([FromRoute] GetFileQueryRequest getFileQueryRequest)
        {
            GetFileQueryResponse response = await _mediator.Send(getFileQueryRequest);
            return Ok(response.Files); // response.Files zaten camelCase içinde olacak
        }


        [HttpDelete("delete-image/{imageId}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Products, ActionType = ActionType.Deleting, Definition = "Delete Image")]
        public async Task<IActionResult> DeleteImage([FromRoute] RemoveFileCommandRequest removeFileCommandRequest)
        {
            RemoveFileCommandResponse response = await _mediator.Send(removeFileCommandRequest);

            if (!response.Success && response.Message == "Image not found.")
                return NotFound(response.Message);

            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(new { response.Message });
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Products, ActionType = ActionType.Updating, Definition = "Change Showcase File")]
        public async Task<IActionResult> ChangeShowcaseFile([FromQuery] ChangeShowcaseFileCommandRequest changeShowcaseFileCommandRequest)
        {
            ChangeShowcaseFileCommandResponse response = await _mediator.Send(changeShowcaseFileCommandRequest);
            return Ok(response);
        }

    }
}
