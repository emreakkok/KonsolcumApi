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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(Context context) : base(context)
        {
        }

        public async Task<int> GetTotalProductCountAsync()
        {
            var sql = "SELECT COUNT(*) FROM Products";
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>(sql);
            }
        }

        public async Task<IEnumerable<Product>> GetAllPagedAsync(int page, int size)
        {
            var sql = @"
        SELECT
        p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.IsActive, p.IsFeatured,
        p.CategoryId, p.CreatedDate, p.UpdatedDate, p.ShowcaseImagePath,
        c.Id,
        c.Name
        FROM Products AS p
        LEFT JOIN Categories AS c ON p.CategoryId = c.Id
        ORDER BY p.Id
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;";

            using (var connection = _context.CreateConnection())
            {
                var offset = page * size;
                var products = await connection.QueryAsync<Product, Category, Product>(
                    sql,
                    (product, category) =>
                    {
                        product.Category = category;
                        return product;
                    },
                    new { Offset = offset, PageSize = size },
                    splitOn: "Id"
                );
                return products;
            }
        }


        public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
            SELECT p.*, c.Id, c.Name, c.Description, c.IsActive, c.CreatedDate, c.UpdatedDate
            FROM Products p
            LEFT JOIN Categories c ON p.CategoryId = c.Id
            WHERE p.CategoryId = @CategoryId";

            return await connection.QueryAsync<Product, Category, Product>(
                sql,
                (product, category) =>
                {
                    product.Category = category;
                    return product;
                },
                new { CategoryId = categoryId },
                splitOn: "Id"
            );
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
            SELECT p.*, c.Id, c.Name, c.Description, c.IsActive, c.CreatedDate, c.UpdatedDate
            FROM Products p
            LEFT JOIN Categories c ON p.CategoryId = c.Id
            WHERE p.Name LIKE @SearchTerm OR p.Description LIKE @SearchTerm";

            return await connection.QueryAsync<Product, Category, Product>(
                sql,
                (product, category) =>
                {
                    product.Category = category;
                    return product;
                },
                new { SearchTerm = $"%{searchTerm}%" },
                splitOn: "Id"
            );
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
            SELECT p.*, c.Id, c.Name, c.Description, c.IsActive, c.CreatedDate, c.UpdatedDate
            FROM Products p
            LEFT JOIN Categories c ON p.CategoryId = c.Id
            WHERE p.Price >= @MinPrice AND p.Price <= @MaxPrice";

            return await connection.QueryAsync<Product, Category, Product>(
                sql,
                (product, category) =>
                {
                    product.Category = category;
                    return product;
                },
                new { MinPrice = minPrice, MaxPrice = maxPrice },
                splitOn: "Id"
            );
        }

        public async Task<bool> IsInStockAsync(Guid id)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT StockQuantity FROM Products WHERE Id = @Id";
            var stock = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { Id = id });
            return stock > 0;
        }

        public async Task<int> GetActiveProductCountAsync()
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT COUNT(*) FROM Products WHERE IsActive = 1";
            return await connection.ExecuteScalarAsync<int>(sql);
        }

        public async Task<IEnumerable<Product>> GetActiveProductsPagedAsync(int page, int size)
        {
            using var connection = _context.CreateConnection();
            var offset = page * size;

            var sql = @"
        SELECT 
            p.Id, p.Name, p.Description, p.Price, p.StockQuantity, p.IsActive, p.IsFeatured,
            p.CategoryId, p.CreatedDate, p.UpdatedDate, p.ShowcaseImagePath,
            c.Id, c.Name, c.Description, c.IsActive, c.CreatedDate, c.UpdatedDate
        FROM Products p
        LEFT JOIN Categories c ON p.CategoryId = c.Id
        WHERE p.IsActive = 1
        ORDER BY p.CreatedDate DESC
        OFFSET @Offset ROWS FETCH NEXT @Size ROWS ONLY";

            return await connection.QueryAsync<Product, Category, Product>(
                sql,
                (product, category) =>
                {
                    product.Category = category;
                    return product;
                },
                new { Offset = offset, Size = size },
                splitOn: "Id"
            );
        }

        public async Task ReduceStockAsync(Guid productId, int quantity)
        {
            using var connection = _context.CreateConnection();

            try
            {
                // Önce ürünün mevcut stok durumunu kontrol et
                var checkSql = "SELECT Id, Name, StockQuantity FROM Products WHERE Id = @ProductId";
                var product = await connection.QuerySingleOrDefaultAsync(checkSql, new { ProductId = productId });

                Console.WriteLine($"=== STOK DÜŞÜRME DEBUG ===");
                Console.WriteLine($"ProductId: {productId}");
                Console.WriteLine($"Düşürülecek Miktar: {quantity}");

                if (product == null)
                {
                    Console.WriteLine($"HATA: Ürün bulunamadı!");
                    return;
                }

                Console.WriteLine($"Ürün Adı: {product.Name}");
                Console.WriteLine($"Mevcut Stok: {product.StockQuantity}");

                // Stok güncelleme
                var updateSql = @"
            UPDATE Products 
            SET StockQuantity = StockQuantity - @Quantity,
                UpdatedDate = GETDATE()
            WHERE Id = @ProductId";

                var affectedRows = await connection.ExecuteAsync(updateSql, new { ProductId = productId, Quantity = quantity });

                Console.WriteLine($"Güncellenen Satır Sayısı: {affectedRows}");

                // Güncellemeden sonra kontrol et
                var afterUpdate = await connection.QuerySingleOrDefaultAsync(checkSql, new { ProductId = productId });
                Console.WriteLine($"Güncellemeden Sonra Stok: {afterUpdate?.StockQuantity}");
                Console.WriteLine($"=== STOK DEBUG BİTTİ ===\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"STOK DÜŞÜRME HATASI: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

    }
}
