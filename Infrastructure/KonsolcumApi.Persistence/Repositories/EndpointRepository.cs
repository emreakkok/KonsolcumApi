using Dapper;
using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Domain.Entities;
using KonsolcumApi.Domain.Entities.Identity;
using KonsolcumApi.Persistence.Contexts.DapperContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Persistence.Repositories
{
    public class EndpointRepository : Repository<Endpoint>, IEndpointRepository
    {
        public EndpointRepository(Context context) : base(context)
        {

        }

        public async Task<Endpoint?> GetEndpointWithRolesByCodeAsync(string code)
        {
            using var connection = _context.CreateConnection();

            var sql = @"
                SELECT 
                    e.Id, e.ActionType, e.HttpType, e.Definition, e.Code, e.CreatedDate, e.UpdatedDate,
                    r.Id, r.Name, r.NormalizedName
                FROM Endpoints e
                LEFT JOIN AppRoleEndpoint are ON e.Id = are.EndpointsId
                LEFT JOIN AspNetRoles r ON are.RolesId = r.Id
                WHERE e.Code = @Code";

            var endpointDict = new Dictionary<Guid, Endpoint>();

            var result = await connection.QueryAsync<Endpoint, AppRole, Endpoint>(
                sql,
                (endpoint, role) =>
                {
                    if (!endpointDict.TryGetValue(endpoint.Id, out var endpointEntry))
                    {
                        endpointEntry = endpoint;
                        endpointEntry.Roles = new List<AppRole>();
                        endpointDict.Add(endpoint.Id, endpointEntry);
                    }

                    if (role != null)
                    {
                        endpointEntry.Roles.Add(role);
                    }

                    return endpointEntry;
                },
                new { Code = code },
                splitOn: "Id"
            );

            return endpointDict.Values.FirstOrDefault();
        }
    }
}
