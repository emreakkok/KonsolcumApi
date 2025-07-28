using KonsolcumApi.Application.Abstractions.Services;
using KonsolcumApi.Application.CustomAttributes;
using KonsolcumApi.Application.Enums;
using KonsolcumApi.Application.Features.Commands.AppUser.AssignRoleToUser;
using KonsolcumApi.Application.Features.Commands.AppUser.CreateUser;
using KonsolcumApi.Application.Features.Commands.AppUser.LoginUser;
using KonsolcumApi.Application.Features.Commands.AppUser.UpdatePassword;
using KonsolcumApi.Application.Features.Queries.AppUser.GetAllUsers;
using KonsolcumApi.Application.Features.Queries.AppUser.GetRolesToUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KonsolcumApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task <IActionResult> CreateUser(CreateUserCommandRequest createUserCommandRequest)
        {
            CreateUserCommandResponse response = await _mediator.Send(createUserCommandRequest);
            return Ok(response);
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordCommandRequest request)
        {
            UpdatePasswordCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes ="Admin")]
        [AuthorizeDefinition(ActionType = ActionType.Reading,Definition ="Get All Users", Menu = "Users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUsersQueryRequest request)
        {
            GetAllUsersQueryResponse response = await _mediator.Send(request);
            return Ok(response);
        }


        [HttpGet("get-roles-to-user/{UserId}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Get Roles To User", Menu = "Users")]
        public async Task<IActionResult> GetRolesToUser([FromRoute] GetRolesToUserQueryRequest request)
        {
            GetRolesToUserQueryResponse response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("assign-role-to-user")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Assign Role To User", Menu = "Users")]
        public async Task<IActionResult> AssignRoleToUser(AssignRoleToUserCommandRequest request)
        {
            AssignRoleToUserCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }


    }
}
