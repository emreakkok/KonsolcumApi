using Dapper;
using KonsolcumApi.Application.DTOs.Role;
using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Persistence.Contexts.DapperContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        protected readonly Context _context;

        public RoleRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<ListRole>> GetAllRolesAsync()
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                SELECT 
                    Id,
                    Name,
                    NormalizedName,
                    ConcurrencyStamp
                FROM AspNetRoles
                ORDER BY Name;";

            var roles = await connection.QueryAsync<ListRole>(sql);
            return roles.ToList();
        }

        public async Task<List<ListRole>> GetAllRolesPagedAsync(int page, int size)
        {
            using var connection = _context.CreateConnection();

            // -1 ile tüm rolleri döndür
            if (page == -1 && size == -1)
            {
                var allSql = @"
            SELECT 
                Id,
                Name,
                NormalizedName,
                ConcurrencyStamp
            FROM AspNetRoles
            ORDER BY Name;";
                var allRoles = await connection.QueryAsync<ListRole>(allSql);
                return allRoles.ToList();
            }

            var offset = page * size;

            var sql = @"
        SELECT 
            Id,
            Name,
            NormalizedName,
            ConcurrencyStamp
        FROM AspNetRoles
        ORDER BY Name
        OFFSET @Offset ROWS FETCH NEXT @Size ROWS ONLY;";

            var roles = await connection.QueryAsync<ListRole>(sql, new { Offset = offset, Size = size });
            return roles.ToList();
        }

        public async Task<int> GetTotalRoleCountAsync()
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT COUNT(*) FROM AspNetRoles";
            return await connection.ExecuteScalarAsync<int>(sql);
        }
    }
}