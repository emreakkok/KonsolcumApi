using KonsolcumApi.Application.ViewModels.Baskets;
using KonsolcumApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Abstractions.Services
{
    public interface IBasketService
    {
        public Task<List<BasketItem>> GetBasketItemsAsync();

        public Task AddItemToBasketAsync(VM_Create_BasketItem basketItem);

        public Task UpdateQuantityAsync(VM_Update_BasketItem basketItem);
        
        public Task RemoveBasketItemAsync(string basketItemId);

        public Task<bool> HasItemsInBasketAsync();

        public Basket? GetUserActiveBasketAsync { get; }

    }
}
