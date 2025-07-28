using KonsolcumApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllPagedAsync(int page, int size);
        Task<int> GetTotalProductCountAsync();

        Task<int> GetActiveProductCountAsync();
        Task<IEnumerable<Product>> GetActiveProductsPagedAsync(int page, int size);


        Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<bool> IsInStockAsync(Guid id);

        public Task ReduceStockAsync(Guid productId, int quantity);
    }
}
