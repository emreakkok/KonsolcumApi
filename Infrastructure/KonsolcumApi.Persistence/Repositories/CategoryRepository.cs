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
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(Context context) : base(context)
        {
        }
        public async Task<int> GetTotalCategoryCountAsync()
        {
            var sql = "SELECT COUNT(*) FROM Categories";
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>(sql);
            }
        }

        public async Task<IEnumerable<Category>> GetAllPagedAsync(int page, int size)
        {
            var sql = @"
            SELECT
                Id,
                Name,
                Description,
                IsActive,
                ShowcaseImagePath,
                CreatedDate,
                UpdatedDate
            FROM Categories
            ORDER BY Id
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;";

            using (var connection = _context.CreateConnection())
            {
                var offset = page * size;
                return await connection.QueryAsync<Category>(sql, new { Offset = offset, PageSize = size });
            }
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Categories WHERE Name = @Name";
            return await connection.QueryFirstOrDefaultAsync<Category>(sql, new { Name = name });
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM Categories WHERE IsActive = 1";
            return await connection.QueryAsync<Category>(sql);
        }

        public async Task<int> GetActiveCategoryCountAsync()
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT COUNT(*) FROM Categories WHERE IsActive = 1";
            return await connection.ExecuteScalarAsync<int>(sql);
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesPagedAsync(int page, int size)
        {
            using var connection = _context.CreateConnection();
            var offset = page * size;
            var sql = "SELECT * FROM Categories WHERE IsActive = 1 ORDER BY CreatedDate DESC OFFSET @Offset ROWS FETCH NEXT @Size ROWS ONLY";
            return await connection.QueryAsync<Category>(sql, new { Offset = offset, Size = size });
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
            SELECT p.*, c.Name as CategoryName 
            FROM Products p 
            INNER JOIN Categories c ON p.CategoryId = c.Id 
            WHERE p.CategoryId = @CategoryId";

            return await connection.QueryAsync<Product, string, Product>(
                sql,
                (product, categoryName) =>
                {
                    product.Category = new Category { Name = categoryName };
                    return product;
                },
                new { CategoryId = categoryId },
                splitOn: "CategoryName"
            );
        }
        //!
        public async Task<Category?> GetCategoryWithProductsAsync(Guid categoryId)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
            SELECT c.*, p.*
            FROM Categories c
            LEFT JOIN Products p ON c.Id = p.CategoryId
            WHERE c.Id = @CategoryId";

            var categoryDict = new Dictionary<Guid, Category>(); // zaten işlenmiş kategorileri Id'lerine göre saklar.
                                                                 // Bu, aynı kategoriye ait birden fazla ürün olduğunda, kategorinin tekrar tekrar oluşturulmamasını sağlar.

            await connection.QueryAsync<Category, Product, Category>(
                sql,
                (category, product) =>
                {
                    if (!categoryDict.TryGetValue(category.Id, out var categoryEntry))
                    {
                        categoryEntry = category;
                        categoryEntry.Products = new List<Product>();
                        categoryDict.Add(category.Id, categoryEntry);
                    }

                    if (product != null)
                    {
                        categoryEntry.Products.Add(product);
                    }

                    return categoryEntry;
                },
                new { CategoryId = categoryId },
                splitOn: "Id"
            );

            return categoryDict.Values.FirstOrDefault();
        }




        public async Task<(IEnumerable<Product> products, int totalCount)> GetProductsByCategoryPagedAsync(Guid categoryId, int page = 0, int size = 12)
        {
            using var connection = _context.CreateConnection();

            // Önce toplam ürün sayısını al
            var countSql = @"
        SELECT COUNT(*) 
        FROM Products p 
        INNER JOIN Categories c ON p.CategoryId = c.Id 
        WHERE p.CategoryId = @CategoryId AND p.IsActive = 1";

            var totalCount = await connection.QuerySingleAsync<int>(countSql, new { CategoryId = categoryId });

            // Sonra sayfalanmış ürünleri al
            var offset = page * size;
            var dataSql = @"
        SELECT 
            p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.IsActive, p.IsFeatured,
            p.CategoryId, p.CreatedDate, p.UpdatedDate, p.ShowcaseImagePath,
            c.Id, c.Name, c.Description, c.IsActive, c.CreatedDate, c.UpdatedDate
        FROM Products p 
        INNER JOIN Categories c ON p.CategoryId = c.Id 
        WHERE p.CategoryId = @CategoryId AND p.IsActive = 1
        ORDER BY p.CreatedDate DESC
        OFFSET @Offset ROWS 
        FETCH NEXT @Size ROWS ONLY";

            var products = await connection.QueryAsync<Product, Category, Product>(
                dataSql,
                (product, category) =>
                {
                    product.Category = category;
                    return product;
                },
                new
                {
                    CategoryId = categoryId,
                    Offset = offset,
                    Size = size
                },
                splitOn: "Id"
            );

            return (products, totalCount);
        }

        public async Task<string> GetCategoryNameByIdAsync(Guid categoryId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT Name FROM Categories WHERE Id = @CategoryId";
            return await connection.QueryFirstOrDefaultAsync<string>(sql, new { CategoryId = categoryId }) ?? "Bilinmeyen Kategori";
        }

    }
}
