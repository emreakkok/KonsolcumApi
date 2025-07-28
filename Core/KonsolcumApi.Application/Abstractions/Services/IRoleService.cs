
using KonsolcumApi.Application.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Abstractions.Services
{
    public interface IRoleService
    {
        Task<List<ListRole>> GetAllRoles(int page, int size);
        Task<int> GetTotalRoleCountAsync();
        Task<(string id, string name)> GetRoleById(string id);
        Task<bool> CreateRole(string name);
        Task<bool> DeleteRole(string id);
        Task<bool> UpdateRole(string id, string name);

    }
}
