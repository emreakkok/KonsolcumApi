using KonsolcumApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Repositories
{
    public interface IBasketItemRepository : IRepository<BasketItem>
    {
        Task<BasketItem?> GetByBasketIdAndProductIdAsync(Guid basketId, Guid productId);

        Task<List<BasketItem>> GetBasketItemsWithProductAsync(Guid basketId);
    }
}
