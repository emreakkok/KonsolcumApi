using Dapper;
using KonsolcumApi.Application.DTOs.Order;
using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Domain.Entities;
using KonsolcumApi.Persistence.Contexts.DapperContext;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KonsolcumApi.Application.Repositories.IOrderRepository;

namespace KonsolcumApi.Persistence.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OrderRepository(Context context) : base(context)
        {

        }

        public async Task<List<BasketItemDto>> GetBasketItemsByBasketIdAsync(Guid basketId)
        {
            using var connection = _context.CreateConnection();

            try
            {
                var sql = @"
            SELECT 
                bi.ProductId,
                bi.Quantity
            FROM BasketItems bi
            WHERE bi.BasketId = @BasketId";

                var basketItems = await connection.QueryAsync<BasketItemDto>(sql, new { BasketId = basketId });

                Console.WriteLine($"GetBasketItemsByBasketIdAsync - BasketId: {basketId}, Items: {basketItems.Count()}");

                return basketItems.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetBasketItemsByBasketIdAsync HATA: {ex.Message}");
                return new List<BasketItemDto>();
            }
        }

        public async Task<List<ListOrder>> GetAllOrdersPagedAsync(int page, int size)
        {
            using var connection = _context.CreateConnection();
            var offset = page * size;

            var sql = @"
                    SELECT 
                CONVERT(varchar(36), o.Id) AS Id,
                o.OrderCode,
                o.ShippingAddress,
                o.CreatedDate,
                u.NameSurname AS Username,
                SUM(p.Price * bi.Quantity) AS TotalPrice,
                CASE 
                    WHEN co.OrderId IS NOT NULL THEN CAST(1 AS bit)
                    ELSE CAST(0 AS bit)
                END AS IsCompleted
            FROM Orders o
            INNER JOIN Baskets b ON o.BasketId = b.Id
            INNER JOIN AspNetUsers u ON b.UserId = u.Id
            INNER JOIN BasketItems bi ON bi.BasketId = b.Id
            INNER JOIN Products p ON p.Id = bi.ProductId
            LEFT JOIN CompletedOrders co ON co.OrderId = o.Id
            GROUP BY o.Id, o.OrderCode, o.ShippingAddress, o.CreatedDate, u.NameSurname, co.OrderId
            ORDER BY o.CreatedDate DESC
            OFFSET @Offset ROWS FETCH NEXT @Size ROWS ONLY;";

            var orders = await connection.QueryAsync<ListOrder>(sql, new { Offset = offset, Size = size });
            return orders.ToList();
        }

        public async Task<int> GetTotalOrderCountAsync()
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT COUNT(*) FROM Orders";
            return await connection.ExecuteScalarAsync<int>(sql);
        }

        public async Task<OrderDetailDto> GetOrderDetailByIdAsync(Guid orderId)
        {
            using var connection = _context.CreateConnection();

            try
            {
                // Sipariş var mı kontrolü
                var orderExistsSql = "SELECT COUNT(*) FROM Orders WHERE Id = @OrderId";
                var orderExists = await connection.ExecuteScalarAsync<int>(orderExistsSql, new { OrderId = orderId });

                if (orderExists == 0)
                    return null;

                // Sipariş genel bilgileri ve CompletedOrder bilgisi
                var orderSql = @"
                SELECT 
                    o.Id AS OrderId,
                    o.OrderCode,
                    o.ShippingAddress,
                    o.CreatedDate,
                    ISNULL(u.NameSurname, 'Bilinmiyor') AS Username,
                    CASE WHEN co.OrderId IS NOT NULL THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS IsCompleted
                FROM Orders o
                INNER JOIN Baskets b ON o.BasketId = b.Id
                LEFT JOIN AspNetUsers u ON b.UserId = u.Id
                LEFT JOIN CompletedOrders co ON co.OrderId = o.Id
                WHERE o.Id = @OrderId";

                var order = await connection.QuerySingleOrDefaultAsync<OrderDetailDto>(orderSql, new { OrderId = orderId });

                if (order == null)
                {
                    Console.WriteLine($"UYARI: OrderRepository.GetOrderDetailByIdAsync - Sipariş bulundu (COUNT > 0) ancak detayları çekilemedi. OrderId: {orderId}");
                    return null;
                }

                // Sipariş ürünleri sorgusu
                var productSql = @"
                SELECT 
                    bi.Id AS BasketItemId,
                    ISNULL(p.Name, 'Ürün Adı Bulunamadı') AS ProductName,
                    bi.Quantity,
                    ISNULL(p.Price, 0) AS UnitPrice,
                    p.ShowcaseImagePath,
                    ISNULL(p.Description, '') AS Description
                FROM Orders o
                INNER JOIN Baskets b ON o.BasketId = b.Id
                INNER JOIN BasketItems bi ON bi.BasketId = b.Id
                LEFT JOIN Products p ON p.Id = bi.ProductId
                WHERE o.Id = @OrderId
                ORDER BY p.Name";

                var products = (await connection.QueryAsync<OrderDetailProductDto>(productSql, new { OrderId = orderId })).ToList();

                order.Products = products ?? new List<OrderDetailProductDto>();

                order.TotalPrice = order.Products.Sum(p => p.UnitPrice * p.Quantity);

                return order;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- HATA: OrderRepository.GetOrderDetailByIdAsync ---");
                Console.WriteLine($"Sipariş ID: {orderId}");
                Console.WriteLine($"Mesaj: {ex.Message}");
                Console.WriteLine($"Inner Exception Mesajı: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: \n{ex.StackTrace}\n");
                throw;
            }
        }

        public async Task<(bool, CompletedOrderDto?)> CompleteOrderAsync(string id)
        {
            using var connection = _context.CreateConnection();

            try
            {
                var orderId = Guid.Parse(id);

                // Önce siparişin zaten tamamlanmış olup olmadığını kontrol et
                var checkCompletedSql = "SELECT COUNT(*) FROM CompletedOrders WHERE OrderId = @OrderId";
                var isAlreadyCompleted = await connection.ExecuteScalarAsync<int>(
                    checkCompletedSql, new { OrderId = orderId });

                if (isAlreadyCompleted > 0)
                {
                    // Zaten tamamlanmış, mevcut bilgileri döndür
                    var existingOrderSql = @"
                        SELECT 
                            o.OrderCode,
                            o.CreatedDate AS OrderDate,
                            ISNULL(u.NameSurname, 'Müşteri') AS Username,
                            ISNULL(u.Email, '') AS Email
                        FROM Orders o
                        INNER JOIN Baskets b ON o.BasketId = b.Id
                        LEFT JOIN AspNetUsers u ON b.UserId = u.Id
                        WHERE o.Id = @OrderId";

                    var existingOrder = await connection.QuerySingleOrDefaultAsync<CompletedOrderDto>(
                        existingOrderSql, new { OrderId = orderId });

                    return (true, existingOrder);
                }

                // Siparişin varlığını ve kullanıcı bilgilerini kontrol et (Email dahil)
                var sqlOrder = @"
                    SELECT 
                        o.OrderCode,
                        o.CreatedDate AS OrderDate,
                        ISNULL(u.NameSurname, 'Müşteri') AS Username,
                        ISNULL(u.Email, '') AS Email
                    FROM Orders o
                    INNER JOIN Baskets b ON o.BasketId = b.Id
                    LEFT JOIN AspNetUsers u ON b.UserId = u.Id
                    WHERE o.Id = @OrderId";

                var orderData = await connection.QuerySingleOrDefaultAsync<CompletedOrderDto>(
                    sqlOrder, new { OrderId = orderId });

                if (orderData == null)
                {
                    Console.WriteLine($"UYARI: CompleteOrderAsync - Sipariş bulunamadı. OrderId: {orderId}");
                    return (false, null);
                }

                // CompletedOrders tablosuna yeni kayıt ekle
                var sqlInsert = @"
                    INSERT INTO CompletedOrders (Id, OrderId, CreatedDate)
                    VALUES (@Id, @OrderId, GETDATE())";

                var insertResult = await connection.ExecuteAsync(sqlInsert, new
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId
                });

                if (insertResult > 0)
                {
                    Console.WriteLine($"BİLGİ: Sipariş başarıyla tamamlandı. OrderCode: {orderData.OrderCode}");
                    return (true, orderData);
                }
                else
                {
                    Console.WriteLine($"HATA: CompleteOrderAsync - Insert işlemi başarısız. OrderId: {orderId}");
                    return (false, null);
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"HATA: CompleteOrderAsync - Geçersiz GUID formatı: {id}");
                Console.WriteLine($"Mesaj: {ex.Message}");
                throw new ArgumentException("Geçersiz sipariş ID formatı", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- HATA: OrderRepository.CompleteOrderAsync ---");
                Console.WriteLine($"Sipariş ID: {id}");
                Console.WriteLine($"Mesaj: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack Trace: \n{ex.StackTrace}\n");
                throw;
            }


        }

        public async Task<List<ListOrder>> GetMyOrdersPagedAsync(string username, int page, int size)
        {
            using var connection = _context.CreateConnection();
            var offset = page * size;

            const string query = @"
            SELECT 
                CONVERT(varchar(36), o.Id) AS Id,
                o.OrderCode,
                u.UserName AS Username,
                o.ShippingAddress,
                ISNULL(SUM(p.Price * bo.Quantity), 0) AS TotalPrice,
                o.CreatedDate,
                CASE WHEN co.Id IS NOT NULL THEN 1 ELSE 0 END AS IsCompleted
            FROM Orders o
            INNER JOIN Baskets b ON o.BasketId = b.Id
            INNER JOIN AspNetUsers u ON b.UserId = u.Id
            INNER JOIN BasketItems bo ON bo.BasketId = b.Id
            INNER JOIN Products p ON bo.ProductId = p.Id
            LEFT JOIN CompletedOrders co ON co.OrderId = o.Id
            WHERE u.UserName = @UserName
            GROUP BY 
                o.Id, o.OrderCode, u.UserName, o.ShippingAddress, o.CreatedDate, co.Id
            ORDER BY o.CreatedDate DESC
            OFFSET @Offset ROWS FETCH NEXT @Size ROWS ONLY;
        ";

            var result = await connection.QueryAsync<ListOrder>(query, new
            {
                UserName = username,
                Offset = offset,
                Size = size
            });

            return result.ToList();
        }

        public async Task<int> GetMyOrderCountAsync(string username)
        {
            using var connection = _context.CreateConnection();

            const string sql = @"
            SELECT COUNT(DISTINCT o.Id)
            FROM Orders o
            INNER JOIN Baskets b ON o.BasketId = b.Id
            INNER JOIN AspNetUsers u ON b.UserId = u.Id
            WHERE u.UserName = @UserName";

            var count = await connection.ExecuteScalarAsync<int>(sql, new { UserName = username });
            return count;
        }
    }

}


