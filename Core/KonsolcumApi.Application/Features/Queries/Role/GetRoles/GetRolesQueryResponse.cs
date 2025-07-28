using KonsolcumApi.Application.DTOs.Role;

namespace KonsolcumApi.Application.Features.Queries.Role.GetRoles
{
    public class GetRolesQueryResponse
    {
        public List<ListRole> Roles { get; set; }
        public int TotalCount { get; set; }
    }
    
}