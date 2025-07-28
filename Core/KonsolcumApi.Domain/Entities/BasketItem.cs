using KonsolcumApi.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Domain.Entities
{
    public class BasketItem : BaseEntity
    {
        public Guid BasketId { get; set; }
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public virtual Basket? Basket { get; set; }
        public virtual Product? Product { get; set; }
    }
}
