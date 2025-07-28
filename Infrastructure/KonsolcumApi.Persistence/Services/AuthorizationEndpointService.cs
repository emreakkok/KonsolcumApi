using Dapper;
using KonsolcumApi.Application.Abstractions.Services;
using KonsolcumApi.Application.Abstractions.Services.Configurations;
using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Domain.Entities;
using KonsolcumApi.Domain.Entities.Identity;
using KonsolcumApi.Persistence.Contexts.DapperContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace KonsolcumApi.Persistence.Services
{
    public class AuthorizationEndpointService : IAuthorizationEndpointService
    {
        private readonly Context _context;
        private readonly IApplicationService _applicationService;
        private readonly RoleManager<AppRole> _roleManager;

        public AuthorizationEndpointService(
            Context context,
            IApplicationService applicationService,
            RoleManager<AppRole> roleManager)
        {
            _context = context;
            _applicationService = applicationService;
            _roleManager = roleManager;
        }

        public async Task AssignRoleEndpointAsync(string[] roles, string menu, string code, Type type)
        {
            using var connection = _context.CreateConnection();
            await ((SqlConnection)connection).OpenAsync();
            using var transaction = connection.BeginTransaction();



            try
            {
                // Menu kontrolü ve eklenmesi
                var menuId = await GetOrCreateMenuAsync(connection, transaction, menu);

                // Endpoint kontrolü ve eklenmesi
                var endpointId = await GetOrCreateEndpointAsync(connection, transaction, code, menu, menuId, type);

                // Mevcut rolleri temizle (AppRoleEndpoint tablosundan)
                await ClearEndpointRolesAsync(connection, transaction, endpointId);

                // Yeni rolleri ata
                await AssignRolesToEndpointAsync(connection, transaction, endpointId, roles);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception($"Error in AssignRoleEndpointAsync: {ex.Message}", ex);
            }
        }

        private async Task<Guid> GetOrCreateMenuAsync(IDbConnection connection, IDbTransaction transaction, string menuName)
        {
            // Önce mevcut menüyü kontrol et
            var existingMenuSql = "SELECT Id FROM Menus WHERE Name = @Name";
            var existingMenuId = await connection.QueryFirstOrDefaultAsync<Guid?>(existingMenuSql, new { Name = menuName }, transaction);

            if (existingMenuId.HasValue)
                return existingMenuId.Value;

            // Menü yoksa oluştur
            var newMenuId = Guid.NewGuid();
            var insertMenuSql = @"
                INSERT INTO Menus (Id, Name, CreatedDate, UpdatedDate) 
                VALUES (@Id, @Name, @CreatedDate, @UpdatedDate)";

            await connection.ExecuteAsync(insertMenuSql, new
            {
                Id = newMenuId,
                Name = menuName,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            }, transaction);

            return newMenuId;
        }

        private async Task<Guid> GetOrCreateEndpointAsync(IDbConnection connection, IDbTransaction transaction, string code, string menuName, Guid menuId, Type type)
        {
            // Mevcut endpoint'i kontrol et
            var existingEndpointSql = @"
                SELECT e.Id 
                FROM Endpoints e 
                INNER JOIN Menus m ON e.MenuId = m.Id 
                WHERE e.Code = @Code AND m.Name = @MenuName";

            var existingEndpointId = await connection.QueryFirstOrDefaultAsync<Guid?>(existingEndpointSql,
                new { Code = code, MenuName = menuName }, transaction);

            if (existingEndpointId.HasValue)
                return existingEndpointId.Value;

            // Endpoint yoksa oluştur
            var action = _applicationService.GetAuthorizeDefinitionEndpoints(type)
                    .FirstOrDefault(m => m.Name == menuName)
                    ?.Actions.FirstOrDefault(e => e.Code == code);

            if (action == null)
                throw new InvalidOperationException($"Action with code '{code}' not found in menu '{menuName}'");

            var newEndpointId = Guid.NewGuid();
            var insertEndpointSql = @"
                INSERT INTO Endpoints (Id, Code, ActionType, HttpType, Definition, MenuId, CreatedDate, UpdatedDate) 
                VALUES (@Id, @Code, @ActionType, @HttpType, @Definition, @MenuId, @CreatedDate, @UpdatedDate)";

            await connection.ExecuteAsync(insertEndpointSql, new
            {
                Id = newEndpointId,
                Code = action.Code,
                ActionType = action.ActionType,
                HttpType = action.HttpType,
                Definition = action.Definition,
                MenuId = menuId,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            }, transaction);

            return newEndpointId;
        }

        private async Task ClearEndpointRolesAsync(IDbConnection connection, IDbTransaction transaction, Guid endpointId)
        {
            var deleteSql = "DELETE FROM AppRoleEndpoint WHERE EndpointsId = @EndpointId";
            await connection.ExecuteAsync(deleteSql, new { EndpointId = endpointId }, transaction);
        }

        private async Task AssignRolesToEndpointAsync(IDbConnection connection, IDbTransaction transaction, Guid endpointId, string[] roles)
        {
            if (roles == null || roles.Length == 0)
                return;

            // Rol ID'lerini getir
            var rolesSql = "SELECT Id FROM AspNetRoles WHERE Name IN @Names";
            var roleIds = await connection.QueryAsync<string>(rolesSql, new { Names = roles }, transaction);

            // Rolleri endpoint'e ata (AppRoleEndpoint tablosuna)
            var insertRolesSql = "INSERT INTO AppRoleEndpoint (EndpointsId, RolesId) VALUES (@EndpointId, @RoleId)";

            foreach (var roleId in roleIds)
            {
                await connection.ExecuteAsync(insertRolesSql, new
                {
                    EndpointId = endpointId,
                    RoleId = roleId
                }, transaction);
            }
        }

        public async Task<List<string>> GetRolesToEndpointAsync(string code, string menu)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                SELECT r.Name 
                FROM AspNetRoles r
                INNER JOIN AppRoleEndpoint are ON r.Id = are.RolesId
                INNER JOIN Endpoints e ON are.EndpointsId = e.Id
                INNER JOIN Menus m ON e.MenuId = m.Id
                WHERE e.Code = @Code AND m.Name = @Menu";

            var roles = await connection.QueryAsync<string>(sql, new { Code = code, Menu = menu });
            return roles.ToList();
        }



    }
}