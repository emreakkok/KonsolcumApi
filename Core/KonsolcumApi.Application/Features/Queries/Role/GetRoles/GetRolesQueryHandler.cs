using KonsolcumApi.Application.Abstractions.Services;
using KonsolcumApi.Application.DTOs.Role;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Queries.Role.GetRoles
{
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQueryRequest, GetRolesQueryResponse>
    {
        readonly IRoleService _roleService;

        public GetRolesQueryHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<GetRolesQueryResponse> Handle(GetRolesQueryRequest request, CancellationToken cancellationToken)
        {
            List<ListRole> roles;
            int totalCount;

            // Eğer tüm roller isteniyorsa
            if (request.Page == -1 && request.Size == -1)
            {
                roles = await _roleService.GetAllRoles(-1, -1);
                totalCount = roles.Count; // Ayrı sorguya gerek yok
            }
            else
            {
                roles = await _roleService.GetAllRoles(request.Page, request.Size);
                totalCount = await _roleService.GetTotalRoleCountAsync();
            }

            return new GetRolesQueryResponse
            {
                Roles = roles,
                TotalCount = totalCount
            };
        }
    }
}
