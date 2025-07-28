using Dapper;
using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Domain.Entities;
using KonsolcumApi.Persistence.Contexts.DapperContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Persistence.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(Context context) : base(context)
        {
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT COUNT(1) FROM Customers WHERE Email = @Email";
            var count = await connection.QuerySingleAsync<int>(sql, new { Email = email });
            return count > 0;
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Customers WHERE Email = @Email";
            return await connection.QueryFirstOrDefaultAsync<Customer>(sql, new { Email = email });
        }
    }
}
