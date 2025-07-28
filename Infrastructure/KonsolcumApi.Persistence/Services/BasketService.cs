using Dapper;
using KonsolcumApi.Application.Abstractions.Services;
using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Application.ViewModels.Baskets;
using KonsolcumApi.Domain.Entities;
using KonsolcumApi.Domain.Entities.Identity;
using KonsolcumApi.Persistence.Contexts.DapperContext;
using KonsolcumApi.Persistence.Contexts.EfContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Persistence.Services
{
    public class BasketService : IBasketService
    {
        readonly IHttpContextAccessor _httpContextAccessor;
        readonly UserManager<AppUser> _userManager;
        readonly IOrderRepository _orderRepository;
        readonly IBasketRepository _basketRepository;
        readonly Context _context;
        readonly IBasketItemRepository _basketItemRepository;
        readonly IProductRepository _productRepository;
        public BasketService(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IOrderRepository orderRepository, IBasketRepository basketRepository, Context context, IBasketItemRepository basketItemRepository, IProductRepository productRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _orderRepository = orderRepository;
            _basketRepository = basketRepository;
            _context = context;
            _basketItemRepository = basketItemRepository;
            _productRepository = productRepository;
        }
        public async Task<Basket?> ContextUser()
        {
            var username = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return null;

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return null;

            using var connection = _context.CreateConnection();

            // Kullanıcının tamamlanmamış sepeti var mı kontrol et
            var basket = await connection.QueryFirstOrDefaultAsync<Basket>(
                @"SELECT b.* FROM Baskets b
              LEFT JOIN Orders o ON b.Id = o.BasketId
              WHERE b.UserId = @UserId AND o.Id IS NULL",
                new { UserId = user.Id });

            if (basket != null)
                return basket;

            // Yoksa yeni sepet oluştur
            var newBasket = new Basket
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CreatedDate = DateTime.UtcNow
            };

            await connection.ExecuteAsync(
                "INSERT INTO Baskets (Id, UserId, CreatedDate) VALUES (@Id, @UserId, @CreatedDate)",
                newBasket);

            return newBasket;
        }


        public async Task AddItemToBasketAsync(VM_Create_BasketItem basketItem)
        {
            var basket = await ContextUser();

            if (basket == null)
                throw new UnauthorizedAccessException("Kullanıcı giriş yapmamış.");

            // Ürün stok kontrolü
            var product = await _productRepository.GetByIdAsync(Guid.Parse(basketItem.ProductId));
            if (product == null)
                throw new ArgumentException("Ürün bulunamadı.");

            if (product.StockQuantity <= 0) // Stock property'si kullanıldığını varsayıyorum
                throw new InvalidOperationException("Ürün stokta bulunmuyor.");

            using var connection = _context.CreateConnection();

            // Sepette aynı ürün varsa miktar artır
            var existingItem = await connection.QueryFirstOrDefaultAsync<BasketItem>(
                @"SELECT * FROM BasketItems 
              WHERE BasketId = @BasketId AND ProductId = @ProductId",
                new { BasketId = basket.Id, ProductId = Guid.Parse(basketItem.ProductId) });

            if (existingItem != null)
            {
                var newQuantity = existingItem.Quantity + basketItem.Quantity;

                // Stok kontrolü
                if (newQuantity > product.StockQuantity)
                    throw new InvalidOperationException($"Stok yetersiz! Maksimum {product.StockQuantity} adet ekleyebilirsiniz. Sepetinizde zaten {existingItem.Quantity} adet var.");

                existingItem.Quantity = newQuantity;

                await connection.ExecuteAsync(
                    @"UPDATE BasketItems SET Quantity = @Quantity, UpdatedDate = @UpdatedDate
                  WHERE Id = @Id",
                    new { existingItem.Quantity, UpdatedDate = DateTime.UtcNow, existingItem.Id });
            }
            else
            {
                // Yeni ürün için stok kontrolü
                if (basketItem.Quantity > product.StockQuantity)
                    throw new InvalidOperationException($"Stok yetersiz! Maksimum {product.StockQuantity} adet ekleyebilirsiniz.");

                var newItem = new BasketItem
                {
                    Id = Guid.NewGuid(),
                    BasketId = basket.Id,
                    ProductId = Guid.Parse(basketItem.ProductId),
                    Quantity = basketItem.Quantity,
                    CreatedDate = DateTime.UtcNow
                };

                await connection.ExecuteAsync(
                    @"INSERT INTO BasketItems (Id, BasketId, ProductId, Quantity, CreatedDate)
                  VALUES (@Id, @BasketId, @ProductId, @Quantity, @CreatedDate)",
                    newItem);
            }
        }


        public async Task<List<BasketItem>> GetBasketItemsAsync()
        {
            Basket? basket = await ContextUser();
            if (basket == null)
                return new();

            return await _basketItemRepository.GetBasketItemsWithProductAsync(basket.Id);
        }

        public async Task RemoveBasketItemAsync(string basketItemId)
        {
            if (!Guid.TryParse(basketItemId, out var id))
                return;

            await _basketItemRepository.DeleteAsync(id);
        }

        public async Task UpdateQuantityAsync(VM_Update_BasketItem basketItem)
        {
            // Guid validation ekle
            if (!Guid.TryParse(basketItem.BasketItemId, out var basketItemGuid))
                throw new ArgumentException("Geçersiz sepet öğesi ID'si");

            var existing = await _basketItemRepository.GetByIdAsync(basketItemGuid);
            if (existing == null)
                throw new ArgumentException("Sepet öğesi bulunamadı");

            // Quantity validation
            if (basketItem.Quantity <= 0)
                throw new ArgumentException("Miktar 0'dan büyük olmalıdır");

            // Ürün ve stok kontrolü
            var product = await _productRepository.GetByIdAsync(existing.ProductId);
            if (product == null)
                throw new ArgumentException("Ürün bulunamadı");

            if (basketItem.Quantity > product.StockQuantity)
                throw new InvalidOperationException($"Stok yetersiz! Maksimum {product.StockQuantity} adet seçebilirsiniz.");

            if (product.StockQuantity <= 0)
                throw new InvalidOperationException("Ürün stokta bulunmuyor.");

            existing.Quantity = basketItem.Quantity;
            await _basketItemRepository.UpdateAsync(existing);
        }


        public async Task<bool> HasItemsInBasketAsync()
        {
            var basket = await ContextUser();
            if (basket == null)
                return false;

            using var connection = _context.CreateConnection();

            var itemCount = await connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(*) FROM BasketItems WHERE BasketId = @BasketId",
                new { BasketId = basket.Id });

            return itemCount > 0;
        }

        public Basket? GetUserActiveBasketAsync
        {
            get{
                Basket? basket = ContextUser().Result;
                return basket;
            }
        }


    }
}
