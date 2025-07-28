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
    public class CompletedOrderRepository : Repository<CompletedOrder>, ICompletedOrderRepository
    {
        public CompletedOrderRepository(Context context) : base(context)
        {
        }
    }
}
