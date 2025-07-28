using KonsolcumApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Repositories
{
    public interface IEndpointRepository : IRepository<Endpoint>
    {
        Task<Endpoint?> GetEndpointWithRolesByCodeAsync(string code);
    }
}
