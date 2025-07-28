using KonsolcumApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<int> GetTotalCategoryCountAsync();
        Task<IEnumerable<Category>> GetAllPagedAsync(int page, int size);
        
        Task<Category?> GetByNameAsync(string name);
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();

        Task<int> GetActiveCategoryCountAsync();

        Task<IEnumerable<Category>> GetActiveCategoriesPagedAsync(int page, int size);


        Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId);
        Task<Category?> GetCategoryWithProductsAsync(Guid categoryId); //!


        Task<(IEnumerable<Product> products, int totalCount)> GetProductsByCategoryPagedAsync(Guid categoryId, int page = 0, int size = 12);

        Task<string> GetCategoryNameByIdAsync(Guid categoryId);
    }
}
