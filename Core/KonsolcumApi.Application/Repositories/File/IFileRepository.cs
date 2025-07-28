using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Repositories
{
    public interface IFileRepository : IRepository<Domain.Entities.File.File>
    {
        Task<IEnumerable<Domain.Entities.File.File>> GetFilesByCategoryIdAsync(Guid categoryId);
        Task<IEnumerable<Domain.Entities.File.File>> GetFilesByProductIdAsync(Guid productId);

    }
}
