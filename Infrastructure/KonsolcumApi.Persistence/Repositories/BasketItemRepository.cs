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
    public class BasketItemRepository : Repository<BasketItem>, IBasketItemRepository
    {
        public BasketItemRepository(Context context) : base(context)
        {
        }

        public async Task<BasketItem?> GetByBasketIdAndProductIdAsync(Guid basketId, Guid productId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM BasketItems WHERE BasketId = @BasketId AND ProductId = @ProductId";
            return await connection.QueryFirstOrDefaultAsync<BasketItem>(sql, new { BasketId = basketId, ProductId = productId });
        }

        public async Task<List<BasketItem>> GetBasketItemsWithProductAsync(Guid basketId)
        {
            using var connection = _context.CreateConnection();

            // SQL'e stok bilgisini de ekledik
            var sql = @"
                SELECT 
                    bi.Id, bi.BasketId, bi.ProductId, bi.Quantity, bi.CreatedDate, bi.UpdatedDate,
                    p.Id, p.Name, p.Price, p.StockQuantity, p.Description, p.CategoryId, p.CreatedDate, p.UpdatedDate
                FROM BasketItems bi
                INNER JOIN Products p ON bi.ProductId = p.Id
                WHERE bi.BasketId = @BasketId";

            var basketItemDict = new Dictionary<Guid, BasketItem>();

            var result = await connection.QueryAsync<BasketItem, Product, BasketItem>(
                sql,
                (bi, p) =>
                {
                    if (!basketItemDict.TryGetValue(bi.Id, out var basketItem))
                    {
                        basketItem = bi;
                        basketItem.Product = p;
                        basketItemDict.Add(basketItem.Id, basketItem);
                    }
                    return basketItem;
                },
                new { BasketId = basketId },
                splitOn: "Id" // İkinci Id Product tablosundan gelir
            );

            return result.Distinct().ToList();
        }

    }
}
