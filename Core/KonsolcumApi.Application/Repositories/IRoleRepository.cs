using KonsolcumApi.Application.DTOs.Role;
using KonsolcumApi.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Repositories
{
    public interface IRoleRepository
    {
        Task<List<ListRole>> GetAllRolesAsync();
        Task<List<ListRole>> GetAllRolesPagedAsync(int page, int size);
        Task<int> GetTotalRoleCountAsync();
    }
}
