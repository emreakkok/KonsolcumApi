using KonsolcumApi.Application.Consts;
using KonsolcumApi.Application.CustomAttributes;
using KonsolcumApi.Application.Enums;
using KonsolcumApi.Application.Features.Commands.Category.CreateCategory;
using KonsolcumApi.Application.Features.Commands.Category.RemoveCategory;
using KonsolcumApi.Application.Features.Commands.Category.UpdateCategory;
using KonsolcumApi.Application.Features.Commands.File.ChangeShowcaseFile;
using KonsolcumApi.Application.Features.Commands.File.ChangeShowcaseFileCategory;
using KonsolcumApi.Application.Features.Commands.File.RemoveFile;
using KonsolcumApi.Application.Features.Commands.File.UploadFile;
using KonsolcumApi.Application.Features.Queries.Category.GetActiveCategories;
using KonsolcumApi.Application.Features.Queries.Category.GetAllCategory;
using KonsolcumApi.Application.Features.Queries.Category.GetByIdCategory;
using KonsolcumApi.Application.Features.Queries.Category.GetProductsByCategory;
using KonsolcumApi.Application.Features.Queries.File.GetFile;
using KonsolcumApi.Application.Features.Queries.GetAllCategory;
using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Application.RequestParameters;
using KonsolcumApi.Application.Services;
using KonsolcumApi.Application.ViewModels.Categories;
using KonsolcumApi.Application.ViewModels.File;
using KonsolcumApi.Domain.Entities;
using KonsolcumApi.Domain.Entities.File;
using KonsolcumApi.Persistence.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KonsolcumApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class CategoriesController : ControllerBase
    {

        readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("GetActiveCategories")]
        public async Task<ActionResult> GetActiveCategories([FromQuery] GetActiveCategoriesQueryRequest getActiveCategoriesQueryRequest)
        {
            GetActiveCategoriesQueryResponse response = await _mediator.Send(getActiveCategoriesQueryRequest);
            return Ok(response);
        }

        [HttpGet("GetProductsByCategory")]
        public async Task<ActionResult> GetProductsByCategory([FromQuery] GetProductsByCategoryQueryRequest request)
        {
            GetProductsByCategoryQueryResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Categories, ActionType = ActionType.Reading, Definition = "Get All Categories")]
        public async Task<ActionResult> GetAllCategories([FromQuery] GetAllCategoryQueryRequest getAllCategoryQueryRequest)
        {
            GetAllCategoryQueryResponse response = await _mediator.Send(getAllCategoryQueryRequest);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] GetByIdCategoryQueryRequest getByIdCategoryQueryRequest)
        {
            GetByIdCategoryQueryResponse response = await _mediator.Send(getByIdCategoryQueryRequest);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Categories, ActionType = ActionType.Writing, Definition = "Create Category")]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] CreateCategoryCommandRequest createCategoryCommandRequest)
        {
            CreateCategoryCommandResponse response = await _mediator.Send(createCategoryCommandRequest);
            return Ok();
        }

        // PUT: api/Categories/{id} 
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Categories, ActionType = ActionType.Updating, Definition = "Update Category")]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryCommandRequest updateCategoryCommandRequest)
        {
            UpdateCategoryCommandResponse response = await _mediator.Send(updateCategoryCommandRequest);
            return Ok(response);
        }

        // DELETE: api/Categories/{id}
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Categories, ActionType = ActionType.Deleting, Definition = "Delete Category")]
        public async Task<IActionResult> DeleteCategory([FromRoute] RemoveCategoryCommandRequest removeCategoryCommandRequest)
        {
            RemoveCategoryCommandResponse response = await _mediator.Send(removeCategoryCommandRequest);
            return Ok();
        }

        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Categories, ActionType = ActionType.Writing, Definition = "Upload Images")]
        public async Task<IActionResult> UploadImages([FromQuery] string id, [FromForm] IFormFileCollection files)
        {
            var uploadFileCommandRequest = new UploadFileCommandRequest
            {
                id = id,
                type = "category",
                files = files
            };

            UploadFileCommandResponse response = await _mediator.Send(uploadFileCommandRequest);

            if (!response.Success)
                return BadRequest(response.Message);

            return Ok(new { Message = response.Message, Files = response.Files });
        }

        [HttpGet("{id}/images")]
        public async Task<IActionResult> GetCategoryImages([FromRoute] GetFileQueryRequest getFileQueryRequest)
        {
            GetFileQueryResponse response = await _mediator.Send(getFileQueryRequest);
            return Ok(response.Files); // response.Files zaten camelCase içinde olacak
        }


        [HttpDelete("delete-image/{imageId}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Categories, ActionType = ActionType.Deleting, Definition = "Delete Images")]
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
        [AuthorizeDefinition(Menu = AuthorizeDefinitionConstants.Categories, ActionType = ActionType.Updating, Definition = "Change Showcase File")]
        public async Task<IActionResult> ChangeShowcaseFile([FromQuery] ChangeShowcaseFileCategoryCommandRequest changeShowcaseFileCategoryCommandRequest)
        {
            ChangeShowcaseFileCategoryCommandResponse response = await _mediator.Send(changeShowcaseFileCategoryCommandRequest);
            return Ok(response);
        }
    }
}
