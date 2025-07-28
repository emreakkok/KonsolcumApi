using KonsolcumApi.Application.Features.Commands.AuthorizationEndpoint.AssignRoleEndpoint;
using KonsolcumApi.Application.Features.Queries.AuthorizationEndpoint.GetRolesToEndpoint;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KonsolcumApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationEndpointsController : ControllerBase
    {
        readonly IMediator _mediator;

        public AuthorizationEndpointsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetRolesToEndpoint(GetRolesToEndpointQueryRequest rolesToEndpointQueryRequest)
        {
            GetRolesToEndpointQueryResponse response = await _mediator.Send(rolesToEndpointQueryRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleEndpoint(AssignRoleEndpointCommandRequest assignRoleEndpointCommandRequest)
        {
            assignRoleEndpointCommandRequest.Type = typeof(Program);

            try
            {
                var response = await _mediator.Send(assignRoleEndpointCommandRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log hatayı
                return StatusCode(500, ex.Message + " | " + ex.InnerException?.Message);
            }
        }
    }
}
