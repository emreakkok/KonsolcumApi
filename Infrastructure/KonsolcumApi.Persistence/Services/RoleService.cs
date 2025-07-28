using KonsolcumApi.Application.Abstractions.Services;
using KonsolcumApi.Application.DTOs.Order;
using KonsolcumApi.Application.DTOs.Role;
using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Persistence.Services
{
    public class RoleService : IRoleService
    {
        readonly RoleManager<AppRole> _roleManager;
        readonly IRoleRepository _roleRepository;

        public RoleService(RoleManager<AppRole> roleManager, IRoleRepository roleRepository)
        {
            _roleManager = roleManager;
            _roleRepository = roleRepository;
        }

        public async Task<bool> CreateRole(string name)
        {
            IdentityResult result = await _roleManager.CreateAsync(new() { Id=Guid.NewGuid().ToString(), Name = name });
            return result.Succeeded;
        }

        public async Task<bool> DeleteRole(string id)
        {
            IdentityResult result = await _roleManager.DeleteAsync(new() {Id = id});
            return result.Succeeded;
        }

        public async Task<List<ListRole>> GetAllRoles(int page, int size)
        {
            if (page == -1 || size == -1)
            {
                return await _roleRepository.GetAllRolesAsync();
            }
            else
            {
                return await _roleRepository.GetAllRolesPagedAsync(page, size);
            }
        }

        public async Task<int> GetTotalRoleCountAsync()
        {
            return await _roleRepository.GetTotalRoleCountAsync();
        }
        public async Task<(string id, string name)> GetRoleById(string id)
        {
            string role = await _roleManager.GetRoleIdAsync(new() { Id = id });
            return(id,role);
        }

        public async Task<bool> UpdateRole(string id ,string name)
        {
            IdentityResult result = await _roleManager.UpdateAsync(new() { Id = id , Name = name});
            return result.Succeeded;
        }
    }
}
